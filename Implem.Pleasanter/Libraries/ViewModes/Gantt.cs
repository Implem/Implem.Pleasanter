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
            var completionTime = ss.GetColumn("CompletionTime");
            var workValue = ss.GetColumn("WorkValue");
            var progressRate = ss.GetColumn("ProgressRate");
            dataRows.ForEach(dataRow =>
                Add(new GanttElement(
                    groupBy: groupBy != null
                        ? dataRow.String(groupBy.ColumnName)
                        : string.Empty,
                    sortBy: sortBy != null
                        ? dataRow.Object(sortBy.ColumnName)
                        : string.Empty,
                    id: dataRow.Long(Rds.IdColumn(ss.ReferenceType)),
                    title: dataRow.String("ItemTitle"),
                    workValue: dataRow.Decimal("WorkValue"),
                    startTime: dataRow.DateTime("StartTime"),
                    completionTime: dataRow.DateTime("CompletionTime"),
                    progressRate: dataRow.Decimal("ProgressRate"),
                    status: dataRow.Int("Status"),
                    owner: dataRow.Int("Owner"),
                    updatorId: dataRow.Int("Updator"),
                    createdTime: dataRow.DateTime("CreatedTime"),
                    updatedTime: dataRow.DateTime("UpdatedTime"),
                    statusColumn: status,
                    completionTimeColumn: completionTime,
                    workValueColumn: workValue,
                    progressRateColumn: progressRate,
                    showProgressRate: ss.ShowGanttProgressRate.ToBool())));
            AddSummary(
                ss: ss,
                dataRows: dataRows,
                groupBy: groupBy,
                status: status,
                completionTime: completionTime,
                workValue: workValue,
                progressRate: progressRate);
        }

        private void AddSummary(
            SiteSettings ss,
            IEnumerable<DataRow> dataRows,
            Column groupBy,
            Column status,
            Column completionTime,
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
                            ss: ss,
                            groupBy: choice.Key,
                            title: choice.Value.Text,
                            status: status,
                            completionTime: completionTime,
                            workValue: workValue,
                            progressRate: progressRate,
                            dataRows: groupDataRows);
                    }
                });
            }
            else
            {
                if (dataRows.Any())
                {
                    groupCount++;
                    AddSummary(
                        ss: ss,
                        groupBy: string.Empty,
                        title: string.Empty,
                        status: status,
                        completionTime: completionTime,
                        workValue: workValue,
                        progressRate: progressRate,
                        dataRows: dataRows);
                }
            }
        }

        private void AddSummary(
            SiteSettings ss,
            string groupBy,
            string title,
            Column status,
            Column completionTime,
            Column workValue,
            Column progressRate,
            IEnumerable<DataRow> dataRows)
        {
            var workValueData = dataRows.Sum(o => o.Decimal("WorkValue"));
            Add(new GanttElement(
                groupBy: groupBy,
                sortBy: string.Empty,
                id: 0,
                title: Displays.Total() + ": " + title,
                workValue: workValueData,
                startTime: dataRows.Min(o => o.DateTime("StartTime")),
                completionTime: dataRows.Max(o => o.DateTime("CompletionTime")),
                progressRate: workValueData != 0
                    ? dataRows.Sum(o =>
                        o.Decimal("WorkValue") *
                        o.Decimal("ProgressRate")) /
                        workValueData
                    : 0,
                status: dataRows.Select(o => o.Int("Status")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Status")
                    : 0,
                owner: dataRows.Select(o => o.Int("Owner")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Owner")
                    : 0,
                updatorId: dataRows.Select(o => o.Int("Updator")).AllEqual()
                    ? dataRows.FirstOrDefault().Int("Updator")
                    : 0,
                createdTime: dataRows.Min(o => o.DateTime("CreatedTime")),
                updatedTime: dataRows.Max(o => o.DateTime("UpdatedTime")),
                statusColumn: status,
                completionTimeColumn: completionTime,
                workValueColumn: workValue,
                progressRateColumn: progressRate,
                showProgressRate: ss.ShowGanttProgressRate.ToBool(),
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
