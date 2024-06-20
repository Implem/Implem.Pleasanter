namespace Implem.ParameterAccessor.Parts
{
    public class BinaryStorage
    {
        public string Provider;
        public string Path;
        public bool Attachments;
        public bool Images;
        public bool RestoreLocalFiles;
        public decimal LimitQuantity;
        public decimal LimitSize;
        public decimal LimitTotalSize;
        public decimal MinQuantity;
        public decimal MaxQuantity;
        public decimal MinSize;
        public decimal MaxSize;
        public decimal TotalMinSize;
        public decimal TotalMaxSize;
        public decimal Image;
        public decimal? ImageLimitSize;
        public decimal? ThumbnailLimitSize;
        public decimal ThumbnailMinSize;
        public decimal ThumbnailMaxSize;
        public decimal LocalFolderLimitSize;
        public decimal LocalFolderMinSize;
        public decimal LocalFolderMaxSize;
        public decimal LocalFolderLimitTotalSize;
        public decimal LocalFolderTotalMinSize;
        public decimal LocalFolderTotalMaxSize;
        public bool UseStorageSelect;
        public string DefaultBinaryStorageProvider;
        public string TemporaryBinaryStorageProvider;
        public string ImagesProvider;
        public string SiteImageProvider;

        public string GetProvider(string provider)
        {
            return !string.IsNullOrEmpty(provider) ? provider : Provider;
        }

        public string GetImagesProvider()
        {
            return GetProvider(ImagesProvider);
        }

        public string GetSiteImageProvider()
        {
            return GetProvider(SiteImageProvider);
        }

        public bool IsLocal(string provider)
        {
            var p = (!string.IsNullOrEmpty(provider) ? provider : Provider);
            return p == "Local" || p == "LocalFolder";
        }

        public bool IsLocal()
        {
            return IsLocal(Provider);
        }

        public bool IsLocalImages()
        {
            return IsLocal(ImagesProvider);
        }

        public bool IsLocalSiteImage()
        {
            return IsLocal(SiteImageProvider);
        }
    }
}