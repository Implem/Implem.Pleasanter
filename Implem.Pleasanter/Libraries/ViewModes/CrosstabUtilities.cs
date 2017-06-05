using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public static class CrosstabUtilities
    {
        public static string DateGroup(SiteSettings ss, Column column, string timePeriod)
        {
            var columnBracket = ColumnBracket(ss, column.ColumnName);
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

        public static Rds.ResultsWhereCollection Where(
            SiteSettings ss, string columnName, string timePeriod, DateTime month)
        {
            string columnBracket = ColumnBracket(ss, columnName);
            switch (timePeriod)
            {
                case "Monthly":
                    return new Rds.ResultsWhereCollection()
                        .Add(new string[]
                        {
                            "{0} between '{1}' and '{2}'".Params(
                                columnBracket,
                                month.AddMonths(-11).ToUniversal(),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null);
                case "Weekly":
                    var end = WeeklyEndDate(month);
                    return new Rds.ResultsWhereCollection()
                        .Add(new string[]
                        {
                            "{0} between '{1}' and '{2}'".Params(
                                columnBracket,
                                end.AddDays(-77).ToUniversal(),
                                end.AddDays(7).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null);
                case "Daily":
                    return new Rds.ResultsWhereCollection()
                        .Add(new string[]
                        {
                            "{0} between '{1}' and '{2}'".Params(
                                columnBracket,
                                month.ToUniversal(),
                                month.AddMonths(1).AddMilliseconds(-3).ToUniversal())
                        }, _operator: null);
                default: return null;
            }
        }

        private static string ColumnBracket(SiteSettings ss, string columnName)
        {
            var columnBracket = "[{0}].[{1}]".Params(ss.ReferenceType, columnName);
            var now = DateTime.Now;
            var diff = (now.ToLocal() - now).Hours;
            switch (columnName)
            {
                case "CompletionTime":
                    diff -= 24;
                    break;
            }
            if (diff != 0)
            {
                columnBracket = "dateadd(hour,{0},{1})".Params(diff, columnBracket);
            }
            return columnBracket;
        }
    }
}