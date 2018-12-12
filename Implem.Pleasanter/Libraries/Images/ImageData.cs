using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace Implem.Pleasanter.Libraries.Images
{
    public class ImageData
    {
        public Image Data;
        public long ReferenceId;
        public Types Type;

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
            Data = Image.FromStream(new MemoryStream(data));
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
            return Files.Bytes(Path(ReferenceId, Type, sizeType));
        }

        public bool Exists(SizeTypes sizeType)
        {
            return File.Exists(Path(ReferenceId, Type, sizeType));
        }

        public string UrlPrefix(SizeTypes sizeType)
        {
            return new FileInfo(Path(ReferenceId, Type, sizeType))
                .LastWriteTime.ToString("?yyyyMMddHHmmss");
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

        private void WriteToLocal(Image image, long referenceId, Types type, SizeTypes sizeType)
        {
            image.Write(Path(referenceId, type, sizeType), ImageFormat.Png);
        }

        public void DeleteLocalFiles()
        {
            if (Type == Types.SiteImage)
            {
                WriteToLocal(ReSize(SizeTypes.Regular), ReferenceId, Type, SizeTypes.Regular);
                WriteToLocal(ReSize(SizeTypes.Thumbnail), ReferenceId, Type, SizeTypes.Thumbnail);
                WriteToLocal(ReSize(SizeTypes.Icon), ReferenceId, Type, SizeTypes.Icon);
            }
            else
            {
                Files.DeleteFile(Path(ReferenceId, Type, SizeTypes.Logo));
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
            using (var memory = new MemoryStream())
            {
                memory.Position = 0;
                ReSize(sizeType).Save(memory, ImageFormat.Bmp);
                var ret = new byte[memory.Length];
                memory.Position = 0;
                memory.Read(ret, 0, (int)memory.Length);
                memory.Close();
                return ret;
            }
        }

        private Image ReSize(SizeTypes sizeType)
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
                var resizedImage = new Bitmap(width, height);
                resizedImage.MakeTransparent();
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(Data, x, y, width, height);
                    return resizedImage;
                }
            }
            else
            {
                return Data;
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
    }
}