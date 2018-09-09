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
    public static class DeptUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(Context context, SiteSettings ss)
        {
            var invalid = DeptValidators.OnEntry(
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
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.Template(
                context: context,
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Depts",
                script: JavaScripts.ViewMode(viewMode),
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
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = DeptValidators.OnEntry(context: context, ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Depts",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DeptsForm")
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
                            .Hidden(controlId: "TableName", value: "Depts")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .MoveDialog(context: context, bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export())))
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

        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls(context: context);
            return new GridData(
                context: context,
                ss: ss,
                view: view,
                where: Rds.DeptsWhere().TenantId(context.TenantId),
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
                    .CopyDirectUrlToClipboard(ss: ss))
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
            var sqlColumnCollection = Rds.DeptsColumn();
            new List<string> { "DeptId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.DeptsColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.DeptsColumn();
            new List<string> { "DeptId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.DeptsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            DeptModel deptModel)
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
                var mine = deptModel.Mine(context: context);
                switch (column.Name)
                {
                    case "DeptId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptId)
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
                                    value: deptModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptCode)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.Dept)
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
                                    value: deptModel.Body)
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
                                    value: deptModel.Comments)
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
                                    value: deptModel.Creator)
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
                                    value: deptModel.Updator)
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
                                    value: deptModel.CreatedTime)
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
                                    value: deptModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, DeptModel deptModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
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

        public static string EditorNew(Context context, SiteSettings ss)
        {
            return Editor(context: context, ss: ss, deptModel: new DeptModel(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int deptId, bool clearSessions)
        {
            var deptModel = new DeptModel(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: deptId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            deptModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: deptModel.DeptId);
            return Editor(context: context, ss: ss, deptModel: deptModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, DeptModel deptModel)
        {
            var invalid = DeptValidators.OnEditing(
                context: context, ss: ss, deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                verType: deptModel.VerType,
                methodType: deptModel.MethodType,
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts() + " - " + Displays.New()
                    : deptModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            context: context,
                            ss: ss,
                            deptModel: deptModel)
                        .Hidden(controlId: "TableName", value: "Depts")
                        .Hidden(controlId: "Id", value: deptModel.DeptId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, DeptModel deptModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
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
                            context: context,
                            ss: ss,
                            baseModel: deptModel,
                            tableName: "Depts")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: deptModel.Comments,
                                    column: commentsColumn,
                                    verType: deptModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(deptModel: deptModel)
                            .FieldSetGeneral(context: context, ss: ss, deptModel: deptModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: deptModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: 0,
                                verType: deptModel.VerType,
                                referenceId: deptModel.DeptId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
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
                .OutgoingMailsForm(
                    context: context,
                    referenceType: "Depts",
                    referenceId: deptModel.DeptId,
                    referenceVer: deptModel.Ver)
                .CopyDialog("Depts", deptModel.DeptId)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    deptModel: deptModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, DeptModel deptModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General()))
                .Li(
                    _using: deptModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
        {
            var mine = deptModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, deptModel: deptModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "DeptId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: deptModel.MethodType,
                            value: deptModel.DeptId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: deptModel.MethodType,
                            value: deptModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DeptCode":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: deptModel.MethodType,
                            value: deptModel.DeptCode
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DeptName":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: deptModel.MethodType,
                            value: deptModel.DeptName
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: deptModel.MethodType,
                            value: deptModel.Body
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
                    baseModel: deptModel);
            }
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, Context context, SiteSettings ss, DeptModel deptModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, Context context, SiteSettings ss, DeptModel deptModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, int deptId)
        {
            return EditorResponse(context, ss, new DeptModel(
                context, ss, deptId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            Message message = null,
            string switchTargets = null)
        {
            deptModel.MethodType = BaseModel.MethodTypes.Edit;
            return new DeptsResponseCollection(deptModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, deptModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int deptId)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var where = view.Where(context: context, ss: ss, where: Rds.DeptsWhere().TenantId(context.TenantId));
            var join = ss.Join(context: context);
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectDepts(
                    column: Rds.DeptsColumn().DeptsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectDepts(
                                column: Rds.DeptsColumn().DeptId(),
                                join: join,
                                where: where,
                                orderBy: view.OrderBy(context: context, ss: ss)
                                    .Depts_UpdatedTime(SqlOrderBy.Types.desc)))
                                        .AsEnumerable()
                                        .Select(o => o["DeptId"].ToInt())
                                        .ToList()
                        : new List<int>();
            if (!switchTargets.Contains(deptId))
            {
                switchTargets.Add(deptId);
            }
            return switchTargets;
        }

        public static string Create(Context context, SiteSettings ss)
        {
            var deptModel = new DeptModel(context, ss, 0, setByForm: true);
            var invalid = DeptValidators.OnCreating(
                context: context, ss: ss, deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = deptModel.Create(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Created(deptModel.Title.Value));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? Forms.Long("LinkId")
                                : deptModel.DeptId))
                        .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Update(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(
                context: context, ss: ss, deptId: deptId, setByForm: true);
            var invalid = DeptValidators.OnUpdating(
                context: context, ss: ss, deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = deptModel.Update(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    var res = new DeptsResponseCollection(deptModel);
                    return ResponseByUpdate(res, context, ss, deptModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: deptModel.Comments,
                            verType: deptModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        deptModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            DeptsResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
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
                    where: Rds.DeptsWhere().DeptId(deptModel.DeptId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{deptModel.DeptId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(deptModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context)
                    .Timestamp(context: context)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", deptModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: deptModel,
                        tableName: "Depts"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(deptModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: deptModel.Comments,
                        deleteCommentId: deptModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string Delete(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(context, ss, deptId);
            var invalid = DeptValidators.OnDeleting(
                context: context, ss: ss, deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = deptModel.Delete(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Deleted(deptModel.Title.Value));
                    var res = new DeptsResponseCollection(deptModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Index("Depts"));
                    return res.ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int deptId, Message message = null)
        {
            var deptModel = new DeptModel(context: context, ss: ss, deptId: deptId);
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb
                .HistoryCommands(context: context, ss: ss)
                .Table(
                    attributes: new HtmlAttributes().Class("grid history"),
                    action: () => hb
                        .THead(action: () => hb
                            .GridHeader(
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                deptModel: deptModel)));
            return new DeptsResponseCollection(deptModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            DeptModel deptModel)
        {
            new DeptCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.DeptsWhere().DeptId(deptModel.DeptId),
                orderBy: Rds.DeptsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(deptModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(deptModelHistory.Ver)
                                .DataLatest(1, _using:
                                    deptModelHistory.Ver == deptModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: deptModelHistory.Ver.ToString(),
                                            _using: deptModelHistory.Ver < deptModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            deptModel: deptModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.DeptsColumnCollection()
                .DeptId()
                .Ver();
            columns.ForEach(column => sqlColumn.DeptsColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(context: context, ss: ss, deptId: deptId);
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
            deptModel.Get(
                context: context,
                ss: ss,
                where: Rds.DeptsWhere()
                    .DeptId(deptModel.DeptId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            deptModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, deptModel).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                offset: DataViewGrid.Offset());
        }
    }
}
