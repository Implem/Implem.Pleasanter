using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.PleasanterTest.Utilities
{
    internal static class Sites
    {
        public static SiteSettings GetSiteSettings(
            Context context,
            long siteId)
        {
            return Rds.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteSettings(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(siteId)))
                            .Deserialize<SiteSettings>();
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
