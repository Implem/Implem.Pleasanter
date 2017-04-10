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
    public static class DeptUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var deptCollection = DeptCollection(ss, view);
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: Sessions.User().TenantManager,
                referenceType: "Depts",
                title: Displays.Depts() + " - " + Displays.List(),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("DeptForm")
                                .Class("main-form")
                                .Action(Locations.Action("Depts")),
                            action: () => hb
                                .Aggregations(
                                    ss: ss,
                                    aggregations: deptCollection.Aggregations)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        ss: ss,
                                        deptCollection: deptCollection,
                                        view: view))
                                .MainCommands(
                                    ss: ss,
                                    siteId: ss.SiteId,
                                    verType: Versions.VerTypes.Latest)
                                .Div(css: "margin-bottom")
                                .Hidden(controlId: "TableName", value: "Depts")
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
            DeptCollection deptCollection,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            ss.SetColumnAccessControls();
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: ss.HasPermission(),
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Depts",
                script: Libraries.Scripts.JavaScripts.ViewMode(viewMode),
                userScript: ss.GridScript,
                userStyle: ss.GridStyle,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DeptsForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(ss: ss, view: view)
                            .ViewFilters(ss: ss, view: view)
                            .Aggregations(
                                ss: ss,
                                aggregations: deptCollection.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest,
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Depts")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSettingsDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSettings())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var deptCollection = DeptCollection(ss, view);
            return new ResponseCollection()
                .Html("#ViewModeContainer", new HtmlBuilder().Grid(
                    ss: ss,
                    deptCollection: deptCollection,
                    view: view))
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: deptCollection.Aggregations))
                .ToJson();
        }

        private static DeptCollection DeptCollection(
            SiteSettings ss, View view, int offset = 0)
        {
            return new DeptCollection(
                ss: ss,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss, Rds.DeptsWhere().TenantId(Sessions.TenantId())),
                orderBy: view.OrderBy(ss, Rds.DeptsOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            DeptCollection deptCollection,
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
                            deptCollection: deptCollection,
                            view: view))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        deptCollection.Count(),
                        deptCollection.Aggregations.TotalCount)
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
            var deptCollection = DeptCollection(ss, view, offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: deptCollection.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    deptCollection: deptCollection,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    deptCollection.Count(),
                    deptCollection.Aggregations.TotalCount))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings ss,
            DeptCollection deptCollection,
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
                .TBody(action: () => deptCollection
                    .ForEach(deptModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(deptModel.DeptId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: deptModel.DeptId.ToString()));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            ss: ss,
                                            column: column,
                                            deptModel: deptModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.DeptsColumn();
            new List<string> { "DeptId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.DeptsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, DeptModel deptModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    deptModel: deptModel);
            }
            else
            {
                var mine = deptModel.Mine();
                switch (column.ColumnName)
                {
                    case "DeptId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.DeptId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.DeptCode)
                            : hb.Td(column: column, value: string.Empty);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Dept)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: deptModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        public static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, DeptModel deptModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.ColumnName)
                {
                    case "DeptId": value = deptModel.DeptId.GridText(column: column); break;
                    case "Ver": value = deptModel.Ver.GridText(column: column); break;
                    case "DeptCode": value = deptModel.DeptCode.GridText(column: column); break;
                    case "Dept": value = deptModel.Dept.GridText(column: column); break;
                    case "Body": value = deptModel.Body.GridText(column: column); break;
                    case "Comments": value = deptModel.Comments.GridText(column: column); break;
                    case "Creator": value = deptModel.Creator.GridText(column: column); break;
                    case "Updator": value = deptModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = deptModel.CreatedTime.GridText(column: column); break;
                    case "UpdatedTime": value = deptModel.UpdatedTime.GridText(column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(SiteSettings ss)
        {
            return Editor(ss, new DeptModel(
                SiteSettingsUtilities.DeptsSiteSettings(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, int deptId, bool clearSessions)
        {
            var deptModel = new DeptModel(
                SiteSettingsUtilities.DeptsSiteSettings(),
                deptId: deptId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            deptModel.SwitchTargets = GetSwitchTargets(
                SiteSettingsUtilities.DeptsSiteSettings(), deptModel.DeptId);
            return Editor(ss, deptModel);
        }

        public static string Editor(SiteSettings ss, DeptModel deptModel)
        {
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(deptModel.Mine());
            return hb.Template(
                ss: ss,
                verType: deptModel.VerType,
                methodType: deptModel.MethodType,
                allowAccess:
                    SiteSettingsUtilities.DeptsSiteSettings().CanRead() &&
                    deptModel.AccessStatus != Databases.AccessStatuses.NotFound,
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts() + " - " + Displays.New()
                    : deptModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            ss: ss,
                            deptModel: deptModel)
                        .Hidden(controlId: "TableName", value: "Depts")
                        .Hidden(controlId: "Id", value: deptModel.DeptId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb, SiteSettings ss, DeptModel deptModel)
        {
            var commentsColumnPermissionType = ss.GetColumn("Comments").ColumnPermissionType();
            var showComments = ss.EditorColumns?.Contains("Comments") == true &&
                commentsColumnPermissionType != Permissions.ColumnPermissionTypes.Deny;
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("DeptForm")
                        .Class("main-form confirm-reload")
                        .Action(deptModel.DeptId != 0
                            ? Locations.Action("Depts", deptModel.DeptId)
                            : Locations.Action("Depts")),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: deptModel,
                            tableName: "Depts")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: deptModel.Comments,
                                    verType: deptModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(deptModel: deptModel)
                            .FieldSetGeneral(ss: ss, deptModel: deptModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: deptModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                ss: ss,
                                siteId: 0,
                                verType: deptModel.VerType,
                                referenceType: "Depts",
                                referenceId: deptModel.DeptId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        deptModel: deptModel,
                                        ss: ss)))
                        .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                        .Hidden(
                            controlId: "MethodType",
                            value: deptModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Depts_Timestamp",
                            css: "always-send",
                            value: deptModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: deptModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Depts", deptModel.DeptId, deptModel.Ver)
                .CopyDialog("Depts", deptModel.DeptId)
                .OutgoingMailDialog()
                .EditorExtensions(deptModel: deptModel, ss: ss));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, DeptModel deptModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: deptModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            DeptModel deptModel)
        {
            var mine = deptModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                ss.GetEditorColumns().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "DeptId":
                            hb.Field(
                                ss,
                                column,
                                deptModel.MethodType,
                                deptModel.DeptId.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Ver":
                            hb.Field(
                                ss,
                                column,
                                deptModel.MethodType,
                                deptModel.Ver.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DeptCode":
                            hb.Field(
                                ss,
                                column,
                                deptModel.MethodType,
                                deptModel.DeptCode.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DeptName":
                            hb.Field(
                                ss,
                                column,
                                deptModel.MethodType,
                                deptModel.DeptName.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Body":
                            hb.Field(
                                ss,
                                column,
                                deptModel.MethodType,
                                deptModel.Body.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                    }
                });
                hb.VerUpCheckBox(deptModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, DeptModel deptModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, DeptModel deptModel, SiteSettings ss)
        {
            return hb;
        }

        public static string EditorJson(SiteSettings ss, int deptId)
        {
            return EditorResponse(ss, new DeptModel(ss, deptId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss, 
            DeptModel deptModel,
            Message message = null,
            string switchTargets = null)
        {
            deptModel.MethodType = BaseModel.MethodTypes.Edit;
            return new DeptsResponseCollection(deptModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, deptModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static List<int> GetSwitchTargets(SiteSettings ss, int deptId)
        {
            var view = Views.GetBySession(ss);
            var switchTargets = Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectDepts(
                    column: Rds.DeptsColumn().DeptId(),
                    where: view.Where(
                        ss: ss, where: Rds.DeptsWhere().TenantId(Sessions.TenantId())),
                    orderBy: view.OrderBy(ss, Rds.DeptsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc))))
                            .AsEnumerable()
                            .Select(o => o["DeptId"].ToInt())
                            .ToList();
            if (!switchTargets.Contains(deptId))
            {
                switchTargets.Add(deptId);
            }
            return switchTargets;
        }

        public static string Create(SiteSettings ss)
        {
            var deptModel = new DeptModel(ss, 0, setByForm: true);
            var invalid = DeptValidators.OnCreating(ss, deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = deptModel.Create(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(
                    ss,
                    deptModel,
                    Messages.Created(deptModel.Title.Value),
                    GetSwitchTargets(ss, deptModel.DeptId).Join()).ToJson();
            }
        }

        public static string Update(SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(ss, deptId, setByForm: true);
            var invalid = DeptValidators.OnUpdating(ss, deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = deptModel.Update(ss);
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(deptModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new DeptsResponseCollection(deptModel);
                return ResponseByUpdate(res, ss, deptModel)
                    .PrependComment(deptModel.Comments, deptModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            DeptsResponseCollection res,
            SiteSettings ss, 
            DeptModel deptModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", deptModel.Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: deptModel, tableName: "Depts"))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(deptModel.Title.ToString()))
                .RemoveComment(deptModel.DeleteCommentId, _using: deptModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Delete(SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(ss, deptId);
            var invalid = DeptValidators.OnDeleting(ss, deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = deptModel.Delete(ss);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(deptModel.Title.Value).Html);
                var res = new DeptsResponseCollection(deptModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Index("Depts"));
                return res.ToJson();
            }
        }

        public static string Restore(SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel();
            var invalid = DeptValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = deptModel.Restore(ss, deptId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new DeptsResponseCollection(deptModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(ss, deptId);
            ss.SetColumnAccessControls(deptModel.Mine());
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
                        new DeptCollection(
                            ss: ss,
                            where: Rds.DeptsWhere().DeptId(deptModel.DeptId),
                            orderBy: Rds.DeptsOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(deptModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(deptModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                deptModelHistory.Ver == deptModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    deptModel: deptModelHistory))))));
            return new DeptsResponseCollection(deptModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        public static string History(SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(ss, deptId);
            ss.SetColumnAccessControls(deptModel.Mine());
            deptModel.Get(
                ss, 
                where: Rds.DeptsWhere()
                    .DeptId(deptModel.DeptId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            deptModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, deptModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows()
        {
            return GridRows(
                SiteSettingsUtilities.DeptsSiteSettings(),
                offset: DataViewGrid.Offset());
        }
    }
}
