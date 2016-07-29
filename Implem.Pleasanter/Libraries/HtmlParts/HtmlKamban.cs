using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataViews;
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
            SiteSettings siteSettings,
            string groupByColumn,
            string aggregateType,
            string valueColumn,
            Permissions.Types permissionType,
            IEnumerable<KambanElement> data)
        {
            return hb.Div(css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "KambanGroupByColumn",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupBy(),
                        optionCollection: siteSettings.ColumnCollection.Where(o => o.HasChoices())
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        selectedValue: groupByColumn,
                        action: "DataView",
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
                        action: "DataView",
                        method: "post")
                    .FieldDropDown(
                        fieldId: "KambanValueColumnField",
                        controlId: "KambanValueColumn",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.SettingAggregationTarget(),
                        optionCollection: KambanValueColumnOptionCollection(siteSettings),
                        selectedValue: valueColumn,
                        action: "DataView",
                        method: "post")
                    .Div(id: "KambanChart", action: () => hb
                        .KambanGrid(
                            siteSettings: siteSettings,
                            groupByColumn: siteSettings.AllColumn(groupByColumn),
                            aggregateType: aggregateType,
                            valueColumn: siteSettings.AllColumn(valueColumn),
                            data: data))
                    .MainCommands(
                        siteId: siteSettings.SiteId,
                        permissionType: permissionType,
                        verType: Versions.VerTypes.Latest,
                        backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                        importButton: true,
                        exportButton: true);
            });
        }

        private static Dictionary<string, string> KambanValueColumnOptionCollection(
            SiteSettings siteSettings)
        {
            return new Dictionary<string, string>
            {
                { string.Empty, string.Empty }
            }.Concat(siteSettings.ColumnCollection
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .ToList()
                .ToDictionary(o => o.ColumnName, o => o.LabelText))
                .ToDictionary(o => o.Key, o => o.Value);
        }

        public static HtmlBuilder KambanGrid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Column groupByColumn,
            string aggregateType,
            Column valueColumn,
            IEnumerable<KambanElement> data,
            long changedItemId = 0)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("KambanGrid")
                    .DataAction("UpdateByKamban")
                    .DataMethod("post"),
                action: () => groupByColumn.EditChoices(
                    siteSettings.InheritPermission,
                    insertBlank: groupByColumn.Nullable)
                        .Chunk(Parameters.General.KambanChunk)
                        .ForEach(choices => hb
                            .Table(
                                siteSettings: siteSettings,
                                choices: CorrectedChoices(groupByColumn, choices),
                                aggregateType: aggregateType,
                                valueColumn: valueColumn,
                                data: data,
                                changedItemId: changedItemId)));
        }

        private static IEnumerable<KeyValuePair<string, ControlData>> CorrectedChoices(
            Column groupByColumn,
            IEnumerable<KeyValuePair<string, ControlData>> choices)
        {
            return groupByColumn.TypeName.CsTypeSummary() != Types.CsNumeric
                ? choices
                : choices.ToDictionary(
                    o => o.Key != string.Empty ? o.Key : "0",
                    o => o.Value);
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            IEnumerable<KeyValuePair<string, ControlData>> choices,
            string aggregateType,
            Column valueColumn,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            return hb.Table(
                css: "grid fixed",
                action: () => hb
                    .Tr(css: "ui-widget-header", action: () => choices
                        .ForEach(choice => hb
                            .Th(action: () => hb
                                .Header(
                                    siteSettings: siteSettings,
                                    aggregateType: aggregateType,
                                    valueColumn: valueColumn,
                                    data: data,
                                    choice: choice))))
                    .Tr(css: "kamban-row", action: () => choices
                        .ForEach(choice => hb
                            .Td(
                                attributes: new HtmlAttributes()
                                    .Class("kamban-container")
                                    .DataValue(HttpUtility.HtmlEncode(choice.Key)),
                                action: () =>
                                    data.Where(o => o.Group == choice.Key)
                                        .ForEach(o => hb
                                            .Item(
                                                siteSettings: siteSettings,
                                                valueColumn: valueColumn,
                                                data: o,
                                                changedItemId: changedItemId))))));
        }

        private static HtmlBuilder Header(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string aggregateType,
            Column valueColumn,
            IEnumerable<KambanElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            var d = data.Where(o => o.Group == choice.Key);
            return hb.Text(text:  "{0}({1}){2}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(),
                d.Count(),
                valueColumn != null
                    ? " : " + valueColumn.Display(Summary(d, aggregateType), unit: true)
                    : string.Empty));
        }

        private static decimal Summary(IEnumerable<KambanElement> data, string aggregateType)
        {
            if (data.Count() == 0) return 0;
            switch (aggregateType)
            {
                case "Total": return data.Sum(o => o.Value);
                case "Average": return data.Average(o => o.Value);
                case "Min": return data.Min(o => o.Value);
                case "Max": return data.Max(o => o.Value);
                default: return 0;
            }
        }

        private static HtmlBuilder Item(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Column valueColumn,
            KambanElement data,
            long changedItemId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("kamban-item" + ItemChanged(data.Id, changedItemId))
                    .DataId(data.Id.ToString()),
                action: () => hb
                    .Span(css: "ui-icon ui-icon-pencil")
                    .Text(text: ItemText(valueColumn, data)));
        }

        private static string ItemText(Column valueColumn, KambanElement data)
        {
            return valueColumn == null
                ? data.Title
                : "{0} : {1}".Params(
                    data.Title,
                    valueColumn.Display(data.Value, unit: true));
        }

        private static string ItemChanged(long id, long changedItemId)
        {
            return id == changedItemId
                ? " changed"
                : string.Empty;
        }
    }
}