using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
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
            if (ss.Version < 1.014M) ss.Migrate1_014();
            if (ss.Version < 1.015M) ss.Migrate1_015();
            if (ss.Version < 1.016M) ss.Migrate1_016();
        }

        public static void Migrate(Context context)
        {
            if (context.HasPrivilege)
            {
                Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn()
                            .SiteId()
                            .SiteSettings())).AsEnumerable().ForEach(dataRow =>
                                MigrateSiteSettingsFormat(
                                    context: context,
                                    ss: dataRow.String("SiteSettings")
                                        .DeserializeSiteSettings(context: context),
                                    siteId: dataRow.Long("SiteId")));
            }
        }

        private static void MigrateSiteSettingsFormat(
            Context context, SiteSettings ss, long siteId)
        {
            if (ss == null) return;
            if (ss.Migrated)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateSites(
                        where: Rds.SitesWhere().SiteId(siteId),
                        param: Rds.SitesParam().SiteSettings(
                            ss.RecordingJson(context: context)),
                        addUpdatedTimeParam: false,
                        addUpdatorParam: false));
            }
        }
    }
}