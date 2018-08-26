using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHelpers
    {
        public static HtmlBuilder HtmlUser(
            this HtmlBuilder hb, Context context, int id)
        {
            return hb.P(css: "user", action: () => hb
                .Icon(iconCss: "ui-icon-person", text: SiteInfo.UserName(
                    context: context,
                    userId: id)));
        }

        public static HtmlBuilder HtmlDept(
            this HtmlBuilder hb, Context context, int id)
        {
            return hb.P(css: "dept", action: () => hb
                .Icon(iconCss: "ui-icon-contact", text: SiteInfo.Dept(
                    tenantId: context.TenantId,
                    deptId: id).Name));
        }
    }
}