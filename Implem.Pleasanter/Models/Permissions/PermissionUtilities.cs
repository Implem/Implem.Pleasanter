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
            Current,
            Source
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long referenceId)
        {
            var itemModel = new ItemModel(referenceId);
            return new ResponseCollection()
                .Html(
                    "#FieldSetPermissions",
                    new HtmlBuilder().Editor(
                        siteModel: itemModel.GetSite(),
                        referenceId: referenceId,
                        site: itemModel.ReferenceType == "Sites"))
                .RemoveAttr("#FieldSetPermissions", "data-action")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
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
                        .Add(raw: Def.Sql.CanRead),
                    orderBy: Rds.SitesOrderBy().Title()))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["SiteId"].ToString(),
                            o => new ControlData(o["Title"].ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PermissionEditor(
            this HtmlBuilder hb, long referenceId, bool _using)
        {
            return _using
                ? hb
                    .CurrentPermissions(permissionCollection:
                        CurrentCollection(referenceId))
                    .SourcePermissions(permissionCollection:
                        SourceCollection(referenceId, Forms.Data("SearchPermissionElements")))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CurrentPermissions(
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "CurrentPermissions",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlCss: " send-all",
                labelText: Displays.Permissions(),
                listItemCollection: permissionCollection.OrderBy(o => o.PermissionId)
                    .ToDictionary(
                        o => o.PermissionId,
                        o => new ControlData(
                            o.PermissionTitle + " - [" + o.PermissionTypeName + "]")),
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
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "SourcePermissions",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                labelText: Displays.OptionList(),
                listItemCollection: permissionCollection
                    .OrderBy(o => o.PermissionId)
                    .ToDictionary(
                        o => o.PermissionId,
                        o => new ControlData(o.PermissionTitle)),
                commandOptionPositionIsTop: true,
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
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchPermissionElements",
                            controlCss: " auto-postback w100",
                            placeholder: Displays.Search(),
                            action: "SearchPermissionElements",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection CurrentCollection(long referenceId)
        {
            return new PermissionCollection(
                where: Rds.PermissionsWhere().ReferenceId(referenceId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection SourceCollection(long referenceId, string searchText)
        {
            return !searchText.IsNullOrEmpty()
                ? GetSourceCollection(referenceId, searchText)
                : new PermissionCollection(get: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection GetSourceCollection(
            long referenceId, string searchText)
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
                                        referenceId,
                                        Permissions.General(),
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
                                        referenceId,
                                        Permissions.General(),
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
                                        referenceId,
                                        Permissions.General(),
                                        dataRow)));
            return sourceCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string PermissionListItem(
            SiteModel siteModel,
            Types type,
            PermissionCollection permissionCollection,
            IEnumerable<string> selectedValueTextCollection = null)
        {
            switch (type)
            {
                case Types.Current:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: permissionCollection
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(
                                o => o.PermissionId,
                                o => new ControlData(
                                    o.PermissionTitle + " - [" + o.PermissionTypeName + "]")),
                        selectedValueTextCollection: permissionCollection
                            .Where(o => selectedValueTextCollection?.Any(p =>
                                Same(p, o.PermissionId)) == true)
                            .Select(o => o.PermissionId)).ToString();
                case Types.Source:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: permissionCollection
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(
                                o => o.PermissionId,
                                o => new ControlData(o.PermissionTitle)),
                        selectedValueTextCollection: permissionCollection
                            .Where(o => selectedValueTextCollection?.Any(p =>
                                Same(p, o.PermissionId)) == true)
                            .Select(o => o.PermissionId)).ToString();
                default:
                    return string.Empty;
            }
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
                        statements.Add(Insert(permissionModel, referenceId)));
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
        private static SqlInsert Insert(PermissionModel permissionModel, long referenceId)
        {
            return Rds.InsertPermissions(param: Rds.PermissionsParam()
                .ReferenceId(raw: referenceId.ToString())
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
                    ? new PermissionCollection(referenceId, Forms.List("CurrentPermissionsAll"))
                    : CurrentCollection(referenceId);
                var sourcePermissions = SourceCollection(
                    referenceId, Forms.Data("SearchPermissionElements"));
                sourcePermissions.RemoveAll(o => currentPermissions.Any(p =>
                    Same(p.PermissionId, o.PermissionId)));
                switch (Forms.ControlId())
                {
                    case "InheritPermission":
                        var inheritPermission = Forms.Long("InheritPermission");
                        var hb = new HtmlBuilder();
                        if (siteModel.SiteId == inheritPermission)
                        {
                            var inheritSite = siteModel.InheritSite();
                            hb.PermissionEditor(
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
                        break;
                    case "AddPermissions":
                        currentPermissions.AddRange(
                            new PermissionCollection(referenceId, selectedSourcePermissions));
                        currentPermissions
                            .Where(o => selectedSourcePermissions
                                .Any(p => Same(p, o.PermissionId)))
                            .ForEach(o => o.PermissionType = Permissions.General());
                        sourcePermissions.RemoveAll(o =>
                            selectedSourcePermissions.Contains(o.PermissionId));
                        res
                            .Html("#CurrentPermissions", PermissionListItem(
                                siteModel,
                                Types.Current,
                                currentPermissions,
                                selectedSourcePermissions))
                            .Html("#SourcePermissions", PermissionListItem(
                                siteModel, Types.Source, sourcePermissions))
                            .SetData("#CurrentPermissions")
                            .SetData("#SourcePermissions");
                        break;
                    case "PermissionPattern":
                        res.ReplaceAll(
                            "#PermissionParts",
                            new HtmlBuilder().PermissionParts(
                                (Permissions.Types)Forms.Long("PermissionPattern")));
                        break;
                    case "ChangePermissions":
                        res.ChangePermissions(
                            siteModel,
                            currentPermissions,
                            selectedCurrentPermissions,
                            GetPermissionTypeByForm());
                        break;
                    case "DeletePermissions":
                        sourcePermissions.AddRange(
                            currentPermissions.Where(o =>
                                selectedCurrentPermissions.Any(p =>
                                    Same(p, o.PermissionId))));
                        currentPermissions.RemoveAll(o =>
                            selectedCurrentPermissions.Any(p =>
                                Same(p, o.PermissionId)));
                        res
                            .Html("#CurrentPermissions", PermissionListItem(
                                siteModel, Types.Current, currentPermissions))
                            .Html("#SourcePermissions", PermissionListItem(
                                siteModel, Types.Source,
                                sourcePermissions,
                                selectedCurrentPermissions))
                            .SetData("#CurrentPermissions")
                            .SetData("#SourcePermissions");
                        break;
                }
            }
            return res.ToJson();
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
            var sourcePermissions = SourceCollection(
                referenceId, Forms.Data("SearchPermissionElements"));
            var currentPermissions = Forms.Data("CurrentPermissionsAll")
                .Deserialize<List<string>>()?
                .Where(o => o != string.Empty) ??
                    CurrentCollection(referenceId).Select(o => o.PermissionId);
            sourcePermissions.RemoveAll(o => currentPermissions.Any(p =>
                Same(p, o.PermissionId)));
            res.Html("#SourcePermissions", PermissionListItem(
                siteModel,
                Types.Source,
                sourcePermissions,
                Forms.Data("SourcePermissions")
                    .Deserialize<List<string>>()?
                    .Where(o => o != string.Empty)));
            return res.ToJson();
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
                    .PermissionParts(permissionType)
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
            this HtmlBuilder hb, Permissions.Types permissionType)
        {
            return hb.FieldSet(
                id: "PermissionParts",
                css: " enclosed",
                legendText: Displays.Permissions(),
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
        public static void ChangePermissions(
            this ResponseCollection res,
            SiteModel siteModel,
            PermissionCollection currentPermissions,
            IEnumerable<string> selectedCurrentPermissions,
            Permissions.Types permissionType)
        {
            selectedCurrentPermissions.ForEach(o =>
                currentPermissions
                    .Where(p => Same(p.PermissionId, o))
                    .First()
                    .PermissionType = permissionType);
            res
                .CloseDialog()
                .Html("#CurrentPermissions", PermissionListItem(
                    siteModel,
                    Types.Current,
                    currentPermissions,
                    selectedCurrentPermissions))
                .SetData("#CurrentPermissions");
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
    }
}
