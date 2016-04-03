using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Images
    {
        public enum Types : int
        {
            SiteImage = 1
        }

        public enum SizeTypes : int
        {
            Regular = 1,
            Thumbnail = 2,
            Icon = 3
        }

        public static byte[] Get(long referenceId, Types type, SizeTypes sizeType)
        {
            return Files.Bytes(Path(referenceId, type, sizeType));
        }

        public static bool Exists(long referenceId, Types type, SizeTypes sizeType)
        {
            return File.Exists(Path(referenceId, type, sizeType));
        }

        public static string UrlPrefix(long referenceId, Types type, SizeTypes sizeType)
        {
            return new FileInfo(Path(referenceId, type, sizeType))
                .LastWriteTime.ToString("?yyyyMMddHHmmss");
        }

        public static void Write(this byte[] self, long referenceId, Types type)
        {
            using (var image = Image.FromStream(new MemoryStream(self)))
            {
                image.ReSize(referenceId, type, SizeTypes.Regular);
                image.ReSize(referenceId, type, SizeTypes.Thumbnail);
                image.ReSize(referenceId, type, SizeTypes.Icon);
            }
        }

        private static void ReSize(
            this Image image, long referenceId, Types type, SizeTypes sizeType)
        {
            var size = (double)Size(sizeType);
            var rate = image.Width > image.Height
                ? size / image.Height
                : size / image.Width;
            if (rate != 1)
            {
                var width = (image.Width * rate).ToInt();
                var height = (image.Height * rate).ToInt();
                var x = ((size - width) / 2).ToInt();
                var y = ((size - height) / 2).ToInt();
                var resizedImage = new Bitmap(size.ToInt(), size.ToInt());
                var graphics = Graphics.FromImage(resizedImage);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, x, y, width, height);
                resizedImage.Save(referenceId, type, sizeType);
                graphics.Dispose();
            }
            else
            {
                image.Save(referenceId, type, sizeType);
            }
        }

        private static void Save(
            this Image image, long referenceId, Types type, SizeTypes sizeType)
        {
            image.Write(Path(referenceId, type, sizeType), ImageFormat.Png);
        }

        private static string Path(long referenceId, Types type, SizeTypes sizeType)
        {
            return System.IO.Path.Combine(
                Directories.Data(),
                type.ToString(),
                "{0}_{1}.png".Params(referenceId, sizeType));
        }

        private static int Size(SizeTypes sizeType)
        {
            switch (sizeType)
            {
                case SizeTypes.Regular: return Parameters.ImageSizeRegular;
                case SizeTypes.Thumbnail:return Parameters.ImageSizeThumbnail;
                case SizeTypes.Icon:return Parameters.ImageSizeIcon;
                default: return Parameters.ImageSizeRegular;
            }
        }
    }
}