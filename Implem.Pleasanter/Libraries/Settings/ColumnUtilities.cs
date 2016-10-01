using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ColumnUtilities
    {
        public static IEnumerable<ColumnDefinition> GridDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.GridColumn > 0)
                .Where(o => o.GridVisible || !visibleOnly)
                .OrderBy(o => o.GridColumn);
        }

        public static IEnumerable<ColumnDefinition> FilterDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.FilterColumn > 0)
                .Where(o => o.FilterVisible || !visibleOnly)
                .OrderBy(o => o.FilterColumn);
        }

        public static IEnumerable<ColumnDefinition> EditorDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.EditorColumn)
                .Where(o => o.EditorVisible || !visibleOnly)
                .Where(o => !o.NotEditorSettings)
                .OrderBy(o => o.No);
        }

        public static IEnumerable<ColumnDefinition> TitleDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.TitleColumn > 0)
                .Where(o => o.ColumnName == "Title" || !visibleOnly)
                .OrderBy(o => o.TitleColumn);
        }

        public static IEnumerable<ColumnDefinition> LinkDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.LinkColumn > 0)
                .Where(o => o.LinkVisible || !visibleOnly)
                .OrderBy(o => o.LinkColumn);
        }

        public static IEnumerable<ColumnDefinition> HistoryDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.HistoryColumn > 0)
                .Where(o => o.HistoryVisible || !visibleOnly)
                .OrderBy(o => o.HistoryColumn);
        }

        public static IEnumerable<ColumnDefinition> MonitorChangesDefinitions(
            string referenceType, bool visibleOnly = false)
        {
            return Def.ColumnDefinitionCollection
                .Where(o => o.TableName == referenceType)
                .Where(o => o.EditorColumn)
                .Where(o => o.EditorVisible || !visibleOnly)
                .Where(o => !o.NotEditorSettings)
                .Where(o => !o.Unique)
                .Where(o => o.ColumnName != "Ver")
                .OrderBy(o => o.No);
        }

        public static Dictionary<string, string> SelectableOptions(
            SiteSettings siteSettings, IEnumerable<string> columns, bool visible = true)
        {
            return columns.ToDictionary(
                o => o,
                o => visible
                    ? Displays.Get(siteSettings.GetColumn(o).LabelText)
                    : Displays.Get(siteSettings.GetColumn(o).LabelText) +
                        " (" + Displays.Disabled() + ")");
        }

        public static string ChangeCommand(string controlId)
        {
            if (controlId.StartsWith("MoveUp")) return "MoveUp";
            if (controlId.StartsWith("MoveDown")) return "MoveDown";
            if (controlId.StartsWith("ToDisable")) return "ToDisable";
            if (controlId.StartsWith("ToEnable")) return "ToEnable";
            return null;
        }

        public static List<string> GetChanged(
            List<string> order,
            string command,
            List<string> selectedColumns,
            List<string> selectedSourceColumns)
        {
            switch (command)
            {
                case "MoveUp":
                case "MoveDown":
                    order = Sort(order.ToArray(), command, selectedColumns);
                    break;
                case "ToDisable":
                    order.RemoveAll(o => selectedColumns.Contains(o));
                    break;
                case "ToEnable":
                    order.AddRange(selectedSourceColumns);
                    break;
            }
            return order;
        }

        private static List<string> Sort(
            string[] order, string command, List<string> selectedColumns)
        {
            if (command == "MoveDown") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedColumns.Contains(data.ColumnName) &&
                    data.Index > 0 &&
                    !selectedColumns.Contains(order[data.Index - 1]))
                {
                    order = Arrays.Swap(order, data.Index, data.Index - 1);
                }
            });
            if (command == "MoveDown") Array.Reverse(order);
            return order.ToList();
        }
    }
}