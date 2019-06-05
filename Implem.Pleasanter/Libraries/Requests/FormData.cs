using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class FormData
    {
        public long SiteId;
        public long Id;
        public string Suffix;
        public SiteSettings SiteSettings;
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public FormData(long siteId, long id, string suffix, SiteSettings ss)
        {
            SiteId = siteId;
            Id = id;
            Suffix = suffix;
            SiteSettings = ss;
        }
    }
}