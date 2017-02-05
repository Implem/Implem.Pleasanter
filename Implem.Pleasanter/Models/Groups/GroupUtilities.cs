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
    public static class GroupUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings ss, Permissions.Types pt)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var groupCollection = GroupCollection(
                ss,
                Permissions.Admins(),
                view);
            return hb.Template(
                pt: pt,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: Sessions.User().TenantAdmin,
                referenceType: "Groups",
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
                                .Aggregations(
                                    ss: ss,
                                    aggregations: groupCollection.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        groupCollection: groupCollection,
                                        pt: pt,
                                        ss: ss,
                                        view: view))
                                .MainCommands(
                                    siteId: ss.SiteId,
                                    pt: pt,
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
            Permissions.Types pt,
            GroupCollection groupCollection,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            return hb.Template(
                pt: pt,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: pt.CanRead(),
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Groups",
                script: Libraries.Scripts.JavaScripts.ViewMode(
                    ss: ss, pt: pt, viewMode: viewMode),
                userScript: ss.GridScript,
                userStyle: ss.GridStyle,
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
                                aggregations: groupCollection.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                siteId: ss.SiteId,
                                pt: pt,
                                verType: Versions.VerTypes.Latest,
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Groups")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSettingsDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSettings())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss, Permissions.Types pt)
        {
            var view = Views.GetBySession(ss);
            var groupCollection = GroupCollection(ss, pt, view);
            return new ResponseCollection()
                .Html("#ViewModeContainer", new HtmlBuilder().Grid(
                    ss: ss,
                    groupCollection: groupCollection,
                    pt: pt,
                    view: view))
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: groupCollection.Aggregations))
                .ToJson();
        }

        private static GroupCollection GroupCollection(
            SiteSettings ss,
            Permissions.Types pt,
            View view,
            int offset = 0)
        {
            return new GroupCollection(
                ss: ss,
                pt: pt,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss, Rds.GroupsWhere().TenantId(Sessions.TenantId())),
                orderBy: view.OrderBy(ss, Rds.GroupsOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            Permissions.Types pt,
            GroupCollection groupCollection,
            View view)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            ss: ss,
                            groupCollection: groupCollection,
                            view: view))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        groupCollection.Count(),
                        groupCollection.Aggregations.TotalCount)
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
            Permissions.Types pt,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var view = Views.GetBySession(ss);
            var groupCollection = GroupCollection(ss, pt, view, offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    groupCollection: groupCollection,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    groupCollection.Count(),
                    groupCollection.Aggregations.TotalCount))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings ss,
            GroupCollection groupCollection,
            View view,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns();
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columnCollection: columns, 
                            view: view,
                            checkAll: checkAll))
                .TBody(action: () => groupCollection
                    .ForEach(groupModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(groupModel.GroupId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: groupModel.GroupId.ToString()));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            ss: ss,
                                            column: column,
                                            groupModel: groupModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.GroupsColumn();
            new List<string> { "GroupId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
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
                switch (column.ColumnName)
                {
                    case "GroupId": return hb.Td(column: column, value: groupModel.GroupId);
                    case "Ver": return hb.Td(column: column, value: groupModel.Ver);
                    case "GroupName": return hb.Td(column: column, value: groupModel.GroupName);
                    case "Body": return hb.Td(column: column, value: groupModel.Body);
                    case "Comments": return hb.Td(column: column, value: groupModel.Comments);
                    case "Creator": return hb.Td(column: column, value: groupModel.Creator);
                    case "Updator": return hb.Td(column: column, value: groupModel.Updator);
                    case "CreatedTime": return hb.Td(column: column, value: groupModel.CreatedTime);
                    case "UpdatedTime": return hb.Td(column: column, value: groupModel.UpdatedTime);
                    default: return hb;
                }
            }
        }

        public static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, GroupModel groupModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.ColumnName)
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

        public static string EditorNew()
        {
            return Editor(new GroupModel(
                SiteSettingsUtilities.GroupsSiteSettings(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(int groupId, bool clearSessions)
        {
            var groupModel = new GroupModel(
                SiteSettingsUtilities.GroupsSiteSettings(),
                groupId: groupId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            groupModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtilities.GroupsSiteSettings(), groupModel.GroupId);
            return Editor(groupModel);
        }

        public static string Editor(GroupModel groupModel)
        {
            var hb = new HtmlBuilder();
            var pt = Permissions.Admins();
            return hb.Template(
                pt: pt,
                verType: groupModel.VerType,
                methodType: groupModel.MethodType,
                allowAccess:
                    pt.CanEditTenant() &&
                    groupModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "Groups",
                title: groupModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Groups() + " - " + Displays.New()
                    : groupModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            ss: groupModel.SiteSettings,
                            pt: pt,
                            groupModel: groupModel)
                        .Hidden(controlId: "TableName", value: "Groups")
                        .Hidden(controlId: "Id", value: groupModel.GroupId.ToString());
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            Permissions.Types pt,
            GroupModel groupModel)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("GroupForm")
                        .Class("main-form")
                        .Action(groupModel.GroupId != 0
                            ? Locations.Action("Groups", groupModel.GroupId)
                            : Locations.Action("Groups")),
                    action: () => hb
                        .RecordHeader(
                            pt: pt,
                            baseModel: groupModel,
                            tableName: "Groups")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: groupModel.Comments,
                                verType: groupModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(groupModel: groupModel)
                            .FieldSetGeneral(
                                ss: ss,
                                pt: pt,
                                groupModel: groupModel)
                            .FieldSetMembers(groupModel: groupModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: groupModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                pt: pt,
                                verType: groupModel.VerType,
                                referenceType: "Groups",
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
                            css: "must-transport",
                            value: groupModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
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
                        text: Displays.Basic()))
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
            Permissions.Types pt,
            GroupModel groupModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                ss.GetEditorColumns().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "GroupId": hb.Field(ss, column, groupModel.MethodType, groupModel.GroupId.ToControl(column, pt), column.ColumnPermissionType(pt)); break;
                        case "Ver": hb.Field(ss, column, groupModel.MethodType, groupModel.Ver.ToControl(column, pt), column.ColumnPermissionType(pt)); break;
                        case "GroupName": hb.Field(ss, column, groupModel.MethodType, groupModel.GroupName.ToControl(column, pt), column.ColumnPermissionType(pt)); break;
                        case "Body": hb.Field(ss, column, groupModel.MethodType, groupModel.Body.ToControl(column, pt), column.ColumnPermissionType(pt)); break;
                    }
                });
                hb.VerUpCheckBox(groupModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, GroupModel groupModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            GroupModel groupModel,
            SiteSettings ss)
        {
            return hb;
        }

        public static string EditorJson(
            SiteSettings ss, Permissions.Types pt, int groupId)
        {
            return EditorResponse(new GroupModel(ss, groupId))
                .ToJson();
        }

        private static ResponseCollection EditorResponse(
            GroupModel groupModel, Message message = null, string switchTargets = null)
        {
            groupModel.MethodType = BaseModel.MethodTypes.Edit;
            return new GroupsResponseCollection(groupModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(groupModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static List<int> GetSwitchTargets(SiteSettings ss, int groupId)
        {
            var view = Views.GetBySession(ss);
            var switchTargets = Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectGroups(
                    column: Rds.GroupsColumn().GroupId(),
                    where: view.Where(
                        ss: ss, where: Rds.GroupsWhere().TenantId(Sessions.TenantId())),
                    orderBy: view.OrderBy(ss, Rds.GroupsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc))))
                            .AsEnumerable()
                            .Select(o => o["GroupId"].ToInt())
                            .ToList();
            if (!switchTargets.Contains(groupId))
            {
                switchTargets.Add(groupId);
            }
            return switchTargets;
        }

        public static string Create(SiteSettings ss, Permissions.Types pt)
        {
            var groupModel = new GroupModel(ss, 0, setByForm: true);
            var invalid = GroupValidators.OnCreating(ss, pt, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Create();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(
                    groupModel,
                    Messages.Created(groupModel.Title.Value),
                    GetSwitchTargets(ss, groupModel.GroupId).Join()).ToJson();
            }
        }

        public static string Update(SiteSettings ss, Permissions.Types pt, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId, setByForm: true);
            var invalid = GroupValidators.OnUpdating(ss, pt, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return new ResponseCollection().Message(invalid.Message()).ToJson();
            }
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = groupModel.Update();
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(groupModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new GroupsResponseCollection(groupModel);
                return ResponseByUpdate(pt, res, groupModel)
                    .PrependComment(groupModel.Comments, groupModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            Permissions.Types pt, GroupsResponseCollection res, GroupModel groupModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", groupModel.Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: groupModel, tableName: "Groups"))
                .Message(Messages.Updated(groupModel.Title.ToString()))
                .RemoveComment(groupModel.DeleteCommentId, _using: groupModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Delete(
            SiteSettings ss, Permissions.Types pt, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            var invalid = GroupValidators.OnDeleting(ss, pt, groupModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Delete();
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(groupModel.Title.Value).Html);
                var res = new GroupsResponseCollection(groupModel);
                res.Href(Locations.Index("Groups"));
                return res.ToJson();
            }
        }

        public static string Restore(int groupId)
        {
            var groupModel = new GroupModel();
            var invalid = GroupValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = groupModel.Restore(groupId);
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

        public static string Histories(
            SiteSettings ss, Permissions.Types pt, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            var columns = ss.GetHistoryColumns();
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new GroupCollection(
                            ss: ss,
                            pt: pt,
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

        public static string History(
            SiteSettings ss, Permissions.Types pt, int groupId)
        {
            var groupModel = new GroupModel(ss, groupId);
            groupModel.Get(
                where: Rds.GroupsWhere()
                    .GroupId(groupModel.GroupId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            groupModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(groupModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows()
        {
            return GridRows(
                SiteSettingsUtilities.GroupsSiteSettings(),
                Permissions.Admins(),
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
                controlCss: " must-transport send-all",
                labelText: Displays.CurrentMembers(),
                listItemCollection: CurrentMembers(groupModel),
                selectedValueCollection: null,
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
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
                        .UserId(),
                    where: Rds.GroupMembersWhere()
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
                            controlCss: " must-transport auto-postback w100",
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
                var currentMembers = Forms.Data("CurrentMembersAll").Deserialize<List<string>>();
                Rds.ExecuteTable(statements: new SqlStatement[]
                {
                    Rds.SelectDepts(
                        column: Rds.DeptsColumn()
                            .DeptId()
                            .Add("0 as [UserId]"),
                        where: Rds.DeptsWhere()
                            .TenantId(Sessions.TenantId())
                            .DeptId_In(
                                currentMembers?
                                    .Where(o => o.StartsWith("Dept,"))
                                    .Select(o => o.Split_2nd().ToInt()),
                                negative: true)
                            .SqlWhereLike(
                                searchText,
                                Rds.Depts_DeptId_WhereLike(),
                                Rds.Depts_DeptCode_WhereLike(),
                                Rds.Depts_DeptName_WhereLike()),
                        unionType: Sqls.UnionTypes.Union),
                    Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .Add("0 as [DeptId]")
                            .UserId(),
                        where: Rds.UsersWhere()
                            .TenantId(Sessions.TenantId())
                            .UserId_In(
                                currentMembers?
                                    .Where(o => o.StartsWith("User,"))
                                    .Select(o => o.Split_2nd().ToInt()),
                                negative: true)
                            .SqlWhereLike(
                                searchText,
                                Rds.Users_LoginId_WhereLike(),
                                Rds.Users_UserId_WhereLike(),
                                Rds.Users_FirstName_WhereLike(),
                                Rds.Users_LastName_WhereLike()))
                })
                    .AsEnumerable()
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
            if (deptId > 0)
            {
                data.Add(
                    "Dept," + deptId.ToString(),
                    new ControlData(SiteInfo.Dept(deptId)?.Name));
            }
            else if (userId > 0)
            {
                data.Add(
                    "User," + userId.ToString(),
                    new ControlData(SiteInfo.UserFullName(userId)));
            }
        }
    }
}
