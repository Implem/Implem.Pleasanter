﻿using Implem.DefinitionAccessor;
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
            var scriptValues = new ItemModel().SetByBeforeOpeningPageServerScript(
                context: context,
                ss: ss);
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
                scriptValues: scriptValues,
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
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                view: view,
                                verType: Versions.VerTypes.Latest,
                                backButton: !context.Publish)
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
                        .Id("ExportSitePackageDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSitePackage(context: context)))
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
                .Log(context.GetLog())
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
            var checkRow = ss.CheckRow(
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
                        recordSelector: null,
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
                ss.GetEditorColumnNames(
                    context: context,
                    columnOnly: true)
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
                    .Log(context.GetLog())
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
                            recordSelector: null,
                            editRow: true,
                            checkRow: false,
                            idColumn: "ResultId"))
                    .Log(context.GetLog())
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
            ResultModel resultModel,
            int? tabIndex = null,
            ServerScriptModelColumn serverScriptValues = null)
        {
            if (serverScriptValues?.RawText.IsNullOrEmpty() == false)
            {
                return hb.Td(
                    context: context,
                    column: column,
                    action: () => hb.Raw(serverScriptValues?.RawText),
                    tabIndex: tabIndex,
                    serverScriptValues: serverScriptValues);
            }
            else if (!column.GridDesign.IsNullOrEmpty())
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
                                    value: resultModel.SiteId,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.UpdatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.ResultId,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Title,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Body,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.TitleBody,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Status,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Manager,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Owner,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
                    case "Locked":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Locked,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.SiteTitle,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Comments,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Creator,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.Updator,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                    value: resultModel.CreatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Class(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Num(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Date(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Description(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Check(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
                                            value: resultModel.Attachments(columnName: column.Name),
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex,
                                            serverScriptValues: serverScriptValues);
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
            var scriptValues = resultModel.SetByBeforeOpeningPageServerScript(
                context: context,
                ss: ss);
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
                    view: null,
                    verType: resultModel.VerType,
                    methodType: resultModel.MethodType,
                    siteId: resultModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Results",
                    title: resultModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : resultModel.Title.DisplayValue,
                    body: resultModel.Body,
                    useTitle: ss.TitleColumns?.Any(o => ss
                        .GetEditorColumnNames()
                        .Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: resultModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: resultModel.MethodType),
                    scriptValues: scriptValues,
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            resultModel: resultModel)
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
            return ss.Tabs?.Any() != true
                ? hb.FieldSetGeneral(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    editInDialog: editInDialog)
                : hb.Div(
                    id: "EditorTabsContainer",
                    css: "max",
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

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(
                    context: context,
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
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: tabsCss,
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
                                    readOnly: resultModel.ReadOnly,
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
                            _using: !context.Ajax)
                        .Hidden(
                            controlId: "TriggerRelatingColumns_Editor", 
                            value: Jsons.ToJson(ss.RelatingColumns)))
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
                        && !editInDialog,
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
            DataSet dataSet = null,
            List<Link> links = null,
            bool editInDialog = false)
        {
            var mine = resultModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
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
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkId: resultModel.ResultId,
                                methodType: resultModel.MethodType,
                                links: links))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId,
                                dataSet: dataSet));
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
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
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
                    serverScriptModelColumns: resultModel
                        ?.ServerScriptModelRows
                        ?.Select(row => row.Columns.Get(column.ColumnName))
                        .ToArray(),
                    methodType: resultModel.MethodType,
                    value: value,
                    columnPermissionType: column.ColumnPermissionType(
                        context: context,
                        baseModel: resultModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
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
                hb.FieldSet(
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
                case "Locked":
                    return resultModel.Locked
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
                context, ss, resultId,
                formData: context.QueryStrings.Bool("control-auto-postback") ? context.Forms : null)).ToJson();
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
                    .Log(context.GetLog())
                : new ResultsResponseCollection(resultModel)
                    .Response("id", resultModel.ResultId.ToString())
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, resultModel))
                    .Val("#Id", resultModel.ResultId.ToString())
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Invoke("initRelatingColumnEditor")
                    .Message(message)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"))
                    .Events("on_editor_load")
                    .Log(context.GetLog());
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
            var switchTargets = new List<long>();
            if (Parameters.General.SwitchTargetsLimit > 0)
            {
                if (Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn().ResultsCount(),
                        join: join,
                        where: where)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultId(),
                            join: join,
                            where: where,
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
            var mine = resultModel.Mine(context: context);
            ss.GetEditorColumnNames()
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
                        case "Locked":
                            res.Val(
                                "#Results_Locked" + idSuffix,
                                resultModel.Locked);
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
                                                fieldCss: column.FieldCss
                                                    + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                                        ? " right-align"
                                                        : string.Empty),
                                                fieldDescription: column.Description,
                                                labelText: column.LabelText,
                                                value: resultModel.Attachments(columnName: column.Name).ToJson(),
                                                readOnly: column.ColumnPermissionType(
                                                    context: context,
                                                    baseModel: resultModel)
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
            var view = api?.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    tableBracket: "\"Items\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Results\".\"ResultId\"=\"Results_Items\".\"ReferenceId\"",
                    _as: "Results_Items")),
                where: view.Where(context: context, ss: ss),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api?.Offset ?? 0,
                pageSize: pageSize,
                tableType: tableType);
            SiteUtilities.UpdateApiCount(context, ss);
            return ApiResults.Get(
                statusCode: 200,
                limitPerDate: Parameters.Api.LimitPerSite,
                limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
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

        public static System.Web.Mvc.ContentResult GetByApi(
            Context context, SiteSettings ss, long resultId, bool internalRequest)
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
            SiteUtilities.UpdateApiCount(context, ss);
            return ApiResults.Get(
                statusCode: 200,
                limitPerDate: Parameters.Api.LimitPerSite,
                limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                response: new
                {
                    Data = resultModel.GetByApi(
                        context: context,
                        ss: ss).ToSingleList()
                });
        }

        public static ResultModel[] GetByServerScript(
            Context context,
            SiteSettings ss,
            bool internalRequest)
        {
            var invalid = ResultValidators.OnEntry(
                context: context,
                ss: ss,
                api: !internalRequest);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            var api = context.RequestDataString.Deserialize<Api>();
            var view = api?.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    tableBracket: "\"Items\"",
                    joinType: SqlJoin.JoinTypes.Inner,
                    joinExpression: "\"Results\".\"ResultId\"=\"Results_Items\".\"ReferenceId\"",
                    _as: "Results_Items")),
                where: view.Where(context: context, ss: ss),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api?.Offset ?? 0,
                pageSize: pageSize,
                tableType: tableType);
            SiteUtilities.UpdateApiCount(context, ss);
            return resultCollection.ToArray();
        }

        public static ResultModel GetByServerScript(
            Context context,
            SiteSettings ss,
            long resultId,
            bool internalRequest)
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
                api: !internalRequest);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
            SiteUtilities.UpdateApiCount(context, ss);
            return resultModel;
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
                default: return invalid.MessageJson(context: context);
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
                                : resultModel.ResultId)
                                    + "?new=1"
                                    + (ss.Columns.Any(o => o.Linking)
                                        && context.Forms.Long("FromTabIndex") > 0
                                            ? $"&TabIndex={context.Forms.Long("FromTabIndex")}"
                                            : string.Empty))
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: errorData.ColumnName)?.LabelText)
                                .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult CreateByApi(Context context, SiteSettings ss)
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
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return ApiResults.Success(
                        id: resultModel.ResultId,
                        limitPerDate: Parameters.Api.LimitPerSite,
                        limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                        message: Displays.Created(
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

        public static bool CreateByServerScript(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return false;
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
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
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
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle,
                permissions: context.Forms.List("CurrentPermissionsAll"),
                permissionChanged: context.Forms.Exists("CurrentPermissionsAll"));
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new ResultsResponseCollection(resultModel);
                    switch (Parameters.General.UpdateResponseType)
                    {
                        case 1:
                            return ResponseByUpdate(res, context, ss, resultModel)
                                .PrependComment(
                                    context: context,
                                    ss: ss,
                                    column: ss.GetColumn(context: context, columnName: "Comments"),
                                    comments: resultModel.Comments,
                                    verType: resultModel.VerType)
                                .ToJson();
                        default:
                            return ResponseByUpdate(res, context, ss, resultModel)
                                .ToJson();
                    }
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
                    return errorData.MessageJson(context: context);
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
                            recordSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: resultModel.Title.DisplayValue));
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
                        .Message(Messages.Updated(
                            context: context,
                            data: resultModel.Title.DisplayValue))
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
                            .Html("#HeaderTitle", HttpUtility.HtmlEncode(resultModel.Title.DisplayValue))
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
                    default:
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            message: Messages.Updated(
                                context: context,
                                data: resultModel.Title.DisplayValue));
                }
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
            ss.SetColumnAccessControls(context: context);
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
                                resultModel: new ResultModel(),
                                column: column,
                                alwaysSend: true,
                                disableSection: true)))
                .ToJson();
        }

        public static string BulkUpdateSelectChanged(Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
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
                        resultModel: new ResultModel(),
                        column: column,
                        alwaysSend: true,
                        disableSection: true))
                .ToJson();
        }

        public static string BulkUpdate(Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var param = new Rds.ResultsParamCollection();
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("BulkUpdateColumnName"));
            ss.SetColumnAccessControls(context: context);
            if (!column.CanUpdate)
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                formData: context.Forms);
            resultModel.PropertyValue(
                context: context,
                column: column);
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
            var invalid = ExistsLockedRecord(
                context: context,
                ss: ss,
                where: where,
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                bulkUpdateColumn: column);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var count = BulkUpdate(
                context: context,
                ss: ss,
                where: where);
            Summaries.Synchronize(context: context, ss: ss);
            ss.Notifications.ForEach(notification =>
            {
                var body = new Dictionary<string, string>();
                body.Add(
                    Displays.Results_Updator(context: context),
                    context.User.Name);
                body.Add(
                    Displays.Column(context: context),
                    column.LabelText);
                body.Add(
                    Displays.Value(context: context),
                    resultModel.PropertyValue(
                          context: context,
                          column: column));
                notification.Send(
                    context: context,
                    ss: ss,
                    title: Displays.BulkUpdated(
                        context: context,
                        data: count.ToString()).ToString(),
                    body: Locations.ItemIndex(
                        context: context,
                        ss.SiteId)
                            + body.Select(o => o.Key + ":" + o.Value).Join("\n"));
            });
            return GridRows(
                context: context,
                ss: ss,
                clearCheck: true,
                message: Messages.BulkUpdated(
                    context: context,
                    data: count.ToString()));
        }

        private static int BulkUpdate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where)
        {
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        where
                    }),
                where: where);
            var verUpWhere = VerUpWhere(
                context: context,
                ss: ss, 
                sub: sub);
            var column = ss.GetColumn(
                context: context,
                columnName: context.Forms.Data("BulkUpdateColumnName"));
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                formData: context.Forms);
                resultModel.PropertyValue(
                    context: context,
                    column: column);
            var statements = new List<SqlStatement>();
            statements.OnBulkUpdatingExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            statements.Add(Rds.ResultsCopyToStatement(
                where: verUpWhere,
                tableType: Sqls.TableTypes.History,
                resultModel.ColumnNames()));
            statements.Add(Rds.UpdateResults(
                where: verUpWhere,
                param: Rds.ResultsParam().Ver(raw: "\"Ver\"+1"),
                addUpdatorParam: false,
                addUpdatedTimeParam: false));
            var param = new Rds.ResultsParamCollection();
            switch (column.ColumnName)
            {
                case "Title":
                    param.Title(resultModel.Title.Value.MaxLength(1024));
                    break;
                case "Body":
                    param.Body(resultModel.Body);
                    break;
                case "Status":
                    param.Status(resultModel.Status.Value);
                    break;
                case "Manager":
                    param.Manager(resultModel.Manager.Id);
                    break;
                case "Owner":
                    param.Owner(resultModel.Owner.Id);
                    break;
                case "Locked":
                    param.Locked(resultModel.Locked);
                    break;
                default:
                    var columnNameBracket = $"\"{column.ColumnName}\"";
                    switch (Def.ExtendedColumnTypes.Get(column.ColumnName))
                    {
                        case "Class":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: resultModel.Class(column.ColumnName).MaxLength(1024));
                            break;
                        case "Num":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: resultModel.Num(column.ColumnName).Value);
                            break;
                        case "Date":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: resultModel.Date(column.ColumnName));
                            break;
                        case "Check":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: resultModel.Check(column.ColumnName));
                            break;
                        case "Description":
                            param.Add(
                                columnBracket: columnNameBracket,
                                name: column.ColumnName,
                                value: resultModel.Description(column.ColumnName));
                            break;
                    }
                    break;
            }
            statements.Add(Rds.UpdateResults(
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(sub: sub),
                param: param));
            statements.Add(Rds.RowCount());
            statements.OnBulkUpdatedExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        private static SqlWhereCollection VerUpWhere(
            Context context, SiteSettings ss, SqlSelect sub)
        {
            var where = Rds.ResultsWhere()
                .SiteId(ss.SiteId)
                .ResultId_In(sub: sub);
            switch (ss.AutoVerUpType)
            {
                case Versions.AutoVerUpTypes.Always:
                    return where;
                case Versions.AutoVerUpTypes.Disabled:
                    return where.Add(raw: "0=1");
                default:
                    return where.Add(or: Rds.ResultsWhere()
                        .Updator(context.UserId, _operator: "<>")
                        .UpdatedTime(
                            DateTime.Today.ToDateTime().ToUniversal(context: context),
                            _operator: "<"));
            }
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
            statements.OnUpdatingByGridExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
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
            var updatedModels = new List<BaseItemModel>();
            var createdModels = new List<BaseItemModel>();
            foreach (var formData in formDataSet)
            {
                var resultModel = resultCollection
                    .FirstOrDefault(o => o.ResultId == formData.Id);
                if (resultModel != null)
                {
                    resultModel.SetByBeforeUpdateServerScript(
                        context: context,
                        ss: ss);
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
                        default: return invalid.MessageJson(context: context);
                    }
                    statements.AddRange(resultModel.UpdateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                    updatedModels.Add(resultModel);
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
                        default: return invalid.MessageJson(context: context);
                    }
                    statements.AddRange(resultModel.CreateStatements(
                        context: context,
                        ss: ss,
                        dataTableName: formData.Id.ToString()));
                    createdModels.Add(resultModel);
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
            statements.OnUpdatedByGridExtendedSqls(
                context: context,
                siteId: ss.SiteId);
            var responses = Repository.ExecuteDataSet_responses(
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
                    createdModels.ForEach(model => model.SetByAfterCreateServerScript(
                        context: context,
                        ss: ss));
                    updatedModels.ForEach(model => model.SetByAfterUpdateServerScript(
                        context: context,
                        ss: ss));
                    return UpdateByGridSuccess(
                        context: context,
                        ss: ss,
                        formDataSet: formDataSet,
                        resultCollection: resultCollection,
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
                .ToJson();
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(
            Context context,
            SiteSettings ss,
            long resultId,
            string previousTitle)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
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
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return ApiResults.Success(
                        resultModel.ResultId,
                        limitPerDate: Parameters.Api.LimitPerSite,
                        limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                        message: Displays.Updated(
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

        public static bool UpdateByServerScript(
            Context context,
            SiteSettings ss,
            long resultId,
            string previousTitle)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                setByApi: true);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
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
            var errorData = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                previousTitle: previousTitle);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
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
                default: return invalid.MessageJson(context: context);
            }
            resultModel.ResultId = 0;
            if (ss.GetEditorColumnNames().Contains("Title"))
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
                            data: resultModel.Title.Value));
                    var res = new ResultsResponseCollection(resultModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult DeleteByApi(
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
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return ApiResults.Success(
                        id: resultModel.ResultId,
                        limitPerDate: Parameters.Api.LimitPerSite,
                        limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                        message: Displays.Deleted(
                            context: context,
                            data: resultModel.Title.DisplayValue));
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
                api: true);
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
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
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
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
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
                        .Distinct()
                        .ToArray()
                })
                .Where(o => o.attachments.Length > 0);
            var guid = Strings.NewGuid();
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
                        where: where),
                    Rds.RowCount(),
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
                            .Distinct().ToArray());
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: resultId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
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
            var invalid = ExistsLockedRecord(
                context: context,
                ss: ss,
                where: where,
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
                        where: where))))
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
                    where: where);
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
            SqlWhereCollection where)
        {
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                where: where);
            var guid = Strings.NewGuid();
            return Repository.ExecuteScalar_response(
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
                    Rds.UpdateResults(
                        where: where,
                        param: Rds.ResultsParam()
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
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
                    orderBy: view.OrderBy(
                        context: context,
                        ss: ss));
                switch (invalid.Type)
                {
                    case Error.Types.None: break;
                    default: return invalid.MessageJson(context: context);
                }
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where);
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
                    join: new IJoin[]
                    {
                        where
                    }),
                where: where);
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
                    .ReferenceId_In(sub: sub)));
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
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static System.Web.Mvc.ContentResult BulkDeleteByApi(
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
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
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
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where);
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                return ApiResults.Success(
                    id: context.SiteId,
                    limitPerDate: Parameters.Api.LimitPerSite,
                    limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                    message: Displays.BulkDeleted(
                        context: context,
                        data: count.ToString()));
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
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
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
                var count = BulkDelete(
                    context: context,
                    ss: ss,
                    where: where);
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
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
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
                                    where: Views.GetBySession(
                                        context: context,
                                        ss: ss).Where(
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
                where: where);
            var guid = Strings.NewGuid();
            return Repository.ExecuteScalar_response(
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
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
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
        }

        public static System.Web.Mvc.ContentResult PhysicalBulkDeleteByApi(
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
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
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
                    tableType: Sqls.TableTypes.Normal);
                Summaries.Synchronize(
                    context: context,
                    ss: ss);
                return ApiResults.Success(
                    id: context.SiteId,
                    limitPerDate: Parameters.Api.LimitPerSite,
                    limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
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
                var view = recordSelector.View ?? Views.GetBySession(
                    context: context,
                    ss: ss);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: selectedWhere,
                    itemJoin: false);
                var invalid = ExistsLockedRecord(
                    context: context,
                    ss: ss,
                    where: where,
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
            ss.SetColumnAccessControls(context: context);
            var invalid = ResultValidators.OnImporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
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
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(
                            context: context,
                            siteId: ss.SiteId)
                                .ToArray());
                var resultHash = new Dictionary<int, ResultModel>();
                var previousTitle = string.Empty;
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss);
                    if (updatableImport && idColumn > -1)
                    {
                        var model = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: data.Row.Count > idColumn
                                ? data.Row[idColumn].ToLong()
                                : 0);
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
                        }
                    }
                    previousTitle = resultModel.Title.DisplayValue;
                    columnHash
                        .Where(column => (column.Value.Column.CanCreate && resultModel.ResultId == 0)
                            || (column.Value.Column.CanUpdate && resultModel.ResultId > 0))
                        .ForEach(column =>
                        {
                            var recordingData = ImportRecordingData(
                                context: context,
                                column: column.Value.Column,
                                value: ImportUtilities.RecordingData(
                                    columnHash: columnHash,
                                    row: data.Row,
                                    column: column),
                                inheritPermission: ss.InheritPermission);
                            switch (column.Value.Column.ColumnName)
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
                                case "Locked":
                                    resultModel.Locked = recordingData.ToBool();
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
                                        column: column.Value.Column,
                                        value: recordingData);
                                    break;
                            }
                        });
                    resultHash.Add(data.Index, resultModel);
                });
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
                foreach (var resultModel in resultHash.Values)
                {
                    resultModel.SetByFormula(context: context, ss: ss);
                    resultModel.SetTitle(context: context, ss: ss);
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        if (resultModel.Updated(context: context))
                        {
                            var errorData = resultModel.Update(
                                context: context,
                                ss: ss,
                                extendedSqls: false,
                                previousTitle: previousTitle,
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
                                    return errorData.MessageJson(context: context);
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
                                return errorData.MessageJson(context: context);
                        }
                        insertCount++;
                    }
                }
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportedExtendedSqls(
                            context: context,
                            siteId: ss.SiteId)
                                .ToArray());
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
                default: return invalid.MessageJson(context: context);
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
                    title: ss.Title,
                    name: export.Name,
                    extension: export.Type.ToString()),
                encoding: context.QueryStrings.Data("encoding"));
        }

        public static string ExportAsync(
            Context context, SiteSettings ss, SiteModel siteModel)
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
                                    .BinaryType("Attachments")
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
                SiteUtilities.UpdateApiCount(context: context, ss: ss);
                return ApiResults.Get(
                    statusCode: 200,
                    limitPerDate: Parameters.Api.LimitPerSite,
                    limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
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
                siteId: ss.SiteId);
            var timePeriod = view.GetCalendarTimePeriod(ss: ss);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss));
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
            var begin = Calendars.BeginDate(
                context: context,
                date: date,
                timePeriod: timePeriod);
            var end = Calendars.EndDate(
                context: context,
                date: date,
                timePeriod: timePeriod);
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
                        groupBy: groupBy,
                        fromColumn: fromColumn,
                        toColumn: toColumn,
                        date: date,
                        begin: begin,
                        choices: choices,
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
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
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
            var begin = Calendars.BeginDate(
                context: context,
                date: date,
                timePeriod: timePeriod);
            var end = Calendars.EndDate(
                context: context,
                date: date,
                timePeriod: timePeriod);
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
            return inRange
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
                                groupBy: groupBy,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                date: date,
                                begin: begin,
                                choices: choices,
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
                        message: inRangeY
                            ? Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.CalendarLimit.ToString())
                            : Messages.TooManyRowCases(
                                context: context,
                                data: Parameters.General.CalendarYLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
                                context: context,
                                ss: ss,
                                timePeriod: timePeriod,
                                groupBy: groupBy,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                date: date,
                                begin: begin,
                                choices: choices,
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
            Column groupBy,
            DateTime begin,
            DateTime end)
        {
            var where = new SqlWhereCollection();
            if (toColumn == null)
            {
                where.Add(
                    tableName: "Results",
                    raw: $"\"Results\".\"{fromColumn.ColumnName}\" between '{begin}' and '{end}'");
            }
            else
            {
                where.Add(or: Rds.ResultsWhere()
                    .Add(raw: $"\"Results\".\"{fromColumn.ColumnName}\" between '{begin}' and '{end}'")
                    .Add(raw: $"\"Results\".\"{toColumn.ColumnName}\" between '{begin}' and '{end}'")
                    .Add(raw: $"\"Results\".\"{fromColumn.ColumnName}\"<='{begin}' and \"Results\".\"{toColumn.ColumnName}\">='{end}'"));
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
                            .ItemTitle(ss.ReferenceType)
                            .Add(
                                column: groupBy,
                                function: Sqls.Functions.SingleColumn),
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
            Column groupBy,
            Column fromColumn,
            Column toColumn,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices,
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
                    groupBy: groupBy,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    date: date,
                    begin: begin,
                    choices: choices,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId)
                : hb.CalendarBody(
                    context: context,
                    ss: ss,
                    timePeriod: timePeriod,
                    groupBy: groupBy,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    date: date,
                    begin: begin,
                    choices: choices,
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
                    .Add(
                        context: context,
                        column: groupBy)
                    .Add(
                        context: context,
                        column: value);
                var where = view.Where(context: context, ss: ss);
                var dataRows = Repository.ExecuteTable(
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
                .Add(
                    context: context,
                    column: groupByX)
                .Add(
                    context: context,
                    column: groupByY)
                .Add(
                    context: context,
                    column: value);
            var where = view.Where(context: context, ss: ss);
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
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return KambanJson(context: context, ss: ss);
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
                ss: ss);
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
                            .SetMemory("formChanged", false)
                            .Message(Messages.UnlockedRecord(context: context))
                            .ClearFormData()
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
                    where: where)) <= limit;
        }

        private static ErrorData ExistsLockedRecord(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            Column bulkUpdateColumn = null)
        {
            var lockedRecordWhere = new Rds.ResultsWhereCollection()
                .Results_Locked(true);
            lockedRecordWhere.AddRange(where);
            if (bulkUpdateColumn?.ColumnName == "Locked")
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
    }
}
