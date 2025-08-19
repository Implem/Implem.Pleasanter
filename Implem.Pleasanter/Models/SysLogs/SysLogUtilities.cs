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
    public static class SysLogUtilities
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
                viewModeBody: () => hb.Grid(
                    context: context,
                    gridData: gridData,
                    ss: ss,
                    view: view,
                    serverScriptModelRow: serverScriptModelRow));
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
            var invalid = SysLogValidators.OnEntry(
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
                referenceType: "SysLogs",
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
                                        view: view))
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
                    .DropDownSearchDialog(
                        context: context,
                        id: ss.SiteId)
                    .MoveDialog(context: context, bulk: true)
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
                    context: context,
                    attributes: new HtmlAttributes()
                        .Id($"Grid{suffix}")
                        .Class(ss.GridCss(context: context))
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    scrollable: ss.DashboardParts.Count == 1 ? false : true,
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            columns: columns,
                            view: view,
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
                    value: gridData.DataRows.Select(g => g.Long("SysLogId")).ToJson())
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
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            GridData gridData = null;
            try
            {
                gridData = GetGridData(
                    context: context,
                    ss: ss,
                    view: view,
                    offset: offset);
            }
            catch (Implem.Libraries.Exceptions.CanNotGridSortException)
            {
                return new ResponseCollection(context: context)
                    .Message(context.Messages.Last())
                    .Log(context.GetLog())
                    .ToJson();
            }
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return new ResponseCollection(context: context)
                .WindowScrollTop(_using: windowScrollTop)
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridOffset")
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
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
                    offset: offset,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.TotalCount))
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("SysLogId")).ToJson())
                .Val("#GridColumns", columns.Select(o => o.ColumnName).ToJson())
                .Paging("#Grid")
                .Message(message)
                .Messages(context.Messages)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            List<Column> columns,
            View view,
            int offset = 0,
            bool clearCheck = false,
            string action = "GridRows",
            ServerScriptModelRow serverScriptModelRow = null,
            string suffix = "")
        {
            var checkRow = ss.CheckRow(
                context: context,
                gridColumns: view.GridColumns);
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
            return hb
                .THead(
                    _using: offset == 0,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            ss: ss,
                            columns: columns,
                            view: view,
                            sort: false,
                            checkRow: checkRow,
                            checkAll: checkAll,
                            action: action,
                            serverScriptModelRow: serverScriptModelRow,
                            suffix: suffix))
                .TBody(action: () => hb
                    .GridRows(
                        context: context,
                        ss: ss,
                        view: view,
                        dataRows: gridData.DataRows,
                        columns: columns,
                        checkRow: checkRow,
                        clearCheck: clearCheck));
        }

        private static SqlWhereCollection SelectedWhere(
            Context context,
            SiteSettings ss)
        {
            var selector = new RecordSelector(context: context);
            return !selector.Nothing
                ? Rds.SysLogsWhere().SysLogId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.SysLogsWhere().SysLogId_In(
                    value: recordSelector.Selected?.Select(o => o.ToLong()) ?? new List<long>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long sysLogId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.SysLogsWhere().SysLogId(sysLogId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: sysLogId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{sysLogId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + sysLogId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("SysLogId")}\"][data-latest]",
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
                            idColumn: "SysLogId"))
                    .Messages(context.Messages)
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            SysLogModel sysLogModel,
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
                    sysLogModel: sysLogModel);
            }
            else
            {
                var mine = sysLogModel.Mine(context: context);
                switch (column.Name)
                {
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.CreatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "SysLogId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.SysLogId,
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
                                    value: sysLogModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "SysLogType":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.SysLogType,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "MachineName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.MachineName,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Class":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.Class,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Method":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.Method,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Api":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.Api,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.SiteId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ReferenceId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.ReferenceId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ReferenceType":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.ReferenceType,
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
                                    value: sysLogModel.Status,
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
                                    value: sysLogModel.Description,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "RequestData":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.RequestData,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "HttpMethod":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.HttpMethod,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "RequestSize":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.RequestSize,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ResponseSize":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.ResponseSize,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Elapsed":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.Elapsed,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Url":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.Url,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UrlReferer":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.UrlReferer,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserHostName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.UserHostName,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserHostAddress":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.UserHostAddress,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserLanguage":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.UserLanguage,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserAgent":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.UserAgent,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ErrMessage":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.ErrMessage,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ErrStackTrace":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.ErrStackTrace,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AssemblyVersion":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: sysLogModel.AssemblyVersion,
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
                                    value: sysLogModel.Creator,
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
                                            value: sysLogModel.GetClass(columnName: column.Name),
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
                                            value: sysLogModel.GetNum(columnName: column.Name),
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
                                            value: sysLogModel.GetDate(columnName: column.Name),
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
                                            value: sysLogModel.GetDescription(columnName: column.Name),
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
                                            value: sysLogModel.GetCheck(columnName: column.Name),
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
                                            value: sysLogModel.GetAttachments(columnName: column.Name),
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
            SysLogModel sysLogModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "CreatedTime": value = sysLogModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "SysLogId": value = sysLogModel.SysLogId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = sysLogModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "SysLogType": value = sysLogModel.SysLogType.GridText(
                        context: context,
                        column: column); break;
                    case "MachineName": value = sysLogModel.MachineName.GridText(
                        context: context,
                        column: column); break;
                    case "Class": value = sysLogModel.Class.GridText(
                        context: context,
                        column: column); break;
                    case "Method": value = sysLogModel.Method.GridText(
                        context: context,
                        column: column); break;
                    case "Api": value = sysLogModel.Api.GridText(
                        context: context,
                        column: column); break;
                    case "SiteId": value = sysLogModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "ReferenceId": value = sysLogModel.ReferenceId.GridText(
                        context: context,
                        column: column); break;
                    case "ReferenceType": value = sysLogModel.ReferenceType.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = sysLogModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "Description": value = sysLogModel.Description.GridText(
                        context: context,
                        column: column); break;
                    case "RequestData": value = sysLogModel.RequestData.GridText(
                        context: context,
                        column: column); break;
                    case "HttpMethod": value = sysLogModel.HttpMethod.GridText(
                        context: context,
                        column: column); break;
                    case "RequestSize": value = sysLogModel.RequestSize.GridText(
                        context: context,
                        column: column); break;
                    case "ResponseSize": value = sysLogModel.ResponseSize.GridText(
                        context: context,
                        column: column); break;
                    case "Elapsed": value = sysLogModel.Elapsed.GridText(
                        context: context,
                        column: column); break;
                    case "Url": value = sysLogModel.Url.GridText(
                        context: context,
                        column: column); break;
                    case "UrlReferer": value = sysLogModel.UrlReferer.GridText(
                        context: context,
                        column: column); break;
                    case "UserHostName": value = sysLogModel.UserHostName.GridText(
                        context: context,
                        column: column); break;
                    case "UserHostAddress": value = sysLogModel.UserHostAddress.GridText(
                        context: context,
                        column: column); break;
                    case "UserLanguage": value = sysLogModel.UserLanguage.GridText(
                        context: context,
                        column: column); break;
                    case "UserAgent": value = sysLogModel.UserAgent.GridText(
                        context: context,
                        column: column); break;
                    case "ErrMessage": value = sysLogModel.ErrMessage.GridText(
                        context: context,
                        column: column); break;
                    case "ErrStackTrace": value = sysLogModel.ErrStackTrace.GridText(
                        context: context,
                        column: column); break;
                    case "AssemblyVersion": value = sysLogModel.AssemblyVersion.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = sysLogModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = sysLogModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = sysLogModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = sysLogModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = sysLogModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = sysLogModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = sysLogModel.GetAttachments(columnName: column.Name).GridText(
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
            return Editor(context: context, ss: ss, sysLogModel: new SysLogModel(
                context: context,
                ss: SiteSettingsUtilities.SysLogsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, long sysLogId, bool clearSessions)
        {
            var sysLogModel = new SysLogModel(
                context: context,
                ss: SiteSettingsUtilities.SysLogsSiteSettings(context: context),
                sysLogId: sysLogId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            sysLogModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.SysLogsSiteSettings(context: context),
                sysLogId: sysLogModel.SysLogId);
            return Editor(context: context, ss: ss, sysLogModel: sysLogModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, SysLogModel sysLogModel)
        {
            var invalid = SysLogValidators.OnEditing(
                context: context,
                ss: ss,
                sysLogModel: sysLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                referenceType: "SysLogs",
                title: sysLogModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.SysLogs(context: context) + " - " + Displays.New(context: context)
                    : sysLogModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        sysLogModel: sysLogModel)).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, SysLogModel sysLogModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType =  Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: sysLogModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(sysLogModel.SysLogId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "SysLogs",
                                id: sysLogModel.SysLogId)
                            : Locations.Action(
                                context: context,
                                controller: "SysLogs")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: sysLogModel,
                            tableName: "SysLogs")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: sysLogModel.Comments,
                                    column: commentsColumn,
                                    verType: sysLogModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    sysLogModel: sysLogModel)
                                .FieldSetGeneral(context: context, ss: ss, sysLogModel: sysLogModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: sysLogModel.MethodType != BaseModel.MethodTypes.New
                                        && !context.Publish)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: sysLogModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            sysLogModel: sysLogModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: sysLogModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: sysLogModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "SysLogs_Timestamp",
                            css: "always-send",
                            value: sysLogModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: sysLogModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "SysLogs",
                    referenceId: sysLogModel.SysLogId,
                    referenceVer: sysLogModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    sysLogModel: sysLogModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, SysLogModel sysLogModel)
        {
            // 履歴の表示機能がないため全般タブのみ出力
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel)
        {
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb.FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    sysLogModel: sysLogModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    sysLogModel: sysLogModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: sysLogModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = sysLogModel.ControlValue(
                context: context,
                ss: ss,
                column: column);
            if (value != null)
            {
                hb.Field(
                    context: context,
                    ss: ss,
                    column: column,
                    value: value,
                    columnPermissionType: Permissions.ColumnPermissionType(
                        context: context,
                        ss: ss,
                        column: column,
                        baseModel: sysLogModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this SysLogModel sysLogModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "SysLogId":
                    return sysLogModel.SysLogId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return sysLogModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "SysLogType":
                    return sysLogModel.SysLogType
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "MachineName":
                    return sysLogModel.MachineName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Class":
                    return sysLogModel.Class
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Method":
                    return sysLogModel.Method
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Api":
                    return sysLogModel.Api
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "SiteId":
                    return sysLogModel.SiteId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ReferenceId":
                    return sysLogModel.ReferenceId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ReferenceType":
                    return sysLogModel.ReferenceType
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Status":
                    return sysLogModel.Status
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Description":
                    return sysLogModel.Description
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "RequestData":
                    return sysLogModel.RequestData
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "HttpMethod":
                    return sysLogModel.HttpMethod
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "RequestSize":
                    return sysLogModel.RequestSize
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ResponseSize":
                    return sysLogModel.ResponseSize
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Elapsed":
                    return sysLogModel.Elapsed
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Url":
                    return sysLogModel.Url
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UrlReferer":
                    return sysLogModel.UrlReferer
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserHostName":
                    return sysLogModel.UserHostName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserHostAddress":
                    return sysLogModel.UserHostAddress
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserLanguage":
                    return sysLogModel.UserLanguage
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserAgent":
                    return sysLogModel.UserAgent
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ErrMessage":
                    return sysLogModel.ErrMessage
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ErrStackTrace":
                    return sysLogModel.ErrStackTrace
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AssemblyVersion":
                    return sysLogModel.AssemblyVersion
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return sysLogModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return sysLogModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return sysLogModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return sysLogModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return sysLogModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return sysLogModel.GetAttachments(columnName: column.Name)
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
            SysLogModel sysLogModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long sysLogId)
        {
            return EditorResponse(context, ss, new SysLogModel(
                context, ss, sysLogId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            Message message = null,
            string switchTargets = null)
        {
            sysLogModel.MethodType = sysLogModel.SysLogId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new SysLogsResponseCollection(
                context: context,
                sysLogModel: sysLogModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, sysLogModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long sysLogId)
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
                    .SysLogs_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectSysLogs(
                        column: Rds.SysLogsColumn().SysLogsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectSysLogs(
                            column: Rds.SysLogsColumn().SysLogId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["SysLogId"].ToLong())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(sysLogId))
            {
                switchTargets.Add(sysLogId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: sysLogModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = sysLogModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#SysLogs_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                sysLogModel: sysLogModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "SysLogId":
                                res.Val(
                                    target: "#SysLogs_SysLogId" + idSuffix,
                                    value: sysLogModel.SysLogId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SysLogType":
                                res.Val(
                                    target: "#SysLogs_SysLogType" + idSuffix,
                                    value: sysLogModel.SysLogType.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "OnAzure":
                                res.Val(
                                    target: "#SysLogs_OnAzure" + idSuffix,
                                    value: sysLogModel.OnAzure,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "MachineName":
                                res.Val(
                                    target: "#SysLogs_MachineName" + idSuffix,
                                    value: sysLogModel.MachineName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ServiceName":
                                res.Val(
                                    target: "#SysLogs_ServiceName" + idSuffix,
                                    value: sysLogModel.ServiceName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TenantName":
                                res.Val(
                                    target: "#SysLogs_TenantName" + idSuffix,
                                    value: sysLogModel.TenantName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Application":
                                res.Val(
                                    target: "#SysLogs_Application" + idSuffix,
                                    value: sysLogModel.Application.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Class":
                                res.Val(
                                    target: "#SysLogs_Class" + idSuffix,
                                    value: sysLogModel.Class.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Method":
                                res.Val(
                                    target: "#SysLogs_Method" + idSuffix,
                                    value: sysLogModel.Method.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Api":
                                res.Val(
                                    target: "#SysLogs_Api" + idSuffix,
                                    value: sysLogModel.Api,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "RequestData":
                                res.Val(
                                    target: "#SysLogs_RequestData" + idSuffix,
                                    value: sysLogModel.RequestData.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "HttpMethod":
                                res.Val(
                                    target: "#SysLogs_HttpMethod" + idSuffix,
                                    value: sysLogModel.HttpMethod.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "RequestSize":
                                res.Val(
                                    target: "#SysLogs_RequestSize" + idSuffix,
                                    value: sysLogModel.RequestSize.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ResponseSize":
                                res.Val(
                                    target: "#SysLogs_ResponseSize" + idSuffix,
                                    value: sysLogModel.ResponseSize.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Elapsed":
                                res.Val(
                                    target: "#SysLogs_Elapsed" + idSuffix,
                                    value: sysLogModel.Elapsed.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ApplicationAge":
                                res.Val(
                                    target: "#SysLogs_ApplicationAge" + idSuffix,
                                    value: sysLogModel.ApplicationAge.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ApplicationRequestInterval":
                                res.Val(
                                    target: "#SysLogs_ApplicationRequestInterval" + idSuffix,
                                    value: sysLogModel.ApplicationRequestInterval.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SessionAge":
                                res.Val(
                                    target: "#SysLogs_SessionAge" + idSuffix,
                                    value: sysLogModel.SessionAge.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SessionRequestInterval":
                                res.Val(
                                    target: "#SysLogs_SessionRequestInterval" + idSuffix,
                                    value: sysLogModel.SessionRequestInterval.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "WorkingSet64":
                                res.Val(
                                    target: "#SysLogs_WorkingSet64" + idSuffix,
                                    value: sysLogModel.WorkingSet64.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "VirtualMemorySize64":
                                res.Val(
                                    target: "#SysLogs_VirtualMemorySize64" + idSuffix,
                                    value: sysLogModel.VirtualMemorySize64.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ProcessId":
                                res.Val(
                                    target: "#SysLogs_ProcessId" + idSuffix,
                                    value: sysLogModel.ProcessId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ProcessName":
                                res.Val(
                                    target: "#SysLogs_ProcessName" + idSuffix,
                                    value: sysLogModel.ProcessName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "BasePriority":
                                res.Val(
                                    target: "#SysLogs_BasePriority" + idSuffix,
                                    value: sysLogModel.BasePriority.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Url":
                                res.Val(
                                    target: "#SysLogs_Url" + idSuffix,
                                    value: sysLogModel.Url.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UrlReferer":
                                res.Val(
                                    target: "#SysLogs_UrlReferer" + idSuffix,
                                    value: sysLogModel.UrlReferer.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserHostName":
                                res.Val(
                                    target: "#SysLogs_UserHostName" + idSuffix,
                                    value: sysLogModel.UserHostName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserHostAddress":
                                res.Val(
                                    target: "#SysLogs_UserHostAddress" + idSuffix,
                                    value: sysLogModel.UserHostAddress.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserLanguage":
                                res.Val(
                                    target: "#SysLogs_UserLanguage" + idSuffix,
                                    value: sysLogModel.UserLanguage.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserAgent":
                                res.Val(
                                    target: "#SysLogs_UserAgent" + idSuffix,
                                    value: sysLogModel.UserAgent.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SessionGuid":
                                res.Val(
                                    target: "#SysLogs_SessionGuid" + idSuffix,
                                    value: sysLogModel.SessionGuid.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ErrMessage":
                                res.Val(
                                    target: "#SysLogs_ErrMessage" + idSuffix,
                                    value: sysLogModel.ErrMessage.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ErrStackTrace":
                                res.Val(
                                    target: "#SysLogs_ErrStackTrace" + idSuffix,
                                    value: sysLogModel.ErrStackTrace.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "InDebug":
                                res.Val(
                                    target: "#SysLogs_InDebug" + idSuffix,
                                    value: sysLogModel.InDebug,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AssemblyVersion":
                                res.Val(
                                    target: "#SysLogs_AssemblyVersion" + idSuffix,
                                    value: sysLogModel.AssemblyVersion.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#SysLogs_{column.Name}{idSuffix}",
                                            value: sysLogModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#SysLogs_{column.Name}{idSuffix}",
                                            value: sysLogModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#SysLogs_{column.Name}{idSuffix}",
                                            value: sysLogModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#SysLogs_{column.Name}{idSuffix}",
                                            value: sysLogModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#SysLogs_{column.Name}{idSuffix}",
                                            value: sysLogModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#SysLogs_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"SysLogs_{column.Name}Field",
                                                    controlId: $"SysLogs_{column.Name}",
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
                                                    value: sysLogModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: sysLogModel)
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

        public static string Create(Context context, SiteSettings ss)
        {
            ﻿//Issues、Results以外は参照コピーを使用しないためcopyFromを0にする
            var copyFrom = 0;
            var sysLogModel = new SysLogModel(
                context: context,
                ss: ss,
                sysLogId: copyFrom,
                formData: context.Forms);
            sysLogModel.SysLogId = 0;
            sysLogModel.Ver = 1;
            var invalid = SysLogValidators.OnCreating(
                context: context,
                ss: ss,
                sysLogModel: sysLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var processes = (List<Process>)null;
            var errorData = sysLogModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            sysLogModel: sysLogModel,
                            processes: processes));
                    return new ResponseCollection(
                        context: context,
                        id: sysLogModel.SysLogId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .Href(Locations.Edit(
                                context: context,
                                controller: context.Controller,
                                id: ss.Columns.Any(o => o.Linking)
                                    ? context.Forms.Long("LinkId")
                                    : sysLogModel.SysLogId)
                                        + "?new=1"
                                        + (ss.Columns.Any(o => o.Linking)
                                            && context.Forms.Long("FromTabIndex") > 0
                                                ? $"&TabIndex={context.Forms.Long("FromTabIndex")}"
                                                : string.Empty))
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: sysLogModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = sysLogModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, long sysLogId)
        {
            var sysLogModel = new SysLogModel(
                context: context,
                ss: ss,
                sysLogId: sysLogId,
                formData: context.Forms);
            var invalid = SysLogValidators.OnUpdating(
                context: context,
                ss: ss,
                sysLogModel: sysLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (sysLogModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var processes = (List<Process>)null;
            var errorData = sysLogModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new SysLogsResponseCollection(
                        context: context,
                        sysLogModel: sysLogModel);
                    return ResponseByUpdate(res, context, ss, sysLogModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: sysLogModel.Comments,
                            verType: sysLogModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: sysLogModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            SysLogsResponseCollection res,
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: sysLogModel);
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
                    where: Rds.SysLogsWhere().SysLogId(sysLogModel.SysLogId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{sysLogModel.SysLogId}\"][data-latest]",
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
                        sysLogModel: sysLogModel,
                        processes: processes))
                    .Messages(context.Messages);
            }
            else
            {
                var verUp = Versions.VerUp(
                    context: context,
                    ss: ss,
                    verUp: false);
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, sysLogModel: sysLogModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", sysLogModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(sysLogModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: sysLogModel,
                        tableName: "SysLogs"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: sysLogModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: sysLogModel.Comments,
                        deleteCommentId: sysLogModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: sysLogModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = sysLogModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, long sysLogId)
        {
            var sysLogModel = new SysLogModel(context, ss, sysLogId);
            var invalid = SysLogValidators.OnDeleting(
                context: context,
                ss: ss,
                sysLogModel: sysLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = sysLogModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: sysLogModel.Title.MessageDisplay(context: context)));
                    var res = new SysLogsResponseCollection(
                        context: context,
                        sysLogModel: sysLogModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long sysLogId, Message message = null)
        {
            var sysLogModel = new SysLogModel(context: context, ss: ss, sysLogId: sysLogId);
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
                        context: context,
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
                                    sysLogModel: sysLogModel))));
            return new SysLogsResponseCollection(
                context: context,
                sysLogModel: sysLogModel)
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
            SysLogModel sysLogModel)
        {
            new SysLogCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.SysLogsWhere().SysLogId(sysLogModel.SysLogId),
                orderBy: Rds.SysLogsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(sysLogModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(sysLogModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: sysLogModelHistory.Ver == sysLogModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: sysLogModelHistory.Ver.ToString(),
                                            _using: sysLogModelHistory.Ver < sysLogModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        sysLogModel: sysLogModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.SysLogsColumnCollection()
                .SysLogId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.SysLogsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long sysLogId)
        {
            var sysLogModel = new SysLogModel(context: context, ss: ss, sysLogId: sysLogId);
            sysLogModel.Get(
                context: context,
                ss: ss,
                where: Rds.SysLogsWhere()
                    .SysLogId(sysLogModel.SysLogId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            sysLogModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, sysLogModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        context.Controller,
                        sysLogId.ToString() 
                            + (sysLogModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.SysLogsSiteSettings(context: context),
                offset: context.Forms.Int("GridOffset"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenExportSelectorDialog(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InvalidRequest));
            }
            var invalid = SysLogValidators.OnExporting(
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseFile Export(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = SysLogValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var export = ss.GetExport(context: context);
            var view = Views.GetBySession(context: context, ss: ss);
            var csv = new System.Text.StringBuilder();
            if (export.Header == true)
            {
                csv.Append(
                    export.Columns.Select(column =>
                        "\"" + column.GetLabelText() + "\"").Join(","),
                    "\n");
            }
            new SysLogCollection(
                context: context,
                ss: ss,
                where: view.Where(
                    context: context,
                    ss: ss),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                top: Parameters.SysLog.ExportLimit)
                    .ForEach(sysLogModel =>
                        csv.Append(
                            export.Columns.Select(exportColumn =>
                                sysLogModel.CsvData(
                                    context: context,
                                    ss: ss,
                                    column: ss.GetColumn(
                                        context: context,
                                        columnName: exportColumn.ColumnName),
                                    exportColumn: exportColumn,
                                    mine: sysLogModel.Mine(context: context),
                                    encloseDoubleQuotes: true)).Join(),
                            "\n"));
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
                    name: Displays.SysLogs(context: context)),
                encoding: context.Forms.Data("ExportEncoding"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Maintain(Context context, bool force = false)
        {
            if (Parameters.SysLog.RetentionPeriod > 0
                && (force || (DateTime.Now - Applications.SysLogsMaintenanceDate).Days > 0))
            {
                PhysicalDelete(context: context);
                Applications.SysLogsMaintenanceDate = DateTime.Now.Date;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void PhysicalDelete(Context context)
        {
            var chunkSize = Parameters.BackgroundService.DeleteSysLogsChunkSize;
            if (chunkSize <= 0)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteSysLogs(
                        where: Rds.SysLogsWhere().CreatedTime(
                            DateTime.Now.Date.AddDays(
                                Parameters.SysLog.RetentionPeriod * -1),
                            _operator: "<")));
            }
            else
            {
                // dbmsによって delete文 の limit句 の指定方法が違うために、SysLogId(Primary Key)で条件を作成している。
                var (min, max) = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSysLogs(
                        column: Rds.SysLogsColumn()
                            .SysLogs_SysLogId(function: Sqls.Functions.Min, _as: "SysLogIdMin")
                            .SysLogs_SysLogId(function: Sqls.Functions.Max, _as: "SysLogIdMax"),
                        where: Rds.SysLogsWhere().CreatedTime(
                                DateTime.Now.Date.AddDays(
                                    Parameters.SysLog.RetentionPeriod * -1),
                                _operator: "<")))
                            .AsEnumerable()
                            .Select(o => ( min: o["SysLogIdMin"].ToLong(), max: o["SysLogIdMax"].ToLong() ))
                            .FirstOrDefault((min: 0L, max: 0L));
                if (min != 0 && max != 0)
                {
                    for (var i = min; i <= max; i += chunkSize)
                    {
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.PhysicalDeleteSysLogs(
                                where: Rds.SysLogsWhere()
                                    .CreatedTime(
                                        DateTime.Now.Date.AddDays(
                                            Parameters.SysLog.RetentionPeriod * -1),
                                        _operator: "<")
                                    .SysLogId_Between(
                                        begin: i,
                                        end: i + chunkSize - 1)));
                    }
                }
            }
        }
    }
}
