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
            ss.GridColumnsOrder = hash
                .Where(o => (o.Value?.GridVisible).ToBool() ||
                    (o.Value?.GridVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.GridColumn > 0)
                        .Where(p => p.GridEnabled)
                        .Any()))
                .OrderBy(o => (ss.GridColumnsOrder!= null)
                    ? ss.GridColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.GridColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.FilterColumnsOrder = hash
                .Where(o => (o.Value?.FilterVisible).ToBool() ||
                    (o.Value?.FilterVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.FilterColumn > 0)
                        .Where(p => p.FilterEnabled)
                        .Any()))
                .OrderBy(o => (ss.FilterColumnsOrder!= null)
                    ? ss.FilterColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.FilterColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.EditorColumnsOrder = hash
                .Where(o => (o.Value?.EditorVisible).ToBool() ||
                    (o.Value?.EditorVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.EditorColumn)
                        .Where(p => p.EditorEnabled)
                        .Where(p => !p.NotEditorSettings)
                        .Any()))
                .OrderBy(o => (ss.EditorColumnsOrder!= null)
                    ? ss.EditorColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.No)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.TitleColumnsOrder = hash
                .Where(o => (o.Value?.TitleVisible).ToBool() ||
                    (o.Value?.TitleVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.ColumnName == "Title")
                        .Any()))
                .OrderBy(o => (ss.TitleColumnsOrder!= null)
                    ? ss.TitleColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.TitleColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.LinkColumnsOrder = hash
                .Where(o => (o.Value?.LinkVisible).ToBool() ||
                    (o.Value?.LinkVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.LinkColumn > 0)
                        .Where(p => p.LinkEnabled)
                        .Any()))
                .OrderBy(o => (ss.LinkColumnsOrder!= null)
                    ? ss.LinkColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.LinkColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            ss.HistoryColumnsOrder = hash
                .Where(o => (o.Value?.HistoryVisible).ToBool() ||
                    (o.Value?.HistoryVisible == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == ss.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.HistoryColumn > 0)
                        .Where(p => p.HistoryEnabled)
                        .Any()))
                .OrderBy(o => (ss.HistoryColumnsOrder!= null)
                    ? ss.HistoryColumnsOrder.IndexOf(o.Key)
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