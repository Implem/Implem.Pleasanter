using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
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
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Depts",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Depts(context: context) + " - " + Displays.List(context: context),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("DeptForm")
                                .Class("main-form")
                                .Action(Locations.Action(
                                    context: context,
                                    controller: "Depts")),
                            action: () => hb
                                .ViewFilters(context: context, ss: ss, view: view)
                                .Aggregations(
                                    context: context,
                                    ss: ss,
                                    view: view)
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
                                .Hidden(
                                    controlId: "TableName",
                                    value: "Depts")
                                .Hidden(
                                    controlId: "BaseUrl",
                                    value: Locations.BaseUrl(context: context))
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
            View view,
            string viewMode,
            Action viewModeBody,
            Aggregations aggregations = null)
        {
            var invalid = DeptValidators.OnEntry(
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
                referenceType: "Depts",
                body: ss.Body,
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DeptsForm")
                            .Class("main-form")
                            .Action(Locations.Action(
                                context: context,
                                controller: context.Controller,
                                id: ss.SiteId)),
                        action: () => hb
                            .ViewSelector(
                                context: context,
                                ss: ss,
                                view: view)
                            .ViewFilters(
                                context: context,
                                ss: ss,
                                view: view)
                            .Aggregations(
                                context: context,
                                ss: ss,
                                view: view)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest,
                                backButton: !context.Publish)
                            .Div(css: "margin-bottom")
                            .Hidden(
                                controlId: "TableName",
                                value: "Depts")
                            .Hidden(
                                controlId: "BaseUrl",
                                value: Locations.BaseUrl(context: context)))
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
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .Events("on_grid_load")
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
                pageSize: ss.GridPageSize.ToInt());
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
                        gridData.TotalCount)
                            .ToString())
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
                .ClearFormData("GridOffset")
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        view: view))
                .ReplaceAll(
                    "#Aggregations",
                    new HtmlBuilder().Aggregations(
                        context: context,
                        ss: ss,
                        view: view),
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
                    gridData.TotalCount))
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
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
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
                            action: action,
                            checkRow: !ss.GridColumnsHasSources()))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    context: context,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll,
                    checkRow: !ss.GridColumnsHasSources()));
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

        private static SqlWhereCollection SelectedWhere(
            Context context, SiteSettings ss)
        {
            var selector = new GridSelector(context: context);
            return !selector.Nothing
                ? Rds.DeptsWhere().DeptId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
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
                    context: context,
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
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            DeptModel deptModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "DeptId": value = deptModel.DeptId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = deptModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "DeptCode": value = deptModel.DeptCode.GridText(
                        context: context,
                        column: column); break;
                    case "Dept": value = deptModel.Dept.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = deptModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = deptModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = deptModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = deptModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = deptModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = deptModel.UpdatedTime.GridText(
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
                context: context,
                ss: ss,
                deptModel: deptModel);
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
                view: null,
                verType: deptModel.VerType,
                methodType: deptModel.MethodType,
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts(context: context) + " - " + Displays.New(context: context)
                    : deptModel.Title.Value,
                action: () =>
                {
                    hb
                        .Editor(
                            context: context,
                            ss: ss,
                            deptModel: deptModel)
                        .Hidden(controlId: "TableName", value: "Depts")
                        .Hidden(controlId: "Controller", value: context.Controller)
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
                            ? Locations.Action(
                                context: context,
                                controller: "Depts",
                                id: deptModel.DeptId)
                            : Locations.Action(
                                context: context,
                                controller: "Depts")),
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
                            .EditorTabs(
                                context: context,
                                deptModel: deptModel)
                            .FieldSetGeneral(context: context, ss: ss, deptModel: deptModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: deptModel.MethodType != BaseModel.MethodTypes.New
                                    && !context.Publish)
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
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
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
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Depts",
                    referenceId: deptModel.DeptId,
                    referenceVer: deptModel.Ver)
                .CopyDialog(
                    context: context,
                    referenceType: "Depts",
                    id: deptModel.DeptId)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    deptModel: deptModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, DeptModel deptModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(
                    _using: deptModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context))));
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
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
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
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.DeptsWhere().TenantId(context.TenantId));
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Depts_UpdatedTime(SqlOrderBy.Types.desc);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
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
                                orderBy: orderBy))
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

        public static ResponseCollection FieldResponse(
            this DeptsResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
        {
            var mine = deptModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "DeptId":
                            res.Val(
                                "#Depts_DeptId",
                                deptModel.DeptId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptCode":
                            res.Val(
                                "#Depts_DeptCode",
                                deptModel.DeptCode.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptName":
                            res.Val(
                                "#Depts_DeptName",
                                deptModel.DeptName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Depts_Body",
                                deptModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static string Create(Context context, SiteSettings ss)
        {
            var deptModel = new DeptModel(context, ss, 0, setByForm: true);
            var invalid = DeptValidators.OnCreating(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = deptModel.Create(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: deptModel.Title.Value));
                    return new ResponseCollection()
                        .Response("id", deptModel.DeptId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : deptModel.DeptId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Update(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(
                context: context, ss: ss, deptId: deptId, setByForm: true);
            var invalid = DeptValidators.OnUpdating(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
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
                        context: context,
                        data: deptModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            DeptsResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
        {
            if (context.Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss,
                    setSession: false);
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
                    .Message(Messages.Updated(
                        context: context,
                        data: deptModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, deptModel: deptModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", deptModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: deptModel,
                        tableName: "Depts"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: deptModel.Title.Value))
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
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = deptModel.Delete(context: context, ss: ss);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: deptModel.Title.Value));
                    var res = new DeptsResponseCollection(deptModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.Index(
                            context: context,
                            controller: "Depts"));
                    return res.ToJson();
                default:
                    return error.MessageJson(context: context);
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
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            deptModel.VerType = context.Forms.Bool("Latest")
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
                offset: context.Forms.Int("GridOffset"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss)
        {
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api?.View ?? new View();
            var siteId = view.ColumnFilterHash
                ?.Where(f => f.Key == "SiteId")
                ?.Select(f => f.Value)
                ?.FirstOrDefault()?.ToLong();
            var userId = view.ColumnFilterHash
                ?.Where(f => f.Key == "UserId")
                ?.Select(f => f.Value)
                ?.FirstOrDefault()?.ToLong();
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
            var siteDepts = siteModel != null
                ? SiteInfo.SiteDepts(context, siteModel.InheritPermission)?
                    .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var deptCollection = new DeptCollection(
                context: context,
                ss: ss,
                where: view.Where(context: context, ss: ss)
                .Depts_TenantId(context.TenantId)
                .SqlWhereLike(
                    tableName: "Depts",
                    name: "SearchText",
                    searchText: view.ColumnFilterHash
                    ?.Where(f => f.Key == "SearchText")
                    ?.Select(f => f.Value)
                    ?.FirstOrDefault(),
                    clauseCollection: new List<string>()
                    {
                        Rds.Depts_DeptId_WhereLike(),
                        Rds.Depts_DeptName_WhereLike(),
                        Rds.Depts_Body_WhereLike()
                    })
                .Add(
                    tableName: "Users",
                    subLeft: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere()
                        .UserId(userId)
                        .DeptId(raw: "[Depts].[DeptId]")
                        .Add(raw: "[Depts].[DeptId]>0")),
                    _operator: ">0",
                    _using: userId.HasValue),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api.Offset,
                pageSize: pageSize,
                countRecord: true);
            var groups = siteDepts == null
                ? deptCollection
                : deptCollection.Join(siteDepts, c => c.DeptId, s => s, (c, s) => c);
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
