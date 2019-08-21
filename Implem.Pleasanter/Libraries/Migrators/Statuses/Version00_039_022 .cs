using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators.Statuses
{
    public static class Version00_039_022
    {
        public static void Migrate(Context context)
        {
            new ExportSettingCollection(context: context)
                .ForEach(exportSettingModel =>
                    Migrate(context: context, exportSettingModel: exportSettingModel));
        }

        private static void Migrate(Context context, ExportSettingModel exportSettingModel)
        {
            var ss = Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SiteSettings(),
                    where: Rds.SitesWhere().SiteId(exportSettingModel.ReferenceId)))
                        .DeserializeSiteSettings(context: context);
            if (ss != null)
            {
                ss.SiteId = exportSettingModel.ReferenceId;
                ss.Exports = ss.Exports ?? new SettingList<Export>();
                ss.Exports.Add(new Export(
                    id: ss.Exports.Any()
                        ? ss.Exports.Max(o => o.Id) + 1
                        : 1,
                    type: Export.Types.Csv,
                    name: exportSettingModel.Title.Value,
                    header: exportSettingModel.AddHeader,
                    columns: exportSettingModel.ExportColumns.Columns
                        .Where(o => o.Value)
                        .Select((o, i) => new ExportColumn(
                            context: context,
                            column: ss.GetColumn(
                                context: context,
                                columnName: o.Key),
                            id: i + 1))
                        .ToList()));
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateSites(
                        param: Rds.SitesParam().SiteSettings(ss.RecordingJson(context: context)),
                        where: Rds.SitesWhere().SiteId(exportSettingModel.ReferenceId)));
            }
        }
    }
}