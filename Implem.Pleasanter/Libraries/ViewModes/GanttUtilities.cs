using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
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

        public static Rds.IssuesWhereCollection Where(SiteSettings ss, View view)
        {
            var start = view.GanttStartDate.ToDateTime().ToUniversal();
            var end = start.AddDays(view.GanttPeriod.ToInt()).AddMilliseconds(-3);
            return Rds.IssuesWhere()
                .Or(Rds.IssuesWhere()
                    .Add(raw: "(({0}) <= '{1}' and {2} >= '{3}')".Params(
                        Def.Sql.StartTimeColumn, start, CompletionTimeSql(ss), end))
                    .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                        Def.Sql.StartTimeColumn, start, end))
                    .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                        CompletionTimeSql(ss), start, end)));
        }

        private static string CompletionTimeSql(SiteSettings ss)
        {
            return Def.Sql.CompletionTimeColumn.Replace(
                "#DifferenceOfDates#",
                TimeExtensions.DifferenceOfDates(
                    ss.GetColumn("CompletionTime")?.EditorFormat, minus: true).ToString());
        }
    }
}