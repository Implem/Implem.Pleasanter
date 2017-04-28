using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
            return hb.Ul(
                id: "NavigationMenu",
                action: () => hb
                    .Li(
                        action: () => hb
                            .Div(action: () => hb
                                .A(
                                    href: NewHref(siteId),
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
                            .ViewModeMenu(siteId: siteId, referenceType: referenceType),
                        _using: Def.ViewModeDefinitionCollection
                            .Any(o => o.ReferenceType == referenceType))
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("SettingsMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.Setting()))
                            .SettingsMenu(siteId: siteId, ss: ss))
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

        private static string NewHref(long siteId)
        {
            var controller = Routes.Controller();
            switch (controller)
            {
                case "items":
                    return Locations.ItemNew(siteId);
                default:
                    return Locations.New(controller);
            }
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb, long siteId, string referenceType)
        {
            return hb.Ul(id: "ViewModeMenu", css: "menu", action: () =>
            {
                Def.ViewModeDefinitionCollection
                    .Where(o => o.ReferenceType == referenceType)
                    .Select(o => o.Name)
                    .ForEach(action => hb
                        .ViewModeMenu(
                            siteId: siteId,
                            referenceType: referenceType,
                            action: action,
                            ajax: EditorActions()));
            });
        }

        private static bool EditorActions()
        {
            return Routes.Action(
                "new",
                "create",
                "edit",
                "copy",
                "move",
                "separate",
                "history");
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb,
            long siteId,
            string referenceType,
            string action,
            bool ajax)
        {
            return hb.Li(action: () => hb
                .A(
                    attributes: ajax
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
            this HtmlBuilder hb, SiteSettings ss, long siteId)
        {
            return hb.Ul(id: "SettingsMenu", css: "menu", action: () => hb
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.ItemEdit(siteId),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Text(text: Displays.SiteSettings())),
                    _using: siteId != 0 && ss.CanManageSite(site: true))
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Index("Depts"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Text(text: Displays.DeptAdmin())),
                    _using: Permissions.CanManageTenant())
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Index("Groups"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Text(text: Displays.GroupAdmin())))
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Index("Users"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Text(text: Displays.UserAdmin())),
                    _using: Permissions.CanManageTenant()));
        }

        private static HtmlBuilder AccountMenu(this HtmlBuilder hb)
        {
            return hb.Ul(id: "AccountMenu", css: "menu", action: () => hb
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Edit("Users", Sessions.UserId()),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-wrench")
                                .Text(text: Displays.EditProfile())),
                    _using: Parameters.Service.ShowProfiles)
                .Li(action: () => hb
                    .A(
                        href: Locations.Logout(),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-locked")
                            .Text(text: Displays.Logout())))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlCopyrightUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-contact")
                            .Text(text: Displays.Support())))
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