using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
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
            Context context,
            SiteSettings ss,
            Column groupBy,
            string aggregationType,
            Column value,
            IEnumerable<DataRow> dataRows,
            bool inRange)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "TimeSeriesGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(context: context),
                    optionCollection: ss.TimeSeriesGroupByOptions(),
                    selectedValue: groupBy?.ColumnName,
                    addSelectedValue: false,
                    method: "post")
                .FieldDropDown(
                    context: context,
                    controlId: "TimeSeriesAggregateType",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationType(context: context),
                    optionCollection: ss.TimeSeriesAggregationTypeOptions(context: context),
                    selectedValue: aggregationType,
                    addSelectedValue: false,
                    method: "post")
                .FieldDropDown(
                    context: context,
                    fieldId: "TimeSeriesValueField",
                    controlId: "TimeSeriesValue",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.AggregationTarget(context: context),
                    optionCollection: ss.TimeSeriesValueOptions(),
                    selectedValue: value?.ColumnName,
                    addSelectedValue: false,
                    method: "post")
                .Div(id: "TimeSeriesBody", action: () => hb
                    .TimeSeriesBody(
                        context: context,
                        ss: ss,
                        groupBy: groupBy,
                        aggregationType: aggregationType,
                        value: value,
                        dataRows: dataRows,
                        inRange: inRange)));
        }

        public static HtmlBuilder TimeSeriesBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column groupBy,
            string aggregationType,
            Column value,
            IEnumerable<DataRow> dataRows,
            bool inRange)
        {
            if (inRange && dataRows != null && dataRows.Any())
            {
                var timeSeries = new TimeSeries(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    dataRows: dataRows);
                return hb
                    .Svg(id: "TimeSeries")
                    .Hidden(
                        controlId: "TimeSeriesJson",
                        value: timeSeries.Json(
                            context: context,
                            groupBy: groupBy,
                            value: value));
            }
            else
            {
                return hb;
            }
        }
    }
}