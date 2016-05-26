using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHelpers
    {
        public static HtmlBuilder HtmlUser(this HtmlBuilder hb, int id)
        {
            return hb.P(css: "user", action: () => hb
                .Icon(iconCss: "ui-icon-person", text: SiteInfo.UserFullName(id)));
        }

        public static HtmlBuilder HtmlDept(this HtmlBuilder hb, int id)
        {
            return hb.P(css: "dept", action: () => hb
                .Icon(iconCss: "ui-icon-contact", text: SiteInfo.DeptModel(id).DeptName));
        }

        public static HtmlBuilder HtmlStatus(
            this HtmlBuilder hb, Column column, string selectedValue)
        {
            var choice = column.Choice(selectedValue);
            return hb.P(
                attributes: new HtmlAttributes()
                    .Class(choice.CssClass())
                    .Style(choice.Style()),
                action: () => hb
                    .Text(choice.TextMini()));
        }

        public static HtmlBuilder LabelValue(
            this HtmlBuilder hb,
            string label,
            string value,
            HtmlAttributes attributes)
        {
            return hb.Span(attributes: attributes, action: () =>
            {
                if (label != string.Empty)
                {
                    hb.Em(action: () => hb
                        .Text(label));
                }
                hb.Text(value);
            });
        }
    }
}