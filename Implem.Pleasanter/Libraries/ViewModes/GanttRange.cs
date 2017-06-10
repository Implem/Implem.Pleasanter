using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class GanttRange
    {
        public DateTime Min;
        public DateTime Max;
        public int Period;

        public GanttRange(SiteSettings ss, View view)
        {
            Set(ss, view);
            if (view.GanttPeriod == null)
            {
                if (view.GanttStartDate == null) view.GanttStartDate = Min;
                view.GanttPeriod = Period;
            }
            else if (Forms.ControlId().StartsWith("ViewFilters_"))
            {
                Min = view.GanttStartDate.ToDateTime();
                view.GanttPeriod = Period;
            }
            else
            {
                Min = view.GanttStartDate.ToDateTime();
                Max = Min.AddDays(Period);
                if (view.GanttPeriod.ToInt() > Period)
                {
                    view.GanttPeriod = Period;
                }
            }
        }

        private void Set(SiteSettings ss, View view)
        {
            var dataRow = Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .Add(
                            Def.Sql.StartTimeColumn,
                            _as: "StartTimeMin",
                            function: Sqls.Functions.Min)
                        .Add(
                            Def.Sql.StartTimeColumn,
                            _as: "StartTimeMax",
                            function: Sqls.Functions.Max)
                        .IssuesColumn(
                            "CompletionTime",
                            _as: "CompletionTimeMin",
                            function: Sqls.Functions.Min)
                        .IssuesColumn(
                            "CompletionTime",
                            _as: "CompletionTimeMax",
                            function: Sqls.Functions.Max),
                    where: view.Where(ss: ss)))
                        .AsEnumerable()
                        .FirstOrDefault();
            if (dataRow != null)
            {
                Min =
                    dataRow["StartTimeMin"].ToDateTime() <
                    dataRow["CompletionTimeMin"].ToDateTime()
                        ? dataRow["StartTimeMin"].ToDateTime()
                        : dataRow["CompletionTimeMin"].ToDateTime();
                Max =
                    dataRow["StartTimeMax"].ToDateTime() >
                    dataRow["CompletionTimeMax"].ToDateTime()
                        ? dataRow["StartTimeMax"].ToDateTime()
                        : dataRow["CompletionTimeMax"].ToDateTime();
                if (Min > Max)
                {
                    var work = Min;
                    Min = Max;
                    Max = Min;
                }
                Period = (Max - Min).Days;
                if (Period > Parameters.General.GanttPeriodMax)
                {
                    Period = Parameters.General.GanttPeriodMax;
                }
                if (Period < Parameters.General.GanttPeriodMin)
                {
                    Period = Parameters.General.GanttPeriodMin;
                }
            }
        }

        public Rds.IssuesWhereCollection Where()
        {
            var min = Min.ToUniversal();
            var max = Max.ToUniversal().AddMilliseconds(-3);
            return Rds.IssuesWhere()
                .Or(Rds.IssuesWhere()
                    .Add(raw: "(({0}) <= '{1}' and {2} >= '{3}')".Params(
                        Def.Sql.StartTimeColumn, min, Def.Sql.CompletionTimeColumn, max))
                    .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                        Def.Sql.StartTimeColumn, min, max))
                    .Add(raw: "({0}) between '{1}' and '{2}'".Params(
                        Def.Sql.CompletionTimeColumn, min, max)));
        }
    }
}