using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ColumnUtilities
    {
        public static List<string> ExtendedColumns(string tableName = null)
        {
            return Def.ColumnDefinitionCollection
                .Where(columnDefinition => !columnDefinition.ExtendedColumnType.IsNullOrEmpty())
                .Where(columnDefinition => columnDefinition.TableName == tableName || tableName == null)
                .Select(columnDefinition => columnDefinition.ColumnName)
                .OrderBy(columnName => columnName)
                .ToList();
        }

        public enum CheckFilterTypes : int
        {
            On = 1,
            Off = 2
        }

        public static Dictionary<string, string> CheckFilterTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { CheckFilterTypes.On.ToInt().ToString(), Displays.On(context: context) },
                { CheckFilterTypes.Off.ToInt().ToString(), Displays.Off(context: context) }
            };
        }

        public enum CheckFilterControlTypes : int
        {
            OnOnly = 1,
            OnAndOff = 2
        }

        public static Dictionary<string, string> CheckFilterControlTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { CheckFilterControlTypes.OnOnly.ToInt().ToString(), Displays.OnOnly(context: context) },
                { CheckFilterControlTypes.OnAndOff.ToInt().ToString(), Displays.OnAndOff(context: context) }
            };
        }

        public enum DateFilterSetMode : int
        {
            Default = 1,
            Range = 2
        }

        public static Dictionary<string, string> DateFilterSetModeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { DateFilterSetMode.Default.ToInt().ToString(), Displays.DateFilterSetModeDefault(context: context) },
                { DateFilterSetMode.Range.ToInt().ToString(), Displays.DateFilterSetModeRange(context: context) }
            };
        }

        public static Dictionary<string, string> SearchTypeOptions(Context context)
        {
            return new Dictionary<string, string>
            {
                { Column.SearchTypes.PartialMatch.ToInt().ToString(), Displays.PartialMatch(context: context) },
                { Column.SearchTypes.ExactMatch.ToInt().ToString(), Displays.ExactMatch(context: context) },
                { Column.SearchTypes.ForwardMatch.ToInt().ToString(), Displays.ForwardMatch(context: context) }
            };
        }

        public static IEnumerable<ColumnDefinition> GridDefinitions(
            this Dictionary<string, ColumnDefinition> definitions,
            Context context,
            bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.GridColumn > 0)
                .Where(o => o.GridEnabled || !enableOnly)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
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
            this Dictionary<string, ColumnDefinition> definitions,
            Context context,
            bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.EditorColumn > 0)
                .Where(o => o.EditorEnabled || !enableOnly)
                .Where(o => !o.NotEditorSettings)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .OrderBy(o => o.EditorColumn);
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
            this Dictionary<string, ColumnDefinition> definitions,
            Context context,
            bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.LinkColumn > 0)
                .Where(o => o.LinkEnabled || !enableOnly)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .OrderBy(o => o.LinkColumn);
        }

        public static IEnumerable<ColumnDefinition> HistoryDefinitions(
            this Dictionary<string, ColumnDefinition> definitions,
            Context context,
            bool enableOnly = false)
        {
            return definitions.Values
                .Where(o => o.HistoryColumn > 0)
                .Where(o => o.HistoryEnabled || !enableOnly)
                .Where(o => context.ContractSettings.Attachments()
                    || o.ControlType != "Attachments")
                .OrderBy(o => o.HistoryColumn);
        }

        public static IEnumerable<ColumnDefinition> MonitorChangesDefinitions(
            this Dictionary<string, ColumnDefinition> definitions)
        {
            return definitions.Values
                .Where(o => o.EditorColumn > 0 || o.ColumnName == "Comments")
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
            Context context,
            SiteSettings ss,
            IEnumerable<string> columns,
            string labelType = null,
            List<string> order = null)
        {
            return columns
                .Distinct()
                .ToDictionary(
                    columnName => columnName,
                    columnName => SelectableOptionsControlData(
                        context: context,
                        ss: ss?.GetJoinedSs(columnName),
                        columnName: columnName,
                        labelType: labelType,
                        order: order?.IndexOf(columnName)));
        }

        public static Dictionary<string, ControlData> SelectableSourceOptions(
            Context context,
            SiteSettings ss,
            IEnumerable<string> columns,
            string labelType = null,
            List<string> order = null)
        {
            return columns.ToDictionary(
                columnName => columnName,
                columnName => SelectableOptionsControlData(
                    context: context,
                    ss: ss,
                    columnName: columnName,
                    labelType: labelType,
                    order: order?.IndexOf(columnName)));
        }

        private static ControlData SelectableOptionsControlData(
            Context context,
            SiteSettings ss,
            string columnName,
            string labelType,
            int? order = null)
        {
            if (ss == null)
            {
                return new ControlData(string.Empty);
            }
            var column = ss.GetColumn(
                context: context,
                columnName: columnName.Split(',').Last());
            if (column != null)
            {
                var labelText = column.LabelText;
                var labelTextDefault = column.LabelTextDefault;
                switch (labelType)
                {
                    case "Grid":
                        labelTextDefault += $" ({labelText})";
                        labelText = column.GridLabelText;
                        break;
                }
                return new ControlData(
                    text: $"[{ss.Title}] " + Displays.Get(
                        context: context,
                        id: labelText),
                    title: labelTextDefault,
                    order: order);
            }
            else
            {
                var linkId = ss.LinkId(columnName);
                var sectionId = ss.SectionId(columnName);
                return new ControlData(linkId > 0
                    ? ss.Sources.Get(linkId)?.Title
                        ?? ss.Destinations.Get(linkId)?.Title
                        ?? string.Empty
                    : sectionId > 0
                        ? ss.Sections.FirstOrDefault(o => o.Id == sectionId)?.LabelText
                        : string.Empty);
            }
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

        public static long GetSiteIdByTableAlias(string tableAlias, long siteId)
        {
            return tableAlias.IsNullOrEmpty()
                ? siteId
                : GetSiteIdByTableAlias(tableAlias);
        }

        public static long GetSiteIdByTableAlias(string tableAlias)
        {
            return tableAlias.Split('-').Last().Split('~').Last().ToLong();
        }

        public static SqlColumnCollection SqlColumnCollection(
            Context context, SiteSettings ss, List<Column> columns)
        {
            return new SqlColumnCollection(Columns(
                context: context,
                ss: ss,
                columns: columns)
                    .SelectMany(column => column.SqlColumnCollection())
                    .GroupBy(o => o.ColumnBracket + o.As)
                    .Select(o => o.First())
                    .ToArray());
        }

        private static List<Column> Columns(
            Context context, SiteSettings ss, List<Column> columns)
        {
            columns
                .GroupBy(o => o.TableAlias)
                .Select(o => o.First())
                .ToList()
                .ForEach(o => AddDefaultColumns(
                    context: context,
                    ss: ss,
                    currentSs: o.SiteSettings,
                    tableAlias: o.Joined
                        ? o.TableAlias
                        : string.Empty,
                    columns: columns));
            columns = columns
                .Concat(ss.IncludedColumns().Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName)))
                .ToList();
            return columns
                .Where(o => o != null)
                .GroupBy(o => o.ColumnName)
                .Select(o => o.First())
                .AllowedColumns(checkPermission: true)
                .ToList();
        }

        private static void AddDefaultColumns(
            Context context,
            SiteSettings ss,
            SiteSettings currentSs,
            string tableAlias,
            List<Column> columns)
        {
            var idColumn = Rds.IdColumn(currentSs.ReferenceType);
            if (currentSs.ColumnHash.ContainsKey(idColumn))
            {
                columns.Add(GetColumn(
                    context: context,
                    ss: ss,
                    tableAlias: tableAlias,
                    columnName: idColumn));
            }
            if (currentSs.ColumnHash.ContainsKey("SiteId"))
            {
                columns.Add(GetColumn(
                    context: context,
                    ss: ss,
                    tableAlias: tableAlias,
                    columnName: "SiteId"));
            }
            if (currentSs.ColumnHash.ContainsKey("Locked"))
            {
                columns.Add(GetColumn(
                    context: context,
                    ss: ss,
                    tableAlias: tableAlias,
                    columnName: "Locked"));
            }
            currentSs.TitleColumns
                .Where(o => currentSs.ColumnHash.ContainsKey(o))
                .ForEach(name =>
                    columns.Add(GetColumn(
                        context: context,
                        ss: ss,
                        tableAlias: tableAlias,
                        columnName: name)));
            currentSs.Links
                .Where(link => columns.Any(p =>
                    (!tableAlias.IsNullOrEmpty()
                        ? tableAlias + ","
                        : string.Empty) + link.ColumnName == p?.ColumnName))
                .ForEach(link =>
                    columns.Add(GetColumn(
                        context: context,
                        ss: ss,
                        tableAlias: (!tableAlias.IsNullOrEmpty()
                            ? tableAlias + "-"
                            : string.Empty)
                                + link.LinkedTableName(),
                        columnName: "Title")));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: Rds.IdColumn(currentSs.ReferenceType)));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: "Creator"));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: "Updator"));
        }

        private static Column GetColumn(
            Context context, SiteSettings ss, string tableAlias, string columnName)
        {
            return ss.GetColumn(
                context: context,
                columnName: ColumnName(
                    tableAlias: tableAlias,
                    columnName: columnName));
        }

        public static string ColumnName(string tableAlias, string columnName)
        {
            return !tableAlias.IsNullOrEmpty()
                ? tableAlias + "," + columnName
                : columnName;
        }
    }
}