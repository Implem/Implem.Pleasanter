using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNavigations
    {
        public static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, long siteId, Permissions.Types permissionType, bool _using)
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
                        { Navigations.Index("Admins"), Displays.Admin() }
                    });
                case "users":
                    return hb.Breadcrumb(new Dictionary<string, string>
                    {
                        { Navigations.Index("Admins"), Displays.Admin() },
                        { Navigations.Index(controller), Displays.Users() }
                    });
                case "depts":
                    return hb.Breadcrumb(new Dictionary<string, string>
                    {
                        { Navigations.Index("Admins"), Displays.Admin() },
                        { Navigations.Index(controller), Displays.Depts() }
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
                        ? Navigations.ItemIndex(o.SiteId)
                        : Navigations.ItemEdit(o.OnlyOneChildId),
                    o => o.Title));
        }

        private static HtmlBuilder Breadcrumb(
            this HtmlBuilder hb, Dictionary<string, string> breadcrumb)
        {
            return hb.Ul(css: "nav-breadcrumb", action: () =>
            {
                hb.BreadcrumbItem(Navigations.Top(), Displays.Top());
                breadcrumb.ForEach(item => hb
                    .BreadcrumbItem(
                        href: item.Key,
                        text: item.Value));
            });
        }

        private static HtmlBuilder BreadcrumbItem(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "nav-breadcrumb-item", action: () => hb
                .A(href: href, text: text)
                .Span(css: "nav-breadcrumb-separator", action: () => hb
                    .Text(text: ">")));
        }

        public static HtmlBuilder NavigationFunctions(
            this HtmlBuilder hb,
            long siteId,
            string referenceId,
            Permissions.Types permissionType,
            bool useSearch,
            bool useNavigationButtons)
        {
            return hb.Ul(css: "nav-functions", action: () => hb
                .Search(siteId: siteId, permissionType: permissionType, _using: useSearch)
                .NavigationButtons(
                    siteId: siteId,
                    referenceId: referenceId,
                    permissionType: permissionType,
                    _using: useNavigationButtons));
        }

        private static HtmlBuilder Search(
            this HtmlBuilder hb, long siteId, Permissions.Types permissionType, bool _using)
        {
            return _using
                ? hb
                    .Li(css: "nav-function", action: () => hb
                        .Div(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "Search",
                            controlCss: " w200",
                            placeholder: Displays.Search()))
                : hb;
        }

        private static HtmlBuilder NavigationButtons(
            this HtmlBuilder hb,
            long siteId,
            string referenceId,
            Permissions.Types permissionType,
            bool _using)
        {
            return _using
                ? hb
                    .Li(
                        css: "nav-function",
                        _using: 
                            (permissionType.CanCreate() || siteId == 0) &&
                            referenceId != "Wikis",
                        action: () => hb
                            .Button(
                                text: Displays.New(),
                                controlCss: "button-create",
                                accessKey: "i",
                                href: SiteInfo.IsItem()
                                    ? Navigations.ItemNew(siteId)
                                    : Navigations.New(Url.RouteData("controller"))))
                    .Li(
                        css: "nav-function",
                        _using: 
                            permissionType.CanEditSite() && siteId != 0 &&
                            referenceId != "Wikis",
                        action: () => hb
                            .Button(
                                text: Displays.List(),
                                controlCss: "button-list",
                                accessKey: "k",
                                href: Navigations.ItemIndex(siteId)))
                    .Li(
                        css: "nav-function",
                        _using: permissionType.CanEditSite() && siteId != 0,
                        action: () => hb
                            .Button(
                                text: Displays.EditSettings(),
                                controlCss: "button-setting",
                                accessKey: "g",
                                href: Navigations.ItemEdit(siteId)))
                : hb;
        }
    }
}