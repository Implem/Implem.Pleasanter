using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class PermissionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public enum Types
        {
            Destination,
            Source
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long siteId)
        {
            var siteModel = new SiteModel(siteId, clearSessions: true);
            var ss = siteModel.PermissionsSiteSettings();
            var hb = new HtmlBuilder();
            hb.Template(
                pt: siteModel.PermissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Edit,
                allowAccess: siteModel.PermissionType.CanManagePermission(),
                siteId: siteModel.SiteId,
                referenceType: "Permissions",
                title: siteModel.Title.Value + " - " + Displays.ManagePermissions(),
                useNavigationMenu: false,
                action: () => hb
                    .Editor(siteModel: siteModel, ss: ss));
            return hb.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, SiteModel siteModel, SiteSettings ss)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("PermissionForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(siteModel.SiteId, "Permissions")),
                    action: () => hb
                        .Div(
                            id: "EditorTabsContainer",
                            css: "max",
                            action: () => hb
                                .EditorTabs()
                                .Fields(siteModel: siteModel, ss: ss))
                        .Hidden(controlId: "MethodType", value: "edit")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetPermissionEditor",
                        text: Displays.PermissionSetting())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Fields(
            this HtmlBuilder hb, SiteModel siteModel, SiteSettings ss)
        {
            SetPermissionCollectionSession(siteModel);
            return hb.FieldSet(
                id: "FieldSetPermissionEditor",
                action: () => hb
                    .Inherit(siteModel: siteModel)
                    .Div(id: "Selectables", action: () => hb
                        .Selectables(siteModel: siteModel))
                    .MainCommands(
                        siteId: siteModel.SiteId,
                        pt: siteModel.PermissionType,
                        verType: Versions.VerTypes.Latest,
                        updateButton: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Inherit(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.FieldDropDown(
                    controlId: "Sites_InheritPermission",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Sites_InheritPermission(),
                    optionCollection: InheritTargets(siteModel.SiteId),
                    selectedValue: siteModel.InheritPermission.ToString(),
                    action: "ChangeInherit",
                    method: "put")
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> InheritTargets(long siteId)
        {
            return new Dictionary<string, ControlData>
            {
                { siteId.ToString(), new ControlData(Displays.NotInheritPermission()) },
            }.AddRange(Rds.ExecuteTable(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    join: Rds.SitesJoinDefault(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId(siteId, _operator: "<>")
                        .InheritPermission(raw: "[Sites].[SiteId]")
                        .PermissionType(_operator: " is not null "),
                    orderBy: Rds.SitesOrderBy().Title()))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["SiteId"].ToString(),
                            o => new ControlData(o["Title"].ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Selectables(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId == siteModel.InheritPermission
                ? hb
                    .Destinations(
                        permissionCollection: siteModel.Session_PermissionDestinationCollection())
                    .Sources(
                        permissionCollection: siteModel.Session_PermissionSourceCollection())
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetPermissionCollectionSession(SiteModel siteModel)
        {
            siteModel.Session_PermissionDestinationCollection(
                DestinationCollection("Sites", siteModel.SiteId));
            siteModel.Session_PermissionSourceCollection(
                SourceCollection("Sites", siteModel.SiteId, Forms.Data("SearchText")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "PermissionDestination",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                labelText: Displays.PermissionDestination(),
                listItemCollection: permissionCollection.OrderBy(o => o.PermissionId)
                    .ToDictionary(
                        o => o.PermissionId,
                        o => new ControlData(
                            o.PermissionTitle + " - [" + o.PermissionTypeName + "]")),
                selectedValueCollection: new List<string>(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "ReadOnly",
                            controlCss: "button-icon post",
                            text: Displays.ReadOnly(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-person",
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "ReadWrite",
                            controlCss: "button-icon post",
                            text: Displays.ReadWrite(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-person",
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Leader",
                            controlCss: "button-icon post",
                            text: Displays.Leader(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-person",
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Manager",
                            controlCss: "button-icon post",
                            text: Displays.Manager(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-person",
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Delete",
                            controlCss: "button-icon post",
                            text: Displays.DeletePermission(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "Set",
                            method: "delete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Sources(
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "PermissionSource",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h550",
                labelText: Displays.PermissionSource(),
                listItemCollection: permissionCollection
                    .OrderBy(o => o.PermissionId)
                    .ToDictionary(
                        o => o.PermissionId,
                        o => new ControlData(o.PermissionTitle)),
                selectedValueCollection: new List<string>(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "Add",
                            controlCss: "button-icon post",
                            text: Displays.AddPermission(),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-circle-triangle-w",
                            action: "Set",
                            method: "post")
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchText",
                            controlCss: " auto-postback w100",
                            placeholder: Displays.Search(),
                            action: "Search",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection DestinationCollection(
            string referenceType, long referenceId)
        {
            return new PermissionCollection(
                where: Rds.PermissionsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection SourceCollection(
            string referenceType, long referenceId, string searchText)
        {
            return !searchText.IsNullOrEmpty()
                ? GetSourceCollection(referenceType, referenceId, searchText)
                : new PermissionCollection(get: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection GetSourceCollection(
            string referenceType, long referenceId, string searchText)
        {
            var sourceCollection = new PermissionCollection(get: false);
            Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectDepts(
                    column: Rds.DeptsColumn()
                        .DeptId()
                        .DeptName(),
                    where: Rds.DeptsWhere()
                        .TenantId(Sessions.TenantId())
                        .DeptId(_operator: ">0")
                        .SqlWhereLike(
                            searchText,
                            Rds.Depts_DeptId_WhereLike(),
                            Rds.Depts_DeptCode_WhereLike(),
                            Rds.Depts_DeptName_WhereLike(),
                            Rds.Depts_Body_WhereLike())))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                    sourceCollection.Add(new PermissionModel(
                                        referenceType,
                                        referenceId,
                                        Permissions.Types.ReadWrite,
                                        dataRow)));
            Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectGroups(
                    column: Rds.GroupsColumn()
                        .GroupId()
                        .GroupName(),
                    where: Rds.GroupsWhere()
                        .TenantId(Sessions.TenantId())
                        .GroupId(_operator: ">0")
                        .SqlWhereLike(
                            searchText,
                            Rds.Groups_GroupId_WhereLike(),
                            Rds.Groups_GroupName_WhereLike(),
                            Rds.Groups_Body_WhereLike())))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                    sourceCollection.Add(new PermissionModel(
                                        referenceType,
                                        referenceId,
                                        Permissions.Types.ReadWrite,
                                        dataRow)));
            Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .UserId()
                        .FullName1()
                        .FullName2()
                        .FirstAndLastNameOrder(),
                    where: Rds.UsersWhere()
                        .TenantId(Sessions.TenantId())
                        .UserId(_operator: ">0")
                        .SqlWhereLike(
                            searchText,
                            Rds.Users_UserId_WhereLike(),
                            Rds.Users_LoginId_WhereLike(),
                            Rds.Users_UserCode_WhereLike(),
                            Rds.Users_FirstName_WhereLike(),
                            Rds.Users_LastName_WhereLike(),
                            Rds.Users_DeptId_WhereLike())))
                                .AsEnumerable()
                                .ForEach(dataRow =>
                                    sourceCollection.Add(new PermissionModel(
                                        referenceType,
                                        referenceId,
                                        Permissions.Types.ReadWrite,
                                        dataRow)));
            return sourceCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string PermissionListItem(
            SiteModel siteModel,
            Types sourceOrDestination,
            List<string> selectedValueTextCollection = null)
        {
            switch (sourceOrDestination)
            {
                case Types.Destination:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: siteModel.Session_PermissionDestinationCollection()
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(
                                o => o.PermissionId,
                                o => new ControlData(
                                    o.PermissionTitle + " - [" + o.PermissionTypeName + "]")),
                        selectedValueTextCollection:
                            selectedValueTextCollection ?? new List<string>()).ToString();
                case Types.Source:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: siteModel.Session_PermissionSourceCollection()
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(
                                o => o.PermissionId,
                                o => new ControlData(o.PermissionTitle)),
                        selectedValueTextCollection:
                            selectedValueTextCollection ?? new List<string>()).ToString();
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangeInherit(long siteId)
        {
            var inheritPermission = Forms.Long("Sites_InheritPermission");
            var hb = new HtmlBuilder();
            if (siteId == inheritPermission)
            {
                var inheritSite = new SiteModel(siteId).InheritSite();
                SetPermissionCollectionSession(inheritSite);
                hb.Selectables(inheritSite);
            }
            return new ResponseCollection()
                .Html("#Selectables", hb)
                .SetFormData("Sites_InheritPermission", inheritPermission).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Update(long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            if (siteModel.PermissionType.CanManagePermission())
            {
                var statements = new List<SqlStatement>();
                statements.Add(Rds.PhysicalDeletePermissions(
                    where: Rds.PermissionsWhere().ReferenceId(siteId)));
                if (siteModel.InheritPermission == siteId)
                {
                    siteModel.Session_PermissionDestinationCollection()
                        .ForEach(permissionModel =>
                            statements.Add(Insert(permissionModel, siteId)));
                }
                statements.Add(Rds.UpdateSites(
                    verUp: false,
                    where: Rds.SitesWhere().SiteId(siteModel.SiteId),
                    param: Rds.SitesParam().InheritPermission(siteModel.InheritPermission)));
                Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
                SiteInfo.SetSiteUserHash(siteModel.InheritPermission, reload: true);
                return Messages.ResponseUpdated("permissions").ToJson();
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlInsert Insert(PermissionModel permissionModel, long siteId)
        {
            return Rds.InsertPermissions(param: Rds.PermissionsParam()
                .ReferenceType(raw: "'Sites'")
                .ReferenceId(raw: siteId.ToString())
                .PermissionType(raw: permissionModel.PermissionType.ToLong().ToString())
                .DeptId(raw: permissionModel.DeptId.ToString())
                .GroupId(raw: permissionModel.GroupId.ToString())
                .UserId(raw: permissionModel.UserId.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Set(long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            var res = new ResponseCollection();
            var permissionDestination = Forms.List("PermissionDestination");
            var permissionSource = Forms.List("PermissionSource");
            if (Forms.ControlId() != "Add" &&
                permissionDestination.Contains("User," + Sessions.UserId()))
            {
                res.Message(Messages.PermissionNotSelfChange());
            }
            else
            {
                switch (Forms.ControlId())
                {
                    case "ReadOnly":
                        res.SetPermissionType(
                            siteModel,
                            permissionDestination,
                            Permissions.Types.ReadOnly);
                        break;
                    case "ReadWrite":
                        res.SetPermissionType(
                            siteModel,
                            permissionDestination,
                            Permissions.Types.ReadWrite);
                        break;
                    case "Leader":
                        res.SetPermissionType(
                            siteModel,
                            permissionDestination,
                            Permissions.Types.Leader);
                        break;
                    case "Manager":
                        res.SetPermissionType(
                            siteModel,
                            permissionDestination,
                            Permissions.Types.Manager);
                        break;
                    case "Add":
                        siteModel.Session_PermissionDestinationCollection().AddRange(
                            siteModel.Session_PermissionSourceCollection().Where(o =>
                                permissionSource.Contains(o.PermissionId)));
                        siteModel.Session_PermissionDestinationCollection().Where(o =>
                            permissionSource.Contains(o.PermissionId))
                                .ForEach(o =>
                                    o.PermissionType = Permissions.Types.ReadWrite);
                        siteModel.Session_PermissionSourceCollection().RemoveAll(o =>
                            permissionSource.Contains(o.PermissionId));
                        res
                            .Html("#PermissionDestination", PermissionListItem(
                                siteModel, Types.Destination,
                                permissionSource))
                            .Html("#PermissionSource", PermissionListItem(siteModel, Types.Source))
                            .SetFormData("PermissionDestination", permissionSource.ToJson())
                            .SetFormData("PermissionSource", string.Empty);
                        break;
                    case "Delete":
                        siteModel.Session_PermissionSourceCollection().AddRange(
                            siteModel.Session_PermissionDestinationCollection().Where(o =>
                                permissionDestination.Contains(o.PermissionId)));
                        siteModel.Session_PermissionDestinationCollection().RemoveAll(o =>
                            permissionDestination.Contains(o.PermissionId));
                        res
                            .Html("#PermissionDestination", PermissionListItem(
                                siteModel, Types.Destination))
                            .Html("#PermissionSource", PermissionListItem(
                                siteModel, Types.Source,
                                permissionDestination))
                            .SetFormData("PermissionDestination", string.Empty)
                            .SetFormData("PermissionSource", permissionDestination.ToJson());
                        break;
                }
            }
            return res.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Search(long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            var res = new ResponseCollection();
            var permissionDestination = Forms.Data("PermissionDestination")
                .Deserialize<List<string>>()?
                .Where(o => o != string.Empty)
                .ToList();
            siteModel.Session_PermissionSourceCollection(
                SourceCollection("Sites", siteModel.SiteId, Forms.Data("SearchText")));
            siteModel.Session_PermissionSourceCollection().RemoveAll(o =>
                siteModel.Session_PermissionDestinationCollection()
                    .Any(p => p.PermissionId == o.PermissionId));
            res.Html("#PermissionSource", PermissionListItem(
                siteModel, Types.Source,
                permissionDestination));
            return res.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetPermissionType(
            this ResponseCollection res,
            SiteModel siteModel,
            List<string> permissionDestination,
            Permissions.Types pt)
        {
            permissionDestination.ForEach(permissionType_ItemId =>
                siteModel.Session_PermissionDestinationCollection()
                    .Where(o => (o.PermissionId == permissionType_ItemId))
                    .First()
                    .PermissionType = pt);
            res.Html("#PermissionDestination", PermissionListItem(
                siteModel,
                Types.Destination,
                permissionDestination));
        }
    }
}
