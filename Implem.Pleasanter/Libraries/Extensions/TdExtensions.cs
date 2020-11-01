using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class TdExtensions
    {
        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            IConvertable value,
            int? tabIndex)
        {
            return column != null && value != null
                ? value.Td(
                    hb: hb,
                    context: context,
                    column: column,
                    tabIndex: tabIndex)
                : hb.Td(
                    css: column.CellCss(), 
                    action: () => hb
                        .Text(string.Empty));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            TimeZoneInfo value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(), 
                action: () => hb
                    .Text(text: value?.StandardName));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            string value,
            int? tabIndex)
        {
            return column.HasChoices()
                ? hb.Td(
                    css: column.CellCss(),
                    action: () =>
                    {
                        column.AddToChoiceHash(
                            context: context,
                            value: value);
                        var choice = column.Choice(
                            value,
                            nullCase: value.IsNullOrEmpty()
                                ? null
                                : "? " + value);
                        hb.P(
                            attributes: new HtmlAttributes()
                                .Class(choice.CssClass)
                                .Style(choice.Style),
                            action: () => hb
                                .Text(choice.TextMini));
                    })
                : column.ControlType == "MarkDown"
                    ? hb.Td(
                        css: column.CellCss(),
                        action: () => hb
                            .Div(css: "grid-title-body", action: () => hb
                                .P(css: "body markup", action: () => hb
                                    .Text(text: value))))
                    : hb.Td(
                        css: column.CellCss(),
                        action: () => hb
                            .Text(text: value));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            int value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(),
                action: () => hb
                    .Text(text: value.ToString(column.StringFormat) + column.Unit));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            long value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(),
                action: () => hb
                    .Text(text: value.ToString(column.StringFormat) + column.Unit));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            decimal value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(), 
                action: () => hb
                    .Text(text: column.Display(
                        context: context,
                        value: value,
                        unit: true)));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            DateTime value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(), 
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
            int? tabIndex)
        {
            return column.HasChoices()
                ? value
                    ? hb.Td(
                        css: column.CellCss(), 
                        action: () => hb
                            .Text(text: column.ChoicesText.SplitReturn()._1st()))
                    : hb.Td(
                        css: column.CellCss(), 
                        action: () => hb
                            .Text(text: column.ChoicesText.SplitReturn()._2nd()))
                : hb.Td(
                    css: column.CellCss(), 
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-circle-check", _using: value));
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            Column column,
            Enum value,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(), 
                action: () => hb
                    .Text(text: value.ToString()));
        }
    }
}