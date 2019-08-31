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
    public static class IssueUtilities
    {
        public static string Index(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Grid(
                   context: context,
                   gridData: gridData,
                   ss: ss,
                   view: view));
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = IssueValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Issues",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("MainForm")
                            .Class("main-form")
                            .Action(Locations.Action(
                                context: context,
                                controller: context.Controller,
                                id: ss.SiteId)),
                        action: () => hb
                            .Div(
                                id: "ViewSelectorField", 
                                action: ()=> hb
                                    .ViewSelector(
                                        context: context,
                                        ss: ss,
                                        view: view)
                                    .FieldCheckBox(
                                        fieldId: "ShowHistoryField",
                                        fieldCss: "field-auto-thin",
                                        controlId: "ViewFilters_ShowHistory",
                                        controlCss: " auto-postback",
                                        method: "post",
                                        _checked: view.ShowHistory == true,
                                        labelText: Displays.ShowHistory(context:context),
                                        _using: ss.HistoryOnGrid == true))
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
                                verType: Versions.VerTypes.Latest,
                                backButton: !context.Publish)
                            .Div(css: "margin-bottom")
                            .Hidden(
                                controlId: "TableName",
                                value: "Issues")
                            .Hidden(
                                controlId: "BaseUrl",
                                value: Locations.BaseUrl(context: context))
                            .Hidden(
                                controlId: "EditOnGrid",
                                css: "always-send",
                                value: context.Forms.Data("EditOnGrid"))
                            .Hidden(
                                controlId: "NewRowId",
                                css: "always-send",
                                value: context.Forms.Data("NewRowId")))
                    .EditorDialog(context: context, ss: ss)
                    .DropDownSearchDialog(
                        context: context,
                        id: ss.SiteId)
                    .MoveDialog(context: context, bulk: true)
                    .ImportSettingsDialog(context: context)
                    .Div(attributes: new HtmlAttributes()
                        .Id("SetNumericRangeDialog")
                        .Class("dialog")
                        .Title(Displays.NumericRange(context)))
                    .Div(attributes: new HtmlAttributes()
                        .Id("SetDateRangeDialog")
                        .Class("dialog")
                        .Title(Displays.DateRange(context)))
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context)))
                        .Div(attributes: new HtmlAttributes()
                                .Id("BulkUpdateSelectorDialog")
                                .Class("dialog")
                                .Title(Displays.BulkUpdate(context: context))))
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
                    editOnGrid: context.Forms.Bool("EditOnGrid"),
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
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class(ss.GridCss(context: context))
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            columns: columns,
                            view: view,
                        editRow: context.Forms.Bool("EditOnGrid"),
                            action: action))
                .GridHeaderMenus(
                    context: context,
                    ss: ss,
                    view: view,
                    columns: columns)
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.TotalCount)
                            .ToString())
                .Hidden(
                    controlId: "GridRowIds",
                    value: gridData.DataRows.Select(g => g.Long("IssueId")).ToJson())
                .Hidden(
                    controlId: "GridColumns",
                    value: columns.Select(o => o.ColumnName).ToJson())
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
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            var newOnGrid = context.Action == "newongrid"
                || context.Action == "copyrow";
            if (newOnGrid && !context.CanCreate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var editRow = context.Forms.Bool("EditOnGrid");
            var newRowId = newOnGrid
                ? (context.Forms.Int("NewRowId") - 1)
                : 0;
            IssueModel issueModel = null;
            var originalId = context.Forms.Long("OriginalId");
            if (editRow && offset == 0)
            {
                if (newRowId != 0)
                {
                    issueModel = originalId > 0
                        ? new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: originalId)
                        : new IssueModel(
                            context: context,
                            ss: ss,
                            methodType: BaseModel.MethodTypes.New);
                    issueModel.IssueId = 0;
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: issueModel.Mine(context: context));
                }
            }
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridOffset")
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .ClearFormData("OriginalId", _using: newOnGrid)
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
                .ReplaceAll(
                    "#ViewFilters",
                    new HtmlBuilder()
                        .ViewFilters(
                            context: context,
                            ss: ss,
                            view: view),
                    _using: context.Forms.ControlId().StartsWith("ViewFiltersOnGridHeader__"))
                .Append("#Grid", new HtmlBuilder().GridRows(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    columns: columns,
                    view: view,
                    issueModel: issueModel,
                    editRow: editRow,
                    newRowId: newRowId,
                    offset: offset,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#NewRowId", newRowId, _using: newOnGrid)
                .CopyRowFormData(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    columns: columns,
                    newOnGrid: newOnGrid,
                    newRowId: newRowId,
                    originalId: originalId)
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.TotalCount))
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("IssueId")).ToJson())
                .Val("#GridColumns", columns.Select(o => o.ColumnName).ToJson())
                .Paging("#Grid")
                .Message(message)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            List<Column> columns,
            View view,
            IssueModel issueModel = null,
            bool editRow = false,
            long newRowId = 0,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkRow = !ss.GridColumnsHasSources();
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss);
            return hb
                .THead(
                    _using: offset == 0,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            ss: ss,
                            columns: columns, 
                            view: view,
                            editRow: editRow,
                            checkRow: checkRow,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => hb
                    .GridNewRows(
                        context: context,
                        ss: ss,
                        issueModel: issueModel,
                        columns: columns,
                        formDataSet: formDataSet,
                        editRow: editRow,
                        newRowId: newRowId,
                        offset: offset)
                    .GridRows(
                        context: context,
                        ss: ss,
                        dataRows: gridData.DataRows,
                        columns: columns,
                        formDataSet: formDataSet,
                        gridSelector: null,
                        editRow: editRow,
                        checkRow: checkRow));
        }

        private static HtmlBuilder GridNewRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            List<Column> columns,
            FormDataSet formDataSet,
            bool editRow,
            long newRowId,
            int offset)
        {
            if (editRow && offset == 0)
            {
                if (newRowId != 0)
                {
                    hb.NewOnGrid(
                        context: context,
                        ss: ss,
                        columns: columns,
                        issueModel: issueModel,
                        newRowId: newRowId);
                }
                formDataSet
                    .Where(formData => formData.Id < 0)
                    .OrderBy(formData => formData.Id)
                    .ForEach(formData =>
                    {
                        issueModel = new IssueModel(
                            context: context,
                            ss: ss,
                            formData: formData.Data);
                        ss.SetColumnAccessControls(
                            context: context,
                            mine: issueModel.Mine(context: context));
                        hb.NewOnGrid(
                            context: context,
                            ss: ss,
                            columns: columns,
                            issueModel: issueModel,
                            newRowId: formData.Id);
                    });
            }
            return hb;
        }

        private static ResponseCollection CopyRowFormData(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            List<Column> columns,
            bool newOnGrid,
            int newRowId,
            long originalId)
        {
            if (newOnGrid && originalId > 0)
            {
                issueModel.SetCopyDefault(
                    context: context,
                    ss: ss);
                ss.EditorColumns
                    .Select(columnName => ss.GetColumn(
                        context: context,
                        columnName: columnName))
                    .Where(column => column.CanUpdate)
                    .Where(column => !column.Id_Ver)
                    .Where(column => !columns.Any(p =>
                        p.ColumnName == column.ColumnName))
                    .ForEach(column =>
                        res.SetFormData(
                            $"{ss.ReferenceType}_{column.ColumnName}_{ss.SiteId}_{newRowId}",
                            issueModel.ControlValue(
                                context: context,
                                ss: ss,
                                column: column)));
            }
            return res;
        }

        private static SqlWhereCollection SelectedWhere(
            Context context, SiteSettings ss)
        {
            var selector = new GridSelector(context: context);
            return !selector.Nothing
                ? Rds.IssuesWhere().IssueId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long issueId)
        {
            ss.SetColumnAccessControls(context: context);
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.IssuesWhere().IssueId(issueId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: issueId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{issueId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + issueId)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("IssueId")}\"][data-latest]",
                        new HtmlBuilder().Tr(
                            context: context,
                            ss: ss,
                            dataRow: dataRow,
                            columns: ss.GetGridColumns(
                                context: context,
                                view: view,
                                checkPermission: true),
                            gridSelector: null,
                            editRow: true,
                            checkRow: false,
                            idColumn: "IssueId"))
                    .ToJson();
        }

        public static string TrashBox(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TrashBoxCommands(context: context, ss: ss)
                    .Grid(
                        context: context,
                        ss: ss,
                        gridData: gridData,
                        view: view,
                        action: "TrashBoxGridRows"));
        }

        public static string TrashBoxJson(Context context, SiteSettings ss)
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
                        .TrashBoxCommands(context: context, ss: ss)
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: "TrashBoxGridRows"))
                .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            IssueModel issueModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    issueModel: issueModel);
            }
            else
            {
                var mine = issueModel.Mine(context: context);
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.SiteId)
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
                                    value: issueModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "IssueId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.IssueId)
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
                                    value: issueModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.Title)
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
                                    value: issueModel.Body)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.TitleBody)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "StartTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.StartTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CompletionTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.CompletionTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "WorkValue":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.WorkValue)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ProgressRate":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.ProgressRate)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "RemainingWorkValue":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.RemainingWorkValue)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Status":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.Status)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Manager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.Manager)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Owner":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.Owner)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "SiteTitle":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.SiteTitle)
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
                                    value: issueModel.Comments)
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
                                    value: issueModel.Creator)
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
                                    value: issueModel.Updator)
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
                                    value: issueModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Class(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Num":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Num(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Date":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Date(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Description":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Description(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Check":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Check(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            case "Attachments":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    type: ss.PermissionType,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: issueModel.Attachments(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
                            default:
                                return hb;
                        }
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            IssueModel issueModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = issueModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = issueModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "IssueId": value = issueModel.IssueId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = issueModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = issueModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = issueModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = issueModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "StartTime": value = issueModel.StartTime.GridText(
                        context: context,
                        column: column); break;
                    case "CompletionTime": value = issueModel.CompletionTime.GridText(
                        context: context,
                        column: column); break;
                    case "WorkValue": value = issueModel.WorkValue.GridText(
                        context: context,
                        column: column); break;
                    case "ProgressRate": value = issueModel.ProgressRate.GridText(
                        context: context,
                        column: column); break;
                    case "RemainingWorkValue": value = issueModel.RemainingWorkValue.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = issueModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "Manager": value = issueModel.Manager.GridText(
                        context: context,
                        column: column); break;
                    case "Owner": value = issueModel.Owner.GridText(
                        context: context,
                        column: column); break;
                    case "SiteTitle": value = issueModel.SiteTitle.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = issueModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = issueModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = issueModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = issueModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = issueModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = issueModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = issueModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = issueModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = issueModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = issueModel.Attachments(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                        }
                        break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ItemsLimit));
            }
            return Editor(
                context: context,
                ss: ss,
                issueModel: new IssueModel(
                    context: context,
                    ss: ss,
                    methodType: BaseModel.MethodTypes.New,
                    formData: context.Forms));
        }

        public static string Editor(
            Context context, SiteSettings ss, long issueId, bool clearSessions)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            issueModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: ss,
                issueId: issueModel.IssueId,
                siteId: issueModel.SiteId);
            return Editor(
                context: context,
                ss: ss,
                issueModel: issueModel);
        }

        public static string Editor(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool editInDialog = false)
        {
            var invalid = IssueValidators.OnEditing(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    ss: ss,
                    siteId: issueModel.SiteId,
                    referenceId: issueModel.IssueId,
                    isHistory: issueModel.VerType == Versions.VerTypes.History,
                    action: () => hb
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            issueModel: issueModel,
                            editInDialog: editInDialog))
                                .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: issueModel.VerType,
                    methodType: issueModel.MethodType,
                    siteId: issueModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Issues",
                    title: issueModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : issueModel.Title.DisplayValue,
                    body: issueModel.Body,
                    useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: issueModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: issueModel.MethodType),
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            issueModel: issueModel)
                        .Hidden(controlId: "TriggerRelatingColumns", value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: issueModel.IssueId != 0 
                                ? issueModel.IssueId
                                : issueModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: issueModel,
                            tableName: "Issues")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: issueModel.Comments,
                                    column: commentsColumn,
                                    verType: issueModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(
                                context: context,
                                ss: ss,
                                issueModel: issueModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                issueModel: issueModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: issueModel.MethodType != BaseModel.MethodTypes.New
                                    && !context.Publish)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: context.CanManagePermission(ss: ss)
                                    && !ss.Locked()
                                    && issueModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                verType: issueModel.VerType,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
                                        ss: ss,
                                        issueModel: issueModel)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "FromSiteId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("FromSiteId"),
                            _using: context.QueryStrings.Long("FromSiteId") > 0)
                        .Hidden(
                            controlId: "LinkId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("LinkId"),
                            _using: context.QueryStrings.Long("LinkId") > 0)
                        .Hidden(
                            controlId: "MethodType",
                            value: issueModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Issues_Timestamp",
                            css: "always-send",
                            value: issueModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: issueModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Issues",
                    referenceId: issueModel.IssueId,
                    referenceVer: issueModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    referenceType: "items",
                    id: issueModel.IssueId)
                .MoveDialog(context: context)
                .OutgoingMailDialog()
                .PermissionsDialog(context: context)
                .EditorExtensions(
                    context: context,
                    issueModel: issueModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(
                    _using: issueModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(_using: context.CanManagePermission(ss: ss) &&
                        issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl(context: context))));
        }

        public static string PreviewTemplate(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var name = Strings.NewGuid();
            return hb
                .Div(css: "samples-displayed", action: () => hb
                    .Text(text: Displays.SamplesDisplayed(context: context)))
                .Div(css: "template-tab-container", action: () => hb
                    .Ul(action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Editor",
                                text: Displays.Editor(context: context)))
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Grid",
                                text: Displays.Grid(context: context))))
                    .FieldSet(
                        id: name + "Editor",
                        action: () => hb
                            .FieldSetGeneralColumns(
                                context: context,
                                ss: ss,
                                issueModel: new IssueModel(),
                                preview: true))
                    .FieldSet(
                        id: name + "Grid",
                        action: () => hb
                            .Table(css: "grid", action: () => hb
                                .THead(action: () => hb
                                    .GridHeader(
                                        context: context,
                                        ss: ss,
                                        columns: ss.GetGridColumns(context: context),
                                        view: new View(context: context, ss: ss),
                                        sort: false,
                                        checkRow: false)))))
                                            .ToString();
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool editInDialog = false)
        {
            var mine = issueModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool preview = false,
            bool editInDialog = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: issueModel);
                if (!editInDialog)
                {
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkId: issueModel.IssueId,
                                methodType: issueModel.MethodType))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: issueModel.IssueId));
                }
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = issueModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    methodType: issueModel.MethodType,
                    value: value,
                    columnPermissionType: column.ColumnPermissionType(context: context),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this IssueModel issueModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "IssueId":
                    return issueModel.IssueId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return issueModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Title":
                    return issueModel.Title
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return issueModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "StartTime":
                    return issueModel.StartTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "CompletionTime":
                    return issueModel.CompletionTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "WorkValue":
                    return issueModel.WorkValue
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ProgressRate":
                    return issueModel.ProgressRate
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "RemainingWorkValue":
                    return issueModel.RemainingWorkValue
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Status":
                    return issueModel.Status
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Manager":
                    return issueModel.Manager
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Owner":
                    return issueModel.Owner
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return issueModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return issueModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return issueModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return issueModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return issueModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return issueModel.Attachments(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        default: return null;
                    }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.Button(
                        text: Displays.Separate(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openSeparateSettingsDialog($(this));",
                        icon: "ui-icon-extlink",
                        action: "EditSeparateSettings",
                        method: "post",
                        _using: context.CanUpdate(ss: ss)
                            && ss.AllowSeparate == true)
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.SeparateSettingsDialog(context: context)
                    : hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long issueId)
        {
            return EditorResponse(context, ss, new IssueModel(
                context, ss, issueId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            Message message = null,
            string switchTargets = null)
        {
            issueModel.MethodType = BaseModel.MethodTypes.Edit;
            var editInDialog = context.Forms.Bool("EditInDialog");
            return editInDialog
                ? new IssuesResponseCollection(issueModel)
                    .Response("id", issueModel.IssueId.ToString())
                    .Html("#EditInDialogBody", Editor(
                        context: context,
                        ss: ss,
                        issueModel: issueModel,
                        editInDialog: editInDialog))
                    .Invoke("openEditorDialog")
                    .Events("on_editor_load")
                : new IssuesResponseCollection(issueModel)
                    .Response("id", issueModel.IssueId.ToString())
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, issueModel))
                    .Val("#Id", issueModel.IssueId.ToString())
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .ClearFormData()
                    .Events("on_editor_load");
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long issueId, long siteId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Issues_UpdatedTime(SqlOrderBy.Types.desc);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                join: join,
                                where: where,
                                orderBy: orderBy))
                                    .AsEnumerable()
                                    .Select(o => o["IssueId"].ToLong())
                                    .ToList()
                        : new List<long>();
            if (!switchTargets.Contains(issueId))
            {
                switchTargets.Add(issueId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            string idSuffix = null)
        {
            var mine = issueModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "IssueId":
                            res.Val(
                                "#Issues_IssueId" + idSuffix,
                                issueModel.IssueId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Issues_Title" + idSuffix,
                                issueModel.Title.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Issues_Body" + idSuffix,
                                issueModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "StartTime":
                            res.Val(
                                "#Issues_StartTime" + idSuffix,
                                issueModel.StartTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "CompletionTime":
                            res.Val(
                                "#Issues_CompletionTime" + idSuffix,
                                issueModel.CompletionTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "WorkValue":
                            res.Val(
                                "#Issues_WorkValue" + idSuffix,
                                issueModel.WorkValue.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ProgressRate":
                            res.Val(
                                "#Issues_ProgressRate" + idSuffix,
                                issueModel.ProgressRate.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Status":
                            res.Val(
                                "#Issues_Status" + idSuffix,
                                issueModel.Status.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Issues_Manager" + idSuffix,
                                issueModel.Manager.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Issues_Owner" + idSuffix,
                                issueModel.Owner.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Issues_{column.Name}{idSuffix}",
                                        issueModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Issues_{column.Name}{idSuffix}",
                                        issueModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Issues_{column.Name}{idSuffix}",
                                        issueModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Issues_{column.Name}{idSuffix}",
                                        issueModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Issues_{column.Name}{idSuffix}",
                                        issueModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Issues_{column.Name}Field",
                                        new HtmlBuilder()
                                            .FieldAttachments(
                                                context: context,
                                                fieldId: $"Issues_{column.Name}Field",
                                                controlId: $"Issues_{column.Name}",
                                                columnName: column.ColumnName,
                                                fieldCss: column.FieldCss,
                                                fieldDescription: column.Description,
                                                controlCss: column.ControlCss,
                                                labelText: column.LabelText,
                                                value: issueModel.Attachments(columnName: column.Name).ToJson(),
                                                placeholder: column.LabelText,
                                                readOnly: column.ColumnPermissionType(context: context)
                                                    != Permissions.ColumnPermissionTypes.Update));
                                    break;
                            }
                            break;
                    }
                });
            return res;
        }

        public static HtmlBuilder NewOnGrid(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            IssueModel issueModel,
            long newRowId)
        {
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row new")
                    .DataId(newRowId.ToString())
                    .DataLatest(1),
                action: () =>
                {
                    hb.Td(action: () => hb
                        .Button(
                            title: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: $"$p.cancelNewRow($(this));",
                            icon: "ui-icon-close",
                            action: "CancelNewRow",
                            method: "post"));
                    columns.ForEach(column =>
                    {
                        if (!column.Joined
                            && column.CanCreate
                            && !column.Id_Ver
                            && column.EditorColumn
                            && column.TypeCs != "Attachments"
                            && column.GridDesign.IsNullOrEmpty())
                        {
                            hb.Td(action: () => hb
                                .Field(
                                    context: context,
                                    ss: ss,
                                    issueModel: issueModel,
                                    column: column,
                                    controlOnly: true,
                                    alwaysSend: true,
                                    idSuffix: $"_{ss.SiteId}_{newRowId}"));
                        }
                        else if (!column.Joined
                            && column.CanRead
                            && !column.Id_Ver)
                        {
                            hb.TdValue(
                                context: context,
                                ss: column.SiteSettings,
                                column: column,
                                issueModel: issueModel);
                        }
                        else
                        {
                            hb.Td();
                        }
                    });
                });
        }

        public static string CancelNewRow(Context context, SiteSettings ss, long id)
        {
            var res = new ResponseCollection()
                .Remove($"[data-id=\"{id}\"][data-latest]")
                .ClearFormData("CancelRowId");
            new FormDataSet(
                context: context,
                ss: ss)
                    .Where(o => !o.Suffix.IsNullOrEmpty())
                    .Where(o => o.SiteId == ss.SiteId)
                    .Where(o => o.Id == id)
                    .ForEach(formData =>
                        formData.Data.Keys.ForEach(controlId =>
                            res.ClearFormData(controlId + formData.Suffix)));
            return res.ToJson();
        }

        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss, bool internalRequest)
        {
            var invalid = IssueValidators.OnEntry(
                context: context,
                ss: ss,
                api: !internalRequest);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            var issueCollection = new IssueCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    "[Items]",
                    SqlJoin.JoinTypes.Inner,
                    "[Issues].[IssueId]=[Items].[ReferenceId]")),
                where: view.Where(context: context, ss: ss),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api.Offset,
                pageSize: pageSize,
                tableType: tableType);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    api.Offset,
                    PageSize = pageSize,
                    issueCollection.TotalCount,
                    Data = issueCollection.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(
            Context context, SiteSettings ss, long issueId, bool internalRequest)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnEditing(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: !internalRequest);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = issueModel.GetByApi(
                        context: context,
                        ss: ss).ToSingleList()
                }
            }.ToJson());
        }

        public static string Create(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                formData: context.Forms);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = issueModel.Create(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: issueModel.Title.DisplayValue));
                    return new ResponseCollection()
                        .Response("id", issueModel.IssueId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : issueModel.IssueId))
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText)
                                .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult CreateByApi(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ItemsLimit));
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                setByApi: true);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(context: context, ss: ss);
            var errorData = issueModel.Create(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Created(
                            context: context,
                            data: issueModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText);
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static string Update(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                formData: context.Forms);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var errorData = issueModel.Update(
                context: context,
                ss: ss,
                notice: true,
                permissions: context.Forms.List("CurrentPermissionsAll"),
                permissionChanged: context.Forms.Exists("CurrentPermissionsAll"));
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new IssuesResponseCollection(issueModel);
                    res.Val(
                        "#Issues_RemainingWorkValue",
                        ss.GetColumn(context: context, columnName: "RemainingWorkValue")
                            .Display(
                                context: context,
                                ss: ss,
                                value: issueModel.RemainingWorkValue));
                    return ResponseByUpdate(res, context, ss, issueModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: issueModel.Comments,
                            verType: issueModel.VerType)
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText)
                                .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: issueModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            IssuesResponseCollection res,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
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
                    tableType: Sqls.TableTypes.Normal,
                    where: Rds.IssuesWhere().IssueId(issueModel.IssueId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{issueModel.IssueId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: issueModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, issueModel: issueModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", issueModel.Title.DisplayValue)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: issueModel,
                        tableName: "Issues"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: issueModel.IssueId))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: issueModel.Title.DisplayValue))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: issueModel.Comments,
                        deleteCommentId: issueModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string OpenBulkUpdateSelectorDialog(Context context, SiteSettings ss)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                formData: context.Forms);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var columns = ss.GetAllowBulkUpdateColumns(context, ss);
            var column = columns.FirstOrDefault();
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html(
                    "#BulkUpdateSelectorDialog",
                    hb.BulkUpdateSelectorDialog(
                        context: context,
                        ss: ss,
                        columns: columns,
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                issueModel: issueModel,
                                column: column,
                                alwaysSend: true,
                                disableSection: true)))
                .ToJson();
        }

        public static string BulkUpdateSelectChanged(Context context, SiteSettings ss)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                formData: context.Forms);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("BulkUpdateColumnName"));
            return new ResponseCollection()
                .Html(
                    "#BulkUpdateSelectedField",
                    new HtmlBuilder().Field(
                        context: context,
                        ss: ss,
                        issueModel: issueModel,
                        column: column,
                        alwaysSend: true,
                        disableSection: true))
                .ToJson();
        }

        public static string BulkUpdate(Context context, SiteSettings ss)
        {
            var param = new Rds.IssuesParamCollection();
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("BulkUpdateColumnName"));
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                formData: context.Forms);
            issueModel.PropertyValue(
                context: context,
                name: column.ColumnName);
            if (context.CanUpdate(ss: ss))
            {
                var where = SelectedWhere(
                    context: context,
                    ss: ss);
                var count = BulkUpdate(
                    context: context,
                    ss: ss,
                    where: Views.GetBySession(
                        context: context,
                        ss: ss)
                            .Where(
                                context: context,
                                ss: ss,
                                where: where,
                                itemJoin: false));
                Summaries.Synchronize(context: context, ss: ss);
                ss.Notifications.ForEach(notification =>
                {
                    var body = new Dictionary<string, string>();
                    body.Add(
                        Displays.Issues_Updator(context: context),
                        context.User.Name);
                    body.Add(
                        Displays.Column(context: context),
                        column.LabelText);
                    body.Add(
                        Displays.Value(context: context),
                        issueModel.PropertyValue(
                              context: context,
                              name: column.ColumnName));
                    notification.Send(
                        context: context,
                        ss: ss,
                        title: Displays.BulkUpdated(
                            context: context,
                            data: count.ToString()).ToString(),
                        url: Locations.ItemIndex(
                            context: context,
                            ss.SiteId),
                        body: body.Select(o => o.Key + ":" + o.Value).Join("\n"));
                });
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkUpdated(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int BulkUpdate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where)
        {
            var sub = Rds.SelectIssues(
                column: Rds.IssuesColumn().IssueId(),
                where: where);
            var verUpWhere = Rds.IssuesWhere()
                .SiteId(ss.SiteId)
                .IssueId_In(sub: sub)
                .Or(or: Rds.IssuesWhere()
                    .Updator(context.UserId, _operator: "<>")
                    .UpdatedTime(
                        DateTime.Today.ToUniversal(context: context),
                        _operator: "<"));
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("BulkUpdateColumnName"));
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                formData: context.Forms);
                issueModel.PropertyValue(
                    context: context,
                    name: column.ColumnName);
            var statements = new List<SqlStatement>();
            statements.OnBulkUpdatingExtendedSqls(ss.SiteId);
            statements.Add(Rds.IssuesCopyToStatement(
                where: verUpWhere,
                tableType: Sqls.TableTypes.History,
                issueModel.ColumnNames()));
            statements.Add(Rds.UpdateIssues(
                where: verUpWhere,
                param: Rds.IssuesParam().Ver(raw: "[Ver]+1"),
                addUpdatorParam: false,
                addUpdatedTimeParam: false));
            var param = new Rds.IssuesParamCollection();
            switch (column.ColumnName)
            {
                case "Title":
                    param.Title(issueModel.Title.Value.MaxLength(1024));
                    break;
                case "Body":
                    param.Body(issueModel.Body);
                    break;
                case "StartTime":
                    param.StartTime(issueModel.StartTime);
                    break;
                case "CompletionTime":
                    param.CompletionTime(issueModel.CompletionTime.Value);
                    break;
                case "WorkValue":
                    param.WorkValue(issueModel.WorkValue.Value);
                    break;
                case "ProgressRate":
                    param.ProgressRate(issueModel.ProgressRate.Value);
                    break;
                case "Status":
                    param.Status(issueModel.Status.Value);
                    break;
                case "Manager":
                    param.Manager(issueModel.Manager.Id);
                    break;
                case "Owner":
                    param.Owner(issueModel.Owner.Id);
                    break;
                default:
                    var columnNameBracket = $"[{column.ColumnName}]";
                    switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                    {
                        case "Class":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: issueModel.Class(column.ColumnName).MaxLength(1024));
                            break;
                        case "Num":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: issueModel.Num(column.ColumnName));
                            break;
                        case "Date":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: issueModel.Date(column.ColumnName));
                            break;
                        case "Check":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: issueModel.Check(column.ColumnName));
                            break;
                        case "Description":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: issueModel.Description(column.ColumnName));
                            break;
                    }
                    break;
            }
            statements.Add(Rds.UpdateIssues(
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(sub: sub),
                param: param));
            statements.Add(Rds.RowCount());
            statements.OnBulkUpdatedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string UpdateByGrid(Context context, SiteSettings ss)
        {
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss)
                    .Where(o => !o.Suffix.IsNullOrEmpty())
                    .Where(o => o.SiteId == ss.SiteId)
                    .ToList();
            var statements = new List<SqlStatement>();
            var issueCollection = new IssueCollection(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .IssueId_In(formDataSet
                        .Where(formData => formData.Id > 0)
                        .Select(formData => formData.Id)),
                formDataSet: formDataSet);
            var notificationHash = issueCollection.ToDictionary(
                o => o.IssueId,
                o => o.GetNotifications(
                    context: context,
                    ss: ss,
                    notice: true,
                    before: true));
            foreach (var formData in formDataSet)
            {
                var issueModel = issueCollection
                    .FirstOrDefault(o => o.IssueId == formData.Id);
                if (issueModel != null)
                {
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: issueModel.Mine(context: context));
                    var invalid = IssueValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        issueModel: issueModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.Type.MessageJson(context: context);
                    }
                    issueModel.VerUp = Versions.MustVerUp(
                        context: context,
                        baseModel: issueModel);
                    statements.AddRange(issueModel.UpdateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                }
                else if (formData.Id < 0)
                {
                    issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        formData: formData.Data);
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: issueModel.Mine(context: context));
                    var invalid = IssueValidators.OnCreating(
                        context: context,
                        ss: ss,
                        issueModel: issueModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.Type.MessageJson(context: context);
                    }
                    statements.AddRange(issueModel.CreateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                }
                else
                {
                    return ItemUtilities.ClearItemDataResponse(
                        context: context,
                        ss: ss,
                        id: formData.Id)
                            .Remove($"[data-id=\"{formData.Id}\"][data-latest]")
                            .Message(Messages.NotFound(context: context))
                            .ToJson();
                }
            }
            var responses = Rds.ExecuteDataSet_responses(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            var response = responses.FirstOrDefault(o => !o.Event.IsNullOrEmpty());
            switch (response?.Event)
            {
                case "Duplicated":
                    return UpdateByGridDuplicated(
                        context: context,
                        ss: ss,
                        response: response);
                case "Conflicted":
                    return UpdateByGridConflicted(
                        context: context,
                        ss: ss,
                        response: response);
                default:
                    return UpdateByGridSuccess(
                        context: context,
                        ss: ss,
                        formDataSet: formDataSet,
                        issueCollection: issueCollection,
                        responses: responses,
                        notificationHash: notificationHash);
            }
        }

        private static string UpdateByGridDuplicated(
            Context context, SiteSettings ss, SqlResponse response)
        {
            return Messages.ResponseDuplicated(
                context: context,
                target: "row_" + response.Id,
                data: ss.GetColumn(
                    context: context,
                    columnName: response.ColumnName)?.LabelText)
                        .ToJson();
        }

        private static string UpdateByGridConflicted(
            Context context, SiteSettings ss, SqlResponse response)
        {
            var target = "row_" + response.Id;
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: response.Id.ToLong());
            return issueModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(
                    context: context,
                    target: target,
                    data: issueModel.Updator.Name)
                        .ToJson()
                : Messages.ResponseNotFound(
                    context: context,
                    target: target)
                        .ToJson();
        }

        private static string UpdateByGridSuccess(
            Context context,
            SiteSettings ss,
            List<FormData> formDataSet,
            IssueCollection issueCollection,
            List<SqlResponse> responses,
            Dictionary<long, List<Notification>> notificationHash)
        {
            responses
                .Where(o => o.DataTableName.ToInt() < 0)
                .ForEach(response =>
                    issueCollection.Add(new IssueModel(
                        context: context,
                        ss: ss,
                        issueId: response.Id.ToLong())));
            issueCollection.ForEach(issueModel =>
            {
                issueModel.SynchronizeSummary(
                    context: context,
                    ss: ss);
                var response = responses.FirstOrDefault(o => o.Id == issueModel.IssueId);
                if (response?.DataTableName.ToInt() < 0)
                {
                    issueModel.Notice(
                        context: context,
                        ss: ss,
                        notifications: issueModel.GetNotifications(
                            context: context,
                            ss: ss,
                            notice: true),
                        type: "Created");
                }
                else
                {
                    issueModel.Notice(
                        context: context,
                        ss: ss,
                        notifications: NotificationUtilities.MeetConditions(
                            ss: ss,
                            before: notificationHash.Get(issueModel.IssueId),
                            after: issueModel.GetNotifications(
                                context: context,
                                ss: ss,
                                notice: true)),
                        type: "Updated");
                }
            });
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new IssueCollection(
                    context: context,
                    ss: ss,
                    column: Rds.IssuesEditorColumns(ss: ss),
                    where: Rds.IssuesWhere()
                        .SiteId(ss.SiteId)
                        .IssueId_In(responses.Select(o => o.Id.ToLong())))
                            .SelectMany(o => o.UpdateRelatedRecordsStatements(
                                context: context,
                                ss: ss))
                                    .ToArray());
            var res = new ResponseCollection();
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var gridData = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(responses.Select(o => o.Id.ToLong())));
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            gridData.DataRows.ForEach(dataRow =>
                res.ReplaceAll(
                    $"[data-id=\"{responses.FirstOrDefault(o => o.Id == dataRow.Long("IssueId"))?.DataTableName}\"][data-latest]",
                    new HtmlBuilder().Tr(
                        context: context,
                        ss: ss,
                        dataRow: dataRow,
                        columns: columns,
                        gridSelector: null,
                        editRow: true,
                        checkRow: false,
                        idColumn: "IssueId")));
            formDataSet.ForEach(formData =>
                formData.Data.Keys.ForEach(controlId =>
                    res.ClearFormData(controlId + formData.Suffix)));
            return res
                .SetMemory("formChanged", false)
                .Message(Messages.UpdatedByGrid(
                    context: context,
                    data: responses.Count().ToString()))
                .ToJson();
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                setByApi: true);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(
                context: context,
                ss: ss);
            issueModel.VerUp = Versions.MustVerUp(
                context: context,
                baseModel: issueModel);
            var errorData = issueModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Updated(
                            context: context,
                            data: issueModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText);
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static string Copy(Context context, SiteSettings ss, long issueId)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                formData: context.Forms);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            issueModel.IssueId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                issueModel.Title.Value += Displays.SuffixCopy(context: context);
            }
            if (!context.Forms.Bool("CopyWithComments"))
            {
                issueModel.Comments.Clear();
            }
            issueModel.SetCopyDefault(
                context: context,
                ss: ss);
            var errorData = issueModel.Create(
                context, ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (ss.SwitchRecordWithAjax == true)
                    {
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            issueModel: issueModel,
                            message: Messages.Copied(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                issueId: issueModel.IssueId,
                                siteId: issueModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Copied(context: context));
                        return new ResponseCollection()
                            .Response("id", issueModel.IssueId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: issueModel.IssueId))
                            .ToJson();
                    }
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText)
                                .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Move(Context context, SiteSettings ss, long issueId)
        {
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(context: context, siteId: siteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnMoving(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: issueId));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var targetSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            var errorData = issueModel.Move(
                context: context,
                ss: ss,
                targetSs: targetSs);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (ss.SwitchRecordWithAjax == true)
                    {
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            issueModel: issueModel,
                            message: Messages.Moved(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                issueId: issueModel.IssueId,
                                siteId: issueModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Moved(
                                context: context,
                                data: issueModel.Title.DisplayValue));
                        return new ResponseCollection()
                            .Response("id", issueModel.IssueId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: issueModel.IssueId))
                            .ToJson();
                    }
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: targetSs.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText)
                                .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Delete(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(context, ss, issueId);
            var invalid = IssueValidators.OnDeleting(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = issueModel.Delete(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: issueModel.Title.Value));
                    var res = new IssuesResponseCollection(issueModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.Get(
                            context: context,
                            parts: new string[]
                            {
                                "Items",
                                ss.SiteId.ToString(),
                                ViewModes.GetSessionData(
                                    context: context,
                                    siteId: ss.SiteId)
                            }));
                    return res.ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult DeleteByApi(
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnDeleting(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(context: context, ss: ss);
            var errorData = issueModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Deleted(
                            context: context,
                            data: issueModel.Title.DisplayValue));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static string Restore(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            else if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector(context: context);
                var count = 0;
                if (selector.All)
                {
                    count = Restore(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = Restore(
                            context: context,
                            ss: ss,
                            selected: selector.Selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                Summaries.Synchronize(context: context, ss: ss);
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkRestored(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        public static int Restore(
            Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var where = Rds.IssuesWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Issues_Deleted")
                .IssueId_In(
                    value: selected,
                    tableName: "Issues_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .IssueId_In(
                    tableName: "Issues_Deleted",
                    sub: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.IssuesColumn().IssueId(),
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
            var sub = Rds.SelectIssues(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Issues_Deleted",
                column: Rds.IssuesColumn()
                    .IssueId(tableName: "Issues_Deleted"),
                where: where);
            var guid = Strings.NewGuid();
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.RestoreIssues(where: where),
                    Rds.RowCount(),
                    Rds.RestoreItems(where: Rds.ItemsWhere()
                        .SiteId(ss.SiteId)
                        .ReferenceType(guid)),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid),
                        param: Rds.ItemsParam()
                            .ReferenceType(ss.ReferenceType))
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long issueId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var issueModel = new IssueModel(context, ss, issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var ver = context.Forms.Data("GridCheckedItems")
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            if (ver.Count() != 1)
            {
                return Error.Types.SelectOne.MessageJson(context: context);
            }
            issueModel.SetByModel(new IssueModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId(issueId)
                    .Ver(ver.First())));
            issueModel.VerUp = true;
            var errorData = issueModel.Update(
                context: context,
                ss: ss,
                otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: issueId))
                        .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long issueId, Message message = null)
        {
            var issueModel = new IssueModel(context: context, ss: ss, issueId: issueId);
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
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
                                ss: ss,
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                issueModel: issueModel)));
            return new IssuesResponseCollection(issueModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            IssueModel issueModel)
        {
            new IssueCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.IssuesWhere().IssueId(issueModel.IssueId),
                orderBy: Rds.IssuesOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(issueModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(issueModelHistory.Ver)
                                .DataLatest(1, _using:
                                    issueModelHistory.Ver == issueModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: issueModelHistory.Ver.ToString(),
                                            _using: issueModelHistory.Ver < issueModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            issueModel: issueModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.IssuesColumnCollection()
                .IssueId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.IssuesColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(context: context, ss: ss, issueId: issueId);
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
            issueModel.Get(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .IssueId(issueModel.IssueId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            issueModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, issueModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        issueId.ToString() 
                            + (issueModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        public static string EditSeparateSettings(
            Context context, SiteSettings ss, long issueId)
        {
            if (ss.AllowSeparate != true)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            return new ResponseCollection()
                .Html(
                    "#SeparateSettingsDialog",
                    new HtmlBuilder().SeparateSettings(
                        context: context,
                        ss: ss,
                        title: issueModel.Title.Value,
                        workValue: issueModel.WorkValue.Value,
                        mine: issueModel.Mine(context: context)))
                .Invoke("separateSettings")
                .ToJson();
        }

        public static string Separate(Context context, SiteSettings ss, long issueId)
        {
            if (ss.AllowSeparate != true)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var number = context.Forms.Int("SeparateNumber");
            if (context.ContractSettings.ItemsLimit(
                context: context,
                siteId: ss.SiteId,
                number: number - 1))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (number >= 2)
            {
                var hash = new Dictionary<int, IssueModel>
                {
                    {
                        1,
                        new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: issueModel.IssueId)
                    }
                };
                var ver = issueModel.Ver;
                var comments = issueModel.Comments.ToJson();
                for (var index = 2; index <= number; index++)
                {
                    issueModel.SetCopyDefault(
                        context: context,
                        ss: ss);
                    issueModel.IssueId = 0;
                    issueModel.Create(
                        context: context,
                        ss: ss,
                        otherInitValue: true,
                        get: false);
                    hash.Add(index, new IssueModel(
                        context: context,
                        ss: ss,
                        issueId: issueModel.IssueId));
                }
                var addCommentCollection = new List<string>();
                addCommentCollection.AddRange(hash.Select(o => "[{0}]({1}{2})  ".Params(
                    context.Forms.Data("SeparateTitle_" + o.Key),
                    context.Server,
                    Locations.ItemEdit(
                        context: context,
                        id: o.Value.IssueId))));
                var addComment = "[md]\n{0}  \n{1}".Params(
                    Displays.Separated(context: context),
                    addCommentCollection.Join("\n"));
                for (var index = number; index >= 1; index--)
                {
                    var source = index == 1;
                    issueModel = hash[index];
                    issueModel.Ver = source
                        ? ver
                        : 1;
                    issueModel.Title.Value = context.Forms.Data("SeparateTitle_" + index);
                    issueModel.WorkValue.Value = source
                        ? context.Forms.Decimal(
                            context: context,
                            key: "SourceWorkValue")
                        : context.Forms.Decimal(
                            context: context,
                            key: "SeparateWorkValue_" + index);
                    issueModel.Comments.Clear();
                    if (source || context.Forms.Bool("SeparateCopyWithComments"))
                    {
                        issueModel.Comments = comments.Deserialize<Comments>();
                    }
                    issueModel.Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: addComment);
                    issueModel.Update(
                        context: context,
                        ss: ss,
                        forceSynchronizeSourceSummary: true,
                        otherInitValue: true);
                }
                return EditorResponse(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    message: Messages.Separated(context: context)).ToJson();
            }
            else
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
        }

        public static string BulkMove(Context context, SiteSettings ss)
        {
            var where = SelectedWhere(
                context: context,
                ss: ss);
            if (where == null)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(
                context: context,
                siteId: siteId,
                number: BulkMoveCount(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    where: where)))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            if (Permissions.CanMove(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: siteId)))
            {
                var count = BulkMove(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    where: Views.GetBySession(
                        context: context,
                        ss: ss)
                            .Where(
                                context: context,
                                ss: ss,
                                where: where,
                                itemJoin: false));
                Summaries.Synchronize(context: context, ss: ss);
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkMoved(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int BulkMoveCount(
            Context context,
            SiteSettings ss,
            long siteId,
            SqlWhereCollection where)
        {
            return Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: ss.Join(
                        context: context,
                        join: where),
                    where: where));
        }

        private static int BulkMove(
            Context context,
            SiteSettings ss,
            long siteId,
            SqlWhereCollection where)
        {
            var sub = Rds.SelectIssues(
                column: Rds.IssuesColumn().IssueId(),
                where: where);
            var guid = Strings.NewGuid();
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.UpdateIssues(
                        where: where,
                        param: Rds.IssuesParam()
                            .SiteId(siteId)),
                    Rds.RowCount(),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid),
                        param: Rds.ItemsParam()
                            .SiteId(siteId)
                            .ReferenceType(ss.ReferenceType))
                }).Count.ToInt();
        }

        public static string BulkDelete(Context context, SiteSettings ss)
        {
            if (context.CanDelete(ss: ss))
            {
                var where = SelectedWhere(
                    context: context,
                    ss: ss);
                if (where == null)
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: Views.GetBySession(
                        context: context,
                        ss: ss)
                            .Where(
                                context: context,
                                ss: ss,
                                where: where,
                                itemJoin: false));
                Summaries.Synchronize(context: context, ss: ss);
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int BulkDelete(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where)
        {
            var sub = Rds.SelectIssues(
                column: Rds.IssuesColumn().IssueId(),
                join: ss.Join(
                    context: context,
                    join: where),
                where: where);
            var statements = new List<SqlStatement>();
            var guid = Strings.NewGuid();
            statements.OnBulkDeletingExtendedSqls(ss.SiteId);
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere()
                    .SiteId(ss.SiteId)
                    .ReferenceId_In(sub: sub),
                param: Rds.ItemsParam()
                    .ReferenceType(guid)));
            statements.Add(Rds.DeleteBinaries(
                where: Rds.BinariesWhere()
                    .TenantId(context.TenantId)
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteIssues(where: where));
            statements.Add(Rds.RowCount());
            statements.Add(Rds.DeleteItems(
                where: Rds.ItemsWhere()
                    .SiteId(ss.SiteId)
                    .ReferenceType(guid)));
            statements.Add(Rds.UpdateItems(
                tableType: Sqls.TableTypes.Deleted,
                where: Rds.ItemsWhere()
                    .SiteId(ss.SiteId)
                    .ReferenceType(guid),
                param: Rds.ItemsParam()
                    .ReferenceType(ss.ReferenceType)));
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long issueId)
        {
            if (!Parameters.History.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector(context: context);
                var selected = selector
                    .Selected
                    .Select(o => o.ToInt())
                    .ToList();
                var count = 0;
                if (selector.All)
                {
                    count = DeleteHistory(
                        context: context,
                        ss: ss,
                        issueId: issueId,
                        selected: selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = DeleteHistory(
                            context: context,
                            ss: ss,
                            issueId: issueId,
                            selected: selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                return Histories(
                    context: context,
                    ss: ss,
                    issueId: issueId,
                    message: Messages.HistoryDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int DeleteHistory(
            Context context,
            SiteSettings ss,
            long issueId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteIssues(
                        tableType: Sqls.TableTypes.History,
                        where: Rds.IssuesWhere()
                            .SiteId(
                                value: ss.SiteId,
                                tableName: "Issues_History")
                            .IssueId(
                                value: issueId,
                                tableName: "Issues_History")
                            .Ver_In(
                                value: selected,
                                tableName: "Issues_History",
                                negative: negative,
                                _using: selected.Any())
                            .IssueId_In(
                                tableName: "Issues_History",
                                sub: Rds.SelectIssues(
                                    tableType: Sqls.TableTypes.History,
                                    column: Rds.IssuesColumn().IssueId(),
                                    where: Views.GetBySession(
                                        context: context,
                                        ss: ss).Where(
                                            context: context,
                                            ss: ss)))),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string PhysicalDelete(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector(context: context);
                var count = 0;
                if (selector.All)
                {
                    count = PhysicalDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = PhysicalDelete(
                            context: context,
                            ss: ss,
                            selected: selector.Selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.PhysicalDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int PhysicalDelete(
            Context context,
            SiteSettings ss,
            List<long> selected,
            bool negative = false)
        {
            var where = Rds.IssuesWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Issues_Deleted")
                .IssueId_In(
                    value: selected,
                    tableName: "Issues_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .IssueId_In(
                    tableName: "Issues_Deleted",
                    sub: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.IssuesColumn().IssueId(),
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
            var sub = Rds.SelectIssues(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Issues_Deleted",
                column: Rds.IssuesColumn()
                    .IssueId(tableName: "Issues_Deleted"),
                where: where);
            var guid = Strings.NewGuid();
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        where: where),
                    Rds.RowCount(),
                    Rds.PhysicalDeleteItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid)),
                }).Count.ToInt();
        }

        public static string Import(Context context, SiteModel siteModel)
        {
            var ss = siteModel.IssuesSiteSettings(
                context: context,
                referenceId: siteModel.SiteId,
                setAllChoices: true);
            if (context.ContractSettings.Import == false)
            {
                return Messages.ResponseRestricted(context: context).ToJson();
            }
            if (!context.CanCreate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var res = new ResponseCollection();
            Csv csv;
            try
            {
                csv = new Csv(
                    csv: context.PostedFiles.FirstOrDefault().Byte(),
                    encoding: context.Forms.Data("Encoding"));
            }
            catch
            {
                return Messages.ResponseFailedReadFile(context: context).ToJson();
            }
            var count = csv.Rows.Count();
            if (Parameters.General.ImportMax > 0 && Parameters.General.ImportMax < count)
            {
                return Error.Types.ImportMax.MessageJson(
                    context: context,
                    data: Parameters.General.ImportMax.ToString());
            }
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId, number: count))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            if (csv != null && count > 0)
            {
                var idColumn = -1;
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column?.ColumnName == "IssueId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalid = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.ColumnName), "CompletionTime");
                if (invalid != null) return invalid;
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var issueHash = new Dictionary<int, IssueModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var issueModel = new IssueModel() { SiteId = ss.SiteId };
                    if (context.Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            issueModel = model;
                        }
                    }
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            context: context,
                            column: column.Value,
                            value: data.Row[column.Key],
                            inheritPermission: ss.InheritPermission);
                        switch (column.Value.ColumnName)
                        {
                            case "Title":
                                issueModel.Title.Value = recordingData.ToString();
                                break;
                            case "Body":
                                issueModel.Body = recordingData.ToString();
                                break;
                            case "StartTime":
                                issueModel.StartTime = recordingData.ToDateTime();
                                break;
                            case "CompletionTime":
                                issueModel.CompletionTime.Value = recordingData.ToDateTime();
                                break;
                            case "WorkValue":
                                issueModel.WorkValue.Value = recordingData.ToDecimal();
                                break;
                            case "ProgressRate":
                                issueModel.ProgressRate.Value = recordingData.ToDecimal();
                                break;
                            case "Status":
                                issueModel.Status.Value = recordingData.ToInt();
                                break;
                            case "Manager":
                                issueModel.Manager = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Owner":
                                issueModel.Owner = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Comments":
                                if (issueModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    issueModel.Comments.Prepend(
                                        context: context,
                                        ss: ss,
                                        body: data.Row[column.Key]);
                                }
                                break;
                            default:
                                issueModel.Value(
                                    context: context,
                                    columnName: column.Value.ColumnName,
                                    value: recordingData);
                                break;
                        }
                    });
                    issueHash.Add(data.Index, issueModel);
                });
                var errorCompletionTime = Imports.Validate(
                    context: context,
                    hash: issueHash.ToDictionary(
                        o => o.Key,
                        o => o.Value.CompletionTime.Value.ToString()),
                    column: ss.GetColumn(context: context, columnName: "CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                var insertCount = 0;
                var updateCount = 0;
                foreach (var issueModel in issueHash.Values)
                {
                    issueModel.SetByFormula(context: context, ss: ss);
                    issueModel.SetTitle(context: context, ss: ss);
                    if (issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        issueModel.VerUp = Versions.MustVerUp(
                            context: context, baseModel: issueModel);
                        if (issueModel.Updated(context: context))
                        {
                            var errorData = issueModel.Update(
                                context: context,
                                ss: ss,
                                extendedSqls: false,
                                get: false);
                            switch (errorData.Type)
                            {
                                case Error.Types.None:
                                    break;
                                case Error.Types.Duplicated:
                                    return Messages.ResponseDuplicated(
                                        context: context,
                                        data: ss.GetColumn(
                                            context: context,
                                            columnName: errorData.ColumnName)?.LabelText)
                                                .ToJson();
                                default:
                                    return errorData.Type.MessageJson(context: context);
                            }
                            updateCount++;
                        }
                    }
                    else
                    {
                        var errorData = issueModel.Create(
                            context: context,
                            ss: ss,
                            extendedSqls: false,
                            get: false);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            case Error.Types.Duplicated:
                                return Messages.ResponseDuplicated(
                                    context: context,
                                    data: ss.GetColumn(
                                        context: context,
                                        columnName: errorData.ColumnName)?.LabelText)
                                            .ToJson();
                            default:
                                return errorData.Type.MessageJson(context: context);
                        }
                        insertCount++;
                    }
                }
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportedExtendedSqls(ss.SiteId).ToArray());
                return GridRows(
                    context: context,
                    ss: ss,
                    res: res.WindowScrollTop(),
                    message: Messages.Imported(
                        context: context,
                        data: new string[]
                        {
                            insertCount.ToString(),
                            updateCount.ToString()
                        }));
            }
            else
            {
                return Messages.ResponseFileNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ImportRecordingData(
            Context context, Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(
                context: context,
                value: value,
                siteId: inheritPermission);
            switch (column.ColumnName)
            {
                case "CompletionTime":
                    recordingData = recordingData
                        .ToDateTime()
                        .AddDifferenceOfDates(column.EditorFormat)
                        .ToString();
                    break;
            }
            return recordingData;
        }

        public static string OpenExportSelectorDialog(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidRequest));
            }
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            return new ResponseCollection()
                .Html(
                    "#ExportSelectorDialog",
                    new HtmlBuilder().ExportSelectorDialog(
                        context: context,
                        ss: ss))
                .ToJson();
        }

        public static ResponseFile Export(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var export = ss.GetExport(
                    context: context,
                    id: context.QueryStrings.Int("id"));
            var content = ExportUtilities.Export(
                context: context,
                ss: ss,
                export: export,
                where: SelectedWhere(
                    context: context,
                    ss: ss),
                view: Views.GetBySession(
                    context: context,
                    ss: ss));
            return new ResponseFile(
                fileContent: content,
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    ss: ss,
                    name: export.Name,
                    extension: export.Type.ToString()));
        }

        public static string ExportAsync(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var export = ss.GetExport(
                        context: context,
                        id: context.Forms.Int("ExportId"));
                var fileName = ExportUtilities.FileName(
                    context: context,
                    ss: ss,
                    name: export.Name,
                    extension: export.Type.ToString());
                try
                {
                    var content = ExportUtilities.Export(
                        context: context,
                        ss: ss,
                        export: export,
                        where: SelectedWhere(
                            context: context,
                            ss: ss),
                        view: Views.GetBySession(
                            context: context,
                            ss: ss));
                    var guid = Strings.NewGuid();
                    var bytes = content.ToBytes();
                    Rds.ExecuteNonQuery(
                        context: context,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertBinaries(
                                param: Rds.BinariesParam()
                                    .TenantId(context.TenantId)
                                    .ReferenceId(ss.SiteId)
                                    .Guid(guid)
                                    .Title(fileName)
                                    .BinaryType("Attachments")
                                    .Bin(bytes)
                                    .FileName(fileName)
                                    .Extension(export.Type.ToString())
                                    .Size(bytes.Length)
                                    .ContentType(export.Type == Libraries.Settings.Export.Types.Csv
                                        ? "text/csv"
                                        : "application/json"))
                        });
                    new OutgoingMailModel()
                    {
                        Title = new Title(Displays.ExportEmailTitle(context: context,fileName)),
                        Body = Displays.ExportEmailBody(context: context) + "\n" +
                            $"{(Parameters.Service.AbsoluteUri ?? context.Server)}" +
                            $"/{Locations.DownloadFile(context: context, guid: guid)}",
                        From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                        To = MailAddressUtilities.Get(context: context, context.UserId),
                    }.Send(context: context, ss);
                }
                catch(Exception e)
                {
                    new OutgoingMailModel()
                    {
                        Title = new Title(Displays.ExportEmailTitleFaild(context: context)),
                        Body = Displays.ExportEmailBodyFaild(context: context, fileName, e.Message),
                        From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                        To = MailAddressUtilities.Get(context: context, context.UserId),
                    }.Send(context: context, ss);
                }
            });
            return Messages.ResponseExportAccepted(context: context).ToJson();
        }

        public static System.Web.Mvc.ContentResult ExportByApi(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            var api = context.RequestDataString.Deserialize<ExportApi>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var export = api.Export ?? ss.GetExport(
                context: context,
                id: api.ExportId);
            var content = ExportUtilities.Export(
                context: context,
                ss: ss,
                export: export,
                where: SelectedWhere(
                    context: context,
                    ss: ss),
                view: api.View ?? new View());
            return ApiResults.Get(
                new
                {
                    StatusCode = 200,
                    Response = new
                    {
                        Name = ExportUtilities.FileName(
                            context: context,
                            ss: ss,
                            name: export.Name,
                            extension: export.Type.ToString()),
                        Content = content
                    }
                }.ToJson());
        }

        public static ResponseFile ExportCrosstab(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false) return null;
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context, columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            return new ResponseFile(
                Libraries.ViewModes.CrosstabUtilities.Csv(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    columns: columns,
                    aggregateType: aggregateType,
                    value: value,
                    timePeriod: timePeriod,
                    month: month,
                    dataRows: dataRows),
                ExportUtilities.FileName(
                    context: context,
                    ss: ss,
                    name: Displays.Crosstab(context: context)));
        }

        public static string Calendar(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Calendar"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var timePeriod = view.GetCalendarTimePeriod(ss: ss);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(
                context: context,
                date: month,
                timePeriod: timePeriod);
            var end = Calendars.EndDate(
                context: context,
                date: month,
                timePeriod: timePeriod);
            var dataRows = CalendarDataRows(
                context: context,
                ss: ss,
                view: view,
                fromColumn: fromColumn,
                toColumn: toColumn,
                begin: Calendars.BeginDate(
                    context: context,
                    date: month,
                    timePeriod: timePeriod),
                end: Calendars.EndDate(
                    context: context,
                    date: month,
                    timePeriod: timePeriod));
            var inRange = dataRows.Count() <= Parameters.General.CalendarLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.CalendarLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Calendar(
                        context: context,
                        ss: ss,
                        timePeriod: timePeriod,
                        fromColumn: fromColumn,
                        toColumn: toColumn,
                        month: month,
                        begin: begin,
                        dataRows: dataRows,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string UpdateByCalendar(Context context, SiteSettings ss)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: context.Forms.Long("Id"),
                formData: context.Forms);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: issueModel);
            issueModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return CalendarJson(
                context: context,
                ss: ss,
                changedItemId: issueModel.IssueId,
                update: true,
                message: Messages.Updated(
                    context: context,
                    data: issueModel.Title.DisplayValue));
        }

        public static string CalendarJson(
            Context context,
            SiteSettings ss,
            long changedItemId = 0,
            bool update = false,
            Message message = null)
        {
            if (!ss.EnableViewMode(context: context, name: "Calendar"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Calendar");
            var timePeriod = view.GetCalendarTimePeriod(ss: ss);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(
                context: context,
                date: month,
                timePeriod: timePeriod);
            var end = Calendars.EndDate(
                context: context,
                date: month,
                timePeriod: timePeriod);
            var dataRows = CalendarDataRows(
                context: context,
                ss: ss,
                view: view,
                fromColumn: fromColumn,
                toColumn: toColumn,
                begin: Calendars.BeginDate(
                    context: context,
                    date: month,
                    timePeriod: timePeriod),
                end: Calendars.EndDate(
                    context: context,
                    date: month,
                    timePeriod: timePeriod));
            return dataRows.Count() <= Parameters.General.CalendarLimit
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "setCalendar",
                        message: message,
                        loadScroll: update,
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
                                context: context,
                                ss: ss,
                                timePeriod: timePeriod,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                month: month,
                                begin: begin,
                                dataRows: dataRows,
                                bodyOnly: bodyOnly,
                                inRange: true,
                                changedItemId: changedItemId))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.CalendarLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
                                context: context,
                                ss: ss,
                                timePeriod: timePeriod,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                month: month,
                                begin: begin,
                                dataRows: dataRows,
                                bodyOnly: bodyOnly,
                                inRange: false,
                                changedItemId: changedItemId))
                    .ToJson();
        }

        private static EnumerableRowCollection<DataRow> CalendarDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column fromColumn,
            Column toColumn,
            DateTime begin,
            DateTime end)
        {
            var where = new SqlWhereCollection();
            if (toColumn == null)
            {
                where.Add(
                    tableName: "Issues",
                    raw: $"[Issues].[{fromColumn.ColumnName}] between '{begin}' and '{end}'");
            }
            else
            {
                where.Or(or: Rds.IssuesWhere()
                    .Add(raw: $"[Issues].[{fromColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Issues].[{toColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Issues].[{fromColumn.ColumnName}]<='{begin}' and [Issues].[{toColumn.ColumnName}]>='{end}'"));
            }
            where = view.Where(context: context, ss: ss, where: where);
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(
                        context: context,
                        ss: ss)
                            .IssueId(_as: "Id")
                            .IssuesColumn(
                                columnName: fromColumn.ColumnName,
                                _as: "From")
                            .IssuesColumn(
                                columnName: toColumn?.ColumnName,
                                _as: "To")
                            .UpdatedTime()
                            .ItemTitle(ss.ReferenceType),
                    join: ss.Join(
                        context: context,
                        join: where),
                    where: where))
                        .AsEnumerable();
        }

        private static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
            Column fromColumn,
            Column toColumn,
            DateTime month,
            DateTime begin,
            EnumerableRowCollection<DataRow> dataRows,
            bool bodyOnly,
            bool inRange,
            long changedItemId = 0)
        {
            return !bodyOnly
                ? hb.Calendar(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId)
                : hb.CalendarBody(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId);
        }

        public static string Crosstab(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Crosstab"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(
                context: context,
                dataRows: dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns"
                    || Libraries.ViewModes.CrosstabUtilities.InRangeY(
                        context: context,
                        dataRows: dataRows);
            if (!inRangeX)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyColumnCases(
                        context: context,
                        data: Parameters.General.CrosstabXLimit.ToString()));
            }
            else if (!inRangeY)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyColumnCases(
                        context: context,
                        data: Parameters.General.CrosstabYLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Crosstab(
                        context: context,
                        ss: ss,
                        view: view,
                        groupByX: groupByX,
                        groupByY: groupByY,
                        columns: columns,
                        aggregateType: aggregateType,
                        value: value,
                        timePeriod: timePeriod,
                        month: month,
                        dataRows: dataRows,
                        inRange: inRangeX && inRangeY));
        }

        public static string CrosstabJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Crosstab"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(
                context: context,
                dataRows: dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns"
                    || Libraries.ViewModes.CrosstabUtilities.InRangeY(
                        context: context,
                        dataRows: dataRows);
            var bodyOnly = context.Forms.ControlId().StartsWith("Crosstab");
            return inRangeX && inRangeY
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "setCrosstab",
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows)
                            : new HtmlBuilder().CrosstabBody(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: !inRangeX
                            ? Messages.TooManyColumnCases(
                                context: context,
                                data: Parameters.General.CrosstabXLimit.ToString())
                            : Messages.TooManyRowCases(
                                context: context,
                                data: Parameters.General.CrosstabYLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows,
                                inRange: false)
                            : new HtmlBuilder())
                    .ToJson();
        }

        private static List<Column> CrosstabColumns(
            Context context, SiteSettings ss, View view)
        {
            return Libraries.ViewModes.CrosstabUtilities.GetColumns(
                context: context,
                ss: ss,
                columns: view.CrosstabColumns?.Deserialize<IEnumerable<string>>()?
                    .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                    .ToList());
        }

        private static EnumerableRowCollection<DataRow> CrosstabDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            List<Column> columns,
            Column value,
            string aggregateType,
            string timePeriod,
            DateTime month)
        {
            EnumerableRowCollection<DataRow> dataRows;
            if (groupByX?.TypeName != "datetime")
            {
                var column = Rds.IssuesColumn()
                    .WithItemTitle(
                        ss: ss,
                        column: groupByX)
                    .CrosstabColumns(
                        context: context,
                        ss: ss,
                        view: view,
                        groupByY: groupByY,
                        columns: columns,
                        value: value,
                        aggregateType: aggregateType);
                var where = view.Where(
                    context: context,
                    ss: ss);
                var groupBy = Rds.IssuesGroupBy()
                    .WithItemTitle(
                        ss: ss,
                        column: groupByX)
                    .WithItemTitle(
                        ss: ss,
                        column: groupByY);
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectIssues(
                        column: column,
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                column,
                                where,
                                groupBy
                            }),
                        where: where,
                        groupBy: groupBy))
                            .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    context: context,
                    ss: ss,
                    column: groupByX,
                    timePeriod: timePeriod);
                var column = Rds.IssuesColumn()
                    .Add(dateGroup, _as: groupByX.ColumnName)
                    .CrosstabColumns(
                        context: context,
                        ss: ss,
                        view: view,
                        groupByY: groupByY,
                        columns: columns,
                        value: value,
                        aggregateType: aggregateType);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: Libraries.ViewModes.CrosstabUtilities.Where(
                        context: context,
                        ss: ss,
                        column: groupByX,
                        timePeriod: timePeriod,
                        month: month));
                var groupBy = Rds.IssuesGroupBy()
                    .Add(dateGroup)
                    .WithItemTitle(
                        ss: ss,
                        column: groupByY);
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectIssues(
                        column: column,
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                column,
                                where,
                                groupBy
                            }),
                        where: where,
                        groupBy: groupBy))
                            .AsEnumerable();
            }
            ss.SetChoiceHash(dataRows: dataRows);
            return dataRows;
        }

        private static SqlColumnCollection CrosstabColumns(
            this SqlColumnCollection self,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByY,
            List<Column> columns,
            Column value,
            string aggregateType)
        {
            if (view.CrosstabGroupByY != "Columns")
            {
                return self
                    .WithItemTitle(
                        ss: ss,
                        column: groupByY)
                    .Add(
                        column: value,
                        _as: "Value",
                        function: Sqls.Function(aggregateType));
            }
            else
            {
                columns.ForEach(column =>
                    self.Add(
                        column: column,
                        _as: column.ColumnName,
                        function: Sqls.Function(aggregateType)));
                return self;
            }
        }

        public static string Gantt(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Gantt"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttSortBy());
            var range = new Libraries.ViewModes.GanttRange(
                context: context,
                ss: ss,
                view: view);
            var dataRows = GanttDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                sortBy: sortBy);
            var inRange = dataRows.Count() <= Parameters.General.GanttLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.GanttLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Gantt(
                        context: context,
                        ss: ss,
                        view: view,
                        dataRows: dataRows,
                        groupBy: groupBy,
                        sortBy: sortBy,
                        period: view.GanttPeriod.ToInt(),
                        startDate: view.GanttStartDate.ToDateTime(),
                        range: range,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string GanttJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Gantt"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Gantt");
            var range = new Libraries.ViewModes.GanttRange(
                context: context,
                ss: ss,
                view: view);
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttSortBy());
            var period = view.GanttPeriod.ToInt();
            var startDate = view.GanttStartDate.ToDateTime();
            var dataRows = GanttDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                sortBy: sortBy);
            if (dataRows.Count() <= Parameters.General.GanttLimit)
            {
                return new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "drawGantt",
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
                                context: context,
                                ss: ss,
                                view: view,
                                dataRows: dataRows,
                                groupBy: groupBy,
                                sortBy: sortBy,
                                period: period,
                                startDate: startDate,
                                range: range,
                                bodyOnly: bodyOnly,
                                inRange: true))
                    .ToJson();
            }
            else
            {
                return new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.GanttLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
                                context: context,
                                ss: ss,
                                view: view,
                                dataRows: dataRows,
                                groupBy: groupBy,
                                sortBy: sortBy,
                                period: view.GanttPeriod.ToInt(),
                                startDate: view.GanttStartDate.ToDateTime(),
                                range: range,
                                bodyOnly: bodyOnly,
                                inRange: false))
                    .ToJson();
            }
        }

        private static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            EnumerableRowCollection<DataRow> dataRows,
            Column groupBy,
            Column sortBy,
            int period,
            DateTime startDate,
            Libraries.ViewModes.GanttRange range,
            bool bodyOnly,
            bool inRange = true)
        {
            return !bodyOnly
                ? hb.Gantt(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    period: period,
                    startDate: startDate,
                    range: range,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.GanttBody(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    period: period,
                    startDate: startDate,
                    range: range,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> GanttDataRows(
            Context context, SiteSettings ss, View view, Column groupBy, Column sortBy)
        {
            var where = view.Where(
                context: context,
                ss: ss,
                where: Libraries.ViewModes.GanttUtilities.Where(
                    context: context, ss: ss, view: view));
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(context: context, ss: ss)
                        .IssueId()
                        .WorkValue()
                        .StartTime()
                        .CompletionTime()
                        .ProgressRate()
                        .Status()
                        .Owner()
                        .Updator()
                        .CreatedTime()
                        .UpdatedTime()
                        .ItemTitle(ss.ReferenceType)
                        .Add(
                            column: groupBy,
                            function: Sqls.Functions.SingleColumn)
                        .Add(
                            column: sortBy,
                            function: Sqls.Functions.SingleColumn),
                    join: ss.Join(
                        context: context,
                        join: where),
                    where: where))
                                .AsEnumerable();
        }

        public static string BurnDown(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "BurnDown"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.BurnDownLimit);
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.BurnDownLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.BurnDown(
                            context: context,
                            ss: ss,
                            dataRows: BurnDownDataRows(context: context, ss: ss, view: view),
                            ownerLabelText: ss.GetColumn(context: context, columnName: "Owner").GridLabelText,
                            column: ss.GetColumn(context: context, columnName: "WorkValue"));
                    }
                });
        }

        public static string BurnDownJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "BurnDown"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            return InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.BurnDownLimit)
                    ? new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "drawBurnDown",
                            body: new HtmlBuilder()
                                .BurnDown(
                                    context: context,
                                    ss: ss,
                                    dataRows: BurnDownDataRows(
                                        context: context,
                                        ss: ss,
                                        view: view),
                                    ownerLabelText: ss.GetColumn(
                                        context: context,
                                        columnName: "Owner").GridLabelText,
                                    column: ss.GetColumn(
                                        context: context,
                                        columnName: "WorkValue")))
                        .ToJson()
                    : new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.BurnDownLimit.ToString()),
                            body: new HtmlBuilder())
                        .ToJson();
        }

        public static string BurnDownRecordDetails(Context context, SiteSettings ss)
        {
            var date = context.Forms.DateTime("BurnDownDate");
            return new ResponseCollection()
                .After(string.Empty, new HtmlBuilder().BurnDownRecordDetails(
                    context: context,
                    elements: new Libraries.ViewModes.BurnDown(
                        context: context,
                        ss: ss,
                        dataRows: BurnDownDataRows(
                            context: context,
                            ss: ss,
                            view: Views.GetBySession(context: context, ss: ss)))
                                .Where(o => o.UpdatedTime == date),
                    progressRateColumn: ss.GetColumn(
                        context: context,
                        columnName: "ProgressRate"),
                    statusColumn: ss.GetColumn(
                        context: context,
                        columnName: "Status"),
                    colspan: context.Forms.Int("BurnDownColspan"),
                    unit: ss.GetColumn(
                        context: context,
                        columnName: "WorkValue").Unit)).ToJson();
        }

        private static EnumerableRowCollection<DataRow> BurnDownDataRows(
            Context context, SiteSettings ss, View view)
        {
            var where = view.Where(context: context, ss: ss);
            var join = ss.Join(
                context: context,
                join: where);
            return Rds.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectIssues(
                        column: Rds.IssuesTitleColumn(context: context, ss: ss)
                            .IssueId()
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: join,
                        where: where),
                    Rds.SelectIssues(
                        unionType: Sqls.UnionTypes.Union,
                        tableType: Sqls.TableTypes.HistoryWithoutFlag,
                        column: Rds.IssuesTitleColumn(context: context, ss: ss)
                            .IssueId(_as: "Id")
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: join,
                        where: Rds.IssuesWhere()
                            .IssueId_In(sub: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: where)),
                        orderBy: Rds.IssuesOrderBy()
                            .IssueId()
                            .Ver())
                }).AsEnumerable();
        }

        public static string TimeSeries(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "TimeSeries"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.TimeSeriesLimit);
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.TimeSeriesLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TimeSeries(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string TimeSeriesJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "TimeSeries"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("TimeSeries");
            return InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.TimeSeriesLimit)
                    ? new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "drawTimeSeries",
                            bodyOnly: bodyOnly,
                            bodySelector: "#TimeSeriesBody",
                            body: new HtmlBuilder()
                                .TimeSeries(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: true))
                        .ToJson()
                    : new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.TimeSeriesLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: "#TimeSeriesBody",
                            body: new HtmlBuilder()
                                .TimeSeries(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: false))
                        .ToJson();
        }

        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            bool inRange)
        {
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesGroupBy(ss));
            var aggregationType = view.GetTimeSeriesAggregationType(
                context: context,
                ss: ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesValue(ss));
            var dataRows = TimeSeriesDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                value: value);
            if (groupBy == null)
            {
                return hb;
            }
            return !bodyOnly
                ? hb.TimeSeries(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.TimeSeriesBody(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            Context context, SiteSettings ss, View view, Column groupBy, Column value)
        {
            if (groupBy != null && value != null)
            {
                var column = Rds.IssuesColumn()
                    .IssueId(_as: "Id")
                    .Ver()
                    .UpdatedTime()
                    .Add(column: groupBy)
                    .Add(column: value);
                var where = view.Where(context: context, ss: ss);
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: column,
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                                {
                                    column,
                                    where
                                }),
                        where: where))
                            .AsEnumerable();
                ss.SetChoiceHash(dataRows: dataRows);
                return dataRows;
            }
            else
            {
                return null;
            }
        }

        public static string Kamban(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Kamban"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.KambanLimit);
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.KambanLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Kamban(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string KambanJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Kamban"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Kamban");
            return InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.KambanLimit)
                    ? new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "setKamban",
                            bodyOnly: bodyOnly,
                            bodySelector: "#KambanBody",
                            body: new HtmlBuilder()
                                .Kamban(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    changedItemId: context.Forms.Long("KambanId")))
                        .ToJson()
                    : new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.KambanLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: "#KambanBody",
                            body: new HtmlBuilder()
                                .Kamban(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: false))
                        .ToJson();
        }

        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            long changedItemId = 0,
            bool inRange = true)
        {
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetKambanGroupByX(
                    context: context,
                    ss: ss));
            if (groupByX == null)
            {
                return hb;
            }
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetKambanGroupByY(
                    context: context,
                    ss: ss));
            var aggregateType = view.GetKambanAggregationType(
                context: context,
                ss: ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetKambanValue(ss));
            var aggregationView = view.KambanAggregationView ?? false;
            var columns = view.GetKambanColumns();
            var data = KambanData(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                value: value);
            return !bodyOnly
                ? hb.Kamban(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    columns: columns,
                    aggregationView: aggregationView,
                    data: data,
                    inRange: inRange)
                : hb.KambanBody(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    columns: columns,
                    aggregationView: aggregationView,
                    data: data,
                    changedItemId: changedItemId,
                    inRange: inRange);
        }

        private static IEnumerable<Libraries.ViewModes.KambanElement> KambanData(
            Context context, 
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            Column value)
        {
            var column = Rds.IssuesColumn()
                .IssueId()
                .ItemTitle(ss.ReferenceType)
                .Add(column: groupByX)
                .Add(column: groupByY)
                .Add(column: value);
            var where = view.Where(context: context, ss: ss);
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: column,
                    join: ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            column,
                            where
                        }),
                    where: where))
                        .AsEnumerable()
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Long("IssueId"),
                            Title = o.String("ItemTitle"),
                            GroupX = groupByX.ConvertIfUserColumn(o),
                            GroupY = groupByY?.ConvertIfUserColumn(o),
                            Value = o.Decimal(value.ColumnName)
                        });
        }

        public static string UpdateByKamban(Context context, SiteSettings ss)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: context.Forms.Long("KambanId"),
                formData: context.Forms);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: issueModel);
            issueModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return KambanJson(context: context, ss: ss);
        }

        public static string ImageLib(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "ImageLib"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .ImageLib(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false));
        }

        public static string ImageLibJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "ImageLib"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("ImageLib");
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setImageLib",
                    bodyOnly: bodyOnly,
                    bodySelector: "#ImageLibBody",
                    body: new HtmlBuilder()
                        .ImageLib(
                            context: context,
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly))
                .ToJson();
        }

        private static HtmlBuilder ImageLib(
            this HtmlBuilder hb,
            Context context, 
            SiteSettings ss,
            View view,
            bool bodyOnly,
            int offset = 0)
        {
            return !bodyOnly
                ? hb.ImageLib(
                    ss: ss,
                    context: context,
                    imageLibData: new ImageLibData(
                        context: context,
                        ss: ss,
                        view: view,
                        offset: offset,
                        pageSize: ss.ImageLibPageSize.ToInt()))
                : hb.ImageLibBody(
                    ss: ss,
                    context: context,
                    imageLibData: new ImageLibData(
                        context: context,
                        ss: ss,
                        view: view,
                        offset: offset,
                        pageSize: ss.ImageLibPageSize.ToInt()));
        }

        public static string ImageLibNext(Context context, SiteSettings ss, int offset)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var imageLibData = new ImageLibData(
                context: context,
                ss: ss,
                view: view,
                offset: offset,
                pageSize: ss.ImageLibPageSize.ToInt());
            var hb = new HtmlBuilder();
            new ImageLibData(
                context: context,
                ss: ss,
                view: view,
                offset: offset,
                pageSize: Parameters.General.ImageLibPageSize)
                    .DataRows
                    .ForEach(dataRow => hb
                        .ImageLibItem(
                            context: context,
                            ss: ss,
                            dataRow: dataRow));
            return (new ResponseCollection())
                .Append("#ImageLib", hb)
                .Val("#ImageLibOffset", ss.ImageLibNextOffset(
                    offset,
                    imageLibData.DataRows.Count(),
                    imageLibData.TotalCount))
                .Paging("#ImageLib")
                .ToJson();
        }

        public static void SetLinks(
            this List<IssueModel> issues, Context context, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks(context: context);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    context: context,
                    columnName: link.ColumnName,
                    selectedValues: issues
                        .Select(o => o.PropertyValue(
                            context: context,
                            name: link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issues.ForEach(issueModel => issueModel
                    .SetTitle(context: context, ss: ss));
            }
        }

        private static bool InRange(Context context, SiteSettings ss, View view, int limit)
        {
            var where = view.Where(context: context, ss: ss);
            return Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            where
                        }),
                    where: where)) <= limit;
        }
    }
}
