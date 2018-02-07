using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.General;
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
    public static class HtmlNavigationMenu
    {
        public static HtmlBuilder NavigationMenu(
            this HtmlBuilder hb,
            SiteSettings ss,
            long siteId,
            string referenceType,
            Error.Types errorType,
            bool useNavigationMenu,
            bool useSearch)
        {
            return errorType == Error.Types.None && useNavigationMenu
                ? hb.Nav(
                    id: "Navigations",
                    css: "ui-widget-header",
                    action: () => hb
                        .NavigationMenu(
                            ss: ss,
                            siteId: siteId,
                            referenceType: referenceType,
                            useNavigationMenu: useNavigationMenu)
                        .Search(_using: useSearch))
                : hb;
        }

        private static HtmlBuilder NavigationMenu(
            this HtmlBuilder hb,
            SiteSettings ss,
            long siteId,
            string referenceType,
            bool useNavigationMenu)
        {
            var canManageGroups = Sessions.UserSettings().DisableGroupAdmin != true;
            var canManageSite = siteId != 0 && ss.CanManageSite(site: true);
            var canManageDepts = Permissions.CanManageTenant();
            var canManageUsers = Permissions.CanManageTenant();
            return hb.Ul(
                id: "NavigationMenu",
                action: () => hb
                    .Li(
                        action: () => hb
                            .Div(action: () => hb
                                .A(
                                    href: NewHref(ss),
                                    attributes: SiteIndex(ss)
                                        ? new HtmlAttributes()
                                            .OnClick("$p.templates($(this));")
                                            .DataAction("Templates")
                                            .DataMethod("post")
                                        : null,
                                    action: () => hb
                                        .Span(css: "ui-icon ui-icon-plus")
                                        .Text(text: Displays.New()))),
                        _using: ss.ReferenceType == "Sites" && Routes.Action() == "index"
                            ? ss.CanManageSite()
                            : ss.CanCreate() && ss.ReferenceType != "Wikis")
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("ViewModeMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-triangle-1-e")
                                    .Text(text: Displays.View()))
                            .ViewModeMenu(ss: ss),
                        _using: Def.ViewModeDefinitionCollection
                            .Any(o => o.ReferenceType == referenceType))
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("SettingsMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.Manage()))
                            .SettingsMenu(
                                ss: ss,
                                siteId: siteId,
                                canManageSite: canManageSite,
                                canManageDepts: canManageDepts,
                                canManageGroups: canManageGroups,
                                canManageUsers: canManageUsers),
                        _using:
                            canManageSite ||
                            canManageDepts ||
                            canManageGroups ||
                            canManageUsers)
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("AccountMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-person")
                                    .Text(text: SiteInfo.UserName((
                                        Sessions.UserId()))))
                            .AccountMenu()));

        }

        private static string NewHref(SiteSettings ss)
        {
            var controller = Routes.Controller();
            switch (controller)
            {
                case "items":
                    return SiteIndex(ss)
                        ? "javascript:void(0);"
                        : Locations.ItemNew(ss.SiteId);
                default:
                    return Locations.New(controller);
            }
        }

        private static bool SiteIndex(SiteSettings ss)
        {
            return ss.ReferenceType == "Sites" && Routes.Action() == "index";
        }

        private static HtmlBuilder ViewModeMenu(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Ul(id: "ViewModeMenu", css: "menu", action: () =>
            {
                Def.ViewModeDefinitionCollection
                    .Where(o => o.ReferenceType == ss.ReferenceType)
                    .Where(o => ss.EnableViewMode(o.Name))
                    .Select(o => o.Name)
                    .ForEach(action => hb
                        .ViewModeMenu(
                            siteId: ss.SiteId,
                            referenceType: ss.ReferenceType,
                            action: action,
                            postBack: PostBack(ss)));
            });
        }

        private static bool PostBack(SiteSettings ss)
        {
            return new List<string>
            {
                "new",
                "create",
                "edit",
                "copy",
                "move",
                "separate",
                "history"
            }.Contains(Routes.Action()) || ss.Scripts?.Any() == true || ss.Styles.Any() == true;
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb,
            long siteId,
            string referenceType,
            string action,
            bool postBack)
        {
            return hb.Li(action: () => hb
                .A(
                    attributes: postBack
                        ? new HtmlAttributes().OnClick(
                            "location.href='" + Locations.ItemView(siteId, action) + "'")
                        : new HtmlAttributes()
                            .OnClick("$p.viewMode($(this));")
                            .DataAction(action),
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-triangle-1-e")
                        .Text(text: Displays.Get(action))));
        }

        private static HtmlBuilder SettingsMenu(
            this HtmlBuilder hb,
            SiteSettings ss,
            long siteId,
            bool canManageSite,
            bool canManageDepts,
            bool canManageGroups,
            bool canManageUsers)
        {
            return hb.Ul(
                id: "SettingsMenu",
                css: "menu",
                action: () => hb
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.ItemEdit(siteId),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: SiteSettingsDisplayName(ss))),
                        _using: canManageSite)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index("Depts"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.DeptAdmin())),
                        _using: canManageDepts)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index("Groups"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.GroupAdmin())),
                        _using: canManageGroups)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index("Users"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.UserAdmin())),
                        _using: canManageUsers));
        }

        private static string SiteSettingsDisplayName(SiteSettings ss)
        {
            switch (ss.ReferenceType)
            {
                case "Sites":
                    return Displays.ManageFolder();
                case "Issues":
                case "Results":
                    return Displays.ManageTable();
                case "Wikis":
                    return Displays.ManageWiki();
                default:
                    return null;
            }
        }

        private static HtmlBuilder AccountMenu(this HtmlBuilder hb)
        {
            return hb.Ul(id: "AccountMenu", css: "menu", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: Locations.Logout(),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-locked")
                            .Text(text: Displays.Logout())))
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Edit("Users", Sessions.UserId()),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-wrench")
                                .Text(text: Displays.EditProfile())),
                    _using: Parameters.Service.ShowProfiles)
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Get("Users", "EditApi"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-link")
                                .Text(text: Displays.ApiSettings())),
                    _using: Contract.Api())
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlUsageGuideUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-help")
                            .Text(text: Displays.UsageGuide())))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlBlogUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-info")
                            .Text(text: Displays.Blog())))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlSupportUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-contact")
                            .Text(text: Displays.Support())))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlContactUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-contact")
                            .Text(text: Displays.Contact())))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlPortalUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-cart")
                            .Text(text: Displays.Portal())))
                .Li(action: () => hb
                    .A(
                        href: Locations.Get("versions"),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-info")
                            .Text(text: Displays.Version()))));
        }

        private static HtmlBuilder Search(this HtmlBuilder hb, bool _using)
        {
            return _using
                ? hb
                    .Div(id: "SearchField", action: () => hb
                        .TextBox(
                            controlId: "Search",
                            controlCss: " w150 redirect",
                            placeholder: Displays.Search()))
                : hb;
        }
    }
}