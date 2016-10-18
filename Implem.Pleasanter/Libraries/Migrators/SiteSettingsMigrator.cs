using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class SiteSettingsMigrator
    {
        public static void Migrate(SiteSettings siteSettings)
        {
            if (siteSettings.Version < 1.001M) siteSettings.Migrate1_001();
            if (siteSettings.Version < 1.002M) siteSettings.Migrate1_002();
            if (siteSettings.Version < 1.003M) siteSettings.Migrate1_003();
        }

        public static void Migrate()
        {
            Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .SiteId()
                    .SiteSettings())).AsEnumerable().ForEach(dataRow =>
                        MigrateSiteSettingsFormat(
                            dataRow["SiteId"].ToLong(),
                            dataRow["SiteSettings"].ToString().Deserialize<SiteSettings>()));
        }

        private static void MigrateSiteSettingsFormat(long siteId, Settings.SiteSettings siteSettings)
        {
            if (siteSettings == null) return;
            if (siteSettings.Migrated)
            {
                Rds.ExecuteNonQuery(statements: Rds.UpdateSites(
                    where: Rds.SitesWhere().SiteId(siteId),
                    param: Rds.SitesParam().SiteSettings(siteSettings.ToJson()),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
            }
        }
    }
}