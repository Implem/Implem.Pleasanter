using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ColumnUtilities
    {
        public enum CheckFilterTypes : int
        {
            On = 1,
            Off = 2
        }

        public static Dictionary<string, string> CheckFilterTypeOptions()
        {
            return new Dictionary<string, string>
            {
                { CheckFilterTypes.On.ToInt().ToString(), Displays.On() },
                { CheckFilterTypes.Off.ToInt().ToString(), Displays.Off() }
            };
        }

        public enum CheckFilterControlTypes : int
        {
            OnOnly = 1,
            OnAndOff = 2
        }

        public static Dictionary<string, string> CheckFilterControlTypeOptions()
        {
            return new Dictionary<string, string>
            {
                { CheckFilterControlTypes.OnOnly.ToInt().ToString(), Displays.OnOnly() },
                { CheckFilterControlTypes.OnAndOff.ToInt().ToString(), Displays.OnAndOff() }
            };
        }

        public static IEnumerable<ColumnDefinition> GridDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.GridColumn > 0)
                .Where(o => o.GridEnabled || !enableOnly)
                .Where(o => Contract.Attachments() || o.ControlType != "Attachments")
                .OrderBy(o => o.GridColumn);
        }

        public static IEnumerable<ColumnDefinition> FilterDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.FilterColumn > 0)
                .Where(o => o.FilterEnabled || !enableOnly)
                .OrderBy(o => o.FilterColumn);
        }

        public static IEnumerable<ColumnDefinition> EditorDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.EditorColumn)
                .Where(o => o.EditorEnabled || !enableOnly)
                .Where(o => !o.NotEditorSettings)
                .Where(o => Contract.Attachments() || o.ControlType != "Attachments")
                .OrderBy(o => o.No);
        }

        public static IEnumerable<ColumnDefinition> TitleDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.TitleColumn > 0)
                .Where(o => o.ColumnName == "Title" || !enableOnly)
                .OrderBy(o => o.TitleColumn);
        }

        public static IEnumerable<ColumnDefinition> LinkDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.LinkColumn > 0)
                .Where(o => o.LinkEnabled || !enableOnly)
                .Where(o => Contract.Attachments() || o.ControlType != "Attachments")
                .OrderBy(o => o.LinkColumn);
        }

        public static IEnumerable<ColumnDefinition> HistoryDefinitions(
            this Dictionary<string, ColumnDefinition> definitions, bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.HistoryColumn > 0)
                .Where(o => o.HistoryEnabled || !enableOnly)
                .Where(o => Contract.Attachments() || o.ControlType != "Attachments")
                .OrderBy(o => o.HistoryColumn);
        }

        public static IEnumerable<ColumnDefinition> MonitorChangesDefinitions(
            this Dictionary<string, ColumnDefinition> definitions)
        {
            return definitions.Values
                .Where(o => o.EditorColumn || o.ColumnName == "Comments")
                .Where(o => !o.NotEditorSettings)
                .Where(o => !o.Unique)
                .Where(o => o.ControlType != "Attachment")
                .Where(o => o.ColumnName != "Ver")
                .OrderBy(o => o.No);
        }

        public static IEnumerable<ColumnDefinition> ExportDefinitions(
            this Dictionary<string, ColumnDefinition> definitions)
        {
            return definitions.Values
                .Where(o => o.GridColumn > 0)
                .Where(o => o.ControlType != "Attachment")
                .OrderBy(o => o.GridColumn);
        }

        public static Dictionary<string, ControlData> SelectableOptions(
            SiteSettings ss, IEnumerable<string> columns, string labelType = null)
        {
            return columns
                .Distinct()
                .ToDictionary(
                    columnName => columnName,
                    columnName => SelectableOptionsControlData(
                        ss: ss.GetJoinedSs(columnName),
                        columnName: columnName,
                        labelType: labelType));
        }

        public static Dictionary<string, ControlData> SelectableSourceOptions(
            SiteSettings ss, IEnumerable<string> columns, string labelType = null)
        {
            return columns.ToDictionary(
                columnName => columnName,
                columnName => SelectableOptionsControlData(
                    ss: ss,
                    columnName: columnName,
                    labelType: labelType));
        }

        private static ControlData SelectableOptionsControlData(
            SiteSettings ss, string columnName, string labelType)
        {
            var column = ss.GetColumn(columnName.Split(',').Last());
            var labelText = column.LabelText;
            var labelTextDefault = column.LabelTextDefault;
            switch (labelType)
            {
                case "Grid":
                    labelTextDefault += $" ({labelText})";
                    labelText = column.GridLabelText;
                    break;
            }
            return column != null
                ? new ControlData(
                    text: "[" + ss.Title + "] " + Displays.Get(labelText),
                    title: labelTextDefault)
                : new ControlData(string.Empty);
        }

        public static string ChangeCommand(string controlId)
        {
            if (controlId.StartsWith("MoveUp")) return "MoveUp";
            if (controlId.StartsWith("MoveDown")) return "MoveDown";
            if (controlId.StartsWith("ToDisable")) return "ToDisable";
            if (controlId.StartsWith("ToEnable")) return "ToEnable";
            return null;
        }

        public static List<T> GetChanged<T>(
            List<T> order,
            string command,
            List<T> selectedColumns,
            List<T> selectedSourceColumns = null)
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

        private static List<T> Sort<T>(T[] order, string command, List<T> selectedColumns)
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

        public static string ColumnName(string tableAlias, string columnName)
        {
            return !tableAlias.IsNullOrEmpty()
                ? tableAlias + "," + columnName
                : columnName;
        }

        public static long GetSiteIdByTableAlias(string tableAlias, long siteId)
        {
            return tableAlias.IsNullOrEmpty()
                ? siteId
                : tableAlias.Split('-').Last().Split_2nd('~').ToLong();
        }

        public static long GetSiteIdByTableAlias(string tableAlias)
        {
            return tableAlias.Split('-').Last().Split_2nd('~').ToLong();
        }
    }
}