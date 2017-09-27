using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class Gantt : List<GanttElement>
    {
        public Column GroupBy;
        public Column SortBy;

        public Gantt(SiteSettings ss, IEnumerable<DataRow> dataRows, string groupBy, string sortBy)
        {
            GroupBy = ss.GetColumn(groupBy);
            SortBy = ss.GetColumn(sortBy);
            var status = ss.GetColumn("Status");
            var workValue = ss.GetColumn("WorkValue");
            var progressRate = ss.GetColumn("ProgressRate");
            dataRows.ForEach(dataRow =>
                Add(new GanttElement(
                    GroupBy != null
                        ? dataRow.String("GroupBy")
                        : string.Empty,
                    SortBy != null
                        ? dataRow.Object("SortBy")
                        : string.Empty,
                    dataRow.Long(Rds.IdColumn(ss.ReferenceType)),
                    new Title(ss, dataRow).DisplayValue,
                    dataRow.Decimal("WorkValue"),
                    dataRow.DateTime("StartTime"),
                    dataRow.DateTime("CompletionTime"),
                    dataRow.Decimal("ProgressRate"),
                    dataRow.Int("Status"),
                    dataRow.Int("Owner"),
                    dataRow.Int("Updator"),
                    dataRow.DateTime("CreatedTime"),
                    dataRow.DateTime("UpdatedTime"),
                    status,
                    workValue,
                    progressRate,
                    ss.ShowGanttProgressRate.ToBool())));
            AddSummary(ss, dataRows, status, workValue, progressRate);
        }

        private void AddSummary(
            SiteSettings ss,
            IEnumerable<DataRow> dataRows,
            Column status,
            Column workValue,
            Column progressRateColumn)
        {
            var groupCount = 0;
            if (GroupBy != null)
            {
                GroupBy.EditChoices(insertBlank: true).ForEach(choice =>
                {
                    var groupBy = dataRows.Where(o => o.String("GroupBy") == choice.Key);
                    if (groupBy.Any())
                    {
                        groupCount++;
                        AddSummary(
                            ss,
                            choice.Key,
                            choice.Value.Text,
                            status,
                            workValue,
                            progressRateColumn,
                            groupBy);
                    }
                });
            }
            else
            {
                if (dataRows.Any())
                {
                    groupCount++;
                    AddSummary(
                        ss,
                        string.Empty,
                        string.Empty,
                        status,
                        workValue,
                        progressRateColumn,
                        dataRows);
                }
            }
        }

        private void AddSummary(
            SiteSettings ss,
            string groupBy,
            string title,
            Column status,
            Column workValue,
            Column progressRate,
            IEnumerable<DataRow> dataRows)
        {
            var workValueData = dataRows.Sum(o => o.Decimal("WorkValue"));
            Add(new GanttElement(
                groupBy,
                string.Empty,
                0,
                Displays.Total() + ": " + title,
                workValueData,
                dataRows.Min(o => o.DateTime("StartTime")),
                dataRows.Max(o => o.DateTime("CompletionTime")),
                workValueData != 0
                    ? dataRows.Sum(o =>
                        o.Decimal("WorkValue") *
                        o.Decimal("ProgressRate")) /
                        workValueData
                    : 0,
                dataRows.Select(o => o.Int("Status")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Status")
                    : 0,
                dataRows.Select(o => o.Int("Owner")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Owner")
                    : 0,
                dataRows.Select(o => o.Int("Updator")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Updator")
                    : 0,
                dataRows.Min(o => o.DateTime("CreatedTime")),
                dataRows.Max(o => o.DateTime("UpdatedTime")),
                status,
                workValue,
                progressRate,
                ss.ShowGanttProgressRate.ToBool(),
                summary: true));
        }

        public string Json()
        {
            var choices = GroupBy?
                .EditChoices(insertBlank: true)
                .Select(o => o.Key)
                .ToList();
            return this
                .OrderBy(o => choices?.IndexOf(o.GroupBy))
                .ThenByDescending(o => o.GroupSummary)
                .Sort(SortBy)
                .ThenBy(o => o.StartTime)
                .ThenBy(o => o.CompletionTime)
                .ThenBy(o => o.Title)
                .ToList()
                .ToJson();
        }
    }
}
