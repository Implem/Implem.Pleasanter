using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public static class CrosstabUtilities
    {
        public static List<string> JoinColumns(
            View view, Column groupByX, Column groupByY, List<Column> columns, Column value)
        {
            var data = new List<string>()
            {
                groupByX?.ColumnName,
                groupByY?.ColumnName,
                value?.ColumnName
            };
            if (view.CrosstabGroupByY == "Columns")
            {
                columns?.ForEach(o => data.Add(o.ColumnName));
            }
            return data;
        }

        public static bool InRangeX(Context context, EnumerableRowCollection<DataRow> dataRows)
        {
            var inRange = dataRows.Select(o => o.String("GroupByX")).Distinct().Count() <=
                Parameters.General.CrosstabXLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.CrosstabXLimit.ToString()));
            }
            return inRange;
        }

        public static bool InRangeY(Context context, EnumerableRowCollection<DataRow> dataRows)
        {
            var inRange = dataRows.Select(o => o.String("GroupByY")).Distinct().Count() <=
                Parameters.General.CrosstabYLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.CrosstabYLimit.ToString()));
            }
            return inRange;
        }

        public static string DateGroup(
            Context context, SiteSettings ss, Column column, string timePeriod)
        {
            var columnBracket = ColumnBracket(
                context: context,
                column: column);
            switch (timePeriod)
            {
                case "Yearly":
                    return context.Sqls.DateGroupYearly.Params(columnBracket);
                case "Monthly":
                    return context.Sqls.DateGroupMonthly.Params(columnBracket);
                case "Weekly":
                    var part = context.Sqls.DateGroupWeeklyPart.Params(
                        columnBracket);
                    return context.Sqls.DateGroupWeekly.Params(part);
                case "Daily":
                    return context.Sqls.DateGroupDaily.Params(columnBracket);
                default:
                    return null;
            }
        }

        public static DateTime WeeklyEndDate(DateTime month)
        {
            var end = month.AddMonths(1).AddDays(-1);
            var week = (int)end.DayOfWeek;
            return week < 1
                ? end.AddDays(-6)
                : end.AddDays((week - 1) * -1);
        }

        public static SqlWhereCollection Where(
            Context context, SiteSettings ss, Column column, string timePeriod, DateTime month)
        {
            switch (timePeriod)
            {
                case "Yearly":
                    var year = new DateTime(month.Year, 1, 1);
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "\"{0}\" between '{1}' and '{2:yyyy/MM/dd HH:mm:ss.fff}'".Params(
                                column.Name,
                                year.AddYears(-11).ToUniversal(context: context),
                                year.AddYears(1).AddMilliseconds(-3).ToUniversal(context: context))
                        }, _operator: null));
                case "Monthly":
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "\"{0}\" between '{1}' and '{2:yyyy/MM/dd HH:mm:ss.fff}'".Params(
                                column.Name,
                                month.AddMonths(-11).ToUniversal(context: context),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal(context: context))
                        }, _operator: null));
                case "Weekly":
                    var end = WeeklyEndDate(month);
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "\"{0}\" between '{1}' and '{2:yyyy/MM/dd HH:mm:ss.fff}'".Params(
                                column.Name,
                                end.AddDays(-77).ToUniversal(context: context),
                                end.AddDays(7).AddMilliseconds(-3).ToUniversal(context: context))
                        }, _operator: null));
                case "Daily":
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "\"{0}\" between '{1}' and '{2:yyyy/MM/dd HH:mm:ss.fff}'".Params(
                                column.Name,
                                month.ToUniversal(context: context),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal(context: context))
                        }, _operator: null));
                default: return null;
            }
        }

        private static string ColumnBracket(Context context, Column column)
        {
            var columnBracket = "\"{0}\".\"{1}\"".Params(column.TableName(), column.Name);
            var diff = Diff(
                context: context,
                column: column);
            if (diff != 0)
            {
                columnBracket = context.Sqls.DateAddHour(diff,columnBracket);
            }
            return columnBracket;
        }

        private static int Diff(Context context, Column column)
        {
            var now = DateTime.Now.ToLocal(context: context);
            switch (column.Name)
            {
                case "CompletionTime":
                    return Diff(now.AddDifferenceOfDates(
                        format: column.EditorFormat,
                        minus: true));
                default:
                    return Diff(now);
            }
        }

        private static int Diff(DateTime from)
        {
            return (from - DateTime.Now).TotalHours.ToInt();
        }

        public static List<Column> GetColumns(
            Context context, SiteSettings ss, List<Column> columns)
        {
            return columns?.Any() == true
                ? columns
                    .Where(column => column != null)
                    .ToList()
                : ss.CrosstabColumnsOptions(context: context)
                    .Select(o => ss.GetColumn(
                        context: context,
                        columnName: o.Key))
                    .Where(column => column != null)
                    .ToList();
        }

        public static string Csv(
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            List<Column> columns,
            string aggregateType,
            Column value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var csv = new StringBuilder();
            if (groupByY != null)
            {
                csv.Body(
                    context: context,
                    ss: ss,
                    choicesX: ChoicesX(
                        context: context,
                        groupByX: groupByX,
                        view: view,
                        timePeriod: timePeriod,
                        month: month),
                    choicesY: ChoicesY(
                        context: context,
                        groupByY: groupByY,
                        view: view),
                    aggregateType: aggregateType,
                    value: value,
                    firstHeaderText: $"{groupByY.LabelText} | {groupByX.LabelText}",
                    data: Elements(
                        groupByX: groupByX,
                        groupByY: groupByY,
                        dataRows: dataRows));
            }
            else
            {
                var columnList = GetColumns(
                    context: context,
                    ss: ss,
                    columns: columns);
                csv.Body(
                    context: context,
                    ss: ss,
                    choicesX: ChoicesX(
                        context: context,
                        groupByX: groupByX,
                        view: view,
                        timePeriod: timePeriod,
                        month: month),
                    choicesY: ChoicesY(columnList),
                    aggregateType: aggregateType,
                    value: value,
                    firstHeaderText: groupByX.LabelText,
                    data: ColumnsElements(
                        groupByX: groupByX,
                        dataRows: dataRows,
                        columnList: columnList),
                    columnList: columnList);
            }
            return csv.ToString();
        }

        private static void Body(
            this StringBuilder csv,
            Context context,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            string firstHeaderText,
            Dictionary<string, CrosstabElement> data,
            IEnumerable<Column> columnList = null)
        {
            var headers = new List<string>()
            {
                firstHeaderText
            };
            headers.AddRange(choicesX.Select(o => o.Value.DisplayValue(context: context)));
            csv.AppendRow(headers);
            choicesY?.ForEach(choiceY =>
            {
                var cells = new List<string>()
                {
                    choiceY.Value.DisplayValue(context: context)
                };
                var column = columnList?.Any() != true
                    ? value
                    : ss.GetColumn(context: context, columnName: choiceY.Key);
                cells.AddRange(choicesX
                    .Select(choiceX => CellText(
                        context: context,
                        value: column,
                        aggregateType: aggregateType,
                        data: CellValue(
                            data: data,
                            choiceX: choiceX.Key,
                            choiceY: choiceY.Key))));
                csv.AppendRow(cells);
            });
        }

        public static Dictionary<string, CrosstabElement> Elements(
            Column groupByX, Column groupByY, EnumerableRowCollection<DataRow> dataRows)
        {
            var data = dataRows.Select(dataRow => new CrosstabElement(
                groupByX: dataRow.String(groupByX?.ColumnName ?? string.Empty),
                groupByY: dataRow.String(groupByY?.ColumnName ?? string.Empty),
                value: dataRow.Decimal("Value")));
            return ElementHash(data: data);
        }

        public static Dictionary<string, CrosstabElement> ColumnsElements(
            Column groupByX, EnumerableRowCollection<DataRow> dataRows, List<Column> columnList)
        {
            var data = new List<CrosstabElement>();
            dataRows.ForEach(dataRow =>
                columnList.ForEach(column =>
                    data.Add(new CrosstabElement(
                        groupByX: dataRow.String(groupByX?.ColumnName),
                        groupByY: column?.ColumnName,
                        value: dataRow.Decimal(column?.ColumnName)))));
            return ElementHash(data: data);
        }

        public static Dictionary<string, CrosstabElement> ElementHash(IEnumerable<CrosstabElement> data)
        {
            return data
                .Select(o => new
                {
                    Key = Key(
                        choiceX: o.GroupByX,
                        choiceY: o.GroupByY),
                    Value = o
                })
                .GroupBy(o => o.Key)
                .Select(o => o.First())
                .ToDictionary(o => o.Key, o => o.Value);
        }

        public static Dictionary<string, ControlData> ChoicesX(
            Context context, Column groupByX, View view, string timePeriod, DateTime month)
        {
            return groupByX?.TypeName == "datetime"
                ? CorrectedChoices(context, groupByX, timePeriod, month)
                : groupByX?.EditChoices(
                    context: context,
                    insertBlank: true,
                    checkBlankInSelection: true,
                    view: view,
                    limit: Parameters.General.CrosstabXLimit);
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Context context,Column groupBy, string timePeriod, DateTime date)
        {
            switch (timePeriod)
            {
                case "Yearly": return Yearly(date);
                case "Monthly": return Monthly(context,date);
                case "Weekly": return Weekly(context, date);
                case "Daily": return Daily(date);
                default: return null;
            }
        }

        private static Dictionary<string, ControlData> Yearly(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            for (var i = -11; i <= 0; i++)
            {
                var day = date.AddYears(i);
                hash.Add(day.ToString("yyyy"), new ControlData(day.ToString("yyyy")));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Monthly(Context context,DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            for (var i = -11; i <= 0; i++)
            {
                var day = date.AddMonths(i);
                hash.Add(day.ToString("yyyy/MM"),new ControlData(day.ToString(Displays.Get(
                    context: context,
                    id: "YmFormat"))));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Weekly(Context context, DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            var end = WeeklyEndDate(date);
            for (var i = -77; i <= 0; i += 7)
            {
                var day = end.AddDays(i);
                var append = (int)(new DateTime(day.Year, 1, 1).DayOfWeek) > 1
                    ? 8 - (int)(new DateTime(day.Year, 1, 1).DayOfWeek)
                    : 0;
                var key = day.Year * 100 + ((day.DayOfYear + append) / 7) + 1;
                hash.Add(key.ToString(), new ControlData(day.ToString(Displays.Get(
                    context: context,
                    id: "MdFormat"))));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Daily(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            var month = date.Month;
            while (month == date.Month)
            {
                hash.Add(date.ToString("yyyy/MM/dd"), new ControlData(date.ToString("dd")));
                date = date.AddDays(1);
            }
            return hash;
        }

        public static Dictionary<string, ControlData> ChoicesY(
            Context context, Column groupByY, View view)
        {
            return groupByY?.EditChoices(
                context: context,
                insertBlank: true,
                checkBlankInSelection: true,
                view: view,
                limit: Parameters.General.CrosstabYLimit);
        }

        public static Dictionary<string, ControlData> ChoicesY(List<Column> columnList)
        {
            return columnList.ToDictionary(
                o => o.ColumnName,
                o => new ControlData(o?.LabelText));
        }

        public static decimal CellValue(
            Dictionary<string, CrosstabElement> data,
            string choiceX,
            string choiceY)
        {
            return data.Get(Key(
                choiceX: choiceX,
                choiceY: choiceY))
                    ?.Value ?? 0;
        }

        public static string Key(string choiceX, string choiceY)
        {
            return $"{choiceX}|{choiceY}";
        }

        public static string CellText(
            Context context, Column value, string aggregateType, decimal data)
        {
            return value?.Display(
                context: context,
                value: data,
                unit: aggregateType != "Count",
                format: aggregateType != "Count") ?? data.ToString();
        }

        private static void AppendRow(this StringBuilder csv, List<string> cells)
        {
            csv.Append(cells.Select(o => "\"" + o + "\"").Join(","), "\n");
        }
    }
}