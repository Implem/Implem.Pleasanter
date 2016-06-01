using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.ServerData;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNavigations
    {
        public static HtmlBuilder BreadCrumbs(
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
                    return hb.BreadCrumbs(Rds.ExecuteTable(statements: new SqlStatement(
                        commandText: Def.Sql.SiteBreadcrumb,
                        param: Rds.SitesParam().SiteId(siteId)))
                            .AsEnumerable()
                            .ToDictionary(
                                o => Navigations.ItemIndex(o["SiteId"].ToInt()),
                                o => o["Title"].ToString()));
                case "admins":
                    return hb.BreadCrumbs(new Dictionary<string, string>
                    {
                        { Navigations.Index("Admins"), Displays.Admin() }
                    });
                case "users":
                    return hb.BreadCrumbs(new Dictionary<string, string>
                    {
                        { Navigations.Index("Admins"), Displays.Admin() },
                        { Navigations.Index(controller), Displays.Users() }
                    });
                case "depts":
                    return hb.BreadCrumbs(new Dictionary<string, string>
                    {
                        { Navigations.Index("Admins"), Displays.Admin() },
                        { Navigations.Index(controller), Displays.Depts() }
                    });
                default:
                    return hb;
            }
        }

        private static HtmlBuilder BreadCrumbs(
            this HtmlBuilder hb, Dictionary<string, string> breadCrumbs)
        {
            return hb.Ul(css: "nav-breadcrumbs", action: () =>
            {
                hb.BreadCrumbs(Navigations.Top(), Displays.Top());
                breadCrumbs.ForEach(breadcrumb => hb
                    .BreadCrumbs(
                        href: breadcrumb.Key,
                        text: breadcrumb.Value));
            });
        }

        private static HtmlBuilder BreadCrumbs(this HtmlBuilder hb, string href, string text)
        {
            return hb.Li(css: "nav-breadcrumb", action: () => hb
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