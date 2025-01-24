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
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class ResultUtilities
    {
        public static string Index(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = ss.DashboardParts.Any()
                ? ss.DashboardParts.FirstOrDefault().View
                : Views.GetBySession(
                    context: context,
                    ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view,
                gridData: gridData);
            var suffix = ss.DashboardParts.Any()
                ? Views.GetBySession(
                    context: context,
                    ss: ss)
                    .GetIndexSuffix()
                : "";
            if (ss.DashboardParts?.Any() != true)
            {
                return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
                viewModeBody: () => hb.Grid(
                    context: context,
                    gridData: gridData,
                    ss: ss,
                    view: view,
                    serverScriptModelRow: serverScriptModelRow));
            }
            else
            {
                return hb.Grid(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    serverScriptModelRow: serverScriptModelRow,
                    suffix: suffix).ToString();
            }
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string viewMode,
            ServerScriptModelRow serverScriptModelRow,
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
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Results",
                useTitle: false,
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                serverScriptModelRow: serverScriptModelRow,
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
                                action: () => hb
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
                            .ViewExtensions(
                                context: context,
                                ss: ss,
                                view: view)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                view: view,
                                verType: Versions.VerTypes.Latest,
                                backButton: !context.Publish,
                                serverScriptModelRow: serverScriptModelRow)
                            .Div(css: "margin-bottom")
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
                    .ImportSettingsDialog(
                        context: context,
                        ss: ss)
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
                        .Id("ExportSitePackageDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSitePackage(context: context)))
                    .Div(attributes: new HtmlAttributes()
                        .Id("BulkUpdateSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.BulkUpdate(context: context)))
                    .Div(attributes: new HtmlAttributes()
                        .Id("AnalyPartDialog")
                        .Class("dialog")
                        .Title(Displays.AnalyPart(context: context))))
                    .ToString();
        }

        public static string IndexJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view);
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view,
                gridData: gridData);
            var body = new HtmlBuilder().Grid(
                context: context,
                ss: ss,
                gridData: gridData,
                view: view,
                serverScriptModelRow: serverScriptModelRow);
            return new ResponseCollection(context: context)
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setGrid",
                    editOnGrid: context.Forms.Bool("EditOnGrid"),
                    serverScriptModelRow: serverScriptModelRow,
                    body: body)
                .Events("on_grid_load")
                .ToJson();
        }

        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
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
            string action = "GridRows",
            ServerScriptModelRow serverScriptModelRow = null,
            string suffix = "")
        {
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .GridTable(
                    attributes: new HtmlAttributes()
                        .Id($"Grid{suffix}")
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
                            serverScriptModelRow: serverScriptModelRow,
                            action: action,
                            suffix: suffix))
                .GridHeaderMenus(
                    context: context,
                    ss: ss,
                    view: view,
                    columns: columns,
                    suffix: suffix)
                .Hidden(
                    controlId: $"GridOffset{suffix}",
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
            int offset = 0,
            bool windowScrollTop = false,
            bool clearCheck = false,
            string action = "GridRows",
            Message message = null,
            string suffix = "")
        {
            var view = ss.DashboardParts.Any()
                ? ss.DashboardParts.FirstOrDefault().View
                : Views.GetBySession(
                    context: context,
                    ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view,
                offset: offset);
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view,
                gridData: gridData);
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
                    if (originalId > 0)
                    {
                        resultModel = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: originalId);
                        resultModel.SetCopyDefault(
                            context: context,
                            ss: ss);
                    }
                    else
                    {
                        resultModel = new ResultModel(
                            context: context,
                            ss: ss,
                            methodType: BaseModel.MethodTypes.New);
                    }
                    resultModel.ResultId = 0;
                    resultModel.SetByBeforeOpeningRowServerScript(
                        context: context,
                        ss: ss,
                        view: view);
                }
            }
            if (suffix.IsNullOrEmpty())
            {
                return new ResponseCollection(context: context, logFlush: false)
                    .WindowScrollTop(_using: windowScrollTop)
                    .Remove(".grid tr", _using: offset == 0)
                    .ClearFormData("GridOffset")
                    .ClearFormData("GridCheckAll", _using: clearCheck)
                    .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                    .ClearFormData("GridCheckedItems", _using: clearCheck)
                    .ClearFormData("OriginalId", _using: newOnGrid)
                    .CloseDialog(_using: offset == 0)
                    .ReplaceAll("#CopyToClipboards", new HtmlBuilder()
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
                    .Messages(context.Messages)
                    .Log(context.GetLog())
                    .ToJson();
            }
            else
            {
                return new ResponseCollection(context: context, logFlush: false)
                    .ClearFormData("GridOffset")
                    .Append($"#Grid{suffix}", new HtmlBuilder().GridRows(
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
                    .Val($"#GridOffset{suffix}", ss.GridNextOffset(
                        offset,
                        gridData.DataRows.Count(),
                        gridData.TotalCount))
                    .Log(context.GetLog())
                    .ToJson();
            }
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
            string action = "GridRows",
            ServerScriptModelRow serverScriptModelRow = null,
            string suffix = "")
        {
            var checkRow = ss.DashboardParts.Any()
                ? false
                : ss.CheckRow(
                    context: context,
                    gridColumns: view.GridColumns);
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss);
            return hb
                .THead(
                    css: ss.DashboardParts.Any()
                        ? "dashboard-grid-header"
                        : "",
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
                            action: action,
                            serverScriptModelRow: serverScriptModelRow,
                            suffix: suffix))
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
                        view: view,
                        dataRows: gridData.DataRows,
                        columns: columns,
                        formDataSet: formDataSet,
                        editRow: editRow,
                        checkRow: checkRow,
                        clearCheck: clearCheck));
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
                ss.GetEditorColumnNames(
                    context: context,
                    columnOnly: true)
                        .Select(columnName => ss.GetColumn(
                            context: context,
                            columnName: columnName))
                        .Where(column => column.CanUpdate(
                            context: context,
                            ss: ss,
                            mine: resultModel.Mine(context: context)))
                        .Where(column => !column.Id_Ver)
                        .Where(column => !columns.Any(p =>
                            p.ColumnName == column.ColumnName))
                        .ForEach(column =>
                        {
                            var value = resultModel.ControlValue(
                                context: context,
                                ss: ss,
                                column: column);
                            if(value != null)
                            {
                                //数値項目の場合、「単位」を値に連結する
                                value += resultModel.NumUnit(
                                    context: context,
                                    ss: ss,
                                    column: column);
                            }
                            res.SetFormData(
                                $"{ss.ReferenceType}_{column.ColumnName}_{ss.SiteId}_{newRowId}",
                                value);
                        });
            }
            return res;
        }

        private static SqlWhereCollection SelectedWhere(
            Context context,
            SiteSettings ss)
        {
            var selector = new RecordSelector(context: context);
            return !selector.Nothing
                ? Rds.ResultsWhere().ResultId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.ResultsWhere().ResultId_In(
                    value: recordSelector.Selected?.Select(o => o.ToLong()) ?? new List<long>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long resultId)
        {
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
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("ResultId")}\"][data-latest]",
                        new HtmlBuilder().Tr(
                            context: context,
                            ss: ss,
                            view: view,
                            dataRow: dataRow,
                            columns: ss.GetGridColumns(
                                context: context,
                                view: view,
                                checkPermission: true),
                            recordSelector: null,
                            editRow: true,
                            checkRow: false,
                            idColumn: "ResultId"))
                    .Messages(context.Messages)
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view,
                gridData: gridData);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
                viewModeBody: () => hb
                    .TrashBoxCommands(context: context, ss: ss)
                    .Grid(
                        context: context,
                        ss: ss,
                        gridData: gridData,
                        view: view,
                        action: "TrashBoxGridRows",
                        serverScriptModelRow: serverScriptModelRow));
        }

        public static string TrashBoxJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var gridData = GetGridData(
                context: context,
                ss: ss,
                view: view);
            var body = new HtmlBuilder()
                .TrashBoxCommands(context: context, ss: ss)
                .Grid(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    action: "TrashBoxGridRows");
            return new ResponseCollection(context: context)
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setGrid",
                    body: body)
                .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            ResultModel resultModel,
            int? tabIndex = null,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            if (serverScriptModelColumn?.HideChanged == true && serverScriptModelColumn?.Hide == true)
            {
                return hb.Td();
            }
            if (serverScriptModelColumn?.RawText.IsNullOrEmpty() == false)
            {
                return hb.Td(
                    context: context,
                    column: column,
                    action: () => hb.Raw(serverScriptModelColumn?.RawText),
                    tabIndex: tabIndex,
                    serverScriptModelColumn: serverScriptModelColumn);
            }
            else if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
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
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.SiteId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.UpdatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ResultId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ResultId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Title,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Body,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.TitleBody,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Status":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Status,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Manager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Manager,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Owner":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Owner,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Locked":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Locked,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "SiteTitle":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.SiteTitle,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Comments,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Creator,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Updator,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CreatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetClass(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Num":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetNum(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Date":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetDate(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Description":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetDescription(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Check":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetCheck(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
                            case "Attachments":
                                return ss.ReadColumnAccessControls.Allowed(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    mine: mine)
                                        ? hb.Td(
                                            context: context,
                                            column: column,
                                            value: resultModel.GetAttachments(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptModelColumn: serverScriptModelColumn);
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
            string css,
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
                    case "Locked": value = resultModel.Locked.GridText(
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
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = resultModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = resultModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = resultModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = resultModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = resultModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = resultModel.GetAttachments(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                        }
                        break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(
                css: css,
                action: () => hb
                    .Div(
                        css: "markup",
                        action: () => hb
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
            ResultModel resultModel = null;
            var copyFrom = context.QueryStrings.Long("CopyFrom");
            if (ss.AllowReferenceCopy == true && copyFrom > 0)
            {
                resultModel = new ResultModel(
                    context: context,
                    ss: ss,
                    resultId: copyFrom,
                    methodType: BaseModel.MethodTypes.New);
                if (resultModel.AccessStatus == Databases.AccessStatuses.Selected
                    && Permissions.CanRead(
                        context: context,
                        siteId: ss.SiteId,
                        id: resultModel.ResultId))
                {
                    resultModel = resultModel.CopyAndInit(
                        context: context,
                        ss: ss);
                }
                else
                {
                    return HtmlTemplates.Error(
                       context: context,
                       errorData: new ErrorData(type: Error.Types.NotFound));
                }
            }
            return Editor(
                context: context,
                ss: ss,
                resultModel: resultModel ?? new ResultModel(
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                itemModel: resultModel);
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    ss: ss,
                    siteId: resultModel.SiteId,
                    referenceId: resultModel.ResultId,
                    isHistory: resultModel.VerType == Versions.VerTypes.History,
                    action: () => hb.EditorInDialog(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        editInDialog: editInDialog))
                            .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: Views.GetBySession(
                        context: context,
                        ss: ss),
                    siteId: resultModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Results",
                    title: resultModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : resultModel.Title.MessageDisplay(context: context),
                    body: resultModel.Body,
                    useTitle: ss.TitleColumns?.Any(o => ss
                        .GetEditorColumnNames()
                        .Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: resultModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: resultModel.MethodType),
                    methodType: resultModel.MethodType,
                    serverScriptModelRow: serverScriptModelRow,
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            serverScriptModelRow: serverScriptModelRow)
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder EditorInDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool editInDialog)
        {
            if (ss.Tabs?.Any() != true)
            {
                hb.FieldSetGeneral(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    editInDialog: editInDialog);
            }
            else
            {
                hb.Div(
                    id: "EditorTabsContainer",
                    css: "tab-container max",
                    attributes: new HtmlAttributes().TabActive(context: context),
                    action: () => hb
                        .EditorTabs(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            editInDialog: editInDialog)
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            editInDialog: editInDialog)
                        .FieldSetTabs(
                            context: context,
                            ss: ss,
                            id: resultModel.ResultId,
                            resultModel: resultModel,
                            editInDialog: editInDialog));
            }
            return hb.Hidden(
                controlId: "EditorInDialogRecordId",
                value: context.Id.ToString());
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            ServerScriptModelRow serverScriptModelRow)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType =  Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: resultModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            var linksDataSet = HtmlLinks.DataSet(
                context: context,
                ss: ss,
                id: resultModel.ResultId);
            var links = HtmlLinkCreations.Links(
                context: context,
                ss: ss);
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
                                    columnPermissionType: commentsColumnPermissionType,
                                    serverScriptModelColumn: resultModel
                                        ?.ServerScriptModelRow
                                        ?.Columns.Get(commentsColumn.ColumnName)),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            attributes: new HtmlAttributes().TabActive(context: context),
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    ss: ss,
                                    resultModel: resultModel)
                                .FieldSetGeneral(
                                    context: context,
                                    ss: ss,
                                    resultModel: resultModel,
                                    dataSet: linksDataSet,
                                    links: links)
                                .FieldSetTabs(
                                    context: context,
                                    ss: ss,
                                    id: resultModel.ResultId,
                                    resultModel: resultModel,
                                    dataSet: linksDataSet,
                                    links: links)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: resultModel.MethodType != BaseModel.MethodTypes.New
                                        && !context.Publish)
                                .TabsPanelField(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetRecordAccessControl")
                                        .DataAction("Permissions")
                                        .DataMethod("post"),
                                    innerId: "FieldSetRecordAccessControlEditor",
                                    _using: context.CanManagePermission(ss: ss)
                                        && !ss.Locked()
                                        && resultModel.MethodType != BaseModel.MethodTypes.New)
                                .EditorMainCommands(
                                    context: context,
                                    ss: ss,
                                    resultModel: resultModel,
                                    serverScriptModelRow: serverScriptModelRow)
)
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: resultModel.Ver.ToString())
                        .Hidden(
                            controlId: "LockedTable",
                            value: ss.LockedTable()
                                ? "1"
                                : "0")
                        .Hidden(
                            controlId: "LockedRecord",
                            value: ss.LockedRecord()
                                ? "1"
                                : "0")
                        .Hidden(
                            controlId: "FromSiteId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("FromSiteId"),
                            _using: context.QueryStrings.Long("FromSiteId") > 0)
                        .Hidden(
                            controlId: "CopyFrom",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Long("CopyFrom").ToString(),
                            _using: context.QueryStrings.Long("CopyFrom") > 0)
                        .Hidden(
                            controlId: "LinkId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("LinkId"),
                            _using: context.QueryStrings.Long("LinkId") > 0)
                        .Hidden(
                            controlId: "FromTabIndex",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("FromTabIndex"),
                            _using: context.QueryStrings.Long("FromTabIndex") > 0)
                        .Hidden(
                            controlId: "ControlledOrder",
                            css: "control-hidden always-send",
                            value: string.Empty)
                        .Hidden(
                            controlId: "MethodType",
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "always-send",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "IsNew",
                            css: "always-send",
                            value: (context.Action == "new") ? "1" : "0")
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: resultModel.SwitchTargets?.Join(),
                            _using: !context.Ajax)
                        .Hidden(
                            controlId: "TriggerRelatingColumns_Editor", 
                            value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(
                            controlId: "NotReturnParentRecord",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("NotReturnParentRecord"))
                        .PostInitHiddenData(context: context))
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
                    ss: ss)
                .MoveDialog(context: context)
                .OutgoingMailDialog()
                .PermissionsDialog(context: context)
                .EditorExtensions(
                    context: context,
                    resultModel: resultModel,
                    ss: ss));
        }

        private static HtmlBuilder PostInitHiddenData(
            this HtmlBuilder hb,
            Context context)
        {
            var postInitData = context.Forms.Where(o => o.Key.StartsWith("PostInit_"));
            postInitData.ForEach(data =>
            {
                hb.Hidden(
                    controlId: data.Key,
                    value: data.Value,
                    css: "always-send");
            });
            return hb;
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool editInDialog = false)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: ss.GeneralTabLabelText))
                .Tabs(
                    context: context,
                    ss: ss)
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish
                        && !editInDialog,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(
                    _using: context.CanManagePermission(ss: ss)
                        && !ss.Locked()
                        && resultModel.MethodType != BaseModel.MethodTypes.New
                        && !editInDialog
                        && ss.ReferenceType != "Wikis",
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
                    .TabsPanelField(
                        id: name + "Editor",
                        action: () => hb
                            .FieldSetGeneralColumns(
                                context: context,
                                ss: ss,
                                resultModel: new ResultModel(),
                                preview: true))
                    .TabsPanelField(
                        id: name + "Grid",
                        action: () => hb
                            .GridTable(action: () => hb
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
            DataSet dataSet = null,
            List<Link> links = null,
            bool editInDialog = false)
        {
            var mine = resultModel.Mine(context: context);
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb.FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    dataSet: dataSet,
                    links: links,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            hb.Fields(
                context: context,
                ss: ss,
                id: resultModel.ResultId,
                resultModel: resultModel,
                dataSet: dataSet,
                links: links,
                preview: preview,
                editInDialog: editInDialog);
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: resultModel);
                if (!editInDialog
                    && ss
                        .EditorColumnHash
                        ?.SelectMany(tab => tab.Value ?? Enumerable.Empty<string>())
                        .Any(columnName => ss.LinkId(columnName) != 0) == false)
                {
                    hb.Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            context: context,
                            ss: ss,
                            linkId: resultModel.ResultId,
                            methodType: resultModel.MethodType,
                            links: links));
                    if (ss.HideLink != true)
                    {
                        hb.Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId,
                                dataSet: dataSet));
                    }
                }
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            bool disableAutoPostBack = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = resultModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            var rawValue = resultModel.ControlRawValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                //数値項目の場合、「単位」を値に連結する
                value += resultModel.NumUnit(
                    context: context,
                    ss: ss,
                    column: column);
                SetChoiceHashByFilterExpressions(
                    context: context,
                    ss: ss,
                    column: column,
                    resultModel: resultModel);
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    serverScriptModelColumn: resultModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName),
                    value: value,
                    rawValue: rawValue,
                    controlConstraintsType: resultModel.GetStatusControl(
                        context: context,
                        ss: ss,
                        column: column),
                    columnPermissionType: Permissions.ColumnPermissionType(
                        context: context,
                        ss: ss,
                        column: column,
                        baseModel: resultModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    disableAutoPostBack: disableAutoPostBack,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        private static HtmlBuilder Tabs(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            ss.Tabs?.ForEach(tab => hb.Li(action: () => hb.A(
                href: $"#FieldSetTab{tab.Id}",
                action: () => hb.Label(action: () => hb.Text(tab.LabelText)))));
            return hb;
        }

        private static HtmlBuilder FieldSetTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            ResultModel resultModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            dataSet = dataSet ?? HtmlLinks.DataSet(
                context: context,
                ss: ss,
                id: id);
            links = links ?? HtmlLinkCreations.Links(
                context: context,
                ss: ss);
            ss.Tabs?.Select((tab, index) => new { tab = tab, index = index + 1 })?.ForEach(data =>
            {
                hb.TabsPanelField(
                    id: $"FieldSetTab{data.tab.Id}",
                    css: " fieldset cf ui-tabs-panel ui-corner-bottom ui-widget-content ",
                    action: () => hb.Fields(
                        context: context,
                        ss: ss,
                        id: id,
                        tab: data.tab,
                        dataSet: dataSet,
                        links: links,
                        preview: preview,
                        editInDialog: editInDialog,
                        resultModel: resultModel,
                        tabIndex: data.index));
            });
            return hb;
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            ResultModel resultModel,
            DataSet dataSet = null,
            List<Link> links = null,
            bool preview = false,
            bool editInDialog = false)
        {
            return hb.Fields(
                context: context,
                ss: ss,
                id: id,
                tab: new Tab { Id = 0 },
                dataSet: !preview
                    ? dataSet ?? HtmlLinks.DataSet(
                        context: context,
                        ss: ss,
                        id: id)
                    : null,
                links: links ?? HtmlLinkCreations.Links(
                    context: context,
                    ss: ss),
                resultModel: resultModel,
                preview: preview,
                editInDialog: editInDialog);
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            Tab tab,
            DataSet dataSet,
            List<Link> links,
            ResultModel resultModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            ss
                .GetEditorColumns(
                    context: context,
                    tab: tab,
                    columnOnly: false)
                ?.Aggregate(new List<KeyValuePair<Section, List<string>>>(), (columns, column) =>
                {
                    var sectionId = ss.SectionId(column.ColumnName);
                    var section = ss
                        .Sections
                        ?.FirstOrDefault(o => o.Id == sectionId);
                    if (section != null)
                    {
                        columns.Add(new KeyValuePair<Section, List<string>>(
                            new Section
                            {
                                Id = section.Id,
                                LabelText = section.LabelText,
                                AllowExpand = section.AllowExpand,
                                Expand = section.Expand,
                                Hide = section.Hide
                            },
                            new List<string>()));
                    }
                    else
                    {
                        if (!columns.Any())
                        {
                            columns.Add(new KeyValuePair<Section, List<string>>(
                                null,
                                new List<string>()));
                        }
                        columns.Last().Value.Add(column.ColumnName);
                    }
                    return columns;
                }).ForEach(section =>
                {
                    if (section.Key == null)
                    {
                        hb.Fields(
                            context: context,
                            ss: ss,
                            id: id,
                            columnNames: section.Value,
                            dataSet: dataSet,
                            links: links,
                            resultModel: resultModel,
                            preview: preview,
                            editInDialog: editInDialog,
                            tabIndex: tabIndex);
                    }
                    else if (section.Key.Hide != true)
                    {
                        hb
                            .Div(
                                id: $"SectionFields{section.Key.Id}Container",
                                css: "section-fields-container",
                                action: () => hb
                                    .Div(action: () => hb.Label(
                                        css: "field-section" + (section.Key.AllowExpand == true
                                            ? " expand"
                                            : string.Empty),
                                        attributes: new HtmlAttributes()
                                            .For($"SectionFields{section.Key.Id}"),
                                        action: () => hb
                                            .Span(css: section.Key.AllowExpand == true
                                                ? section.Key.Expand == true
                                                    ? "ui-icon ui-icon-triangle-1-s"
                                                    : "ui-icon ui-icon-triangle-1-e"
                                                : string.Empty)
                                            .Text(text: section.Key.LabelText)))
                                    .Div(
                                        id: $"SectionFields{section.Key.Id}",
                                        css: section.Key.AllowExpand == true && section.Key.Expand != true
                                            ? "section-fields hidden"
                                            : "section-fields",
                                        action: () => hb.Fields(
                                            context: context,
                                            ss: ss,
                                            id: id,
                                            columnNames: section.Value,
                                            dataSet: dataSet,
                                            links: links,
                                            resultModel: resultModel,
                                            preview: preview,
                                            editInDialog: editInDialog,
                                            tabIndex: tabIndex)));
                    }
                });
            return hb;
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            List<string> columnNames,
            DataSet dataSet,
            List<Link> links,
            ResultModel resultModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            columnNames.ForEach(columnName => hb.Field(
                context: context,
                ss: ss,
                id: id,
                columnName: columnName,
                dataSet: dataSet,
                links: links,
                resultModel: resultModel,
                preview: preview,
                editInDialog: editInDialog,
                tabIndex: tabIndex));
            return hb;
        }

        private static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            string columnName,
            DataSet dataSet,
            List<Link> links,
            ResultModel resultModel,
            bool preview = false,
            bool editInDialog = false,
            int tabIndex = 0)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            var linkId = !preview && !editInDialog ? ss.LinkId(columnName) : 0;
            if (column != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    column: column,
                    preview: preview);
            }
            else if (!editInDialog && linkId != 0)
            {
                hb.LinkField(
                    context: context,
                    ss: ss,
                    id: resultModel.ResultId,
                    linkId: linkId,
                    links: links,
                    dataSet: dataSet,
                    methodType: resultModel?.MethodType,
                    tabIndex: tabIndex);
            }
            return hb;
        }

        private static HtmlAttributes TabActive(
            this HtmlAttributes attributes,
            Context context)
        {
            var tabIndex = context.QueryStrings.Get("TabIndex").ToInt();
            return attributes.Add(
                name: "tab-active",
                value: tabIndex.ToString(),
                _using: tabIndex > 0);
        }

        public static string NumUnit(
            this ResultModel resultModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            if (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty) != "Num"
                || column.ControlType == "Spinner")
            {
                return string.Empty;
            }
            return (column.GetEditorReadOnly()
                || Permissions.ColumnPermissionType(
                    context: context,
                    ss: ss,
                    column: column,
                    baseModel: resultModel) != Permissions.ColumnPermissionTypes.Update
                        ? column.Unit
                        : string.Empty);
        }

        public static string ControlValue(
            this ResultModel resultModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            column.StatusReadOnly = resultModel.GetStatusControl(
                context: context,
                ss: ss,
                column: column) == StatusControl.ControlConstraintsTypes.ReadOnly;
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
                case "Locked":
                    return resultModel.Locked
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return resultModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return resultModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return resultModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return resultModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return resultModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return resultModel.GetAttachments(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        default: return null;
                    }
            }
        }

        public static object ControlRawValue(
            this ResultModel resultModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "ResultId":
                    return resultModel.ResultId
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return resultModel.Ver
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Title":
                    return resultModel.Title
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return resultModel.Body
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Status":
                    return resultModel.Status
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Manager":
                    return resultModel.Manager
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Owner":
                    return resultModel.Owner
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                case "Locked":
                    return resultModel.Locked
                        .ToApiValue(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return resultModel.GetClass(columnName: column.Name)
                                .ToApiValue(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return resultModel.GetNum(columnName: column.Name)
                                .ToApiValue(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return resultModel.GetDate(columnName: column.Name)
                                .ToApiValue(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return resultModel.GetDescription(columnName: column.Name)
                                .ToApiValue(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return resultModel.GetCheck(columnName: column.Name)
                                .ToApiValue(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return resultModel.GetAttachments(columnName: column.Name)
                                .ToApiValue(
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
            ResultModel resultModel = null;
            var copyFrom = context.Forms.Long("CopyFrom");
            if (ss.AllowReferenceCopy == true && copyFrom > 0)
            {
                resultModel = new ResultModel(
                    context: context,
                    ss: ss,
                    resultId: copyFrom,
                    methodType: BaseModel.MethodTypes.New);
                if (resultModel.AccessStatus == Databases.AccessStatuses.Selected
                    && Permissions.CanRead(
                        context: context,
                        siteId: ss.SiteId,
                        id: resultModel.ResultId))
                {
                    resultModel = resultModel.CopyAndInit(
                        context: context,
                        ss: ss);
                }
                else
                {
                    return Messages.ResponseNotFound(context: context).ToJson();
                }
            }
            return EditorResponse(context, ss, resultModel ?? new ResultModel(
                context, ss, resultId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Message message = null,
            string switchTargets = null)
        {
            resultModel.MethodType = resultModel.ResultId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            if (context.QueryStrings.Bool("control-auto-postback"))
            {
                return EditorFields(
                    context: context,
                    ss: ss,
                    resultModel: resultModel);
            }
            else
            {
                var editInDialog = context.Forms.Bool("EditInDialog");
                var process = Process.GetProcess(
                    context: context,
                    ss: ss,
                    getProcessMatchConditions: (o) => resultModel.GetProcessMatchConditions(
                        context: context,
                        ss: ss,
                        process: o));
                var html = Editor(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    editInDialog: editInDialog);
                return editInDialog
                    ? new ResultsResponseCollection(
                        context: context,
                        resultModel: resultModel)
                            .Response("id", resultModel.ResultId.ToString())
                            .Html("#EditInDialogBody", html)
                            .Invoke("openEditorDialog")
                            .Messages(context.Messages)
                            .Events("on_editor_load")
                    : new ResultsResponseCollection(
                        context: context,
                        resultModel: resultModel)
                            .Response("id", resultModel.ResultId.ToString())
                            .Invoke("clearDialogs")
                            .ReplaceAll("#MainContainer", html)
                            .Val("#Id", resultModel.ResultId.ToString())
                            .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                            .SetMemory("formChanged", false)
                            .Invoke("setCurrentIndex")
                            .Invoke("initRelatingColumnEditor")
                            // ?? 以降はプロセスのアクション種別がポストバックの場合にもメッセージを出せるようにする処理
                            .Message(message ?? process?.GetSuccessMessage(context: context))
                            .Messages(context.Messages)
                            .ClearFormData(_using: process?.ActionType != Process.ActionTypes.PostBack)
                            .Events("on_editor_load");
            }
        }

        private static ResponseCollection EditorFields(
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return new ResponseCollection(context: context)
                        .Message(invalid.Message(context: context))
                        .Messages(context.Messages);
            }
            var serverScriptModelRow = resultModel.SetByBeforeOpeningPageServerScript(
                context: context,
                ss: ss);
            var ret = new ResponseCollection(context: context)
                .FieldResponse(
                    context: context,
                    ss: ss,
                    resultModel: resultModel)
                .Html("#Notes", new HtmlBuilder().Notes(
                    context: context,
                    ss: ss,
                    verType: resultModel.VerType,
                    readOnly: resultModel.ReadOnly))
                .ReplaceAll(
                    "#MainCommandsContainer",
                    new HtmlBuilder().EditorMainCommands(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        serverScriptModelRow: serverScriptModelRow),
                    _using: ss.SwitchCommandButtonsAutoPostBack == true)
                .Val("#ControlledOrder", context.ControlledOrder?.ToJson())
                .Invoke("initRelatingColumnEditorNoSend")
                .Messages(context.Messages);
            return ret;
        }

        private static HtmlBuilder EditorMainCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            ServerScriptModelRow serverScriptModelRow)
        {
            resultModel.SetProcessMatchConditions(
                context: context,
                ss: ss);
            return hb.MainCommands(
                context: context,
                ss: ss,
                verType: resultModel.VerType,
                readOnly: resultModel.ReadOnly,
                updateButton: true,
                copyButton: true,
                moveButton: true,
                mailButton: true,
                deleteButton: true,
                serverScriptModelRow: serverScriptModelRow,
                extensions: () => hb
                    .MainCommandExtensions(
                        context: context,
                        ss: ss,
                        resultModel: resultModel)
                    .ProcessCommands(
                        context: context,
                        ss: ss,
                        serverScriptModelRow: serverScriptModelRow));
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long resultId, long siteId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss);
            var param = view.Param(
                context: context,
                ss: ss);
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
            var switchTargets = new List<long>();
            if (Parameters.General.SwitchTargetsLimit > 0)
            {
                if (Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn().ResultsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["ResultId"].ToLong())
                                .ToList();
                }
            }
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
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: resultModel.ServerScriptModelRow);
            res.Val(
                target: "#ReplaceFieldColumns",
                value: replaceFieldColumns?.ToJson());
            res.LookupClearFormData(
                context: context,
                ss: ss);
            var columnNames = ss.GetEditorColumnNames(context.QueryStrings.Bool("control-auto-postback")
                ? ss.GetColumn(
                    context: context,
                    columnName: context.Forms.ControlId().Split_2nd('_'))
                : null);
            columnNames
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    var serverScriptModelColumn = resultModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Results_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                resultModel: resultModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "ResultId":
                                res.Val(
                                    target: "#Results_ResultId" + idSuffix,
                                    value: resultModel.ResultId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Title":
                                res.Val(
                                    target: "#Results_Title" + idSuffix,
                                    value: resultModel.Title.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Results_Body" + idSuffix,
                                    value: resultModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Status":
                                res.Val(
                                    target: "#Results_Status" + idSuffix,
                                    value: resultModel.Status.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Manager":
                                res.Val(
                                    target: "#Results_Manager" + idSuffix,
                                    value: resultModel.Manager.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Owner":
                                res.Val(
                                    target: "#Results_Owner" + idSuffix,
                                    value: resultModel.Owner.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Locked":
                                res.Val(
                                    target: "#Results_Locked" + idSuffix,
                                    value: resultModel.Locked,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Results_{column.Name}{idSuffix}",
                                            value: resultModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Results_{column.Name}{idSuffix}",
                                            value: resultModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Results_{column.Name}{idSuffix}",
                                            value: resultModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Results_{column.Name}{idSuffix}",
                                            value: resultModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Results_{column.Name}{idSuffix}",
                                            value: resultModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Results_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Results_{column.Name}Field",
                                                    controlId: $"Results_{column.Name}",
                                                    columnName: column.ColumnName,
                                                    fieldCss: column.FieldCss
                                                        + (
                                                            column.TextAlign switch
                                                            {
                                                                SiteSettings.TextAlignTypes.Right => " right-align",
                                                                SiteSettings.TextAlignTypes.Center => " center-align",
                                                                _ => string.Empty
                                                            }),
                                                    fieldDescription: column.Description,
                                                    labelText: column.LabelText,
                                                    value: resultModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: resultModel)
                                                            != Permissions.ColumnPermissionTypes.Update,
                                                    allowDelete: column.AllowDeleteAttachments != false,
                                                    validateRequired: column.ValidateRequired != false,
                                                    inputGuide: column.InputGuide),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                }
                                break;
                        }
                    }
                });
            return res;
        }

        public static void SetChoiceHashByFilterExpressions(
            Context context,
            SiteSettings ss,
            Column column,
            ResultModel resultModel,
            string searchText = null,
            int offset = 0,
            bool search = false,
            bool searchFormat = false)
        {
            var link = ss.ColumnFilterExpressionsLink(
                context: context,
                column: column);
            if (link != null)
            {
                var view = link.View;
                var targetSs = ss.GetLinkedSiteSettings(
                    context: context,
                    link: link);
                if (targetSs != null)
                {
                    if (view.ColumnFilterHash == null)
                    {
                        view.ColumnFilterHash = new Dictionary<string, string>();
                    }
                    view.ColumnFilterExpressions.ForEach(data =>
                    {
                        var columnName = data.Key;
                        var targetColumn = targetSs?.GetFilterExpressionsColumn(
                            context: context,
                            link: link,
                            columnName: columnName);
                        if (targetColumn != null)
                        {
                            var expression = data.Value;
                            var raw = expression.StartsWith("=");
                            var hash = new Dictionary<string, Column>();
                            ss.IncludedColumns(value: data.Value).ForEach(includedColumn =>
                            {
                                var guid = Strings.NewGuid();
                                expression = expression.Replace(
                                    $"[{includedColumn.ExpressionColumnName()}]",
                                    guid);
                                hash.Add(guid, includedColumn);
                            });
                            hash.ForEach(hashData =>
                            {
                                var guid = hashData.Key;
                                var includedColumn = hashData.Value;
                                expression = expression.Replace(
                                    guid,
                                    includedColumn.OutputType == Column.OutputTypes.DisplayValue
                                        ? resultModel.ToDisplay(
                                            context: context,
                                            ss: ss,
                                            column: includedColumn,
                                            mine: resultModel.Mine(context: context))
                                        : resultModel.ToValue(
                                            context: context,
                                            ss: ss,
                                            column: includedColumn,
                                            mine: resultModel.Mine(context: context)));
                            });
                            view.SetColumnFilterHashByExpression(
                                ss: targetSs,
                                targetColumn: targetColumn,
                                columnName: columnName,
                                expression: expression,
                                raw: raw);
                        }
                    });
                    column.SetChoiceHash(
                        context: context,
                        ss: ss,
                        link: link,
                        searchText: searchText,
                        offset: offset,
                        search: search,
                        searchFormat: searchFormat,
                        setChoices: true);
                }
            }
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
                            && column.CanCreate(
                                context: context,
                                ss: ss,
                                mine: null)
                            && !column.Id_Ver
                            && column.EditorColumn
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
                            && column.CanRead(
                                context: context,
                                ss: ss,
                                mine: null)
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
            var res = new ResponseCollection(context: context)
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

        public static string SelectedIds(Context context, SiteSettings ss)
        {
            var invalid = ResultValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return SelectedIdsList(
                context: context,
                ss: ss).ToJson();
        }

        public static List<long> SelectedIdsByServerScript(Context context, SiteSettings ss)
        {
            var invalid = ResultValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return SelectedIdsList(
                context: context,
                ss: ss);
        }

        private static List<long> SelectedIdsList(Context context, SiteSettings ss)
        {
            var where = SelectedWhere(
                context: context,
                ss: ss);
            if (where == null)
            {
                return new List<long>();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var ids = new GridData(
                context: context,
                ss: ss,
                view: view,
                column: Rds.ResultsColumn().ResultId(),
                where: where)
                    .DataRows
                    .Select(dataRow => dataRow.Long("ResultId"))
                    .ToList();
            return ids;
        }

        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            long resultId,
            bool internalRequest)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
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
            if (api == null && !context.RequestDataString.IsNullOrEmpty())
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidJsonData));
            }
            var view = api?.View ?? new View();
            var pageSize = api?.PageSize > 0 && api?.PageSize < Parameters.Api.PageSize
                ? api.PageSize
                : Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (resultId > 0)
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash.Add("ResultId", resultId.ToString());
            }
            view.MergeSession(sessionView: Views.GetBySession(
                context: context,
                ss: ss));
            switch (view.ApiDataType)
            {
                case View.ApiDataTypes.KeyValues:
                    var gridData = new GridData(
                        context: context,
                        ss: ss,
                        view: view,
                        tableType: tableType,
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize);
                    return ApiResults.Get(
                        statusCode: 200,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        response: new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = gridData.TotalCount,
                            Data = gridData.KeyValues(
                                context: context,
                                ss: ss,
                                view: view)
                        });
                default:
                    var resultCollection = new ResultCollection(
                        context: context,
                        ss: ss,
                        join: Rds.ItemsJoin().Add(new SqlJoin(
                            tableBracket: "\"Items\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Results\".\"ResultId\"=\"Results_Items\".\"ReferenceId\"",
                            _as: "Results_Items")),
                        where: view.Where(
                            context: context,
                            ss: ss),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss),
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize,
                        tableType: tableType);
                    return ApiResults.Get(
                        statusCode: 200,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        response: new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            resultCollection.TotalCount,
                            Data = resultCollection.Select(o => o.GetByApi(
                                context: context,
                                ss: ss))
                        });
            }
        }

        public static ResultModel[] GetByServerScript(
            Context context,
            SiteSettings ss)
        {
            var invalid = ResultValidators.OnEntry(
                context: context,
                ss: ss,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            var api = context.RequestDataString.Deserialize<Api>();
            var view = api?.View ?? new View();
            var where = view.Where(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where,
                    orderBy
                });
            var pageSize = api?.PageSize > 0 && api?.PageSize < Parameters.Api.PageSize
                ? api.PageSize
                : Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                join: join,
                where: where,
                orderBy: orderBy,
                offset: api?.Offset ?? 0,
                pageSize: pageSize,
                tableType: tableType);
            return resultCollection.ToArray();
        }

        public static ResultModel GetByServerScript(
            Context context,
            SiteSettings ss,
            long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            return resultModel;
        }

        public static string Create(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            ﻿var copyFrom = context.Forms.Int("CopyFrom");
            if (copyFrom > 0 && !Permissions.CanRead(
                context: context,
                siteId: context.SiteId,
                id: copyFrom))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: copyFrom,
                setCopyDefault: copyFrom > 0,
                formData: context.Forms);
            resultModel.ResultId = 0;
            resultModel.Ver = 1;
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                copy: copyFrom > 0);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (copyFrom > 0)
            {
                resultModel.Comments.RemoveAll(o => !o.Created);
            }
            var processes = ss.Processes
                ?.Where(process => process.IsTarget(context: context))
                .ToList() ?? new List<Process>();
            foreach (var process in processes)
            {
                process.MatchConditions = resultModel.GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process);
                if (process.MatchConditions && process.Accessable(
                    context: context,
                    ss: ss))
                {
                    resultModel.SetByProcess(
                        context: context,
                        ss: ss,
                        process: process);
                }
                else if ((process.ExecutionType ?? Process.ExecutionTypes.AddedButton) == Process.ExecutionTypes.AddedButton)
                {
                    var message = process.GetErrorMessage(context: context);
                    message.Text = resultModel.ReplacedDisplayValues(
                        context: context,
                        ss: ss,
                        value: message.Text);
                    return new ResponseCollection(context: context)
                        .Message(message: message)
                        .ToJson();
                }
            }
            var errorData = resultModel.Create(
                context: context,
                ss: ss,
                processes: processes,
                copyFrom: context.Forms.Long("CopyFrom"),
                notice: true,
                otherInitValue: copyFrom > 0);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            process: processes?.FirstOrDefault(o => o.MatchConditions)));
                    return new ResponseCollection(
                        context: context,
                        id: resultModel.ResultId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .Href(Locations.Edit(
                                context: context,
                                controller: context.Controller,
                                id: ss.Columns.Any(o => o.Linking)
                                    ? context.Forms.Long("LinkId")
                                    : resultModel.ResultId)
                                        + "?new=1"
                                        + (ss.Columns.Any(o => o.Linking)
                                            && context.Forms.Long("FromTabIndex") > 0
                                                ? $"&TabIndex={context.Forms.Long("FromTabIndex")}"
                                                : string.Empty))
                            .ToJson();
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                    {
                        return Messages.ResponseDuplicated(
                            context: context,
                            data: ss.GetColumn(
                                context: context,
                                columnName: errorData.ColumnName)?.LabelText)
                                    .ToJson();
                    }
                    else
                    {
                        return new ResponseCollection(context: context).Message(
                            message: new Message()
                            {
                                Id = "MessageWhenDuplicated",
                                Text = duplicatedColumn.MessageWhenDuplicated,
                                Css = "alert-error"
                            }).ToJson();
                    }
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: resultModel.Title.DisplayValue);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = resultModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static ContentResultInheritance CreateByApi(Context context, SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ItemsLimit));
            }
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            if(HasInvalidValueAsApiDataAtCreate(resultApiModel))
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                resultApiModel: resultApiModel);
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
            resultModel.SetTitle(
                context: context,
                ss: ss);
            var process = ss.Processes?.FirstOrDefault(process => process.Id == resultApiModel.ProcessId);
            if (process != null)
            {
                process.MatchConditions = resultModel.GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process);
                if (process.MatchConditions && process.Accessable(
                    context: context,
                    ss: ss))
                {
                    resultModel.SetByProcess(
                        context: context,
                        ss: ss,
                        process: process);
                }
                else if ((process.ExecutionType ?? Process.ExecutionTypes.AddedButton) == Process.ExecutionTypes.AddedButton)
                {
                    return ApiResults.BadRequest(context: context);
                }
            }
            var errorData = resultModel.Create(
                context: context,
                ss: ss,
                processes: process?.ToSingleList(),
                notice: true);
            BinaryUtilities.UploadImage(
                context: context,
                ss: ss,
                id: resultModel.ResultId,
                postedFileHash: resultModel.PostedImageHash);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: resultModel.ResultId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            process: process).Text);
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    return ApiResults.Duplicated(
                        context: context,
                        message: duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                            ? Displays.Duplicated(
                                context: context,
                                data: duplicatedColumn?.LabelText)
                            : duplicatedColumn?.MessageWhenDuplicated);
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool CreateByServerScript(Context context, SiteSettings ss, object model)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return false;
            }
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            if(HasInvalidValueAsApiDataAtCreate(resultApiModel))
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                resultApiModel: resultApiModel);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
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
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is ResultModel data)
                        {
                            data.ResultId = resultModel.ResultId;
                            data.SetByModel(resultModel: resultModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

        private static bool HasInvalidValueAsApiDataAtCreate(ResultApiModel model)
        {
            if (model is null)
                return false;
            foreach (var o in model.AttachmentsHash)
            {
                foreach (var attachment in o.Value)
                {
                    if (attachment.Deleted ?? false)
                        continue;
                    if (attachment.Name.IsNullOrEmpty())
                        return true;
                    if (attachment.Base64 is null && attachment.Base64Binary is null)
                        return true;
                }
            }
            return false;
        }

        public static string Update(Context context, SiteSettings ss, long resultId, string previousTitle)
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
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var processes = ss.Processes
                ?.Where(process => process.IsTarget(context: context))
                .ToList() ?? new List<Process>();
            foreach (var process in processes)
            {
                process.MatchConditions = resultModel.GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process);
                if (process.MatchConditions && process.Accessable(
                    context: context,
                    ss: ss))
                {
                    resultModel.SetByProcess(
                        context: context,
                        ss: ss,
                        process: process);
                }
                else if ((process.ExecutionType ?? Process.ExecutionTypes.AddedButton) == Process.ExecutionTypes.AddedButton)
                {
                    var message = process.GetErrorMessage(context: context);
                    message.Text = resultModel.ReplacedDisplayValues(
                        context: context,
                        ss: ss,
                        value: message.Text);
                    return new ResponseCollection(context: context)
                        .Message(message: message)
                        .ToJson();
                }
            }
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                processes: processes,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new ResultsResponseCollection(
                        context: context,
                        resultModel: resultModel);
                    switch (Parameters.General.UpdateResponseType)
                    {
                        case 1:
                            return ResponseByUpdate(res, context, ss, resultModel, processes)
                                .PrependComment(
                                    context: context,
                                    ss: ss,
                                    column: ss.GetColumn(context: context, columnName: "Comments"),
                                    comments: resultModel.Comments,
                                    verType: resultModel.VerType)
                                .ToJson();
                        default:
                            return ResponseByUpdate(res, context, ss, resultModel, processes)
                                .ToJson();
                    }
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                    {
                        return Messages.ResponseDuplicated(
                            context: context,
                            data: ss.GetColumn(
                                context: context,
                                columnName: errorData.ColumnName)?.LabelText)
                                    .ToJson();
                    }
                    else
                    {
                        return new ResponseCollection(context: context).Message(
                            message: new Message()
                            {
                                Id = "MessageWhenDuplicated",
                                Text = duplicatedColumn.MessageWhenDuplicated,
                                Css = "alert-error"
                            }).ToJson();
                    }
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: resultModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            ResultsResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: resultModel);
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
                            view: view,
                            dataRows: gridData.DataRows,
                            columns: columns))
                    .CloseDialog()
                    .Message(message: UpdatedMessage(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        processes: processes))
                    .Messages(context.Messages);
            }
            else if (resultModel.Locked)
            {
                ss.SetLockedRecord(
                    context: context,
                    time: resultModel.UpdatedTime,
                    user: resultModel.Updator);
                return EditorResponse(
                    context: context,
                    ss: ss,
                    resultModel: resultModel)
                        .SetMemory("formChanged", false)
                        .Message(message: UpdatedMessage(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            processes: processes))
                        .Messages(context.Messages)
                        .ClearFormData();
            }
            else
            {
                switch (Parameters.General.UpdateResponseType)
                {
                    case 1:
                        var verUp = Versions.VerUp(
                            context: context,
                            ss: ss,
                            verUp: false);
                        return res
                            .Ver(context: context, ss: ss)
                            .Timestamp(context: context, ss: ss)
                            .FieldResponse(context: context, ss: ss, resultModel: resultModel)
                            .Val("#VerUp", verUp)
                            .Val("#Ver", resultModel.Ver)
                            .Disabled("#VerUp", verUp)
                            .Html("#HeaderTitle", HttpUtility.HtmlEncode(resultModel.Title.MessageDisplay(context: context)))
                            .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                                context: context,
                                baseModel: resultModel,
                                tableName: "Results"))
                            .Html("#Links", new HtmlBuilder().Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId))
                            .Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId,
                                methodType: resultModel.MethodType)
                            .SetMemory("formChanged", false)
                            .Message(message: UpdatedMessage(
                                context: context,
                                ss: ss,
                                resultModel: resultModel,
                                processes: processes))
                            .Messages(context.Messages)
                            .Comment(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(context: context, columnName: "Comments"),
                                comments: resultModel.Comments,
                                deleteCommentId: resultModel.DeleteCommentId)
                            .ClearFormData();
                    default:
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            message: UpdatedMessage(
                                context: context,
                                ss: ss,
                                resultModel: resultModel,
                                processes: processes));
                }
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: resultModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = resultModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string OpenBulkUpdateSelectorDialog(Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var where = SelectedWhere(
                context: context,
                ss: ss);
            if (where == null)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var optionCollection = ss.GetAllowBulkUpdateOptions(context: context);
            var target = optionCollection.FirstOrDefault().Key;
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .Html(
                    "#BulkUpdateSelectorDialog",
                    hb.BulkUpdateSelectorDialog(
                        context: context,
                        ss: ss,
                        optionCollection: optionCollection,
                        action: () => hb
                            .BulkUpdateEditor(
                                context: context,
                                ss: ss,
                                target: target)))
                .ToJson();
        }

        public static string BulkUpdateSelectChanged(Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#BulkUpdateSelectedField",
                    new HtmlBuilder().BulkUpdateEditor(
                        context: context,
                        ss: ss,
                        target: context.Forms.Data("BulkUpdateColumnName")))
                .ClearFormData()
                .ToJson();
        }

        private static HtmlBuilder BulkUpdateEditor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string target)
        {
            var columns = ss.GetBulkUpdateColumns(
                context: context,
                target: target);
            ss.SetBulkUpdateColumnDetail(
                columns: columns,
                target: target);
            var resultModel = new ResultModel();
            hb.FieldSet(id: "BulkUpdateEditor", css: "both", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.Editor(context: context),
                    action: () => columns.ForEach(column =>
                    {
                        resultModel.SetDefault(
                            context: context,
                            ss: ss,
                            column: column);
                        hb.Field(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            column: column,
                            disableAutoPostBack: true,
                            disableSection: true);
                    })));
            return hb;
        }

        public static string BulkUpdate(Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var param = new Rds.ResultsParamCollection();
            var target = context.Forms.Data("BulkUpdateColumnName");
            var columns = ss.GetBulkUpdateColumns(
                context: context,
                target: target);
            if (!columns.All(column => column.CanUpdate(
                context: context,
                ss: ss,
                mine: null)))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                formData: context.Forms);
            var selectedWhere = SelectedWhere(
                context: context,
                ss: ss);
            if (selectedWhere == null)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var where = view.Where(
                context: context,
                ss: ss,
                where: selectedWhere,
                itemJoin: false);
            param = (Rds.ResultsParamCollection)view.Param(
                context: context,
                ss: ss,
                param: param);
            var invalid = ExistsLockedRecord(
                context: context,
                ss: ss,
                where: where,
                param: param,
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                lockedColumn: columns.Any(o => o.ColumnName == "Locked"));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return Error.Types.ImportLock.MessageJson(context: context);
            }
            var count = BulkUpdate(
                context: context,
                ss: ss,
                columns: columns,
                where: where,
                watchdog: () => exclusiveObj.Refresh());
            var data = new string[]
            {
                ss.Title,
                count.ToString()
            };
            ss.Notifications
                .Where(o => o.MonitorChangesColumns.Any(columnName => columns.Any(q => q.ColumnName == columnName)))
                .ForEach(notification =>
                {
                    var body = new System.Text.StringBuilder();
                    body.Append(Locations.ItemIndexAbsoluteUri(
                        context: context,
                        ss.SiteId) + "\n");
                    body.Append(
                        $"{Displays.Results_Updator(context: context)}: ",
                        $"{context.User.Name}\n");
                    columns.ForEach(column =>
                        body.Append(
                            $"{column.LabelText} : ",
                            $"{resultModel.ToDisplay(context: context, ss: ss, column: column, mine: null)}\n"));
                    if (notification.AfterBulkUpdate != false)
                    {
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.BulkUpdated(
                                context: context,
                                data: data),
                            body: body.ToString());
                    }
                });
            var res = GridRows(
                context: context,
                ss: ss,
                clearCheck: true,
                message: Messages.BulkUpdated(
                    context: context,
                    data: data));
            return res;
        }

        private static int BulkUpdate(
            Context context,
            SiteSettings ss,
            List<Column> columns,
            SqlWhereCollection where,
            Action watchdog = null)
        {
            var onBulkUpdatingExtendedStatements = new List<SqlStatement>().OnBulkUpdatingExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            if (onBulkUpdatingExtendedStatements.Count > 0)
            {
                watchdog?.Invoke();
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: onBulkUpdatingExtendedStatements.ToArray());
            }
            var count = 0;
            new ResultCollection(
                context: context,
                ss: ss,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        where
                    }),
                where: where).ForEach(resultModel =>
                {
                    watchdog?.Invoke();
                    resultModel.SetByForm(
                        context: context,
                        ss: ss,
                        formData: context.Forms);
                    resultModel.SetBySettings(
                        context: context,
                        ss: ss);
                    resultModel.VerUp = Versions.MustVerUp(
                        context: context,
                        ss: ss,
                        baseModel: resultModel);
                    resultModel.Update(
                        context: context,
                        ss: ss,
                        extendedSqls: false);
                    count++;
                });
            var onBulkUpdatedExtendedStatements = new List<SqlStatement>().OnBulkUpdatedExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            if (onBulkUpdatedExtendedStatements.Count > 0)
            {
                watchdog?.Invoke();
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: onBulkUpdatedExtendedStatements.ToArray());
            }
            return count;
        }

        public static string UpdateByGrid(Context context, SiteSettings ss)
        {
            if (ss.GridEditorType != SiteSettings.GridEditorTypes.Grid)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss)
                    .Where(o => !o.Suffix.IsNullOrEmpty())
                    .Where(o => o.SiteId == ss.SiteId)
                    .ToList();
            var statements = new List<SqlStatement>();
            statements.OnUpdatingByGridExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(formDataSet
                        .Where(formData => formData.Id > 0)
                        .Select(formData => formData.Id)),
                formDataSet: formDataSet);
            var exists = ExistsLockedRecord(
                context: context,
                ss: ss,
                targets: resultCollection.Select(o => o.ResultId).ToList());
            switch (exists.Type)
            {
                case Error.Types.None: break;
                default: return exists.MessageJson(context: context);
            }
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
                    resultModel.SetByBeforeUpdateServerScript(
                        context: context,
                        ss: ss);
                    if (context.ErrorData.Type != Error.Types.None)
                    {
                        return (context.ErrorData).MessageJson(context: context);
                    }
                    var invalid = ResultValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.MessageJson(context: context);
                    }
                    resultModel.VerUp = Versions.MustVerUp(
                        context: context,
                        ss: ss,
                        baseModel: resultModel);
                    statements.AddRange(resultModel.UpdateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString(),
                        verUp: resultModel.VerUp));
                }
                else if (formData.Id < 0)
                {
                    resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        formData: formData.Data);
                    resultModel.SetByBeforeCreateServerScript(
                        context: context,
                        ss: ss);
                    if (context.ErrorData.Type != Error.Types.None)
                    {
                        return (context.ErrorData).MessageJson(context: context);
                    }
                    var invalid = ResultValidators.OnCreating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel);
                    switch (invalid.Type)
                    {
                        case Error.Types.None: break;
                        default: return invalid.MessageJson(context: context);
                    }
                    statements.AddRange(resultModel.CreateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                    resultCollection.Add(resultModel);
                }
                else
                {
                    return ItemUtilities.ClearItemDataResponse(
                        context: context,
                        ss: ss,
                        id: formData.Id)
                            .Remove($"[data-id=\"{formData.Id}\"][data-latest]")
                            .Message(Messages.NotFound(context: context))
                            .Messages(context.Messages)
                            .ToJson();
                }
                resultModel?.SetByBeforeOpeningRowServerScript(
                    context: context,
                    ss: ss,
                    view: view);
            }
            statements.OnUpdatedByGridExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            var responses = Repository.ExecuteDataSet_responses(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            resultCollection.ForEach(resultModel =>
                resultModel.WriteAttachments(
                    context: context,
                    ss: ss,
                    verUp: Versions.VerUp(
                        context: context,
                        ss: ss,
                        verUp: resultModel.VerUp)));
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
                        view: view,
                        formDataSet: formDataSet,
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

        private static string UpdateByGridSuccess(
            Context context,
            SiteSettings ss,
            View view,
            List<FormData> formDataSet,
            List<SqlResponse> responses,
            Dictionary<long, List<Notification>> notificationHash)
        {
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(responses
                        .Where(o => o.Id > 0)
                        .Select(o => o.Id.ToLong())
                        .ToList()));
            resultCollection.ForEach(resultModel =>
            {
                resultModel.SynchronizeSummary(
                    context: context,
                    ss: ss);
                if (!formDataSet.Any(o => o.Id == resultModel.ResultId))
                {
                    resultModel.ExecuteAutomaticNumbering(
                        context: context,
                        ss: ss);
                    resultModel.Notice(
                        context: context,
                        ss: ss,
                        notifications: resultModel.GetNotifications(
                            context: context,
                            ss: ss,
                            notice: true),
                        type: "Created");
                    resultModel.SetByAfterCreateServerScript(
                        context: context,
                        ss: ss);                
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
                    resultModel.SetByAfterUpdateServerScript(
                        context: context,
                        ss: ss);
                }
            });
            Repository.ExecuteNonQuery(
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
            var res = new ResponseCollection(context: context);
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
                        view: view,
                        dataRow: dataRow,
                        columns: columns,
                        recordSelector: null,
                        editRow: true,
                        checkRow: false,
                        idColumn: "ResultId")));
            formDataSet.ForEach(formData =>
                formData.Data.Keys.ForEach(controlId =>
                    res.ClearFormData(controlId + formData.Suffix)));
            return res
                .SetMemory("formChanged", false)
                .Message(Messages.UpdatedByGrid(
                    context: context,
                    data: responses.Count().ToString()))
                .Messages(context.Messages)
                .ToJson();
        }

        public static string BulkProcess(Context context, SiteSettings ss)
        {
            if (!context.HasPermission(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var processId = context.Forms.Int("BulkProcessingItems");
            var process = ss.GetProcess(
                context: context,
                id: processId);
            if (process == null || !process.GetAllowBulkProcessing())
            {
                return Messages.NotFound(context: context).ToJson();
            }
            var processes = ss.Processes
                ?.Where(o => o.Id == processId 
                || (o.ExecutionType == Process.ExecutionTypes.AddedButtonOrCreateOrUpdate
                    && ((process.ExecutionType == Process.ExecutionTypes.CreateOrUpdate)
                    || ((process.ExecutionType ?? Process.ExecutionTypes.AddedButton) == Process.ExecutionTypes.AddedButton)
                        && ((process.ActionType ?? Process.ActionTypes.Save) == Process.ActionTypes.Save))))
                .ToList() ?? new List<Process>();
            var selectedWhere = SelectedWhere(
                context: context,
                ss: ss);
            if (selectedWhere == null)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var where = view.Where(
                context: context,
                ss: ss,
                where: selectedWhere,
                itemJoin: false);
            var param = view.Param(
                context: context,
                ss: ss);
            var invalid = ExistsLockedRecord(
                context: context,
                ss: ss,
                where: where,
                param: param,
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var succeeded = 0;
            Message errorMessage = null;
            foreach (var resultModel in new ResultCollection(
                context: context,
                ss: ss,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        where
                    }),
                where: where))
            {
                // 前回の処理でエラーが起きていたら次の処理をせずに中断
                if (errorMessage != null) break;
                process.MatchConditions = resultModel.GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process);
                if (!process.MatchConditions)
                {
                    errorMessage = process.GetErrorMessage(context: context);
                }
                else
                {
                    var previousTitle = resultModel.Title.DisplayValue;
                    foreach (var p in processes)
                    {
                        p.MatchConditions = resultModel.GetProcessMatchConditions(
                            context: context,
                            ss: ss,
                            process: p);
                        if (p.MatchConditions && p.Accessable(
                            context: context,
                            ss: ss))
                        {
                            resultModel.SetByProcess(
                                context: context,
                                ss: ss,
                                process: p);
                        }
                    }
                    resultModel.VerUp = Versions.MustVerUp(
                        context: context,
                        ss: ss,
                        baseModel: resultModel);
                    var errorData = resultModel.Update(
                        context: context,
                        ss: ss,
                        processes: processes,
                        notice: true,
                        previousTitle: previousTitle);
                    switch (errorData.Type)
                    {
                        case Error.Types.None:
                            succeeded++;
                            break;
                        case Error.Types.Duplicated:
                            var duplicatedColumn = ss.GetColumn(
                                context: context,
                                columnName: errorData.ColumnName);
                            if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                            {
                                errorMessage = Messages.Duplicated(
                                    context: context,
                                    data: ss.GetColumn(
                                        context: context,
                                        columnName: errorData.ColumnName)?.LabelText);
                            }
                            else
                            {
                                errorMessage = new Message()
                                {
                                    Id = "MessageWhenDuplicated",
                                    Text = duplicatedColumn.MessageWhenDuplicated,
                                    Css = "alert-error"
                                };
                            }
                            break;
                        case Error.Types.UpdateConflicts:
                            errorMessage = Messages.UpdateConflicts(
                                context: context,
                                data: resultModel.Updator.Name);
                            break;
                        default:
                            errorMessage = errorData.Message(context: context);
                            break;
                    }
                }
            }
            if (errorMessage != null)
            {
                context.Messages.Add(errorMessage);
            }
            if (succeeded > 0)
            {
                context.Messages.Add(
                    Messages.BulkProcessed(
                        context: context,
                        data: new string[]
                        {
                            process.GetSuccessMessage(context:context).Text,
                            succeeded.ToString()
                        }));
            }
            var res = GridRows(
                context: context,
                ss: ss,
                clearCheck: true);
            return res;
        }

        public static ContentResultInheritance UpdateByApi(
            Context context,
            SiteSettings ss,
            long resultId,
            string previousTitle)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            if(HasInvalidValueAsApiDataAtUpdate(resultApiModel))
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                resultApiModel: resultApiModel);
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
                ss: ss,
                baseModel: resultModel);
            var process = ss.Processes?.FirstOrDefault(process => process.Id == resultApiModel.ProcessId);
            if (process != null)
            {
                process.MatchConditions = resultModel.GetProcessMatchConditions(
                    context: context,
                    ss: ss,
                    process: process);
                if (process.MatchConditions && process.Accessable(
                    context: context,
                    ss: ss))
                {
                    resultModel.SetByProcess(
                        context: context,
                        ss: ss,
                        process: process);
                }
                else if ((process.ExecutionType ?? Process.ExecutionTypes.AddedButton) == Process.ExecutionTypes.AddedButton)
                {
                    return ApiResults.BadRequest(context: context);
                }
            }
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                processes: process?.ToSingleList(),
                notice: true,
                previousTitle: previousTitle);
            BinaryUtilities.UploadImage(
                context: context,
                ss: ss,
                id: resultModel.ResultId,
                postedFileHash: resultModel.PostedImageHash);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: UpdatedMessage(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            processes: process?.ToSingleList()).Text);
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    return ApiResults.Duplicated(
                        context: context,
                        message: duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                            ? Displays.Duplicated(
                                context: context,
                                data: duplicatedColumn?.LabelText)
                            : duplicatedColumn?.MessageWhenDuplicated);
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool UpdateByServerScript(
            Context context,
            SiteSettings ss,
            long resultId,
            string previousTitle,
            object model)
        {
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            if(HasInvalidValueAsApiDataAtUpdate(resultApiModel))
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                resultApiModel: resultApiModel);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(
                context: context,
                ss: ss);
            resultModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: resultModel);
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is ResultModel data)
                        {
                            data.SetByModel(resultModel: resultModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

                private static bool HasInvalidValueAsApiDataAtUpdate(ResultApiModel model)
                {
                    if (model is null)
                        return false;
                    foreach (var o in model.AttachmentsHash)
                    {
                        foreach (var attachment in o.Value)
                        {
                            if (attachment.Deleted ?? false)
                                continue;
                            if (attachment.Name.IsNullOrEmpty())
                                return true;
                            if (attachment.Base64 is null && attachment.Base64Binary is null)
                                return true;
                        }
                    }
                    return false;
                }

        public static ContentResultInheritance UpsertByApi(
            Context context,
            SiteSettings ss,
            string previousTitle)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            var data = context.RequestDataString.Deserialize<ResultApiModel>();
            if (api?.Keys?.Any() != true || data == null)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidJsonData));
            }
            var error = new ErrorData(Error.Types.None);
            api.View = api.View ?? new View();
            api.Keys.ForEach(columnName =>
            {
                if (error.Type != Error.Types.None) return;
                var objectValue = data.ObjectValue(columnName: columnName);
                if (objectValue != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column?.TypeName == "datetime"
                        && objectValue.ToDateTime().InRange() == false)
                    {
                        error = new ErrorData(
                            type: Error.Types.invalidUpsertKey,
                            data: $"('{columnName}'='{objectValue.ToStr()}')");
                        return;
                    }
                    api.View.AddColumnFilterHash(
                        context: context,
                        ss: ss,
                        column: column,
                        objectValue: objectValue);
                    api.View.AddColumnFilterSearchTypes(
                        columnName: columnName,
                        searchType: Column.SearchTypes.ExactMatch);
                }
            });
            if (error.Type != Error.Types.None)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: error);
            }
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                view: api.View,
                resultApiModel: resultApiModel);
            switch (resultModel.AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    break;
                case Databases.AccessStatuses.NotFound:
                    return CreateByApi(context: context, ss: ss);
                case Databases.AccessStatuses.Overlap:
                    return ApiResults.Get(ApiResponses.Overlap(context: context));
                default:
                    return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            // サイトの書き込み権限で可否判定を行い、レコード単位のアクセス権はチェックは行わない。
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                context: context,
                errorData: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(
                context: context,
                ss: ss);
            resultModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: resultModel);
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            BinaryUtilities.UploadImage(
                context: context,
                ss: ss,
                id: resultModel.ResultId,
                postedFileHash: resultModel.PostedImageHash);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Updated(
                            context: context,
                            data: resultModel.Title.MessageDisplay(context: context)));
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    return ApiResults.Duplicated(
                        context: context,
                        message: duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                            ? Displays.Duplicated(
                                context: context,
                                data: duplicatedColumn?.LabelText)
                            : duplicatedColumn?.MessageWhenDuplicated);
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool UpsertByServerScript(
            Context context,
            SiteSettings ss,
            string previousTitle,
            object model)
        {
            var api = context.RequestDataString.Deserialize<Api>();
            var resultApiModel = context.RequestDataString.Deserialize<ResultApiModel>();
            if (api?.Keys?.Any() != true || resultApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
                return false;
            }
            var error = Error.Types.None;
            api.View = api.View ?? new View();
            api.Keys.ForEach(columnName =>
            {
                if (error != Error.Types.None) return;
                var objectValue = resultApiModel.ObjectValue(columnName: columnName);
                if (objectValue != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column?.TypeName == "datetime"
                        && objectValue.ToDateTime().InRange() == false)
                    {
                        error = Error.Types.invalidUpsertKey;
                        return;
                    }
                    api.View.AddColumnFilterHash(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(
                            context: context,
                            columnName: columnName),
                        objectValue: objectValue);
                    api.View.AddColumnFilterSearchTypes(
                        columnName: columnName,
                        searchType: Column.SearchTypes.ExactMatch);
                }
            });
            if (error != Error.Types.None)
            {
                return false;
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                view: api.View,
                resultApiModel: resultApiModel);
            switch (resultModel.AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    break;
                case Databases.AccessStatuses.NotFound:
                    return CreateByServerScript(
                        context: context,
                        ss: ss,
                        model: model);
                default:
                    return false;
            }
            // サイトの書き込み権限で可否判定を行い、レコード単位のアクセス権はチェックは行わない。
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                default:
                    return false;
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(
                context: context,
                ss: ss);
            resultModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: resultModel);
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is ResultModel data)
                        {
                            data.SetByModel(resultModel: resultModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

        public static ContentResultInheritance BulkUpsertByApi(
            Context context,
            SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            var list = context.RequestDataString.Deserialize<Results.ResultBulkUpsertApiModel>();
            if (list?.Data == null)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidJsonData));
            }
            if (Parameters.General.BulkUpsertMax > 0 && Parameters.General.BulkUpsertMax < list.Data.Count)
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Error.Types.ImportMax.Message(
                        context: context,
                        data: Parameters.General.BulkUpsertMax.ToString()).Text));
            }
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 429,
                    message: Messages.ImportLock(context: context).Text));
            }
            var recodeCount = 0;
            var insertCount = 0;
            var updateCount = 0;
            var error = new ErrorData(type: Error.Types.None);
            foreach (var resultApiModel in list.Data)
            {
                recodeCount++;
                exclusiveObj.Refresh();
                var view = api.View ?? new View();
                api.Keys?.ForEach(columnName =>
                {
                    if (error.Type != Error.Types.None) return;
                    var objectValue = resultApiModel.ObjectValue(columnName: columnName);
                    if (objectValue != null)
                    {
                        var column = ss.GetColumn(
                            context: context,
                            columnName: columnName);
                        if (column?.TypeName == "datetime"
                            && objectValue.ToDateTime().InRange() == false)
                        {
                            error = new ErrorData(
                                type: Error.Types.invalidUpsertKey,
                                data: $"('{columnName}'='{objectValue.ToStr()}')");
                            return;
                        }
                        view.AddColumnFilterHash(
                            context: context,
                            ss: ss,
                            column: column,
                            objectValue: objectValue);
                        view.AddColumnFilterSearchTypes(
                            columnName: columnName,
                            searchType: Column.SearchTypes.ExactMatch);
                    }
                });
                if (error.Type != Error.Types.None) break;
                var resultModel = new ResultModel(
                    context: context,
                    ss: ss,
                    resultId: 0,
                    view: api.Keys?.Any() != true ? null : view,
                    resultApiModel: resultApiModel);
                switch (resultModel.AccessStatus)
                {
                    case Databases.AccessStatuses.Selected:
                    case Databases.AccessStatuses.NotFound:
                        break;
                    case Databases.AccessStatuses.Overlap:
                        error = new ErrorData(type: Error.Types.Overlap);
                        break;
                    default:
                        error = new ErrorData(type: Error.Types.NotFound);
                        break;
                }
                if (error.Type != Error.Types.None) break;
                if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    // Keysの指定があり、該当レコードがある場合に更新
                    // サイトの書き込み権限で可否判定を行い、レコード単位のアクセス権はチェックは行わない。
                    error = ResultValidators.OnUpdating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        api: true,
                        serverScript: true);
                    if (error.Type != Error.Types.None) break;
                    resultModel.SiteId = ss.SiteId;
                    resultModel.SetTitle(
                        context: context,
                        ss: ss);
                    resultModel.VerUp = Versions.MustVerUp(
                        context: context,
                        ss: ss,
                        baseModel: resultModel);
                    error = resultModel.Update(
                        context: context,
                        ss: ss,
                        notice: true);
                    BinaryUtilities.UploadImage(
                        context: context,
                        ss: ss,
                        id: resultModel.ResultId,
                        postedFileHash: resultModel.PostedImageHash);
                    if (error.Type != Error.Types.None) break;
                    updateCount++;
                }
                else if (resultModel.AccessStatus == Databases.AccessStatuses.NotFound
                    && (api.Keys?.Any() != true || list.KeyNotFoundCreate != false))
                {
                    // Keysの指定が無い場合は全て新規作成。
                    // Keysの指定があり、該当レコードがなく KeyNotFoundCreate =true の場合に新規作成
                    error = ResultValidators.OnCreating(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        api: true);
                    if (error.Type != Error.Types.None) break;
                    resultModel.SiteId = ss.SiteId;
                    resultModel.SetTitle(
                        context: context,
                        ss: ss);
                    var errorData = resultModel.Create(
                        context: context,
                        ss: ss,
                        notice: true);
                    BinaryUtilities.UploadImage(
                        context: context,
                        ss: ss,
                        id: resultModel.ResultId,
                        postedFileHash: resultModel.PostedImageHash);
                    if (error.Type != Error.Types.None) break;
                    insertCount++;
                }
            }
            exclusiveObj.Refresh();
            if (error.Type != Error.Types.None)
            {
                // エラー時の戻り
                var errMessage = error.Data?.Any() == true
                        ? Displays.Get(
                            context: context,
                            id: error.Type.ToString()).Params(error.Data)
                        : Displays.Get(
                            context: context,
                            id: error.Type.ToString());
                if (error.Type == Error.Types.Duplicated)
                {
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: error.ColumnName);
                    errMessage = duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                        ? Displays.Duplicated(
                            context: context,
                            data: duplicatedColumn?.LabelText)
                        : duplicatedColumn?.MessageWhenDuplicated;
                }
                var recodeIndex = recodeCount.ToString();
                if(api.Keys?.Any() != false)
                {
                    var resultApiModel = list.Data[recodeCount - 1];
                    recodeIndex += "("
                        + api.Keys.Select(
                                columnName => $"{columnName}={resultApiModel.ObjectValue(columnName: columnName) ?? string.Empty}"
                            ).Join()
                        + ")";
                }
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Displays.FailedBulkUpsert(
                        context: context,
                        data: new string[]
                        {
                            ss.Title,
                            insertCount.ToString(),
                            updateCount.ToString(),
                            recodeIndex,
                            errMessage
                        })));
            }
            ss.Notifications.ForEach(notification =>
            {
                var body = new System.Text.StringBuilder();
                body.Append(Locations.ItemIndexAbsoluteUri(
                    context: context,
                    ss.SiteId) + "\n");
                body.Append(
                    $"{Displays.Results_Updator(context: context)}: ",
                    $"{context.User.Name}\n");
                if (notification.AfterImport != false)
                {
                    notification.Send(
                        context: context,
                        ss: ss,
                        title: Displays.Imported(
                            context: context,
                            data: new string[]
                            {
                                ss.Title,
                                insertCount.ToString(),
                                updateCount.ToString()
                            }),
                        body: body.ToString());
                }
            });
            return ApiResults.Success(
                id: context.Id,
                limitPerDate: context.ContractSettings.ApiLimit(),
                limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                message: Messages.Imported(
                    context: context,
                    data: new string[]
                    {
                        ss.Title,
                        insertCount.ToString(),
                        updateCount.ToString()
                    }).Text);
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
                default: return invalid.MessageJson(context: context);
            }
            resultModel.ResultId = 0;
            resultModel.Ver = 1;
            if (ss.GetEditorColumnNames().Contains("Title"))
            {
                resultModel.Title.Value += ss.CharToAddWhenCopying;
            }
            if (!context.Forms.Bool("CopyWithComments"))
            {
                resultModel.Comments.Clear();
            }
            resultModel.SetCopyDefault(
                context: context,
                ss: ss);
            var errorData = resultModel.Create(
                context: context,
                ss: ss,
                copyFrom: resultId,
                forceSynchronizeSourceSummary: true,
                notice: true,
                noticeType: "Copied",
                otherInitValue: true);
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
                        return new ResponseCollection(context: context)
                            .Response("id", resultModel.ResultId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: resultModel.ResultId))
                            .ToJson();
                    }
                case Error.Types.Duplicated:
                    var duplicatedColumn = ss.GetColumn(
                        context: context,
                        columnName: errorData.ColumnName);
                    if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                    {
                        return Messages.ResponseDuplicated(
                            context: context,
                            data: ss.GetColumn(
                                context: context,
                                columnName: errorData.ColumnName)?.LabelText)
                                    .ToJson();
                    }
                    else
                    {
                        return new ResponseCollection(context: context).Message(
                            message: new Message()
                            {
                                Id = "MessageWhenDuplicated",
                                Text = duplicatedColumn.MessageWhenDuplicated,
                                Css = "alert-error"
                            }).ToJson();
                    }
                default:
                    return errorData.MessageJson(context: context);
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
                ss: ss,
                destinationSs: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: resultId),
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
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
                            message: Messages.Moved(
                                context: context,
                                data: resultModel.Title.MessageDisplay(context: context)),
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
                                data: resultModel.Title.MessageDisplay(context: context)));
                        return new ResponseCollection(context: context)
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
                    return errorData.MessageJson(context: context);
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
                default: return invalid.MessageJson(context: context);
            }
            var errorData = resultModel.Delete(context: context, ss: ss, notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: resultModel.Title.MessageDisplay(context: context)));
                    var res = new ResultsResponseCollection(
                        context: context,
                        resultModel: resultModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static ContentResultInheritance DeleteByApi(
            Context context, SiteSettings ss, long resultId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
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
                        id: resultModel.ResultId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Deleted(
                            context: context,
                            data: resultModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        public static bool DeleteByServerScript(
            Context context,
            SiteSettings ss,
            long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = ResultValidators.OnDeleting(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
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
                    return true;
                default:
                    return false;
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
                var selector = new RecordSelector(context: context);
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
            var subWhere = Views.GetBySession(
                context: context,
                ss: ss)
                    .Where(
                        context: context,
                        ss: ss,
                        itemJoin: false);
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
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectResults(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Results_Deleted",
                column: Rds.ResultsColumn()
                    .ResultId(tableName: "Results_Deleted"),
                where: where);
            var column = new Rds.ResultsColumnCollection();
                column.ResultId();
                ss.Columns
                    .Where(o => o.TypeCs == "Attachments")
                    .Select(o => o.ColumnName)
                    .ForEach(columnName =>
                        column.Add($"\"{columnName}\""));
            var attachments = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    tableType: Sqls.TableTypes.Deleted,
                    column: column,
                    where: Rds.ResultsWhere()
                        .SiteId(ss.SiteId)
                        .ResultId_In(sub: sub)))
                .AsEnumerable()
                .Select(dataRow => new
                {
                    resultId = dataRow.Long("ResultId"),
                    attachments = dataRow
                        .Columns()
                        .Where(columnName => columnName.StartsWith("Attachments"))
                        .SelectMany(columnName => 
                            Jsons.Deserialize<IEnumerable<Attachment>>(dataRow.String(columnName)) 
                                ?? Enumerable.Empty<Attachment>())
                        .Where(o => o != null)
                        .Select(o => o.Guid)
                        .Concat(GetNotDeleteHistoryGuids(
                            context: context,
                            ss: ss,
                            resultId: dataRow.Long("ResultId")))
                        .Distinct()
                        .ToArray()
                })
                .Where(o => o.attachments.Length > 0);
            var guid = Strings.NewGuid();
            var itemsSub = Rds.SelectItems(
                tableType: Sqls.TableTypes.Deleted,
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere().ReferenceType(guid));
            var count = Repository.ExecuteScalar_response(
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
                    Rds.RestoreResults(
                        factory: context,
                        where: Rds.ResultsWhere()
                            .ResultId_In(sub: itemsSub)),
                    Rds.RowCount(),
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId_In(sub: itemsSub)
                            .BinaryType("Images")),
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid)),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid),
                        param: Rds.ItemsParam()
                            .ReferenceType(ss.ReferenceType))
                }).Count.ToInt();
            attachments.ForEach(o =>
            {
                RestoreAttachments(context, o.resultId, o.attachments);
            });    
            return count;
        }

        private static void RestoreAttachments(Context context, long resultId, IList<string> attachments)
        {
            var raw = $" ({string.Join(", ", attachments.Select(o => $"'{o}'"))}) ";
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                statements: new SqlStatement[] {
                    Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId(resultId)
                            .BinaryType("Attachments")
                            .Binaries_Guid(
                                _operator:" not in ",
                                raw: raw,
                                _using: attachments.Any())),
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .ReferenceId(resultId)
                            .BinaryType("Attachments")
                            .Binaries_Guid(
                                _operator: $" in ",
                                raw: raw),
                        _using: attachments.Any())
            }, transactional: true);
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long resultId)
        {
            if (!Parameters.History.Restore
                || ss.AllowRestoreHistories == false)
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
                default: return invalid.MessageJson(context: context);
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
                    .Ver(ver.First().ToInt())));
            resultModel.VerUp = true;
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    RestoreAttachments(
                        context: context,
                        resultId: resultModel.ResultId,
                        attachments: resultModel
                            .AttachmentsHash
                            .Values
                            .SelectMany(o => o.AsEnumerable())
                            .Select(o => o.Guid)
                            .Concat(GetNotDeleteHistoryGuids(
                                context: context,
                                ss: ss,
                                resultId: resultModel.ResultId))
                            .Distinct()
                            .ToArray());
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return new ResponseCollection(context: context)
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: resultId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static IEnumerable<string> GetNotDeleteHistoryGuids(
            Context context,
            SiteSettings ss,
            long resultId)
        {
            var ret = new List<string>();
            var sqlColumn = new SqlColumnCollection();
            ss.Columns
                ?.Where(column => column.ControlType == "Attachments")
                .Where(column => column?.NotDeleteExistHistory == true)
                .ForEach(column => sqlColumn.Add(column: column));
            if (sqlColumn.Any())
            {
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
                        tableType: Sqls.TableTypes.History,
                        column: sqlColumn,
                        where: Rds.ResultsWhere()
                            .SiteId(ss.SiteId)
                            .ResultId(resultId),
                        distinct: true))
                            .AsEnumerable();
                foreach (var dataRow in dataRows)
                {
                    foreach (DataColumn dataColumn in dataRow.Table.Columns)
                    {
                        var column = new ColumnNameInfo(dataColumn.ColumnName);
                        if (dataRow[column.ColumnName] != DBNull.Value)
                        {
                            dataRow[column.ColumnName].ToString().Deserialize<Attachments>()
                                ?.ForEach(attachment =>
                                    ret.Add(attachment.Guid));
                        }
                    }
                }
            }
            return ret.Distinct();
        }

        public static string Histories(
            Context context, SiteSettings ss, long resultId, Message message = null)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
            var columns = ss.GetHistoryColumns(context: context, checkPermission: true);
            if (!context.CanRead(ss: ss))
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            hb.Div(
                css: "tabs-panel-inner",
                action: () => hb
                    .HistoryCommands(context: context, ss: ss)
                    .GridTable(
                        css: "history",
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
                                    resultModel: resultModel))));
            return new ResultsResponseCollection(
                context: context,
                resultModel: resultModel)
                    .Html("#FieldSetHistories", hb)
                    .Message(message)
                    .Messages(context.Messages)
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
                column: HistoryColumn(
                    context: context,
                    ss: ss,
                    columns: columns
                        // GridDesignに含まれるカラムを追加する。
                        .Concat(ss.IncludedColumns()
                            .Select(columnName => ss.GetColumn(
                                context: context,
                                columnName: columnName))
                            .Where(column => column != null)
                            .AllowedColumns(
                                context: context,
                                ss: ss,
                                checkPermission: true)
                            .Where(o => context.ContractSettings.Attachments()
                                || o.ControlType != "Attachments"))
                        .Distinct()
                        .ToList()),
                join: ss.Join(context: context),
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
                                .DataLatest(
                                    value: 1,
                                    _using: resultModelHistory.Ver == resultModel.Ver),
                            action: () =>
                            {
                                resultModelHistory.SetChoiceHash(
                                    context: context,
                                    ss: ss);
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: resultModelHistory.Ver.ToString(),
                                            _using: resultModelHistory.Ver < resultModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        resultModel: resultModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(
            Context context,
            SiteSettings ss,
            List<Column> columns)
        {
            var sqlColumn = Rds.ResultsTitleColumn(
                context: context,
                ss: ss)
                    .ResultId()
                    .Ver();
            columns.ForEach(column =>
                sqlColumn.ResultsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
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
            var selectedWhere = SelectedWhere(
                context: context,
                ss: ss);
            if (selectedWhere == null)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var where = view.Where(
                context: context,
                ss: ss,
                where: selectedWhere,
                itemJoin: false);
            var param = view.Param(
                context: context,
                ss: ss);
            var invalid = ExistsLockedRecord(
                context: context,
                ss: ss,
                where: where,
                param: param,
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(
                context: context,
                siteId: siteId,
                number: Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn().ResultsCount(),
                        where: where,
                        param: param))))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var destination = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId,
                referenceId: siteId);
            if (destination.SiteId == 0)
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
            if (Permissions.CanMove(
                context: context,
                source: ss,
                destination: destination))
            {
                var count = BulkMove(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    where: where,
                    param: param);
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

        private static int BulkMove(
            Context context,
            SiteSettings ss,
            long siteId,
            SqlWhereCollection where,
            SqlParamCollection param)
        {
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                where: where,
                param: param);
            var guid = Strings.NewGuid();
            var siteIds = ss.GetIntegratedSites(context: context);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId_In(siteIds)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.UpdateResults(
                        where: where,
                        param: Rds.ResultsParam()
                            .SiteId(siteId)),
                    Rds.RowCount(),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .SiteId_In(siteIds)
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
                var selectedWhere = SelectedWhere(
                    context: context,
                    ss: ss);
                if (selectedWhere == null)
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None: break;
                    default: return invalid.MessageJson(context: context);
                }
                using var exclusiveObj = new Sessions.TableExclusive(context: context);
                if (!exclusiveObj.TryLock())
                {
                    return Error.Types.ImportLock.MessageJson(context: context);
                }
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    watchdog: () => exclusiveObj.Refresh());
                exclusiveObj.Refresh();
                Summaries.Synchronize(context: context, ss: ss);
                var data = new string[]
                {
                    ss.Title,
                    count.ToString()
                };
                ss.Notifications.ForEach(notification =>
                {
                    var body = new System.Text.StringBuilder();
                    body.Append(Locations.ItemIndexAbsoluteUri(
                        context: context,
                        ss.SiteId) + "\n");
                    body.Append(
                        $"{Displays.Results_Updator(context: context)}: ",
                        $"{context.User.Name}\n");
                    if (notification.AfterBulkDelete != false)
                    {
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.BulkDeleted(
                                context: context,
                                data: data),
                            body: body.ToString());
                    }
                });
                exclusiveObj.Refresh();
                return GridRows(
                    context: context,
                    ss: ss,
                    clearCheck: true,
                    message: Messages.BulkDeleted(
                        context: context,
                        data: data));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        public static int BulkDelete(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            SqlParamCollection param,
            Action watchdog = null)
        {
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        where
                    }),
                where: where,
                param: param);
            ss.LinkActions(
                context: context,
                type: "DeleteWithLinks",
                sub: sub);
            var sites = ss.IntegratedSites?.Any() == true
                ? ss.AllowedIntegratedSites
                : ss.SiteId.ToSingleList();
            var statements = new List<SqlStatement>();
            var guid = Strings.NewGuid();
            statements.OnBulkDeletingExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            statements.Add(Rds.UpdateItems(
                where: Rds.ItemsWhere()
                    .SiteId_In(sites)
                    .ReferenceId_In(sub: sub),
                param: Rds.ItemsParam()
                    .ReferenceType(guid)));
            statements.Add(Rds.DeleteBinaries(
                factory: context,
                where: Rds.BinariesWhere()
                    .TenantId(context.TenantId)
                    .ReferenceId_In(sub: sub)
                    .BinaryType(
                        value: "Images",
                        _operator: "<>",
                        _using: ss.DeleteImageWhenDeleting == false)));
            statements.Add(Rds.DeleteResults(
                factory: context,
                where: Rds.ResultsWhere()
                    .SiteId_In(sites)
                    .ResultId_In(sub: Rds.SelectItems(
                        column: Rds.ItemsColumn().ReferenceId(),
                        where: Rds.ItemsWhere()
                            .SiteId_In(sites)
                            .ReferenceType(guid)))));
            statements.Add(Rds.RowCount());
            statements.Add(Rds.DeleteItems(
                factory: context,
                where: Rds.ItemsWhere()
                    .SiteId_In(sites)
                    .ReferenceType(guid)));
            statements.Add(Rds.UpdateItems(
                tableType: Sqls.TableTypes.Deleted,
                where: Rds.ItemsWhere()
                    .SiteId_In(sites)
                    .ReferenceType(guid),
                param: Rds.ItemsParam()
                    .ReferenceType(ss.ReferenceType)));
            statements.OnBulkDeletedExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            watchdog?.Invoke();
            var ids = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().ReferenceId(),
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId_In(sub: sub)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("ReferenceId"))
                            .ToList();
            watchdog?.Invoke();
            var affectedRows = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray()).Count.ToInt();
            if (ss.DeleteImageWhenDeleting == false)
            {
                ids.ForEach(referenceId => BinaryUtilities.UpdateImageReferenceId(
                    context: context,
                    siteId: ss.SiteId,
                    referenceId: referenceId));
            }
            return affectedRows;
        }

        public static ContentResultInheritance BulkDeleteByApi(
            Context context,
            SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.CanDelete(ss: ss))
            {
                var recordSelector = context.RequestDataString.Deserialize<RecordSelector>();
                if (recordSelector == null)
                {
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
                }
                var selectedWhere = SelectedWhereByApi(
                    ss: ss,
                    recordSelector: recordSelector);
                if (selectedWhere == null && recordSelector.View == null)
                {
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
                }
                using var exclusiveObj = new Sessions.TableExclusive(context: context);
                if (!exclusiveObj.TryLock())
                {
                    return ApiResults.Get(new ApiResponse(
                        id: context.Id,
                        statusCode: 429,
                        message: Messages.ImportLock(context: context).Text));
                }
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        break;
                    default:
                        return ApiResults.Error(
                            context: context,
                            errorData: invalid);
                }
                exclusiveObj.Refresh();
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    watchdog: () => exclusiveObj.Refresh());
                exclusiveObj.Refresh();
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                var data = new string[]
                {
                    ss.Title,
                    count.ToString()
                };
                ss.Notifications.ForEach(notification =>
                    {
                        var body = new System.Text.StringBuilder();
                        body.Append(Locations.ItemIndexAbsoluteUri(
                            context: context,
                            ss.SiteId) + "\n");
                        body.Append(
                            $"{Displays.Results_Updator(context: context)}: ",
                            $"{context.User.Name}\n");
                        if (notification.AfterBulkDelete != false)
                        {
                            notification.Send(
                                context: context,
                                ss: ss,
                                title: Displays.BulkDeleted(
                                    context: context,
                                    data: data),
                                body: body.ToString());
                        }
                    });
                return ApiResults.Success(
                    id: context.SiteId,
                    limitPerDate: context.ContractSettings.ApiLimit(),
                    limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                    message: Displays.BulkDeleted(
                        context: context,
                        data: data));
            }
            else
            {
                return ApiResults.Get(ApiResponses.Forbidden(context: context));
            }
        }

        public static long BulkDeleteByServerScript(
            Context context,
            SiteSettings ss)
        {
            if (context.CanDelete(ss: ss))
            {
                var recordSelector = context.RequestDataString.Deserialize<RecordSelector>();
                if (recordSelector == null)
                {
                    return 0;
                }
                var selectedWhere = SelectedWhereByApi(
                    ss: ss,
                    recordSelector: recordSelector);
                if (selectedWhere == null && recordSelector.View == null)
                {
                    return 0;
                }
                using var exclusiveObj = new Sessions.TableExclusive(context: context);
                if (!exclusiveObj.TryLock())
                {
                    return 0;
                }
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        break;
                    default:
                        return 0;
                }
                exclusiveObj.Refresh();
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    watchdog: () => exclusiveObj.Refresh());
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                return count;
            }
            else
            {
                return 0;
            }
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId);
            var invalid = ResultValidators.OnDeleteHistory(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selector = new RecordSelector(context: context);
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

        private static int DeleteHistory(
            Context context,
            SiteSettings ss,
            long resultId,
            List<int> selected,
            bool negative = false)
        {
            return Repository.ExecuteScalar_response(
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
                                    where: new View()
                                        .Where(
                                            context: context,
                                            ss: ss)))),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string PhysicalBulkDelete(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new RecordSelector(context: context);
                var count = 0;
                if (selector.All)
                {
                    count = PhysicalBulkDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = PhysicalBulkDelete(
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
                    message: Messages.PhysicalBulkDeletedFromRecycleBin(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int PhysicalBulkDelete(
            Context context,
            SiteSettings ss,
            List<long> selected = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool negative = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Deleted)
        {
            var tableName = string.Empty;
            switch (tableType)
            {
                case Sqls.TableTypes.History:
                    tableName = "_History";
                    break;
                case Sqls.TableTypes.Deleted:
                    tableName = "_Deleted";
                    break;
                default:
                    break;
            }
            where = where ?? Rds.ResultsWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Results" + tableName)
                .ResultId_In(
                    value: selected,
                    tableName: "Results" + tableName,
                    negative: negative,
                    _using: selected.Any())
                .ResultId_In(
                    tableName: "Results" + tableName,
                    sub: Rds.SelectResults(
                        tableType: tableType,
                        column: Rds.ResultsColumn().ResultId(),
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
            var sub = Rds.SelectResults(
                tableType: tableType,
                _as: "Results" + tableName,
                column: Rds.ResultsColumn()
                    .ResultId(tableName: "Results" + tableName),
                where: where,
                param: param);
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    tableType: tableType,
                    column: Rds.BinariesColumn().Guid().BinaryType(),
                    where: Rds.BinariesWhere().ReferenceId_In(sub: sub)))
                        .AsEnumerable();
            var guid = Strings.NewGuid();
            var count = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateItems(
                        tableType: tableType,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceId_In(sub: sub),
                        param: Rds.ItemsParam()
                            .ReferenceType(guid)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: tableType,
                        where: Rds.BinariesWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteResults(
                        tableType: tableType,
                        where: where),
                    Rds.RowCount(),
                    Rds.PhysicalDeleteItems(
                        tableType: tableType,
                        where: Rds.ItemsWhere()
                            .SiteId(ss.SiteId)
                            .ReferenceType(guid)),
                }).Count.ToInt();
                if (tableType == Sqls.TableTypes.Deleted)
                {
                    BinaryUtilities.DeleteFromLocal(
                        context: context,
                        dataRows: dataRows);
                }
            return count;
        }

        public static ContentResultInheritance PhysicalBulkDeleteByApi(
            Context context,
            SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            if (context.CanManageSite(ss: ss))
            {
                var recordSelector = context.RequestDataString.Deserialize<RecordSelector>();
                if (recordSelector == null)
                {
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
                }
                var selectedWhere = SelectedWhereByApi(
                    ss: ss,
                    recordSelector: recordSelector);
                if (selectedWhere == null && recordSelector.View == null)
                {
                    return ApiResults.Get(ApiResponses.BadRequest(context: context));
                }
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        break;
                    default:
                        return ApiResults.Error(
                            context: context,
                            errorData: invalid);
                }
                var count = PhysicalBulkDelete(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    tableType: Sqls.TableTypes.Normal);
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                return ApiResults.Success(
                    id: context.SiteId,
                    limitPerDate: context.ContractSettings.ApiLimit(),
                    limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                    message: Displays.PhysicalBulkDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return ApiResults.Get(ApiResponses.Forbidden(context: context));
            }
        }

        public static long PhysicalBulkDeleteByServerScript(
            Context context,
            SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return 0;
            }
            if (context.CanManageSite(ss: ss))
            {
                var recordSelector = context.RequestDataString.Deserialize<RecordSelector>();
                if (recordSelector == null)
                {
                    return 0;
                }
                var selectedWhere = SelectedWhereByApi(
                    ss: ss,
                    recordSelector: recordSelector);
                if (selectedWhere == null && recordSelector.View == null)
                {
                    return 0;
                }
                using var exclusiveObj = new Sessions.TableExclusive(context: context);
                if (!exclusiveObj.TryLock())
                {
                    return 0;
                }
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None:
                        break;
                    default:
                        return 0;
                }
                var count = PhysicalBulkDelete(
                    context: context,
                    ss: ss,
                    where: where,
                    param: param,
                    tableType: Sqls.TableTypes.Normal);
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                return count;
            }
            else
            {
                return 0;
            }
        }

        public static string Import(Context context, SiteModel siteModel)
        {
            var updatableImport = context.Forms.Bool("UpdatableImport");
            var ss = siteModel.ResultsSiteSettings(
                context: context,
                referenceId: siteModel.SiteId,
                setAllChoices: true);
            var invalid = ResultValidators.OnImporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return Error.Types.ImportLock.MessageJson(context: context);
            }
            var res = new ResponseCollection(context: context);
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
            exclusiveObj.Refresh();
            var count = csv.Rows.Count();
            if (Parameters.General.ImportMax > 0 && Parameters.General.ImportMax < count)
            {
                return Error.Types.ImportMax.MessageJson(
                    context: context,
                    data: Parameters.General.ImportMax.ToString());
            }
            if (context.ContractSettings.ItemsLimit(
                context: context,
                siteId: ss.SiteId,
                number: count))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            if (csv != null && count > 0)
            {
                var columnHash = ImportUtilities.GetColumnHash(ss, csv);
                var idColumn = columnHash
                    .Where(o => o.Value.Column.ColumnName == "ResultId")
                    .Select(o => new { Id = o.Key })
                    .FirstOrDefault()?.Id ?? -1;
                if (updatableImport && idColumn > -1)
                {
                    var exists = ExistsLockedRecord(
                        context: context,
                        ss: ss,
                        targets: csv.Rows.Select(o => o[idColumn].ToLong()).ToList());
                    switch (exists.Type)
                    {
                        case Error.Types.None: break;
                        default: return exists.MessageJson(context: context);
                    }
                }
                var invalidColumn = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.Column.ColumnName));
                if (invalidColumn != null) return invalidColumn;
                ImportUtilities.SetOnImportingExtendedSqls(context, ss);
                var resultHash = new Dictionary<int, ResultModel>();
                var importKeyColumnName = context.Forms.Data("Key");
                var importKeyColumn = columnHash
                    .FirstOrDefault(column => column.Value.Column.ColumnName == importKeyColumnName);
                if (updatableImport && importKeyColumn.Value == null)
                {
                    return Messages.ResponseNotContainKeyColumn(context: context).ToJson();
                }
                var csvRows = csv.Rows
                    .Select((o, i) => new { Index = i, Row = o })
                    .ToDictionary(o => o.Index, o => o.Row);
                foreach (var data in csvRows)
                {
                    exclusiveObj.Refresh();
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss);
                    if (updatableImport
                        && !data.Value[importKeyColumn.Key].IsNullOrEmpty())
                    {
                        var view = new View();
                        view.AddColumnFilterHash(
                            context: context,
                            ss: ss,
                            column: importKeyColumn.Value.Column,
                            objectValue: data.Value[importKeyColumn.Key]);
                        view.AddColumnFilterSearchTypes(
                            columnName: importKeyColumnName,
                            searchType: Column.SearchTypes.ExactMatch);
                        var model = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: 0,
                            view: view);
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
                        }
                        else if (model.AccessStatus == Databases.AccessStatuses.Overlap)
                        {
                            return new ErrorData(
                                type: Error.Types.OverlapCsvImport,
                                data: new string[] {
                                    (data.Key + 1).ToString(),
                                    importKeyColumn.Value.Column.GridLabelText,
                                    data.Value[importKeyColumn.Key]
                                })
                                .MessageJson(context: context);
                        }
                    }
                    resultModel.SetByCsvRow(
                        context: context,
                        ss: ss,
                        columnHash: columnHash,
                        row: data.Value);
                    resultHash.Add(data.Key, resultModel);
                }
                var inputErrorData = ResultValidators.OnInputValidating(
                    context: context,
                    ss: ss,
                    resultHash: resultHash).FirstOrDefault();
                switch (inputErrorData.Type)
                {
                    case Error.Types.None: break;
                    default: return inputErrorData.MessageJson(context: context);
                }
                var insertCount = 0;
                var updateCount = 0;
                foreach (var data in resultHash)
                {
                    exclusiveObj.Refresh();
                    var resultModel = data.Value;
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        ErrorData errorData = null;
                        while (errorData?.Type != Error.Types.None)
                        {
                            switch (errorData?.Type)
                            {
                                case Error.Types.Duplicated:
                                    var duplicatedColumn = ss.GetColumn(
                                        context: context,
                                        columnName: errorData.ColumnName);
                                    if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                                    {
                                        return Messages.ResponseDuplicated(
                                            context: context,
                                            data: ss.GetColumn(
                                                context: context,
                                                columnName: errorData.ColumnName)?.LabelText)
                                                    .ToJson();
                                    }
                                    else
                                    {
                                        return new ResponseCollection(context: context).Message(
                                            message: new Message()
                                            {
                                                Id = "MessageWhenDuplicated",
                                                Text = duplicatedColumn.MessageWhenDuplicated,
                                                Css = "alert-error"
                                            }).ToJson();
                                    }
                                case null:
                                case Error.Types.UpdateConflicts:
                                    // 初回(null)
                                    // または更新の競合が発生した場合
                                    resultModel = new ResultModel(
                                        context: context,
                                        ss: ss,
                                        resultId: resultModel.ResultId);
                                    var previousTitle = resultModel.Title.DisplayValue;
                                    resultModel.SetByCsvRow(
                                        context: context,
                                        ss: ss,
                                        columnHash: columnHash,
                                        row: csvRows.Get(data.Key));
                                    switch (resultModel.AccessStatus)
                                    {
                                        case Databases.AccessStatuses.Selected:
                                            // 更新による競合のため再更新
                                            if (resultModel.Updated(context: context, ss: ss))
                                            {
                                                resultModel.VerUp = Versions.MustVerUp(
                                                    context: context,
                                                    ss: ss,
                                                    baseModel: resultModel);
                                                errorData = resultModel.Update(
                                                    context: context,
                                                    ss: ss,
                                                    extendedSqls: false,
                                                    previousTitle: previousTitle,
                                                    get: false);
                                                updateCount++;
                                            }
                                            else
                                            {
                                                errorData = new ErrorData(type: Error.Types.None);
                                            }
                                            break;
                                        case Databases.AccessStatuses.NotFound:
                                            // 削除による競合のため再作成
                                            resultModel.ResultId = 0;
                                            resultModel.Ver = 1;
                                            errorData = resultModel.Create(
                                                context: context,
                                                ss: ss,
                                                extendedSqls: false);
                                            insertCount++;
                                            break;
                                        default:
                                            return Messages.ResponseUpdateConflicts(context: context).ToJson();
                                    }
                                    break;
                                default:
                                    return errorData.MessageJson(context: context);
                            }
                        }
                    }
                    else
                    {
                        resultModel.ResultId = 0;
                        resultModel.Ver = 1;
                        var errorData = resultModel.Create(
                            context: context,
                            ss: ss,
                            extendedSqls: false);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            case Error.Types.Duplicated:
                                var duplicatedColumn = ss.GetColumn(
                                    context: context,
                                    columnName: errorData.ColumnName);
                                if (duplicatedColumn.MessageWhenDuplicated.IsNullOrEmpty())
                                {
                                    return Messages.ResponseDuplicated(
                                        context: context,
                                        data: ss.GetColumn(
                                            context: context,
                                            columnName: errorData.ColumnName)?.LabelText)
                                                .ToJson();
                                }
                                else
                                {
                                    return new ResponseCollection(context: context).Message(
                                        message: new Message()
                                        {
                                            Id = "MessageWhenDuplicated",
                                            Text = duplicatedColumn.MessageWhenDuplicated,
                                            Css = "alert-error"
                                        }).ToJson();
                                }
                            default:
                                return errorData.MessageJson(context: context);
                        }
                        insertCount++;
                    }
                }
                ImportUtilities.SetOnImportedExtendedSqls(context, ss);
                ss.Notifications.ForEach(notification =>
                {
                    var body = new System.Text.StringBuilder();
                    body.Append(Locations.ItemIndexAbsoluteUri(
                        context: context,
                        ss.SiteId) + "\n");
                    body.Append(
                        $"{Displays.Results_Updator(context: context)}: ",
                        $"{context.User.Name}\n");
                    if (notification.AfterImport != false)
                    {
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Imported(
                                context: context,
                                data: new string[]
                                {
                                    ss.Title,
                                    insertCount.ToString(),
                                    updateCount.ToString()
                                }),
                            body: body.ToString());
                    }
                });
                return GridRows(
                    context: context,
                    ss: ss,
                    windowScrollTop: true,
                    message: Messages.Imported(
                        context: context,
                        data: new string[]
                        {
                            ss.Title,
                            insertCount.ToString(),
                            updateCount.ToString()
                        }));
            }
            else
            {
                return Messages.ResponseFileNotFound(context: context).ToJson();
            }
        }

        public static ContentResultInheritance ImportByApi(
            Context context,
            SiteSettings ss,
            SiteModel siteModel)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType, multipart: true))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.Import == false)
            {
                return null;
            }
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 429,
                    message: Messages.ImportLock(context: context).Text));
            }
            var invalid = ResultValidators.OnImporting(
                context: context,
                ss: ss,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            var api = context.RequestDataString.Deserialize<ImportApi>();
            var updatableImport = api.UpdatableImport;
            var encoding = api.Encoding;
            var key = api.Key;
            Csv csv;
            try
            {
                csv = new Csv(
                    csv: context.PostedFiles.FirstOrDefault().Byte(),
                    encoding: encoding);
            }
            catch
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Messages.FailedReadFile(context: context).Text));
            }
            var count = csv.Rows.Count();
            if (Parameters.General.ImportMax > 0 && Parameters.General.ImportMax < count)
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Error.Types.ImportMax.Message(
                        context: context,
                        data: Parameters.General.ImportMax.ToString()).Text)); 
            }
            if (context.ContractSettings.ItemsLimit(
                context: context,
                siteId: ss.SiteId,
                number: count))
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Error.Types.ItemsLimit.Message(context: context).Text));
            }
            if (csv != null && count > 0)
            {
                var columnHash = ImportUtilities.GetColumnHash(ss, csv);
                var idColumn = columnHash
                    .Where(o => o.Value.Column.ColumnName == "ResultId")
                    .Select(o => new { Id = o.Key })
                    .FirstOrDefault()?.Id ?? -1;
                if (updatableImport && idColumn > -1)
                {
                    var exists = ExistsLockedRecord(
                        context: context,
                        ss: ss,
                        targets: csv.Rows.Select(o => o[idColumn].ToLong()).ToList());
                    switch (exists.Type)
                    {
                        case Error.Types.None: break;
                        default: return ApiResults.Error(
                            context: context,
                            errorData: exists);
                    }
                }
                var invalidColumn = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.Column.ColumnName));
                if (invalidColumn != null) return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: invalidColumn));
                ImportUtilities.SetOnImportingExtendedSqls(context, ss);
                var resultHash = new Dictionary<int, ResultModel>();
                var importKeyColumnName = key;
                var importKeyColumn = columnHash
                    .FirstOrDefault(column => column.Value.Column.ColumnName == importKeyColumnName);
                if (updatableImport && importKeyColumn.Value == null)
                {
                    return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Messages.NotContainKeyColumn(context: context).Text));
                }
                var csvRows = csv.Rows
                    .Select((o, i) => new { Index = i, Row = o })
                    .ToDictionary(o => o.Index, o => o.Row);
                foreach (var data in csvRows)
                {
                    exclusiveObj.Refresh();
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss);
                    if (updatableImport
                        && !data.Value[importKeyColumn.Key].IsNullOrEmpty())
                    {
                        var view = new View();
                        view.AddColumnFilterHash(
                            context: context,
                            ss: ss,
                            column: importKeyColumn.Value.Column,
                            objectValue: data.Value[importKeyColumn.Key]);
                        view.AddColumnFilterSearchTypes(
                            columnName: importKeyColumnName,
                            searchType: Column.SearchTypes.ExactMatch);
                        var model = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: 0,
                            view: view);
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
                        }
                        else if (model.AccessStatus == Databases.AccessStatuses.Overlap)
                        {
                            return ApiResults.Error(
                                context: context,
                                errorData: new ErrorData(
                                    type: Error.Types.OverlapCsvImport,
                                    data: new string[] {
                                        (data.Key + 1).ToString(),
                                        importKeyColumn.Value.Column.GridLabelText,
                                        data.Value[importKeyColumn.Key]}));
                        }
                    }
                    resultModel.SetByCsvRow(
                        context: context,
                        ss: ss,
                        columnHash: columnHash,
                        row: data.Value);
                    resultHash.Add(data.Key, resultModel);
                }
                var inputErrorData = ResultValidators.OnInputValidating(
                    context: context,
                    ss: ss,
                    resultHash: resultHash,
                    api: true).FirstOrDefault();
                switch (inputErrorData.Type)
                {
                    case Error.Types.None: break;
                    default: return ApiResults.Error(
                        context: context,
                        errorData: inputErrorData);
                }
                var insertCount = 0;
                var updateCount = 0;
                foreach (var data in resultHash)
                {
                    exclusiveObj.Refresh();
                    var resultModel = data.Value;
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        ErrorData errorData = null;
                        while (errorData?.Type != Error.Types.None)
                        {
                            switch (errorData?.Type)
                            {
                                case Error.Types.Duplicated:
                                    var duplicatedColumn = ss.GetColumn(
                                        context: context,
                                        columnName: errorData.ColumnName);
                                    return ApiResults.Duplicated(
                                        context: context,
                                        message: duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                                            ? Displays.Duplicated(
                                                context: context,
                                                data: duplicatedColumn?.LabelText)
                                            : duplicatedColumn?.MessageWhenDuplicated); 
                                case null:
                                case Error.Types.UpdateConflicts:
                                    // 初回(null)
                                    // または更新の競合が発生した場合
                                    resultModel = new ResultModel(
                                        context: context,
                                        ss: ss,
                                        resultId: resultModel.ResultId);
                                    var previousTitle = resultModel.Title.DisplayValue;
                                    resultModel.SetByCsvRow(
                                        context: context,
                                        ss: ss,
                                        columnHash: columnHash,
                                        row: csvRows.Get(data.Key));
                                    switch (resultModel.AccessStatus)
                                    {
                                        case Databases.AccessStatuses.Selected:
                                            // 更新による競合のため再更新
                                            if (resultModel.Updated(context: context, ss: ss))
                                            {
                                                resultModel.VerUp = Versions.MustVerUp(
                                                    context: context,
                                                    ss: ss,
                                                    baseModel: resultModel);
                                                errorData = resultModel.Update(
                                                    context: context,
                                                    ss: ss,
                                                    extendedSqls: false,
                                                    previousTitle: previousTitle,
                                                    get: false);
                                                updateCount++;
                                            }
                                            else
                                            {
                                                errorData = new ErrorData(type: Error.Types.None);
                                            }
                                            break;
                                        case Databases.AccessStatuses.NotFound:
                                            // 削除による競合のため再作成
                                            resultModel.ResultId = 0;
                                            resultModel.Ver = 1;
                                            errorData = resultModel.Create(
                                                context: context,
                                                ss: ss,
                                                extendedSqls: false);
                                            insertCount++;
                                            break;
                                        default:
                                            return ApiResults.Get(new ApiResponse(
                                                id: context.Id,
                                                statusCode: 500,
                                                message: Messages.UpdateConflicts(context: context).Text));
                                    }
                                    break;
                                default:
                                    return ApiResults.Error(
                                        context: context,
                                        errorData: errorData);
                            }
                        }
                    }
                    else
                    {
                        resultModel.ResultId = 0;
                        resultModel.Ver = 1;
                        var errorData = resultModel.Create(
                            context: context,
                            ss: ss,
                            extendedSqls: false);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            case Error.Types.Duplicated:
                                var duplicatedColumn = ss.GetColumn(
                                    context: context,
                                    columnName: errorData.ColumnName);
                                return ApiResults.Duplicated(
                                    context: context,
                                    message: duplicatedColumn?.MessageWhenDuplicated.IsNullOrEmpty() != false
                                        ? Displays.Duplicated(
                                            context: context,
                                            data: duplicatedColumn?.LabelText)
                                        : duplicatedColumn?.MessageWhenDuplicated);                                
                            default:
                                return ApiResults.Error(
                                    context: context,
                                    errorData: errorData);
                        }
                        insertCount++;
                    }
                }
                exclusiveObj.Refresh();
                ImportUtilities.SetOnImportedExtendedSqls(context, ss);
                ss.Notifications.ForEach(notification =>
                {
                    var body = new System.Text.StringBuilder();
                    body.Append(Locations.ItemIndexAbsoluteUri(
                        context: context,
                        ss.SiteId) + "\n");
                    body.Append(
                        $"{Displays.Results_Updator(context: context)}: ",
                        $"{context.User.Name}\n");
                    if (notification.AfterImport != false)
                    {
                        notification.Send(
                            context: context,
                            ss: ss,
                            title: Displays.Imported(
                                context: context,
                                data: new string[]
                                {
                                    ss.Title,
                                    insertCount.ToString(),
                                    updateCount.ToString()
                                }),
                            body: body.ToString());
                    }
                });
                return ApiResults.Success(
                    id: context.Id,
                    limitPerDate: context.ContractSettings.ApiLimit(),
                    limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                    message: Messages.Imported(
                        context: context,
                        data: new string[]
                        {
                            ss.Title,
                            insertCount.ToString(),
                            updateCount.ToString()
                        }).Text);
            }
            else
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Messages.FileNotFound(context: context).Text));
            }
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
                default: return invalid.MessageJson(context: context);
            }
            return new ResponseCollection(context: context)
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
                id: context.Forms.Int("ExportId"));
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
                    title: ss.Title,
                    name: export.Name,
                    extension: export.Type.ToString()),
                encoding: context.Forms.Data("ExportEncoding"));
        }

        public static string ExportAndMailNotify(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var invalid = ResultValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var export = ss.GetExport(
                    context: context,
                    id: context.Forms.Int("ExportId"));
                var fileName = ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
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
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: new SqlStatement[]
                        {
                            Rds.InsertBinaries(
                                param: Rds.BinariesParam()
                                    .TenantId(context.TenantId)
                                    .ReferenceId(ss.SiteId)
                                    .Guid(guid)
                                    .Title(fileName)
                                    .BinaryType("ExportData")
                                    .Bin(bytes)
                                    .FileName(fileName)
                                    .Extension(export.Type.ToString())
                                    .Size(bytes.Length)
                                    .ContentType(export.Type == Libraries.Settings.Export.Types.Csv
                                        ? "text/csv"
                                        : "application/json"))
                        });
                    var serverName = (Parameters.Service.AbsoluteUri == null)
                        ? context.Server
                        : System.Text.RegularExpressions.Regex.Replace(
                            Parameters.Service.AbsoluteUri.TrimEnd('/'),
                            $"{context.ApplicationPath.TrimEnd('/')}$",
                            string.Empty);
                    new OutgoingMailModel()
                    {
                        Title = new Title(Displays.ExportEmailTitle(
                            context: context,
                            data: new string[] { fileName })),
                        Body = Displays.ExportEmailBody(context: context) + "\n" +
                            $"{serverName}{Locations.DownloadFile(context: context, guid: guid)}",
                        From = Libraries.Mails.Addresses.SupportFrom(),
                        To = MailAddressUtilities.Get(
                            context: context,
                            userId: context.UserId),
                    }.Send(
                        context: context,
                        ss: ss);
                }
                catch(Exception e)
                {
                    new OutgoingMailModel()
                    {
                        Title = new Title(Displays.ExportEmailTitleFaild(context: context)),
                        Body = Displays.ExportEmailBodyFaild(context: context, fileName, e.Message),
                        From = Libraries.Mails.Addresses.SupportFrom(),
                        To = MailAddressUtilities.Get(context: context, context.UserId),
                    }.Send(context: context, ss);
                }
            });
            return Messages.ResponseExportAccepted(context: context).ToJson();
        }

        public static ContentResultInheritance ExportByApi(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = ResultValidators.OnExporting(
                context: context,
                ss: ss,
                api: true);
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
            using var exclusiveObj = new Sessions.TableExclusive(context: context);
            if (!exclusiveObj.TryLock())
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 429,
                    message: Messages.ImportLock(context: context).Text));
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
                    statusCode: 200,
                    limitPerDate: context.ContractSettings.ApiLimit(),
                    limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                    response: new
                    {
                        Name = ExportUtilities.FileName(
                            context: context,
                            title: ss.Title,
                            name: export.Name,
                            extension: export.Type.ToString()),
                        Content = content
                    });
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
                columnName: view.GetCrosstabValue(
                    context: context,
                    ss: ss));
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
                    title: ss.Title,
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
                siteId: view.GetCalendarSiteId(ss: ss));
            var timePeriod = view.GetCalendarTimePeriod(ss: ss);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss: ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss: ss));
            var date = view.GetCalendarDate();
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarGroupBy());
            var choices = groupBy?.EditChoices(
                context: context,
                insertBlank: true,
                view: view);
            var inRangeY = Libraries.ViewModes
                .CalendarUtilities.InRangeY(
                    context: context,
                    choices?.Count ?? 0);
            var calendarType = view.GetCalendarType(ss: ss);
            var begin = Calendars.BeginDate(
                context: context,
                ss: ss,
                date: date,
                timePeriod: timePeriod,
                view: view);
            var end = Calendars.EndDate(
                context: context,
                ss: ss,
                date: date,
                timePeriod: timePeriod,
                view: view);
            var CalendarViewType = view.GetCalendarViewType();
            var dataRows = inRangeY
                ? CalendarDataRows(
                    context: context,
                    ss: ss,
                    view: view,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    groupBy: groupBy,
                    begin: begin,
                    end: end)
                : null;
            var inRange = inRangeY
                && Libraries.ViewModes
                    .CalendarUtilities.InRange(
                        context: context,
                        dataRows: dataRows);
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            var suffix = view.GetCalendarSuffix();
            var calendarSiteId = view.GetCalendarSiteId(ss: ss);
            var calendarFromTo = view.GetCalendarFromTo(ss: ss);
            if (ss.DashboardParts?.Any() != true)
            {
                return hb.ViewModeTemplate(
                    context: context,
                    ss: ss,
                    view: view,
                    viewMode: viewMode,
                    serverScriptModelRow: serverScriptModelRow,
                    viewModeBody: () => hb.Calendar(
                        context: context,
                        ss: ss,
                        timePeriod: timePeriod,
                        groupBy: groupBy,
                        fromColumn: fromColumn,
                        toColumn: toColumn,
                        date: date,
                        siteId: calendarSiteId,
                        begin: begin,
                        end: end,
                        CalendarViewType: CalendarViewType,
                        choices: choices,
                        dataRows: dataRows,
                        bodyOnly: false,
                        showStatus: view.CalendarShowStatus == true,
                        inRange: inRange,
                        calendarType: calendarType,
                        suffix: suffix,
                        calendarFromTo: calendarFromTo));
            }
            else
            {
                return hb.Calendar(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    groupBy: groupBy,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    date: date,
                    siteId: calendarSiteId,
                    begin: begin,
                    end: end,
                    CalendarViewType: CalendarViewType,
                    choices: choices,
                    dataRows: dataRows,
                    bodyOnly: false,
                    showStatus: view.GetCalendarShowStatus(),
                    inRange: inRange,
                    calendarType: calendarType,
                    suffix: suffix,
                    calendarFromTo: calendarFromTo).ToString();
            }
        }

        public static string UpdateByCalendar(Context context, SiteSettings ss)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: ss.DashboardParts?.Any() == true
                    ? context.Forms.Long("EventId")
                    : context.Forms.Long("Id"),
                formData: context.Forms);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var updated = resultModel.Updated(context: context);
            if (updated)
            {
                resultModel.VerUp = Versions.MustVerUp(
                    context: context,
                    ss: ss,
                    baseModel: resultModel);
                resultModel.Update(
                    context: context,
                    ss: ss,
                    notice: true);
            }
            return CalendarJson(
                context: context,
                ss: ss,
                changedItemId: updated
                    ? resultModel.ResultId
                    : 0,
                update: true,
                message: updated
                    ? context.ErrorData.Type != Error.Types.None
                        ? context.ErrorData.Message(context: context)
                        : Messages.Updated(
                            context: context,
                            data: resultModel.Title.MessageDisplay(context: context))
                    : null);
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
            var bodyOnly = ss.DashboardParts?.Any() == true
                ? false
                : context.Forms.ControlId().StartsWith("Calendar");
            var timePeriod = view.GetCalendarTimePeriod(ss: ss);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss: ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss: ss));
            var date = view.GetCalendarDate();
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarGroupBy());
            var choices = groupBy?.EditChoices(
                context: context,
                insertBlank: true,
                view: view);
            var inRangeY = Libraries.ViewModes
                .CalendarUtilities.InRangeY(
                    context: context,
                    choices?.Count ?? 0);
            var calendarType = view.GetCalendarType(ss: ss);
            var begin = Calendars.BeginDate(
                context: context,
                ss: ss,
                date: date,
                timePeriod: timePeriod,
                view: view);
            var end = Calendars.EndDate(
                context: context,
                ss: ss,
                date: date,
                timePeriod: timePeriod,
                view: view);
            var calendarViewType = view.GetCalendarViewType();
            var dataRows = inRangeY 
                ? CalendarDataRows(
                    context: context,
                    ss: ss,
                    view: view,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    groupBy: groupBy,
                    begin: begin,
                    end: end)
                :null;
            var inRange = inRangeY
                && Libraries.ViewModes
                    .CalendarUtilities.InRange(
                        context: context,
                        dataRows: dataRows);
            var suffix = view.GetCalendarSuffix();
            var calendarSiteId = view.GetCalendarSiteId(ss: ss);
            var calendarFromTo = view.GetCalendarFromTo(ss: ss);
            var body = new HtmlBuilder().Calendar(
                context: context,
                ss: ss,
                timePeriod: timePeriod,
                groupBy: groupBy,
                fromColumn: fromColumn,
                toColumn: toColumn,
                date: date,
                siteId: calendarSiteId,
                begin: begin,
                end: end,
                CalendarViewType: calendarViewType,
                choices: choices,
                dataRows: dataRows,
                bodyOnly: bodyOnly,
                showStatus: view.GetCalendarShowStatus(),
                inRange: inRange,
                changedItemId: changedItemId,
                calendarType: calendarType,
                suffix: suffix,
                calendarFromTo: calendarFromTo);
            var CalendarBodyName = "";
            switch (calendarType)
            {
                case "Standard":
                    CalendarBodyName = "#CalendarBody";
                    break;
                case "FullCalendar":
                    CalendarBodyName = "#FullCalendarBody";
                    break;
                default:
                    CalendarBodyName = "";
                    break;
            }
            if (ss.DashboardParts?.Any() != true)
            {
                if (inRange)
                {
                    return new ResponseCollection(context: context)
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "setCalendar",
                            message: message,
                            loadScroll: update,
                            bodyOnly: bodyOnly,
                            bodySelector: CalendarBodyName,
                            body: body)
                        .Events("on_calendar_load")
                        .ToJson();
                }
                else
                {
                    return new ResponseCollection(context: context)
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: inRangeY
                                ? Messages.TooManyCases(
                                    context: context,
                                    data: Parameters.General.CalendarLimit.ToString())
                                : Messages.TooManyRowCases(
                                    context: context,
                                    data: Parameters.General.CalendarYLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: CalendarBodyName,
                            body: body)
                        .Events("on_calendar_load")
                        .ToJson();
                }
            }
            else
            {
                return body.ToString();
            }
        }

        private static EnumerableRowCollection<DataRow> CalendarDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column fromColumn,
            Column toColumn,
            Column groupBy,
            DateTime begin,
            DateTime end)
        {
            var where = ss.DashboardParts?.Any() == true
                ? ss.DashboardParts[0].View.Where(context: context, ss: ss)
                : new SqlWhereCollection();
            if (toColumn == null)
            {
                where.Add(
                    tableName: "Results",
                    raw: $"\"Results\".\"{fromColumn.ColumnName}\" between @Begin and @End");
            }
            else
            {
                where.Add(or: Rds.ResultsWhere()
                    .Add(raw: $"\"Results\".\"{fromColumn.ColumnName}\" between @Begin and @End")
                    .Add(raw: $"\"Results\".\"{toColumn.ColumnName}\" between @Begin and @End")
                    .Add(raw: $"\"Results\".\"{fromColumn.ColumnName}\"<=@Begin and \"Results\".\"{toColumn.ColumnName}\">=@End"));
            }
            where = view.Where(
                context: context,
                ss: ss,
                where: where);
            var param = view.Param(
                context: context,
                ss: ss);
            param.Add(new SqlParam()
            {
                VariableName = "Begin",
                Value = begin,
                NoCount = true
            });
            param.Add(new SqlParam()
            {
                VariableName = "End",
                Value = end,
                NoCount = true
            });
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsTitleColumn(
                        context: context,
                        ss: ss)
                            .ResultId(_as: "Id")
                            .SiteId(_as: "SiteId")
                            .Status()
                            .ResultsColumn(
                                columnName: fromColumn.ColumnName,
                                _as: "From")
                            .ResultsColumn(
                                columnName: toColumn?.ColumnName,
                                _as: "To")
                            .UpdatedTime()
                            .ItemTitle(ss.ReferenceType)
                            .Add(
                                column: groupBy,
                                function: Sqls.Functions.SingleColumn),
                    join: ss.Join(
                        context: context,
                        join: where),
                    where: where,
                    param: param))
                        .AsEnumerable();
        }

        private static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
            Column groupBy,
            Column fromColumn,
            Column toColumn,
            DateTime date,
            long siteId,
            DateTime begin,
            DateTime end,
            string CalendarViewType,
            Dictionary<string, ControlData> choices,
            EnumerableRowCollection<DataRow> dataRows,
            bool bodyOnly,
            bool showStatus,
            bool inRange,
            long changedItemId = 0,
            string calendarType = null,
            string suffix = null,
            string calendarFromTo = null)
        {
            return !bodyOnly
                ? hb.Calendar(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    groupBy: groupBy,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    date: date,
                    siteId: siteId,
                    begin: begin,
                    end: end,
                    CalendarViewType: CalendarViewType,
                    choices: choices,
                    dataRows: dataRows,
                    showStatus: showStatus,
                    inRange: inRange,
                    changedItemId: changedItemId,
                    calendarType: calendarType,
                    suffix: suffix,
                    calendarFromTo: calendarFromTo)
                : hb.CalendarBody(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    groupBy: groupBy,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    date: date,
                    siteId: siteId,
                    begin: begin,
                    end: end,
                    CalendarViewType: CalendarViewType,
                    choices: choices,
                    dataRows: dataRows,
                    showStatus: showStatus,
                    inRange: inRange,
                    changedItemId: changedItemId,
                    calendarType: calendarType,
                    suffix: suffix,
                    calendarFromTo: calendarFromTo);
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
            if (groupByX?.CanRead(
                    context: context,
                    ss: ss,
                    mine: null) == false
                        || groupByY?.CanRead(
                            context: context,
                            ss: ss,
                            mine: null) == false)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(
                    context: context,
                    ss: ss));
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
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
                        notShowZeroRows: view.CrosstabNotShowZeroRows == true,
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
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            if (groupByX?.CanRead(
                    context: context,
                    ss: ss,
                    mine: null) == false
                        || groupByY?.CanRead(
                            context: context,
                            ss: ss,
                            mine: null) == false)
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(
                    context: context,
                    ss: ss));
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
            if (inRangeX && inRangeY)
            {
                var body = !bodyOnly
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
                        notShowZeroRows: view.CrosstabNotShowZeroRows == true,
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
                        notShowZeroRows: view.CrosstabNotShowZeroRows == true,
                        dataRows: dataRows);
                return new ResponseCollection(context: context)
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "setCrosstab",
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: body)
                    .Events("on_crosstab_load")
                    .ToJson();
            }
            else
            {
                var body = !bodyOnly
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
                        notShowZeroRows: view.CrosstabNotShowZeroRows == true,
                        dataRows: dataRows,
                        inRange: false)
                    : new HtmlBuilder();
                return new ResponseCollection(context: context)
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
                        body: body)
                    .Events("on_crosstab_load")
                    .ToJson();
            }
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
                        context: context,
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
                var param = view.Param(
                    context: context,
                    ss: ss);
                var groupBy = Rds.ResultsGroupBy()
                    .WithItemTitle(
                        context: context,
                        ss: ss,
                        column: groupByX)
                    .WithItemTitle(
                        context: context,
                        ss: ss,
                        column: groupByY);
                dataRows = Repository.ExecuteTable(
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
                        param: param,
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
                var param = view.Param(
                    context: context,
                    ss: ss);
                var groupBy = Rds.ResultsGroupBy()
                    .Add(dateGroup)
                    .WithItemTitle(
                        context: context,
                        ss: ss,
                        column: groupByY);
                dataRows = Repository.ExecuteTable(
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
                        param: param,
                        groupBy: groupBy))
                            .AsEnumerable();
            }
            ss.SetChoiceHash(
                context: context,
                dataRows: dataRows);
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
                        context: context,
                        ss: ss,
                        column: groupByY)
                    .Add(
                        context: context,
                        column: value,
                        _as: "Value",
                        function: Sqls.Function(aggregateType));
            }
            else
            {
                columns.ForEach(column =>
                    self.Add(
                        context: context,
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
            var horizontalAxis = view.GetTimeSeriesHorizontalAxis(
                context: context,
                ss: ss);
            if (horizontalAxis == null)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.BadRequest));
            }
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
                viewModeBody: () => hb
                    .TimeSeries(
                        context: context,
                        ss: ss,
                        view: view,
                        horizontalAxis: horizontalAxis,
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
            var horizontalAxis = view.GetTimeSeriesHorizontalAxis(
                context: context,
                ss: ss);
            if (horizontalAxis == null)
            {
                return Messages.ResponseBadRequest(context: context).ToJson();
            }
            var bodyOnly = context.Forms.ControlId().StartsWith("TimeSeries");
            if (InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.TimeSeriesLimit))
            {
                var body = new HtmlBuilder().TimeSeries(
                    context: context,
                    ss: ss,
                    view: view,
                    horizontalAxis: horizontalAxis,
                    bodyOnly: bodyOnly,
                    inRange: true);
                return new ResponseCollection(context: context)
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "drawTimeSeries",
                        bodyOnly: bodyOnly,
                        bodySelector: "#TimeSeriesBody",
                        body: body)
                    .Events("on_timeseries_load")
                    .ToJson();
            }
            else
            {
                var body = new HtmlBuilder().TimeSeries(
                    context: context,
                    ss: ss,
                    view: view,
                    horizontalAxis: horizontalAxis,
                    bodyOnly: bodyOnly,
                    inRange: false);
                return new ResponseCollection(context: context)
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.TimeSeriesLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#TimeSeriesBody",
                        body: body)
                    .Events("on_timeseries_load")
                    .ToJson();
            }
        }

        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string horizontalAxis,
            bool bodyOnly,
            bool inRange)
        {
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesGroupBy(
                    context: context,
                    ss: ss));
            var aggregationType = view.GetTimeSeriesAggregationType(
                context: context,
                ss: ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesValue(
                    context: context,
                    ss: ss));
            var chartType = view.GetTimeSeriesChartType(
                context: context,
                ss: ss);
            var dataRows = TimeSeriesDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                value: value,
                horizontalAxis: horizontalAxis);
            if (groupBy == null)
            {
                return hb;
            }
            var historyHorizontalAxis = horizontalAxis == "Histories";
            return !bodyOnly
                ? hb.TimeSeries(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    chartType: chartType,
                    historyHorizontalAxis: historyHorizontalAxis,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.TimeSeriesBody(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    historyHorizontalAxis: historyHorizontalAxis,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column groupBy,
            Column value,
            string horizontalAxis)
        {
            if (groupBy != null && value != null)
            {
                var historyHorizontalAxis = horizontalAxis == "Histories";
                var column = Rds.ResultsColumn();
                if (historyHorizontalAxis)
                {
                    column.UpdatedTime(_as: "HorizontalAxis");
                }
                else
                {
                    column.ResultsColumn(
                        columnName: horizontalAxis,
                        _as: "HorizontalAxis");
                }
                column.ResultId(_as: "Id")
                    .Ver()
                    .Add(
                        context: context,
                        column: groupBy)
                    .Add(
                        context: context,
                        column: value);
                var where = view.Where(
                    context: context,
                    ss: ss);
                var param = view.Param(
                    context: context,
                    ss: ss);
                var join = ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        column,
                        where
                    });
                var dataRows = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
                        tableType: (historyHorizontalAxis
                            ? Sqls.TableTypes.NormalAndHistory
                            : Sqls.TableTypes.Normal),
                        column: column,
                        join: join,
                        where: historyHorizontalAxis
                            ? new Rds.ResultsWhereCollection().ResultId_In(sub: Rds.SelectResults(
                                column: Rds.ResultsColumn().ResultId(),
                                join: join,
                                where: where))
                            : where.Add(raw: $"\"Results\".\"{horizontalAxis}\" is not null"),
                        param: param))
                            .AsEnumerable();
                ss.SetChoiceHash(
                    context: context,
                    dataRows: dataRows);
                return dataRows;
            }
            else
            {
                return null;
            }
        }

        public static string OpenAnalyPartDialog(Context context, SiteSettings ss)
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
                default: return invalid.MessageJson(context: context);
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            return new ResponseCollection(context: context)
                .Html(
                    "#AnalyPartDialog",
                    new HtmlBuilder().AnalyPartDialog(
                        context: context,
                        ss: ss,
                        view: view,
                        analyPartId: context.Forms.Int("AnalyPartId")))
                .CloseDialog()
                .ToJson();
        }

        public static string Analy(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(
                context: context,
                name: "Analy"))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.HasNotPermission));
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
                viewModeBody: () => hb
                    .Analy(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: true));
        }

        public static string AnalyJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Analy"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Analy");
            var body = new HtmlBuilder().Analy(
                context: context,
                ss: ss,
                view: view,
                bodyOnly: bodyOnly,
                inRange: true);
            return new ResponseCollection(context: context)
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "drawAnaly",
                    bodyOnly: bodyOnly,
                    bodySelector: "#AnalyBody",
                    body: body)
                .Events("on_analy_load")
                .CloseDialog(_using: context.Forms.ControlId() == "AddAnalyPart")
                .ToJson();
        }

        private static HtmlBuilder Analy(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            bool inRange)
        {
            var analyDataList = new List<Libraries.ViewModes.AnalyData>();
            view.AnalyPartSettings?.ForEach(analyPart =>
            {
                var groupBy = ss.GetColumn(
                    context: context,
                    columnName: view.GetAnalyGroupBy(
                        context: context,
                        ss: ss,
                        value: analyPart.GroupBy));
                var aggregationTarget = ss.GetColumn(
                    context: context,
                    columnName: analyPart.AggregationTarget);
                var analyData = AnalyDat(
                    context: context,
                    ss: ss,
                    view: view,
                    analyPartSetting: analyPart,
                    groupBy: groupBy,
                    timePeriodValue: analyPart.TimePeriodValue,
                    timePeriod: analyPart.TimePeriod,
                    aggregationTarget: aggregationTarget);
                analyDataList.Add(analyData);
            });
            return !bodyOnly
                ? hb.Analy(
                    context: context,
                    ss: ss,
                    analyDataList: analyDataList,
                    inRange: inRange)
                : hb.AnalyBody(
                    context: context,
                    ss: ss,
                    analyDataList: analyDataList,
                    inRange: inRange);
        }

        private static Libraries.ViewModes.AnalyData AnalyDat(
            Context context,
            SiteSettings ss,
            View view,
            AnalyPartSetting analyPartSetting,
            Column groupBy,
            decimal timePeriodValue,
            string timePeriod,
            Column aggregationTarget)
        {
            if (groupBy != null)
            {
                var column = Rds.ResultsColumn()
                    .ResultsColumn(
                        columnName: Rds.IdColumn(tableName: "Results"),
                        _as: "Id")
                    .ResultsColumn(columnName: "Ver")
                    .ResultsColumn(
                        columnName: groupBy.ColumnName,
                        _as: "GroupBy");
                if (aggregationTarget != null)
                {
                    column.ResultsColumn(
                        columnName: aggregationTarget.ColumnName,
                        _as: "Value");
                }
                var where = view.Where(
                    context: context,
                    ss: ss);
                if (timePeriodValue > 0)
                {
                    where.Results_UpdatedTime(
                        value: DateTime.Now.DateAdd(
                            timePeriod: timePeriod,
                            timePeriodValue: (timePeriodValue * -1).ToInt()),
                        _operator: "<=");
                }
                var param = view.Param(
                    context: context,
                    ss: ss);
                var join = ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        column,
                        where
                    });
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectResults(
                            dataTableName: "Normal",
                            column: column,
                            join: join,
                            where: new Rds.ResultsWhereCollection()
                                .ResultId_In(sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    join: join,
                                    where: where)),
                            param: param),
                        Rds.SelectResults(
                            dataTableName: "History",
                            tableType: Sqls.TableTypes.History,
                            column: column,
                            join: join,
                            where: new Rds.ResultsWhereCollection()
                                .ResultId_In(sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    join: join,
                                    where: where))
                                .Ver(
                                    sub: Rds.SelectResults(
                                        tableType: Sqls.TableTypes.History,
                                        _as: "b",
                                        column: Rds.ResultsColumn().Ver(
                                            tableName: "b",
                                            function: Sqls.Functions.Max),
                                        where: Rds.ResultsWhere().ResultId(
                                            tableName: "b",
                                            raw: "\"Results\".\"ResultId\""),
                                        groupBy: Rds.ResultsGroupBy().ResultId(tableName: "b"))),
                            param: param,
                            _using: timePeriodValue > 0)
                    });
                return new Libraries.ViewModes.AnalyData(
                    analyPartSetting: analyPartSetting,
                    dataSet: dataSet);
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            if (ss.DashboardParts?.Any() != true)
            {
                return hb.ViewModeTemplate(
                    context: context,
                    ss: ss,
                    view: view,
                    viewMode: viewMode,
                    serverScriptModelRow: serverScriptModelRow,
                    viewModeBody: () => hb
                        .Kamban(
                            context: context,
                            ss: ss,
                            view: view,
                            bodyOnly: false,
                            inRange: inRange));
            }
            else
            {
                return hb.Kamban(
                    context: context,
                    ss: ss,
                    view: view,
                    bodyOnly: false,
                    inRange: inRange).ToString();
            }
        }

        public static string KambanJson(
            Context context,
            SiteSettings ss,
            bool updated = false)
        {
            if (!ss.EnableViewMode(context: context, name: "Kamban"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = ss.DashboardParts?.Any() == true
                ? false
                : context.Forms.ControlId().StartsWith("Kamban");
            var res = new ResponseCollection(context: context);
            var suffix = view.GetKambanSuffix();
            if (context.ErrorData.Type != Error.Types.None)
            {
                res.Message(context.ErrorData.Message(context: context));
            }
            if (ss.DashboardParts?.Any() != true)
            {
                if (InRange(
                    context: context,
                    ss: ss,
                    view: view,
                    limit: Parameters.General.KambanLimit))
                {
                    var body = new HtmlBuilder().Kamban(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: bodyOnly,
                        changedItemId: updated
                            ? context.Forms.Long("KambanId")
                            : 0);
                    return res
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "setKamban",
                            bodyOnly: bodyOnly,
                            bodySelector: $"#KambanBody{suffix}",
                            body: body,
                            replaceAllBody: true)
                        .Events("on_kamban_load")
                        .ToJson();
                }
                else
                {
                    var body = new HtmlBuilder().Kamban(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: bodyOnly,
                        inRange: false);
                    return res
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.KambanLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: $"#KambanBody{suffix}",
                            body: body,
                            replaceAllBody: true)
                        .Events("on_kamban_load")
                        .ToJson();
                }
            }
            else
            {
                var body = new HtmlBuilder().Kamban(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: bodyOnly,
                        changedItemId: updated
                            ? context.Forms.Long("KambanId")
                            : 0);
                return body.ToString();
            }
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
                columnName: view.GetKambanValue(
                    context: context,
                    ss: ss));
            var aggregationView = view.KambanAggregationView ?? false;
            var showStatus = view.KambanShowStatus ?? false;
            var columns = view.GetKambanColumns();
            var data = KambanData(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                value: value);
            var suffix = view.GetKambanSuffix();
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
                    showStatus: showStatus,
                    data: data,
                    inRange: inRange,
                    suffix: suffix,
                    changedItemId: changedItemId)
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
                    showStatus: showStatus,
                    data: data,
                    suffix: suffix,
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
                .SiteId()
                .Status()
                .ItemTitle(ss.ReferenceType)
                .Add(
                    context: context,
                    column: groupByX)
                .Add(
                    context: context,
                    column: groupByY)
                .Add(
                    context: context,
                    column: value);
            var where = ss.DashboardParts?.Any() == true
                ? ss.DashboardParts[0].View.Where(context: context, ss: ss)
                : new SqlWhereCollection();
            where = view.Where(
                context: context,
                ss: ss,
                where: where);
            var param = view.Param(
                context: context,
                ss: ss);
            return Repository.ExecuteTable(
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
                    where: where,
                    param: param))
                        .AsEnumerable()
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Long("ResultId"),
                            SiteId = o.Long("SiteId"),
                            Title = o.String("ItemTitle"),
                            Status = new Status(o.Int("Status")),
                            GroupX = groupByX?.ConvertIfUserColumn(o),
                            GroupY = groupByY?.ConvertIfUserColumn(o),
                            Value = o.Decimal(value?.ColumnName)
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
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var updated = resultModel.Updated(context: context);
            if (updated)
            {
                resultModel.VerUp = Versions.MustVerUp(
                    context: context,
                    ss: ss,
                    baseModel: resultModel);
                resultModel.Update(
                    context: context,
                    ss: ss,
                    notice: true);
            }
            return KambanJson(
                context: context,
                ss: ss,
                updated: updated);
        }

        public static (Plugins.PdfData pdfData, string error) Pdf(
            Context context,
            SiteSettings ss,
            long resultId,
            int reportId)
        {
            var invalid = ResultValidators.OnGet(
                context: context,
                ss: ss,
                api: false);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return (
                        null,
                        HtmlTemplates.Error(
                            context: context,
                            errorData: new ErrorData(type: invalid.Type)));
            }
            var extendedPlugin = Parameters.ExtendedPlugins
                .ExtensionWhere<ParameterAccessor.Parts.ExtendedPlugin>(
                    context: context,
                    siteId: ss.SiteId)
                .FirstOrDefault(o => o.PluginType == ParameterAccessor.Parts.ExtendedPlugin.PluginTypes.Pdf);
            if (extendedPlugin == null)
            {
                return (
                    null,
                    HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound)));
            }
            View view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false) ?? new View();
            SqlWhereCollection where = null;
            if (resultId > 0)
            {
                view = new View()
                {
                    GridColumns = view.GridColumns?.ToList(),
                    ColumnFilterHash = new Dictionary<string, string>()
                    {
                        { "ResultId", resultId.ToString() }
                    }
                };
            }
            else
            {
                where = SelectedWhere(
                    context: context,
                    ss: ss);
            }
            view.ApiColumnValueDisplayType = ApiColumn.ValueDisplayTypes.Text;
            var host = new Libraries.Pdf.PdfPluginHost(
                context: context,
                ss: ss,
                view: view,
                where: where,
                reportId: reportId);
            var plugin = Libraries.Pdf.PdfPluginCache.LoadPdfPlugin(extendedPlugin.LibraryPath);
            if (plugin == null)
            {
                return (
                    null,
                    HtmlTemplates.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotFound)));
            }
            return (plugin.CreatePdf(host), null);
        }

        public static string UnlockRecord(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                formData: context.Forms);
            var invalid = ResultValidators.OnUnlockRecord(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            resultModel.Timestamp = context.Forms.Get("Timestamp");
            resultModel.Locked = false;
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    ss.LockedRecordTime = null;
                    ss.LockedRecordUser = null;
                    return EditorResponse(
                        context: context,
                        ss: ss,
                        resultModel: resultModel)
                            .Message(Messages.UnlockedRecord(context: context))
                            .Messages(context.Messages)
                            .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: resultModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
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
            var serverScriptModelRow = ss.GetServerScriptModelRow(
                context: context,
                view: view);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                serverScriptModelRow: serverScriptModelRow,
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
            var body = new HtmlBuilder().ImageLib(
                context: context,
                ss: ss,
                view: view,
                bodyOnly: bodyOnly);
            return new ResponseCollection(context: context)
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setImageLib",
                    bodyOnly: bodyOnly,
                    bodySelector: "#ImageLibBody",
                    body: body)
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
            return (new ResponseCollection(context: context))
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
                            column: ss.GetColumn(
                                context: context,
                                columnName: link.ColumnName)))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel => resultModel
                    .SetTitle(context: context, ss: ss));
            }
        }

        private static bool InRange(Context context, SiteSettings ss, View view, int limit)
        {
            var where = view.Where(
                context: context,
                ss: ss);
            var param = view.Param(
                context: context,
                ss: ss);
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            where
                        }),
                    where: where,
                    param: param)) <= limit;
        }

        public static ErrorData ExistsLockedRecord(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            SqlParamCollection param,
            SqlOrderByCollection orderBy,
            bool lockedColumn = false)
        {
            var lockedRecordWhere = new Rds.ResultsWhereCollection()
                .Results_Locked(true);
            lockedRecordWhere.AddRange(where);
            if (lockedColumn)
            {
                if (context.HasPrivilege)
                {
                    return new ErrorData(type: Error.Types.None);
                }
                else
                {    
                    lockedRecordWhere.Results_Updator(
                        value: context.UserId,
                        _operator: "<>");
                }
            }
            var resultId = Repository.ExecuteScalar_long(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    join: ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            where,
                            orderBy
                        }),
                    where: lockedRecordWhere,
                    param: param,
                    orderBy: orderBy,
                    top: 1));
            return resultId > 0
                ? ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    resultId: resultId)
                : new ErrorData(type: Error.Types.None);
        }

        private static ErrorData ExistsLockedRecord(
            Context context,
            SiteSettings ss,
            List<long> targets)
        {
            var data = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    where: Rds.ResultsWhere()
                        .SiteId(ss.SiteId)
                        .Locked(true)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("ResultId"))
                            .ToList();
            var resultId = data.FirstOrDefault(id => targets.Contains(id));
            return resultId > 0
                ? ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    resultId: resultId)
                : new ErrorData(type: Error.Types.None);
        }

        private static ErrorData ExistsLockedRecord(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId);
            return new ErrorData(
                type: Error.Types.LockedRecord,
                data: new string[]
                {
                    resultModel.ResultId.ToString(),
                    resultModel.Updator.Name,
                    resultModel.UpdatedTime.DisplayValue.ToString(context.CultureInfo())
                });
        }

        public static string ReplaceLineByResultModel(
            this ResultModel resultModel,
            Context context,
            SiteSettings ss,
            string line,
            string itemTitle,
            bool checkColumnAccessControl = false)
        {
            foreach (var column in ss.IncludedColumns(line))
            {
                var allowed = checkColumnAccessControl == false
                    || ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: resultModel.Mine(context: context));
                if (!allowed)
                {
                    line = line.Replace($"[{column.Name}]", string.Empty);
                    continue;
                }
                switch (column.ColumnName)
                {
                    case "Title":
                        line = line.Replace("[Title]", itemTitle);
                        break;
                    case "SiteId":
                        line = line.Replace(
                            "[SiteId]", resultModel.SiteId.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "UpdatedTime":
                        line = line.Replace(
                            "[UpdatedTime]", resultModel.UpdatedTime.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "ResultId":
                        line = line.Replace(
                            "[ResultId]", resultModel.ResultId.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Ver":
                        line = line.Replace(
                            "[Ver]", resultModel.Ver.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Body":
                        line = line.Replace(
                            "[Body]", resultModel.Body.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Status":
                        line = line.Replace(
                            "[Status]", resultModel.Status.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Manager":
                        line = line.Replace(
                            "[Manager]", resultModel.Manager.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Owner":
                        line = line.Replace(
                            "[Owner]", resultModel.Owner.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Locked":
                        line = line.Replace(
                            "[Locked]", resultModel.Locked.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Comments":
                        line = line.Replace(
                            "[Comments]", resultModel.Comments.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Creator":
                        line = line.Replace(
                            "[Creator]", resultModel.Creator.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Updator":
                        line = line.Replace(
                            "[Updator]", resultModel.Updator.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "CreatedTime":
                        line = line.Replace(
                            "[CreatedTime]", resultModel.CreatedTime.ToExport(
                                context: context,
                                column: column));
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                line = line.Replace(
                                    $"[{column.Name}]", resultModel.GetClass(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Num":
                                line = line.Replace(
                                    $"[{column.Name}]", resultModel.GetNum(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Date":
                                line = line.Replace(
                                    $"[{column.Name}]", resultModel.GetDate(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Description":
                                line = line.Replace(
                                    $"[{column.Name}]", resultModel.GetDescription(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Check":
                                line = line.Replace(
                                    $"[{column.Name}]", resultModel.GetCheck(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                        }
                        break;
                }
            }
            return line;
        }
    }
}
