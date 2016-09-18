using Implem.Pleasanter.Libraries.DataViews;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTimeSeries
    {
        public static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string groupByColumn,
            string aggregateType,
            string valueColumn,
            Permissions.Types permissionType,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "TimeSeriesGroupByColumn",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: siteSettings.ColumnCollection.Where(o => o.HasChoices())
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
                    selectedValue: groupByColumn,
                    method: "post")
                .FieldDropDown(
                    controlId: "TimeSeriesAggregateType",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.SettingAggregationType(),
                    optionCollection: new Dictionary<string, string>
                    {
                        { "Count", Displays.Count() },
                        { "Total", Displays.Total() },
                        { "Average", Displays.Average() },
                        { "Max", Displays.Max() },
                        { "Min", Displays.Min() }
                    },
                    selectedValue: aggregateType,
                    method: "post")
                .FieldDropDown(
                    fieldId: "TimeSeriesValueColumnField",
                    controlId: "TimeSeriesValueColumn",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.SettingAggregationTarget(),
                    optionCollection: siteSettings.ColumnCollection
                        .Where(o => o.Computable)
                        .Where(o => o.TypeName != "datetime")
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
                    selectedValue: valueColumn,
                    method: "post")
                .Div(id: "TimeSeriesBody", action: () => hb
                    .TimeSeriesBody(
                        siteSettings: siteSettings,
                        groupByColumn: groupByColumn,
                        aggregateType: aggregateType,
                        valueColumn: valueColumn,
                        dataRows: dataRows))
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    importButton: true,
                    exportButton: true));
        }

        public static HtmlBuilder TimeSeriesBody(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string groupByColumn,
            string aggregateType,
            string valueColumn,
            IEnumerable<DataRow> dataRows)
        {
            if (dataRows != null && dataRows.Any())
            {
                var timeSeries = new TimeSeries(
                    siteSettings, groupByColumn, aggregateType, valueColumn, dataRows);
                return hb
                    .Svg(
                        id: "TimeSeries",
                        css: "time-series",
                        action: () => { })
                    .Hidden(
                        controlId: "TimeSeriesJson",
                        value: timeSeries.Json());
            }
            else
            {
                return hb;
            }
        }
    }
}