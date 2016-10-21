using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_001
    {
        public static void Migrate1_001(this Settings.SiteSettings ss)
        {
            var hash = Def.ColumnDefinitionCollection
                .Where(p => p.TableName == ss.ReferenceType)
                .ToDictionary(
                    o => o.ColumnName,
                    o => ss.ColumnCollection.FirstOrDefault(p =>
                        p.ColumnName == o.ColumnName));
            ss.GridColumns = hash
                .Where(o => (o.Value?.GridVisible).ToBool() ||
                    (o.Value?.GridVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.GridColumn > 0)
                        .Where(p => p.GridEnabled)
                        .Any()))
                .OrderBy(o => (ss.GridColumns != null)
                    ? ss.GridColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.GridColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.FilterColumns = hash
                .Where(o => (o.Value?.FilterVisible).ToBool() ||
                    (o.Value?.FilterVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.FilterColumn > 0)
                        .Where(p => p.FilterEnabled)
                        .Any()))
                .OrderBy(o => (ss.FilterColumns != null)
                    ? ss.FilterColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.FilterColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.EditorColumns = hash
                .Where(o => (o.Value?.EditorVisible).ToBool() ||
                    (o.Value?.EditorVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.EditorColumn)
                        .Where(p => p.EditorEnabled)
                        .Where(p => !p.NotEditorSettings)
                        .Any()))
                .OrderBy(o => (ss.EditorColumns != null)
                    ? ss.EditorColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.No)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.TitleColumns = hash
                .Where(o => (o.Value?.TitleVisible).ToBool() ||
                    (o.Value?.TitleVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.ColumnName == "Title")
                        .Any()))
                .OrderBy(o => (ss.TitleColumns != null)
                    ? ss.TitleColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.TitleColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.LinkColumns = hash
                .Where(o => (o.Value?.LinkVisible).ToBool() ||
                    (o.Value?.LinkVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.LinkColumn > 0)
                        .Where(p => p.LinkEnabled)
                        .Any()))
                .OrderBy(o => (ss.LinkColumns != null)
                    ? ss.LinkColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.LinkColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.HistoryColumns = hash
                .Where(o => (o.Value?.HistoryVisible).ToBool() ||
                    (o.Value?.HistoryVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.HistoryColumn > 0)
                        .Where(p => p.HistoryEnabled)
                        .Any()))
                .OrderBy(o => (ss.HistoryColumns != null)
                    ? ss.HistoryColumns.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.HistoryColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.Migrated = true;
        }
    }
}