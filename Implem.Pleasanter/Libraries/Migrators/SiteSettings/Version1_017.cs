using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_017
    {
        public static void Migrate1_017(this SiteSettings ss)
        {
            ss.AddOrUpdateEditorColumnHash(ss.EditorColumns?.SelectMany(columnName =>
            {
                var sectionText = ss
                    .Columns
                    ?.FirstOrDefault(column => column?.ColumnName == columnName)
                    ?.Section;
                return sectionText.IsNullOrEmpty() != false
                    ? new[] { columnName }
                    : new[]
                    {
                        ss.SectionName(ss.AddSection(new Section { LabelText = sectionText }).Id),
                        columnName
                    };
            }).ToList() ?? new List<string>());
            ss.Columns?.ForEach(column => column.Section = null);
            ss.EditorColumns = null;
            ss.Migrated = true;
        }
    }
}