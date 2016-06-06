using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Items;
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
            return hb.Div(css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "TimeSeriesGroupByColumn",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupBy(),
                        optionCollection: siteSettings.ColumnCollection.Where(o => o.HasChoices())
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        selectedValue: groupByColumn,
                        action: "DataView",
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
                        action: "DataView",
                        method: "post")
                    .FieldDropDown(
                        fieldId: "TimeSeriesValueColumnField",
                        controlId: "TimeSeriesValueColumn",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.SettingAggregationTarget(),
                        optionCollection: siteSettings.ColumnCollection.Where(o => o.Computable)
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        selectedValue: valueColumn,
                        action: "DataView",
                        method: "post")
                    .Div(id: "TimeSeriesChart", action: () => hb
                        .TimeSeriesChart(
                            siteSettings: siteSettings,
                            groupByColumn: groupByColumn,
                            aggregateType: aggregateType,
                            dataRows: dataRows))
                    .MainCommands(
                        siteId: siteSettings.SiteId,
                        permissionType: permissionType,
                        verType: Versions.VerTypes.Latest,
                        backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                        importButton: true,
                        exportButton: true);
            });
        }

        public static HtmlBuilder TimeSeriesChart(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string groupByColumn,
            string aggregateType,
            IEnumerable<DataRow> dataRows)
        {
            if (dataRows != null && dataRows.Count() > 0)
            {
                var timeSeries = new TimeSeries(
                    siteSettings, groupByColumn, aggregateType, dataRows);
                return hb
                    .Svg(
                        attributes: new HtmlAttributes().Id_Css("TimeSeries", "time-series"),
                        action: () => { })
                    .Hidden(
                        controlId: "TimeSeriesJson",
                        value: timeSeries.ChartJson());
            }
            else
            {
                return hb;
            }
        }
    }
}