using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using NavigationMenu = Implem.ParameterAccessor.Parts.NavigationMenu;
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
            bool useSearch,
            ServerScriptModelRow serverScriptModelRow)
        {
            if (errorType == Error.Types.None
                && useNavigationMenu
                && !context.Publish)
            {
                if (context.ThemeVersion1_0())
                {
                    return Navigations(
                        hb: hb,
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        referenceType: referenceType,
                        useSearch: useSearch,
                        serverScriptModelRow: serverScriptModelRow);
                }
                else if (context.ThemeVersionOver2_0() && context.Action != "login")
                {
                    return hb
                        .Label(
                            attributes: new HtmlAttributes()
                                .For("hamburger")
                                .OnClick("$p.closeSideMenu($(this));"),
                            css: "hamburger-switch",
                            action: () => hb
                                .Div(css: "hamburger-switch-line"))
                        .Div(
                            css: "hamburger-menubox",
                            action: () => hb
                                .Input(
                                    attributes: new HtmlAttributes()
                                        .Type("checkbox"),
                                    id: "hamburger",
                                    css: "input-hidden")
                                .Div(
                                    css: "hamburger-menuwrap hamburger-menuwrap-left",
                                    action: () => hb
                                        .Section(
                                            attributes: new HtmlAttributes()
                                                .Class("accordion"),
                                            action: () => Navigations(
                                                hb: hb,
                                                context: context,
                                                ss: ss,
                                                siteId: siteId,
                                                referenceType: referenceType,
                                                useSearch: useSearch,
                                                serverScriptModelRow: serverScriptModelRow)))
                                .Div(
                                    css: "hamburger-closelabel",
                                    action: () => hb
                                        .Label(
                                            attributes: new HtmlAttributes()
                                                .For("hamburger")
                                                .OnClick("$p.closeSideMenu($(this));"),
                                            css: "hamburger-cover")))
                        .Div(
                            id: "NavigationsUpperRight",
                            action: () => hb
                                .Div(
                                    id: "AccountUserName",
                                    action: () => hb
                                        .Span(
                                            css: "ui-icon ui-icon-person")
                                        .Text(SiteInfo.UserName(
                                            context: context,
                                            userId: context.UserId)))
                                .Search(
                                    context: context,
                                    _using: useSearch && !Parameters.Search.DisableCrossSearch));
                }
                else
                {
                    return hb;
                }
            }
            else
            {
                return hb;
            }
        }

        public static HtmlBuilder Navigations(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            bool useSearch,
            ServerScriptModelRow serverScriptModelRow)
        {
            return hb.Nav(
                id: "Navigations",
                css: context.ThemeVersion1_0()
                    ? "ui-widget-header"
                    : "hamburger-menubox",
                action: () => hb
                    .Menus(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        referenceType: referenceType,
                        muenuId: "NavigationMenu",
                        menus: ExtendedAssembleNavigationMenu(
                            navigationMenus: Parameters.NavigationMenus,
                            extendedNavigationMenus: ExtendedNavigationMenu(context)),
                        serverScriptModelRow: serverScriptModelRow)
                    .Search(
                        context: context,
                        _using: useSearch
                            && !Parameters.Search.DisableCrossSearch
                            && context.ThemeVersion1_0()))
                    .ResponsiveMenu(context: context);
        }

        private static HtmlBuilder Menus(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            string muenuId,
            string css = null,
            string cssUiWidget = null,
            List<NavigationMenu> menus = null,
            bool childMenu = false,
            ServerScriptModelRow serverScriptModelRow = null)
        {
            if (menus?.Any() != true)
            {
                return hb;
            }
            return hb.Ul(
                id: muenuId,
                css: $"{css} {cssUiWidget}".Trim(),
                action: () => menus
                    .ExtensionWhere<NavigationMenu>(context)
                    .ForEach(menu =>
                    {
                        var id = menu.ContainerId ?? menu.MenuId;
                        if (Check(
                            context: context,
                            ss: ss,
                            menus: menu,
                            _using: serverScriptModelRow?.Elements?.None(id) != true
                                && Using(
                                    context: context,
                                    ss: ss,
                                    referenceType: referenceType,
                                    siteId: siteId,
                                    menu: menu)))
                        {
                            if (context.ThemeVersion1_0())
                            {
                                SubMenu(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    siteId: siteId,
                                    referenceType: referenceType,
                                    id: id,
                                    menu: menu,
                                    childMenu: childMenu,
                                    serverScriptModelRow: serverScriptModelRow);
                            }
                            else if (context.ThemeVersionOver2_0() && context.Action != "login")
                            {
                                var attributesForId = string.Empty;
                                var iconName = string.Empty;
                                var displayText = string.Empty;
                                switch (menu.ContainerId)
                                {
                                    case "NewMenuContainer":
                                        iconName = "icon-menu-new.svg";
                                        displayText = Displays.New(context: context);
                                        goto case "MenuContainer";
                                    case "ViewModeMenuContainer":
                                        attributesForId = "block-02";
                                        iconName = "icon-menu-view-mode.svg";
                                        displayText = Displays.View(context: context);
                                        goto case "MenuContainer";
                                    case "SettingsMenuContainer":
                                        attributesForId = "block-03";
                                        iconName = "icon-menu-settings.svg";
                                        displayText = Displays.Manage(context: context);
                                        goto case "MenuContainer";
                                    case "HelpMenuContainer":
                                        attributesForId = "block-04";
                                        iconName = "icon-menu-help.svg";
                                        displayText = Displays.HelpMenu(context: context);
                                        goto case "MenuContainer";
                                    case "AccountMenuContainer":
                                        attributesForId = "block-05";
                                        iconName = "icon-menu-account.svg";
                                        displayText = Displays.UserMenu(context: context);
                                        goto case "MenuContainer";
                                    case "MenuContainer":
                                        hb.Div(
                                            css: "menubox",
                                            action: menu.ContainerId == "NewMenuContainer"
                                                ? () => hb
                                                    .A(
                                                        attributes: referenceType == "Sites" && context.Action == "index"
                                                            ? new HtmlAttributes()
                                                                .OnClick("$p.templates($(this));")
                                                                .DataAction("Templates")
                                                                .DataMethod("post")
                                                            : null,
                                                        href: menu.Url
                                                            ?? Href(
                                                                context: context,
                                                                ss: ss,
                                                                siteId: siteId,
                                                                menu: menu)
                                                            ?? "javascript:void(0);",
                                                        action: () => hb
                                                            .Span(
                                                                action: () => hb
                                                                    .Img(
                                                                        css: "new",
                                                                        src: Locations.Get(
                                                                            context: context,
                                                                            "Images",
                                                                            iconName)))
                                                            .Span(
                                                                action: () => hb
                                                                    .Text(displayText)))
                                                : () => hb
                                                    .Input(
                                                        attributes: new HtmlAttributes()
                                                            .Type("checkbox"),
                                                        id: attributesForId,
                                                        css: "toggle")
                                                    .Label(
                                                        css: "menulabel",
                                                        attributes: new HtmlAttributes()
                                                            .For(attributesForId),
                                                        action: () => hb
                                                            .Span(
                                                                attributes: new HtmlAttributes()
                                                                    .OnClick("$p.expandSideMenu($(this));"),
                                                                action: () => hb
                                                                    .Img(src: Locations.Get(
                                                                        context: context,
                                                                        "Images",
                                                                        iconName)))
                                                            .Span(
                                                                action: () => hb
                                                                    .Text(displayText)))
                                                .Div(
                                                    css: "menubox-sub",
                                                    action: () => hb
                                                        .Ul(action: () => hb
                                                            .Li(
                                                                id: id,
                                                                css: menu.ChildMenus?.Any() != true
                                                                    ? null
                                                                    : "sub-menu",
                                                                attributes: new HtmlAttributes()
                                                                    .Style(
                                                                        value: string.Empty,
                                                                        _using: serverScriptModelRow?.Elements?.Hidden(id) == true),
                                                                action: () => hb
                                                                    .Menus(
                                                                        context: context,
                                                                        ss: ss,
                                                                        siteId: siteId,
                                                                        referenceType: referenceType,
                                                                        muenuId: menu.MenuId,
                                                                        childMenu: true,
                                                                        menus: menu.ChildMenus,
                                                                        serverScriptModelRow: serverScriptModelRow)))));
                                        break;
                                    default:
                                        SubMenu(
                                            hb: hb,
                                            context: context,
                                            ss: ss,
                                            siteId: siteId,
                                            referenceType: referenceType,
                                            id: id,
                                            menu: menu,
                                            childMenu: childMenu,
                                            serverScriptModelRow: serverScriptModelRow);
                                        break;
                                }
                            }
                        }
                    }));
        }

        private static HtmlBuilder SubMenu(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            string id,
            NavigationMenu menu,
            bool childMenu = false,
            ServerScriptModelRow serverScriptModelRow = null)
        {
            return hb.Li(
                id: id,
                css: menu.ChildMenus?.Any() != true
                    ? null
                    : "sub-menu",
                attributes: new HtmlAttributes()
                    .Style(
                        value: context.ThemeVersion1_0()
                            ? "display:none;"
                            : string.Empty,
                        _using: serverScriptModelRow?.Elements?.Hidden(id) == true),
                action: () => hb
                    .Content(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        childMenu: childMenu,
                        menu: menu,
                        serverScriptLabelText: serverScriptModelRow?.Elements?.LabelText(id))
                    .Menus(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        referenceType: referenceType,
                        muenuId: menu.MenuId,
                        css: context.ThemeVersion1_0()
                            ? "menu"
                            : null,
                        cssUiWidget: context.ThemeVersion1_0()
                            ? "ui-widget-content"
                            : null,
                        childMenu: true,
                        menus: menu.ChildMenus,
                        serverScriptModelRow: serverScriptModelRow));
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            bool childMenu,
            NavigationMenu menu,
            string serverScriptLabelText)
        {
            return childMenu
                ? hb.ContentBody(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    childMenu: childMenu,
                    menu: menu,
                    serverScriptLabelText: serverScriptLabelText)
                : hb.Div(
                    attributes: Attributes(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        menu: menu)
                            .DataId(menu.MenuId)
                            .Id(menu.Id),
                    action: () => hb.ContentBody(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        childMenu: childMenu,
                        menu: menu,
                        serverScriptLabelText: serverScriptLabelText));
        }

        private static HtmlBuilder ContentBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            bool childMenu,
            NavigationMenu menu,
            string serverScriptLabelText)
        {
            if (menu.LinkParams == null
                && menu.Url == null
                && !childMenu)
            {
                return hb
                    .Span(
                        css: menu.Icon,
                        _using: context.ThemeVersion1_0())
                    .Text(text: Text(
                        context: context,
                        ss: ss,
                        menu: menu,
                        serverScriptLabelText: serverScriptLabelText));
            }
            else
            {
                return hb.A(
                    href: menu.Url
                        ?? Href(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            menu: menu)
                        ?? "javascript:void(0);",
                    target: menu.Target,
                    attributes: Attributes(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        childMenu: childMenu,
                        menu: menu),
                    action: () => hb
                        .Span(
                            css: menu.Icon,
                            _using: context.ThemeVersion1_0())
                        .Text(text: Text(
                            context: context,
                            ss: ss,
                            menu: menu,
                            serverScriptLabelText: serverScriptLabelText)));
            }
        }

        private static HtmlAttributes Attributes(
            Context context,
            SiteSettings ss,
            long siteId,
            NavigationMenu menu,
            bool childMenu = true)
        {
            if (menu.Function == null)
            {
                return new HtmlAttributes();
            }
            switch (menu.Function)
            {
                case "OpenImportDialog":
                    return new HtmlAttributes()
                        .OnClick("$p.openImportSitePackageDialog($(this));")
                        .DataAction("OpenImportSitePackageDialog")
                        .DataMethod("post");
                case "OpenExportDialog":
                    return new HtmlAttributes()
                        .OnClick("$p.openExportSitePackageDialog($(this));")
                        .DataAction("OpenExportSitePackageDialog")
                        .DataMethod("post");
                case "OpenChangePasswordDialog":
                    return new HtmlAttributes()
                        .OnClick("$p.openChangePasswordDialog($(this), 'ChangePasswordForm');")
                        .DataAction("OpenChangePasswordDialog")
                        .DataMethod("post");
                case "DisplayStartGuide":
                    return new HtmlAttributes()
                        .OnClick("$p.setStartGuide(0,1);");
                case "SendMainForm":
                    return new HtmlAttributes()
                        .OnClick("$p.send($(this),'MainForm');")
                        .DataAction(menu.Name)
                        .DataMethod("post");
                case "CreateTemplates":
                    return childMenu
                        ? new HtmlAttributes()
                        : SiteIndex(
                            context: context,
                            ss: ss)
                                ? new HtmlAttributes()
                                    .OnClick("$p.templates($(this));")
                                    .DataAction("Templates")
                                    .DataMethod("post")
                                : new HtmlAttributes();
                case "PostBack":
                    var settingName = menu.MenuId.Substring(menu.MenuId.IndexOf("_") + 1);
                    return PostBack(
                        context: context,
                        ss: ss)
                            ? new HtmlAttributes().OnClick(
                                "$p.transition('" + Locations.ItemView(
                                    context: context,
                                    id: siteId,
                                    action: settingName) + "')")
                            : new HtmlAttributes()
                                .OnClick("$p.viewMode($(this));")
                                .DataAction(settingName);
                case "Responsive":
                    return new HtmlAttributes()
                        .OnClick("$p.switchResponsive($(this));")
                        .DataAction(context.Responsive.ToString())
                        .DataMethod("post");
                default:
                    return new HtmlAttributes();
            }
        }

        private static string Text(
            Context context,
            SiteSettings ss,
            NavigationMenu menu,
            string serverScriptLabelText)
        {
            if (!serverScriptLabelText.IsNullOrEmpty())
            {
                return serverScriptLabelText;
            }
            switch (menu.Name)
            {
                case null:
                case "":
                    return null;
                case "{UserName}":
                    return SiteInfo.UserName(
                        context: context,
                        userId: context.UserId);
                case "{SiteSettingsDisplayName}":
                    return SiteSettingsDisplayName(
                        context: context,
                        ss: ss);
                case "{ResponsiveDisplayName}":
                    return Parameters.Mobile.Responsive
                        && context.Mobile
                        && context.Responsive
                            ? Displays.DesktopDisplay(
                                context: context)
                            : Displays.MobileDisplay(
                                context: context);
                default:
                    return Displays.Get(
                        context: context,
                        id: menu.Name);
            }
        }

        private static string Href(
             Context context,
             SiteSettings ss,
             long siteId,
             NavigationMenu menu)
        {
            var linkParams = menu.LinkParams?.ConvertLinkParams(
                context: context,
                ss: ss,
                siteId: siteId);
            if (linkParams?.Any(o => o != null) != true)
            {
                return null;
            }
            return Locations.Get(
                context: context,
                parts: linkParams.ToArray());
        }

        private static bool Using(
            Context context,
            SiteSettings ss,
            string referenceType,
            long siteId,
            NavigationMenu menu)
        {
            var canCreateGroups = context.UserSettings?.AllowGroupCreation(context: context) == true;
            var canManageSite = siteId != 0 && context.CanManageSite(
                ss: ss,
                site: true);
            var canManageSysLogs = context.HasPrivilege;
            var canManageDepts = Permissions.CanManageTenant(context: context);
            var canManageGroups = context.UserSettings?.AllowGroupAdministration(context: context) == true;
            var canManageUsers = Permissions.CanManageUser(context: context);
            var canManageRegistrations = Permissions.CanManageRegistrations(context: context);
            var canManageTenants = Permissions.CanManageTenant(context: context)
                || context.UserSettings?.EnableManageTenant == true;
            var canManageTrashBox = CanManageTrashBox(
                context: context,
                ss: ss);
            var canManageGroupTrashBox = CanManageGroupTrashBox(
                context: context,
                ss: ss);
            var canManageDeptTrashBox = CanManageDeptTrashBox(
                context: context,
                ss: ss);
            var canManageUserTrashBox = CanManageUserTrashBox(
                context: context,
                ss: ss);
            var canUseApi = context.UserSettings?.AllowApi(context: context) == true;
            var canUnlockSite = ss.LockedTable()
                && ss.LockedTableUser.Id == context.UserId;
            switch (menu.MenuId)
            {
                case "NewMenu":
                    return ss.ReferenceType == "Sites" && context.Action == "index"
                        ? context.CanManageSite(ss: ss)
                        : ss.ReferenceType == "Groups"
                            ? canCreateGroups
                            : context.CanCreate(ss: ss, site: true)
                                && ss.ReferenceType != "Wikis"
                                && context.Action != "trashbox"
                                && ss.ReferenceType != "Dashboards"
                                && !(ss.ReferenceType == "Sites" && context.Action == "edit");
                case "ViewModeMenu":
                    return Def.ViewModeDefinitionCollection
                        .Any(o => o.ReferenceType == referenceType);
                case "SettingsMenu":
                    return canManageSite
                        || canManageDepts
                        || canManageGroups
                        || canManageUsers
                        || canUnlockSite;
                case "SettingsMenu_SiteSettings":
                    return canManageSite;
                case "SettingsMenu_SysLogAdmin":
                    return canManageSysLogs;
                case "SettingsMenu_DeptAdmin":
                    return canManageDepts;
                case "SettingsMenu_GroupAdmin":
                    return canManageGroups;
                case "SettingsMenu_UserAdmin":
                    return canManageUsers;
                case "SettingsMenu_Registrations":
                    return canManageRegistrations;
                case "SettingsMenu_TrashBox":
                    return canManageTrashBox
                        && ss.ReferenceType != "Wikis";
                case "SettingsMenu_GroupTrashBox":
                    return canManageGroupTrashBox;
                case "SettingsMenu_DeptTrashBox":
                    return canManageDeptTrashBox;
                case "SettingsMenu_UserTrashBox":
                    return canManageUserTrashBox;
                case "SettingsMenu_TenantAdmin":
                    return canManageTenants;
                case "SettingsMenu_ImportSitePackage":
                    return Parameters.SitePackage.Import
                        && canManageSite
                        && ss.IsSite(context: context)
                        && ss.ReferenceType == "Sites"
                        || (context.Controller == "items"
                            && context.Action == "index"
                            && ss.SiteId == 0
                            && context.UserSettings?.AllowCreationAtTopSite(context: context) == true);
                case "SettingsMenu_ExportSitePackage":
                    return Parameters.SitePackage.Export
                        && canManageSite
                        && context.Action == "index"
                        && ss.IsSite(context: context);
                case "AccountMenu_ShowStartGuide":
                    return context.UserSettings?.ShowStartGuideAvailable(context: context) == true;
                case "AccountMenu_EditProfile":
                    return Parameters.Service.ShowProfiles;
                case "AccountMenu_ChangePassword":
                    return Parameters.Service.ShowChangePassword;
                case "AccountMenu_ApiSettings":
                    return Parameters.Api.Enabled
                        && context.ContractSettings.Api != false
                        && canUseApi;
                case "LockTableMenu_LockTable":
                case "LockTableMenu_ForceUnlockTable":
                    return canManageSite
                        && ss.AllowLockTable == true;
                case "LockTableMenu_UnlockTable":
                    return ss.AllowLockTable == true;
                case "AccountMenu_Responsive":
                    return context.Mobile;
                default:
                    return true;
            }
        }

        private static bool Check(
            Context context,
            SiteSettings ss,
            NavigationMenu menus,
            bool _using)
        {
            return CheckRequest(
                type: context.Controller,
                paramList: menus.Controllers)
                    && CheckRequest(
                        type: ss.ReferenceType,
                        paramList: menus.ReferenceTypes)
                    && CheckRequest(
                        type: context.Action,
                        paramList: menus.Actions)
                    && Check(
                        context: context,
                        ss: ss,
                        menus: menus)
                    && _using;
        }

        private static bool CheckRequest(string type, List<string> paramList)
        {
            if (paramList?.Any() != true)
            {
                return true;
            }
            if (paramList.Any(paramItem =>
                paramItem != string.Empty && paramItem.Substring(0, 1) == "-"))
            {
                return !(paramList
                    .Where(paramItem =>
                        paramItem != string.Empty && paramItem.Substring(0, 1) == "-")
                    .Select(paramItem =>
                        paramItem.Replace("-", string.Empty))
                    .Any(paramItem => paramItem == type).ToBool());
            }
            else
            {
                return (paramList
                    .Any(paramItem =>
                        (paramItem == type || paramItem == string.Empty)));
            }
        }

        private static bool Check(
            Context context,
            SiteSettings ss,
            NavigationMenu menus)
        {
            var menuId = menus.MenuId;

            switch (menuId)
            {
                case "LockTableMenu_LockTable":
                case "LockTableMenu_UnlockTable":
                case "LockTableMenu_ForceUnlockTable":
                    if (ss.IsTable())
                    {
                        if (!ss.Locked())
                        {
                            return (menuId == "LockTableMenu_LockTable");
                        }
                        else if (ss.LockedTableUser.Id == context.UserId)
                        {
                            return (menuId == "LockTableMenu_UnlockTable");
                        }
                        else if (context.HasPrivilege)
                        {
                            return (menuId == "LockTableMenu_ForceUnlockTable");
                        }
                    }
                    return false;
                case "ViewModelMenu_Index":
                case "ViewModelMenu_Calendar":
                case "ViewModelMenu_Crosstab":
                case "ViewModelMenu_Gantt":
                case "ViewModelMenu_BurnDown":
                case "ViewModelMenu_TimeSeries":
                case "ViewModelMenu_Analy":
                case "ViewModelMenu_Kamban":
                case "ViewModelMenu_ImageLib":
                    return (Def.ViewModeDefinitionCollection
                        .Where(mode => mode.ReferenceType == ss.ReferenceType)
                        .Where(mode => ss.EnableViewMode(
                            context: context,
                            name: mode.Name))
                        .Select(mode => mode.Name).ToList()).Contains(menuId.Substring(menuId.IndexOf("_") + 1));
                default:
                    return true;
            }
        }

        private static bool SiteIndex(Context context, SiteSettings ss)
        {
            return ss.ReferenceType == "Sites" && context.Action == "index";
        }

        private static bool PostBack(Context context, SiteSettings ss)
        {
            return new List<string>
            {
                "new",
                "create",
                "edit",
                "update",
                "copy",
                "move",
                "separate",
                "history"
            }.Contains(context.Action) || ss.SwitchRecordWithAjax != true;
        }

        private static List<string> ConvertLinkParams(
            this List<string> linkParams,
            Context context,
            SiteSettings ss,
            long siteId)
        {
            return linkParams = linkParams?.Select(
                linkItem => (linkItem == "{UserId}")
                    ? context.UserId.ToString()
                    : (linkItem == "{SiteId}")
                        ? siteId.ToString()
                        : (linkItem == "{Publish}")
                            ? (context.Publish ? "Publishes" : "Items")
                            : (linkItem == "{New}")
                                ? NewHref(
                                    context: context,
                                    ss: ss)
                                : linkItem)?.ToList();
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
                case "Dashboards":
                    return Displays.ManageDashboard(context: context);
                default:
                    return null;
            }
        }

        private static string NewHref(Context context, SiteSettings ss)
        {
            switch (context.Controller)
            {
                case "items":
                    return SiteIndex(
                        context: context,
                        ss: ss)
                            ? null
                            : $"Items/{ss.SiteId}/New";
                default:
                    return $"{context.Controller}/New";
            }
        }

        private static HtmlBuilder Search(this HtmlBuilder hb, Context context, bool _using)
        {
            return _using
                ? hb.Div(
                    id: "SearchField",
                    action: () => hb
                        .TextBox(
                            controlId: "Search",
                            controlCss: " w150 redirect",
                            placeholder: Displays.Search(context: context),
                            disabled: !context.Mobile))
                : hb;
        }

        private static bool CanManageTrashBox(Context context, SiteSettings ss)
        {
            return (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                && context.Controller == "items"
                && context.CanManageSite(ss: ss)
                && !ss.Locked()
                && (context.Id != 0 || context.HasPrivilege);
        }

        private static bool CanManageGroupTrashBox(Context context, SiteSettings ss)
        {
            return (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                && context.Controller == "groups"
                && Permissions.CanEditGroup(context: context)
                && !ss.Locked();
        }

        private static bool CanManageDeptTrashBox(Context context, SiteSettings ss)
        {
            return (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                && context.Controller == "depts"
                && Permissions.CanManageTenant(context: context);
        }

        private static bool CanManageUserTrashBox(Context context, SiteSettings ss)
        {
            return (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                && context.Controller == "users"
                && Permissions.CanManageUser(context: context)
                && !ss.Locked();
        }
        
        private static HtmlBuilder ResponsiveMenu(this HtmlBuilder hb, Context context)
        {
            return Parameters.Mobile.Responsive
                && context.Mobile
                && context.Responsive
                    ? hb.A(
                        id: "navtgl",
                        href: "javascript:void(0);",
                        attributes: new HtmlAttributes()
                            .OnClick("$p.openResponsiveMenu();")
                            .DataAction("OpenResponsiveMenu"))
                    : hb;
        }

        private static List<ParameterAccessor.Parts.ExtendedNavigationMenu> ExtendedNavigationMenu(Context context)
        {
            return ExtendedNavigationMenu(
                deptId: context.DeptId,
                groups: context.Groups,
                userId: context.UserId,
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
        }

        private static List<ParameterAccessor.Parts.ExtendedNavigationMenu> ExtendedNavigationMenu(
            int deptId,
            List<int> groups,
            int userId,
            long siteId,
            long id,
            string controller,
            string action)
        {
            var extendedNavigationMenus = ExtensionUtilities.ExtensionWhere<ParameterAccessor.Parts.ExtendedNavigationMenu>(
                extensions: Parameters.ExtendedNavigationMenus,
                name: null,
                deptId: deptId,
                groups: groups,
                userId: userId,
                siteId: siteId,
                id: id,
                controller: controller,
                action: action);
            return extendedNavigationMenus.ToList();
        }

        private static List<NavigationMenu> ExtendedAssembleNavigationMenu(
            List<NavigationMenu> navigationMenus,
            List<ParameterAccessor.Parts.ExtendedNavigationMenu> extendedNavigationMenus)
        {
            var menus = navigationMenus
                .ToJson()
                .Deserialize<List<NavigationMenu>>();
            var exMenus = extendedNavigationMenus
                .ToJson()
                .Deserialize<List<ParameterAccessor.Parts.ExtendedNavigationMenu>>();
            exMenus?.ForEach(extendedMenu =>
            {
                switch (extendedMenu.Action)
                {
                    case "Append":
                        AppendExtendedNavigationMenu(
                            menus: menus,
                            targetId: extendedMenu.TargetId,
                            extendedMenus: extendedMenu.NavigationMenus);
                        break;
                    case "Prepend":
                        PrependExtendedNavigationMenu(
                            menus: menus,
                            targetId: extendedMenu.TargetId,
                            extendedMenus: extendedMenu.NavigationMenus);
                        break;
                    case "Remove":
                        RemoveExtendedNavigationMenu(
                            menus: menus,
                            targetId: extendedMenu.TargetId);
                        break;
                    case "Replace":
                        ReplaceExtendedNavigationMenu(
                            menus: menus,
                            targetId: extendedMenu.TargetId,
                            extendedMenus: extendedMenu.NavigationMenus);
                        break;
                    case "ReplaceAll":
                        menus = extendedMenu.NavigationMenus;
                        break;
                    default:
                        break;
                }
            });
            return menus;
        }

        private static void AppendExtendedNavigationMenu(
            List<NavigationMenu> menus,
            string targetId,
            List<NavigationMenu> extendedMenus)
        {
            var targetMenu = menus?
                .Select((m, i) => new { Index = i, Menu = m })
                .FirstOrDefault(o => o.Menu.MenuId == targetId);
            if (targetMenu != null && extendedMenus != null)
            {
                menus.InsertRange(targetMenu.Index + 1, extendedMenus);
            }
            else
            {
                menus?.ForEach(menu =>
                    AppendExtendedNavigationMenu(
                        menus: menu.ChildMenus,
                        targetId: targetId,
                        extendedMenus: extendedMenus));
            }
        }

        private static void PrependExtendedNavigationMenu(
            List<NavigationMenu> menus,
            string targetId,
            List<NavigationMenu> extendedMenus)
        {
            var targetMenu = menus?
                .Select((m, i) => new { Index = i, Menu = m })
                .FirstOrDefault(o => o.Menu.MenuId == targetId);
            if (targetMenu != null && extendedMenus != null)
            {
                menus.InsertRange(targetMenu.Index, extendedMenus);
            }
            else
            {
                menus?.ForEach(menu =>
                    PrependExtendedNavigationMenu(
                        menus: menu.ChildMenus,
                        targetId: targetId,
                        extendedMenus: extendedMenus));
            }
        }

        private static void RemoveExtendedNavigationMenu(
            List<NavigationMenu> menus,
            string targetId)
        {
            var targetMenu = menus?.FirstOrDefault(o => o.MenuId == targetId);
            if (targetMenu != null)
            {
                menus.Remove(targetMenu);
            }
            else
            {
                menus?.ForEach(menu =>
                    RemoveExtendedNavigationMenu(
                        menus: menu.ChildMenus,
                        targetId: targetId));
            }
        }

        private static void ReplaceExtendedNavigationMenu(
            List<NavigationMenu> menus,
            string targetId,
            List<NavigationMenu> extendedMenus)
        {
            var targetMenu = menus?
                .Select((m, i) => new { Index = i, Menu = m })
                .FirstOrDefault(o => o.Menu.MenuId == targetId);
            if (targetMenu != null && extendedMenus != null)
            {
                menus.RemoveAt(targetMenu.Index);
                menus.InsertRange(targetMenu.Index, extendedMenus);
            }
            else
            {
                menus?.ForEach(menu =>
                    ReplaceExtendedNavigationMenu(
                        menus: menu.ChildMenus,
                        targetId: targetId,
                        extendedMenus: extendedMenus));
            }
        }
    }
}