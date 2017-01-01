using Implem.Pleasanter.Libraries.ViewModes;
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
            SiteSettings ss,
            string groupBy,
            string aggregateType,
            string value,
            Permissions.Types pt,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "TimeSeriesGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: ss.TimeSeriesGroupByOptions(),
                    selectedValue: groupBy,
                    method: "post")
                .FieldDropDown(
                    controlId: "TimeSeriesAggregateType",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationType(),
                    optionCollection: ss.TimeSeriesAggregationTypeOptions(),
                    selectedValue: aggregateType,
                    method: "post")
                .FieldDropDown(
                    fieldId: "TimeSeriesValueField",
                    controlId: "TimeSeriesValue",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationTarget(),
                    optionCollection: ss.TimeSeriesValueOptions(),
                    selectedValue: value,
                    method: "post")
                .Div(id: "TimeSeriesBody", action: () => hb
                    .TimeSeriesBody(
                        ss: ss,
                        groupBy: groupBy,
                        aggregateType: aggregateType,
                        value: value,
                        dataRows: dataRows))
                .MainCommands(
                    siteId: ss.SiteId,
                    pt: pt,
                    verType: Versions.VerTypes.Latest,
                    importButton: true,
                    exportButton: true));
        }

        public static HtmlBuilder TimeSeriesBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string aggregateType,
            string value,
            IEnumerable<DataRow> dataRows)
        {
            if (dataRows != null && dataRows.Any())
            {
                var timeSeries = new TimeSeries(
                    ss, groupBy, aggregateType, value, dataRows);
                return hb
                    .Svg(id: "TimeSeries")
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