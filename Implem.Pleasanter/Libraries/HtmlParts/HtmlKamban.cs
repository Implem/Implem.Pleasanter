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
            string groupByX,
            string groupByY,
            string aggregateType,
            string value,
            int? columns,
            Permissions.Types pt,
            IEnumerable<KambanElement> data)
        {
            return hb.Div(id: "Kamban", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "KambanGroupByX",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByX(),
                        optionCollection: ss.KambanGroupByOptions(),
                        selectedValue: groupByX,
                        method: "post")
                    .FieldDropDown(
                        controlId: "KambanGroupByY",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByY(),
                        optionCollection: ss.KambanGroupByOptions(),
                        selectedValue: groupByY,
                        insertBlank: true,
                        method: "post")
                    .FieldDropDown(
                        controlId: "KambanAggregateType",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AggregationType(),
                        optionCollection: ss.KambanAggregationTypeOptions(),
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "KambanValueField",
                        controlId: "KambanValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: ss.KamvanValueOptions(),
                        selectedValue: value,
                        method: "post")
                    .FieldDropDown(
                        controlId: "KambanColumns",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.MaxColumns(),
                        optionCollection: Enumerable.Range(
                            Parameters.General.KambanMinColumns,
                            Parameters.General.KambanMaxColumns)
                                .ToDictionary(o => o.ToString(), o => o.ToString()),
                        selectedValue: columns?.ToString(),
                        method: "post")
                    .KambanBody(
                        ss: ss,
                        groupByX: ss.GetColumn(groupByX),
                        groupByY: ss.GetColumn(groupByY),
                        aggregateType: aggregateType,
                        value: ss.GetColumn(value),
                        columns: columns,
                        data: data)
                    .MainCommands(
                        siteId: ss.SiteId,
                        pt: pt,
                        verType: Versions.VerTypes.Latest,
                        importButton: true,
                        exportButton: true);
            });
        }

        public static HtmlBuilder KambanBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column groupByX,
            Column groupByY,
            string aggregateType,
            Column value,
            int? columns,
            IEnumerable<KambanElement> data,
            long changedItemId = 0)
        {
            var choicesY = CorrectedChoices(
                groupByY, groupByY?.EditChoices(insertBlank: groupByY.Nullable));
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("KambanBody")
                    .DataAction("UpdateByKamban")
                    .DataMethod("post"),
                action: () => groupByX.EditChoices(
                    insertBlank: groupByX.Nullable)
                        .Chunk(columns.ToInt())
                        .ForEach(choicesX => hb
                            .Table(
                                ss: ss,
                                choicesX: CorrectedChoices(groupByX, choicesX),
                                choicesY: choicesY,
                                aggregateType: aggregateType,
                                value: value,
                                data: data,
                                changedItemId: changedItemId)));
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

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
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
                                        data: data.Where(o => o.GroupX == choice.Key),
                                        choice: choice)));
                        }))
                    .TBody(action: () =>
                    {
                        if (choicesY != null)
                        {
                            choicesY.ForEach(choiceY =>
                            {
                                hb.Tr(css: "kamban-row", action: () =>
                                {
                                    hb.Th(action: () => hb
                                        .HeaderText(
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: data.Where(o => o.GroupY == choiceY.Key),
                                        choice: choiceY));
                                    choicesX.ForEach(choiceX => hb
                                        .TBody(
                                            ss: ss,
                                            choiceX: choiceX.Key,
                                            choiceY: choiceY.Key,
                                            value: value,
                                            data: data,
                                            changedItemId: changedItemId));
                                });
                            });
                        }
                        else
                        {
                            hb.Tr(css: "kamban-row", action: () =>
                               choicesX.ForEach(choiceX => hb
                                    .TBody(
                                        ss: ss,
                                        choiceX: choiceX.Key,
                                        choiceY: null,
                                        value: value,
                                        data: data,
                                        changedItemId: changedItemId)));
                        }
                    }));

        }

        private static HtmlBuilder TBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            string choiceX,
            string choiceY,
            Column value,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            return hb.Td(
                attributes: new HtmlAttributes()
                    .Class("kamban-container")
                    .DataX(HttpUtility.HtmlEncode(choiceX))
                    .DataY(HttpUtility.HtmlEncode(choiceY)),
                action: () => hb
                    .Div(action: () => 
                        data
                            .Where(o => o.GroupX == choiceX)
                            .Where(o => choiceY == null || o.GroupY == choiceY)
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
            return hb.Text(text: "{0}({1}){2}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(),
                data.Count(),
                value != null && data.Any()
                    ? " : " + value.Display(Summary(data, aggregateType), unit: true)
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