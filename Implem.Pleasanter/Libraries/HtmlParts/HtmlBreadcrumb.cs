using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBreadcrumb
    {
        public static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, SiteSettings ss, long siteId, bool _using)
        {
            if (!Sessions.LoggedIn() || !_using)
            {
                return hb;
            }
            var controller = Routes.Controller();
            switch (controller)
            {
                case "admins":
                    return Breadcrumb(hb, controller);
                case "depts":
                    return Breadcrumb(hb, controller, Displays.Depts());
                case "groups":
                    return Permissions.CanManageTenant()
                        ? Breadcrumb(hb, controller, Displays.Groups())
                        : Breadcrumb(hb);
                case "users":
                    switch (Routes.Action())
                    {
                        case "editapi":
                            return hb.Breadcrumb(new Dictionary<string, string>
                            {
                                { Locations.Get("Users", "EditApi"), Displays.ApiSettings() }
                            });
                        default:
                            return Permissions.CanManageTenant()
                                ? Breadcrumb(hb, controller, Displays.Users())
                                : Breadcrumb(hb);
                    }
                case "items":
                case "permissions":
                    return hb.Breadcrumb(siteId);
                default:
                    return hb;
            }
        }

        private static HtmlBuilder Breadcrumb(
            HtmlBuilder hb, string controller, string display = null)
        {
            return display != null
                ? hb.Breadcrumb(new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() },
                    { Locations.Index(controller), display }
                })
                : hb.Breadcrumb(new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() }
                });
        }

        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, long siteId)
        {
            return hb.Breadcrumb(data: SiteInfo.TenantCaches[Sessions.TenantId()]
                .SiteMenu.Breadcrumb(siteId)
                    .ToDictionary(
                        o => !o.HasOnlyOneChild()
                            ? Locations.ItemIndex(o.SiteId)
                            : Locations.ItemEdit(o.OnlyOneChildId),
                        o => o.Title));
        }

        private static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, Dictionary<string, string> data = null)
        {
            return hb.Ul(id: "Breadcrumb", action: () =>
            {
                hb.Li(Locations.Top(), Displays.Top());
                data?.ForEach(item => hb
                    .Li(href: item.Key, text: item.Value));
            });
        }

        private static HtmlBuilder Li(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "item", action: () => hb
                .A(href: href, text: text)
                .Span(css: "separator", action: () => hb
                    .Text(text: ">")));
        }
    }
}