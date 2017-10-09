using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class Gantt : List<GanttElement>
    {
        public Gantt(SiteSettings ss, IEnumerable<DataRow> dataRows, Column groupBy, Column sortBy)
        {
            var status = ss.GetColumn("Status");
            var workValue = ss.GetColumn("WorkValue");
            var progressRate = ss.GetColumn("ProgressRate");
            dataRows.ForEach(dataRow =>
                Add(new GanttElement(
                    groupBy != null
                        ? dataRow.String(groupBy.ColumnName)
                        : string.Empty,
                    sortBy != null
                        ? dataRow.Object(sortBy.ColumnName)
                        : string.Empty,
                    dataRow.Long(Rds.IdColumn(ss.ReferenceType)),
                    dataRow.String("ItemTitle"),
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
            AddSummary(
                ss: ss,
                dataRows: dataRows,
                groupBy: groupBy,
                status: status,
                workValue: workValue,
                progressRate: progressRate);
        }

        private void AddSummary(
            SiteSettings ss,
            IEnumerable<DataRow> dataRows,
            Column groupBy,
            Column status,
            Column workValue,
            Column progressRate)
        {
            var groupCount = 0;
            if (groupBy != null)
            {
                groupBy.ChoiceHash.ForEach(choice =>
                {
                    var groupDataRows = dataRows.Where(o =>
                        o.String(groupBy.ColumnName) == choice.Key);
                    if (groupDataRows.Any())
                    {
                        groupCount++;
                        AddSummary(
                            ss,
                            choice.Key,
                            choice.Value.Text,
                            status,
                            workValue,
                            progressRate,
                            groupDataRows);
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
                        progressRate,
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

        public string Json(Column groupBy, Column sortBy)
        {
            var choices = groupBy?
                .ChoiceHash
                .Select(o => o.Key)
                .ToList();
            return this
                .OrderBy(o => choices?.IndexOf(o.GroupBy))
                .ThenByDescending(o => o.GroupSummary)
                .Sort(sortBy)
                .ThenBy(o => o.StartTime)
                .ThenBy(o => o.CompletionTime)
                .ThenBy(o => o.Title)
                .ToList()
                .ToJson();
        }
    }
}
