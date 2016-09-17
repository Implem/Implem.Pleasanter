using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataViews
{
    public class Gantt : List<GanttElement>
    {
        [NonSerialized]
        public SiteSettings SiteSettings;
        [NonSerialized]
        public Column GroupByColumn;

        public Gantt(
            SiteSettings siteSettings, IEnumerable<DataRow> dataRows, string groupByColumn)
        {
            SiteSettings = siteSettings;
            GroupByColumn = SiteSettings.GetColumn(groupByColumn);
            var statusColumn = SiteSettings.GetColumn("Status");
            var workValueColumn = SiteSettings.GetColumn("WorkValue");
            var progressRateColumn = SiteSettings.GetColumn("ProgressRate");
            var groupCount = 0;
            dataRows.ForEach(dataRow =>
                Add(new GanttElement(
                    GroupByColumn != null
                        ? dataRow["GroupBy"].ToString()
                        : string.Empty,
                    dataRow["Id"].ToLong(),
                    Titles.DisplayValue(SiteSettings, dataRow),
                    dataRow["WorkValue"].ToDecimal(),
                    dataRow["StartTime"].ToDateTime(),
                    dataRow["CompletionTime"].ToDateTime(),
                    dataRow["ProgressRate"].ToDecimal(),
                    dataRow["Status"].ToInt(),
                    dataRow["Owner"].ToInt(),
                    dataRow["Updator"].ToInt(),
                    dataRow["CreatedTime"].ToDateTime(),
                    dataRow["UpdatedTime"].ToDateTime(),
                    statusColumn,
                    workValueColumn,
                    progressRateColumn)));
            if (GroupByColumn != null)
            {
                GroupByColumn.EditChoices(insertBlank: true).ForEach(choice =>
                {
                    var groupBy = dataRows.Where(o => o["GroupBy"].ToString() == choice.Key);
                    if (groupBy.Any())
                    {
                        groupCount++;
                        AddSummary(
                            choice.Key,
                            choice.Value.Text,
                            statusColumn,
                            workValueColumn,
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
                        string.Empty,
                        string.Empty,
                        statusColumn,
                        workValueColumn,
                        progressRateColumn,
                        dataRows);
                }
            }
        }

        private void AddSummary(
            string groupBy,
            string title,
            Column statusColumn,
            Column workValueColumn,
            Column progressRateColumn,
            IEnumerable<DataRow> dataRows)
        {
            var workValue = dataRows.Sum(o => o["WorkValue"].ToDecimal());
            Add(new GanttElement(
                groupBy,
                0,
                Displays.Total() + ": " + title,
                workValue,
                dataRows.Min(o => o["StartTime"].ToDateTime()),
                dataRows.Max(o => o["CompletionTime"].ToDateTime()),
                workValue != 0
                    ? dataRows.Sum(o =>
                        o["WorkValue"].ToDecimal() *
                        o["ProgressRate"].ToDecimal()) /
                        workValue
                    : 0,
                dataRows.Select(o => o["Status"]).AllEqual()
                    ? dataRows.FirstOrDefault()["Status"].ToInt()
                    : 0,
                dataRows.Select(o => o["Owner"]).AllEqual()
                    ? dataRows.FirstOrDefault()["Owner"].ToInt()
                    : 0,
                dataRows.Select(o => o["Updator"]).AllEqual()
                    ? dataRows.FirstOrDefault()["Updator"].ToInt()
                    : 0,
                dataRows.Min(o => o["CreatedTime"].ToDateTime()),
                dataRows.Max(o => o["UpdatedTime"].ToDateTime()),
                statusColumn,
                workValueColumn,
                progressRateColumn,
                summary: true));
        }

        public string Json()
        {
            var choices = GroupByColumn?
                .EditChoices(insertBlank: true)
                .Select(o => o.Key)
                .ToList();
            return Jsons.ToJson(this
                .OrderBy(o => choices?.IndexOf(o.GroupBy))
                .ThenByDescending(o => o.GroupSummary)
                .ThenBy(o => o.StartTime)
                .ThenBy(o => o.CompletionTime)
                .ThenBy(o => o.Title));
        }
    }
}
