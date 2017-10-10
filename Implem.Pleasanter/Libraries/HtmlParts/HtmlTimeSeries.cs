using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
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
            Column groupBy,
            string aggregateType,
            Column value,
            IEnumerable<DataRow> dataRows,
            bool inRange)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "TimeSeriesGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: ss.TimeSeriesGroupByOptions(),
                    selectedValue: groupBy?.ColumnName,
                    addSelectedValue: false,
                    method: "post")
                .FieldDropDown(
                    controlId: "TimeSeriesAggregateType",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationType(),
                    optionCollection: ss.TimeSeriesAggregationTypeOptions(),
                    selectedValue: aggregateType,
                    addSelectedValue: false,
                    method: "post")
                .FieldDropDown(
                    fieldId: "TimeSeriesValueField",
                    controlId: "TimeSeriesValue",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationTarget(),
                    optionCollection: ss.TimeSeriesValueOptions(),
                    selectedValue: value?.ColumnName,
                    addSelectedValue: false,
                    method: "post")
                .Div(id: "TimeSeriesBody", action: () => hb
                    .TimeSeriesBody(
                        ss: ss,
                        groupBy: groupBy,
                        aggregateType: aggregateType,
                        value: value,
                        dataRows: dataRows,
                        inRange: inRange)));
        }

        public static HtmlBuilder TimeSeriesBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column groupBy,
            string aggregateType,
            Column value,
            IEnumerable<DataRow> dataRows,
            bool inRange)
        {
            if (inRange && dataRows != null && dataRows.Any())
            {
                var timeSeries = new TimeSeries(
                    ss, groupBy, aggregateType, value, dataRows);
                return hb
                    .Svg(id: "TimeSeries")
                    .Hidden(
                        controlId: "TimeSeriesJson",
                        value: timeSeries.Json(groupBy, value));
            }
            else
            {
                return hb;
            }
        }
    }
}