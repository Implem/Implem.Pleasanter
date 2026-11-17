using SkiaSharp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Images
{
    internal static class ApngEncoder
    {
        private static readonly byte[] Signature =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        private static readonly HashSet<string> CopiedAncillaryChunks = new()
        {
            "sRGB",
            "gAMA",
            "cHRM",
            "iCCP",
            "pHYs"
        };

        private static readonly int[] DelayDenominators = { 1000, 100, 10, 1 };

        private static readonly uint[] CrcTable = CreateCrcTable();

        public static byte[] Encode(
            IReadOnlyList<SKBitmap> frames,
            IReadOnlyList<int> durations,
            int repetitionCount,
            long maxBytes)
        {
            if (frames == null
                || durations == null
                || frames.Count < 2
                || frames.Count != durations.Count
                || maxBytes <= 0)
            {
                throw new InvalidDataException("Invalid animation frames.");
            }
            var canvasWidth = frames[0].Width;
            var canvasHeight = frames[0].Height;
            if (frames.Any(frame => frame == null
                || frame.Width != canvasWidth
                || frame.Height != canvasHeight
                || frame.ColorType != frames[0].ColorType
                || frame.BytesPerPixel <= 0))
            {
                throw new InvalidDataException("Frame size mismatch.");
            }
            var limit = Math.Min(maxBytes, int.MaxValue / 2);
            using var output = new MemoryStream();
            output.Write(Signature);
            byte[] ihdr = null;
            uint sequence = 0;
            for (var i = 0; i < frames.Count; i++)
            {
                var region = i == 0
                    ? new SKRectI(0, 0, canvasWidth, canvasHeight)
                    : ChangedRegion(
                        previous: frames[i - 1],
                        current: frames[i]);
                List<(string Type, byte[] Data)> chunks;
                if (i == 0)
                {
                    chunks = ReadChunks(png: EncodeFrame(frame: frames[0]));
                }
                else
                {
                    using var subset = new SKBitmap();
                    if (!frames[i].ExtractSubset(subset, region))
                    {
                        throw new InvalidDataException("Failed to extract frame region.");
                    }
                    chunks = ReadChunks(png: EncodeFrame(frame: subset));
                }
                var frameIhdr = chunks[0].Data;
                if (frameIhdr.Length != 13
                    || BinaryPrimitives.ReadUInt32BigEndian(frameIhdr.AsSpan(0, 4))
                        != (uint)region.Width
                    || BinaryPrimitives.ReadUInt32BigEndian(frameIhdr.AsSpan(4, 4))
                        != (uint)region.Height)
                {
                    throw new InvalidDataException("Invalid IHDR.");
                }
                if (i == 0)
                {
                    ihdr = frameIhdr;
                    WriteChunk(
                        output: output,
                        type: "IHDR",
                        data: ihdr);
                    foreach (var chunk in chunks
                        .Where(chunk => CopiedAncillaryChunks.Contains(chunk.Type)))
                    {
                        WriteChunk(
                            output: output,
                            type: chunk.Type,
                            data: chunk.Data);
                    }
                    WriteChunk(
                        output: output,
                        type: "acTL",
                        data: AnimationControl(
                            frameCount: frames.Count,
                            repetitionCount: repetitionCount));
                }
                else if (!frameIhdr.AsSpan(8, 5).SequenceEqual(ihdr.AsSpan(8, 5)))
                {
                    throw new InvalidDataException("Frame header mismatch.");
                }
                WriteChunk(
                    output: output,
                    type: "fcTL",
                    data: FrameControl(
                        sequence: sequence,
                        region: region,
                        duration: durations[i]));
                sequence = checked(sequence + 1);
                foreach (var idat in chunks.Where(chunk => chunk.Type == "IDAT"))
                {
                    if (i == 0)
                    {
                        WriteChunk(
                            output: output,
                            type: "IDAT",
                            data: idat.Data);
                    }
                    else
                    {
                        var sequenceBytes = new byte[4];
                        BinaryPrimitives.WriteUInt32BigEndian(sequenceBytes, sequence);
                        WriteChunk(
                            output: output,
                            type: "fdAT",
                            prefix: sequenceBytes,
                            data: idat.Data);
                        sequence = checked(sequence + 1);
                    }
                    if (output.Length > limit)
                    {
                        throw new InvalidDataException("APNG output too large.");
                    }
                }
            }
            WriteChunk(
                output: output,
                type: "IEND",
                data: Array.Empty<byte>());
            return output.ToArray();
        }

        private static SKRectI ChangedRegion(SKBitmap previous, SKBitmap current)
        {
            var bytesPerPixel = current.BytesPerPixel;
            int rowLength;
            long previousRequired;
            long currentRequired;
            try
            {
                rowLength = checked(current.Width * bytesPerPixel);
                previousRequired = checked(
                    (long)(previous.Height - 1) * previous.RowBytes + rowLength);
                currentRequired = checked(
                    (long)(current.Height - 1) * current.RowBytes + rowLength);
            }
            catch (OverflowException)
            {
                throw new InvalidDataException("Frame too large.");
            }
            var previousPixels = previous.GetPixelSpan();
            var currentPixels = current.GetPixelSpan();
            if (previous.RowBytes < rowLength
                || current.RowBytes < rowLength
                || previousPixels.Length < previousRequired
                || currentPixels.Length < currentRequired)
            {
                throw new InvalidDataException("Invalid frame buffer.");
            }
            int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;
            for (var y = 0; y < current.Height; y++)
            {
                var previousRow = previousPixels.Slice(
                    (int)((long)y * previous.RowBytes),
                    rowLength);
                var currentRow = currentPixels.Slice(
                    (int)((long)y * current.RowBytes),
                    rowLength);
                var first = previousRow.CommonPrefixLength(currentRow);
                if (first == rowLength)
                {
                    continue;
                }
                var last = rowLength - 1;
                while (previousRow[last] == currentRow[last])
                {
                    last--;
                }
                left = Math.Min(left, first / bytesPerPixel);
                right = Math.Max(right, last / bytesPerPixel);
                top = Math.Min(top, y);
                bottom = y;
            }
            return right < 0
                ? new SKRectI(0, 0, 1, 1)
                : new SKRectI(left, top, right + 1, bottom + 1);
        }

        private static byte[] EncodeFrame(SKBitmap frame)
        {
            using var data = frame?.Encode(SKEncodedImageFormat.Png, 100)
                ?? throw new InvalidDataException("Failed to encode frame.");
            return data.ToArray();
        }

        private static byte[] AnimationControl(int frameCount, int repetitionCount)
        {
            var data = new byte[8];
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(0, 4), (uint)frameCount);
            var plays = repetitionCount < 0
                ? 0u
                : (uint)Math.Min((long)repetitionCount + 1, uint.MaxValue);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(4, 4), plays);
            return data;
        }

        private static byte[] FrameControl(uint sequence, SKRectI region, int duration)
        {
            var data = new byte[26];
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(0, 4), sequence);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(4, 4), (uint)region.Width);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(8, 4), (uint)region.Height);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(12, 4), (uint)region.Left);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(16, 4), (uint)region.Top);
            var (numerator, denominator) = Delay(duration: duration);
            BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(20, 2), numerator);
            BinaryPrimitives.WriteUInt16BigEndian(data.AsSpan(22, 2), denominator);
            data[24] = 0;
            data[25] = 0;
            return data;
        }

        private static (ushort Numerator, ushort Denominator) Delay(int duration)
        {
            var milliseconds = Math.Max(0L, duration);
            foreach (var denominator in DelayDenominators)
            {
                var numerator = (milliseconds * denominator + 500) / 1000;
                if (numerator <= ushort.MaxValue)
                {
                    return ((ushort)numerator, (ushort)denominator);
                }
            }
            return (ushort.MaxValue, 1);
        }

        private static List<(string Type, byte[] Data)> ReadChunks(byte[] png)
        {
            if (png == null
                || png.Length < Signature.Length
                || !png.AsSpan(0, Signature.Length).SequenceEqual(Signature))
            {
                throw new InvalidDataException("Invalid PNG signature.");
            }
            var chunks = new List<(string Type, byte[] Data)>();
            var offset = Signature.Length;
            while (true)
            {
                if (png.Length - offset < 12)
                {
                    throw new InvalidDataException("Truncated PNG chunk.");
                }
                var length = BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset, 4));
                if (length > int.MaxValue || length > (uint)(png.Length - offset - 12))
                {
                    throw new InvalidDataException("Invalid PNG chunk length.");
                }
                var typeBytes = png.AsSpan(offset + 4, 4);
                foreach (var b in typeBytes)
                {
                    if (!((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z')))
                    {
                        throw new InvalidDataException("Invalid PNG chunk type.");
                    }
                }
                var type = System.Text.Encoding.ASCII.GetString(typeBytes);
                var dataLength = (int)length;
                var crc = BinaryPrimitives.ReadUInt32BigEndian(
                    png.AsSpan(offset + 8 + dataLength, 4));
                if (crc != Crc(
                    first: png.AsSpan(offset + 4, 4 + dataLength),
                    second: ReadOnlySpan<byte>.Empty))
                {
                    throw new InvalidDataException("Invalid PNG chunk CRC.");
                }
                var isCritical = (typeBytes[0] & 0x20) == 0;
                if (isCritical && type != "IHDR" && type != "IDAT" && type != "IEND")
                {
                    throw new InvalidDataException($"Unsupported critical chunk: {type}");
                }
                chunks.Add((type, png.AsSpan(offset + 8, dataLength).ToArray()));
                offset += 12 + dataLength;
                if (type == "IEND")
                {
                    break;
                }
            }
            if (chunks.Count(chunk => chunk.Type == "IHDR") != 1
                || chunks[0].Type != "IHDR"
                || !chunks.Any(chunk => chunk.Type == "IDAT"))
            {
                throw new InvalidDataException("Invalid PNG structure.");
            }
            return chunks;
        }

        private static void WriteChunk(Stream output, string type, byte[] data)
        {
            WriteChunk(
                output: output,
                type: type,
                prefix: ReadOnlySpan<byte>.Empty,
                data: data);
        }

        private static void WriteChunk(
            Stream output,
            string type,
            ReadOnlySpan<byte> prefix,
            ReadOnlySpan<byte> data)
        {
            var header = new byte[8];
            BinaryPrimitives.WriteUInt32BigEndian(
                header.AsSpan(0, 4),
                checked((uint)(prefix.Length + data.Length)));
            for (var i = 0; i < 4; i++)
            {
                header[4 + i] = (byte)type[i];
            }
            output.Write(header);
            output.Write(prefix);
            output.Write(data);
            var crc = Crc(
                first: header.AsSpan(4, 4),
                second: prefix);
            crc = Crc(
                bytes: data,
                crc: crc);
            var crcBytes = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(crcBytes, crc);
            output.Write(crcBytes);
        }

        private static uint Crc(ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
        {
            return Crc(
                bytes: second,
                crc: Crc(
                    bytes: first,
                    crc: 0u));
        }

        private static uint Crc(ReadOnlySpan<byte> bytes, uint crc)
        {
            var c = crc ^ 0xFFFFFFFFu;
            foreach (var b in bytes)
            {
                c = CrcTable[(c ^ b) & 0xFF] ^ (c >> 8);
            }
            return c ^ 0xFFFFFFFFu;
        }

        private static uint[] CreateCrcTable()
        {
            var table = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                var c = n;
                for (var k = 0; k < 8; k++)
                {
                    c = (c & 1) != 0
                        ? 0xEDB88320u ^ (c >> 1)
                        : c >> 1;
                }
                table[n] = c;
            }
            return table;
        }
    }
}
