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
        public static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool _using)
        {
            if (!context.Authenticated || !_using)
            {
                return hb;
            }
            switch (context.Controller)
            {
                case "admins":
                    return Breadcrumb(
                        hb: hb,
                        context: context,
                        ss: ss,
                        controller: context.Controller);
                case "depts":
                    return Breadcrumb(
                        hb: hb,
                        context: context,
                        ss: ss,
                        controller: context.Controller,
                        display: Displays.Depts());
                case "groups":
                    return Permissions.CanManageTenant(context: context)
                        ? Breadcrumb(
                            hb: hb,
                            context: context,
                            ss: ss,
                            controller: context.Controller,
                            display: Displays.Groups())
                        : Breadcrumb(
                            hb: hb,
                            context: context,
                            ss: ss);
                case "users":
                    switch (context.Action)
                    {
                        case "editapi":
                            return hb.Breadcrumb(
                                context: context,
                                ss: ss,
                                data: new Dictionary<string, string>
                                {
                                    {
                                        Locations.Get("Users", "EditApi"),
                                        Displays.ApiSettings()
                                    }
                                });
                        default:
                            return Permissions.CanManageTenant(context: context)
                                ? Breadcrumb(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    controller: context.Controller,
                                    display: Displays.Users())
                                : Breadcrumb(
                                    hb: hb,
                                    context: context,
                                    ss: ss);
                    }
                case "items":
                    return hb
                        .CopyDirectUrlToClipboard(ss: ss)
                        .Breadcrumb(context: context, ss: ss);
                case "permissions":
                    return hb.Breadcrumb(context: context, ss: ss);
                default:
                    return hb;
            }
        }

        private static HtmlBuilder Breadcrumb(
            HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string controller,
            string display = null)
        {
            return display != null
                ? hb.Breadcrumb(context: context, ss: ss, data: new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() },
                    { Locations.Index(controller), display }
                })
                : hb.Breadcrumb(context: context, ss: ss, data: new Dictionary<string, string>
                {
                    { Locations.Index("Admins"), Displays.Admin() }
                });
        }

        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Breadcrumb(
                context: context,
                ss: ss,
                data: SiteInfo.TenantCaches.Get(context.TenantId)?
                    .SiteMenu
                    .Breadcrumb(context: context, siteId: ss.SiteId)
                    .ToDictionary(
                        o => !o.HasOnlyOneChild()
                            ? Locations.ItemIndex(o.SiteId)
                            : Locations.ItemEdit(o.OnlyOneChildId),
                        o => o.Title));
        }

        private static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Dictionary<string, string> data = null)
        {
            return hb.Ul(id: "Breadcrumb", action: () =>
            {
                hb.Li(Locations.Top(), Displays.Top());
                data?.ForEach(item => hb
                    .Li(href: item.Key, text: item.Value));
                hb.TrashBox(context: context, ss: ss);
            });
        }

        private static HtmlBuilder Li(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "item", action: () => hb
                .A(href: href, text: text)
                .Span(css: "separator", action: () => hb
                    .Text(text: ">")));
        }

        public static HtmlBuilder TrashBox(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            switch (context.Action)
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