using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBreadcrumb
    {
        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, SiteSettings ss, bool _using)
        {
            if (!Sessions.LoggedIn() || !_using)
            {
                return hb;
            }
            var controller = Routes.Controller();
            switch (controller)
            {
                case "admins":
                    return Breadcrumb(
                        hb: hb,
                        ss: ss,
                        controller: controller);
                case "depts":
                    return Breadcrumb(
                        hb: hb,
                        ss: ss,
                        controller: controller,
                        display: Displays.Depts());
                case "groups":
                    return Permissions.CanManageTenant()
                        ? Breadcrumb(
                            hb: hb,
                            ss: ss,
                            controller: controller,
                            display: Displays.Groups())
                        : Breadcrumb(
                            hb: hb,
                            ss: ss);
                case "users":
                    switch (Routes.Action())
                    {
                        case "editapi":
                            return hb.Breadcrumb(ss: ss, data: new Dictionary<string, string>
                            {
                                { Locations.Get("Users", "EditApi"), Displays.ApiSettings() }
                            });
                        default:
                            return Permissions.CanManageTenant()
                                ? Breadcrumb(
                                    hb: hb,
                                    ss: ss,
                                    controller: controller,
                                    display: Displays.Users())
                                : Breadcrumb(
                                    hb: hb,
                                    ss: ss);
                    }
                case "items":
                    return hb
                        .CopyDirectUrlToClipboard(ss: ss)
                        .Breadcrumb(ss: ss);
                case "permissions":
                    return hb.Breadcrumb(ss: ss);
                default:
                    return hb;
            }
        }

        private static HtmlBuilder Breadcrumb(
            HtmlBuilder hb, SiteSettings ss, string controller, string display = null)
        {
            return display != null
                ? hb.Breadcrumb(ss: ss, data: new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() },
                    { Locations.Index(controller), display }
                })
                : hb.Breadcrumb(ss: ss, data: new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() }
                });
        }

        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Breadcrumb(ss: ss, data: SiteInfo.TenantCaches[Sessions.TenantId()]
                .SiteMenu.Breadcrumb(siteId: ss.SiteId)
                    .ToDictionary(
                        o => !o.HasOnlyOneChild()
                            ? Locations.ItemIndex(o.SiteId)
                            : Locations.ItemEdit(o.OnlyOneChildId),
                        o => o.Title));
        }

        private static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, SiteSettings ss, Dictionary<string, string> data = null)
        {
            return hb.Ul(id: "Breadcrumb", action: () =>
            {
                hb.Li(Locations.Top(), Displays.Top());
                data?.ForEach(item => hb
                    .Li(href: item.Key, text: item.Value));
                hb.TrashBox(ss: ss);
            });
        }

        private static HtmlBuilder Li(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "item", action: () => hb
                .A(href: href, text: text)
                .Span(css: "separator", action: () => hb
                    .Text(text: ">")));
        }

        public static HtmlBuilder TrashBox(this HtmlBuilder hb, SiteSettings ss)
        {
            switch (ss.Context.Action)
            {
                case "trashbox":
                    return hb.Li(
                        css: "item trashbox",
                        action: () => hb
                            .Span(action: () => hb
                                .Text(text: Displays.TrashBox())));
                default:
                    return hb;
            }
        }

        public static HtmlBuilder CopyDirectUrlToClipboard(this HtmlBuilder hb, SiteSettings ss)
        {
            var queryString = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query);
            var view = (HttpContext.Current.Session["View" + ss.SiteId] as View)?.ToJson();
            if (!view.IsNullOrEmpty())
            {
                queryString["View"] = view;
            }
            var directUrl = new System.UriBuilder(HttpContext.Current.Request.Url.AbsoluteUri)
            {
                Query = queryString.ToString()
            }.ToString();
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("CopyToClipboards"),
                action: () => hb
                    .Div(
                attributes: new HtmlAttributes()
                    .Id("CopyDirectUrlToClipboard")
                    .Class("display-control")
                    .OnClick("$p.copyDirectUrlToClipboard('" + directUrl + "');"),
                action: () => hb
                    .Span(css: "ui-icon ui-icon-link")
                    .Text(text: "")));
        }
    }
}