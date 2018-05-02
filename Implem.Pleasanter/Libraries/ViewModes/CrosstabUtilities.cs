using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
            View view, Column groupByX, Column groupByY, List<Column> columns)
        {
            var data = new List<string>() { groupByX?.ColumnName, groupByY?.ColumnName };
            if (view.CrosstabGroupByY == "Columns")
            {
                columns?.ForEach(o => data.Add(o.ColumnName));
            }
            return data;
        }

        public static bool InRangeX(EnumerableRowCollection<DataRow> dataRows)
        {
            var inRange = dataRows.Select(o => o.String("GroupByX")).Distinct().Count() <=
                Parameters.General.CrosstabXLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.CrosstabXLimit.ToString()));
            }
            return inRange;
        }

        public static bool InRangeY(EnumerableRowCollection<DataRow> dataRows)
        {
            var inRange = dataRows.Select(o => o.String("GroupByY")).Distinct().Count() <=
                Parameters.General.CrosstabYLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.CrosstabYLimit.ToString()));
            }
            return inRange;
        }

        public static string DateGroup(SiteSettings ss, Column column, string timePeriod)
        {
            var columnBracket = ColumnBracket(column);
            switch (timePeriod)
            {
                case "Monthly":
                    return "substring(convert(varchar,{0},111),1,7)".Params(columnBracket);
                case "Weekly":
                    var part = "case datepart(weekday,{0}) when 1 then dateadd(day,-6,{0}) else dateadd(day,(2-datepart(weekday,{0})),{0}) end".Params(
                        columnBracket);
                    return "datepart(year,{0}) * 100 + datepart(week,{0})".Params(part);
                case "Daily":
                    return "convert(varchar,{0},111)".Params(columnBracket);
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
            SiteSettings ss, Column column, string timePeriod, DateTime month)
        {
            switch (timePeriod)
            {
                case "Monthly":
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "[{0}] between '{1}' and '{2}'".Params(
                                column.Name,
                                month.AddMonths(-11).ToUniversal(),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null));
                case "Weekly":
                    var end = WeeklyEndDate(month);
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "[{0}] between '{1}' and '{2}'".Params(
                                column.Name,
                                end.AddDays(-77).ToUniversal(),
                                end.AddDays(7).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null));
                case "Daily":
                    return new SqlWhereCollection(new SqlWhere(
                        tableName: column.TableName(),
                        columnBrackets: new string[]
                        {
                            "[{0}] between '{1}' and '{2}'".Params(
                                column.Name,
                                month.ToUniversal(),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null));
                default: return null;
            }
        }

        private static string ColumnBracket(Column column)
        {
            var columnBracket = "[{0}].[{1}]".Params(column.TableName(), column.Name);
            var diff = Diff(column);
            if (diff != 0)
            {
                columnBracket = $"dateadd(hour,{diff},{columnBracket})";
            }
            return columnBracket;
        }

        private static int Diff(Column column)
        {
            var now = DateTime.Now.ToLocal();
            switch (column.Name)
            {
                case "CompletionTime":
                    now = now.AddDifferenceOfDates(column.EditorFormat, minus: true);
                    break;
            }
            return (now - now).Hours;
        }

        public static List<Column> GetColumns(SiteSettings ss, List<Column> columns)
        {
            return columns?.Any() == true
                ? columns
                : ss.CrosstabColumnsOptions().Select(o => ss.GetColumn(o.Key)).ToList();
        }

        public static string Csv(
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
                    ss: ss,
                    view: view,
                    choicesX: ChoicesX(groupByX, timePeriod, month),
                    choicesY: ChoicesY(groupByY),
                    aggregateType: aggregateType,
                    value: value,
                    firstHeaderText: $"{groupByY.LabelText} | {groupByX.LabelText}",
                    timePeriod: timePeriod,
                    month: month,
                    data: Elements(groupByX, groupByY, dataRows));
            }
            else
            {
                var columnList = GetColumns(ss, columns);
                csv.Body(
                    ss: ss,
                    view: view,
                    choicesX: ChoicesX(groupByX, timePeriod, month),
                    choicesY: ChoicesY(columnList),
                    aggregateType: aggregateType,
                    value: value,
                    firstHeaderText: groupByX.LabelText,
                    timePeriod: timePeriod,
                    month: month,
                    data: ColumnsElements(groupByX, dataRows, columnList),
                    columnList: columnList);
            }
            return csv.ToString();
        }

        private static void Body(
            this StringBuilder csv,
            SiteSettings ss,
            View view,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            string firstHeaderText,
            string timePeriod,
            DateTime month,
            List<CrosstabElement> data,
            IEnumerable<Column> columnList = null)
        {
            var headers = new List<string>()
            {
                firstHeaderText
            };
            headers.AddRange(choicesX.Select(o => o.Value.DisplayValue()));
            csv.AppendRow(headers);
            choicesY?.ForEach(choiceY =>
            {
                var cells = new List<string>()
                {
                    choiceY.Value.DisplayValue()
                };
                var column = columnList?.Any() != true
                    ? value
                    : ss.GetColumn(choiceY.Key);
                var row = data.Where(o => o.GroupByY == choiceY.Key).ToList();
                cells.AddRange(choicesX
                    .Select(choiceX => CellText(
                        column, aggregateType, CellValue(
                            data, choiceX, choiceY))));
                csv.AppendRow(cells);
            });
        }

        public static List<CrosstabElement> Elements(
            Column groupByX, Column groupByY, EnumerableRowCollection<DataRow> dataRows)
        {
            return dataRows
                .Select(o => new CrosstabElement(
                    o.String(groupByX.ColumnName),
                    o.String(groupByY.ColumnName),
                    o.Decimal("Value")))
                .ToList();
        }

        public static List<CrosstabElement> ColumnsElements(
            Column groupByX, EnumerableRowCollection<DataRow> dataRows, List<Column> columnList)
        {
            var data = new List<CrosstabElement>();
            dataRows.ForEach(o =>
                columnList.ForEach(column =>
                    data.Add(new CrosstabElement(
                        o.String(groupByX.ColumnName),
                        column.ColumnName,
                        o.Decimal(column.ColumnName)))));
            return data;
        }

        public static Dictionary<string, ControlData> ChoicesX(
            Column groupByX, string timePeriod, DateTime month)
        {
            return groupByX?.TypeName == "datetime"
                ? CorrectedChoices(groupByX, timePeriod, month)
                : groupByX.ChoiceHash?.ToDictionary(
                    o => o.Key,
                    o => new ControlData(o.Value.Text));
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Column groupBy, string timePeriod, DateTime date)
        {
            switch (timePeriod)
            {
                case "Monthly": return Monthly(date);
                case "Weekly": return Weekly(date);
                case "Daily": return Daily(date);
                default: return null;
            }
        }

        private static Dictionary<string, ControlData> Monthly(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            for (var i = -11; i <= 0; i++)
            {
                var day = date.AddMonths(i);
                hash.Add(day.ToString("yyyy/MM"), new ControlData(day.ToString("yyyy/MM")));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Weekly(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            var end = CrosstabUtilities.WeeklyEndDate(date);
            for (var i = -77; i <= 0; i += 7)
            {
                var day = end.AddDays(i);
                var append = (int)(new DateTime(day.Year, 1, 1).DayOfWeek) > 1
                    ? 8 - (int)(new DateTime(day.Year, 1, 1).DayOfWeek)
                    : 0;
                var key = day.Year * 100 + ((day.DayOfYear + append) / 7) + 1;
                hash.Add(key.ToString(), new ControlData(day.ToString("MM/dd")));
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

        public static Dictionary<string, ControlData> ChoicesY(Column groupByY)
        {
            return groupByY?.ChoiceHash?.ToDictionary(
                o => o.Key, o => new ControlData(o.Value.Text));
        }

        public static Dictionary<string, ControlData> ChoicesY(List<Column> columnList)
        {
            return columnList.ToDictionary(
                o => o.ColumnName, o => new ControlData(o?.LabelText));
        }

        public static decimal CellValue(
            IEnumerable<CrosstabElement> data,
            KeyValuePair<string, ControlData> choiceX,
            KeyValuePair<string, ControlData> choiceY)
        {
            return data.FirstOrDefault(o =>
                o.GroupByX == choiceX.Key &&
                o.GroupByY == choiceY.Key)?.Value ?? 0;
        }

        public static string CellText(Column value, string aggregateType, decimal data)
        {
            return value?.Display(
                data,
                unit: aggregateType != "Count",
                format: aggregateType != "Count") ?? data.ToString();
        }

        private static void AppendRow(this StringBuilder csv, List<string> cells)
        {
            csv.Append(cells.Select(o => "\"" + o + "\"").Join(","), "\n");
        }
    }
}