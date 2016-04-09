namespace Implem.Pleasanter.Libraries.Settings
{
    public class Summary
    {
        public int Id;
        public long SiteId;
        public string DestinationReferenceType;
        public string DestinationColumn;
        public string LinkColumn;
        public string Type;
        public string SourceColumn;
       
        public Summary()
        {
        }

        public Summary(
            int id,
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            string linkColumn,
            string type,
            string sourceColumn)
        {
            Id = id;
            SiteId = siteId;
            DestinationReferenceType = destinationReferenceType;
            DestinationColumn = destinationColumn;
            LinkColumn = linkColumn;
            Type = type;
            SourceColumn = sourceColumn;
        }
    }
}