using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_003
    {
        public static void Migrate1_003(this Settings.SiteSettings siteSettings)
        {
            siteSettings.LinkColumnSiteIdHash?.ForEach(data =>
            {
                if (siteSettings.LinkCollection == null)
                {
                    siteSettings.LinkCollection = new List<Link>();
                }
                siteSettings.LinkCollection.Add(new Link(data.Key.Split_1st('_'), data.Value));
            });
            siteSettings.LinkColumnSiteIdHash = null;
            siteSettings.Migrated = true;
        }
    }
}