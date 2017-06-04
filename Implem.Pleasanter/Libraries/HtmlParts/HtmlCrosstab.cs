using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCrosstab
    {
        public static HtmlBuilder Crosstab(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string aggregateType,
            string value,
            EnumerableRowCollection<DataRow> dataRows)
        {
            return hb.Div(id: "Crosstab", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "CrosstabGroupByX",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByX(),
                        optionCollection: ss.CrosstabGroupByXOptions(),
                        selectedValue: groupByX,
                        method: "post")
                    .FieldDropDown(
                        controlId: "CrosstabGroupByY",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByY(),
                        optionCollection: ss.CrosstabGroupByYOptions(),
                        selectedValue: groupByY,
                        method: "post")
                    .FieldDropDown(
                        controlId: "CrosstabAggregateType",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationType(),
                        optionCollection: ss.CrosstabAggregationTypeOptions(),
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "CrosstabValueField",
                        controlId: "CrosstabValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: ss.CrosstabValueOptions(),
                        selectedValue: value,
                        method: "post")
                    .CrosstabBody(
                        ss: ss,
                        view: view,
                        groupByX: groupByX,
                        groupByY: groupByY,
                        aggregateType: aggregateType,
                        value: value,
                        dataRows: dataRows);
            });
        }

        public static HtmlBuilder CrosstabBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string aggregateType,
            string value,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var data = dataRows.Select(o => new CrosstabElement(
                o["GroupByX"].ToString(),
                o["GroupByY"].ToString(),
                o["Value"].ToDecimal()));
            var xColumn = ss.GetColumn(groupByX);
            var yColumn = ss.GetColumn(groupByY);
            var choicesX = CorrectedChoices(
                xColumn, xColumn?.Choices(data.Select(o => o.GroupByX)));
            var choicesY = CorrectedChoices(
                yColumn, yColumn?.Choices(data.Select(o => o.GroupByY)));
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("CrosstabBody")
                    .DataAction("UpdateByCrosstab")
                    .DataMethod("post"),
                action: () =>
                {
                    if (data.Any())
                    {
                        hb.Table(
                            ss: ss,
                            choicesX: choicesX,
                            choicesY: choicesY,
                            aggregateType: aggregateType,
                            value: ss.GetColumn(value),
                            data: data);
                    }
                });
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Column groupBy, IEnumerable<KeyValuePair<string, ControlData>> choices)
        {
            return groupBy != null
                ? groupBy.TypeName.CsTypeSummary() != Types.CsNumeric
                    ? choices.ToDictionary(o => o.Key, o => o.Value)
                    : choices.ToDictionary(
                        o => o.Key != string.Empty ? o.Key : "0",
                        o => o.Value)
                : null;
        }

        private static Dictionary<string, ControlData> Choices(
            this Column column, IEnumerable<string> data)
        {
            return column
                .EditChoices(insertBlank: true)
                .Where(o => data?.Any() != true || data.Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            IEnumerable<CrosstabElement> data)
        {
            var max = data.Any()
                ? data.Select(o => o.Value).Max()
                : 0;
            return hb.Table(
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(css: "ui-widget-header", action: () =>
                        {
                            if (choicesY != null)
                            {
                                hb.Th();
                            }
                            choicesX.ForEach(choice => hb
                                .Th(action: () => hb
                                    .HeaderText(
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: data.Where(o => o.GroupByX == choice.Key),
                                        choice: choice)));
                        }))
                    .TBody(action: () =>
                    {
                        choicesY.ForEach(choiceY =>
                        {
                            hb.Tr(css: "crosstab-row", action: () =>
                            {
                                hb.Th(action: () => hb
                                    .HeaderText(
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: data.Where(o => o.GroupByY == choiceY.Key),
                                        choice: choiceY));
                                choicesX.ForEach(choiceX => hb
                                    .Td(ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        max: max,
                                        data: data.FirstOrDefault(o =>
                                            o.GroupByX == choiceX.Key &&
                                            o.GroupByY == choiceY.Key)?.Value ?? 0));
                            });
                        });
                    }));

        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column value,
            string aggregateType,
            decimal max,
            decimal data)
        {
            return hb.Td(action: () => hb
                .Text(text: value?.Display(
                    data, unit: aggregateType != "Count", format: aggregateType != "Count") ??
                        data.ToString())
                .Svg(css: "svg-crosstab", action: () => hb
                    .Rect(
                        x: 0,
                        y: 0,
                        width: max > 0
                            ? (data / max * 100).ToString("0.0") + "%"
                            : "0")));
        }

        private static HtmlBuilder HeaderText(
            this HtmlBuilder hb,
            SiteSettings ss,
            string aggregateType,
            Column value,
            IEnumerable<CrosstabElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            return hb.Text(text: "{0}({1}){2}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(),
                data.Count(),
                value != null && data.Any() && aggregateType != "Count"
                    ? " : " + value.Display(data.Summary(aggregateType), unit: true)
                    : string.Empty));
        }

        private static decimal Summary(
            this IEnumerable<CrosstabElement> data, string aggregateType)
        {
            if (data.Any())
            {
                switch (aggregateType)
                {
                    case "Count": return data.Count();
                    case "Total": return data.Sum(o => o.Value);
                    case "Average": return data.Average(o => o.Value);
                    case "Min": return data.Min(o => o.Value);
                    case "Max": return data.Max(o => o.Value);
                    default: return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}