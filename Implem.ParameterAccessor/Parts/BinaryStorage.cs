namespace Implem.ParameterAccessor.Parts
{
    public class BinaryStorage
    {
        public string Provider;
        public string Path;
        public bool Attachments;
        public bool Images;
        public decimal LimitQuantity;
        public decimal LimitSize;
        public decimal LimitTotalSize;
        public decimal MinQuantity;
        public decimal MaxQuantity;
        public decimal MinSize;
        public decimal MaxSize;
        public decimal TotalMinSize;
        public decimal TotalMaxSize;

        public bool IsLocal()
        {
            return Provider == "Local";
        }
    }
}