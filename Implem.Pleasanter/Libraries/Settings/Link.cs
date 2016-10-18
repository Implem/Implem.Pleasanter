namespace Implem.Pleasanter.Libraries.Settings
{
    public class Link
    {
        public string ColumnName;
        public long SiteId;

        public Link(string columnName, long siteId)
        {
            ColumnName = columnName;
            SiteId = siteId;
        }
    }
}