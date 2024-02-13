using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class TdExtensions
    {
        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            IConvertable value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return column != null && value != null
                ? value.Td(
                    hb: hb,
                    context: context,
                    column: column,
                    tabIndex: tabIndex,
                    serverScriptModelColumn: serverScriptModelColumn)
                : hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    action: () => hb
                        .Text(string.Empty));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            TimeZoneInfo value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: value?.StandardName));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            string value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            if (serverScriptModelColumn?.RawText.IsNullOrEmpty() == false)
            {
                return hb.Td(
                    context: context,
                    column: column,
                    action: () =>
                    {
                        if (column.ControlType == "MarkDown")
                        {
                            hb.Div(css: "grid-title-body", action: () => hb
                                .P(
                                    css: "body markup",
                                    attributes: new HtmlAttributes().Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                                    action: () => hb
                                        .Raw(serverScriptModelColumn?.RawText)));
                        }
                        else
                        {
                            hb.Raw(serverScriptModelColumn?.RawText);
                        }
                    },
                    tabIndex: tabIndex,
                    serverScriptModelColumn: serverScriptModelColumn);
            }
            else if (column.HasChoices())
            {
                if (column.MultipleSelections == true)
                {
                    var choiceParts = column.ChoiceParts(
                        context: context,
                        selectedValues: value,
                        type: ExportColumn.Types.TextMini);
                    return hb.Td(
                        css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                        action: () => hb
                            .P(action: () => hb
                                .Text(text: column.MultipleSelections == true
                                    ? choiceParts.Join(", ")
                                    : choiceParts.FirstOrDefault())));
                }
                else
                {
                    column.AddToChoiceHash(
                        context: context,
                        value: value);
                    var choice = column.Choice(
                        value,
                        nullCase: value.IsNullOrEmpty()
                            ? null
                            : "? " + value);
                    return hb.Td(
                        css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                        action: () => hb.P(
                            attributes: new HtmlAttributes()
                                .Class(choice.CssClass)
                                .Style(choice.Style),
                            action: () => hb
                                .Text(choice.TextMini)));
                }
            }
            else
            {
                return hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    action: () =>
                    {
                        if (column.Anchor == true && !column.AnchorFormat.IsNullOrEmpty())
                        {
                            hb.A(
                                text: value,
                                href: column.AnchorFormat.Replace("{Value}", value),
                                target: column.OpenAnchorNewTab == true
                                    ? "_blank"
                                    : string.Empty);
                        }
                        else if (column.ControlType == "MarkDown")
                        {
                            hb.Div(css: "grid-title-body", action: () => hb
                                .P(
                                    css: "body markup",
                                    attributes: new HtmlAttributes().Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                                    action: () => hb
                                        .Text(text: value)));
                        }
                        else
                        {
                            hb.Text(text: value);
                        }
                    });
            }
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            int value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: value.ToString(column.StringFormat) + column.Unit));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            long value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: value.ToString(column.StringFormat) + column.Unit));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            double value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: value.ToString(column.StringFormat) + column.Unit));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            DateTime value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: column.DisplayGrid(
                        context: context,
                        value: value.ToLocal(context: context))));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            bool value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return column.HasChoices()
                ? value
                    ? hb.Td(
                        css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                        action: () => hb
                            .Text(text: column.ChoicesText.SplitReturn()._1st()))
                    : hb.Td(
                        css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                        action: () => hb
                            .Text(text: column.ChoicesText.SplitReturn()._2nd()))
                : hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-circle-check", _using: value));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            Enum value,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: value.ToString()));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            Action action,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: action);
        }
    }
}