using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNavigationMenu
    {
        public static HtmlBuilder NavigationMenu(
            this HtmlBuilder hb,
            Permissions.Types pt,
            long siteId,
            string referenceType,
            bool allowAccess,
            bool useNavigationMenu,
            bool useSearch)
        {
            return allowAccess && useNavigationMenu
                ? hb.Nav(
                    id: "Navigations",
                    css: "ui-widget-header",
                    action: () => hb
                        .NavigationMenu(
                            pt: pt,
                            siteId: siteId,
                            referenceType: referenceType,
                            allowAccess: allowAccess,
                            useNavigationMenu: useNavigationMenu)
                        .Search(_using: useSearch))
                : hb;
        }

        private static HtmlBuilder NavigationMenu(
            this HtmlBuilder hb,
            Permissions.Types pt,
            long siteId,
            string referenceType,
            bool allowAccess,
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
                                        .Displays_New())),
                        _using: !Routes.Action("new", "create", "edit", "history"))
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("ViewModeMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-triangle-1-e")
                                    .Displays_View())
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
                                    .Displays_Setting())
                            .SettingsMenu(siteId: siteId, pt: pt),
                        _using: (
                            (siteId != 0 && pt.CanEditSite()) ||
                            (siteId != 0 && pt.CanEditPermission()) ||
                            pt.CanEditTenant()))
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("AccountMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-person")
                                    .Text(text: SiteInfo.UserFullName((
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
                            editor: Routes.Action("new", "create", "edit", "history")));
            });
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb,
            long siteId,
            string referenceType,
            string action,
            bool editor)
        {
            return hb.Li(action: () => hb
                .A(
                    attributes: editor
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
            this HtmlBuilder hb, Permissions.Types pt, long siteId)
        {
            return hb.Ul(id: "SettingsMenu", css: "menu", action: () => hb
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.ItemEdit(siteId),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Displays_SiteSettings()),
                    _using: siteId != 0 && pt.CanEditSite())
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.ItemEdit(siteId, "Permissions"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-locked")
                                .Displays_EditPermissions()),
                    _using: siteId != 0 && pt.CanEditPermission())
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Index("Admins"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-gear")
                                .Displays_Admin()),
                    _using: pt.CanEditTenant()));
        }

        private static HtmlBuilder AccountMenu(this HtmlBuilder hb)
        {
            return hb.Ul(id: "AccountMenu", css: "menu", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: Locations.Edit("Users", Sessions.UserId()),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-wrench")
                            .Displays_EditProfile()))
                .Li(action: () => hb
                    .A(
                        href: Locations.Logout(),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-locked")
                            .Displays_Logout())));
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