using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Summary : ISettingListItem
    {
        public int Id { get; set; }
        public long SiteId;
        public string DestinationReferenceType;
        public string DestinationColumn;
        public int? DestinationCondition;
        public bool? SetZeroWhenOutOfCondition;
        public string LinkColumn;
        public string Type;
        public string SourceColumn;
        public int? SourceCondition;

        public Summary()
        {
        }

        public Summary(long siteId)
        {
            SiteId = siteId;
        }

        public Summary(
            int id,
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            int? destinationCondition,
            bool? setZeroWhenOutOfCondition,
            string linkColumn,
            string type,
            string sourceColumn,
            int? sourceCondition)
        {
            Id = id;
            SiteId = siteId;
            DestinationReferenceType = destinationReferenceType;
            DestinationColumn = destinationColumn;
            DestinationCondition = destinationCondition;
            LinkColumn = linkColumn;
            Type = type;
            SourceColumn = type != "Count"
                ? sourceColumn
                : null;
            SourceCondition = sourceCondition;
            SetZeroWhenOutOfCondition =
                SourceCondition != null && setZeroWhenOutOfCondition == true
                    ? setZeroWhenOutOfCondition
                    : null;
        }

        public void Update(
            long siteId,
            string destinationReferenceType,
            string destinationColumn,
            int? destinationCondition,
            bool? setZeroWhenOutOfCondition,
            string linkColumn,
            string type,
            string sourceColumn,
            int? sourceCondition)
        {
            SiteId = siteId;
            DestinationReferenceType = destinationReferenceType;
            DestinationColumn = destinationColumn;
            DestinationCondition = destinationCondition;
            LinkColumn = linkColumn;
            Type = type;
            SourceColumn = type != "Count"
                ? sourceColumn
                : null;
            SourceCondition = sourceCondition;
            SetZeroWhenOutOfCondition =
                SourceCondition != null && setZeroWhenOutOfCondition == true
                    ? setZeroWhenOutOfCondition
                    : null;
        }
    }
}