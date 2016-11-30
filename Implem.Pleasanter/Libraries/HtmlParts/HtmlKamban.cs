using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ViewModes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlKamban
    {
        public static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string aggregateType,
            string value,
            Permissions.Types pt,
            IEnumerable<KambanElement> data)
        {
            return hb.Div(id: "Kamban", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "KambanGroupBy",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupBy(),
                        optionCollection: ss.ColumnCollection.Where(o => o.HasChoices())
                            .ToDictionary(o => o.ColumnName, o => o.GridLabelText),
                        selectedValue: groupBy,
                        method: "post")
                    .FieldDropDown(
                        controlId: "KambanAggregateType",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.SettingAggregationType(),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Total", Displays.Total() },
                            { "Average", Displays.Average() },
                            { "Max", Displays.Max() },
                            { "Min", Displays.Min() }
                        },
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "KambanValueField",
                        controlId: "KambanValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.SettingAggregationTarget(),
                        optionCollection: KambanValueOptionCollection(ss),
                        selectedValue: value,
                        method: "post")
                    .KambanBody(
                        ss: ss,
                        groupBy: ss.GetColumn(groupBy),
                        aggregateType: aggregateType,
                        value: ss.GetColumn(value),
                        data: data)
                    .MainCommands(
                        siteId: ss.SiteId,
                        pt: pt,
                        verType: Versions.VerTypes.Latest,
                        importButton: true,
                        exportButton: true);
            });
        }

        private static Dictionary<string, string> KambanValueOptionCollection(
            SiteSettings ss)
        {
            return new Dictionary<string, string>
            {
                { string.Empty, string.Empty }
            }.Concat(ss.ColumnCollection
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .ToList()
                .ToDictionary(o => o.ColumnName, o => o.GridLabelText))
                .ToDictionary(o => o.Key, o => o.Value);
        }

        public static HtmlBuilder KambanBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column groupBy,
            string aggregateType,
            Column value,
            IEnumerable<KambanElement> data,
            long changedItemId = 0)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("KambanBody")
                    .DataAction("UpdateByKamban")
                    .DataMethod("post"),
                action: () => groupBy.EditChoices(
                    insertBlank: groupBy.Nullable)
                        .Chunk(Parameters.General.KambanChunk)
                        .ForEach(choices => hb
                            .Table(
                                ss: ss,
                                choices: CorrectedChoices(groupBy, choices),
                                aggregateType: aggregateType,
                                value: value,
                                data: data,
                                changedItemId: changedItemId)));
        }

        private static IEnumerable<KeyValuePair<string, ControlData>> CorrectedChoices(
            Column groupBy, IEnumerable<KeyValuePair<string, ControlData>> choices)
        {
            return groupBy.TypeName.CsTypeSummary() != Types.CsNumeric
                ? choices
                : choices.ToDictionary(
                    o => o.Key != string.Empty ? o.Key : "0",
                    o => o.Value);
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            SiteSettings ss,
            IEnumerable<KeyValuePair<string, ControlData>> choices,
            string aggregateType,
            Column value,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            return hb.Table(
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(css: "ui-widget-header", action: () => choices
                            .ForEach(choice => hb
                                .Th(action: () => hb
                                    .HeaderText(
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: data,
                                        choice: choice)))))
                    .TBody(action: () => hb
                        .Tr(css: "kamban-row", action: () => choices
                            .ForEach(choice => hb
                                .TBody(
                                    ss: ss,
                                    choice: choice,
                                    value: value,
                                    data: data,
                                    changedItemId: changedItemId)))));

        }

        private static HtmlBuilder TBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            KeyValuePair<string, ControlData> choice,
            Column value,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            return hb.Td(
                attributes: new HtmlAttributes()
                    .Class("kamban-container")
                    .DataValue(HttpUtility.HtmlEncode(choice.Key)),
                action: () => hb
                    .Div(action: () => 
                        data.Where(o => o.Group == choice.Key)
                            .ForEach(o => hb
                                .Element(
                                    ss: ss,
                                    value: value,
                                    data: o,
                                    changedItemId: changedItemId))));
        }

        private static HtmlBuilder HeaderText(
            this HtmlBuilder hb,
            SiteSettings ss,
            string aggregateType,
            Column value,
            IEnumerable<KambanElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            var targets = data.Where(o => o.Group == choice.Key);
            return hb.Text(text: "{0}({1}){2}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(),
                targets.Count(),
                value != null && targets.Any()
                    ? " : " + value.Display(Summary(targets, aggregateType), unit: true)
                    : string.Empty));
        }

        private static decimal Summary(IEnumerable<KambanElement> data, string aggregateType)
        {
            switch (aggregateType)
            {
                case "Total": return data.Sum(o => o.Value);
                case "Average": return data.Average(o => o.Value);
                case "Min": return data.Min(o => o.Value);
                case "Max": return data.Max(o => o.Value);
                default: return 0;
            }
        }

        private static HtmlBuilder Element(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column value,
            KambanElement data,
            long changedItemId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("kamban-item" + ItemChanged(data.Id, changedItemId))
                    .DataId(data.Id.ToString()),
                action: () => hb
                    .Span(css: "ui-icon ui-icon-pencil")
                    .Text(text: ItemText(value, data)));
        }

        private static string ItemText(Column value, KambanElement data)
        {
            return value == null
                ? data.Title
                : "{0} : {1}".Params(
                    data.Title,
                    value.Display(data.Value, unit: true));
        }

        private static string ItemChanged(long id, long changedItemId)
        {
            return id == changedItemId
                ? " changed"
                : string.Empty;
        }
    }
}