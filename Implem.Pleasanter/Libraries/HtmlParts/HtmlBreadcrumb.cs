using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBreadcrumb
    {
        public static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, long siteId, Permissions.Types pt, bool _using)
        {
            if (!Sessions.LoggedIn() || !_using)
            {
                return hb;
            }
            var controller = Url.RouteData("controller").ToLower();
            switch (controller)
            {
                case "items":
                case "permissions":
                    return hb.Breadcrumb(siteId);
                case "admins":
                    return hb.Breadcrumb(new Dictionary<string, string>
                    {
                        { Locations.Index("Admins"), Displays.Admin() }
                    });
                case "users":
                    return pt.CanEditTenant()
                        ? hb.Breadcrumb(new Dictionary<string, string>
                        {
                            { Locations.Index("Admins"), Displays.Admin() },
                            { Locations.Index(controller), Displays.Users() }
                        })
                        : hb.Breadcrumb();
                case "depts":
                    return hb.Breadcrumb(new Dictionary<string, string>
                    {
                        { Locations.Index("Admins"), Displays.Admin() },
                        { Locations.Index(controller), Displays.Depts() }
                    });
                default:
                    return hb;
            }
        }

        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, long siteId)
        {
            return hb.Breadcrumb(SiteInfo.SiteMenu.Breadcrumb(siteId)
                .ToDictionary(
                    o => !o.HasOnlyOneChild()
                        ? Locations.ItemIndex(o.SiteId)
                        : Locations.ItemEdit(o.OnlyOneChildId),
                    o => o.Title));
        }

        private static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, Dictionary<string, string> breadcrumb = null)
        {
            return hb.Ul(id: "Breadcrumb", action: () =>
            {
                hb.BreadcrumbItem(Locations.Top(), Displays.Top());
                breadcrumb?.ForEach(item => hb
                    .BreadcrumbItem(
                        href: item.Key,
                        text: item.Value));
            });
        }

        private static HtmlBuilder BreadcrumbItem(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "item", action: () => hb
                .A(href: href, text: text)
                .Span(css: "separator", action: () => hb
                    .Text(text: ">")));
        }
   }
}