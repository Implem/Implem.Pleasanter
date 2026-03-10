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
    public static class McpLogUtilities
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
            var invalid = McpLogValidators.OnEntry(
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
                referenceType: "McpLogs",
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
                where: Rds.McpLogsWhere().TenantId(context.TenantId),
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
                    value: gridData.DataRows.Select(g => g.Long("McpLogId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("McpLogId")).ToJson())
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
                ? Rds.McpLogsWhere().McpLogId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.McpLogsWhere().McpLogId_In(
                    value: recordSelector.Selected?.Select(o => o.ToLong()) ?? new List<long>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long mcpLogId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.McpLogsWhere().McpLogId(mcpLogId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: mcpLogId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{mcpLogId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + mcpLogId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("McpLogId")}\"][data-latest]",
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
                            idColumn: "McpLogId"))
                    .Messages(context.Messages)
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            McpLogModel mcpLogModel,
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
                    mcpLogModel: mcpLogModel);
            }
            else
            {
                var mine = mcpLogModel.Mine(context: context);
                switch (column.Name)
                {
                    case "McpLogId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.McpLogId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "StartTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.StartTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "EndTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.EndTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "McpRequestId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.McpRequestId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "McpSessionId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.McpSessionId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "McpMethod":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.McpMethod,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "TargetName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.TargetName,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.UserId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ApiKeyPrefix":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.ApiKeyPrefix,
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
                                    value: mcpLogModel.Elapsed,
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
                                    value: mcpLogModel.Status,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "JsonRpcErrorCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.JsonRpcErrorCode,
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
                                    value: mcpLogModel.ErrMessage,
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
                                    value: mcpLogModel.RequestData,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "ResponseData":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: mcpLogModel.ResponseData,
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
                                    value: mcpLogModel.UserHostAddress,
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
                                    value: mcpLogModel.UserAgent,
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
                                    value: mcpLogModel.Ver,
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
                                    value: mcpLogModel.Creator,
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
                                    value: mcpLogModel.CreatedTime,
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
                                            value: mcpLogModel.GetClass(columnName: column.Name),
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
                                            value: mcpLogModel.GetNum(columnName: column.Name),
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
                                            value: mcpLogModel.GetDate(columnName: column.Name),
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
                                            value: mcpLogModel.GetDescription(columnName: column.Name),
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
                                            value: mcpLogModel.GetCheck(columnName: column.Name),
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
                                            value: mcpLogModel.GetAttachments(columnName: column.Name),
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
            McpLogModel mcpLogModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "McpLogId": value = mcpLogModel.McpLogId.GridText(
                        context: context,
                        column: column); break;
                    case "StartTime": value = mcpLogModel.StartTime.GridText(
                        context: context,
                        column: column); break;
                    case "EndTime": value = mcpLogModel.EndTime.GridText(
                        context: context,
                        column: column); break;
                    case "McpRequestId": value = mcpLogModel.McpRequestId.GridText(
                        context: context,
                        column: column); break;
                    case "McpSessionId": value = mcpLogModel.McpSessionId.GridText(
                        context: context,
                        column: column); break;
                    case "McpMethod": value = mcpLogModel.McpMethod.GridText(
                        context: context,
                        column: column); break;
                    case "TargetName": value = mcpLogModel.TargetName.GridText(
                        context: context,
                        column: column); break;
                    case "UserId": value = mcpLogModel.UserId.GridText(
                        context: context,
                        column: column); break;
                    case "ApiKeyPrefix": value = mcpLogModel.ApiKeyPrefix.GridText(
                        context: context,
                        column: column); break;
                    case "Elapsed": value = mcpLogModel.Elapsed.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = mcpLogModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "JsonRpcErrorCode": value = mcpLogModel.JsonRpcErrorCode.GridText(
                        context: context,
                        column: column); break;
                    case "ErrMessage": value = mcpLogModel.ErrMessage.GridText(
                        context: context,
                        column: column); break;
                    case "RequestData": value = mcpLogModel.RequestData.GridText(
                        context: context,
                        column: column); break;
                    case "ResponseData": value = mcpLogModel.ResponseData.GridText(
                        context: context,
                        column: column); break;
                    case "UserHostAddress": value = mcpLogModel.UserHostAddress.GridText(
                        context: context,
                        column: column); break;
                    case "UserAgent": value = mcpLogModel.UserAgent.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = mcpLogModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = mcpLogModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = mcpLogModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = mcpLogModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = mcpLogModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = mcpLogModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = mcpLogModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = mcpLogModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = mcpLogModel.GetAttachments(columnName: column.Name).GridText(
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
                    .MarkDown(
                        context: context,
                        ss: ss,
                        disabled: true,
                        text: gridDesign));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            return Editor(context: context, ss: ss, mcpLogModel: new McpLogModel(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, long mcpLogId, bool clearSessions)
        {
            var mcpLogModel = new McpLogModel(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
                mcpLogId: mcpLogId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            mcpLogModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
                mcpLogId: mcpLogModel.McpLogId);
            return Editor(context: context, ss: ss, mcpLogModel: mcpLogModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, McpLogModel mcpLogModel)
        {
            var invalid = McpLogValidators.OnEditing(
                context: context,
                ss: ss,
                mcpLogModel: mcpLogModel);
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
                referenceType: "McpLogs",
                title: mcpLogModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.McpLogs(context: context) + " - " + Displays.New(context: context)
                    : mcpLogModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        mcpLogModel: mcpLogModel)).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, McpLogModel mcpLogModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType =  Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: mcpLogModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(mcpLogModel.McpLogId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "McpLogs",
                                id: mcpLogModel.McpLogId)
                            : Locations.Action(
                                context: context,
                                controller: "McpLogs")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: mcpLogModel,
                            tableName: "McpLogs")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: mcpLogModel.Comments,
                                    column: commentsColumn,
                                    verType: mcpLogModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    mcpLogModel: mcpLogModel)
                                .FieldSetGeneral(context: context, ss: ss, mcpLogModel: mcpLogModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: mcpLogModel.MethodType != BaseModel.MethodTypes.New
                                        && !context.Publish)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: mcpLogModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            mcpLogModel: mcpLogModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: mcpLogModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: mcpLogModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "McpLogs_Timestamp",
                            css: "always-send",
                            value: mcpLogModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: mcpLogModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "McpLogs",
                    referenceId: mcpLogModel.McpLogId,
                    referenceVer: mcpLogModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog(context: context)
                .EditorExtensions(
                    context: context,
                    mcpLogModel: mcpLogModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, McpLogModel mcpLogModel)
        {
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
            McpLogModel mcpLogModel)
        {
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb.FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    mcpLogModel: mcpLogModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    mcpLogModel: mcpLogModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: mcpLogModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool isResponse = false,
            bool preview = false,
            bool disableSection = false)
        {
            var value = mcpLogModel.ControlValue(
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
                        baseModel: mcpLogModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    isResponse: isResponse,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this McpLogModel mcpLogModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "McpLogId":
                    return mcpLogModel.McpLogId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "StartTime":
                    return mcpLogModel.StartTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "EndTime":
                    return mcpLogModel.EndTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "McpRequestId":
                    return mcpLogModel.McpRequestId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "McpSessionId":
                    return mcpLogModel.McpSessionId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "McpMethod":
                    return mcpLogModel.McpMethod
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "TargetName":
                    return mcpLogModel.TargetName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserId":
                    return mcpLogModel.UserId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ApiKeyPrefix":
                    return mcpLogModel.ApiKeyPrefix
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Elapsed":
                    return mcpLogModel.Elapsed
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Status":
                    return mcpLogModel.Status
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "JsonRpcErrorCode":
                    return mcpLogModel.JsonRpcErrorCode
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ErrMessage":
                    return mcpLogModel.ErrMessage
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "RequestData":
                    return mcpLogModel.RequestData
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ResponseData":
                    return mcpLogModel.ResponseData
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserHostAddress":
                    return mcpLogModel.UserHostAddress
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserAgent":
                    return mcpLogModel.UserAgent
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return mcpLogModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return mcpLogModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return mcpLogModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return mcpLogModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return mcpLogModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return mcpLogModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return mcpLogModel.GetAttachments(columnName: column.Name)
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
            McpLogModel mcpLogModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long mcpLogId)
        {
            return EditorResponse(context, ss, new McpLogModel(
                context, ss, mcpLogId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            Message message = null,
            string switchTargets = null)
        {
            mcpLogModel.MethodType = mcpLogModel.McpLogId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new McpLogsResponseCollection(
                context: context,
                mcpLogModel: mcpLogModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, mcpLogModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long mcpLogId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.McpLogsWhere().TenantId(context.TenantId));
            var param = view.Param(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .McpLogs_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectMcpLogs(
                        column: Rds.McpLogsColumn().McpLogsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectMcpLogs(
                            column: Rds.McpLogsColumn().McpLogId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["McpLogId"].ToLong())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(mcpLogId))
            {
                switchTargets.Add(mcpLogId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: mcpLogModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = mcpLogModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#McpLogs_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                mcpLogModel: mcpLogModel,
                                column: column,
                                idSuffix: idSuffix,
                                isResponse: true));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#McpLogs_{column.Name}{idSuffix}",
                                            value: mcpLogModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#McpLogs_{column.Name}{idSuffix}",
                                            value: mcpLogModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#McpLogs_{column.Name}{idSuffix}",
                                            value: mcpLogModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#McpLogs_{column.Name}{idSuffix}",
                                            value: mcpLogModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#McpLogs_{column.Name}{idSuffix}",
                                            value: mcpLogModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#McpLogs_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"McpLogs_{column.Name}Field",
                                                    controlId: $"McpLogs_{column.Name}",
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
                                                    value: mcpLogModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: mcpLogModel)
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
            var copyFrom = 0;
            var mcpLogModel = new McpLogModel(
                context: context,
                ss: ss,
                mcpLogId: copyFrom,
                formData: context.Forms);
            mcpLogModel.McpLogId = 0;
            mcpLogModel.Ver = 1;
            var invalid = McpLogValidators.OnCreating(
                context: context,
                ss: ss,
                mcpLogModel: mcpLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var processes = (List<Process>)null;
            var errorData = mcpLogModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            mcpLogModel: mcpLogModel,
                            processes: processes));
                    return new ResponseCollection(
                        context: context,
                        id: mcpLogModel.McpLogId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .AfterCreate(
                                ss: ss,
                                id: mcpLogModel.McpLogId)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: mcpLogModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = mcpLogModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, long mcpLogId)
        {
            var mcpLogModel = new McpLogModel(
                context: context,
                ss: ss,
                mcpLogId: mcpLogId,
                formData: context.Forms);
            var invalid = McpLogValidators.OnUpdating(
                context: context,
                ss: ss,
                mcpLogModel: mcpLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (mcpLogModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var processes = (List<Process>)null;
            var errorData = mcpLogModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new McpLogsResponseCollection(
                        context: context,
                        mcpLogModel: mcpLogModel);
                    return ResponseByUpdate(res, context, ss, mcpLogModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: mcpLogModel.Comments,
                            verType: mcpLogModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: mcpLogModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            McpLogsResponseCollection res,
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: mcpLogModel);
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
                    where: Rds.McpLogsWhere().McpLogId(mcpLogModel.McpLogId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{mcpLogModel.McpLogId}\"][data-latest]",
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
                        mcpLogModel: mcpLogModel,
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
                    .FieldResponse(context: context, ss: ss, mcpLogModel: mcpLogModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", mcpLogModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(mcpLogModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: mcpLogModel,
                        tableName: "McpLogs"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: mcpLogModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: mcpLogModel.Comments,
                        deleteCommentId: mcpLogModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            McpLogModel mcpLogModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: mcpLogModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = mcpLogModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, long mcpLogId)
        {
            var mcpLogModel = new McpLogModel(context, ss, mcpLogId);
            var invalid = McpLogValidators.OnDeleting(
                context: context,
                ss: ss,
                mcpLogModel: mcpLogModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = mcpLogModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: mcpLogModel.Title.MessageDisplay(context: context)));
                    var res = new McpLogsResponseCollection(
                        context: context,
                        mcpLogModel: mcpLogModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long mcpLogId, Message message = null)
        {
            var mcpLogModel = new McpLogModel(context: context, ss: ss, mcpLogId: mcpLogId);
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
                                    mcpLogModel: mcpLogModel))));
            return new McpLogsResponseCollection(
                context: context,
                mcpLogModel: mcpLogModel)
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
            McpLogModel mcpLogModel)
        {
            if (ss.ColumnHash.ContainsKey("TitleBody") && ss.ColumnHash.ContainsKey("Body"))
            {
                ss.ColumnHash["TitleBody"].ControlType = ss.ColumnHash["Body"].FieldCss == "field-rte" ? "RTEditor" : "MarkDown";
            }
            new McpLogCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.McpLogsWhere().McpLogId(mcpLogModel.McpLogId),
                orderBy: Rds.McpLogsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(mcpLogModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(mcpLogModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: mcpLogModelHistory.Ver == mcpLogModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: mcpLogModelHistory.Ver.ToString(),
                                            _using: mcpLogModelHistory.Ver < mcpLogModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mcpLogModel: mcpLogModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.McpLogsColumnCollection()
                .McpLogId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.McpLogsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long mcpLogId)
        {
            var mcpLogModel = new McpLogModel(context: context, ss: ss, mcpLogId: mcpLogId);
            mcpLogModel.Get(
                context: context,
                ss: ss,
                where: Rds.McpLogsWhere()
                    .McpLogId(mcpLogModel.McpLogId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            mcpLogModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, mcpLogModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        context.Controller,
                        mcpLogId.ToString() 
                            + (mcpLogModel.VerType == Versions.VerTypes.History
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
                ss: SiteSettingsUtilities.McpLogsSiteSettings(context: context),
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
            var invalid = McpLogValidators.OnExporting(
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
            var invalid = McpLogValidators.OnExporting(
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
            new McpLogCollection(
                context: context,
                ss: ss,
                where: view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.McpLogsWhere().TenantId(context.TenantId)),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                top: Parameters.McpServer.LogExportLimit)
                    .ForEach(mcpLogModel =>
                        csv.Append(
                            export.Columns.Select(exportColumn =>
                                mcpLogModel.CsvData(
                                    context: context,
                                    ss: ss,
                                    column: ss.GetColumn(
                                        context: context,
                                        columnName: exportColumn.ColumnName),
                                    exportColumn: exportColumn,
                                    mine: mcpLogModel.Mine(context: context),
                                    encloseDoubleQuotes: true)).Join(),
                            "\n"));
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
                    name: Displays.McpLogs(context: context)),
                encoding: context.Forms.Data("ExportEncoding"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void PhysicalDelete(Context context)
        {
            var chunkSize = Parameters.BackgroundService.DeleteMcpLogsChunkSize;
            var retentionPeriod = Parameters.BackgroundService.McpLogsRetentionPeriod;
            if (chunkSize <= 0)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteMcpLogs(
                        where: Rds.McpLogsWhere().CreatedTime(
                            Times.RetentionDate(retentionPeriod),
                            _operator: "<")));
            }
            else
            {
                var (min, max) = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectMcpLogs(
                            column: Rds.McpLogsColumn()
                                .McpLogs_McpLogId(function: Sqls.Functions.Min, _as: "McpLogIdMin")
                                .McpLogs_McpLogId(function: Sqls.Functions.Max, _as: "McpLogIdMax"),
                            where: Rds.McpLogsWhere().CreatedTime(
                                Times.RetentionDate(retentionPeriod),
                                _operator: "<")))
                    .AsEnumerable()
                    .Select(o => (min: o["McpLogIdMin"].ToLong(), max: o["McpLogIdMax"].ToLong()))
                    .FirstOrDefault((min: 0L, max: 0L));
                if (min != 0 && max != 0)
                {
                    for (var i = min; i <= max; i += chunkSize)
                    {
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.PhysicalDeleteMcpLogs(
                                where: Rds.McpLogsWhere()
                                    .CreatedTime(
                                        Times.RetentionDate(retentionPeriod),
                                        _operator: "<")
                                    .McpLogId_Between(
                                        begin: i,
                                        end: i + chunkSize - 1)));
                    }
                }
            }
        }
    }
}
