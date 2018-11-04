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
        public static string Index(Context context, SiteSettings ss)
        {
            var invalid = GroupValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Groups",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Groups(context: context) + " - " + Displays.List(context: context),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("GroupForm")
                                .Class("main-form")
                                .Action(Locations.Action("Groups")),
                            action: () => hb
                                .ViewFilters(context: context, ss: ss, view: view)
                                .Aggregations(
                                    context: context,
                                    ss: ss,
                                    aggregations: gridData.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        context: context,
                                        ss: ss,
                                        gridData: gridData,
                                        view: view))
                                .MainCommands(
                                    context: context,
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
                            .Title(Displays.Import(context: context)))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSettings(context: context)));
                }).ToString();
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = GroupValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Groups",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("GroupsForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(context: context, ss: ss, view: view)
                            .ViewFilters(context: context, ss: ss, view: view)
                            .Aggregations(
                                context: context,
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Groups")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(context: context, bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context))))
                    .ToString();
        }

        public static string IndexJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            return new GridData(
                context: context,
                ss: ss,
                view: view,
                where: Rds.GroupsWhere()
                    .TenantId(context.TenantId)
                    .GroupId_In(sub: Rds.SelectGroupMembers(
                        distinct: true,
                        column: Rds.GroupMembersColumn().GroupId(),
                        where: Rds.GroupMembersWhere()
                            .Add(raw: Permissions.DeptOrUser("GroupMembers"))),
                        _using: !Permissions.CanManageTenant(context: context)),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string action = "GridRows")
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class(ss.GridCss())
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: action))
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
                    action: action,
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: action,
                    method: "post");
        }

        public static string GridRows(
            Context context,
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows",
            Message message = null)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view,
                offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        view: view))
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    context: context,
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck,
                    action: action))
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
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            columns: columns, 
                            view: view,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    context: context,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.GroupsColumn();
            new List<string> { "GroupId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.GroupsColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.GroupsColumn();
            new List<string> { "GroupId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.GroupsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            GroupModel groupModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    groupModel: groupModel);
            }
            else
            {
                var mine = groupModel.Mine(context: context);
                switch (column.Name)
                {
                    case "GroupId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.GroupId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "GroupName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.GroupName)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.Body)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.Comments)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.Creator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.Updator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            GroupModel groupModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "GroupId": value = groupModel.GroupId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = groupModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "GroupName": value = groupModel.GroupName.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = groupModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = groupModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = groupModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = groupModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = groupModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = groupModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            return Editor(context: context, ss: ss, groupModel: new GroupModel(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int groupId, bool clearSessions)
        {
            var groupModel = new GroupModel(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: groupId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            groupModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: groupModel.GroupId);
            return Editor(context: context, ss: ss, groupModel: groupModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, GroupModel groupModel)
        {
            var invalid = GroupValidators.OnEditing(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: groupModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: groupModel.VerType,
                methodType: groupModel.MethodType,
                referenceType: "Groups",
                title: groupModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Groups(context: context) + " - " + Displays.New(context: context)
                    : groupModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            context: context,
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
            this HtmlBuilder hb, Context context, SiteSettings ss, GroupModel groupModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
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
                            context: context,
                            ss: ss,
                            baseModel: groupModel,
                            tableName: "Groups")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: groupModel.Comments,
                                    column: commentsColumn,
                                    verType: groupModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(
                                context: context,
                                groupModel: groupModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                groupModel: groupModel)
                            .FieldSetMembers(
                                context: context,
                                groupModel: groupModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: groupModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: 0,
                                verType: groupModel.VerType,
                                referenceId: groupModel.GroupId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
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
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Groups",
                    referenceId: groupModel.GroupId,
                    referenceVer: groupModel.Ver)
                .CopyDialog(
                    context: context,
                    referenceType: "Groups",
                    id: groupModel.GroupId)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    groupModel: groupModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMembers",
                        text: Displays.Members(context: context),
                        _using: groupModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: groupModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            var mine = groupModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, groupModel: groupModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "GroupId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: groupModel.MethodType,
                            value: groupModel.GroupId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: groupModel.MethodType,
                            value: groupModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "GroupName":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: groupModel.MethodType,
                            value: groupModel.GroupName
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: groupModel.MethodType,
                            value: groupModel.Body
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: groupModel);
            }
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, int groupId)
        {
            return EditorResponse(context, ss, new GroupModel(
                context, ss, groupId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            Message message = null,
            string switchTargets = null)
        {
            groupModel.MethodType = BaseModel.MethodTypes.Edit;
            return new GroupsResponseCollection(groupModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, groupModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int groupId)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var where = view.Where(context: context, ss: ss, where: Rds.GroupsWhere().TenantId(context.TenantId));
            var join = ss.Join(context: context);
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectGroups(
                                column: Rds.GroupsColumn().GroupId(),
                                join: join,
                                where: where,
                                orderBy: view.OrderBy(context: context, ss: ss)
                                    .Groups_UpdatedTime(SqlOrderBy.Types.desc)))
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

        public static string Create(Context context, SiteSettings ss)
        {
            var groupModel = new GroupModel(context, ss, 0, setByForm: true);
            var invalid = GroupValidators.OnCreating(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = groupModel.Create(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: groupModel.Title.Value));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? Forms.Long("LinkId")
                                : groupModel.GroupId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Update(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(
                context: context, ss: ss, groupId: groupId, setByForm: true);
            var invalid = GroupValidators.OnUpdating(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var error = groupModel.Update(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    var res = new GroupsResponseCollection(groupModel);
                    return ResponseByUpdate(res, context, ss, groupModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: groupModel.Comments,
                            verType: groupModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: groupModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            GroupsResponseCollection res,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            if (Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    where: Rds.GroupsWhere().GroupId(groupModel.GroupId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{groupModel.GroupId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: groupModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", groupModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: groupModel,
                        tableName: "Groups"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: groupModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: groupModel.Comments,
                        deleteCommentId: groupModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string Delete(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(context, ss, groupId);
            var invalid = GroupValidators.OnDeleting(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = groupModel.Delete(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: groupModel.Title.Value));
                    var res = new GroupsResponseCollection(groupModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.Index("Groups"));
                    return res.ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int groupId, Message message = null)
        {
            var groupModel = new GroupModel(context: context, ss: ss, groupId: groupId);
            ss.SetColumnAccessControls(
                context: context,
                mine: groupModel.Mine(context: context));
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands(context: context, ss: ss)
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                context: context,
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                groupModel: groupModel)));
            return new GroupsResponseCollection(groupModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            GroupModel groupModel)
        {
            new GroupCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.GroupsWhere().GroupId(groupModel.GroupId),
                orderBy: Rds.GroupsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(groupModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(groupModelHistory.Ver)
                                .DataLatest(1, _using:
                                    groupModelHistory.Ver == groupModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: groupModelHistory.Ver.ToString(),
                                            _using: groupModelHistory.Ver < groupModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            groupModel: groupModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.GroupsColumnCollection()
                .GroupId()
                .Ver();
            columns.ForEach(column => sqlColumn.GroupsColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(context: context, ss: ss, groupId: groupId);
            ss.SetColumnAccessControls(
                context: context,
                mine: groupModel.Mine(context: context));
            groupModel.Get(
                context: context,
                ss: ss,
                where: Rds.GroupsWhere()
                    .GroupId(groupModel.GroupId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            groupModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, groupModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                offset: DataViewGrid.Offset());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetMembers(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            if (groupModel.MethodType == BaseModel.MethodTypes.New) return hb;
            return hb.FieldSet(id: "FieldSetMembers", action: () => hb
                .CurrentMembers(
                    context: context,
                    groupModel: groupModel)
                .SelectableMembers(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder CurrentMembers(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            return hb.FieldSelectable(
                controlId: "CurrentMembers",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                controlCss: " always-send send-all",
                labelText: Displays.CurrentMembers(context: context),
                listItemCollection: CurrentMembers(
                    context: context,
                    groupModel: groupModel),
                selectedValueCollection: null,
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "GeneralUser",
                            controlCss: "button-icon post",
                            text: Displays.GeneralUser(context: context),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlId: "Manager",
                            controlCss: "button-icon post",
                            text: Displays.Manager(context: context),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Delete(context: context),
                            onClick: "$p.deleteSelected($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> CurrentMembers(
            Context context, GroupModel groupModel)
        {
            var data = new Dictionary<string, ControlData>();
            Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupMembers(
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
                                data.AddMember(
                                    context: context,
                                    dataRow: dataRow));
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SelectableMembersJson(Context context)
        {
            return new ResponseCollection().Html(
                "#SelectableMembers",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: SelectableMembers(
                        context: context)))
                            .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SelectableMembers(
            this HtmlBuilder hb, Context context)
        {
            return hb.FieldSelectable(
                controlId: "SelectableMembers",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                labelText: Displays.SelectableMembers(context: context),
                listItemCollection: SelectableMembers(context: context),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Add(context: context),
                            onClick: "$p.addSelected($(this), $('#CurrentMembers'));",
                            icon: "ui-icon-circle-triangle-w",
                            action: "SelectableMembers",
                            method: "post")
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchMemberText",
                            controlCss: " always-send auto-postback w100",
                            placeholder: Displays.Search(context: context),
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SelectableMembers(Context context)
        {
            var data = new Dictionary<string, ControlData>();
            var searchText = Forms.Data("SearchMemberText");
            if (!searchText.IsNullOrEmpty())
            {
                var currentMembers = Forms.List("CurrentMembersAll");
                Rds.ExecuteTable(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectDepts(
                            column: Rds.DeptsColumn()
                                .DeptId()
                                .DeptCode()
                                .Add("0 as [UserId]")
                                .Add("'' as [UserCode]")
                                .Add("0 as [IsUser]"),
                            where: Rds.DeptsWhere()
                                .TenantId(context.TenantId)
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
                                .TenantId(context.TenantId)
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
                        .OrderBy(dataRow => dataRow.Bool("IsUser"))
                        .ThenBy(dataRow => dataRow.String("DeptCode"))
                        .ThenBy(dataRow => dataRow.Int("DeptId"))
                        .ThenBy(dataRow => dataRow.String("UserCode"))
                        .ForEach(dataRow =>
                            data.AddMember(
                                context: context,
                                dataRow: dataRow));
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AddMember(
            this Dictionary<string, ControlData> data, Context context, DataRow dataRow)
        {
            var deptId = dataRow.Int("DeptId");
            var userId = dataRow.Int("UserId");
            var admin = dataRow.Table.Columns.Contains("Admin")
                ? dataRow.Bool("Admin")
                : false;
            var manager = admin
                ? $"({Displays.Manager(context: context)})"
                : string.Empty;
            if (deptId > 0)
            {
                data.Add(
                    $"Dept,{deptId.ToString()}," + admin,
                    new ControlData(SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: deptId)?.Name + manager));
            }
            else if (userId > 0)
            {
                data.Add(
                    $"User,{userId.ToString()}," + admin,
                    new ControlData(SiteInfo.UserName(
                        context: context,
                        userId: userId) + manager));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss)
        {
            var api = Forms.String().Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api?.View ?? new View();
            var siteId = view.ColumnFilterHash
                            .Where(f => f.Key == "SiteId")
                            .Select(f => f.Value)
                            .FirstOrDefault()?.ToLong();
            var userId = view.ColumnFilterHash
                            .Where(f => f.Key == "UserId")
                            .Select(f => f.Value)
                            .FirstOrDefault()?.ToLong();
            var siteModel = siteId.HasValue ? new SiteModel(context, siteId.Value) : null;
            if (siteModel != null)
            {
                if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
                }
                var invalid = SiteValidators.OnReading(
                    context,
                    siteModel.SitesSiteSettings(context, siteId.Value),
                    siteModel);
                switch (invalid)
                {
                    case Error.Types.None: break;
                    default: return ApiResults.Error(
                        context: context,
                        type: invalid);
                }
            }
            var siteGroups = siteModel != null
                ? SiteInfo.SiteGroups(context, siteModel.InheritPermission)?
                .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var groupCollection = new GroupCollection(
                context: context,
                ss: ss,
                where: view.Where(context: context, ss: ss)
                .Groups_TenantId(context.TenantId)
                .SqlWhereLike(
                    name: "SearchText",
                    searchText: view.ColumnFilterHash
                    .Where(f => f.Key == "SearchText")
                    .Select(f => f.Value)
                    .FirstOrDefault(),
                    clauseCollection: new List<string>()
                    {
                        Rds.Groups_GroupId_WhereLike(),
                        Rds.Groups_GroupName_WhereLike(),
                        Rds.Groups_Body_WhereLike()
                    })
                .Add(
                    tableName: "Groups",
                    subLeft: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().GroupMembersCount(),
                    where: Rds.GroupMembersWhere().UserId(userId).GroupId(raw: "[Groups].[GroupId]").Add(raw: "[Groups].[GroupId]>0")),
                    _operator: ">0",
                    _using: userId.HasValue),
                orderBy: view.OrderBy(context: context, ss: ss, pageSize: pageSize),
                offset: api.Offset,
                pageSize: pageSize,
                countRecord: true);
            var groups = siteGroups == null
                ? groupCollection
                : groupCollection.Join(siteGroups, c => c.GroupId, s => s, (c, s) => c);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Offset = api.Offset,
                    PageSize = pageSize,
                    TotalCount = groups.Count(),
                    Data = groups.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }
    }
}
