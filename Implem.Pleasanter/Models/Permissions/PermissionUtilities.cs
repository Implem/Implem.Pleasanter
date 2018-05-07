using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
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
        public static string Permission(long referenceId)
        {
            var controlId = Forms.ControlId();
            var selector = "#" + controlId;
            var itemModel = new ItemModel(referenceId);
            var siteModel = new SiteModel(itemModel.SiteId, setByForm: true);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, referenceId);
            var invalid = PermissionValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var hb = new HtmlBuilder();
            var permissions = SourceCollection(
                ss: siteModel.SiteSettings,
                searchText: Forms.Data("SearchPermissionElements"),
                currentPermissions: CurrentPermissions(referenceId));
            var offset = Forms.Int("SourcePermissionsOffset");
            switch (controlId)
            {
                case "SourcePermissions":
                    return new ResponseCollection()
                        .Append(selector, hb.SelectableItems(
                            listItemCollection: permissions
                                .Page(offset)
                                .ListItemCollection(siteModel.SiteSettings)))
                        .Val("#SourcePermissionsOffset", Paging.NextOffset(
                            offset, permissions.Count(), Parameters.Permissions.PageSize)
                                .ToString())
                        .ToJson();
                default:
                    return new ResponseCollection()
                        .Html(selector, hb.Permission(
                            siteModel: siteModel,
                            referenceId: referenceId,
                            site: itemModel.ReferenceType == "Sites"))
                        .Val("#SourcePermissionsOffset", Parameters.Permissions.PageSize)
                        .RemoveAttr(selector, "data-action")
                        .Invoke("setPermissionEvents")
                        .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<Permission> Page(this List<Permission> permissions, int offset)
        {
            return permissions
                .Skip(offset)
                .Take(Parameters.Permissions.PageSize)
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> ListItemCollection(
            this List<Permission> permissions, SiteSettings ss)
        {
            return permissions.ToDictionary(o => o.Key(), o => o.ControlData(ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Permission(
            this HtmlBuilder hb, SiteModel siteModel, long referenceId, bool site)
        {
            return hb.FieldSet(
                id: "FieldSetPermissionEditor",
                css: " enclosed",
                legendText: Displays.PermissionSetting(),
                action: () => hb
                    .Inherit(siteModel: siteModel, site: site)
                    .Div(id: "PermissionEditor", action: () => hb
                        .PermissionEditor(
                            ss: siteModel.SiteSettings,
                            referenceId: referenceId,
                            _using: !site || siteModel.SiteId == siteModel.InheritPermission)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Inherit(this HtmlBuilder hb, SiteModel siteModel, bool site)
        {
            return site && siteModel.SiteId != 0
                ? hb.FieldDropDown(
                    controlId: "InheritPermission",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.InheritPermission(),
                    optionCollection: InheritTargets(siteModel.SiteId),
                    selectedValue: siteModel.InheritPermission.ToString(),
                    action: "SetPermissions",
                    method: "post")
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
            }.AddRange(InheritTargetsDataRows(siteId)
                .ToDictionary(
                    o => o["SiteId"].ToString(),
                    o => new ControlData(o["Title"].ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static EnumerableRowCollection<DataRow> InheritTargetsDataRows(long siteId)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    join: Rds.SitesJoinDefault(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId(siteId, _operator: "<>")
                        .InheritPermission(raw: "[Sites].[SiteId]")
                        .Add(
                            raw: Def.Sql.CanReadSites,
                            _using: !Permissions.HasPrivilege()),
                    orderBy: Rds.SitesOrderBy().Title()))
                        .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionEditor(
            this HtmlBuilder hb, SiteSettings ss, long referenceId, bool _using)
        {
            var currentPermissions = CurrentCollection(referenceId);
            var sourcePermissions = SourceCollection(
                ss: ss,
                searchText: Forms.Data("SearchPermissionElements"),
                currentPermissions: currentPermissions);
            var offset = Forms.Int("PermissionSourceOffset");
            return _using
                ? hb
                    .CurrentPermissions(
                        ss: ss,
                        permissions: currentPermissions)
                    .SourcePermissions(
                        ss: ss,
                        permissions: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(ss),
                        offset: offset,
                        totalCount: sourcePermissions.Count())
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CurrentPermissions(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<Permission> permissions)
        {
            return hb.FieldSelectable(
                controlId: "CurrentPermissions",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlCss: " send-all",
                labelText: Displays.Permissions(),
                listItemCollection: permissions.ToDictionary(
                    o => o.Key(), o => o.ControlData(ss)),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "OpenPermissionsDialog",
                            controlCss: "button-icon post",
                            text: Displays.AdvancedSetting(),
                            onClick: "$p.openPermissionsDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "OpenPermissionsDialog",
                            method: "post")
                        .Button(
                            controlId: "DeletePermissions",
                            controlCss: "button-icon post",
                            text: Displays.DeletePermission(),
                            onClick: "$p.setPermissions($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "SetPermissions",
                            method: "delete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SourcePermissions(
            this HtmlBuilder hb,
            SiteSettings ss,
            Dictionary<string, ControlData> permissions,
            int offset = 0,
            int totalCount = 0)
        {
            return hb
                .FieldSelectable(
                    controlId: "SourcePermissions",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h300",
                    labelText: Displays.OptionList(),
                    listItemCollection: permissions,
                    commandOptionPositionIsTop: true,
                    action: "Permissions",
                    method: "post",
                    commandOptionAction: () => hb
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "AddPermissions",
                                controlCss: "button-icon post",
                                text: Displays.AddPermission(),
                                onClick: "$p.setPermissions($(this));",
                                icon: "ui-icon-circle-triangle-w",
                                action: "SetPermissions",
                                method: "post")
                            .TextBox(
                                controlId: "SearchPermissionElements",
                                controlCss: " auto-postback w100",
                                placeholder: Displays.Search(),
                                action: "SearchPermissionElements",
                                method: "post")
                            .Button(
                                text: Displays.Search(),
                                controlCss: "button-icon",
                                onClick: "$p.send($('#SearchPermissionElements'));",
                                icon: "ui-icon-search")))
                .Hidden(
                    controlId: "SourcePermissionsOffset",
                    css: "always-send",
                    value: Paging.NextOffset(offset, totalCount, Parameters.Permissions.PageSize)
                        .ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<Permission> CurrentCollection(long referenceId)
        {
            return Rds.ExecuteTable(statements: Rds.SelectPermissions(
                column: Rds.PermissionsColumn()
                    .DeptId()
                    .GroupId()
                    .UserId()
                    .PermissionType(),
                where: Rds.PermissionsWhere().ReferenceId(referenceId),
                orderBy: Rds.PermissionsOrderBy()
                    .UserId()
                    .GroupId()
                    .DeptId()))
                        .AsEnumerable()
                        .Select(dataRow => new Permission(dataRow))
                        .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<Permission> SourceCollection(
            SiteSettings ss,
            string searchText,
            List<Permission> currentPermissions,
            int offset = 0)
        {
            var sourceCollection = new List<Permission>();
            Rds.ExecuteTable(statements: new SqlStatement[]
            {
                Rds.SelectDepts(
                    column: Rds.DeptsColumn()
                        .DeptId(_as: "Id")
                        .Add(columnBracket: "'Dept' as [Name]"),
                    where: Rds.DeptsWhere()
                        .TenantId(Sessions.TenantId())
                        .DeptId(_operator: ">0")
                        .SqlWhereLike(
                            name: "SearchText",
                            searchText: searchText,
                            clauseCollection: new List<string>()
                            {
                                Rds.Depts_DeptCode_WhereLike(),
                                Rds.Depts_DeptName_WhereLike(),
                                Rds.Depts_Body_WhereLike()
                            })),
                Rds.SelectGroups(
                    column: Rds.GroupsColumn()
                        .GroupId(_as: "Id")
                        .Add(columnBracket: "'Group' as [Name]"),
                    where: Rds.GroupsWhere()
                        .TenantId(Sessions.TenantId())
                        .GroupId(_operator: ">0")
                        .SqlWhereLike(
                            name: "SearchText",
                            searchText: searchText,
                            clauseCollection: new List<string>()
                            {
                                Rds.Groups_GroupId_WhereLike(),
                                Rds.Groups_GroupName_WhereLike(),
                                Rds.Groups_Body_WhereLike()
                            }),
                    unionType: Sqls.UnionTypes.UnionAll),
                Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .UserId(_as: "Id")
                        .Add(columnBracket: "'User' as [Name]"),
                    join: Rds.UsersJoin()
                        .Add(new SqlJoin(
                            tableBracket: "[Depts]",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "[Users].[DeptId]=[Depts].[DeptId]")),
                    where: Rds.UsersWhere()
                        .TenantId(Sessions.TenantId())
                        .UserId(_operator: ">0")
                        .SqlWhereLike(
                            name: "SearchText",
                            searchText: searchText,
                            clauseCollection: new List<string>()
                            {
                                Rds.Users_LoginId_WhereLike(),
                                Rds.Users_Name_WhereLike(),
                                Rds.Users_UserCode_WhereLike(),
                                Rds.Users_Body_WhereLike(),
                                Rds.Depts_DeptCode_WhereLike(),
                                Rds.Depts_DeptName_WhereLike(),
                                Rds.Depts_Body_WhereLike()
                            })
                        .Users_Disabled(0),
                    unionType: Sqls.UnionTypes.UnionAll)
            })
                .AsEnumerable()
                .ForEach(dataRow =>
                    sourceCollection.Add(
                        new Permission(
                            ss: ss,
                            name: dataRow.String("Name"),
                            id: dataRow.Int("Id"),
                            source: true)));
            return sourceCollection
                .Where(o => !currentPermissions.Any(p => p.NameAndId() == o.NameAndId()))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string PermissionListItem(
            SiteSettings ss,
            IEnumerable<Permission> permissions,
            IEnumerable<string> selectedValueTextCollection = null,
            bool withType = true)
        {
            return new HtmlBuilder().SelectableItems(
                listItemCollection: permissions.ToDictionary(
                    o => o.Key(), o => o.ControlData(ss, withType: withType)),
                selectedValueTextCollection: permissions
                    .Where(o => selectedValueTextCollection?.Any(p =>
                        Same(p, o.Key())) == true)
                    .Select(o => o.Key())).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void CreatePermissions(
            this List<SqlStatement> statements, SiteSettings ss, Dictionary<string, User> users)
        {
            var insertSet = new List<PermissionModel>();
            ss.PermissionForCreating?.ForEach(data =>
            {
                switch (data.Key)
                {
                    case "Dept":
                        insertSet.Add(new PermissionModel(
                            referenceId: 0,
                            deptId: Sessions.User().DeptId,
                            groupId: 0,
                            userId: 0,
                            permissionType: data.Value));
                        break;
                    case "Group":
                        Groups().ForEach(groupId =>
                            insertSet.Add(new PermissionModel(
                                referenceId: 0,
                                deptId: 0,
                                groupId: groupId,
                                userId: 0,
                                permissionType: data.Value)));
                        break;
                    case "User":
                        insertSet.Add(new PermissionModel(
                            referenceId: 0,
                            deptId: 0,
                            groupId: 0,
                            userId: Sessions.UserId(),
                            permissionType: data.Value));
                        break;
                    default:
                        if (users.ContainsKey(data.Key))
                        {
                            var user = users[data.Key];
                            if (!user.Anonymous())
                            {
                                insertSet.Add(new PermissionModel(
                                    referenceId: 0,
                                    deptId: 0,
                                    groupId: 0,
                                    userId: user.Id,
                                    permissionType: data.Value));
                            }
                        }
                        break;
                }
            });
            statements.AddRange(insertSet
                .OrderByDescending(o => o.PermissionType)
                .GroupBy(o => o.DeptId + "," + o.GroupId + "," + o.UserId)
                .Select(o => Insert(o.First())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<int> Groups()
        {
            return Rds.ExecuteTable(statements: Rds.SelectGroups(
                column: Rds.GroupsColumn().GroupId(),
                where: Rds.GroupsWhere()
                    .GroupId_In(sub: Rds.SelectGroupMembers(
                        column: Rds.GroupMembersColumn().GroupId(),
                        where: Rds.GroupMembersWhere()
                            .Or(Rds.GroupMembersWhere()
                                .DeptId(Sessions.DeptId())
                                .UserId(Sessions.UserId()))))))
                                    .AsEnumerable()
                                    .Select(o => o["GroupId"].ToInt());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void UpdatePermissions(
            this List<SqlStatement> statements,
            SiteSettings ss,
            long referenceId,
            IEnumerable<string> permissions,
            bool site = false)
        {
            statements.Add(Rds.PhysicalDeletePermissions(
                where: Rds.PermissionsWhere().ReferenceId(referenceId)));
            if (!site || site && ss.InheritPermission == ss.SiteId)
            {
                new PermissionCollection(referenceId, permissions)
                    .ForEach(permissionModel =>
                        statements.Add(Insert(permissionModel)));
            }
            if (site)
            {
                statements.Add(StatusUtilities.UpdateStatus(
                    StatusUtilities.Types.PermissionsUpdated));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlInsert Insert(PermissionModel permissionModel)
        {
            return Rds.InsertPermissions(param: Rds.PermissionsParam()
                .ReferenceId(raw: permissionModel.ReferenceId == 0
                    ? Def.Sql.Identity
                    : permissionModel.ReferenceId.ToString())
                .PermissionType(raw: permissionModel.PermissionType.ToLong().ToString())
                .DeptId(raw: permissionModel.DeptId.ToString())
                .GroupId(raw: permissionModel.GroupId.ToString())
                .UserId(raw: permissionModel.UserId.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SetPermissions(long referenceId)
        {
            var itemModel = new ItemModel(referenceId);
            var siteModel = new SiteModel(itemModel.SiteId, setByForm: true);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, referenceId);
            var invalid = PermissionValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var res = new ResponseCollection();
            var selectedCurrentPermissions = Forms.List("CurrentPermissions");
            var selectedSourcePermissions = Forms.List("SourcePermissions");
            if (Forms.ControlId() != "AddPermissions" &&
                selectedCurrentPermissions.Any(o =>
                    o.StartsWith("User," + Sessions.UserId() + ",")))
            {
                res.Message(Messages.PermissionNotSelfChange());
            }
            else
            {
                var currentPermissions = Forms.Exists("CurrentPermissionsAll")
                    ? Permissions.Get(Forms.List("CurrentPermissionsAll"))
                    : CurrentCollection(referenceId);
                switch (Forms.ControlId())
                {
                    case "InheritPermission":
                        res.InheritPermission(itemModel, siteModel);
                        break;
                    case "AddPermissions":
                        res.AddPermissions(
                            siteModel: siteModel,
                            selectedSourcePermissions: selectedSourcePermissions,
                            currentPermissions: currentPermissions);
                        break;
                    case "PermissionPattern":
                        res.ReplaceAll(
                            "#PermissionParts",
                            new HtmlBuilder().PermissionParts(
                                controlId: "PermissionParts",
                                labelText: Displays.Permissions(),
                                permissionType: (Permissions.Types)Forms.Long(
                                    "PermissionPattern")));
                        break;
                    case "ChangePermissions":
                        res.ChangePermissions(
                            ss: siteModel.SiteSettings,
                            selector: "#CurrentPermissions",
                            currentPermissions: currentPermissions,
                            selectedCurrentPermissions: selectedCurrentPermissions,
                            permissionType: GetPermissionTypeByForm());
                        break;
                    case "DeletePermissions":
                        res.DeletePermissions(
                            siteModel: siteModel,
                            selectedCurrentPermissions: selectedCurrentPermissions,
                            currentPermissions: currentPermissions);
                        break;
                }
            }
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InheritPermission(
            this ResponseCollection res,
            ItemModel itemModel,
            SiteModel siteModel)
        {
            var inheritPermission = Forms.Long("InheritPermission");
            var hb = new HtmlBuilder();
            if (siteModel.SiteId == inheritPermission)
            {
                var inheritSite = siteModel.InheritSite();
                hb.PermissionEditor(
                    ss: siteModel.SiteSettings,
                    referenceId: siteModel.InheritPermission,
                    _using:
                        itemModel.ReferenceType != "Sites" ||
                        siteModel.SiteId == inheritPermission);
            }
            res
                .Html("#PermissionEditor", hb)
                .SetData("#InheritPermission")
                .SetData("#CurrentPermissions")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AddPermissions(
            this ResponseCollection res,
            SiteModel siteModel,
            List<string> selectedSourcePermissions,
            List<Permission> currentPermissions)
        {
            currentPermissions.AddRange(
                Permissions.Get(selectedSourcePermissions));
            currentPermissions
                .Where(o => selectedSourcePermissions.Any(p => Same(p, o.Key())))
                .ForEach(o => o.Type = Permissions.General());
            var sourcePermissions = SourceCollection(
                ss: siteModel.SiteSettings,
                searchText: Forms.Data("SearchPermissionElements"),
                currentPermissions: currentPermissions);
            res
                .ScrollTop("#SourcePermissionsWrapper")
                .Html("#CurrentPermissions", PermissionListItem(
                    ss: siteModel.SiteSettings,
                    permissions: currentPermissions,
                    selectedValueTextCollection: selectedSourcePermissions))
                .Html("#SourcePermissions", PermissionListItem(
                    ss: siteModel.SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    withType: false))
                .Val("#SourcePermissionsOffset", Parameters.Permissions.PageSize)
                .SetData("#CurrentPermissions")
                .SetData("#SourcePermissions");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void ChangePermissions(
            this ResponseCollection res,
            SiteSettings ss,
            string selector,
            IEnumerable<Permission> currentPermissions,
            IEnumerable<string> selectedCurrentPermissions,
            Permissions.Types permissionType)
        {
            selectedCurrentPermissions.ForEach(o =>
                currentPermissions
                    .Where(p => Same(p.Key(), o))
                    .First()
                    .Type = permissionType);
            res
                .CloseDialog()
                .Html(selector, PermissionListItem(
                    ss: ss,
                    permissions: currentPermissions,
                    selectedValueTextCollection: selectedCurrentPermissions))
                .SetData(selector);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void DeletePermissions(
            this ResponseCollection res,
            SiteModel siteModel,
            List<string> selectedCurrentPermissions,
            List<Permission> currentPermissions)
        {
            currentPermissions.RemoveAll(o =>
                selectedCurrentPermissions.Any(p => Same(p, o.Key())));
            var sourcePermissions = SourceCollection(
                ss: siteModel.SiteSettings,
                searchText: Forms.Data("SearchPermissionElements"),
                currentPermissions: currentPermissions);
            res
                .Html("#CurrentPermissions", PermissionListItem(
                    ss: siteModel.SiteSettings,
                    permissions: currentPermissions))
                .Html("#SourcePermissions", PermissionListItem(
                    ss: siteModel.SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: selectedCurrentPermissions,
                    withType: false))
                .Val("#SourcePermissionsOffset", Parameters.Permissions.PageSize)
                .SetData("#CurrentPermissions")
                .SetData("#SourcePermissions");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchPermissionElements(long referenceId)
        {
            var itemModel = new ItemModel(referenceId);
            var siteModel = new SiteModel(itemModel.SiteId, setByForm: true);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, referenceId);
            var invalid = PermissionValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var res = new ResponseCollection();
            var currentPermissions = CurrentPermissions(referenceId);
            var sourcePermissions = SourceCollection(
                ss: siteModel.SiteSettings,
                searchText: Forms.Data("SearchPermissionElements"),
                currentPermissions: currentPermissions);
            return res
                .Html("#SourcePermissions", PermissionListItem(
                    ss: siteModel.SiteSettings,
                    permissions: sourcePermissions.Page(0),
                    selectedValueTextCollection: Forms.Data("SourcePermissions")
                        .Deserialize<List<string>>()?
                        .Where(o => o != string.Empty),
                    withType: false))
                .Val("#SourcePermissionsOffset", Parameters.Permissions.PageSize)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<Permission> CurrentPermissions(long referenceId)
        {
            return Forms.Exists("CurrentPermissionsAll")
                ? Permissions.Get(Forms.List("CurrentPermissionsAll"))
                : CurrentCollection(referenceId).ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenPermissionsDialog(long referenceId)
        {
            var res = new ResponseCollection();
            var selected = Forms.List("CurrentPermissions");
            if (selected.Any(o => o.StartsWith("User," + Sessions.UserId() + ",")))
            {
                return res.Message(Messages.PermissionNotSelfChange()).ToJson();
            }
            else if (!selected.Any())
            {
                return res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                return res.Html("#PermissionsDialog", PermissionsDialog(
                    (Permissions.Types)selected.FirstOrDefault().Split_3rd().ToLong(),
                    referenceId)).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PermissionsDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("PermissionsDialog")
                .Class("dialog")
                .Title(Displays.AdvancedSetting()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionsDialog(
            Permissions.Types permissionType, long referenceId)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("PermissionsForm")
                    .Action(Locations.ItemAction(referenceId)),
                action: () => hb
                    .FieldDropDown(
                        controlId: "PermissionPattern",
                        controlCss: " auto-postback",
                        labelText: Displays.Pattern(),
                        optionCollection: Parameters.Permissions.Pattern
                            .ToDictionary(
                                o => o.Value.ToString(),
                                o => new ControlData(Displays.Get(o.Key))),
                        selectedValue: permissionType.ToLong().ToString(),
                        addSelectedValue: false,
                        action: "SetPermissions",
                        method: "post")
                    .PermissionParts(
                        controlId: "PermissionParts",
                        labelText: Displays.Permissions(),
                        permissionType: permissionType)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ChangePermissions",
                            text: Displays.Change(),
                            controlCss: "button-icon validate",
                            onClick: "$p.changePermissions($(this));",
                            icon: "ui-icon-disk",
                            action: "SetPermissions",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionParts(
            this HtmlBuilder hb, string controlId,
            string labelText,
            Permissions.Types permissionType)
        {
            return hb.FieldSet(
                id: controlId,
                css: " enclosed",
                legendText: labelText,
                action: () => hb
                    .PermissionPart(permissionType, Permissions.Types.Read)
                    .PermissionPart(permissionType, Permissions.Types.Create)
                    .PermissionPart(permissionType, Permissions.Types.Update)
                    .PermissionPart(permissionType, Permissions.Types.Delete)
                    .PermissionPart(permissionType, Permissions.Types.SendMail)
                    .PermissionPart(permissionType, Permissions.Types.Export)
                    .PermissionPart(permissionType, Permissions.Types.Import)
                    .PermissionPart(permissionType, Permissions.Types.ManageSite)
                    .PermissionPart(permissionType, Permissions.Types.ManagePermission));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionPart(
            this HtmlBuilder hb, Permissions.Types permissionType, Permissions.Types type)
        {
            return hb.FieldCheckBox(
                controlId: type.ToString(),
                fieldCss: "field-auto-thin w200",
                controlCss: " always-send",
                labelText: Displays.Get(type.ToString()),
                _checked: (permissionType & type) > 0,
                dataId: type.ToLong().ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool Same(string value1, string value2)
        {
            return
                value1.Split_1st() == value2.Split_1st() &&
                value1.Split_2nd() == value2.Split_2nd();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Permissions.Types GetPermissionTypeByForm()
        {
            var permissionType = Permissions.Types.NotSet;
            if (Forms.Bool("Read")) permissionType |= Permissions.Types.Read;
            if (Forms.Bool("Create")) permissionType |= Permissions.Types.Create;
            if (Forms.Bool("Update")) permissionType |= Permissions.Types.Update;
            if (Forms.Bool("Delete")) permissionType |= Permissions.Types.Delete;
            if (Forms.Bool("SendMail")) permissionType |= Permissions.Types.SendMail;
            if (Forms.Bool("Export")) permissionType |= Permissions.Types.Export;
            if (Forms.Bool("Import")) permissionType |= Permissions.Types.Import;
            if (Forms.Bool("ManageSite")) permissionType |= Permissions.Types.ManageSite;
            if (Forms.Bool("ManagePermission")) permissionType |= Permissions.Types.ManagePermission;
            return permissionType;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PermissionForCreating(long referenceId)
        {
            var ss = SiteSettingsUtilities.Get(new ItemModel(referenceId).GetSite(), referenceId);
            if (!ss.CanManagePermission())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            return new ResponseCollection()
                .Html(
                    "#FieldSetRecordAccessControl",
                    new HtmlBuilder().PermissionForCreating(ss))
                .RemoveAttr("#FieldSetRecordAccessControl", "data-action")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionForCreating(this HtmlBuilder hb, SiteSettings ss)
        {
            var permissions = PermissionForCreating(ss);
            return hb.FieldSet(
                id: "FieldSetRecordAccessControl",
                css: " enclosed",
                legendText: Displays.PermissionForCreating(),
                action: () => hb
                    .Div(
                        id: "PermissionForCreating",
                        action: () => hb
                            .CurrentPermissionForCreating(
                                ss: ss,
                                permissions: permissions.Where(o => !o.Source))
                            .SourcePermissionForCreating(
                                ss: ss,
                                permissions: permissions.Where(o => o.Source))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CurrentPermissionForCreating(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<Permission> permissions)
        {
            return hb.FieldSelectable(
                controlId: "CurrentPermissionForCreating",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlCss: " send-all",
                labelText: Displays.CurrentSettings(),
                listItemCollection: permissions.ToDictionary(
                    o => o.Key(), o => o.ControlData(ss)),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "OpenPermissionForCreatingDialog",
                            controlCss: "button-icon post",
                            text: Displays.AdvancedSetting(),
                            onClick: "$p.openPermissionForCreatingDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "OpenPermissionForCreatingDialog",
                            method: "post")
                        .Button(
                            controlId: "DeletePermissionForCreating",
                            controlCss: "button-icon post",
                            text: Displays.ToDisable(),
                            onClick: "$p.setPermissionForCreating($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "SetPermissionForCreating",
                            method: "delete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SourcePermissionForCreating(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<Permission> permissions)
        {
            return hb.FieldSelectable(
                controlId: "SourcePermissionForCreating",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                labelText: Displays.OptionList(),
                listItemCollection: permissions.ToDictionary(
                    o => o.Key(), o => o.ControlData(ss, withType: false)),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "AddPermissionForCreating",
                            controlCss: "button-icon post",
                            text: Displays.ToEnable(),
                            onClick: "$p.setPermissionForCreating($(this));",
                            icon: "ui-icon-circle-triangle-w",
                            action: "SetPermissionForCreating",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<Permission> PermissionForCreating(SiteSettings ss)
        {
            var type = (Permissions.Types)Parameters.Permissions.General;
            var permissions = new List<Permission>
            {
                ss.GetPermissionForCreating("Dept"),
                ss.GetPermissionForCreating("Group"),
                ss.GetPermissionForCreating("User")
            };
            permissions.AddRange(ss.Columns
                .Where(o => o.UserColumn)
                .Where(o => o.ColumnName != "Creator" && o.ColumnName != "Updator")
                .Select(o => ss.GetPermissionForCreating(o.ColumnName)));
            return permissions;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SetPermissionForCreating(long referenceId)
        {
            var itemModel = new ItemModel(referenceId);
            var siteModel = new SiteModel(itemModel.SiteId, setByForm: true);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, referenceId);
            var invalid = PermissionValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var res = new ResponseCollection();
            var selectedCurrentPermissionForCreating = Forms.List("CurrentPermissionForCreating");
            var selectedSourcePermissionForCreating = Forms.List("SourcePermissionForCreating");
            if (Forms.ControlId() != "AddPermissionForCreating" &&
                selectedCurrentPermissionForCreating.Any(o =>
                    o.StartsWith("User," + Sessions.UserId() + ",")))
            {
                res.Message(Messages.PermissionNotSelfChange());
            }
            else
            {
                var permissionForCreating = PermissionForCreating(siteModel.SiteSettings);
                var currentPermissionForCreating = Forms.Exists("CurrentPermissionForCreatingAll")
                    ? Permissions.Get(Forms.List("CurrentPermissionForCreatingAll"))
                    : permissionForCreating.Where(o => !o.Source).ToList();
                var sourcePermissionForCreating = permissionForCreating
                    .Where(o => !currentPermissionForCreating.Any(p =>
                        p.NameAndId() == o.NameAndId()))
                    .ToList();
                switch (Forms.ControlId())
                {
                    case "AddPermissionForCreating":
                        currentPermissionForCreating.AddRange(
                            Permissions.Get(selectedSourcePermissionForCreating,
                                Permissions.General()));
                        sourcePermissionForCreating.RemoveAll(o =>
                            selectedSourcePermissionForCreating.Any(p =>
                                Same(p, o.Key())));
                        res
                            .Html("#CurrentPermissionForCreating", PermissionListItem(
                                ss: siteModel.SiteSettings,
                                permissions: currentPermissionForCreating,
                                selectedValueTextCollection: selectedSourcePermissionForCreating))
                            .Html("#SourcePermissionForCreating", PermissionListItem(
                                ss: siteModel.SiteSettings,
                                permissions: sourcePermissionForCreating,
                                withType: false))
                            .SetData("#CurrentPermissionForCreating")
                            .SetData("#SourcePermissionForCreating");
                        break;
                    case "PermissionForCreatingPattern":
                        res.ReplaceAll(
                            "#PermissionForCreatingParts",
                            new HtmlBuilder().PermissionParts(
                                controlId: "PermissionForCreatingParts",
                                labelText: Displays.Permissions(),
                                permissionType: (Permissions.Types)Forms.Long(
                                    "PermissionForCreatingPattern")));
                        break;
                    case "ChangePermissionForCreating":
                        res.ChangePermissions(
                            ss: siteModel.SiteSettings,
                            selector: "#CurrentPermissionForCreating",
                            currentPermissions: currentPermissionForCreating,
                            selectedCurrentPermissions: selectedCurrentPermissionForCreating,
                            permissionType: GetPermissionTypeByForm());
                        break;
                    case "DeletePermissionForCreating":
                        sourcePermissionForCreating.AddRange(
                            currentPermissionForCreating.Where(o =>
                                selectedCurrentPermissionForCreating.Any(p =>
                                    Same(p, o.Key()))));
                        currentPermissionForCreating.RemoveAll(o =>
                            selectedCurrentPermissionForCreating.Any(p =>
                                Same(p, o.Key())));
                        res
                            .Html("#CurrentPermissionForCreating", PermissionListItem(
                                ss: siteModel.SiteSettings,
                                permissions: currentPermissionForCreating))
                            .Html("#SourcePermissionForCreating", PermissionListItem(
                                ss: siteModel.SiteSettings,
                                permissions: sourcePermissionForCreating,
                                selectedValueTextCollection: selectedCurrentPermissionForCreating,
                                withType: false))
                            .SetData("#CurrentPermissionForCreating")
                            .SetData("#SourcePermissionForCreating");
                        break;
                }
            }
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenPermissionForCreatingDialog(long referenceId)
        {
            var res = new ResponseCollection();
            var selected = Forms.List("CurrentPermissionForCreating");
            if (!selected.Any())
            {
                return res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                return res.Html("#PermissionForCreatingDialog", PermissionForCreatingDialog(
                    (Permissions.Types)selected.FirstOrDefault().Split_3rd().ToLong(),
                    referenceId)).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PermissionForCreatingDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("PermissionForCreatingDialog")
                .Class("dialog")
                .Title(Displays.AdvancedSetting()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionForCreatingDialog(
            Permissions.Types permissionType, long referenceId)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("PermissionForCreatingForm")
                    .Action(Locations.ItemAction(referenceId)),
                action: () => hb
                    .FieldDropDown(
                        controlId: "PermissionForCreatingPattern",
                        controlCss: " auto-postback",
                        labelText: Displays.Pattern(),
                        optionCollection: Parameters.Permissions.Pattern
                            .ToDictionary(
                                o => o.Value.ToString(),
                                o => new ControlData(Displays.Get(o.Key))),
                        selectedValue: permissionType.ToLong().ToString(),
                        addSelectedValue: false,
                        action: "SetPermissionForCreating",
                        method: "post")
                    .PermissionParts(
                        controlId: "PermissionForCreatingParts",
                        labelText: Displays.Permissions(),
                        permissionType: permissionType)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ChangePermissionForCreating",
                            text: Displays.Change(),
                            controlCss: "button-icon validate",
                            onClick: "$p.changePermissionForCreating($(this));",
                            icon: "ui-icon-disk",
                            action: "SetPermissionForCreating",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ColumnAccessControl(long referenceId)
        {
            var ss = SiteSettingsUtilities.Get(new ItemModel(referenceId).GetSite(), referenceId);
            if (!ss.CanManagePermission())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            return new ResponseCollection()
                .Html(
                    "#FieldSetColumnAccessControl",
                    new HtmlBuilder().ColumnAccessControl(ss))
                .RemoveAttr("#FieldSetColumnAccessControl", "data-action")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ColumnAccessControl(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.FieldSet(
                id: "FieldSetColumnAccessControl",
                css: " enclosed",
                legendText: Displays.ColumnAccessControl(),
                action: () => hb
                    .Div(
                        id: "ColumnAccessControl",
                        action: () => hb
                            .ColumnAccessControl(
                                ss: ss,
                                type: "Create",
                                labelText: Displays.CreateColumnAccessControl())
                            .ColumnAccessControl(
                                ss: ss,
                                type: "Read",
                                labelText: Displays.ReadColumnAccessControl())
                            .ColumnAccessControl(
                                ss: ss,
                                type: "Update",
                                labelText: Displays.UpdateColumnAccessControl())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ColumnAccessControl(
            this HtmlBuilder hb, SiteSettings ss, string type, string labelText)
        {
            return hb.FieldSelectable(
                controlId: type + "ColumnAccessControl",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                controlCss: " send-all",
                labelText: labelText,
                listItemCollection: ss.ColumnAccessControlOptions(type),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: type + "OpenColumnAccessControlDialog",
                            controlCss: "button-icon post",
                            text: Displays.AdvancedSetting(),
                            onClick: "$p.openColumnAccessControlDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "OpenColumnAccessControlDialog",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ColumnAccessControlDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("ColumnAccessControlDialog")
                .Class("dialog")
                .Title(Displays.AdvancedSetting()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SetColumnAccessControl(long referenceId)
        {
            var itemModel = new ItemModel(referenceId);
            var siteModel = new SiteModel(itemModel.SiteId, setByForm: true);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(siteModel, referenceId);
            var invalid = PermissionValidators.OnUpdating(siteModel.SiteSettings);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var res = new ResponseCollection();
            var type = Forms.Data("ColumnAccessControlType");
            var selected = Forms.List("ColumnAccessControl")
                .Select(o => o.Deserialize<ColumnAccessControl>())
                .ToList();
            var columnAccessControl = Forms.List("ColumnAccessControlAll")
                .Select(o => ColumnAccessControl(o.Deserialize<ColumnAccessControl>(), selected));
            var listItemCollection = siteModel.SiteSettings.ColumnAccessControlOptions(
                type, columnAccessControl);
            res
                .CloseDialog()
                .Html("#" + type + "ColumnAccessControl", new HtmlBuilder().SelectableItems(
                    listItemCollection: listItemCollection,
                    selectedValueTextCollection: columnAccessControl
                        .Where(o => selected.Any(p => p.ColumnName == o.ColumnName))
                        .Select(o => o.ToJson())))
                .SetData("#" + type + "ColumnAccessControl");
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ColumnAccessControl ColumnAccessControl(
            ColumnAccessControl columnAccessControl, List<ColumnAccessControl> selected)
        {
            var allowdUsers = new Dictionary<string, bool>
            {
                { "Creator", Forms.Bool("CreatorAllowed") },
                { "Updator", Forms.Bool("UpdatorAllowed") },
                { "Manager", Forms.Bool("ManagerAllowed") },
                { "Owner", Forms.Bool("OwnerAllowed") },
            }
            .Where(o => o.Value)
            .Select(o => o.Key);
            if (selected.Any(o => o.ColumnName == columnAccessControl.ColumnName))
            {
                columnAccessControl.AllowedType = GetPermissionTypeByForm();
                columnAccessControl.AllowedUsers = allowdUsers;
            }
            return columnAccessControl;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenColumnAccessControlDialog(long referenceId)
        {
            var ss = SiteSettingsUtilities.Get(new ItemModel(referenceId).GetSite(), referenceId);
            if (!ss.CanManagePermission())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var res = new ResponseCollection();
            var type = ColumnAccessControlType();
            var selected = Forms.List(type + "ColumnAccessControl");
            if (!selected.Any())
            {
                return res.Message(Messages.SelectTargets()).ToJson();
            }
            else
            {
                return res.Html("#ColumnAccessControlDialog", ColumnAccessControlDialog(
                    ss: ss,
                    type: type,
                    columnAccessControl: selected.FirstOrDefault()
                        .Deserialize<ColumnAccessControl>(),
                    referenceId: referenceId)).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ColumnAccessControlType()
        {
            switch (Forms.ControlId())
            {
                case "CreateOpenColumnAccessControlDialog": return "Create";
                case "ReadOpenColumnAccessControlDialog": return "Read";
                case "UpdateOpenColumnAccessControlDialog": return "Update";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ColumnAccessControlDialog(
            SiteSettings ss,
            string type,
            ColumnAccessControl columnAccessControl,
            long referenceId)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ColumnAccessControlForm")
                    .Action(Locations.ItemAction(referenceId)),
                action: () => hb
                    .PermissionParts(
                        controlId: "ColumnAccessControlParts",
                        labelText: Displays.RequiredPermission(),
                        permissionType: columnAccessControl.AllowedType)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.AllowedUsers(),
                        action: () => hb
                            .AllowedUser(
                                ss: ss,
                                columnAccessControl: columnAccessControl,
                                columnName: "Creator")
                            .AllowedUser(
                                ss: ss,
                                columnAccessControl: columnAccessControl,
                                columnName: "Updator")
                            .AllowedUser(
                                ss: ss,
                                columnAccessControl: columnAccessControl,
                                columnName: "Manager")
                            .AllowedUser(
                                ss: ss,
                                columnAccessControl: columnAccessControl,
                                columnName: "Owner"),
                        _using: type != "Create")
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ChangeColumnAccessControl",
                            text: Displays.Change(),
                            controlCss: "button-icon validate",
                            onClick: "$p.changeColumnAccessControl($(this), '" + type + "');",
                            icon: "ui-icon-disk",
                            action: "SetColumnAccessControl",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel"))
                    .Hidden(controlId: "ColumnAccessControlType", value: type)
                    .Hidden(
                        controlId: "ColumnAccessControlNullableOnly",
                        value: ss.ColumnAccessControlNullableOnly(type).ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder AllowedUser(
            this HtmlBuilder hb,
            SiteSettings ss,
            ColumnAccessControl columnAccessControl,
            string columnName)
        {
            return hb.FieldCheckBox(
                fieldCss: "field-auto-thin w200",
                controlCss: " always-send",
                controlId: columnName + "Allowed",
                labelText: ss.GetColumn(columnName)?.LabelText,
                _checked: columnAccessControl.AllowedUsers?.Contains(columnName) == true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool HasInheritedSites(long siteId)
        {
            return Rds.ExecuteScalar_long(statements: Rds.SelectSites(
                column: Rds.SitesColumn().SitesCount(),
                where: Rds.SitesWhere()
                    .SiteId(siteId, _operator: "<>")
                    .InheritPermission(siteId))) > 0;
        }
    }
}
