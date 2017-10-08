using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
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
                view.GanttPeriod = Period;
            }
            else
            {
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
                    join: ss.Join(),
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
    }
}