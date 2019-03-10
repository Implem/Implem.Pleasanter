using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Models
{
    public class JsonExportColumn
    {
        public long SiteId;
        public string SiteTitle;
        public Dictionary<string, string> Columns;

        public JsonExportColumn(long siteId, string siteTitle)
        {
            SiteId = siteId;
            SiteTitle = siteTitle;
            Columns = new Dictionary<string, string>();
        }
    }
}