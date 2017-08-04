using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHelpers
    {
        public static HtmlBuilder HtmlUser(this HtmlBuilder hb, int id)
        {
            return hb.P(css: "user", action: () => hb
                .Icon(iconCss: "ui-icon-person", text: SiteInfo.UserName(id)));
        }

        public static HtmlBuilder HtmlDept(this HtmlBuilder hb, int id)
        {
            return hb.P(css: "dept", action: () => hb
                .Icon(iconCss: "ui-icon-contact", text: SiteInfo.Dept(id).Name));
        }
    }
}