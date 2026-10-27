using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources.BinaryStorages;
using SkiaSharp;
using System;
using System.IO;
namespace Implem.Pleasanter.Libraries.Images
{
    public class ImageData : IDisposable
    {
        public SKBitmap Data;
        public long ReferenceId;
        public Types Type;
        private bool disposed;

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
                Data?.Dispose();
                Data = null;
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
            using var codec = SKCodec.Create(new MemoryStream(data))
                ?? throw new FormatException("Unsupported or invalid image.");
            Data = SKBitmap.Decode(codec)
                ?? throw new FormatException("Unsupported or invalid image.");
            ReferenceId = referenceId;
            Type = type;
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
                WriteToLocal(ReSize(SizeTypes.Regular), ReferenceId, Type, SizeTypes.Regular);
                WriteToLocal(ReSize(SizeTypes.Thumbnail), ReferenceId, Type, SizeTypes.Thumbnail);
                WriteToLocal(ReSize(SizeTypes.Icon), ReferenceId, Type, SizeTypes.Icon);
            }
            else
            {
                WriteToLocal(ReSize(SizeTypes.Logo), ReferenceId, Type, SizeTypes.Logo);
            }
        }

        private void WriteToLocal(SKBitmap image, long referenceId, Types type, SizeTypes sizeType)
        {
            var provider = BinaryStorageProviderFactory.Create(ProviderName());
            if (provider != null)
            {
                using var memory = new MemoryStream(EncodePng(image));
                provider.Upload(
                    objectName: ObjectName(referenceId, type, sizeType),
                    stream: memory,
                    contentType: "image/png");
            }
            else
            {
                Files.Write(EncodePng(image), Path(referenceId, type, sizeType));
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
            return EncodePng(ReSize(sizeType));
        }

        private static byte[] EncodePng(SKBitmap image)
        {
            using var skImage = SKImage.FromBitmap(image);
            using var encoded = skImage.Encode(SKEncodedImageFormat.Png, 100);
            return encoded.ToArray();
        }

        private SKBitmap ReSize(SizeTypes sizeType)
        {
            var size = (double)Size(sizeType);
            var rate = (Data.Width > Data.Height) || (sizeType == SizeTypes.Logo)
                ? size / Data.Height
                : size / Data.Width;
            if (rate != 1)
            {
                var width = (Data.Width * rate).ToInt();
                var height = (Data.Height * rate).ToInt();
                var x = (sizeType == SizeTypes.Logo) ? 0 : ((size - width) / 2).ToInt();
                var y = (sizeType == SizeTypes.Logo) ? 0 : ((size - height) / 2).ToInt();
                return GetImage(width, height, x, y);
            }
            else
            {
                return Data;
            }
        }

        public byte[] ReSizeBytes(decimal? size)
        {
            return EncodePng(ReSize(size));
        }

        private SKBitmap ReSize(decimal? size)
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
                return GetImage(width, height, 0, 0);
            }
            else
            {
                return Data;
            }
        }

        private SKBitmap GetImage(int width, int height, int x, int y)
        {
            var resized = Data.Resize(
                new SKImageInfo(
                    Math.Max(width, 1),
                    Math.Max(height, 1),
                    Data.ColorType,
                    Data.AlphaType),
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
            if (resized == null)
            {
                throw new FormatException("Failed to resize image.");
            }
            Data.Dispose();
            Data = resized;
            return Data;
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