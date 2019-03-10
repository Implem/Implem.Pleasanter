using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_015
    {
        public static void Migrate1_015(this SiteSettings ss)
        {
            ss.Exports?.ForEach(export =>
            {
                export.Columns?.ForEach(exportColumn =>
                {
                    var index = export.Join
                        ?.Select(o => o.SiteId)
                        .ToList()
                        .IndexOf(exportColumn.SiteId.ToLong()) + 1 ?? 0;
                    if (index > 0)
                    {
                        exportColumn.ColumnName = export.Join
                            .Take(index)
                            .Select(o => $"{o.ColumnName}~{o.SiteId}")
                            .Join("-")
                                + "," + exportColumn.ColumnName;
                    }
                    exportColumn.SiteId = null;
                });
                export.Join = null;
            });
            ss.Migrated = true;
        }
    }
}