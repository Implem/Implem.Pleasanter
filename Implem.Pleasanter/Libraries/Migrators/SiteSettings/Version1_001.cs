using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_001
    {
        public static void Migrate1_001(this Settings.SiteSettings siteSettings)
        {
            var hash = Def.ColumnDefinitionCollection
                .Where(p => p.TableName == siteSettings.ReferenceType)
                .ToDictionary(o => o.ColumnName, o => siteSettings.AllColumn(o.ColumnName));
            siteSettings.GridColumnsOrder = hash
                .Where(o => (o.Value?.GridVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.GridColumn > 0)
                        .Where(p => p.GridVisible)
                        .Any()))
                .OrderBy(o => (siteSettings.GridColumnsOrder != null)
                    ? siteSettings.GridColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.GridColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.FilterColumnsOrder = hash
                .Where(o => (o.Value?.FilterVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.FilterColumn > 0)
                        .Where(p => p.FilterVisible)
                        .Any()))
                .OrderBy(o => (siteSettings.FilterColumnsOrder != null)
                    ? siteSettings.FilterColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.FilterColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.EditorColumnsOrder = hash
                .Where(o => (o.Value?.EditorVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.EditorColumn)
                        .Where(p => p.EditorVisible)
                        .Where(p => !p.NotEditorSettings)
                        .Any()))
                .OrderBy(o => (siteSettings.EditorColumnsOrder != null)
                    ? siteSettings.EditorColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.No)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.TitleColumnsOrder = hash
                .Where(o => (o.Value?.TitleVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.ColumnName == "Title")
                        .Any()))
                .OrderBy(o => (siteSettings.TitleColumnsOrder != null)
                    ? siteSettings.TitleColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.TitleColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.LinkColumnsOrder = hash
                .Where(o => (o.Value?.LinkVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.LinkColumn > 0)
                        .Where(p => p.LinkVisible)
                        .Any()))
                .OrderBy(o => (siteSettings.LinkColumnsOrder != null)
                    ? siteSettings.LinkColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.LinkColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.HistoryColumnsOrder = hash
                .Where(o => (o.Value?.HistoryVisible).ToBool() || (o.Value == null &&
                    Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Where(p => p.HistoryColumn > 0)
                        .Where(p => p.HistoryVisible)
                        .Any()))
                .OrderBy(o => (siteSettings.HistoryColumnsOrder != null)
                    ? siteSettings.HistoryColumnsOrder.IndexOf(o.Key)
                    : Def.ColumnDefinitionCollection
                        .Where(p => p.TableName == siteSettings.ReferenceType)
                        .Where(p => p.ColumnName == o.Key)
                        .Select(p => p.HistoryColumn)
                        .FirstOrDefault())
                .Select(o => o.Key)
                .ToList();
            siteSettings.Migrated = true;
        }
    }
}