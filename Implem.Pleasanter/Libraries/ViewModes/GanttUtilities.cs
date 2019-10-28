using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Libraries.DataSources.SqlServer;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    public static class GanttUtilities
    {
        public static IOrderedEnumerable<GanttElement> Sort(
            this IOrderedEnumerable<GanttElement> self, Column column)
        {
            if (column != null)
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case "numeric": return self.ThenBy(o => o.SortBy?.ToDecimal());
                    case "DateTime": return self.ThenBy(o => o.SortBy?.ToDateTime());
                    case "string": return self.ThenBy(o => o.SortBy?.ToString());
                    default: return self;
                }
            }
            else
            {
                return self;
            }
        }

        public static SqlWhereCollection Where(
            Context context, SiteSettings ss, View view)
        {
            var start = view.GanttStartDate.ToDateTime().ToUniversal(context: context);
            var end = start.AddDays(view.GanttPeriod.ToInt()).AddMilliseconds(-3);
            return Rds.IssuesWhere().Add(or: Rds.IssuesWhere()
                .Add(raw: "(({0}) <= '{1}' and {2} >= '{3}')".Params(
                    Def.Sql.StartTimeColumn,
                    start,
                    CompletionTimeSql(context: context, ss: ss),
                    end))
                .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                    Def.Sql.StartTimeColumn,
                    start,
                    end))
                .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                    CompletionTimeSql(context: context, ss: ss),
                    start,
                    end)));
        }

        private static string CompletionTimeSql(Context context, SiteSettings ss)
        {
            return Def.Sql.CompletionTimeColumn.Replace(
                "#DifferenceOfDates#",
                TimeExtensions.DifferenceOfDates(
                    ss.GetColumn(context: context, columnName: "CompletionTime")?
                        .EditorFormat, minus: true).ToString());
        }
    }
}