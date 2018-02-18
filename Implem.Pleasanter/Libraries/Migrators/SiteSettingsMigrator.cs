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
            if (ss.Version < 1.005M) ss.Migrate1_005();
            if (ss.Version < 1.006M) ss.Migrate1_006();
            if (ss.Version < 1.007M) ss.Migrate1_007();
            if (ss.Version < 1.008M) ss.Migrate1_008();
            if (ss.Version < 1.009M) ss.Migrate1_009();
            if (ss.Version < 1.010M) ss.Migrate1_010();
            if (ss.Version < 1.011M) ss.Migrate1_011();
            if (ss.Version < 1.012M) ss.Migrate1_012();
            if (ss.Version < 1.013M) ss.Migrate1_013();
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

        private static void MigrateSiteSettingsFormat(long siteId, SiteSettings ss)
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