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
            bool useSearch)
        {
            return errorType == Error.Types.None
                && useNavigationMenu
                && !context.Publish
                    ? hb.Nav(
                        id: "Navigations",
                        css: "ui-widget-header",
                        action: () => hb
                            .Menus(
                                context: context,
                                ss: ss,
                                siteId: siteId,
                                referenceType: referenceType,
                                muenuId: "NavigationMenu",
                                menus: Parameters.NavigationMenus)
                            .Search(
                                context: context,
                                _using: useSearch && !Parameters.Search.DisableCrossSearch))
                    : hb;
        }

        private static HtmlBuilder Menus(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            string muenuId,
            string css = null,
            List<NavigationMenu> menus = null,
            bool childMenu = false)
        {
            if (menus?.Any() != true)
            {
                return hb;
            }
            return hb.Ul(
                id: muenuId,
                css: css,
                action: () => menus
                    .Where(menu => !menu.Disabled)
                    .ForEach(menu =>
                    {
                        if (Check(
                            context: context,
                            ss: ss,
                            menus: menu,
                            _using: Using(
                                context: context,
                                ss: ss,
                                referenceType: referenceType,
                                siteId: siteId,
                                menu: menu)))
                        {
                            hb.Li(
                                id: menu.ContainerId ?? menu.MenuId,
                                css: menu.ChildMenus?.Any() != true
                                    ? null
                                    : "sub-menu",
                                action: () => hb
                                    .Content(
                                        context: context,
                                        ss: ss,
                                        siteId: siteId,
                                        childMenu: childMenu,
                                        menu: menu)
                                    .Menus(
                                        context: context,
                                        ss: ss,
                                        siteId: siteId,
                                        referenceType: referenceType,
                                        muenuId: menu.MenuId,
                                        css: "menu",
                                        childMenu: true,
                                        menus: menu.ChildMenus));
                        }
                    }));
        }

        private static HtmlBuilder Content(
                    this HtmlBuilder hb,
                    Context context,
                    SiteSettings ss,
                    long siteId,
                    bool childMenu,
                    NavigationMenu menu)
        {
            return childMenu
                ? hb.ContentBody(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    childMenu: childMenu,
                    menu: menu)
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
                        menu: menu));
        }

        private static HtmlBuilder ContentBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            bool childMenu,
            NavigationMenu menu)
        {
            if (menu.LinkParams == null
                && menu.Url == null
                && !childMenu)
            {
                return hb
                    .Span(css: menu.Icon)
                    .Text(text: Text(
                        context: context,
                        ss: ss,
                        menu: menu));
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
                        .Span(css: menu.Icon)
                        .Text(text: Text(
                            context: context,
                            ss: ss,
                            menu: menu)));
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
                default:
                    return new HtmlAttributes();
            }
        }

        private static string Text(
            Context context,
            SiteSettings ss,
            NavigationMenu menu)
        {
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
            var canManageGroups = context.UserSettings?.AllowGroupAdministration(context: context) == true;
            var canManageSite = siteId != 0 && context.CanManageSite(
                ss: ss,
                site: true);
            var canManageDepts = Permissions.CanManageTenant(context: context);
            var canManageUsers = Permissions.CanManageTenant(context: context);
            var canManageRegistrations = Permissions.CanManageRegistrations(context: context);
            var canManageTenants = Permissions.CanManageTenant(context: context)
                || context.UserSettings?.EnableManageTenant == true;
            var canManageTrashBox = CanManageTrashBox(
                context: context,
                ss: ss);
            switch (menu.MenuId)
            {
                case "NewMenu":
                    return ss.ReferenceType == "Sites" && context.Action == "index"
                        ? context.CanManageSite(ss: ss)
                        : context.CanCreate(ss: ss)
                            && ss.ReferenceType != "Wikis"
                            && context.Action != "trashbox";
                case "ViewModeMenu":
                    return Def.ViewModeDefinitionCollection
                        .Any(o => o.ReferenceType == referenceType);
                case "SettingsMenu":
                    return canManageSite
                        || canManageDepts
                        || canManageGroups
                        || canManageUsers;
                case "SettingsMenu_SiteSettings":
                    return canManageSite;
                case "SettingsMenu_DeptAdmin":
                    return canManageDepts;
                case "SettingsMenu_GroupAdmin":
                    return canManageGroups;
                case "SettingsMenu_UserAdmin":
                    return canManageUsers;
                case "SettingsMenu_Registrations":
                    return canManageRegistrations;
                case "SettingsMenu_TrashBox":
                    return canManageTrashBox;
                case "SettingsMenu_TenantAdmin":
                    return canManageTenants;
                case "SettingsMenu_ImportSitePackage":
                    return Parameters.SitePackage.Import
                        && canManageSite
                        && ss.IsSite(context: context)
                        && ss.ReferenceType == "Sites"
                        || (context.Controller == "items"
                            && ss.SiteId == 0
                            && context.UserSettings?.AllowCreationAtTopSite(context: context) == true);
                case "SettingsMenu_ExportSitePackage":
                    return Parameters.SitePackage.Export
                        && canManageSite
                        && ss.IsSite(context: context);
                case "AccountMenu_ShowStartGuide":
                    return context.UserSettings?.ShowStartGuideAvailable(context: context) == true;
                case "AccountMenu_EditProfile":
                    return Parameters.Service.ShowProfiles;
                case "AccountMenu_ChangePassword":
                    return !Parameters.Service.ShowProfiles
                        && Parameters.Service.ShowChangePassword;
                case "AccountMenu_ApiSettings":
                    return context.ContractSettings.Api != false
                        && Parameters.Api.Enabled;
                case "LockTableMenu_LockTable":
                case "LockTableMenu_ForceUnlockTable":
                    return canManageSite
                        && ss.AllowLockTable == true;
                case "LockTableMenu_UnlockTable":
                    return ss.AllowLockTable == true;
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
                                ? NewHref(context: context, ss: ss)
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
                            : Locations.ItemNew(
                                context: context,
                                id: ss.SiteId);
                default:
                    return Locations.New(
                        context: context,
                        controller: context.Controller);
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
    }
}