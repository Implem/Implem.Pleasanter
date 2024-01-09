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
    public static class DashboardUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
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
                viewModeBody: () => hb.Div(css: "grid-stack"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string viewMode,
            ServerScriptModelRow serverScriptModelRow,
            Action viewModeBody)
        {
            var invalid = DashboardValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var dashboardPartLayouts = ss.DashboardParts
                .Where(dashboardPart => dashboardPart
                    .Accessable(
                        context: context,
                        ss: ss))
                .Select(dashboardPart =>
            {
                dashboardPart.SetSitesData();
                if (ss.DashboardPartsAsynchronousLoading == true
                    && dashboardPart.DisableAsynchronousLoading == false)
                {
                    return AsynchronousLoadingLayout(dashboardPart: dashboardPart);
                }
                switch (dashboardPart.Type)
                {
                    case DashboardPartType.QuickAccess:
                        return QuickAccessLayout(
                            context: context,
                            dashboardPart: dashboardPart);
                    case DashboardPartType.TimeLine:
                        return TimeLineLayout(
                            context: context,
                            ss: ss,
                            dashboardPart: dashboardPart);
                    case DashboardPartType.Custom:
                        return CustomLayouyt(
                            context: context,
                            dashboardPart: dashboardPart);
                    case DashboardPartType.CustomHtml:
                        return CustomHtmlLayouyt(
                            context: context,
                            dashboardPart: dashboardPart);
                    case DashboardPartType.Calendar:
                        return CalendarLayout(
                            context: context,
                            ss: ss,
                            dashboardPart: dashboardPart);
                    case DashboardPartType.Index:
                        return IndexLayout(
                            context: context,
                            dashboardPart: dashboardPart);
                    default:
                        return new DashboardPartLayout();
                };
            }).ToJson();
            var siteModel = new SiteModel(
                context: context,
                siteId: ss.SiteId);
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Dashboards",
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
                                controlId: "DashboardPartLayouts",
                                value: dashboardPartLayouts)
                            .Hidden(
                                controlId: "Sites_Timestamp",
                                css: "control-hidden always-send",
                                value: siteModel.Timestamp))
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSitePackageDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSitePackage(context: context))))
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
            ServerScriptModelRow serverScriptModelRow = null)
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
                            serverScriptModelRow: serverScriptModelRow,
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
                    value: gridData.DataRows.Select(g => g.Long("DashboardId")).ToJson())
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
            return new ResponseCollection(context: context)
                .WindowScrollTop(_using: windowScrollTop)
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridOffset")
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog(_using: offset == 0)
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
                    offset: offset,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.TotalCount))
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("DashboardId")).ToJson())
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
            ServerScriptModelRow serverScriptModelRow = null)
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
                            serverScriptModelRow: serverScriptModelRow))
                .TBody(action: () => hb
                    .GridRows(
                        context: context,
                        ss: ss,
                        view: view,
                        dataRows: gridData.DataRows,
                        columns: columns,
                        checkRow: checkRow));
        }

        private static SqlWhereCollection SelectedWhere(
            Context context,
            SiteSettings ss)
        {
            var selector = new RecordSelector(context: context);
            return !selector.Nothing
                ? Rds.DashboardsWhere().DashboardId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.DashboardsWhere().DashboardId_In(
                    value: recordSelector.Selected?.Select(o => o.ToLong()) ?? new List<long>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long dashboardId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.DashboardsWhere().DashboardId(dashboardId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: dashboardId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{dashboardId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + dashboardId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("DashboardId")}\"][data-latest]",
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
                            idColumn: "DashboardId"))
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
            DashboardModel dashboardModel,
            int? tabIndex = null,
            ServerScriptModelColumn serverScriptModelColumn = null)
        {
            if (serverScriptModelColumn?.Hide == true)
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
                    dashboardModel: dashboardModel);
            }
            else
            {
                var mine = dashboardModel.Mine(context: context);
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
                                    value: dashboardModel.SiteId,
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
                                    value: dashboardModel.UpdatedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "DashboardId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: dashboardModel.DashboardId,
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
                                    value: dashboardModel.Ver,
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
                                    value: dashboardModel.Title,
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
                                    value: dashboardModel.Body,
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
                                    value: dashboardModel.TitleBody,
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
                                    value: dashboardModel.Locked,
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
                                    value: dashboardModel.Comments,
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
                                    value: dashboardModel.Creator,
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
                                    value: dashboardModel.Updator,
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
                                    value: dashboardModel.CreatedTime,
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
                                            value: dashboardModel.GetClass(columnName: column.Name),
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
                                            value: dashboardModel.GetNum(columnName: column.Name),
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
                                            value: dashboardModel.GetDate(columnName: column.Name),
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
                                            value: dashboardModel.GetDescription(columnName: column.Name),
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
                                            value: dashboardModel.GetCheck(columnName: column.Name),
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
                                            value: dashboardModel.GetAttachments(columnName: column.Name),
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
            DashboardModel dashboardModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = dashboardModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = dashboardModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "DashboardId": value = dashboardModel.DashboardId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = dashboardModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = dashboardModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = dashboardModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = dashboardModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "Locked": value = dashboardModel.Locked.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = dashboardModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = dashboardModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = dashboardModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = dashboardModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = dashboardModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = dashboardModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = dashboardModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = dashboardModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = dashboardModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = dashboardModel.GetAttachments(columnName: column.Name).GridText(
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

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long dashboardId, long siteId)
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
                    .Dashboards_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectDashboards(
                        column: Rds.DashboardsColumn().DashboardsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectDashboards(
                            column: Rds.DashboardsColumn().DashboardId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["DashboardId"].ToLong())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(dashboardId))
            {
                switchTargets.Add(dashboardId);
            }
            return switchTargets;
        }

        private static HtmlBuilder ReferenceType(
            this HtmlBuilder hb,
            Context context,
            string referenceType,
            BaseModel.MethodTypes methodType)
        {
            return methodType == BaseModel.MethodTypes.New
                ? hb.Select(
                    attributes: new HtmlAttributes()
                        .Id("Sites_ReferenceType")
                        .Class("control-dropdown"),
                    action: () => hb
                        .OptionCollection(
                            context: context,
                            optionCollection: new Dictionary<string, ControlData>
                            {
                                {
                                    "Sites",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Sites"))
                                },
                                {
                                    "Dashboards",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Dashboards"))
                                },
                                {
                                    "Issues",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Issues"))
                                }
,
                                {
                                    "Results",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Results"))
                                }
,
                                {
                                    "Wikis",
                                    new ControlData(ReferenceTypeDisplayName(
                                        context: context,
                                        referenceType: "Wikis"))
                                }
                            },
                        selectedValue: referenceType))
                : hb.Span(css: "control-text", action: () => hb
                    .Text(text: ReferenceTypeDisplayName(
                        context: context,
                        referenceType: referenceType)));
        }

        private static string ReferenceTypeDisplayName(Context context, string referenceType)
        {
            switch (referenceType)
            {
                case "Sites": return Displays.Folder(context: context);
                case "Dashboards": return Displays.Get(context: context, id: "Dashboards");
                case "Issues": return Displays.Get(context: context, id: "Issues");
                case "Results": return Displays.Get(context: context, id: "Results");
                case "Wikis": return Displays.Get(context: context, id: "Wikis");
                default: return null;
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            DashboardModel dashboardModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: dashboardModel.Title.Value);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = dashboardModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            DashboardModel dashboardModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: dashboardModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = dashboardModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Copy(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            if (siteModel.ParentId == 0
                && Permissions.SiteTopPermission(context: context) != (Permissions.Types)Parameters.Permissions.Manager)
            {
                return Error.Types.HasNotPermission.MessageJson(context: context);
            }
            siteModel.Title.Value += Parameters.General.CharToAddWhenCopying;
            if (!context.Forms.Bool("CopyWithComments"))
            {
                siteModel.Comments.Clear();
            }
            if (!context.Forms.Bool("CopyWithNotifications")
                || Parameters.Notification.CopyWithNotifications == ParameterAccessor.Parts.Types.OptionTypes.Disabled)
            {
                ss.Notifications.Clear();
            }
            if (!context.Forms.Bool("CopyWithReminders")
                || Parameters.Reminder.CopyWithReminders == ParameterAccessor.Parts.Types.OptionTypes.Disabled)
            {
                ss.Reminders.Clear();
            }
            var beforeSiteId = siteModel.SiteId;
            var beforeInheritPermission = siteModel.InheritPermission;
            var errorData = siteModel.Create(context: context, otherInitValue: true);
            if (siteModel.SiteSettings.Exports?.Any() == true)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateSites(
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(siteModel.SiteId),
                        param: Rds.SitesParam()
                            .SiteSettings(siteModel.SiteSettings.RecordingJson(
                                context: context))));
            }
            if (beforeSiteId == beforeInheritPermission)
            {
                var dataTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectPermissions(
                        column: Rds.PermissionsColumn()
                            .ReferenceId()
                            .DeptId()
                            .GroupId()
                            .UserId()
                            .PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceId(beforeSiteId)));
                var statements = new List<SqlStatement>();
                dataTable
                    .AsEnumerable()
                    .ForEach(dataRow =>
                        statements.Add(Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(siteModel.SiteId)
                                .DeptId(dataRow.Long("DeptId"))
                                .GroupId(dataRow.Long("GroupId"))
                                .UserId(dataRow.Long("UserId"))
                                .PermissionType(dataRow.Long("PermissionType")))));
                statements.Add(
                    Rds.UpdateSites(
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(siteModel.SiteId),
                        param: Rds.SitesParam()
                            .InheritPermission(siteModel.SiteId)));
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: statements.ToArray());
            }
            SessionUtilities.Set(
                context: context,
                message: Messages.Copied(context: context));
            var res = new ResponseCollection(context: context)
                .SetMemory("formChanged", false)
                .Href(Locations.ItemEdit(
                    context: context,
                    id: siteModel.SiteId));
            return res.ToJson();
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
            List<long> selected,
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
            var where = Rds.SitesWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Sites" + tableName)
                .ParentId(
                    value: ss.SiteId,
                    tableName: "Sites" + tableName)
                .SiteId_In(
                    value: selected,
                    tableName: "Sites" + tableName,
                    negative: negative,
                    _using: selected.Any());
            var sub = Rds.SelectSites(
                tableType: tableType,
                _as: "Sites" + tableName,
                column: Rds.SitesColumn()
                    .SiteId(tableName: "Sites" + tableName),
                where: where);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteItems(
                        tableType: tableType,
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(sub:
                                Rds.SelectWikis(
                                    tableType: tableType,
                                    column: Rds.WikisColumn().WikiId(),
                                    where: Rds.WikisWhere().SiteId_In(sub: sub)))
                            .ReferenceType("Wikis")),
                    Rds.PhysicalDeleteWikis(
                        tableType: tableType,
                        where: Rds.WikisWhere().SiteId_In(sub: sub)),
                    Rds.PhysicalDeleteItems(
                        tableType: tableType,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: tableType,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteSites(
                        tableType: tableType,
                        where: where),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DashboardPartJson(
            Context context,
            SiteSettings ss,
            string dashboardPartId)
        {
            var dashboardPartLayout = ss.DashboardParts
                .Where(dashboardPart => dashboardPart
                    .Accessable(
                        context: context,
                        ss: ss)
                    && dashboardPart.Id == dashboardPartId.ToInt())
                .Select(dashboardPart =>
                {
                    dashboardPart.SetSitesData();
                    switch (dashboardPart.Type)
                    {
                        case DashboardPartType.QuickAccess:
                            return QuickAccessLayout(
                                context: context,
                                dashboardPart: dashboardPart).Content;
                        case DashboardPartType.TimeLine:
                            return TimeLineLayout(
                                context: context,
                                ss: ss,
                                dashboardPart: dashboardPart).Content;
                        case DashboardPartType.Custom:
                            return CustomLayouyt(
                                context: context,
                                dashboardPart: dashboardPart).Content;
                        case DashboardPartType.CustomHtml:
                            return CustomHtmlLayouyt(
                                context: context,
                                dashboardPart: dashboardPart).Content;
                        case DashboardPartType.Calendar:
                            return CalendarLayout(
                                context: context,
                                ss: ss,
                                dashboardPart: dashboardPart).Content;
                        case DashboardPartType.Index:
                            return IndexLayout(
                                context: context,
                                dashboardPart: dashboardPart).Content;
                        default:
                            return null;
                    }
                }).ToList();
            return new ResponseCollection(context: context)
                .ReplaceAll(
                    target: $"#DashboardPart_{dashboardPartId}",
                    value: dashboardPartLayout.FirstOrDefault())
                .Invoke("setCalendar", dashboardPartId.ToString())
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ReplaceLineByDashboardModel(
            this DashboardModel dashboardModel,
            Context context,
            SiteSettings ss,
            string line,
            string itemTitle)
        {
            ss.IncludedColumns(line).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        line = line.Replace("[Title]", itemTitle);
                        break;
                    case "SiteId":
                        line = line.Replace(
                            "[SiteId]",
                            dashboardModel.SiteId.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "UpdatedTime":
                        line = line.Replace(
                            "[UpdatedTime]",
                            dashboardModel.UpdatedTime.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "DashboardId":
                        line = line.Replace(
                            "[DashboardId]",
                            dashboardModel.DashboardId.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Ver":
                        line = line.Replace(
                            "[Ver]",
                            dashboardModel.Ver.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Body":
                        line = line.Replace(
                            "[Body]",
                            dashboardModel.Body.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Locked":
                        line = line.Replace(
                            "[Locked]",
                            dashboardModel.Locked.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Comments":
                        line = line.Replace(
                            "[Comments]",
                            dashboardModel.Comments.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Creator":
                        line = line.Replace(
                            "[Creator]",
                            dashboardModel.Creator.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "Updator":
                        line = line.Replace(
                            "[Updator]",
                            dashboardModel.Updator.ToExport(
                                context: context,
                                column: column));
                        break;
                    case "CreatedTime":
                        line = line.Replace(
                            "[CreatedTime]",
                            dashboardModel.CreatedTime.ToExport(
                                context: context,
                                column: column));
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                line = line.Replace(
                                    $"[{column.Name}]",
                                    dashboardModel.GetClass(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Num":
                                line = line.Replace(
                                    $"[{column.Name}]",
                                    dashboardModel.GetNum(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Date":
                                line = line.Replace(
                                    $"[{column.Name}]",
                                    dashboardModel.GetDate(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Description":
                                line = line.Replace(
                                    $"[{column.Name}]",
                                    dashboardModel.GetDescription(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                            case "Check":
                                line = line.Replace(
                                    $"[{column.Name}]",
                                    dashboardModel.GetCheck(column: column).ToExport(
                                        context: context,
                                        column: column));
                                break;
                        }
                        break;
                }
            });
            return line;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout CustomHtmlLayouyt(Context context, DashboardPart dashboardPart)
        {
            var content = new HtmlBuilder()
                .CustomHtml(context: context, dashboardPart: dashboardPart).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = content
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout CustomLayouyt(Context context, DashboardPart dashboardPart)
        {
            var content = new HtmlBuilder()
                .Custom(context: context, dashboardPart: dashboardPart).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = content
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Custom(this HtmlBuilder hb, Context context, DashboardPart dashboardPart)
        {
            return hb.Div(
                id: $"DashboardPart_{dashboardPart.Id}",
                css: dashboardPart.ExtendedCss,
                attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                action: () =>
                {
                    if (dashboardPart.ShowTitle == true)
                    {
                        hb.Div(
                            css: "dashboard-part-title",
                            action: () => hb.Text(dashboardPart.Title));
                    }
                    hb
                        .Div(
                            css: "dashboard-custom-body markup",
                            action: () => hb.Text(text: dashboardPart.Content));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CustomHtml(this HtmlBuilder hb, Context context, DashboardPart dashboardPart)
        {
            return hb.Div(
                id: $"DashboardPart_{dashboardPart.Id}",
                css: dashboardPart.ExtendedCss,
                attributes: new HtmlAttributes()
                    .DataId(dashboardPart.Id.ToString()),
                action: () =>
                {
                    if (dashboardPart.ShowTitle == true)
                    {
                        hb.Div(
                            css: "dashboard-part-title",
                            action: () => hb.Text(dashboardPart.Title));
                    }
                    hb
                        .Div(
                            css: "dashboard-custom-html-body",
                            action: () => hb.Raw(text: dashboardPart.HtmlContent));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout QuickAccessLayout(Context context, DashboardPart dashboardPart)
        {
            var content = new HtmlBuilder()
                .QuickAccessMenu(
                    context: context,
                    dashboardPart)
                .ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = content
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder QuickAccessMenu(this HtmlBuilder hb, Context context, DashboardPart dashboardPart)
        {
            var sites = DashboardPart.GetQuickAccessSites(
                context: context,
                dashboardPart.QuickAccessSitesData);
            return hb.Div(
                id: $"DashboardPart_{dashboardPart.Id}",
                css: dashboardPart.ExtendedCss,
                attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                action: () =>
                {
                    if (dashboardPart.ShowTitle == true)
                    {
                        hb.Div(
                            css: "dashboard-part-title",
                            action: () => hb.Text(dashboardPart.Title));
                    }
                    hb
                        .Nav(css: "dashboard-part-nav",
                            action: () => hb
                                .Ul(
                                    css: dashboardPart.QuickAccessLayout == Libraries.Settings.QuickAccessLayout.Vertical
                                        ? "dashboard-part-nav-menu-vertical" : "dashboard-part-nav-menu",
                                    action: () => QuickAccessSites(context: context, sites: sites)
                                        .ForEach(quickAccess =>
                                        {
                                            if (quickAccess.Model.SiteId == 0)
                                            {
                                                quickAccess.Model.Title = new Title() { DisplayValue = Displays.Top(context: context) };
                                                quickAccess.Model.ReferenceType = "Sites";
                                            }
                                            var itemTypeCss = string.Empty;
                                            var iconName = string.Empty;
                                            switch (quickAccess.Model.ReferenceType)
                                            {
                                                case "Sites":
                                                    itemTypeCss = " dashboard-part-nav-folder " + quickAccess.Css;
                                                    iconName = Strings.CoalesceEmpty(quickAccess.Icon, "folder");
                                                    break;
                                                case "Dashboards":
                                                    itemTypeCss = " dashboard-part-nav-dashboard " + quickAccess.Css;
                                                    iconName = Strings.CoalesceEmpty(quickAccess.Icon, "dashboard");
                                                    break;
                                                case "Wikis":
                                                    itemTypeCss = " dashboard-part-nav-wiki " + quickAccess.Css;
                                                    iconName = Strings.CoalesceEmpty(quickAccess.Icon, "text_snippet");
                                                    break;
                                                default:
                                                    itemTypeCss = " dashboard-part-nav-table " + quickAccess.Css;
                                                    iconName = Strings.CoalesceEmpty(quickAccess.Icon, "table");
                                                    break;
                                            }
                                            hb.Li(css: "dashboard-part-nav-item" + itemTypeCss.TrimEnd(),
                                                action: () => hb
                                                    .Span(css: "material-symbols-outlined",
                                                        action: () => hb.Text(iconName))
                                                    .A(
                                                        css: "dashboard-part-nav-link",
                                                        text: quickAccess.Model.Title.DisplayValue,
                                                        href: quickAccess.Model.ReferenceType == "Wikis"
                                                            ? Locations.ItemEdit(
                                                                context: context,
                                                                id: Repository.ExecuteScalar_long(
                                                                    context: context,
                                                                    statements: Rds.SelectWikis(
                                                                        column: Rds.WikisColumn().WikiId(),
                                                                        where: Rds.WikisWhere().SiteId(quickAccess.Model.SiteId))))
                                                            : Locations.ItemIndex(
                                                                context: context,
                                                                id: quickAccess.Model.SiteId)));
                                        })));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<QuickAccessSiteModel> QuickAccessSites(
            Context context,
            IEnumerable<(long Id, string Icon, string Css)> sites)
        {
            var siteModels = new SiteCollection(
                context: context,
                column: Rds.SitesColumn()
                    .SiteId()
                    .Title()
                    .ReferenceType()
                    .SiteSettings(),
                where: Rds.SitesWhere()
                    .TenantId(context.TenantId)
                    .SiteId_In(sites.Select(o => o.Id))
                    .Add(
                        raw: Def.Sql.HasPermission,
                        _using: !context.HasPrivilege));
            return sites
                .Select(o => o.Id == 0
                    ? new QuickAccessSiteModel()
                    {
                        Model = new SiteModel(context: context, siteId: 0),
                        Icon = o.Icon,
                        Css = o.Css
                    }
                    : new QuickAccessSiteModel()
                    {
                        Model = siteModels.FirstOrDefault(model => model.SiteId == o.Id),
                        Icon = o.Icon,
                        Css = o.Css
                    })
                .Where(model => model.Model != null);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout TimeLineLayout(
            Context context,
            SiteSettings ss,
            DashboardPart dashboardPart)
        {
            var timeLineItems = GetTimeLineRecords(
                context: context,
                dashboardPart: dashboardPart);
            var hb = new HtmlBuilder();
            var timeLine = hb
                .Div(
                    id: $"DashboardPart_{dashboardPart.Id}",
                    attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                    css: "dashboard-timeline-container " + dashboardPart.ExtendedCss,
                    action: () =>
                    {
                        if (dashboardPart.ShowTitle == true)
                        {
                            hb.Div(
                                css: "dashboard-part-title",
                                action: () => hb.Text(dashboardPart.Title));
                        }
                        foreach (var item in timeLineItems)
                        {
                            hb.TimeLineItem(
                                context: context,
                                item: item,
                                dashboardPart.TimeLineDisplayType
                                    ?? TimeLineDisplayType.Standard);
                        }
                    }).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = timeLine
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void TimeLineItem(this HtmlBuilder hb, Context context, DashboardTimeLineItem item, TimeLineDisplayType displayType)
        {
            hb.Div(
                css: "dashboard-timeline-item",
                attributes: new HtmlAttributes()
                    .Add(
                        "data-url",
                        Locations.Edit(
                            context: context,
                            "items",
                            item.Id)),
                action: () =>
                {
                    hb
                        .Div(
                            css: "dashboard-timeline-header"
                                + (displayType == TimeLineDisplayType.Simple
                                    ? " dashboard-timeline-header-closed"
                                    : ""),
                            action: () =>
                            {
                                hb
                                    .A(
                                        text: item.SiteTitle.Title(context: context),
                                        href: Locations.ItemIndex(
                                            context: context,
                                            id: item.SiteId))
                                    .Div(css: "dashboard-timeline-record-time",
                                        action: () =>
                                        {
                                            if (item.UpdatedTime.Value > item.CreatedTime.Value)
                                            {
                                                hb.UpdatedInfo(
                                                    context: context,
                                                    item.UpdatedTime);
                                            }
                                            else
                                            {
                                                hb.CreatedInfo(
                                                    context: context,
                                                    item.CreatedTime);
                                            }
                                        });
                            })
                        .Div(
                            css: "dashboard-timeline-titlebody",
                            action: () => hb
                                .Div(
                                    css: "dashboard-timeline-title",
                                    action: () => hb.Text(item.Title))
                                .Div(
                                    css: "dashboard-timeline-body markup"
                                        + (displayType != TimeLineDisplayType.Detailed
                                        ? " dashboard-timeline-body-closed"
                                        : ""),
                                    action: () => hb.Text(text: item.Body)));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<DashboardTimeLineItem> GetTimeLineRecords(Context context, DashboardPart dashboardPart)
        {
            //基準となるサイトからSiteSettingsを取得
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: dashboardPart.SiteId);
            //対象サイトをサイト統合の仕組みで登録
            ss.IntegratedSites = dashboardPart.TimeLineSitesData;
            ss.SetSiteIntegration(context: context);
            //Viewからフィルタ条件とソート条件を取得
            var where = dashboardPart.View.Where(
                context: context,
                ss: ss);
            var orderBy = dashboardPart.View.OrderBy(
                context: context,
                ss: ss);
            if (ss.ReferenceType == "Issues")
            {
                return GetTimeLineIssues(
                    context: context,
                    ss: ss,
                    where: where,
                    orderBy: orderBy,
                    titleTemplate: dashboardPart.TimeLineTitle,
                    bodyTemplate: dashboardPart.TimeLineBody,
                    top: dashboardPart.TimeLineItemCount);
            }
            else if (ss.ReferenceType == "Results")
            {
                return GetTimeLineResults(
                    context: context,
                    ss: ss,
                    where: where,
                    orderBy: orderBy,
                    titleTemplate: dashboardPart.TimeLineTitle,
                    bodyTemplate: dashboardPart.TimeLineBody,
                    top: dashboardPart.TimeLineItemCount);
            }
            else
            {
                return Enumerable.Empty<DashboardTimeLineItem>();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<DashboardTimeLineItem> GetTimeLineResults(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            string titleTemplate,
            string bodyTemplate,
            int top)
        {
            var results = new ResultCollection(
                context: context,
                ss: ss,
                top: top,
                where: where,
                orderBy: orderBy,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                            where,
                            orderBy
                    }
                ));
            var title = ss.LabelTextToColumnName(titleTemplate);
            var body = ss.LabelTextToColumnName(bodyTemplate);
            //表示対象のサイトID一覧から、サイト設定の辞書を作成（Key: SiteId,Value: SiteSettings）
            //JoinedSsHashにある場合はそちらを利用し、ない場合は作成する
            var ssHash = ss.AllowedIntegratedSites?.ToDictionary(
                siteId => siteId,
                siteId => ss.JoinedSsHash?.Get(siteId)
                    ?? SiteSettingsUtilities.Get(
                        context: context,
                        siteId: siteId))
                        ?? new Dictionary<long, SiteSettings>();
            return results
                .Select(model =>
                {
                    //表示するレコードのサイトIDをキーにサイト設定を取得
                    var currentSs = ssHash.Get(model.SiteId);
                    //カラムの置換処理
                    // currentSsは必ず取得できる想定だが、取得できなかった場合はカラムの置き換えを行わず設定されたテキストをそのまま出力する
                    var replacedTitle = currentSs != null
                        ? model.ReplaceLineByResultModel(
                            context: context,
                            ss: currentSs,
                            line: title,
                            itemTitle: model.Title.DisplayValue)
                        : title;
                    var replacedBody = currentSs != null
                        ? model.ReplaceLineByResultModel(
                            context: context,
                            ss: currentSs,
                            line: body,
                            itemTitle: model.Title.DisplayValue)
                        : body;
                    return new DashboardTimeLineItem
                    {
                        Id = model.ResultId,
                        SiteId = model.SiteId,
                        SiteTitle = model.SiteTitle,
                        Title = replacedTitle,
                        Body = replacedBody,
                        CreatedTime = model.CreatedTime,
                        UpdatedTime = model.UpdatedTime,
                        Creator = model.Creator,
                        Updator = model.Updator
                    };
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<DashboardTimeLineItem> GetTimeLineIssues(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            SqlOrderByCollection orderBy,
            string titleTemplate,
            string bodyTemplate,
            int top)
        {
            var issues = new IssueCollection(
                context: context,
                ss: ss,
                top: top,
                where: where,
                orderBy: orderBy,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                            where,
                            orderBy
                    }
                ));
            var title = ss.LabelTextToColumnName(titleTemplate);
            var body = ss.LabelTextToColumnName(bodyTemplate);
            //表示対象のサイトID一覧から、サイト設定の辞書を作成（Key: SiteId,Value: SiteSettings）
            var ssHash = ss.AllowedIntegratedSites?.ToDictionary(
                siteId => siteId,
                siteId => SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId))
                ?? new Dictionary<long, SiteSettings>();
            return issues
                .Select(model =>
                {
                    //表示するレコードのサイトIDをキーにサイト設定を取得
                    var currentSs = ssHash.TryGetValue(model.SiteId, out var _Ss)
                        ? _Ss
                        : null;
                    //カラムの置換処理
                    // currentSsは必ず取得できる想定だが、取得できなかった場合はカラムの置き換えを行わず設定されたテキストをそのまま出力する
                    var replacedTitle = currentSs != null
                        ? model.ReplaceLineByIssueModel(
                            context: context,
                            ss: currentSs,
                            line: title,
                            itemTitle: model.Title.DisplayValue,
                            checkColumnAccessControl: true)
                        : title;
                    var replacedBody = currentSs != null
                        ? model.ReplaceLineByIssueModel(
                            context: context,
                            ss: currentSs,
                            line: body,
                            itemTitle: model.Title.DisplayValue,
                            checkColumnAccessControl: true)
                        : body;
                    return new DashboardTimeLineItem
                    {
                        Id = model.IssueId,
                        SiteId = model.SiteId,
                        SiteTitle = model.SiteTitle,
                        Title = replacedTitle,
                        Body = replacedBody,
                        CreatedTime = model.CreatedTime,
                        UpdatedTime = model.UpdatedTime,
                        Creator = model.Creator,
                        Updator = model.Updator
                    };
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout CalendarLayout(
            Context context,
            SiteSettings ss,
            DashboardPart dashboardPart)
        {
            var hb = new HtmlBuilder();
            var calendarHtml = GetCalendarRecords(
                context: context,
                dashboardPart: dashboardPart);
            var calendar = hb
                .Div(
                    id: $"DashboardPart_{dashboardPart.Id}",
                    attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                    css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                    action: () =>
                    {
                        if (dashboardPart.ShowTitle == true)
                        {
                            hb.Div(
                                css: "dashboard-part-title",
                                action: () => hb.Text(dashboardPart.Title));
                        }
                        hb.Raw(text: calendarHtml);
                    }).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = calendar
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string GetCalendarRecords(
            Context context,
            DashboardPart dashboardPart)
        {
            //基準となるサイトからSiteSettingsを取得
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: dashboardPart.SiteId);
            //対象サイトをサイト統合の仕組みで登録
            ss.IntegratedSites = dashboardPart.CalendarSitesData;
            ss.SetSiteIntegration(context: context);
            ss.SetDashboardParts(dashboardPart: dashboardPart);
            if (ss.ReferenceType == "Issues")
            {
                return IssueUtilities.Calendar(context: context, ss: ss);
            }
            else if (ss.ReferenceType == "Results")
            {
                return ResultUtilities.Calendar(context: context, ss: ss);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CalendarJson(Context context, SiteSettings ss)
        {
            var matchingKey = context.Forms.Keys.FirstOrDefault(x => x.StartsWith("CalendarSuffix_"));
            DashboardPart dashboardPart = ss.DashboardParts.FirstOrDefault(x => x.Id == context.Forms.Data(matchingKey).ToInt());
            if (dashboardPart == null && context.Forms.ControlId().StartsWith("CalendarDate_"))
            {
                var suffix = context.Forms.ControlId().Replace("CalendarDate_", "");
                dashboardPart = ss.DashboardParts.FirstOrDefault(x => x.Id == suffix.ToInt());
            }
            var currentSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: dashboardPart.SiteId);
            //対象サイトをサイト統合の仕組みで登録
            currentSs.IntegratedSites = dashboardPart.CalendarSitesData;
            currentSs.SetSiteIntegration(context: context);
            currentSs.SetDashboardParts(dashboardPart: dashboardPart);
            var hb = new HtmlBuilder();
            switch (currentSs.ReferenceType)
            {
                case "Issues":
                    var issues = hb.Div(
                        id: $"DashboardPart_{dashboardPart.Id}",
                        attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                        css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                        action: () =>
                        {
                            if (dashboardPart.ShowTitle == true)
                            {
                                hb.Div(
                                    css: "dashboard-part-title",
                                    action: () => hb.Text(dashboardPart.Title));
                            }
                            hb.Raw(text: IssueUtilities.CalendarJson(context: context, ss: currentSs));
                        }).ToString();
                    return new ResponseCollection(context: context)
                        .ReplaceAll(
                            target: $"#DashboardPart_{dashboardPart.Id}",
                            value: issues)
                        .Invoke("setCalendar",dashboardPart.Id.ToString())
                        .ToJson();
                case "Results":
                    var results = hb.Div(
                        id: $"DashboardPart_{dashboardPart.Id}",
                        attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                        css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                        action: () =>
                        {
                            if (dashboardPart.ShowTitle == true)
                            {
                                hb.Div(
                                    css: "dashboard-part-title",
                                    action: () => hb.Text(dashboardPart.Title));
                            }
                            hb.Raw(text: ResultUtilities.CalendarJson(context: context, ss: currentSs));
                        }).ToString();
                    return new ResponseCollection(context: context)
                        .ReplaceAll(
                            target: $"#DashboardPart_{dashboardPart.Id}",
                            value: results)
                        .Invoke("setCalendar", dashboardPart.Id.ToString())
                        .ToJson();
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateByCalendar(
            Context context,
            SiteSettings ss)
        {
            var matchingKey = context.Forms.Keys.FirstOrDefault(x => x.StartsWith("CalendarSuffix_"));
            DashboardPart dashboardPart = ss.DashboardParts.FirstOrDefault(x => x.Id == context.Forms.Data(matchingKey).ToInt());
            var currentSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: context.Forms.Long("SiteId"));
            //対象サイトをサイト統合の仕組みで登録
            currentSs.IntegratedSites = dashboardPart.CalendarSitesData;
            currentSs.SetSiteIntegration(context: context);
            currentSs.SetDashboardParts(dashboardPart: dashboardPart);
            var hb = new HtmlBuilder();
            switch (currentSs.ReferenceType)
            {
                case "Issues":
                    var issues = IssueUtilities.UpdateByCalendar(context: context, ss: currentSs);
                    var issueCalendar = hb.Div(
                        id: $"DashboardPart_{dashboardPart.Id}",
                        attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                        css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                        action: () =>
                        {
                            if (dashboardPart.ShowTitle == true)
                            {
                                hb.Div(
                                    css: "dashboard-part-title",
                                    action: () => hb.Text(dashboardPart.Title));
                            }
                            hb.Raw(text: issues);
                        }).ToString();
                    List<Dictionary<string, string>> issuesJson = Jsons.Deserialize<List<Dictionary<string, string>>>(issues);
                    if (issuesJson != null)
                    {
                        Dictionary<string, string> messageElement = issuesJson.Find(item => item.ContainsValue("Message"));
                        if (messageElement != null)
                        {
                            Dictionary<string, string> messageValue = Jsons.Deserialize<Dictionary<string, string>>(messageElement["Value"]);
                            return new ResponseCollection(context: context)
                                .Message(message: new Message(messageValue["Id"], messageValue["Text"], messageValue["Css"]))
                                .Invoke("setCalendar", dashboardPart.Id.ToString())
                                .ToJson();
                        }
                    }
                    var issueModel = new IssueModel(
                        context: context,
                        ss: currentSs,
                        issueId: context.Forms.Long("EventId"),
                        formData: context.Forms);
                    return new ResponseCollection(context: context)
                        .ReplaceAll(
                            target: $"#DashboardPart_{dashboardPart.Id}",
                            value: issueCalendar)
                        .Message(context.ErrorData.Type != Error.Types.None
                            ? context.ErrorData.Message(context: context)
                            : Messages.Updated(
                                context: context,
                                data: issueModel.Title.MessageDisplay(context: context)))
                        .Invoke("setCalendar", dashboardPart.Id.ToString())
                        .ToJson();
                case "Results":
                    var results = ResultUtilities.UpdateByCalendar(context: context, ss: currentSs);
                    var resultCalendar = hb.Div(
                        id: $"DashboardPart_{dashboardPart.Id}",
                        attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                        css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                        action: () =>
                        {
                            if (dashboardPart.ShowTitle == true)
                            {
                                hb.Div(
                                    css: "dashboard-part-title",
                                    action: () => hb.Text(dashboardPart.Title));
                            }
                            hb.Raw(text: results);
                        }).ToString();
                    List<Dictionary<string, string>> resultsJson = Jsons.Deserialize<List<Dictionary<string, string>>>(results);
                    if (resultsJson != null)
                    {
                        Dictionary<string, string> messageElement = resultsJson.Find(item => item.ContainsValue("Message"));
                        if (messageElement != null)
                        {
                            Dictionary<string, string> messageValue = Jsons.Deserialize<Dictionary<string, string>>(messageElement["Value"]);
                            return new ResponseCollection(context: context)
                                .Message(message: new Message(messageValue["Id"], messageValue["Text"], messageValue["Css"]))
                                .Invoke("setCalendar", dashboardPart.Id.ToString())
                                .ToJson();
                        }
                    }
                    var resultModel = new ResultModel(
                        context: context,
                        ss: currentSs,
                        resultId: context.Forms.Long("EventId"),
                        formData: context.Forms);
                    return new ResponseCollection(context: context)
                        .ReplaceAll(
                            target: $"#DashboardPart_{dashboardPart.Id}",
                            value: resultCalendar)
                        .Message(context.ErrorData.Type != Error.Types.None
                            ? context.ErrorData.Message(context: context)
                            : Messages.Updated(
                                context: context,
                                data: resultModel.Title.MessageDisplay(context: context)))
                        .Invoke("setCalendar", dashboardPart.Id.ToString())
                        .ToJson();
                default:
                    return Messages.ResponseNotFound(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DashboardPartLayout AsynchronousLoadingLayout(DashboardPart dashboardPart)
        {
            var hb = new HtmlBuilder();
            var AsynchronousLoading = hb.Div(
                id: $"DashboardPart_{dashboardPart.Id}",
                attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                css: "dashboard-calendar-container " + dashboardPart.ExtendedCss,
                action: () =>
                {
                    if (dashboardPart.ShowTitle == true)
                    {
                        hb.Div(
                            css: "dashboard-part-title",
                            action: () => hb.Text(dashboardPart.Title));
                    }
                    hb.Hidden(controlId: $"DashboardAsync_{dashboardPart.Id}");
                }).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = AsynchronousLoading
            };
        }

        private static DashboardPartLayout IndexLayout(
            Context context,
            DashboardPart dashboardPart)
        {
            var hb = new HtmlBuilder();
            var indexHtml = GetIndexRecords(
                context: context,
                dashboardPart: dashboardPart);
            var index = hb
                .Div(
                    id: $"DashboardPart_{dashboardPart.Id}",
                    attributes: new HtmlAttributes().DataId(dashboardPart.Id.ToString()),
                    css: "dashboard-index-container " + dashboardPart.ExtendedCss,
                    action: () =>
                    {
                        if (dashboardPart.ShowTitle == true)
                        {
                            hb.Div(
                                css: "dashboard-part-title",
                                action: () => hb.Text(dashboardPart.Title));
                        }
                        hb.Raw(text: indexHtml);
                    }).ToString();
            return new DashboardPartLayout()
            {
                Id = dashboardPart.Id,
                X = dashboardPart.X,
                Y = dashboardPart.Y,
                W = dashboardPart.Width,
                H = dashboardPart.Height,
                Content = index
            };
        }

        private static string GetIndexRecords(
            Context context,
            DashboardPart dashboardPart)
        {
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: dashboardPart.SiteId);
            //対象サイトをサイト統合の仕組みで登録
            ss.IntegratedSites = dashboardPart.IndexSitesData;
            ss.SetSiteIntegration(context: context);
            ss.SetDashboardParts(dashboardPart: dashboardPart);
            if (ss.ReferenceType == "Issues")
            {
                return IssueUtilities.Index(context: context, ss: ss);
            }
            else if (ss.ReferenceType == "Results")
            {
                return ResultUtilities.Index(context: context, ss: ss);
            }
            else
            {
                return null;
            }
        }
    }
}
