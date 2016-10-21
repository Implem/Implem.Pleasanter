using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class SiteSettingsMigrator
    {
        public static void Migrate(SiteSettings ss)
        {
            if (ss.Version < 1.001M) ss.Migrate1_001();
            if (ss.Version < 1.002M) ss.Migrate1_002();
            if (ss.Version < 1.003M) ss.Migrate1_003();
            if (ss.Version < 1.004M) ss.Migrate1_004();
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

        private static void MigrateSiteSettingsFormat(long siteId, Settings.SiteSettings ss)
        {
            if (ss == null) return;
            if (ss.Migrated)
            {
                Rds.ExecuteNonQuery(statements: Rds.UpdateSites(
                    where: Rds.SitesWhere().SiteId(siteId),
                    param: Rds.SitesParam().SiteSettings(ss.RecordingJson()),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
            }
        }
    }
}