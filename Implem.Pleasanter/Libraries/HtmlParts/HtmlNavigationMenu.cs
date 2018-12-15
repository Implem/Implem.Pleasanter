using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
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
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            Error.Types errorType,
            bool useNavigationMenu,
            bool useSearch)
        {
            return errorType == Error.Types.None && useNavigationMenu && !context.Publish
                ? hb.Nav(
                    id: "Navigations",
                    css: "ui-widget-header",
                    action: () => hb
                        .NavigationMenu(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            referenceType: referenceType,
                            useNavigationMenu: useNavigationMenu)
                        .Search(
                            context: context,
                            _using: useSearch))
                : hb;
        }

        private static HtmlBuilder NavigationMenu(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            bool useNavigationMenu)
        {
            var canManageGroups = context.UserSettings?.DisableGroupAdmin != true;
            var canManageSite = siteId != 0 && context.CanManageSite(ss: ss, site: true);
            var canManageDepts = Permissions.CanManageTenant(context: context);
            var canManageUsers = Permissions.CanManageTenant(context: context);
            var canManageTenants = Permissions.CanManageTenant(context: context)
                || context.UserSettings?.EnableManageTenant == true;
            var canManageTrashBox = CanManageTrashBox(context: context, ss: ss);
            return hb.Ul(
                id: "NavigationMenu",
                action: () => hb
                    .Li(
                        action: () => hb
                            .Div(action: () => hb
                                .A(
                                    href: NewHref(context: context, ss: ss),
                                    attributes: SiteIndex(context: context, ss: ss)
                                        ? new HtmlAttributes()
                                            .OnClick("$p.templates($(this));")
                                            .DataAction("Templates")
                                            .DataMethod("post")
                                        : null,
                                    action: () => hb
                                        .Span(css: "ui-icon ui-icon-plus")
                                        .Text(text: Displays.New(context: context)))),
                        _using: ss.ReferenceType == "Sites" && context.Action == "index"
                            ? context.CanManageSite(ss: ss)
                            : context.CanCreate(ss: ss)
                                && ss.ReferenceType != "Wikis"
                                && context.Action != "trashbox")
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("ViewModeMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-triangle-1-e")
                                    .Text(text: Displays.View(context: context)))
                            .ViewModeMenu(context: context, ss: ss),
                        _using: Def.ViewModeDefinitionCollection
                            .Any(o => o.ReferenceType == referenceType))
                    .Li(
                        css: "sub-menu",
                        action: () => hb
                            .Div(
                                attributes: new HtmlAttributes().DataId("SettingsMenu"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.Manage(context: context)))
                            .SettingsMenu(
                                context: context,
                                ss: ss,
                                siteId: siteId,
                                canManageSite: canManageSite,
                                canManageDepts: canManageDepts,
                                canManageGroups: canManageGroups,
                                canManageUsers: canManageUsers,
                                canManageTenants: canManageTenants,
                                canManageTrashBox: canManageTrashBox),
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
                                    .Text(text: SiteInfo.UserName(
                                        context: context,
                                        userId: context.UserId)))
                            .AccountMenu(context: context)));

        }

        private static string NewHref(Context context, SiteSettings ss)
        {
            switch (context.Controller)
            {
                case "items":
                    return SiteIndex(context: context, ss: ss)
                        ? "javascript:void(0);"
                        : Locations.ItemNew(
                            context: context,
                            id: ss.SiteId);
                default:
                    return Locations.New(
                        context: context,
                        controller: context.Controller);
            }
        }

        private static bool SiteIndex(Context context, SiteSettings ss)
        {
            return ss.ReferenceType == "Sites" && context.Action == "index";
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Ul(id: "ViewModeMenu", css: "menu", action: () =>
            {
                Def.ViewModeDefinitionCollection
                    .Where(mode => mode.ReferenceType == ss.ReferenceType)
                    .Where(mode => ss.EnableViewMode(
                        context: context, name: mode.Name))
                    .Select(mode => mode.Name)
                    .ForEach(action => hb
                        .ViewModeMenu(
                            context: context,
                            siteId: ss.SiteId,
                            referenceType: ss.ReferenceType,
                            action: action,
                            postBack: PostBack(context: context, ss: ss)));
            });
        }

        private static bool PostBack(Context context, SiteSettings ss)
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
            }.Contains(context.Action) || ss.SwitchRecordWithAjax != true;
        }

        private static HtmlBuilder ViewModeMenu(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string referenceType,
            string action,
            bool postBack)
        {
            return hb.Li(action: () => hb
                .A(
                    attributes: postBack
                        ? new HtmlAttributes().OnClick(
                            "location.href='" + Locations.ItemView(
                                context: context,
                                id: siteId,
                                action: action) + "'")
                        : new HtmlAttributes()
                            .OnClick("$p.viewMode($(this));")
                            .DataAction(action),
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-triangle-1-e")
                        .Text(text: Displays.Get(
                            context: context,
                            id: action))));
        }

        private static HtmlBuilder SettingsMenu(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            bool canManageSite,
            bool canManageDepts,
            bool canManageGroups,
            bool canManageUsers,
            bool canManageTenants,
            bool canManageTrashBox)
        {
            return hb.Ul(
                id: "SettingsMenu",
                css: "menu",
                action: () => hb
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.ItemEdit(
                                    context: context,
                                    id: siteId),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: SiteSettingsDisplayName(
                                        context: context,
                                        ss: ss))),
                        _using: canManageSite)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Edit(
                                    context: context,
                                    controller: "Tenants",
                                    id: context.TenantId),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.TenantAdmin(context: context))),
                        _using: canManageTenants)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index(
                                    context: context,
                                    controller: "Depts"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.DeptAdmin(context: context))),
                        _using: canManageDepts)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index(
                                    context: context,
                                    controller: "Groups"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.GroupAdmin(context: context))),
                        _using: canManageGroups)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.Index(
                                    context: context,
                                    controller: "Users"),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-gear")
                                    .Text(text: Displays.UserAdmin(context: context))),
                        _using: canManageUsers)
                    .Li(
                        action: () => hb
                            .A(
                                href: Locations.ItemTrashBox(
                                    context: context,
                                    id: siteId),
                                action: () => hb
                                    .Span(css: "ui-icon ui-icon-trash")
                                    .Text(text: Displays.TrashBox(context: context))),
                        _using: canManageTrashBox));
        }

        private static string SiteSettingsDisplayName(Context context, SiteSettings ss)
        {
            switch (ss.ReferenceType)
            {
                case "Sites":
                    return Displays.ManageFolder(context: context);
                case "Issues":
                case "Results":
                    return Displays.ManageTable(context: context);
                case "Wikis":
                    return Displays.ManageWiki(context: context);
                default:
                    return null;
            }
        }

        private static HtmlBuilder AccountMenu(this HtmlBuilder hb, Context context)
        {
            return hb.Ul(id: "AccountMenu", css: "menu", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: Locations.Logout(context: context),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-locked")
                            .Text(text: Displays.Logout(context: context))))
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Edit(
                                context: context,
                                controller: "Users",
                                id: context.UserId),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-wrench")
                                .Text(text: Displays.EditProfile(context: context))),
                    _using: Parameters.Service.ShowProfiles)
                .Li(
                    action: () => hb
                        .A(
                            href: Locations.Get(
                                context: context,
                                parts: new string[]
                                {
                                    "Users",
                                    "EditApi"
                                }),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-link")
                                .Text(text: Displays.ApiSettings(context: context))),
                    _using: context.ContractSettings.Api != false && Parameters.Api.Enabled)
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlUsageGuideUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-help")
                            .Text(text: Displays.UsageGuide(context: context))))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlBlogUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-info")
                            .Text(text: Displays.Blog(context: context))))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlSupportUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-contact")
                            .Text(text: Displays.Support(context: context))))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlContactUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-contact")
                            .Text(text: Displays.Contact(context: context))))
                .Li(action: () => hb
                    .A(
                        href: Parameters.General.HtmlPortalUrl,
                        target: "_blank",
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-cart")
                            .Text(text: Displays.Portal(context: context))))
                .Li(action: () => hb
                    .A(
                        href: Locations.Get(
                            context: context,
                            parts: "versions"),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-info")
                            .Text(text: Displays.Version(context: context)))));
        }

        private static HtmlBuilder Search(this HtmlBuilder hb, Context context, bool _using)
        {
            return _using
                ? hb
                    .Div(id: "SearchField", action: () => hb
                        .TextBox(
                            controlId: "Search",
                            controlCss: " w150 redirect",
                            placeholder: Displays.Search(context: context)))
                : hb;
        }

        private static bool CanManageTrashBox(Context context, SiteSettings ss)
        {
            return (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                && context.Controller == "items"
                && context.CanManageSite(ss: ss)
                && (context.Id != 0 || context.HasPrivilege);
        }
    }
}