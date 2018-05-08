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
    public static class GroupUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings ss)
        {
            var invalid = GroupValidators.OnEntry(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Groups",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Groups() + " - " + Displays.List(),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("GroupForm")
                                .Class("main-form")
                                .Action(Locations.Action("Groups")),
                            action: () => hb
                                .ViewFilters(ss: ss, view: view)
                                .Aggregations(
                                    ss: ss,
                                    aggregations: gridData.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        ss: ss,
                                        gridData: gridData,
                                        view: view))
                                .MainCommands(
                                    ss: ss,
                                    siteId: ss.SiteId,
                                    verType: Versions.VerTypes.Latest)
                                .Div(css: "margin-bottom")
                                .Hidden(controlId: "TableName", value: "Groups")
                                .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                                .Hidden(
                                    controlId: "GridOffset",
                                    value: Parameters.General.GridPageSize.ToString()))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.Import()))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSettings()));
                }).ToString();
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = GroupValidators.OnEntry(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Groups",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(Routes.Action()),
                userStyle: ss.ViewModeStyles(Routes.Action()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("GroupsForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(ss: ss, view: view)
                            .ViewFilters(ss: ss, view: view)
                            .Aggregations(
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Groups")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            return new ResponseCollection()
                .ViewMode(
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(SiteSettings ss, View view, int offset = 0)
        {
            return new GridData(
                ss: ss,
                view: view,
                where: Rds.GroupsWhere()
                    .TenantId(Sessions.TenantId())
                    .GroupId_In(sub: Rds.SelectGroupMembers(
                        distinct: true,
                        column: Rds.GroupMembersColumn().GroupId(),
                        where: Permissions.GroupMembersWhere()),
                        _using: !Permissions.CanManageTenant()),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.Aggregations.TotalCount)
                            .ToString())
                .Button(
                    controlId: "ViewSorter",
                    controlCss: "hidden",
                    action: "GridRows",
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: "GridRows",
                    method: "post");
        }

        public static string GridRows(
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view, offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.Aggregations.TotalCount))
                .Paging("#Grid")
                .Message(message)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columns: columns, 
                            view: view,
                            checkAll: checkAll))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.GroupsColumn();
            new List<string> { "GroupId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.GroupsColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.GroupsColumn();
            new List<string> { "GroupId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.GroupsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, GroupModel groupModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    groupModel: groupModel);
            }
            else
            {
                var mine = groupModel.Mine();
                switch (column.Name)
                {
                    case "GroupId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.GroupId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "GroupName":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.GroupName)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: groupModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, GroupModel groupModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "GroupId": value = groupModel.GroupId.GridText(column: column); break;
                    case "Ver": value = groupModel.Ver.GridText(column: column); break;
                    case "GroupName": value = groupModel.GroupName.GridText(column: column); break;
                    case "Body": value = groupModel.Body.GridText(column: column); break;
                    case "Comments": value = groupModel.Comments.GridText(column: column); break;
                    case "Creator": value = groupModel.Creator.GridText(column: column); break;
                    case "Updator": value = groupModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = groupModel.CreatedTime.GridText(column: column); break;
                    case "UpdatedTime": value = groupModel.UpdatedTime.GridText(column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(SiteSettings ss)
        {
            return Editor(ss, new GroupModel(
                SiteSettingsUtilities.GroupsSiteSettings(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, int groupId, bool clearSessions)
        {
            var groupModel = new GroupModel(
                SiteSettingsUtilities.GroupsSiteSettings(),
                groupId: groupId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            groupModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtilities.GroupsSiteSettings(), groupModel.GroupId);
            return Editor(ss, groupModel);
        }

        public static string Editor(SiteSettings ss, GroupModel groupModel)
        {
            var invalid = GroupValidators.OnEditing(ss, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(groupModel.Mine());
            return hb.Template(
                ss: ss,
                verType: groupModel.VerType,
                methodType: groupModel.MethodType,
                referenceType: "Groups",
                title: groupModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Groups() + " - " + Displays.New()
                    : groupModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            ss: ss,
                            groupModel: groupModel)
                        .Hidden(controlId: "TableName", value: "Groups")
                        .Hidden(controlId: "Id", value: groupModel.GroupId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, SiteSettings ss, GroupModel groupModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("GroupForm")
                        .Class("main-form confirm-reload")
                        .Action(groupModel.GroupId != 0
                            ? Locations.Action("Groups", groupModel.GroupId)
                            : Locations.Action("Groups")),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: groupModel,
                            tableName: "Groups")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: groupModel.Comments,
                                    column: commentsColumn,
                                    verType: groupModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(groupModel: groupModel)
                            .FieldSetGeneral(
                                ss: ss,
                                groupModel: groupModel)
                            .FieldSetMembers(groupModel: groupModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: groupModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                ss: ss,
                                siteId: 0,
                                verType: groupModel.VerType,
                                referenceId: groupModel.GroupId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        groupModel: groupModel,
                                        ss: ss)))
                        .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: groupModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Groups_Timestamp",
                            css: "always-send",
                            value: groupModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: groupModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Groups", groupModel.GroupId, groupModel.Ver)
                .CopyDialog("Groups", groupModel.GroupId)
                .OutgoingMailDialog()
                .EditorExtensions(groupModel: groupModel, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, GroupModel groupModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General()))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMembers",
                        text: Displays.Members(),
                        _using: groupModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: groupModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            GroupModel groupModel)
        {
            var mine = groupModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    ss: ss, groupModel: groupModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            SiteSettings ss,
            GroupModel groupModel,
            bool preview = false)
        {
            ss.GetEditorColumns().ForEach(column =>
            {
                switch (column.Name)
                {
                    case "GroupId":
                        hb.Field(
                            ss,
                            column,
                            groupModel.MethodType,
                            groupModel.GroupId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            ss,
                            column,
                            groupModel.MethodType,
                            groupModel.Ver.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "GroupName":
                        hb.Field(
                            ss,
                            column,
                            groupModel.MethodType,
                            groupModel.GroupName.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            ss,
                            column,
                            groupModel.MethodType,
                            groupModel.Body.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                }
            });
            if (!preview) hb.VerUpCheckBox(groupModel);
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, GroupModel groupModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, GroupModel groupModel, SiteSettings ss)
        {
            return hb;
        }

        public static string EditorJson(SiteSettings ss, int groupId)
        {
            return EditorResponse(ss, new GroupModel(ss, groupId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss,
            GroupModel groupModel,
            Message message = null,
            string switchTargets = null)
        {
            groupModel.MethodType = BaseModel.MethodTypes.Edit;
            return new GroupsResponseCollection(groupModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, groupModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(SiteSettings ss, int groupId)
        {
            var view = Views.GetBySession(ss);
            var where = view.Where(ss: ss, where: Rds.GroupsWhere().TenantId(Sessions.TenantId()));
            var join = ss.Join();
            var switchTargets = Rds.ExecuteScalar_int(statements:
                Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(statements: Rds.SelectGroups(
                            column: Rds.GroupsColumn().GroupId(),
                            join: join,
                            where: where,
                            orderBy: view.OrderBy(ss).Groups_UpdatedTime(SqlOrderBy.Types.desc)))
                                .AsEnumerable()
                                .Select(o => o["GroupId"].ToInt())
                                .ToList()
                        : new List<int>();
            if (!switchTargets.Contains(groupId))
            {
                switchTargets.Add(groupId);
            }
            return switchTargets;
        }

        public static string Create(SiteSettings ss)
        {
            var groupModel = new GroupModel(ss, 0, setByForm: true);
            var invalid = GroupValidators.OnCreating(ss, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Create(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            Sessions.Set("Message", Messages.Created(groupModel.Title.Value));
            return new ResponseCollection()
                .SetMemory("formChanged", false)
                .Href(Locations.Edit(
                    controller: Routes.Controller(),
                    id: ss.Columns.Any(o => o.Linking)
                        ? Forms.Long("LinkId")
                        : groupModel.GroupId))
                .ToJson();
        }

        public static string Update(SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId, setByForm: true);
            var invalid = GroupValidators.OnUpdating(ss, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = groupModel.Update(ss);
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(groupModel.Updator.Name).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new GroupsResponseCollection(groupModel);
                return ResponseByUpdate(res, ss, groupModel)
                    .PrependComment(
                        ss,
                        ss.GetColumn("Comments"),
                        groupModel.Comments,
                        groupModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            GroupsResponseCollection res,
            SiteSettings ss,
            GroupModel groupModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", groupModel.Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: groupModel, tableName: "Groups"))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(groupModel.Title.Value))
                .Comment(
                    ss,
                    ss.GetColumn("Comments"),
                    groupModel.Comments,
                    groupModel.DeleteCommentId)
                .ClearFormData();
        }

        public static string Delete(SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            var invalid = GroupValidators.OnDeleting(ss, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Delete(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(groupModel.Title.Value));
                var res = new GroupsResponseCollection(groupModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Index("Groups"));
                return res.ToJson();
            }
        }

        public static string Restore(SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel();
            var invalid = GroupValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Restore(ss, groupId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new GroupsResponseCollection(groupModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            ss.SetColumnAccessControls(groupModel.Mine());
            var columns = ss.GetHistoryColumns(checkPermission: true);
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columns: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new GroupCollection(
                            ss: ss,
                            column: HistoryColumn(columns),
                            where: Rds.GroupsWhere().GroupId(groupModel.GroupId),
                            orderBy: Rds.GroupsOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(groupModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(groupModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                groupModelHistory.Ver == groupModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    groupModel: groupModelHistory))))));
            return new GroupsResponseCollection(groupModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.GroupsColumnCollection()
                .GroupId()
                .Ver();
            columns.ForEach(column => sqlColumn.GroupsColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            ss.SetColumnAccessControls(groupModel.Mine());
            groupModel.Get(
                ss, 
                where: Rds.GroupsWhere()
                    .GroupId(groupModel.GroupId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            groupModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, groupModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows()
        {
            return GridRows(
                SiteSettingsUtilities.GroupsSiteSettings(),
                offset: DataViewGrid.Offset());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetMembers(this HtmlBuilder hb, GroupModel groupModel)
        {
            if (groupModel.MethodType == BaseModel.MethodTypes.New) return hb;
            return hb.FieldSet(id: "FieldSetMembers", action: () => hb
                .CurrentMembers(groupModel)
                .SelectableMembers());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder CurrentMembers(this HtmlBuilder hb, GroupModel groupModel)
        {
            return hb.FieldSelectable(
                controlId: "CurrentMembers",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                controlCss: " always-send send-all",
                labelText: Displays.CurrentMembers(),
                listItemCollection: CurrentMembers(groupModel),
                selectedValueCollection: null,
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "GeneralUser",
                            controlCss: "button-icon post",
                            text: Displays.GeneralUser(),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlId: "Manager",
                            controlCss: "button-icon post",
                            text: Displays.Manager(),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Delete(),
                            onClick: "$p.deleteSelected($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> CurrentMembers(GroupModel groupModel)
        {
            var data = new Dictionary<string, ControlData>();
            Rds.ExecuteTable(statements:
                Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn()
                        .DeptId()
                        .UserId()
                        .Admin(),
                    where: Rds.GroupMembersWhere()
                        .Or(Rds.GroupMembersWhere()
                            .Sub(sub: Rds.ExistsDepts(where: Rds.DeptsWhere()
                                .DeptId(raw: "[GroupMembers].[DeptId]")))
                            .Sub(sub: Rds.ExistsUsers(where: Rds.UsersWhere()
                                .UserId(raw: "[GroupMembers].[UserId]"))))
                        .GroupId(groupModel.GroupId)))
                            .AsEnumerable()
                            .ForEach(dataRow =>
                                data.AddMember(dataRow));
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SelectableMembersJson()
        {
            return new ResponseCollection().Html(
                "#SelectableMembers",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: SelectableMembers()))
                        .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SelectableMembers(this HtmlBuilder hb)
        {
            return hb.FieldSelectable(
                controlId: "SelectableMembers",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                labelText: Displays.SelectableMembers(),
                listItemCollection: SelectableMembers(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Add(),
                            onClick: "$p.addSelected($(this), $('#CurrentMembers'));",
                            icon: "ui-icon-circle-triangle-w",
                            action: "SelectableMembers",
                            method: "post")
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchMemberText",
                            controlCss: " always-send auto-postback w100",
                            placeholder: Displays.Search(),
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SelectableMembers()
        {
            var data = new Dictionary<string, ControlData>();
            var searchText = Forms.Data("SearchMemberText");
            if (!searchText.IsNullOrEmpty())
            {
                var currentMembers = Forms.List("CurrentMembersAll");
                Rds.ExecuteTable(statements: new SqlStatement[]
                {
                    Rds.SelectDepts(
                        column: Rds.DeptsColumn()
                            .DeptId()
                            .DeptCode()
                            .Add("0 as [UserId]")
                            .Add("'' as [UserCode]")
                            .Add("0 as [IsUser]"),
                        where: Rds.DeptsWhere()
                            .TenantId(Sessions.TenantId())
                            .DeptId_In(
                                currentMembers?
                                    .Where(o => o.StartsWith("Dept,"))
                                    .Select(o => o.Split_2nd().ToInt()),
                                negative: true)
                            .SqlWhereLike(
                                name: "SearchText",
                                searchText: searchText,
                                clauseCollection: new List<string>()
                                {
                                    Rds.Depts_DeptCode_WhereLike(),
                                    Rds.Depts_DeptName_WhereLike()
                                })),
                    Rds.SelectUsers(
                        unionType: Sqls.UnionTypes.Union,
                        column: Rds.UsersColumn()
                            .Add("0 as [DeptId]")
                            .Add("'' as [DeptCode]")
                            .UserId()
                            .UserCode()
                            .Add("1 as [IsUser]"),
                        join: Rds.UsersJoin()
                            .Add(new SqlJoin(
                                tableBracket: "[Depts]",
                                joinType: SqlJoin.JoinTypes.LeftOuter,
                                joinExpression: "[Users].[DeptId]=[Depts].[DeptId]")),
                        where: Rds.UsersWhere()
                            .TenantId(Sessions.TenantId())
                            .UserId_In(
                                currentMembers?
                                    .Where(o => o.StartsWith("User,"))
                                    .Select(o => o.Split_2nd().ToInt()),
                                negative: true)
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
                            .Users_Disabled(0))
                })
                    .AsEnumerable()
                    .OrderBy(o => o["IsUser"])
                    .ThenBy(o => o["DeptCode"])
                    .ThenBy(o => o["DeptId"])
                    .ThenBy(o => o["UserCode"])
                    .ForEach(dataRow =>
                        data.AddMember(dataRow));
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AddMember(this Dictionary<string, ControlData> data, DataRow dataRow)
        {
            var deptId = dataRow["DeptId"].ToInt();
            var userId = dataRow["UserId"].ToInt();
            var admin = dataRow.Table.Columns.Contains("Admin")
                ? dataRow["Admin"].ToBool()
                : false;
            var manager = admin
                ? "(" + Displays.Manager() + ")"
                : string.Empty;
            if (deptId > 0)
            {
                data.Add(
                    "Dept," + deptId.ToString() + "," + admin,
                    new ControlData(SiteInfo.Dept(deptId)?.Name + manager));
            }
            else if (userId > 0)
            {
                data.Add(
                    "User," + userId.ToString() + "," + admin,
                    new ControlData(SiteInfo.UserName(userId) + manager));
            }
        }
    }
}
