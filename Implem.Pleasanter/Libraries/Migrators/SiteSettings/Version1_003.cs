using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_003
    {
        public static void Migrate1_003(this Settings.SiteSettings ss)
        {
            ss.LinkColumnSiteIdHash?.ForEach(data =>
            {
                if (ss.LinkCollection == null)
                {
                    ss.LinkCollection = new List<Link>();
                }
                ss.LinkCollection.Add(new Link(data.Key.Split_1st('_'), data.Value));
            });
            ss.LinkColumnSiteIdHash = null;
            ss.Migrated = true;
        }
    }
}