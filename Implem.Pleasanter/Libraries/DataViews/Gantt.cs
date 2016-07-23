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
        public SiteSettings SiteSettings;
        public Column GroupByColumn;
        public int Height;

        public Gantt(
            SiteSettings siteSettings, IEnumerable<DataRow> dataRows, string groupByColumn)
        {
            SiteSettings = siteSettings;
            GroupByColumn = SiteSettings.AllColumn(groupByColumn);
            var statusColumn = SiteSettings.AllColumn("Status");
            var workValueColumn = SiteSettings.AllColumn("WorkValue");
            var progressRateColumn = SiteSettings.AllColumn("ProgressRate");
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
                GroupByColumn.EditChoices(SiteSettings.SiteId, insertBlank: true).ForEach(choice =>
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
            Height = 60 + (this.Count + (groupCount == 0 ? 0 : groupCount - 1)) * 25;
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
                DateTime.MinValue,
                DateTime.MinValue,
                statusColumn,
                workValueColumn,
                progressRateColumn,
                summary: true));
        }

        public string ChartJson()
        {
            var choices = GroupByColumn?
                .EditChoices(SiteSettings.InheritPermission, insertBlank: true)
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
