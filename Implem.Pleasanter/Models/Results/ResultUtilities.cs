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
    public static class ResultUtilities
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
            var invalid = ResultValidators.OnEntry(
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
                referenceType: "Results",
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
                                value: "Results")
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
                    value: gridData.DataRows.Select(g => g.Long("ResultId")).ToJson())
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
            ResultModel resultModel = null;
            var originalId = context.Forms.Long("OriginalId");
            if (editRow && offset == 0)
            {
                if (newRowId != 0)
                {
                    resultModel = originalId > 0
                        ? new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: originalId)
                        : new ResultModel(
                            context: context,
                            ss: ss,
                            methodType: BaseModel.MethodTypes.New);
                    resultModel.ResultId = 0;
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: resultModel.Mine(context: context));
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
                    resultModel: resultModel,
                    editRow: editRow,
                    newRowId: newRowId,
                    offset: offset,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#NewRowId", newRowId, _using: newOnGrid)
                .CopyRowFormData(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    columns: columns,
                    newOnGrid: newOnGrid,
                    newRowId: newRowId,
                    originalId: originalId)
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.TotalCount))
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("ResultId")).ToJson())
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
            ResultModel resultModel = null,
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
                        resultModel: resultModel,
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
                        checkAll: checkAll,
                        editRow: editRow,
                        checkRow: checkRow));
        }

        private static HtmlBuilder GridNewRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
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
                        resultModel: resultModel,
                        newRowId: newRowId);
                }
                formDataSet
                    .Where(formData => formData.Id < 0)
                    .OrderBy(formData => formData.Id)
                    .ForEach(formData =>
                    {
                        resultModel = new ResultModel(
                            context: context,
                            ss: ss,
                            formData: formData.Data);
                        ss.SetColumnAccessControls(
                            context: context,
                            mine: resultModel.Mine(context: context));
                        hb.NewOnGrid(
                            context: context,
                            ss: ss,
                            columns: columns,
                            resultModel: resultModel,
                            newRowId: formData.Id);
                    });
            }
            return hb;
        }

        private static ResponseCollection CopyRowFormData(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            List<Column> columns,
            bool newOnGrid,
            int newRowId,
            long originalId)
        {
            if (newOnGrid && originalId > 0)
            {
                resultModel.SetCopyDefault(
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
                            resultModel.ControlValue(
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
                ? Rds.ResultsWhere().ResultId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long resultId)
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
                where: Rds.ResultsWhere().ResultId(resultId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: resultId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{resultId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + resultId)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("ResultId")}\"][data-latest]",
                        new HtmlBuilder().Tr(
                            context: context,
                            ss: ss,
                            dataRow: dataRow,
                            columns: ss.GetGridColumns(
                                context: context,
                                view: view,
                                checkPermission: true),
                            checkAll: false,
                            editRow: true,
                            checkRow: false,
                            idColumn: "ResultId"))
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
            ResultModel resultModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    resultModel: resultModel);
            }
            else
            {
                var mine = resultModel.Mine(context: context);
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
                                    value: resultModel.SiteId)
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
                                    value: resultModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ResultId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ResultId)
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
                                    value: resultModel.Ver)
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
                                    value: resultModel.Title)
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
                                    value: resultModel.Body)
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
                                    value: resultModel.TitleBody)
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
                                    value: resultModel.Status)
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
                                    value: resultModel.Manager)
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
                                    value: resultModel.Owner)
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
                                    value: resultModel.SiteTitle)
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
                                    value: resultModel.Comments)
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
                                    value: resultModel.Creator)
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
                                    value: resultModel.Updator)
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
                                    value: resultModel.CreatedTime)
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
                                            value: resultModel.Class(columnName: column.Name))
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
                                            value: resultModel.Num(columnName: column.Name))
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
                                            value: resultModel.Date(columnName: column.Name))
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
                                            value: resultModel.Description(columnName: column.Name))
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
                                            value: resultModel.Check(columnName: column.Name))
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
                                            value: resultModel.Attachments(columnName: column.Name))
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
            ResultModel resultModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = resultModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = resultModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "ResultId": value = resultModel.ResultId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = resultModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = resultModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = resultModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = resultModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = resultModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "Manager": value = resultModel.Manager.GridText(
                        context: context,
                        column: column); break;
                    case "Owner": value = resultModel.Owner.GridText(
                        context: context,
                        column: column); break;
                    case "SiteTitle": value = resultModel.SiteTitle.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = resultModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = resultModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = resultModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = resultModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = resultModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = resultModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = resultModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = resultModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = resultModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = resultModel.Attachments(columnName: column.Name).GridText(
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
                resultModel: new ResultModel(
                    context: context,
                    ss: ss,
                    methodType: BaseModel.MethodTypes.New,
                    formData: context.Forms));
        }

        public static string Editor(
            Context context, SiteSettings ss, long resultId, bool clearSessions)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            resultModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: ss,
                resultId: resultModel.ResultId,
                siteId: resultModel.SiteId);
            return Editor(
                context: context,
                ss: ss,
                resultModel: resultModel);
        }

        public static string Editor(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool editInDialog = false)
        {
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel);
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
                mine: resultModel.Mine(context: context));
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    ss: ss,
                    siteId: resultModel.SiteId,
                    referenceId: resultModel.ResultId,
                    isHistory: resultModel.VerType == Versions.VerTypes.History,
                    action: () => hb
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            editInDialog: editInDialog))
                                .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: resultModel.VerType,
                    methodType: resultModel.MethodType,
                    siteId: resultModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Results",
                    title: resultModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : resultModel.Title.DisplayValue,
                    useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: resultModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: resultModel.MethodType),
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            resultModel: resultModel)
                        .Hidden(controlId: "TriggerRelatingColumns", value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
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
                            id: resultModel.ResultId != 0 
                                ? resultModel.ResultId
                                : resultModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: resultModel,
                            tableName: "Results")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: resultModel.Comments,
                                    column: commentsColumn,
                                    verType: resultModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(
                                context: context,
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New
                                    && !context.Publish)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: context.CanManagePermission(ss: ss)
                                    && !ss.Locked()
                                    && resultModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                verType: resultModel.VerType,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
                                        ss: ss,
                                        resultModel: resultModel)))
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
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "always-send",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: resultModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Results",
                    referenceId: resultModel.ResultId,
                    referenceVer: resultModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    referenceType: "items",
                    id: resultModel.ResultId)
                .MoveDialog(context: context)
                .OutgoingMailDialog()
                .PermissionsDialog(context: context)
                .EditorExtensions(
                    context: context,
                    resultModel: resultModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(_using: context.CanManagePermission(ss: ss) &&
                        resultModel.MethodType != BaseModel.MethodTypes.New,
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
                                resultModel: new ResultModel(),
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
            ResultModel resultModel,
            bool editInDialog = false)
        {
            var mine = resultModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool preview = false,
            bool editInDialog = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: resultModel);
                if (!editInDialog)
                {
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkId: resultModel.ResultId,
                                methodType: resultModel.MethodType))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId));
                }
            }
            return hb;
        }

        public static void Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false)
        {
            var value = resultModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    methodType: resultModel.MethodType,
                    value: value,
                    columnPermissionType: column.ColumnPermissionType(context: context),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview);
            }
        }

        public static string ControlValue(
            this ResultModel resultModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "ResultId":
                    return resultModel.ResultId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return resultModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Title":
                    return resultModel.Title
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return resultModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Status":
                    return resultModel.Status
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Manager":
                    return resultModel.Manager
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Owner":
                    return resultModel.Owner
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return resultModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return resultModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return resultModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return resultModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return resultModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return resultModel.Attachments(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        default: return null;
                    }
            }
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long resultId)
        {
            return EditorResponse(context, ss, new ResultModel(
                context, ss, resultId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Message message = null,
            string switchTargets = null)
        {
            resultModel.MethodType = BaseModel.MethodTypes.Edit;
            var editInDialog = context.Forms.Bool("EditInDialog");
            return editInDialog
                ? new ResultsResponseCollection(resultModel)
                    .Response("id", resultModel.ResultId.ToString())
                    .Html("#EditInDialogBody", Editor(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        editInDialog: editInDialog))
                    .Invoke("openEditorDialog")
                    .Events("on_editor_load")
                : new ResultsResponseCollection(resultModel)
                    .Response("id", resultModel.ResultId.ToString())
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, resultModel))
                    .Val("#Id", resultModel.ResultId.ToString())
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .ClearFormData()
                    .Events("on_editor_load");
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long resultId, long siteId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Results_UpdatedTime(SqlOrderBy.Types.desc);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectResults(
                                column: Rds.ResultsColumn().ResultId(),
                                join: join,
                                where: where,
                                orderBy: orderBy))
                                    .AsEnumerable()
                                    .Select(o => o["ResultId"].ToLong())
                                    .ToList()
                        : new List<long>();
            if (!switchTargets.Contains(resultId))
            {
                switchTargets.Add(resultId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            string idSuffix = null)
        {
            var mine = resultModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "ResultId":
                            res.Val(
                                "#Results_ResultId" + idSuffix,
                                resultModel.ResultId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Results_Title" + idSuffix,
                                resultModel.Title.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Results_Body" + idSuffix,
                                resultModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Status":
                            res.Val(
                                "#Results_Status" + idSuffix,
                                resultModel.Status.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Results_Manager" + idSuffix,
                                resultModel.Manager.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Results_Owner" + idSuffix,
                                resultModel.Owner.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Results_{column.Name}{idSuffix}",
                                        resultModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Results_{column.Name}{idSuffix}",
                                        resultModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Results_{column.Name}{idSuffix}",
                                        resultModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Results_{column.Name}{idSuffix}",
                                        resultModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Results_{column.Name}{idSuffix}",
                                        resultModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Results_{column.Name}Field",
                                        new HtmlBuilder()
                                            .FieldAttachments(
                                                context: context,
                                                fieldId: $"Results_{column.Name}Field",
                                                controlId: $"Results_{column.Name}",
                                                columnName: column.ColumnName,
                                                fieldCss: column.FieldCss,
                                                fieldDescription: column.Description,
                                                controlCss: column.ControlCss,
                                                labelText: column.LabelText,
                                                value: resultModel.Attachments(columnName: column.Name).ToJson(),
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
            ResultModel resultModel,
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
                                    resultModel: resultModel,
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
                                resultModel: resultModel);
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
            var invalid = ResultValidators.OnEntry(
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
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    "[Items]",
                    SqlJoin.JoinTypes.Inner,
                    "[Results].[ResultId]=[Items].[ReferenceId]")),
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
                    resultCollection.TotalCount,
                    Data = resultCollection.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(
            Context context, SiteSettings ss, long resultId, bool internalRequest)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel,
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
                mine: resultModel.Mine(context: context));
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = resultModel.GetByApi(
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
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                formData: context.Forms);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = resultModel.Create(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: resultModel.Title.DisplayValue));
                    return new ResponseCollection()
                        .Response("id", resultModel.ResultId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : resultModel.ResultId))
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
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                setByApi: true);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(context: context, ss: ss);
            var errorData = resultModel.Create(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Created(
                            context: context,
                            data: resultModel.Title.DisplayValue));
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

        public static string Update(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                formData: context.Forms);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                permissions: context.Forms.List("CurrentPermissionsAll"),
                permissionChanged: context.Forms.Exists("CurrentPermissionsAll"));
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new ResultsResponseCollection(resultModel);
                    return ResponseByUpdate(res, context, ss, resultModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: resultModel.Comments,
                            verType: resultModel.VerType)
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
                        data: resultModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            ResultsResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
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
                    where: Rds.ResultsWhere().ResultId(resultModel.ResultId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{resultModel.ResultId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            dataRows: gridData.DataRows,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: resultModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, resultModel: resultModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", resultModel.Title.DisplayValue)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: resultModel,
                        tableName: "Results"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: resultModel.ResultId))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: resultModel.Title.DisplayValue))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: resultModel.Comments,
                        deleteCommentId: resultModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static string BulkUpdate(Context context, SiteSettings ss)
        {
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss)
                    .Where(o => !o.Suffix.IsNullOrEmpty())
                    .Where(o => o.SiteId == ss.SiteId)
                    .ToList();
            var statements = new List<SqlStatement>();
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .ResultId_In(formDataSet
                        .Where(formData => formData.Id > 0)
                        .Select(formData => formData.Id)),
                formDataSet: formDataSet);
            var notificationHash = resultCollection.ToDictionary(
                o => o.ResultId,
                o => o.GetNotifications(
                    context: context,
                    ss: ss,
                    notice: true,
                    before: true));
            foreach (var formData in formDataSet)
            {
                var resultModel = resultCollection
                    .FirstOrDefault(o => o.ResultId == formData.Id);
                if (resultModel != null)
                {
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: resultModel.Mine(context: context));
                    var invalid = ResultValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.Type.MessageJson(context: context);
                    }
                    resultModel.VerUp = Versions.MustVerUp(
                        context: context,
                        baseModel: resultModel);
                    statements.AddRange(resultModel.UpdateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                }
                else if (formData.Id < 0)
                {
                    resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        formData: formData.Data);
                    ss.SetColumnAccessControls(
                        context: context,
                        mine: resultModel.Mine(context: context));
                    var invalid = ResultValidators.OnCreating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.Type.MessageJson(context: context);
                    }
                    statements.AddRange(resultModel.CreateStatements(
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
                    return BulkUpdateDuplicated(
                        context: context,
                        ss: ss,
                        response: response);
                case "Conflicted":
                    return BulkUpdateConflicted(
                        context: context,
                        ss: ss,
                        response: response);
                default:
                    return BulkUpdatedSuccess(
                        context: context,
                        ss: ss,
                        formDataSet: formDataSet,
                        resultCollection: resultCollection,
                        responses: responses,
                        notificationHash: notificationHash);
            }
        }

        private static string BulkUpdateDuplicated(
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

        private static string BulkUpdateConflicted(
            Context context, SiteSettings ss, SqlResponse response)
        {
            var target = "row_" + response.Id;
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: response.Id.ToLong());
            return resultModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(
                    context: context,
                    target: target,
                    data: resultModel.Updator.Name)
                        .ToJson()
                : Messages.ResponseNotFound(
                    context: context,
                    target: target)
                        .ToJson();
        }

        private static string BulkUpdatedSuccess(
            Context context,
            SiteSettings ss,
            List<FormData> formDataSet,
            ResultCollection resultCollection,
            List<SqlResponse> responses,
            Dictionary<long, List<Notification>> notificationHash)
        {
            responses
                .Where(o => o.DataTableName.ToInt() < 0)
                .ForEach(response =>
                    resultCollection.Add(new ResultModel(
                        context: context,
                        ss: ss,
                        resultId: response.Id.ToLong())));
            resultCollection.ForEach(resultModel =>
            {
                resultModel.SynchronizeSummary(
                    context: context,
                    ss: ss);
                var response = responses.FirstOrDefault(o => o.Id == resultModel.ResultId);
                if (response?.DataTableName.ToInt() < 0)
                {
                    resultModel.Notice(
                        context: context,
                        ss: ss,
                        notifications: resultModel.GetNotifications(
                            context: context,
                            ss: ss,
                            notice: true),
                        type: "Created");
                }
                else
                {
                    resultModel.Notice(
                        context: context,
                        ss: ss,
                        notifications: NotificationUtilities.MeetConditions(
                            ss: ss,
                            before: notificationHash.Get(resultModel.ResultId),
                            after: resultModel.GetNotifications(
                                context: context,
                                ss: ss,
                                notice: true)),
                        type: "Updated");
                }
            });
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new ResultCollection(
                    context: context,
                    ss: ss,
                    column: Rds.ResultsEditorColumns(ss: ss),
                    where: Rds.ResultsWhere()
                        .SiteId(ss.SiteId)
                        .ResultId_In(responses.Select(o => o.Id.ToLong())))
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
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(responses.Select(o => o.Id.ToLong())));
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            gridData.DataRows.ForEach(dataRow =>
                res.ReplaceAll(
                    $"[data-id=\"{responses.FirstOrDefault(o => o.Id == dataRow.Long("ResultId"))?.DataTableName}\"][data-latest]",
                    new HtmlBuilder().Tr(
                        context: context,
                        ss: ss,
                        dataRow: dataRow,
                        columns: columns,
                        checkAll: false,
                        editRow: true,
                        checkRow: false,
                        idColumn: "ResultId")));
            formDataSet.ForEach(formData =>
                formData.Data.Keys.ForEach(controlId =>
                    res.ClearFormData(controlId + formData.Suffix)));
            return res
                .SetMemory("formChanged", false)
                .Message(Messages.BulkUpdated(
                    context: context,
                    data: responses.Count().ToString()))
                .ToJson();
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                setByApi: true);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(
                context: context,
                ss: ss);
            resultModel.VerUp = Versions.MustVerUp(
                context: context,
                baseModel: resultModel);
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Updated(
                            context: context,
                            data: resultModel.Title.DisplayValue));
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

        public static string Copy(Context context, SiteSettings ss, long resultId)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                formData: context.Forms);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            resultModel.ResultId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                resultModel.Title.Value += Displays.SuffixCopy(context: context);
            }
            if (!context.Forms.Bool("CopyWithComments"))
            {
                resultModel.Comments.Clear();
            }
            resultModel.SetCopyDefault(
                context: context,
                ss: ss);
            var errorData = resultModel.Create(
                context, ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (ss.SwitchRecordWithAjax == true)
                    {
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            message: Messages.Copied(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                resultId: resultModel.ResultId,
                                siteId: resultModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Copied(context: context));
                        return new ResponseCollection()
                            .Response("id", resultModel.ResultId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: resultModel.ResultId))
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

        public static string Move(Context context, SiteSettings ss, long resultId)
        {
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(context: context, siteId: siteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId);
            var invalid = ResultValidators.OnMoving(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: resultId));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var targetSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            var errorData = resultModel.Move(
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
                            resultModel: resultModel,
                            message: Messages.Moved(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                resultId: resultModel.ResultId,
                                siteId: resultModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Moved(
                                context: context,
                                data: resultModel.Title.DisplayValue));
                        return new ResponseCollection()
                            .Response("id", resultModel.ResultId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: resultModel.ResultId))
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

        public static string Delete(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(context, ss, resultId);
            var invalid = ResultValidators.OnDeleting(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = resultModel.Delete(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: resultModel.Title.Value));
                    var res = new ResultsResponseCollection(resultModel);
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
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnDeleting(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(context: context, ss: ss);
            var errorData = resultModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Deleted(
                            context: context,
                            data: resultModel.Title.DisplayValue));
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
            var where = Rds.ResultsWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Results_Deleted")
                .ResultId_In(
                    value: selected,
                    tableName: "Results_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .ResultId_In(
                    tableName: "Results_Deleted",
                    sub: Rds.SelectResults(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.ResultsColumn().ResultId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(context: context, ss: ss)));
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(where: Rds.ItemsWhere().ReferenceId_In(sub:
                        Rds.SelectResults(
                            tableType: Sqls.TableTypes.Deleted,
                            _as: "Results_Deleted",
                            column: Rds.ResultsColumn()
                                .ResultId(tableName: "Results_Deleted"),
                            where: where))),
                    Rds.RestoreResults(where: where),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long resultId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var resultModel = new ResultModel(context, ss, resultId);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
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
            resultModel.SetByModel(new ResultModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId(resultId)
                    .Ver(ver.First())));
            resultModel.VerUp = true;
            var errorData = resultModel.Update(
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
                    return  new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: resultId))
                        .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long resultId, Message message = null)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
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
                                resultModel: resultModel)));
            return new ResultsResponseCollection(resultModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            ResultModel resultModel)
        {
            new ResultCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.ResultsWhere().ResultId(resultModel.ResultId),
                orderBy: Rds.ResultsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(resultModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(resultModelHistory.Ver)
                                .DataLatest(1, _using:
                                    resultModelHistory.Ver == resultModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: resultModelHistory.Ver.ToString(),
                                            _using: resultModelHistory.Ver < resultModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            resultModel: resultModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.ResultsColumnCollection()
                .ResultId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.ResultsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
            resultModel.Get(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .ResultId(resultModel.ResultId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            resultModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, resultModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        resultId.ToString() 
                            + (resultModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
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
                                where: where));
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
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
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
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: Rds.ResultsWhere().SiteId(siteId)))
                            .SiteId(siteId, _operator: "<>"),
                        param: Rds.ItemsParam().SiteId(siteId)),
                    Rds.UpdateResults(
                        where: where,
                        param: Rds.ResultsParam().SiteId(siteId)),
                    Rds.RowCount()
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
                                where: where));
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
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                join: ss.Join(
                    context: context,
                    join: where),
                where: where);
            var statements = new List<SqlStatement>();
            statements.OnBulkDeletingExtendedSqls(ss.SiteId);
            statements.Add(Rds.DeleteItems(
                where: Rds.ItemsWhere()
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteBinaries(
                where: Rds.BinariesWhere()
                    .TenantId(context.TenantId)
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteResults(where: where));
            statements.Add(Rds.RowCount());
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long resultId)
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
                        resultId: resultId,
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
                            resultId: resultId,
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
                    resultId: resultId,
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
            long resultId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteResults(
                        tableType: Sqls.TableTypes.History,
                        where: Rds.ResultsWhere()
                            .SiteId(
                                value: ss.SiteId,
                                tableName: "Results_History")
                            .ResultId(
                                value: resultId,
                                tableName: "Results_History")
                            .Ver_In(
                                value: selected,
                                tableName: "Results_History",
                                negative: negative,
                                _using: selected.Any())
                            .ResultId_In(
                                tableName: "Results_History",
                                sub: Rds.SelectResults(
                                    tableType: Sqls.TableTypes.History,
                                    column: Rds.ResultsColumn().ResultId(),
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
            var where = Rds.ResultsWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Results_Deleted")
                .ResultId_In(
                    value: selected,
                    tableName: "Results_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .ResultId_In(
                    tableName: "Results_Deleted",
                    sub: Rds.SelectResults(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.ResultsColumn().ResultId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(
                            context: context, ss: ss)));
            var sub = Rds.SelectResults(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Results_Deleted",
                column: Rds.ResultsColumn()
                    .ResultId(tableName: "Results_Deleted"),
                where: where);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteResults(
                        tableType: Sqls.TableTypes.Deleted,
                        where: where),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string Import(Context context, SiteModel siteModel)
        {
            var ss = siteModel.ResultsSiteSettings(
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
                    if (column?.ColumnName == "ResultId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalid = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.ColumnName));
                if (invalid != null) return invalid;
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var resultHash = new Dictionary<int, ResultModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var resultModel = new ResultModel() { SiteId = ss.SiteId };
                    if (context.Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
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
                                resultModel.Title.Value = recordingData.ToString();
                                break;
                            case "Body":
                                resultModel.Body = recordingData.ToString();
                                break;
                            case "Status":
                                resultModel.Status.Value = recordingData.ToInt();
                                break;
                            case "Manager":
                                resultModel.Manager = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Owner":
                                resultModel.Owner = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Comments":
                                if (resultModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    resultModel.Comments.Prepend(
                                        context: context,
                                        ss: ss,
                                        body: data.Row[column.Key]);
                                }
                                break;
                            default:
                                resultModel.Value(
                                    context: context,
                                    columnName: column.Value.ColumnName,
                                    value: recordingData);
                                break;
                        }
                    });
                    resultHash.Add(data.Index, resultModel);
                });
                var insertCount = 0;
                var updateCount = 0;
                foreach (var resultModel in resultHash.Values)
                {
                    resultModel.SetByFormula(context: context, ss: ss);
                    resultModel.SetTitle(context: context, ss: ss);
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        resultModel.VerUp = Versions.MustVerUp(
                            context: context, baseModel: resultModel);
                        if (resultModel.Updated(context: context))
                        {
                            var errorData = resultModel.Update(
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
                        var errorData = resultModel.Create(
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

        private static string ImportRecordingData(
            Context context, Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(
                context: context,
                value: value,
                siteId: inheritPermission);
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
            var invalid = ResultValidators.OnExporting(
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
            var invalid = ResultValidators.OnExporting(
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

        public static System.Web.Mvc.ContentResult ExportByApi(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = ResultValidators.OnExporting(
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
            var invalid = ResultValidators.OnExporting(
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
                value = ss.GetColumn(context: context, columnName: "ResultId");
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
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: context.Forms.Long("Id"),
                formData: context.Forms);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: resultModel);
            resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return CalendarJson(
                context: context,
                ss: ss,
                changedItemId: resultModel.ResultId,
                update: true,
                message: Messages.Updated(
                    context: context,
                    data: resultModel.Title.DisplayValue));
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
                    tableName: "Results",
                    raw: $"[Results].[{fromColumn.ColumnName}] between '{begin}' and '{end}'");
            }
            else
            {
                where.Or(or: Rds.ResultsWhere()
                    .Add(raw: $"[Results].[{fromColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Results].[{toColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Results].[{fromColumn.ColumnName}]<='{begin}' and [Results].[{toColumn.ColumnName}]>='{end}'"));
            }
            where = view.Where(context: context, ss: ss, where: where);
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsTitleColumn(
                        context: context,
                        ss: ss)
                            .ResultId(_as: "Id")
                            .ResultsColumn(
                                columnName: fromColumn.ColumnName,
                                _as: "From")
                            .ResultsColumn(
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
                value = ss.GetColumn(context: context, columnName: "ResultId");
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
                value = ss.GetColumn(context: context, columnName: "ResultId");
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
                var column = Rds.ResultsColumn()
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
                var groupBy = Rds.ResultsGroupBy()
                    .WithItemTitle(
                        ss: ss,
                        column: groupByX)
                    .WithItemTitle(
                        ss: ss,
                        column: groupByY);
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
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
                var column = Rds.ResultsColumn()
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
                var groupBy = Rds.ResultsGroupBy()
                    .Add(dateGroup)
                    .WithItemTitle(
                        ss: ss,
                        column: groupByY);
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
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
                var column = Rds.ResultsColumn()
                    .ResultId(_as: "Id")
                    .Ver()
                    .UpdatedTime()
                    .Add(column: groupBy)
                    .Add(column: value);
                var where = view.Where(context: context, ss: ss);
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
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
            var column = Rds.ResultsColumn()
                .ResultId()
                .ItemTitle(ss.ReferenceType)
                .Add(column: groupByX)
                .Add(column: groupByY)
                .Add(column: value);
            var where = view.Where(context: context, ss: ss);
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
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
                            Id = o.Long("ResultId"),
                            Title = o.String("ItemTitle"),
                            GroupX = groupByX.ConvertIfUserColumn(o),
                            GroupY = groupByY?.ConvertIfUserColumn(o),
                            Value = o.Decimal(value.ColumnName)
                        });
        }

        public static string UpdateByKamban(Context context, SiteSettings ss)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: context.Forms.Long("KambanId"),
                formData: context.Forms);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: resultModel);
            resultModel.Update(
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
            this List<ResultModel> results, Context context, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks(context: context);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    context: context,
                    columnName: link.ColumnName,
                    selectedValues: results
                        .Select(o => o.PropertyValue(
                            context: context,
                            name: link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel => resultModel
                    .SetTitle(context: context, ss: ss));
            }
        }

        private static bool InRange(Context context, SiteSettings ss, View view, int limit)
        {
            var where = view.Where(context: context, ss: ss);
            return Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
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
