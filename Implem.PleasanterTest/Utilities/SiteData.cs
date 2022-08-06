using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.PleasanterTest.Utilities
{
    internal static class SiteData
    {
        public static SiteSettings GetSiteSettings(
            Context context,
            string title)
        {
            var ss = Initializer.Sites.Get(title).SavedSiteSettings.Deserialize<SiteSettings>();
            ss.Init(context: context);
            return ss;
        }

        public static void UpdateSiteSettings(
            Context context,
            long siteId,
            SiteSettings ss)
        {
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateSites(
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(siteId),
                    param: Rds.SitesParam()
                        .SiteSettings(ss.RecordingData(context: context).ToJson())));
        }
    }
}
