using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Link
    {
        public string ColumnName;
        public long SiteId;

        public Link()
        {
        }

        public Link(string columnName, long siteId)
        {
            ColumnName = columnName;
            SiteId = siteId;
        }

        public Link GetRecordingData()
        {
            var link = new Link();
            link.ColumnName = ColumnName;
            link.SiteId = SiteId;
            return link;
        }
    }
}