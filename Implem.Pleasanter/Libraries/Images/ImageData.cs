using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources.BinaryStorages;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Images
{
    public class ImageData : IDisposable
    {
        private static readonly HashSet<SKEncodedImageFormat> SupportedFormats = new()
        {
            SKEncodedImageFormat.Png,
            SKEncodedImageFormat.Jpeg,
            SKEncodedImageFormat.Gif,
            SKEncodedImageFormat.Bmp,
            SKEncodedImageFormat.Webp
        };

        private const int MaxAnimationFrames = 1000;

        private const long DefaultAnimationSizeLimitMegabytes = 256;

        private const long MinApngOutputLimitMegabytes = 2;

        private const long ApngOutputLimitRatio = 4;

        private readonly List<SKBitmap> frames = new();
        private readonly List<int> frameDurations = new();
        private int repetitionCount = -1;
        private byte[] pendingAnimationSource;
        private long sourceLength;
        public long ReferenceId;
        public Types Type;
        private bool disposed;

        public SKBitmap Data => frames.Count > 0 ? frames[0] : null;

        public int FrameCount
        {
            get
            {
                ThrowIfDisposed();
                LoadAnimationFrames();
                return frames.Count;
            }
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(disposed, this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                frames.ForEach(frame => frame.Dispose());
                frames.Clear();
                frameDurations.Clear();
                pendingAnimationSource = null;
            }
            disposed = true;
        }

        public enum Types : int
        {
            SiteImage = 1,
            TenantImage = 2
        }

        public enum SizeTypes : int
        {
            Regular = 1,
            Thumbnail = 2,
            Icon = 3,
            Logo = 4,
        }

        public ImageData(byte[] data, long referenceId, Types type)
        {
            try
            {
                using var codec = CreateCodec(data: data)
                    ?? throw new InvalidDataException("Unsupported image format.");
                var info = codec.Info
                    .WithColorType(SKColorType.Rgba8888)
                    .WithAlphaType(SKAlphaType.Premul);
                frames.Add(SKBitmap.Decode(codec, info)
                    ?? throw new InvalidDataException("Failed to decode image."));
                frameDurations.Add(0);
                frames[0] = Orient(
                    source: frames[0],
                    origin: codec.EncodedOrigin);
                sourceLength = data.Length;
                if (codec.FrameCount > 1 && codec.FrameCount <= MaxAnimationFrames)
                {
                    pendingAnimationSource = data;
                }
            }
            catch
            {
                Dispose();
                throw;
            }
            ReferenceId = referenceId;
            Type = type;
        }

        private void LoadAnimationFrames()
        {
            var source = pendingAnimationSource;
            if (source == null)
            {
                return;
            }
            pendingAnimationSource = null;
            using var codec = CreateCodec(data: source);
            if (codec == null
                || codec.FrameCount < 2
                || codec.FrameCount > MaxAnimationFrames)
            {
                return;
            }
            var info = codec.Info
                .WithColorType(SKColorType.Rgba8888)
                .WithAlphaType(SKAlphaType.Premul);
            var frameInfos = codec.FrameInfo;
            if (frameInfos == null
                || frameInfos.Length != codec.FrameCount
                || !WithinAnimationLimit(
                    width: info.Width,
                    height: info.Height,
                    frameCount: frameInfos.Length))
            {
                return;
            }
            var decoded = new List<SKBitmap>();
            var succeeded = false;
            try
            {
                decoded.Add(SKBitmap.Decode(codec, info)
                    ?? throw new InvalidDataException("Failed to decode frame."));
                for (var i = 1; i < frameInfos.Length; i++)
                {
                    var required = frameInfos[i].RequiredFrame;
                    SKBitmap bitmap;
                    if (required == -1)
                    {
                        bitmap = new SKBitmap(info);
                        bitmap.Erase(SKColors.Transparent);
                    }
                    else if (required >= 0 && required < i)
                    {
                        bitmap = decoded[required].Copy()
                            ?? throw new InvalidDataException("Failed to copy frame.");
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid required frame.");
                    }
                    decoded.Add(bitmap);
                    if (bitmap.GetPixels() == IntPtr.Zero
                        || bitmap.Width != info.Width
                        || bitmap.Height != info.Height
                        || bitmap.ColorType != info.ColorType
                        || bitmap.AlphaType != info.AlphaType
                        || bitmap.RowBytes < info.RowBytes)
                    {
                        throw new InvalidDataException("Invalid frame buffer.");
                    }
                    var result = codec.GetPixels(
                        info,
                        bitmap.GetPixels(),
                        bitmap.RowBytes,
                        new SKCodecOptions(i, required));
                    if (result != SKCodecResult.Success)
                    {
                        throw new InvalidDataException("Failed to decode frame.");
                    }
                }
                for (var i = 1; i < decoded.Count; i++)
                {
                    decoded[i] = Orient(
                        source: decoded[i],
                        origin: codec.EncodedOrigin);
                }
                succeeded = true;
            }
            catch (Exception e) when (e is not OutOfMemoryException)
            {
            }
            finally
            {
                if (decoded.Count > 0)
                {
                    decoded[0].Dispose();
                }
                if (!succeeded)
                {
                    decoded.Skip(1).ToList().ForEach(bitmap => bitmap.Dispose());
                }
            }
            if (!succeeded)
            {
                return;
            }
            frames.AddRange(decoded.Skip(1));
            frameDurations.Clear();
            frameDurations.AddRange(frameInfos.Select(frameInfo => frameInfo.Duration));
            repetitionCount = codec.RepetitionCount;
        }

        private static bool WithinAnimationLimit(long width, long height, int frameCount)
        {
            if (frameCount > MaxAnimationFrames)
            {
                return false;
            }
            try
            {
                var limitBytes = checked(LimitMegabytes(
                    value: Parameters.BinaryStorage?.ImageDecodedSizeLimit,
                    defaultValue: DefaultAnimationSizeLimitMegabytes) * 1024 * 1024);
                return checked(width * height * 4 * frameCount) <= limitBytes;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        private static long LimitMegabytes(decimal? value, long defaultValue)
        {
            return value > 0
                ? (long)Math.Clamp(Math.Ceiling(value.Value), 1m, 1024m * 1024m)
                : defaultValue;
        }

        private void DropAnimation()
        {
            for (var i = frames.Count - 1; i > 0; i--)
            {
                frames[i].Dispose();
                frames.RemoveAt(i);
            }
            frameDurations.RemoveRange(1, frameDurations.Count - 1);
        }

        private static SKBitmap Orient(SKBitmap source, SKEncodedOrigin origin)
        {
            if (origin == SKEncodedOrigin.TopLeft)
            {
                return source;
            }
            float w = source.Width;
            float h = source.Height;
            var (matrix, swap) = origin switch
            {
                SKEncodedOrigin.TopRight => (new SKMatrix(-1, 0, w, 0, 1, 0, 0, 0, 1), false),
                SKEncodedOrigin.BottomRight => (new SKMatrix(-1, 0, w, 0, -1, h, 0, 0, 1), false),
                SKEncodedOrigin.BottomLeft => (new SKMatrix(1, 0, 0, 0, -1, h, 0, 0, 1), false),
                SKEncodedOrigin.LeftTop => (new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1), true),
                SKEncodedOrigin.RightTop => (new SKMatrix(0, -1, h, 1, 0, 0, 0, 0, 1), true),
                SKEncodedOrigin.RightBottom => (new SKMatrix(0, -1, h, -1, 0, w, 0, 0, 1), true),
                SKEncodedOrigin.LeftBottom => (new SKMatrix(0, 1, 0, -1, 0, w, 0, 0, 1), true),
                _ => (SKMatrix.Identity, false)
            };
            var oriented = new SKBitmap(source.Info.WithSize(
                width: swap ? source.Height : source.Width,
                height: swap ? source.Width : source.Height));
            try
            {
                using var canvas = new SKCanvas(oriented);
                canvas.Clear(SKColors.Transparent);
                canvas.SetMatrix(matrix);
                canvas.DrawBitmap(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest));
            }
            catch
            {
                oriented.Dispose();
                throw;
            }
            source.Dispose();
            return oriented;
        }

        public static SKCodec CreateCodec(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            var codec = SKCodec.Create(new SKMemoryStream(data));
            if (codec == null)
            {
                return null;
            }
            if (!SupportedFormats.Contains(codec.EncodedFormat))
            {
                codec.Dispose();
                return null;
            }
            return codec;
        }

        public ImageData(long referenceId, Types type)
        {
            ReferenceId = referenceId;
            Type = type;
        }

        public byte[] Read(SizeTypes sizeType)
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (provider != null)
            {
                return provider.Download(ObjectName(ReferenceId, Type, sizeType));
            }
            return Files.Bytes(Path(ReferenceId, Type, sizeType));
        }

        public bool Exists(SizeTypes sizeType)
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (provider != null)
            {
                try
                {
                    return provider.Exists(ObjectName(ReferenceId, Type, sizeType));
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return File.Exists(Path(ReferenceId, Type, sizeType));
        }

        public DateTime LastWriteTime(SizeTypes sizeType)
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (provider != null)
            {
                try
                {
                    return provider.LastWriteTime(ObjectName(ReferenceId, Type, sizeType));
                }
                catch (IOException)
                {
                    return DateTime.FromOADate(0);
                }
            }
            return DateTime.FromOADate(0);
        }

        public void WriteToLocal()
        {
            if (Type == Types.SiteImage)
            {
                WriteToLocal(
                    bytes: ReSizeBytes(sizeType: SizeTypes.Regular),
                    referenceId: ReferenceId,
                    type: Type,
                    sizeType: SizeTypes.Regular);
                WriteToLocal(
                    bytes: ReSizeBytes(sizeType: SizeTypes.Thumbnail),
                    referenceId: ReferenceId,
                    type: Type,
                    sizeType: SizeTypes.Thumbnail);
                WriteToLocal(
                    bytes: ReSizeBytes(sizeType: SizeTypes.Icon),
                    referenceId: ReferenceId,
                    type: Type,
                    sizeType: SizeTypes.Icon);
            }
            else
            {
                WriteToLocal(
                    bytes: ReSizeBytes(sizeType: SizeTypes.Logo),
                    referenceId: ReferenceId,
                    type: Type,
                    sizeType: SizeTypes.Logo);
            }
        }

        private void WriteToLocal(byte[] bytes, long referenceId, Types type, SizeTypes sizeType)
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (provider != null)
            {
                using var memory = new MemoryStream(bytes);
                provider.Upload(
                    objectName: ObjectName(referenceId, type, sizeType),
                    stream: memory,
                    contentType: "image/png");
            }
            else
            {
                bytes.Write(Path(referenceId, type, sizeType));
            }
        }

        public void DeleteLocalFiles()
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (Type == Types.SiteImage)
            {
                if (provider != null)
                {
                    provider.Delete(ObjectName(ReferenceId, Type, SizeTypes.Regular));
                    provider.Delete(ObjectName(ReferenceId, Type, SizeTypes.Thumbnail));
                    provider.Delete(ObjectName(ReferenceId, Type, SizeTypes.Icon));
                }
                else
                {
                    Files.DeleteFile(Path(ReferenceId, Type, SizeTypes.Regular));
                    Files.DeleteFile(Path(ReferenceId, Type, SizeTypes.Thumbnail));
                    Files.DeleteFile(Path(ReferenceId, Type, SizeTypes.Icon));
                }
            }
            else
            {
                if (provider != null)
                {
                    provider.Delete(ObjectName(ReferenceId, Type, SizeTypes.Logo));
                }
                else
                {
                    Files.DeleteFile(Path(ReferenceId, Type, SizeTypes.Logo));
                }
            }
        }

        private string Path(long referenceId, Types type, SizeTypes sizeType)
        {
            return System.IO.Path.Combine(
                Directories.BinaryStorage(),
                type.ToString(),
                "{0}_{1}.png".Params(referenceId, sizeType));
        }

        public byte[] ReSizeBytes(SizeTypes sizeType)
        {
            ThrowIfDisposed();
            ReSize(sizeType: sizeType);
            return EncodePng();
        }

        private void ReSize(SizeTypes sizeType)
        {
            var size = (double)Size(sizeType);
            var rate = (Data.Width > Data.Height) || (sizeType == SizeTypes.Logo)
                ? size / Data.Height
                : size / Data.Width;
            if (rate != 1)
            {
                var width = (Data.Width * rate).ToInt();
                var height = (Data.Height * rate).ToInt();
                ResizeFrames(
                    width: width,
                    height: height);
            }
        }

        public byte[] ReSizeBytes(decimal? size)
        {
            ThrowIfDisposed();
            ReSize(size: size);
            return EncodePng();
        }

        private void ReSize(decimal? size)
        {
            if (size != null && size > 0)
            {
                var width = 0;
                var height = 0;
                var rate = Data.Width.ToDouble() / Data.Height.ToDouble();
                if (rate > 1)
                {
                    width = size.ToInt();
                    height = (Data.Height * size / Data.Width).ToInt();
                }
                else if (rate < 1)
                {
                    width = (Data.Width * size / Data.Height).ToInt();
                    height = size.ToInt();
                }
                else
                {
                    width = size.ToInt();
                    height = size.ToInt();
                }
                ResizeFrames(
                    width: width,
                    height: height);
            }
        }

        private void ResizeFrames(int width, int height)
        {
            ThrowIfDisposed();
            LoadAnimationFrames();
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            if (frames.Count > 1
                && !WithinAnimationLimit(
                    width: width,
                    height: height,
                    frameCount: frames.Count))
            {
                DropAnimation();
            }
            try
            {
                for (var i = 0; i < frames.Count; i++)
                {
                    frames[i] = ResizeBitmap(
                        source: frames[i],
                        width: width,
                        height: height);
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private static SKBitmap ResizeBitmap(SKBitmap source, int width, int height)
        {
            var current = source;
            try
            {
                while (current.Width / 2 >= width && current.Height / 2 >= height)
                {
                    current = Replace(
                        oldBitmap: current,
                        newBitmap: Resize(
                            source: current,
                            width: current.Width / 2,
                            height: current.Height / 2,
                            sampling: new SKSamplingOptions(SKFilterMode.Linear)));
                }
                if (current.Width != width || current.Height != height)
                {
                    current = Replace(
                        oldBitmap: current,
                        newBitmap: Resize(
                            source: current,
                            width: width,
                            height: height,
                            sampling: new SKSamplingOptions(SKCubicResampler.Mitchell)));
                }
                return current;
            }
            catch
            {
                current.Dispose();
                throw;
            }
        }

        private static SKBitmap Resize(
            SKBitmap source,
            int width,
            int height,
            SKSamplingOptions sampling)
        {
            return source.Resize(
                source.Info.WithSize(
                    width: width,
                    height: height),
                sampling)
                    ?? throw new InvalidDataException("Failed to resize image.");
        }

        private static SKBitmap Replace(SKBitmap oldBitmap, SKBitmap newBitmap)
        {
            oldBitmap.Dispose();
            return newBitmap;
        }

        private byte[] EncodePng()
        {
            ThrowIfDisposed();
            LoadAnimationFrames();
            if (frames.Count > 1)
            {
                try
                {
                    return ApngEncoder.Encode(
                        frames: frames,
                        durations: frameDurations,
                        repetitionCount: repetitionCount,
                        maxBytes: ApngOutputLimitBytes());
                }
                catch (InvalidDataException)
                {
                    DropAnimation();
                }
            }
            using var data = Data.Encode(SKEncodedImageFormat.Png, 100)
                ?? throw new InvalidDataException("Failed to encode image.");
            return data.ToArray();
        }

        private long ApngOutputLimitBytes()
        {
            try
            {
                return Math.Max(
                    checked(sourceLength * ApngOutputLimitRatio),
                    MinApngOutputLimitMegabytes * 1024 * 1024);
            }
            catch (OverflowException)
            {
                return int.MaxValue / 2;
            }
        }

        private int Size(SizeTypes sizeType)
        {
            switch (sizeType)
            {
                case SizeTypes.Regular: return Parameters.General.ImageSizeRegular;
                case SizeTypes.Thumbnail: return Parameters.General.ImageSizeThumbnail;
                case SizeTypes.Icon: return Parameters.General.ImageSizeIcon;
                case SizeTypes.Logo: return Parameters.General.ImageSizeLogo;
                default: return Parameters.General.ImageSizeRegular;
            }
        }
        private string ProviderName()
        {
            return Type == Types.SiteImage
                ? Parameters.BinaryStorage.GetSiteImageProvider()
                : Parameters.BinaryStorage.Provider;
        }

        private string ObjectName(long referenceId, Types type, SizeTypes sizeType)
        {
            return $"{type}/{referenceId}_{sizeType}.png";
        }
    }
}
