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
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            if ((!context.Authenticated && !context.Publish) || !_using)
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
                        display: Displays.Depts(context: context));
                case "tenants":
                    return Breadcrumb(
                        hb: hb,
                        context: context,
                        ss: ss,
                        controller: context.Controller,
                        display: Displays.Tenants(context: context));
                case "groups":
                    return Permissions.CanManageTenant(context: context)
                        ? Breadcrumb(
                            hb: hb,
                            context: context,
                            ss: ss,
                            controller: context.Controller,
                            display: Displays.Groups(context: context))
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
                                        Locations.Get(
                                            context: context,
                                            parts: new string[]
                                            {
                                                "Users",
                                                "EditApi"
                                            }),
                                        Displays.ApiSettings(context: context)
                                    }
                                });
                        default:
                            return Permissions.CanManageTenant(context: context)
                                ? Breadcrumb(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    controller: context.Controller,
                                    display: Displays.Users(context: context))
                                : Breadcrumb(
                                    hb: hb,
                                    context: context,
                                    ss: ss);
                    }
                case "publishes":
                case "items":
                    return hb
                        .CopyDirectUrlToClipboard(
                            context: context,
                            view: view)
                        .Breadcrumb(
                            context: context,
                            ss: ss);
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
                    {
                        Locations.Index(
                            context: context,
                            controller: "Admins"),
                        Displays.Admin(context: context)
                    },
                    {
                        Locations.Index(
                            context: context,
                            controller: controller),
                        display
                    }
                })
                : hb.Breadcrumb(context: context, ss: ss, data: new Dictionary<string, string>
                {
                    {
                        Locations.Index(
                            context: context,
                            controller: "Admins"),
                        Displays.Admin(context: context)
                    }
                });
        }

        public static HtmlBuilder Breadcrumb(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Breadcrumb(
                context: context,
                ss: ss,
                data: SiteInfo.TenantCaches.Get(context.TenantId)?
                    .SiteMenu
                    .Breadcrumb(
                        context: context,
                        siteId: ss.SiteId)
                    .ToDictionary(
                        o => !o.HasOnlyOneChild()
                            ? Locations.ItemIndex(
                                context: context,
                                id: o.SiteId)
                            : Locations.ItemEdit(
                                context: context,
                                id: o.OnlyOneChildId),
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
                if (!context.Publish)
                {
                    hb.Li(
                        href: Locations.Top(context: context),
                        text: Displays.Top(context: context));
                }
                data?.ForEach(item => hb
                    .Li(
                        href: item.Key,
                        text: item.Value));
                hb.TrashBox(
                    context: context,
                    ss: ss);
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
                                .Text(text: Displays.TrashBox(context: context))));
                default:
                    return hb;
            }
        }

        public static HtmlBuilder CopyDirectUrlToClipboard(
            this HtmlBuilder hb, Context context, View view)
        {
            return view != null
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id("CopyToClipboards"),
                    action: () => hb
                        .Div(
                    attributes: new HtmlAttributes()
                        .Id("CopyDirectUrlToClipboard")
                        .Class("display-control")
                        .OnClick($"$p.copyDirectUrlToClipboard('{DirectUrl(context: context, view: view)}');"),
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-link")
                        .Text(text: string.Empty)))
                : hb;
        }

        private static string DirectUrl(Context context, View view)
        {
            var queryString = HttpUtility.ParseQueryString(context.Query);
            if (view != null)
            {
                queryString["View"] = view.ToJson();
            }
            return new System.UriBuilder(context.AbsoluteUri)
            {
                Query = queryString.ToString()
            }.ToString();
        }
    }
}