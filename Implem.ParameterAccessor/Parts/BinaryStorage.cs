namespace Implem.ParameterAccessor.Parts
{
    public class BinaryStorage
    {
        public string Provider;
        public string Path;
        public bool Attachments;
        public bool Images;
        public int LimitQuantity;
        public int LimitSize;
        public int LimitTotalSize;
        public int MinQuantity;
        public int MaxQuantity;
        public int MinSize;
        public int MaxSize;
        public int TotalMinSize;
        public int TotalMaxSize;

        public bool IsLocal()
        {
            return Provider == "Local";
        }
    }
}