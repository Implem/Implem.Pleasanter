using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSorterMenus
    {
        public static HtmlBuilder SorterMenuItems(this HtmlBuilder hb, Context context)
        {
            return hb
                .SorterMenuItem(
                    css: "sort",
                    orderType: "asc",
                    iconCss: "ui-icon-triangle-1-n",
                    text: Displays.OrderAsc(context: context))
                .SorterMenuItem(
                    css: "sort",
                    orderType: "desc",
                    iconCss: "ui-icon-triangle-1-s",
                    text: Displays.OrderDesc(context: context))
                .SorterMenuItem(
                    css: "sort",
                    orderType: "release",
                    iconCss: "ui-icon-close",
                    text: Displays.OrderRelease(context: context))
                .SorterMenuItem(
                    css: "reset",
                    orderType: null,
                    iconCss: "ui-icon-power",
                    text: Displays.ResetOrder(context: context));
        }

        private static HtmlBuilder SorterMenuItem(
            this HtmlBuilder hb,
            string css,
            string orderType,
            string iconCss,
            string text)
        {
            return hb.Li(
                attributes: new HtmlAttributes()
                    .Class(css)
                    .Add(
                        name: "data-order-type",
                        value: orderType),
                action: () => hb
                    .Div(action: () => hb
                        .Span(attributes: new HtmlAttributes().Class("ui-icon " + iconCss))
                        .Text(text: text)));
        }
    }
}
