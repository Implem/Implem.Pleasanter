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
                    optionCollection: ss.ColumnCollection.Where(o => o.HasChoices())
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
                    selectedValue: groupBy,
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
                    fieldId: "TimeSeriesValueField",
                    controlId: "TimeSeriesValue",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.SettingAggregationTarget(),
                    optionCollection: ss.ColumnCollection
                        .Where(o => o.Computable)
                        .Where(o => o.TypeName != "datetime")
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
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