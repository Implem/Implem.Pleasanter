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
            Permissions.Types permissionType,
            long siteId,
            string referenceType,
            bool allowAccess,
            bool useNavigationMenu)
        {
            return allowAccess && useNavigationMenu
                ? hb
                    .Ul(
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
                                _using: Routes.Action() == "index")
                            .Li(
                                css: "sub-menu",
                                action: () => hb
                                    .Div(
                                        attributes: new HtmlAttributes().DataId("DataViewMenu"),
                                        action: () => hb
                                            .Span(css: "ui-icon ui-icon-triangle-1-e")
                                            .Displays_View())
                                    .DataViewMenu(siteId: siteId, referenceType: referenceType),
                                _using: Def.DataViewDefinitionCollection
                                    .Any(o => o.ReferenceType == referenceType))
                            .Li(
                                css: "sub-menu",
                                action: () => hb
                                    .Div(
                                        attributes: new HtmlAttributes().DataId("SettingsMenu"),
                                        action: () => hb
                                            .Span(css: "ui-icon ui-icon-gear")
                                            .Displays_Setting())
                                    .SettingsMenu(siteId: siteId, permissionType: permissionType),
                                _using: (
                                    (siteId != 0 && permissionType.CanEditSite()) ||
                                    (siteId != 0 && permissionType.CanEditPermission()) ||
                                    permissionType.CanEditTenant()))
                            .Li(
                                css: "sub-menu",
                                action: () => hb
                                    .Div(
                                        attributes: new HtmlAttributes().DataId("AccountMenu"),
                                        action: () => hb
                                            .Span(css: "ui-icon ui-icon-person")
                                            .Text(text: SiteInfo.UserFullName((
                                                Sessions.UserId()))))
                                    .AccountMenu()))

                : hb;
        }

        private static string NewHref(long siteId)
        {
            var controller = Routes.Controller();
            switch (controller)
            {
                case "items":
                    return Navigations.ItemNew(siteId);
                default:
                    return Navigations.New(controller);
            }
        }

        private static HtmlBuilder DataViewMenu(
            this HtmlBuilder hb, long siteId, string referenceType)
        {
            return hb.Ul(id: "DataViewMenu", css: "menu", action: () =>
            {
                Def.DataViewDefinitionCollection
                    .Where(o => o.ReferenceType == referenceType)
                    .Select(o => o.Name)
                    .ForEach(name => hb
                        .DataViewMenu(
                            siteId: siteId,
                            referenceType: referenceType,
                            name: name));
            });
        }

        private static HtmlBuilder DataViewMenu(
            this HtmlBuilder hb, long siteId, string referenceType, string name)
        {
            return hb.Li(action: () => hb
                .Div(action: () => hb
                    .A(
                        href: Navigations.ItemView(siteId, name),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-triangle-1-e")
                            .Text(text: Displays.Get(name)))));
        }

        private static HtmlBuilder SettingsMenu(
            this HtmlBuilder hb, Permissions.Types permissionType, long siteId)
        {
            return hb.Ul(id: "SettingsMenu", css: "menu", action: () => hb
                .Li(
                    action: () => hb
                        .Div(action: () => hb
                            .A(
                                href: Navigations.ItemEdit(siteId),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Displays_Sites_SiteSettings())),
                    _using: siteId != 0 && permissionType.CanEditSite())
                .Li(
                    action: () => hb
                        .Div(action: () => hb
                            .A(
                                href: Navigations.ItemEdit(siteId, "Permissions"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-locked")
                                    .Displays_EditPermissions())),
                    _using: siteId != 0 && permissionType.CanEditPermission())
                .Li(
                    action: () => hb
                        .Div(action: () => hb
                            .A(
                                href: Navigations.Index("Admins"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Displays_Admin())),
                    _using: permissionType.CanEditTenant()));
        }

        private static HtmlBuilder AccountMenu(this HtmlBuilder hb)
        {
            return hb.Ul(id: "AccountMenu", css: "menu", action: () => hb
                .Li(action: () => hb
                    .Div(action: () => hb
                        .A(
                            href: Navigations.Edit("Users", Sessions.UserId()),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-wrench")
                                .Displays_EditProfile())))
                .Li(action: () => hb
                    .Div(action: () => hb
                        .A(
                            href: Navigations.Logout(),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-locked")
                                .Displays_Logout()))));
        }
    }
}