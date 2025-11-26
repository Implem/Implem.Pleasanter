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
using Implem.Pleasanter.Models.ApiSiteSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class SiteUtilities
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
            var invalid = SiteValidators.OnEntry(
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
                referenceType: "Sites",
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
                    .EditorDialog(context: context, ss: ss)
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
                where: Rds.SitesWhere().TenantId(context.TenantId),
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
                    value: gridData.DataRows.Select(g => g.Long("SiteId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("SiteId")).ToJson())
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
                ? Rds.SitesWhere().SiteId_In(
                    value: selector.Selected.Select(o => o.ToLong()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.SitesWhere().SiteId_In(
                    value: recordSelector.Selected?.Select(o => o.ToLong()) ?? new List<long>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long siteId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.SitesWhere().SiteId(siteId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: siteId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{siteId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + siteId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("SiteId")}\"][data-latest]",
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
                            idColumn: "SiteId"))
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
            SiteModel siteModel,
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
                    siteModel: siteModel);
            }
            else
            {
                var mine = siteModel.Mine(context: context);
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
                                    value: siteModel.SiteId,
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
                                    value: siteModel.UpdatedTime,
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
                                    value: siteModel.Ver,
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
                                    value: siteModel.Title,
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
                                    value: siteModel.Body,
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
                                    value: siteModel.TitleBody,
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
                                    value: siteModel.Comments,
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
                                    value: siteModel.Creator,
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
                                    value: siteModel.Updator,
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
                                    value: siteModel.CreatedTime,
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
                                            value: siteModel.GetClass(columnName: column.Name),
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
                                            value: siteModel.GetNum(columnName: column.Name),
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
                                            value: siteModel.GetDate(columnName: column.Name),
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
                                            value: siteModel.GetDescription(columnName: column.Name),
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
                                            value: siteModel.GetCheck(columnName: column.Name),
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
                                            value: siteModel.GetAttachments(columnName: column.Name),
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
            SiteModel siteModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = siteModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = siteModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = siteModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = siteModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = siteModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = siteModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = siteModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = siteModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = siteModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = siteModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = siteModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = siteModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = siteModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = siteModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = siteModel.GetAttachments(columnName: column.Name).GridText(
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

        public static string EditorJson(Context context, SiteModel siteModel)
        {
            siteModel.ClearSessions(context: context);
            return EditorResponse(context: context, siteModel: siteModel).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteModel siteModel,
            Message message = null,
            string switchTargets = null)
        {
            siteModel.MethodType = siteModel.SiteId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new SitesResponseCollection(
                context: context,
                siteModel: siteModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, siteModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
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

        internal static string ReferenceTypeDisplayName(Context context, string referenceType)
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

        public static string Create(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission,
                formData: context.Forms);
            var ss = siteModel.SitesSiteSettings(
                context: context,
                referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var processes = (List<Process>)null;
            var errorData = siteModel.Create(context: context);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            siteModel: siteModel,
                            processes: processes));
                    return new ResponseCollection(context: context)
                        .Response("id", siteModel.SiteId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: siteModel.ReferenceType == "Wikis"
                                ? Repository.ExecuteScalar_long(
                                    context: context,
                                    statements: Rds.SelectWikis(
                                        column: Rds.WikisColumn().WikiId(),
                                        where: Rds.WikisWhere().SiteId(siteModel.SiteId)))
                                : siteModel.SiteId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            SiteModel siteModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: siteModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = siteModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteModel siteModel, long siteId)
        {
            siteModel.SetByForm(
                context: context,
                formData: context.Forms);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteId);
            var ss = siteModel.SiteSettings.SiteSettingsOnUpdate(context: context);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var processes = (List<Process>)null;
            if (context.Forms.Exists("InheritPermission"))
            {
                // アクセス権を継承しないを指定している状態でCurrentPermissionsAllがクライアントから
                // 届かない場合、アクセス権が全て失われてしまうため、エラーで処理を中断する
                // クライアント操作のタイミングにより発生し、テーブルにアクセスできなくなる問題の対応
                if (context.Forms.Long("InheritPermission") == siteId
                    && !context.Forms.Exists("CurrentPermissionsAll"))
                {
                    return Messages.ResponseRequireManagePermission(context: context).ToJson();
                }
                siteModel.InheritPermission = context.Forms.Long("InheritPermission");
                ss.InheritPermission = siteModel.InheritPermission;
            }
            if (context.Forms.Exists("CurrentPermissionsAll")
                && Parameters.Permissions.CheckManagePermission)
            {
                if (!new PermissionCollection(
                    context: context,
                    referenceId: siteModel.SiteId,
                    permissions: context.Forms.List("CurrentPermissionsAll"))
                        .Any(permission =>
                            permission.PermissionType.HasFlag(
                                Permissions.Types.ManagePermission
                                | Permissions.Types.ManageSite)))
                {
                    return Messages.ResponseRequireManagePermission(context: context).ToJson();
                }
            }
            var errorData = siteModel.Update(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new SitesResponseCollection(
                        context: context,
                        siteModel: siteModel);
                    ss.Publish = siteModel.Publish;
                    res
                        .ReplaceAll("#Breadcrumb", new HtmlBuilder().Breadcrumb(
                            context: context,
                            ss: ss))
                        .ReplaceAll("#Warnings", new HtmlBuilder().Warnings(
                            context: context,
                            ss: ss));
                    return ResponseByUpdate(res, context, siteModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: siteModel.Comments,
                            verType: siteModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: siteModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            SitesResponseCollection res,
            Context context,
            SiteModel siteModel,
            List<Process> processes)
        {
            var ss = siteModel.SiteSettings;
            ss.ClearColumnAccessControlCaches(baseModel: siteModel);
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
                    where: Rds.SitesWhere().SiteId(siteModel.SiteId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{siteModel.SiteId}\"][data-latest]",
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
                        siteModel: siteModel,
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
                    .Val("#VerUp", verUp)
                    .Val("#Ver", siteModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(siteModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: siteModel,
                        tableName: "Sites"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: siteModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: siteModel.Comments,
                        deleteCommentId: siteModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            SiteModel siteModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: siteModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = siteModel.ReplacedDisplayValues(
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
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
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

        public static string Delete(Context context, SiteSettings ss, long siteId)
        {
            var siteModel = new SiteModel(context, siteId);
            var invalid = SiteValidators.OnDeleting(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = siteModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: siteModel.Title.MessageDisplay(context: context)));
                    var res = new SitesResponseCollection(
                        context: context,
                        siteModel: siteModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemIndex(
                            context: context,
                            id: siteModel.ParentId));
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
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

        public static int Restore(Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var where = Rds.SitesWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Sites_Deleted")
                .ParentId(
                    value: ss.SiteId,
                    tableName: "Sites_Deleted")
                .SiteId_In(
                    value: selected,
                    tableName: "Sites_Deleted",
                    negative: negative,
                    _using: selected.Any());
            var sub = Rds.SelectSites(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Sites_Deleted",
                column: Rds.SitesColumn()
                    .SiteId(tableName: "Sites_Deleted"),
                where: where);
            return Repository.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(sub: Rds.SelectWikis(
                                tableType: Sqls.TableTypes.Deleted,
                                column: Rds.WikisColumn().WikiId(),
                                where: Rds.WikisWhere().SiteId_In(sub: sub)))
                            .ReferenceType("Wikis")),
                    Rds.RestoreWikis(
                        factory: context,
                        where: Rds.WikisWhere().SiteId_In(sub: sub)),
                    Rds.RestoreItems(
                        factory: context,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.RestoreSites(
                        factory: context,
                        where: where),
                    Rds.RowCount()
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(Context context, SiteSettings ss, long siteId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
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
            siteModel.SetByModel(new SiteModel().Get(
                context: context,
                tableType: Sqls.TableTypes.History,
                where: Rds.SitesWhere()
                    .SiteId(ss.SiteId)
                    .SiteId(siteId)
                    .Ver(ver.First().ToInt())));
            siteModel.VerUp = true;
            var errorData = siteModel.Update(
                context: context, ss: ss, setBySession: false, otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return new ResponseCollection(context: context)
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: siteId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(Context context, SiteModel siteModel, Message message = null)
        {
            var ss = siteModel.SiteSettings;
            var columns = new SiteSettings(context: context, referenceType: "Sites")
                .GetHistoryColumns(context: context);
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
                                    siteModel: siteModel))));
            return new SitesResponseCollection(
                context: context,
                siteModel: siteModel)
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
            SiteModel siteModel)
        {
            if (ss.ColumnHash.ContainsKey("TitleBody") && ss.ColumnHash.ContainsKey("Body"))
            {
                ss.ColumnHash["TitleBody"].ControlType = ss.ColumnHash["Body"].FieldCss == "field-rte" ? "RTEditor" : "MarkDown";
            }
            new SiteCollection(
                context: context,
                column: HistoryColumn(columns),
                where: Rds.SitesWhere().SiteId(siteModel.SiteId),
                orderBy: Rds.SitesOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(siteModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(siteModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: siteModelHistory.Ver == siteModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: siteModelHistory.Ver.ToString(),
                                            _using: siteModelHistory.Ver < siteModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        siteModel: siteModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.SitesColumnCollection()
                .SiteId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.SitesColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteModel siteModel)
        {
            return EditorResponse(context: context, siteModel: siteModel).ToJson();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long siteId)
        {
            if (!Parameters.History.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
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
                        siteId: siteId,
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
                            siteId: siteId,
                            selected: selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                var siteModel = new SiteModel(context: context, siteId: siteId);
                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: siteId,
                    tableType: ss.TableType);
                return Histories(
                    context: context,
                    siteModel: siteModel,
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
            long siteId,
            List<int> selected,
            bool negative = false)
        {
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteSites(
                        tableType: Sqls.TableTypes.History,
                        where: Rds.SitesWhere()
                            .TenantId(
                                value: context.TenantId,
                                tableName: "Sites_History")
                            .SiteId(
                                value: ss.SiteId,
                                tableName: "Sites_History")
                            .Ver_In(
                                value: selected,
                                tableName: "Sites_History",
                                negative: negative,
                                _using: selected.Any())),
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
        public static SiteModel GetByServerScript(
            Context context,
            SiteSettings ss,
            long siteId)
        {
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId);
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            var invalid = SiteValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return null;
            }
            return siteModel;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            long siteId,
            bool internalRequest)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var invalid = SiteValidators.OnEntry(
                context: context,
                ss: ss,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                       context: context,
                       errorData: invalid);
            }
            return ApiResults.Get(
                statusCode: 200,
                limitPerDate: context.ContractSettings.ApiLimit(),
                limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                response: new
                {
                    Data = new SiteModel(
                        context: context,
                        siteId: siteId)
                            .GetByApi(context: context)
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance CreateByApi(
            Context context,
            long parentId,
            long inheritPermission)
        {
            var siteApiModel = context.RequestDataString.Deserialize<SiteApiModel>();
            if (siteApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission,
                siteApiModel: siteApiModel);
            var ss = siteModel.SiteSettings;
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ItemsLimit));
            }
            if (parentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                       context: context,
                       errorData: invalid);
            }
            var errorData = siteModel.Create(context: context, otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: siteModel.SiteId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Created(
                            context: context,
                            data: siteModel.Title.MessageDisplay(context: context)));
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool CreateByServerScript(Context context, SiteSettings ss, object model)
        {
            var siteApiModel = context.RequestDataString.Deserialize<SiteApiModel>();
            if (siteApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var siteModel = new SiteModel(
                context: context,
                parentId: ss.SiteId,
                inheritPermission: ss.InheritPermission,
                siteApiModel: siteApiModel);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return false;
            }
            if (ss.ParentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            var errorData = siteModel.Create(
                context: context,
                otherInitValue: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is SiteModel data)
                        {
                            data.SiteId = siteModel.SiteId;
                            data.SetByModel(siteModel: siteModel);
                        }
                    }
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateByApi(
            Context context,
            SiteModel siteModel,
            long siteId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var ss = siteModel.SiteSettings.SiteSettingsOnUpdate(context: context);
            var siteApiModel = context.RequestDataString.Deserialize<SiteApiModel>();
            if (siteApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            siteModel.SetByApi(
                context: context,
                ss: ss,
                data: siteApiModel);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteId);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                      context: context,
                      errorData: invalid);
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Error(
                  context: context,
                  errorData: invalid);
            }
            if (siteModel.InheritPermission > 0)
            {
                ss.InheritPermission = siteModel.InheritPermission;
            }
            if (siteModel.RecordPermissions?.Count > 0
                && Parameters.Permissions.CheckManagePermission)
            {
                if (!new PermissionCollection(
                    context: context,
                    referenceId: siteModel.SiteId,
                    permissions: siteModel.RecordPermissions)
                        .Any(permission =>
                            permission.PermissionType.HasFlag(
                                Permissions.Types.ManagePermission
                                | Permissions.Types.ManageSite)))
                {
                    return ApiResults.Error(
                      context: context,
                      errorData: invalid);
                }
            }
            var errorData = siteModel.Update(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        siteModel.SiteId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Updated(
                            context: context,
                            data: siteModel.Title.MessageDisplay(context: context)));
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool UpdateByServerScript(
            Context context,
            SiteModel siteModel,
            long siteId,
            object model)
        {
            var ss = siteModel.SiteSettings.SiteSettingsOnUpdate(context: context);
            var siteApiModel = context.RequestDataString.Deserialize<SiteApiModel>();
            if (siteApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            siteModel.SetByApi(
                context: context,
                ss: ss,
                data: siteApiModel);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteId);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            siteModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: siteModel);
            if (siteModel.InheritPermission > 0)
            {
                ss.InheritPermission = siteModel.InheritPermission;
            }
            if (siteModel.RecordPermissions?.Count > 0
                && Parameters.Permissions.CheckManagePermission)
            {
                if (!new PermissionCollection(
                    context: context,
                    referenceId: siteModel.SiteId,
                    permissions: siteModel.RecordPermissions)
                        .Any(permission =>
                            permission.PermissionType.HasFlag(
                                Permissions.Types.ManagePermission
                                | Permissions.Types.ManageSite)))
                {
                    return false;
                }
            }
            var errorData = siteModel.Update(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    if (model is Libraries.ServerScripts.ServerScriptModelApiModel serverScriptModelApiModel)
                    {
                        if (serverScriptModelApiModel.Model is SiteModel data)
                        {
                            data.SetByModel(siteModel: siteModel);
                        }
                    }
                    return true;
                case Error.Types.Duplicated:
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance DeleteByApi(
            Context context,
            SiteSettings ss,
            long siteId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var siteModel = new SiteModel(context, siteId);
            var invalid = SiteValidators.OnDeleting(
                context: context,
                ss: ss,
                siteModel: siteModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                      context: context,
                      errorData: invalid);
            }
            var errorData = siteModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: siteModel.SiteId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Deleted(
                            context: context,
                            data: siteModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool DeleteByServerScript(
            Context context,
            SiteSettings ss,
            long siteId)
        {
            var siteModel = new SiteModel(context, siteId);
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return false;
            }
            var invalid = SiteValidators.OnDeleting(
                context: context,
                ss: ss,
                siteModel: siteModel,
                api: true,
                serverScript: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return false;
            }
            var errorData = siteModel.Delete(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        public static string SetSiteSettings(Context context, long siteId)
        {
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId);
            if (context.Forms.ContainsKey("Ver")
                && context.Forms.Int("Ver") != siteModel.Ver)
            {
                // $p.openSiteSettingsDialogでバージョンを受け渡しする
                // 旧バージョンの場合はSiteModelを再取得してダイアログを表示
                // ダイアログを開く処理以外はここに入らない
                siteModel = new SiteModel();
                siteModel.Get(
                    context: context,
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(context.Id)
                        .Ver(context.Forms.Int("Ver")),
                    tableType: Sqls.TableTypes.NormalAndHistory);
                siteModel.VerType = Versions.VerTypes.History;
                return siteModel.SetSiteSettings(
                    context: context,
                    setSiteSettingsPropertiesBySession: false);
            }
            else
            {
                // バージョン指定が無い場合、最新バージョンを指定された場合は
                // 最新のSiteModelで処理
                return siteModel.SetSiteSettings(context: context);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateSiteSettingsByApi(
            Context context,
            SiteSettings ss,
            SiteModel siteModel)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Error(
                 context: context,
                 errorData: new ErrorData(type: Error.Types.InvalidJsonData));
            }
            var invalid = SiteValidators.OnUpdating(
               context: context,
               ss: ss,
               siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                      context: context,
                      errorData: invalid);
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Error(
                  context: context,
                  errorData: invalid);
            }
            var siteSettingsApiModel = context.RequestDataString.Deserialize<ApiSiteSettings.SiteSettingsApiModel>();
            var apiSiteSettingValidator = ApiSiteSettingValidators.OnChageSiteSettingByApi(
                referenceType: siteModel.ReferenceType,
                ss: ss,
                siteSettingsModel: siteSettingsApiModel,
                context: context);
            switch (apiSiteSettingValidator.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                      context: context,
                      errorData: apiSiteSettingValidator);
            }
            if (ApiSiteSetting.ServerScriptRefTypes.Contains(siteModel.ReferenceType)
                && siteSettingsApiModel.ServerScripts != null)
            {
                siteModel.UpsertServerScriptByApi(
                    siteSetting: ss,
                    serverScriptsApiSiteSetting: siteSettingsApiModel.ServerScripts);
            }
            if (siteSettingsApiModel.Scripts != null)
            {
                siteModel.UpsertScriptByApi(
                    siteSetting: ss,
                    scriptsApiSiteSetting: siteSettingsApiModel.Scripts);
            }
            if (siteSettingsApiModel.Styles != null)
            {
                siteModel.UpsertStyleByApi(
                    siteSetting: ss,
                    styleApiSiteSetting: siteSettingsApiModel.Styles);
            }
            if (siteSettingsApiModel.Htmls != null)
            {
                siteModel.UpsertHtmlByApi(
                    siteSetting: ss,
                    htmlsApiSiteSetting: siteSettingsApiModel.Htmls);
            }
            if (siteSettingsApiModel.Processes != null)
            {
                siteModel.UpsertProcessByApi(
                    siteSetting: ss,
                    processesApiSiteSetting: siteSettingsApiModel.Processes,
                    context: context);
            }
            if (siteSettingsApiModel.StatusControls != null)
            {
                siteModel.UpsertStatusControlByApi(
                    siteSetting: ss,
                    statusControlSettings: siteSettingsApiModel.StatusControls,
                    context: context);
            }
            var errorData = siteModel.Update(
               context: context,
               ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        id: siteModel.SiteId,
                        limitPerDate: context.ContractSettings.ApiLimit(),
                        limitRemaining: context.ContractSettings.ApiLimit() - ss.ApiCount,
                        message: Displays.Updated(
                            context: context,
                            data: siteModel.Title.MessageDisplay(context: context)));
                default:
                    return ApiResults.Error(
                      context: context,
                      errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateSmartDesign(
            Context context,
            SiteSettings ss,
            SiteModel siteModel,
            string jsonBody)
        {
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.SdMessageJson(context: context);
            }
            if (siteModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                var response = Messages.ResponseDeleteConflicts(context: context);
                return SdResponse.SdResponseJson(response).ToJson();
            }
            var siteSettingsApiModel = jsonBody.Deserialize<ApiSiteSettings.SiteSettingsApiModel>();
            siteModel.Timestamp = siteSettingsApiModel.Timestamp;
            if (siteSettingsApiModel.Columns != null)
            {
                siteModel.UpsertColumnsByApi(
                    context: context,
                    siteSetting: ss,
                    columnsApiSiteSetting: siteSettingsApiModel.Columns);
            }
            if (siteSettingsApiModel.EditorColumnHash != null)
            {
                siteModel.UpsertEditorColumnHashByApi(
                    siteSetting: ss,
                    columnsApiSiteSetting: siteSettingsApiModel.EditorColumnHash);
            }
            if (siteSettingsApiModel.GridColumns != null)
            {
                siteModel.UpsertGridColumnsByApi(
                    siteSetting: ss,
                    columnsApiSiteSetting: siteSettingsApiModel.GridColumns);
            }
            if (siteSettingsApiModel.FilterColumns != null)
            {
                siteModel.UpsertFilterColumnsByApi(
                    siteSetting: ss,
                    columnsApiSiteSetting: siteSettingsApiModel.FilterColumns);
            }
            if (siteSettingsApiModel.Sections != null)
            {
                siteModel.UpsertSectionsByApi(
                    siteSetting: ss,
                    sectionLatestId: siteSettingsApiModel.SectionLatestId,
                    sectionsApiSiteSetting: siteSettingsApiModel.Sections);
            }
            var errorData = siteModel.Update(
               context: context,
               ss: ss,
               setBySession: false);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Updated(
                            context: context,
                            data: siteModel.Title.Value));
                    return new SdResponse(
                                method: "UpdateSuccess",
                                url: context.UrlReferrer)
                                .ToJson();
                case Error.Types.UpdateConflicts:
                    return new SdResponse(
                                method: "UpdateConflicts",
                                value: Displays.UpdateConflicts(
                                    context: context,
                                    data: siteModel.Updator.Name))
                                .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Templates(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .Html("#SiteMenu", new HtmlBuilder().TemplateTabsContainer(
                    context: context,
                    ss: ss))
                .ReplaceAll("#MainCommandsContainer", hb
                    .MainCommands(
                        context: context,
                        ss: ss,
                        verType: Versions.VerTypes.Latest,
                        backButton: false,
                        extensions: () => hb
                            .Button(
                                text: Displays.GoBack(context: context),
                                controlCss: "button-icon button-neutral",
                                accessKey: "q",
                                onClick: "$p.send($(this),'MainForm');",
                                icon: "ui-icon-disk",
                                action: "SiteMenu",
                                method: "post")
                            .Button(
                                controlId: "OpenSiteTitleDialog",
                                text: Displays.Create(context: context),
                                controlCss: "button-icon hidden button-positive",
                                onClick: "$p.openSiteTitleDialog($(this));",
                                icon: "ui-icon-disk")
                            .Div(
                                attributes: new HtmlAttributes()
                                    .Id("ImportUserTemplateDialog")
                                    .Class("dialog")
                                    .Title(Displays.ImportSitePackage(context: context)),
                                action: () => hb.ImportUserTemplateDialog(context: context, ss: ss))
                            .Div(
                                attributes: new HtmlAttributes()
                                    .Id("EditUserTemplateDialog")
                                    .Class("dialog")
                                    .Title(Displays.AdvancedSetting(context: context)))
                            .CreateUserTemplateDialog(
                                context: context,
                                ss: siteModel.SiteSettings)))
                .Invoke("setTemplate")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TemplateTabsContainer(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var templates = Def.TemplateDefinitionCollection
                .Where(o => o.Language == context.Language)
                .ToList();
            templates.AddRange(GetUserTemplates(context: context));
            if (!templates.Any())
            {
                templates = Def.TemplateDefinitionCollection
                    .Where(o => o.Language == "en")
                    .ToList();
            }
            return hb
                .Div(
                    id: "TemplateTabsContainer",
                    css: "tab-container max",
                    action: () => hb
                        .Ul(id: "EditorTabs", action: () => hb
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetStandard",
                                        text: Displays.Standard(context: context)),
                                _using: templates.Any(o => o.Standard > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetProject",
                                        text: Displays.Project(context: context)),
                                _using: templates.Any(o => o.Project > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetBusinessImprovement",
                                        text: Displays.BusinessImprovement(context: context)),
                                _using: templates.Any(o => o.BusinessImprovement > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetSales",
                                        text: Displays.Sales(context: context)),
                                _using: templates.Any(o => o.Sales > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetCustomer",
                                        text: Displays.Customer(context: context)),
                                _using: templates.Any(o => o.Customer > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetStore",
                                        text: Displays.Store(context: context)),
                                _using: templates.Any(o => o.Store > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetResearchAndDevelopment",
                                        text: Displays.ResearchAndDevelopment(context: context)),
                                _using: templates.Any(o => o.ResearchAndDevelopment > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetMarketing",
                                        text: Displays.Marketing(context: context)),
                                _using: templates.Any(o => o.Marketing > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetManufacture",
                                        text: Displays.Manufacture(context: context)),
                                _using: templates.Any(o => o.Manufacture > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetInformationSystem",
                                        text: Displays.InformationSystem(context: context)),
                                _using: templates.Any(o => o.InformationSystem > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetCorporatePlanning",
                                        text: Displays.CorporatePlanning(context: context)),
                                _using: templates.Any(o => o.CorporatePlanning > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetHumanResourcesAndGeneralAffairs",
                                        text: Displays.HumanResourcesAndGeneralAffairs(context: context)),
                                _using: templates.Any(o => o.HumanResourcesAndGeneralAffairs > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetEducation",
                                        text: Displays.Education(context: context)),
                                _using: templates.Any(o => o.Education > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetPurchase",
                                        text: Displays.Purchase(context: context)),
                                _using: templates.Any(o => o.Purchase > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetLogistics",
                                        text: Displays.Logistics(context: context)),
                                _using: templates.Any(o => o.Logistics > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetLegalAffairs",
                                        text: Displays.LegalAffairs(context: context)),
                                _using: templates.Any(o => o.LegalAffairs > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetProductList",
                                        text: Displays.ProductList(context: context)),
                                _using: templates.Any(o => o.ProductList > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetClassification",
                                        text: Displays.Classification(context: context)),
                                _using: templates.Any(o => o.Classification > 0))
                            .Li(
                                action: () => hb
                                    .A(
                                        href: "#FieldSetUserTemplate",
                                        text: Displays.CustomApps(context: context)),
                                _using: Parameters.UserTemplate.Enabled))
                        .TemplateTab(
                            context: context,
                            name: "Standard",
                            templates: templates
                                .Where(o => o.Standard > 0)
                                .OrderBy(o => o.Standard))
                        .TemplateTab(
                            context: context,
                            name: "Project",
                            templates: templates
                                .Where(o => o.Project > 0)
                                .OrderBy(o => o.Project))
                        .TemplateTab(
                            context: context,
                            name: "BusinessImprovement",
                            templates: templates
                                .Where(o => o.BusinessImprovement > 0)
                                .OrderBy(o => o.BusinessImprovement))
                        .TemplateTab(
                            context: context,
                            name: "Sales",
                            templates: templates
                                .Where(o => o.Sales > 0)
                                .OrderBy(o => o.Sales))
                        .TemplateTab(
                            context: context,
                            name: "Customer",
                            templates: templates
                                .Where(o => o.Customer > 0)
                                .OrderBy(o => o.Customer))
                        .TemplateTab(
                            context: context,
                            name: "Store",
                            templates: templates
                                .Where(o => o.Store > 0)
                                .OrderBy(o => o.Store))
                        .TemplateTab(
                            context: context,
                            name: "ResearchAndDevelopment",
                            templates: templates
                                .Where(o => o.ResearchAndDevelopment > 0)
                                .OrderBy(o => o.ResearchAndDevelopment))
                        .TemplateTab(
                            context: context,
                            name: "Marketing",
                            templates: templates
                                .Where(o => o.Marketing > 0)
                                .OrderBy(o => o.Marketing))
                        .TemplateTab(
                            context: context,
                            name: "Manufacture",
                            templates: templates
                                .Where(o => o.Manufacture > 0)
                                .OrderBy(o => o.Manufacture))
                        .TemplateTab(
                            context: context,
                            name: "InformationSystem",
                            templates: templates
                                .Where(o => o.InformationSystem > 0)
                                .OrderBy(o => o.InformationSystem))
                        .TemplateTab(
                            context: context,
                            name: "CorporatePlanning",
                            templates: templates
                                .Where(o => o.CorporatePlanning > 0)
                                .OrderBy(o => o.CorporatePlanning))
                        .TemplateTab(
                            context: context,
                            name: "HumanResourcesAndGeneralAffairs",
                            templates: templates
                                .Where(o => o.HumanResourcesAndGeneralAffairs > 0)
                                .OrderBy(o => o.HumanResourcesAndGeneralAffairs))
                        .TemplateTab(
                            context: context,
                            name: "Education",
                            templates: templates
                                .Where(o => o.Education > 0)
                                .OrderBy(o => o.Education))
                        .TemplateTab(
                            context: context,
                            name: "Purchase",
                            templates: templates
                                .Where(o => o.Purchase > 0)
                                .OrderBy(o => o.Purchase))
                        .TemplateTab(
                            context: context,
                            name: "Logistics",
                            templates: templates
                                .Where(o => o.Logistics > 0)
                                .OrderBy(o => o.Logistics))
                        .TemplateTab(
                            context: context,
                            name: "LegalAffairs",
                            templates: templates
                                .Where(o => o.LegalAffairs > 0)
                                .OrderBy(o => o.LegalAffairs))
                        .TemplateTab(
                            context: context,
                            name: "ProductList",
                            templates: templates
                                .Where(o => o.ProductList > 0)
                                .OrderBy(o => o.ProductList))
                        .TemplateTab(
                            context: context,
                            name: "Classification",
                            templates: templates
                                .Where(o => o.Classification > 0)
                                .OrderBy(o => o.Classification))
                        .TemplateTab(
                            context: context,
                            name: "UserTemplate",
                            templates: templates
                                .Where(o => o.CustomApps > 0)
                                .OrderBy(o => o.Id),
                            isUserTemplate: true,
                            _using: Parameters.UserTemplate.Enabled));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<TemplateDefinition> GetUserTemplates(
            Context context,
            string searchText = null,
            string templateId = null)
        {
            var column = Rds.ExtensionsColumn().ExtensionId().ExtensionName();
            var where = Rds.ExtensionsWhere().TenantId(context.TenantId).ExtensionType("CustomApps").Disabled(false);
            if (!templateId.IsNullOrEmpty())
            {
                var id = -1;
                if (!templateId.StartsWith("UserTemplate")
                    || int.TryParse(templateId.Substring("UserTemplate".Length), out id) == false)
                {
                    return new List<TemplateDefinition>();
                }
                where.ExtensionId(id);
                column.Description();
            }
            else if (!searchText.IsNullOrEmpty())
            {
                where
                    .SqlWhereLike(
                        tableName: "Extensions",
                        name: "SearchText",
                        searchText: searchText,
                        clauseCollection: new List<string>()
                        {
                            Rds.Extensions_ExtensionName_WhereLike(factory: context)
                        });
            }
            return new ExtensionCollection(
                context: context,
                column: column,
                where: where)
                .Select(o => new TemplateDefinition()
                {
                    Id = $"UserTemplate{o.ExtensionId}",
                    Title = o.ExtensionName,
                    Description = o.Description,
                    CustomApps = o.ExtensionId
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TemplateTab(
            this HtmlBuilder hb,
            Context context,
            string name,
            IEnumerable<TemplateDefinition> templates,
            bool isUserTemplate = false,
            string searchText = null,
            bool _using = true)
        {
            if (_using != true) return hb;
            return templates.Any() || isUserTemplate
                ? hb.FieldSet(id: "FieldSet" + name, css: "template", action: () => hb
                    .Div(
                        id: name + "TemplatesViewer",
                        css: "template-viewer-container",
                        action: () => hb
                            .Div(css: "template-viewer", action: () => hb
                                .P(css: "description", action: () => hb
                                    .Text(text: Displays.SelectTemplate(context: context)))
                                .Div(css: "viewer hidden")))
                    .Div(css: "template-selectable", action: () => hb
                        .FieldSelectable(
                            controlId: name + "Templates",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " single applied",
                            listItemCollection: templates.ToDictionary(
                                o => o.Id, o => new ControlData(o.Title)),
                            action: "PreviewTemplate",
                            method: "post",
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () =>
                            {
                                if (isUserTemplate)
                                {
                                    hb
                                        .Div(css: "command-left", action: () => hb
                                            .Button(
                                                controlId: "AddTemplateButton",
                                                text: Displays.Import(context: context),
                                                controlCss: "button-icon",
                                                icon: "ui-icon-arrowreturnthick-1-e",
                                                onClick: "$p.openImportUserTemplateDialog($(this));")
                                            .Button(
                                                controlId: "EditTemplateButton",
                                                text: Displays.AdvancedSetting(context: context),
                                                controlCss: "button-icon",
                                                icon: "ui-icon-gear",
                                                onClick: "$p.send($(this));",
                                                method: "post",
                                                action: "OpenEditUserTemplateDialog")
                                            .Button(
                                                controlId: "DeleteTemplateButton",
                                                text: Displays.Delete(context: context),
                                                controlCss: "button-icon",
                                                onClick: "$p.send($(this));",
                                                method: "post",
                                                icon: "ui-icon-trash",
                                                confirm: "ConfirmDelete",
                                                action: "DeleteUserTemplate"),
                                            _using: Permissions.CanManageTenantOrEnableManageTenant(context: context))
                                        .Div(css: "command-left", action: () => hb
                                            .TextBox(
                                                controlId: "Template_SearchText",
                                                controlCss: " auto-postback always-send w200",
                                                action: "SearchUserTemplate",
                                                method: "post",
                                                text: searchText ?? string.Empty)
                                            .Button(
                                                text: Displays.Search(context: context),
                                                controlCss: "button-icon",
                                                onClick: "$p.send($('#Template_SearchText'));",
                                                icon: "ui-icon-search"))
                                        .Hidden(
                                            controlId: "Template_Title",
                                            css: " always-send")
                                        .Hidden(
                                            controlId: "Template_Id",
                                            css: " always-send");
                                }
                            })))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ImportUserTemplate(
            Context context,
            SiteSettings ss)
        {
            if (Permissions.CanManageTenantOrEnableManageTenant(context: context) != true
                || Parameters.UserTemplate.Enabled != true)
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            if (Parameters.UserTemplate.CustomAppsMax > 0
                && Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectExtensions(
                        column: Rds.ExtensionsColumn().ExtensionsCount(),
                        where: Rds.ExtensionsWhere()
                            .TenantId(context.TenantId)
                            .ExtensionType("CustomApps")
                            .Disabled(false))) >= Parameters.UserTemplate.CustomAppsMax)
            {
                return Messages.ResponseCustomAppsLimit(context: context).ToJson();
            }
            var sitePackage = Libraries.SitePackages.Utilities.GetSitePackageFromPostedFile(context: context);
            if (sitePackage == null)
            {
                return Messages.ResponseFailedReadFile(context: context).ToJson();
            }
            var title = context.Forms.Data("Title");
            if (title.IsNullOrEmpty())
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            var searchText = context.Forms.Data("SearchText");
            var extension = new ExtensionModel(context: context);
            extension.ExtensionName = title;
            extension.ExtensionType = "CustomApps";
            extension.Description = context.Forms.Data("Description");
            extension.ExtensionSettings = sitePackage.ToJson();
            // テナント毎に管理するのでTemplateDefinition.Languageの指定はなし
            extension.Create(context: context, ss: null);
            return new ResponseCollection(context: context)
                .CloseDialog()
                .ReplaceAll(
                    target: "#FieldSetUserTemplate",
                    value: new HtmlBuilder().TemplateTab(
                        context: context,
                        name: "UserTemplate",
                        templates: GetUserTemplates(
                            context: context,
                            searchText: searchText),
                        searchText: searchText,
                        isUserTemplate: true))
                .Invoke("setTemplate")
                .Invoke("refreshTemplateSelector")
                .Message(Messages.Registered(
                    context: context,
                    data: title))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteUserTemplate(
            Context context,
            SiteSettings ss)
        {
            if (Permissions.CanManageTenantOrEnableManageTenant(context: context) != true)
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var templateTitle = context.Forms.Data("Template_Title");
            var templateId = context.Forms.Data("Template_Id");
            var searchText = context.Forms.Data("SearchText");
            int id = -1;
            if (templateId.IsNullOrEmpty())
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            if (!templateId.StartsWith("UserTemplate")
                || int.TryParse(templateId.Substring("UserTemplate".Length), out id) == false)
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteExtensions(
                    where: Rds.ExtensionsWhere().ExtensionId(id)));
            return new ResponseCollection(context: context)
                .CloseDialog()
                .ReplaceAll(
                    target: "#FieldSetUserTemplate",
                    value: new HtmlBuilder().TemplateTab(
                        context: context,
                        name: "UserTemplate",
                        templates: GetUserTemplates(
                            context: context,
                            searchText: searchText),
                        isUserTemplate: true,
                        searchText: searchText))
                .Invoke("setTemplate")
                .Invoke("refreshTemplateSelector")
                .Message(Messages.Deleted(
                    context: context,
                    data: templateTitle))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchUserTemplate(
            Context context,
            SiteSettings ss)
        {
            var searchText = context.Forms["Template_SearchText"].ToString();
            return new ResponseCollection(context: context)
                .ReplaceAll(
                    target: "#FieldSetUserTemplate",
                    value: new HtmlBuilder().TemplateTab(
                        context: context,
                        name: "UserTemplate",
                        templates: GetUserTemplates(
                            context: context,
                            searchText: searchText),
                        isUserTemplate: true,
                        searchText: searchText))
                .Invoke("setTemplate")
                .Invoke("refreshTemplateSelector")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UpdateUserTemplate(
            Context context,
            SiteSettings ss)
        {
            if (Permissions.CanManageTenantOrEnableManageTenant(context: context) != true)
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var templateTitle = context.Forms.Data("UpdateUserTemplate_Title");
            var templateDescription = context.Forms.Data("UpdateUserTemplate_Description");
            var templateId = context.Forms.Data("UpdateUserTemplate_Id");
            var searchText = context.Forms.Data("UpdateUserTemplate_SearchText");
            int id = -1;
            if (templateId.IsNullOrEmpty())
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            if (!templateId.StartsWith("UserTemplate")
                || int.TryParse(templateId.Substring("UserTemplate".Length), out id) == false)
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
            var extensionModel = new ExtensionModel(
                context: context,
                extensionId: id);
            extensionModel.VerUp = false;
            extensionModel.ExtensionName = templateTitle;
            extensionModel.Description = templateDescription;
            if (extensionModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var errorData = extensionModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return new ResponseCollection(context: context)
                        .ReplaceAll(
                            target: "#FieldSetUserTemplate",
                            value: new HtmlBuilder().TemplateTab(
                                context: context,
                                name: "UserTemplate",
                                templates: GetUserTemplates(
                                    context: context,
                                    searchText: searchText),
                                isUserTemplate: true,
                                searchText: searchText))
                        .CloseDialog()
                        .Invoke("setTemplate")
                        .Invoke("refreshTemplateSelector")
                        .Message(Messages.Updated(
                            context: context,
                            data: templateTitle))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenEditUserTemplateDialog(
            Context context,
            SiteSettings ss)
        {
            var templateId = context.Forms["Template_Id"];
            var searchText = context.Forms["Template_SearchText"];
            if (templateId.IsNullOrEmpty())
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var template = GetUserTemplates(context: context, templateId: templateId).FirstOrDefault();
            if (template == null)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#EditUserTemplateDialog",
                    new HtmlBuilder().ImportUserTemplateDialog(
                        context: context,
                        ss: ss,
                        template: template))
                .Invoke("openEditUserTemplateDialog")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateByTemplate(Context context, long parentId, long inheritPermission)
        {
            var siteModel = new SiteModel(
                context: context,
                parentId: parentId,
                inheritPermission: inheritPermission);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: parentId);
            if (context.ContractSettings.SitesLimit(context: context))
            {
                return Error.Types.SitesLimit.MessageJson(context: context);
            }
            if (parentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var id = context.Forms.Data("TemplateId");
            if (id.IsNullOrEmpty())
            {
                return Error.Types.SelectTargets.MessageJson(context: context);
            }
            if (id.StartsWith("UserTemplate"))
            {
                var extension = new ExtensionModel().Get(
                    context: context,
                    where: Rds.ExtensionsWhere().ExtensionId(int.Parse(id.Substring("UserTemplate".Length))).Disabled(false));
                var sitePackage = extension.ExtensionSettings.Deserialize<Libraries.SitePackages.SitePackage>();
                if (sitePackage == null)
                {
                    return Error.Types.InternalServerError.MessageJson(context: context);
                }
                ss.SiteId = parentId;
                return Libraries.SitePackages.Utilities.ImportSitePackage(
                    context: context,
                    ss: ss,
                    sitePackage: sitePackage);
            }
            else
            {
                var templateDefinition = Def.TemplateDefinitionCollection
                    .FirstOrDefault(o => o.Id == id);
                if (templateDefinition == null)
                {
                    return Error.Types.NotFound.MessageJson(context: context);
                }
                var templateSs = templateDefinition.SiteSettingsTemplate
                    .DeserializeSiteSettings(context: context);
                if (templateSs == null)
                {
                    return Error.Types.NotFound.MessageJson(context: context);
                }
                siteModel.ReferenceType = templateSs.ReferenceType;
                siteModel.Title = new Title(context.Forms.Data("SiteTitle"));
                siteModel.Body = templateDefinition.Body;
                siteModel.SiteSettings = templateSs;
                siteModel.Create(context: context, otherInitValue: true);
                return SiteMenuResponse(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: parentId));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenuJson(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SitesSiteSettings(
                context: context, referenceId: siteModel.ParentId);
            if (siteModel.ParentId == 0)
            {
                ss.PermissionType = context.SiteTopPermission();
            }
            var invalid = SiteValidators.OnCreating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return SiteMenuResponse(context: context, siteModel: siteModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteMenuResponse(Context context, SiteModel siteModel)
        {
            return new ResponseCollection(context: context)
                .CloseDialog()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: siteModel.SiteId != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches.Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder().MainCommands(
                    context: context,
                    ss: siteModel.SiteSettings,
                    verType: siteModel.VerType,
                    backButton: siteModel.SiteId != 0))
                .Invoke("setSiteMenu")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MoveSiteMenu(Context context, long id)
        {
            var siteModel = new SiteModel(context: context, siteId: id);
            siteModel.SiteSettings.PermissionType = id == 0
                ? context.SiteTopPermission()
                : Permissions.Get(
                    context: context,
                    siteId: id);
            var sourceSiteModel = new SiteModel(
                context: context,
                siteId: context.Forms.Long("SiteId"));
            var destinationSiteModel = new SiteModel(
                context: context,
                siteId: context.Forms.Long("DestinationId"));
            if (siteModel.NotFound()
                || sourceSiteModel.NotFound()
                || destinationSiteModel.NotFound())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.NotFound));
            }
            if (destinationSiteModel.ReferenceType != "Sites")
            {
                switch (sourceSiteModel.ReferenceType)
                {
                    case "Sites":
                    case "Wikis":
                        return SiteMenuError(
                            context: context,
                            id: id,
                            siteModel: siteModel,
                            invalid: new ErrorData(type: Error.Types.CanNotPerformed));
                    default:
                        return LinkDialog(
                            context: context,
                            id: id,
                            siteModel: siteModel,
                            sourceSiteModel: sourceSiteModel,
                            destinationSiteModel: destinationSiteModel);
                }
            }
            var toParent = id != 0 &&
                SiteInfo.TenantCaches.Get(context.TenantId)
                    .SiteMenu.Get(id).ParentId == destinationSiteModel.SiteId;
            var invalid = SiteValidators.OnMoving(
                context: context,
                currentId: id,
                destinationId: destinationSiteModel.SiteId,
                current: SiteSettingsUtilities.Get(
                    context: context,
                    siteModel: siteModel,
                    referenceId: id),
                source: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: sourceSiteModel.SiteId,
                    referenceId: sourceSiteModel.SiteId),
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: destinationSiteModel.SiteId,
                    referenceId: destinationSiteModel.SiteId));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            MoveSiteMenu(
                context: context,
                ss: siteModel.SiteSettings,
                sourceId: sourceSiteModel.SiteId,
                destinationId: destinationSiteModel.SiteId);
            return toParent
                ? "[]"
                : new ResponseCollection(context: context)
                    .ReplaceAll(
                        "[data-value=\"" + destinationSiteModel.SiteId + "\"]",
                        siteModel.ReplaceSiteMenu(
                            context: context,
                            sourceId: sourceSiteModel.SiteId,
                            destinationId: destinationSiteModel.SiteId))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string LinkDialog(
            Context context,
            long id,
            SiteModel siteModel,
            SiteModel sourceSiteModel,
            SiteModel destinationSiteModel)
        {
            if (sourceSiteModel.SiteSettings.Links
                ?.Where(o => o.SiteId > 0)
                .Any(o => o.SiteId == destinationSiteModel.SiteId) == true
                    || destinationSiteModel.SiteSettings.Links
                        ?.Where(o => o.SiteId > 0)
                        .Any(o => o.SiteId == sourceSiteModel.SiteId) == true)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.AlreadyLinked));
            }
            var invalid = SiteValidators.OnLinking(
                context: context,
                sourceInheritSiteId: sourceSiteModel.InheritPermission,
                destinationInheritSiteId: destinationSiteModel.InheritPermission);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            var columns = sourceSiteModel.SiteSettings.Columns
                .Where(o => o.ColumnName.StartsWith("Class"));
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .Html("#LinkDialog", hb.Div(action: () => hb
                    .FieldSet(
                        css: "fieldset",
                        action: () => hb
                            .FieldText(
                                labelText: Displays.LinkDestinations(context: context),
                                text: destinationSiteModel.Title.Value)
                            .FieldText(
                                labelText: Displays.LinkSources(context: context),
                                text: sourceSiteModel.Title.Value)
                            .FieldDropDown(
                                context: context,
                                controlId: "LinkColumn",
                                labelText: Displays.LinkColumn(context: context),
                                controlCss: " always-send",
                                optionCollection: columns.ToDictionary(o =>
                                    o.ColumnName, o => new ControlData(o.LabelText)),
                                selectedValue: columns.Where(o => !sourceSiteModel
                                    .SiteSettings
                                    .GetEditorColumnNames()
                                    .Contains(o.ColumnName))
                                        .FirstOrDefault()?
                                        .ColumnName)
                            .FieldTextBox(
                                controlId: "LinkColumnLabelText",
                                labelText: Displays.DisplayName(context: context),
                                controlCss: " always-send",
                                text: destinationSiteModel.Title.Value,
                                validateRequired: true)
                            .Hidden(
                                controlId: "DestinationId",
                                value: destinationSiteModel.SiteId.ToString())
                            .Hidden(
                                controlId: "SiteId",
                                value: sourceSiteModel.SiteId.ToString())
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate button-positive",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "CreateLink",
                                    method: "post",
                                    confirm: "ConfirmCreateLink")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon button-neutral",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel")))))
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Invoke("openLinkDialog").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void MoveSiteMenu(
            Context context, SiteSettings ss, long sourceId, long destinationId)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateSites(
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(sourceId),
                        param: Rds.SitesParam().ParentId(destinationId))
                });
            SiteInfo.Refresh(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateLink(Context context, long id)
        {
            var siteModel = new SiteModel(context: context, siteId: id);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: id);
            var sourceSiteModel = new SiteModel(
                context: context, siteId: context.Forms.Long("SiteId"));
            var destinationSiteModel = new SiteModel(
                context: context, siteId: context.Forms.Long("DestinationId"));
            if (siteModel.NotFound() ||
                sourceSiteModel.NotFound() ||
                destinationSiteModel.NotFound())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.NotFound));
            }
            var invalid = SiteValidators.OnLinking(
                context: context,
                sourceInheritSiteId: sourceSiteModel.InheritPermission,
                destinationInheritSiteId: destinationSiteModel.InheritPermission);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            switch (sourceSiteModel.ReferenceType)
            {
                case "Sites":
                case "Wikis":
                    return SiteMenuError(
                        context: context,
                        id: id,
                        siteModel: siteModel,
                        invalid: new ErrorData(type: Error.Types.CanNotPerformed));
            }
            if (sourceSiteModel.SiteSettings.Links
                ?.Where(o => o.SiteId > 0)
                .Any(o => o.SiteId == destinationSiteModel.SiteId) == true
                    || destinationSiteModel.SiteSettings.Links
                        ?.Where(o => o.SiteId > 0)
                        .Any(o => o.SiteId == sourceSiteModel.SiteId) == true)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.AlreadyLinked));
            }
            var columns = sourceSiteModel.SiteSettings.Columns
                .Where(o => o.ColumnName.StartsWith("Class"));
            if (!columns.Any())
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.CanNotLink));
            }
            var column = sourceSiteModel.SiteSettings.ColumnHash.Get(
                context.Forms.Data("LinkColumn"));
            if (column == null)
            {
                return SiteMenuError(
                    context: context,
                    id: id,
                    siteModel: siteModel,
                    invalid: new ErrorData(type: Error.Types.InvalidRequest));
            }
            var labelText = context.Forms.Data("LinkColumnLabelText");
            column.LabelText = labelText;
            column.GridLabelText = labelText;
            column.ChoicesText = $"[[{destinationSiteModel.SiteId}]]";
            sourceSiteModel.SiteSettings.SetLinks(context: context, column: column);
            if (!sourceSiteModel.SiteSettings.GetEditorColumnNames().Contains(column.ColumnName))
            {
                sourceSiteModel.SiteSettings.EditorColumnHash
                    ?.Get(sourceSiteModel.SiteSettings.TabName(0))
                    .Add(column.ColumnName);
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateSites(
                        param: Rds.SitesParam().SiteSettings(
                            sourceSiteModel.SiteSettings.RecordingJson(context: context)),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId(sourceSiteModel.SiteId)),
                    Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(sourceSiteModel.SiteId)),
                    LinkUtilities.Insert(sourceSiteModel.SiteSettings.Links
                        ?.Where(o => o.SiteId > 0)
                        .Select(o => o.SiteId)
                        .Distinct()
                        .ToDictionary(o => o, o => sourceSiteModel.SiteId))
                });
            return new ResponseCollection(context: context)
                .CloseDialog()
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Message(Messages.LinkCreated(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SortSiteMenu(Context context, long siteId)
        {
            var siteModel = new SiteModel(context: context, siteId: siteId);
            var invalid = SiteValidators.OnSorting(
                context: context, ss: SiteSettingsUtilities.Get(
                    context: context, siteModel: siteModel, referenceId: siteId));
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return SiteMenuError(
                    context: context,
                    id: siteId,
                    siteModel: siteModel,
                    invalid: invalid);
            }
            var ownerId = siteModel.SiteId == 0
                ? Parameters.Site.TopOrderBy > 0
                    ? Parameters.Site.TopOrderBy
                    : context.UserId
                : 0;
            SortSiteMenu(
                context: context,
                siteModel: siteModel,
                ownerId: ownerId);
            return new ResponseCollection(context: context).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SortSiteMenu(Context context, SiteModel siteModel, int ownerId)
        {
            new OrderModel()
            {
                ReferenceId = siteModel.SiteId,
                ReferenceType = "Sites",
                OwnerId = ownerId,
                Data = context.Forms.LongList("Data")
            }.UpdateOrCreate(
                context: context,
                ss: siteModel.SiteSettings);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SiteMenuError(
            Context context, long id, SiteModel siteModel, ErrorData invalid)
        {
            return new ResponseCollection(context: context)
                .ReplaceAll("#SiteMenu", new HtmlBuilder().SiteMenu(
                    context: context,
                    siteModel: id != 0 ? siteModel : null,
                    siteConditions: SiteInfo.TenantCaches
                        .Get(context.TenantId)?
                        .SiteMenu
                        .SiteConditions(context: context, ss: siteModel.SiteSettings)))
                .Invoke("setSiteMenu")
                .Message(invalid.Message(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MoveTargetsSourceColumnsJson(
            Context context,
            long referenceId)
        {
            var siteModel = new SiteModel(
                context: context,
                siteId: context.SiteId);
            var moveTargetsColumns = Jsons.Deserialize<List<long>>(context.Forms["MoveTargetsColumnsAll"]) ?? new List<long>();
            moveTargetsColumns.Add(siteModel.SiteId);
            var where = Rds.SitesWhere()
                .TenantId(context.TenantId)
                .SiteId_In(
                    value: moveTargetsColumns,
                    negative: true)
                .ReferenceType(siteModel.ReferenceType)
                .Add(
                    raw: Def.Sql.CanReadSites,
                    _using: !context.HasPrivilege)
                .SqlWhereLike(
                    tableName: "Sites",
                    name: "SearchText",
                    searchText: context.Forms.Data("MoveTargetsSourceColumnsText"),
                    clauseCollection: new List<string>()
                    {
                        Rds.Sites_Title_WhereLike(factory: context),
                        Rds.Sites_SiteId_WhereLike(factory: context)
                    });
            var offset = context.Forms.Data("ControlId") != "MoveTargetsSourceColumnsText"
                ? context.Forms.Int("MoveTargetsSourceColumnsOffset")
                : 0;
            var pageSize = Parameters.General.DropDownSearchPageSize;
            var statements = new List<SqlStatement>()
            {
                Rds.SelectSites(
                    offset: offset,
                    pageSize: pageSize,
                    dataTableName: "Main",
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    join: Rds.SitesJoinDefault(),
                    where: where,
                    orderBy: Rds.SitesOrderBy()
                        .Title()
                        .SiteId()),
                Rds.SelectCount(
                    tableName: "Sites",
                    join: Rds.SitesJoinDefault(),
                    where: where)
            };
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                statements: statements.ToArray());
            var totalCount = Rds.Count(dataSet);
            var listItemCollection = dataSet.Tables["Main"].AsEnumerable();
            var responseCollection = new ResponseCollection();
            var nextOffset = Paging.NextOffset(
                offset: offset,
                totalCount: totalCount,
                pageSize: pageSize);
            if (context.Forms.Data("ControlId") != "MoveTargetsSourceColumnsText")
            {
                return responseCollection
                    .Append(
                        target: "#MoveTargetsSourceColumns",
                        value: MoveTargetListItems(listItemCollection: listItemCollection))
                    .Val(
                        target: "#MoveTargetsSourceColumnsOffset",
                        value: nextOffset)
                    .ToJson();
            }
            else
            {
                return responseCollection
                    .Invoke(
                        methodName: "clearScrollTop",
                        args: "MoveTargetsSourceColumnsWrapper")
                    .Html(
                        target: "#MoveTargetsSourceColumns",
                        value: MoveTargetListItems(listItemCollection: listItemCollection))
                    .Val(
                        target: "#MoveTargetsSourceColumnsOffset",
                        value: nextOffset)
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MoveTargetListItems(EnumerableRowCollection<DataRow> listItemCollection)
        {
            return new HtmlBuilder()
                .SelectableItems(
                    listItemCollection: listItemCollection
                        .ToDictionary(
                            dataRow => dataRow.String("SiteId"),
                            dataRow => new ControlData($"[{dataRow.String("SiteId")}] {dataRow.String("Title")}")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, long siteId, bool clearSessions)
        {
            var siteModel = new SiteModel(
                context: context,
                siteId: siteId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteId);
            return Editor(context: context, siteModel: siteModel);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var tags = hb.Ul(id: "EditorTabs", action: () =>
            {
                hb
                    .Li(action: () => hb
                        .A(
                            href: "#FieldSetGeneral",
                            text: Displays.General(context: context)))
                    .Li(action: () => hb
                        .A(
                            href: "#GuideEditor",
                            text: Displays.Guide(context: context)));
                if (siteModel.MethodType != BaseModel.MethodTypes.New)
                {
                    hb.Li(action: () => hb
                        .A(
                            href: "#SiteImageSettingsEditor",
                            text: Displays.SiteImageSettingsEditor(context: context)));
                    switch (siteModel.ReferenceType)
                    {
                        case "Sites":
                            hb
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#HtmlsSettingsEditor",
                                            text: Displays.Html(context: context)),
                                    _using: context.ContractSettings.Html != false);
                            break;
                        case "Dashboards":
                            hb
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#DashboardPartSettingsEditor",
                                            text: Displays.DashboardParts(context: context))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewsSettingsEditor",
                                            text: Displays.DataView(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#HtmlsSettingsEditor",
                                            text: Displays.Html(context: context)),
                                    _using: context.ContractSettings.Html != false); ;
                            break;
                        case "Wikis":
                            hb
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#NotificationsSettingsEditor",
                                            text: Displays.Notifications(context: context)),
                                    _using: context.ContractSettings.Notice != false)
                                .Li(action: () => hb
                                    .A(
                                        href: "#MailSettingsEditor",
                                        text: Displays.Mail(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#HtmlsSettingsEditor",
                                            text: Displays.Html(context: context)),
                                    _using: context.ContractSettings.Html != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#PublishSettingsEditor",
                                            text: Displays.Publish(context: context)),
                                    _using: context.ContractSettings.Extensions.Get("Publish"));
                            break;
                        default:
                            hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#GridSettingsEditor",
                                        text: Displays.Grid(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FiltersSettingsEditor",
                                        text: Displays.Filters(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#AggregationsSettingsEditor",
                                        text: Displays.Aggregations(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorSettingsEditor",
                                        text: Displays.Editor(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#LinksSettingsEditor",
                                        text: Displays.Links(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#HistoriesSettingsEditor",
                                        text: Displays.Histories(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#MoveSettingsEditor",
                                        text: Displays.Move(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SummariesSettingsEditor",
                                        text: Displays.Summaries(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#FormulasSettingsEditor",
                                        text: Displays.Formulas(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessesSettingsEditor",
                                        text: Displays.Processes(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StatusControlsSettingsEditor",
                                        text: Displays.StatusControls(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewsSettingsEditor",
                                        text: Displays.DataView(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#NotificationsSettingsEditor",
                                            text: Displays.Notifications(context: context)),
                                    _using: context.ContractSettings.Notice != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#RemindersSettingsEditor",
                                            text: Displays.Reminders(context: context)),
                                    _using: context.ContractSettings.Remind != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ImportsSettingsEditor",
                                            text: Displays.Import(context: context)),
                                    _using: context.ContractSettings.Import != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExportsSettingsEditor",
                                            text: Displays.Export(context: context)),
                                    _using: context.ContractSettings.Export != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#CalendarSettingsEditor",
                                            text: Displays.Calendar(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Calendar")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#CrosstabSettingsEditor",
                                            text: Displays.Crosstab(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Crosstab")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#GanttSettingsEditor",
                                            text: Displays.Gantt(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Gantt")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#BurnDownSettingsEditor",
                                            text: Displays.BurnDown(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "BurnDown")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#TimeSeriesSettingsEditor",
                                            text: Displays.TimeSeries(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "TimeSeries")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#AnalySettingsEditor",
                                            text: Displays.Analy(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Analy")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#KambanSettingsEditor",
                                            text: Displays.Kamban(context: context)),
                                    _using: Def.ViewModeDefinitionCollection
                                        .Where(o => o.Name == "Kamban")
                                        .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ImageLibSettingsEditor",
                                            text: Displays.ImageLib(context: context)),
                                    _using:
                                        context.ContractSettings.Images() &&
                                        Def.ViewModeDefinitionCollection
                                            .Where(o => o.Name == "ImageLib")
                                            .Any(o => o.ReferenceType == siteModel.ReferenceType))
                                .Li(action: () => hb
                                    .A(
                                        href: "#SearchSettingsEditor",
                                        text: Displays.Search(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#MailSettingsEditor",
                                            text: Displays.Mail(context: context)),
                                    _using: context.ContractSettings.Notice != false)
                                .Li(action: () => hb
                                    .A(
                                        href: "#SiteIntegrationEditor",
                                        text: Displays.SiteIntegration(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#StylesSettingsEditor",
                                            text: Displays.Styles(context: context)),
                                    _using: context.ContractSettings.Style != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ScriptsSettingsEditor",
                                            text: Displays.Scripts(context: context)),
                                    _using: context.ContractSettings.Script != false)
                                 .Li(
                                    action: () => hb
                                        .A(
                                            href: "#HtmlsSettingsEditor",
                                            text: Displays.Html(context: context)),
                                    _using: context.ContractSettings.Html != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ServerScriptsSettingsEditor",
                                            text: Displays.ServerScript(context: context)),
                                    _using: context.ContractSettings.ServerScript != false
                                        && Parameters.Script.ServerScript != false)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#PublishSettingsEditor",
                                            text: Displays.Publish(context: context)),
                                    _using: context.ContractSettings.Extensions.Get("Publish"));
                            break;
                    }
                    hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetSiteAccessControl",
                                text: Displays.SiteAccessControl(context: context),
                                _using: context.CanManagePermission(ss: ss)))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetRecordAccessControl",
                                    text: Displays.RecordAccessControl(context: context)),
                            _using: EnableAdvancedPermissions(
                                context: context, siteModel: siteModel))
                        .Li(
                            action: () => hb
                                .A(
                                    href: "#FieldSetColumnAccessControl",
                                    text: Displays.ColumnAccessControl(context: context)),
                            _using: EnableAdvancedPermissions(
                                context: context, siteModel: siteModel))
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetHistories",
                                text: Displays.ChangeHistoryList(context: context)));
                }
                //SimpleMode
                hb
                    .Li(attributes: new HtmlAttributes()
                            .Id("SimpleModeToggleContainer")
                            .Class("ignore-tab"),
                        action: () => hb
                        .Input(
                            attributes: new HtmlAttributes()
                                .Id("SimpleModeToggle")
                                .Name("SimpleModeToggle")
                                .Type("checkbox")
                        )
                        .Label(
                            attributes: new HtmlAttributes()
                                .For("SimpleModeToggle")
                        )
                        .Input(
                            attributes: new HtmlAttributes()
                                .Type("hidden")
                                .Id("SimpleModeEnabled")
                                .Value(Parameters.Site.SimpleMode.Enabled.ToString().ToLower())
                        )
                        .Input(
                            attributes: new HtmlAttributes()
                                .Type("hidden")
                                .Id("SimpleModeDefault")
                                .Value(Parameters.Site.SimpleMode.Default.ToString().ToLower())
                        )
                        .Input(
                            attributes: new HtmlAttributes()
                                .Type("hidden")
                                .Id("SimpleModeDisplaySwitch")
                                .Value(Parameters.Site.SimpleMode.DisplaySwitch.ToString().ToLower())
                        )
                        .Input(
                            attributes: new HtmlAttributes()
                                .Type("hidden")
                                .Id("SimpleModeTabs")
                                .Value(string.Join(",", Parameters.Site.SimpleMode.Tabs.Select(s => s.Trim())))
                        )
                     );
            });
            return tags;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteTop(Context context)
        {
            var hb = new HtmlBuilder();
            var ss = new SiteSettings();
            ss.ReferenceType = "Sites";
            ss.PermissionType = context.SiteTopPermission();
            var verType = Versions.VerTypes.Latest;
            var siteConditions = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu
                .SiteConditions(context: context, ss: ss);
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                referenceType: "Sites",
                useTitle: false,
                script: (Parameters.Site.TopOrderBy <= 0
                    || context.UserId == Parameters.Site.TopOrderBy
                    || Permissions.PrivilegedUsers(loginId: context.LoginId))
                        ? "$p.setSiteMenu();"
                        : null,
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("MainForm")
                                .Class("main-form")
                                .Action(Locations.ItemAction(
                                    context: context,
                                    id: 0)),
                            action: () => hb
                                .StartGuide(context: context)
                                .SiteMenu(
                                    context: context,
                                    siteModel: null,
                                    siteConditions: siteConditions)
                                .SiteMenuData()
                                .LinkDialog(context: context))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSitePackageDialog")
                            .Class("dialog")
                            .Title(Displays.ImportSitePackage(context: context)))
                        .SiteTitleDialog(
                            context: context,
                            ss: ss)
                        .MainCommands(
                            context: context,
                            ss: ss,
                            verType: verType,
                            backButton: false);
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SiteMenu(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            if (context.PermissionHash.ContainsKey(siteModel.SiteId))
            {
                ss.PermissionType = context.PermissionHash.Get(siteModel.SiteId);
            }
            var invalid = SiteValidators.OnShowingMenu(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            var siteConditions = SiteInfo.TenantCaches
                .Get(context.TenantId)?
                .SiteMenu
                .SiteConditions(context: context, ss: ss);
            return hb.Template(
                context: context,
                ss: ss,
                view: Views.GetBySession(
                    context: context,
                    ss: ss),
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                useTitle: false,
                script: "$p.setSiteMenu();",
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("MainForm")
                                .Class("main-form")
                                .Action(Locations.ItemAction(
                                    context: context,
                                    id: ss.SiteId)),
                            action: () => hb
                                .SiteMenu(
                                    context: context,
                                    siteModel: siteModel,
                                    siteConditions: siteConditions)
                                .SiteMenuData()
                                .LinkDialog(context: context))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSitePackageDialog")
                            .Class("dialog")
                            .Title(Displays.ImportSitePackage(context: context)))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSitePackageDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSitePackage(context: context)))
                        .SiteTitleDialog(
                            context: context,
                            ss: siteModel.SiteSettings);
                    if (ss.SiteId != 0)
                    {
                        hb.MainCommands(
                            context: context,
                            ss: ss,
                            verType: Versions.VerTypes.Latest);
                    }
                }).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            Context context,
            SiteModel siteModel,
            IEnumerable<SiteCondition> siteConditions)
        {
            var ss = siteModel != null
                ? siteModel.SiteSettings
                : SiteSettingsUtilities.SitesSiteSettings(
                    context: context,
                    siteId: 0);
            ss.PermissionType = siteModel != null
                ? siteModel.SiteSettings.PermissionType
                : context.SiteTopPermission();
            return hb.Div(id: "SiteMenu", action: () => hb
                .Nav(css: "cf", _using: siteModel != null, action: () => hb
                    .Ul(css: "nav-sites", action: () => hb
                        .ToParent(context: context, siteModel: siteModel)))
                .Nav(css: "cf", action: () => hb
                    .Ul(css: "nav-sites sortable", action: () =>
                        Menu(context: context, ss: ss).ForEach(siteModelChild => hb
                            .SiteMenu(
                                context: context,
                                ss: ss,
                                currentSs: siteModelChild.SiteSettings,
                                siteId: siteModelChild.SiteId,
                                referenceType: siteModelChild.ReferenceType,
                                title: siteModelChild.Title.Value,
                                siteConditions: siteConditions)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ToParent(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.SiteMenu(
                    context: context,
                    ss: siteModel.SiteSettings,
                    currentSs: null,
                    siteId: siteModel.ParentId,
                    referenceType: "Sites",
                    title: Displays.ToParent(context: context),
                    toParent: true)
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SiteMenu(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SiteSettings currentSs,
            long siteId,
            string referenceType,
            string title,
            bool toParent = false,
            IEnumerable<SiteCondition> siteConditions = null)
        {
            var siteImageUpdatedTime = BinaryUtilities.SiteImageUpdatedTime(
                context: context,
                ss: ss,
                referenceId: siteId,
                sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail);
            var hasImage = siteImageUpdatedTime > DateTime.FromOADate(0);
            var siteImagePrefix = siteImageUpdatedTime.ToString("?yyyyMMddHHmmss");
            return hb.Li(
                attributes: new HtmlAttributes()
                    .Class(Css.Class("nav-site " + referenceType.ToLower() +
                        (hasImage
                            ? " has-image"
                            : string.Empty),
                         toParent
                            ? " to-parent"
                            : string.Empty))
                    .DataValue(siteId.ToString())
                    .DataType(referenceType),
                action: () => hb
                    .A(
                        attributes: new HtmlAttributes()
                            .Href(SiteHref(
                                context: context,
                                ss: currentSs,
                                siteId: siteId,
                                referenceType: referenceType)),
                        action: () => hb
                            .SiteMenuInnerElements(
                                context: context,
                                siteId: siteId,
                                referenceType: referenceType,
                                title: title,
                                toParent: toParent,
                                hasImage: hasImage,
                                siteImagePrefix: siteImagePrefix,
                                siteConditions: siteConditions)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        internal static string SiteHref(
            Context context, SiteSettings ss, long siteId, string referenceType)
        {
            switch (referenceType)
            {
                case "Wikis":
                    return Locations.ItemEdit(
                        context: context,
                        id: Repository.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectWikis(
                                column: Rds.WikisColumn().WikiId(),
                                where: Rds.WikisWhere().SiteId(siteId))));
                default:
                    var viewMode = ViewModes.GetSessionData(
                        context: context,
                        siteId: siteId,
                        ss: ss);
                    switch (viewMode.ToLower())
                    {
                        case "trashbox":
                            viewMode = "index";
                            break;
                    }
                    return Locations.Get(
                        context: context,
                        parts: new string[]
                        {
                            "Items",
                            siteId.ToString(),
                            viewMode
                        });
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuInnerElements(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string referenceType,
            string title,
            bool toParent,
            bool hasImage,
            string siteImagePrefix,
            IEnumerable<SiteCondition> siteConditions)
        {
            if (toParent)
            {
                hb.SiteMenuParent(
                    context: context,
                    siteId: siteId,
                    title: title,
                    hasImage: hasImage,
                    siteImagePrefix: siteImagePrefix);
            }
            else
            {
                if (context.ThemeVersionOver2_0())
                {
                    if (hasImage)
                    {
                        hb.Div(
                            css: "site-icon",
                            action: () => hb
                                .Img(src: Locations.Get(
                                    context: context,
                                    parts: new string[]
                                    {
                                        "Items",
                                        siteId.ToString(),
                                        "Binaries",
                                        "SiteImageThumbnail",
                                        siteImagePrefix
                                    })));
                    }
                    else
                    {
                        switch (referenceType)
                        {
                            case "Sites":
                                hb.SiteMenuIcon(
                                    context: context,
                                    iconName: "icon-site-sites.svg");
                                break;
                            case "Issues":
                                hb.SiteMenuIcon(
                                    context: context,
                                    iconName: "icon-site-issues.svg");
                                break;
                            case "Results":
                                hb.SiteMenuIcon(
                                    context: context,
                                    iconName: "icon-site-results.svg");
                                break;
                            case "Wikis":
                                hb.SiteMenuIcon(
                                    context: context,
                                    iconName: "icon-site-wikis.svg");
                                break;
                            case "Dashboards":
                                hb.SiteMenuIcon(
                                    context: context,
                                    iconName: "icon-site-dashboards.svg");
                                break;
                            default:
                                break;
                        }
                    }
                }
                hb.SiteMenuChild(
                    context: context,
                    siteId: siteId,
                    title: title,
                    hasImage: hasImage,
                    siteImagePrefix: siteImagePrefix);
            }
            return hb
                .SiteMenuStyle(referenceType: referenceType)
                .SiteMenuConditions(
                    context: context,
                    siteId: siteId,
                    hasImage: hasImage,
                    referenceType: referenceType,
                    siteConditions: siteConditions);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuParent(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string title,
            bool hasImage,
            string siteImagePrefix)
        {
            if (hasImage)
            {
                return hb
                    .Img(
                        src: Locations.Get(
                            context: context,
                            parts: new string[]
                            {
                                "Items",
                                siteId.ToString(),
                                "Binaries",
                                "SiteImageIcon",
                                siteImagePrefix
                            }),
                        css: "site-image-icon")
                    .Span(css: "title", action: () => hb
                        .Text(title));
            }
            else
            {
                return hb.Icon(
                    iconCss: "ui-icon-circle-arrow-n",
                    cssText: "title",
                    text: title);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuIcon(
            this HtmlBuilder hb,
            Context context,
            string iconName,
            bool _using = true)
        {
            if (_using == false) return hb;
            return hb.Div(
                css: "site-icon",
                action: () => hb
                    .Img(src: Locations.Get(
                        context: context,
                        "Images",
                        iconName)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuChild(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string title,
            bool hasImage,
            string siteImagePrefix)
        {
            if (context.ThemeVersion1_0() && hasImage)
            {
                hb.Img(
                    src: Locations.Get(
                        context: context,
                        parts: new string[]
                        {
                            "Items",
                            siteId.ToString(),
                            "Binaries",
                            "SiteImageThumbnail",
                            siteImagePrefix
                        }),
                    css: "site-image-thumbnail");
            }
            return hb.Span(css: "title", action: () => hb
                .Text(title));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuStyle(
            this HtmlBuilder hb,
            string referenceType)
        {
            if (referenceType == "Sites")
            {
                return hb.Div(css: "heading");
            }
            else
            {
                switch (referenceType)
                {
                    case "Wikis":
                    case "Dashboards":
                        return hb;
                    default: return hb.StackStyles();
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuConditions(
            this HtmlBuilder hb,
            Context context,
            bool hasImage,
            string referenceType,
            long siteId,
            IEnumerable<SiteCondition> siteConditions)
        {
            if (siteConditions != null &&
                siteConditions.Any(o => o.SiteId == siteId))
            {
                var condition = siteConditions
                    .FirstOrDefault(o => o.SiteId == siteId);
                hb.Div(
                    css: "conditions",
                    action: () => hb
                        .ElapsedTime(
                            context: context,
                            value: condition.UpdatedTime.ToLocal(context: context),
                            _using: condition.UpdatedTime > DateTime.MinValue)
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("reference material-symbols-outlined")
                                .Title(ReferenceTypeDisplayName(
                                    context: context,
                                    referenceType: referenceType)),
                            _using: hasImage,
                            action: () =>
                            {
                                switch (referenceType)
                                {
                                    case "Sites":
                                        hb.Text("folder");
                                        break;
                                    case "Issues":
                                        hb.Text("view_timeline");
                                        break;
                                    case "Results":
                                        hb.Text("table");
                                        break;
                                    case "Wikis":
                                        hb.Text("text_snippet");
                                        break;
                                    case "Dashboards":
                                        hb.Text("dashboard");
                                        break;
                                    default:
                                        break;
                                }
                            })
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("count")
                                .Title(Displays.Quantity(context: context)),
                            _using: condition.ItemCount > 0,
                            action: () => hb
                                .Text(condition.ItemCount.ToString()))
                        .Span(
                            attributes: new HtmlAttributes()
                                .Class("overdue")
                                .Title(Displays.Overdue(context: context)),
                            _using: condition.OverdueCount > 0,
                            action: () => hb
                                .Text(condition.OverdueCount.ToString())));
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<SiteModel> Menu(Context context, SiteSettings ss)
        {
            var siteCollection = new SiteCollection(
                context: context,
                column: Rds.SitesColumn()
                    .SiteId()
                    .Title()
                    .ReferenceType()
                    .SiteSettings(),
                where: Rds.SitesWhere()
                    .TenantId(context.TenantId)
                    .ParentId(ss.SiteId)
                    .Add(
                        raw: Def.Sql.HasPermission,
                        _using: !context.HasPrivilege));
            var orderModel = new OrderModel(
                context: context,
                ss: ss,
                referenceId: ss.SiteId,
                referenceType: "Sites");
            siteCollection.ForEach(siteModel =>
            {
                var index = orderModel.Data.IndexOf(siteModel.SiteId);
                siteModel.SiteMenu = (index != -1 ? index : int.MaxValue);
            });
            return siteCollection
                .Where(o => context.HasPrivilege
                    || !(context.PermissionHash.Get(o.SiteId) == Permissions.Types.Read
                        && o.SiteSettings?.NoDisplayIfReadOnly == true))
                .OrderBy(o => o.SiteMenu);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteMenuData(this HtmlBuilder hb)
        {
            return hb
                .Hidden(attributes: new HtmlAttributes()
                    .Id("MoveSiteMenu")
                    .DataAction("MoveSiteMenu")
                    .DataMethod("post"))
                .Hidden(attributes: new HtmlAttributes()
                    .Id("SortSiteMenu")
                    .DataAction("SortSiteMenu")
                    .DataMethod("put"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StartGuide(this HtmlBuilder hb, Context context)
        {
            return context.UserSettings.StartGuide(context: context)
                ? hb.Div(
                    id: "StartGuide",
                    action: () => hb
                        .Div(
                            id: "StartGuideContents",
                            action: () => hb
                                .A(
                                    href: Parameters.General.HtmlApplicationBuildingGuideUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "hands-on1",
                                        "startguide"),
                                    action: () => hb
                                        .Img(src: Locations.Get(
                                            context: context,
                                            "Images",
                                            "Hayato1.png"))
                                        .Div(action: () => hb
                                            .Text(text: Displays.ApplicationBuildingGuide(context: context))))
                                .A(
                                    href: Parameters.General.HtmlUserManualUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "manual",
                                        "startguide"),
                                    action: () => hb
                                        .Img(src: Locations.Get(
                                            context: context,
                                            "Images",
                                            "Hayato2.png"))
                                        .Text(text: Displays.UserManual(context: context)))
                                .A(
                                    href: Parameters.General.HtmlEnterPriseEditionUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "enterprise",
                                        "startguide"),
                                    action: () => hb
                                        .Img(src: Locations.Get(
                                            context: context,
                                            "Images",
                                            "Hayato3.png"))
                                        .Text(text: Displays.EnterpriseEdition(context: context)))
                                .A(
                                    href: Parameters.General.HtmlSupportUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "support",
                                        "startguide"),
                                    action: () => hb
                                        .Img(src: Locations.Get(
                                            context: context,
                                            "Images",
                                            "Hayato4.png"))
                                        .Text(text: Displays.NeedHelp(context: context))))
                        .FieldCheckBox(
                            fieldId: "DisableStartGuideField",
                            controlId: "DisableStartGuide",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.DisableStartGuide(context: context),
                            onChange: "$p.setStartGuide($(this).prop('checked'));")
                        .Icon(
                            iconCss: "ui-icon ui-icon-closethick",
                            onClick: "$('#StartGuide').hide();"))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteTitleDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("SiteTitleDialog")
                    .Class("dialog")
                    .Title(Displays.EnterTitle(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("SiteTitleForm")
                            .Action(Locations.ItemAction(
                                context: context,
                                id: ss.SiteId)),
                        action: () => hb
                            .FieldTextBox(
                                controlId: "SiteTitle",
                                controlCss: " focus always-send",
                                labelText: Displays.Title(context: context),
                                validateRequired: true)
                            .Hidden(
                                controlId: "TemplateId",
                                css: " always-send")
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "CreateByTemplate",
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate button-positive",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-gear",
                                    action: "CreateByTemplate",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon button-neutral",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(Context context)
        {
            var controlId = context.Forms.ControlId();
            var templateId = context.Forms.List(controlId).FirstOrDefault();
            var template = templateId?.StartsWith("UserTemplate") == true
                ? GetUserTemplates(context: context, templateId: templateId).FirstOrDefault()
                : Def.TemplateDefinitionCollection.FirstOrDefault(o => o.Id == templateId);
            return template != null
                ? PreviewTemplate(context: context, template: template, controlId: controlId)
                : new ResponseCollection(context: context)
                    .Html(
                        "#" + controlId + "Viewer .description",
                        Displays.SelectTemplate(context: context))
                    .Toggle("#" + controlId + "Viewer .viewer", false)
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(
            Context context, TemplateDefinition template, string controlId)
        {
            if (template.CustomApps > 0)
            {
                return PreviewUserTemplate(
                    context: context,
                    template: template,
                    controlId: controlId);
            }
            var hb = new HtmlBuilder();
            var ss = template.SiteSettingsTemplate.DeserializeSiteSettings(context: context);
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            var html = string.Empty;
            switch (ss.ReferenceType)
            {
                case "Sites":
                case "Dashboards":
                    html = PreviewTemplate(
                        context: context,
                        ss: ss,
                        title: template.Title).ToString();
                    break;
                case "Issues":
                    html = IssueUtilities.PreviewTemplate(
                        context: context, ss: ss).ToString();
                    break;
                case "Results":
                    html = ResultUtilities.PreviewTemplate(
                        context: context, ss: ss).ToString();
                    break;
                case "Wikis":
                    html = WikiUtilities.PreviewTemplate(
                        context: context,
                        ss: ss,
                        body: template.Body).ToString();
                    break;
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#" + controlId + "Viewer .description",
                    hb.Text(text: Strings.CoalesceEmpty(
                        template.Description, template.Title)))
                .Html("#" + controlId + "Viewer .viewer", html)
                .Invoke("setTemplateViewer")
                .Toggle("#" + controlId + "Viewer .viewer", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewUserTemplate(
            Context context, TemplateDefinition template, string controlId)
        {
            var hb = new HtmlBuilder();
            var name = Strings.NewGuid();
            var html = hb
                .Div(css: "samples-displayed", action: () => hb
                    .Text(text: Displays.SamplesDisplayed(context: context)))
                .Div(css: "template-tab-container", action: () => hb
                    .Ul(action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Editor",
                                text: Displays.Menu(context: context))))
                    .TabsPanelField(
                        id: name + "Editor",
                        action: () => hb
                            .Div(action: () => hb
                                .FieldMarkDown(
                                    context: context,
                                    ss: null,
                                    controlId: "Description",
                                    fieldCss: "field-wide",
                                    labelText: Displays.Description(context: context),
                                    text: template.Description,
                                    readOnly: true))
                                .Div(css: "heading", _using: true)))
                .ToString();
            return new ResponseCollection(context: context)
                .Html(
                    "#" + controlId + "Viewer .description",
                    new HtmlBuilder().Text(template.Title))
                .Html("#" + controlId + "Viewer .viewer", html)
                .Invoke("setTemplateViewer")
                .Toggle("#" + controlId + "Viewer .viewer", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PreviewTemplate(Context context, SiteSettings ss, string title)
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
                                text: Displays.Menu(context: context))))
                    .TabsPanelField(
                        id: name + "Editor",
                        action: () => hb
                            .Div(css: "nav-site"
                                    + (ss.ReferenceType != "Dashboards"
                                        ? " sites"
                                        : " dashboards"), action: () => hb
                                .Span(css: "title", action: () => hb.Text(title))
                                .SiteMenuIcon(
                                    context: context,
                                    iconName: ss.ReferenceType != "Dashboards"
                                        ? "icon-site-sites.svg"
                                        : "icon-site-dashboards.svg",
                                    _using: context.ThemeVersionOver2_0())
                                .Div(css: "heading", _using: ss.ReferenceType != "Dashboards"))))
                                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, SiteModel siteModel)
        {
            var invalid = SiteValidators.OnEditing(
                context: context, ss: siteModel.SiteSettings, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: siteModel.SiteSettings,
                view: null,
                siteId: siteModel.SiteId,
                parentId: siteModel.ParentId,
                referenceType: "Sites",
                siteReferenceType: siteModel.ReferenceType,
                title: siteModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Sites(context: context) + " - " + Displays.New(context: context)
                    : siteModel.Title + " - " + Displays.Manage(context: context),
                script: siteModel.MethodType == BaseModel.MethodTypes.Edit
                    ? "$p.setPaging('MoveTargetsSourceColumns');"
                    : null,
                action: () => hb
                    .Editor(context: context, siteModel: siteModel)
                    .Hidden(
                        controlId: "BaseUrl",
                        value: Locations.BaseUrl(context: context))
                    .Hidden(
                        controlId: "Ver",
                        value: siteModel.Ver.ToString())
                    .Hidden(
                        controlId: "SwitchTargets",
                        css: "always-send",
                        value: siteModel.SiteId.ToString(),
                        _using: !context.Ajax)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: siteModel);
            var showComments = true;
            var showBanner = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: siteModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: siteModel,
                            tableName: "Sites",
                            switcher: false)
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: siteModel.Comments,
                                    column: commentsColumn,
                                    verType: siteModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EnterPriseBanner", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes().Href(Parameters.General.HtmlEnterPriseEditionUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "enterprise",
                                        "table-management")),
                                    action: () => hb
                                        .Img(
                                            id: "EnterPriseBannerImage",
                                            src: Locations.Get(
                                                context: context,
                                                "Images",
                                                "enterprise-banner.png"))),
                            _using: showBanner)
                        .Div(
                            id: "SupportBanner", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes().Href(Parameters.General.HtmlSupportUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "support",
                                        "table-management")),
                                    action: () => hb
                                        .Img(
                                            id: "SupportBannerImage",
                                            src: Locations.Get(
                                                context: context,
                                                "Images",
                                                "support-banner.png"))),
                            _using: showBanner)
                        .Div(
                            id: "CasesBanner", action: () => hb
                                .A(
                                    attributes: new HtmlAttributes().Href(Parameters.General.HtmlCasesUrl.Params(
                                        Parameters.General.PleasanterSource,
                                        "cases",
                                        "table-management")),
                                    action: () => hb
                                        .Img(
                                            id: "CasesBannerImage",
                                            src: Locations.Get(
                                                context: context,
                                                "Images",
                                                "cases-banner.png"))),
                            _using: showBanner)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container" + tabsCss,
                            action: () => hb
                                .EditorTabs(context: context, siteModel: siteModel)
                                .FieldSetGeneral(context: context, siteModel: siteModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: siteModel.MethodType != BaseModel.MethodTypes.New)
                                .TabsPanelField(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetSiteAccessControl")
                                        .DataAction("Permissions")
                                        .DataMethod("post"),
                                    innerId: "FieldSetSiteAccessControlEditor",
                                    _using: context.CanManagePermission(ss: ss))
                                .TabsPanelField(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetRecordAccessControl")
                                        .DataAction("PermissionForRecord")
                                        .DataMethod("post"),
                                    innerId: "FieldSetRecordAccessControlEditor",
                                    _using: EnableAdvancedPermissions(
                                        context: context, siteModel: siteModel))
                                .TabsPanelField(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetColumnAccessControl")
                                        .DataAction("ColumnAccessControl")
                                        .DataMethod("post"),
                                    innerId: "FieldSetColumnAccessControlEditor",
                                    _using: EnableAdvancedPermissions(
                                        context: context, siteModel: siteModel))
                                .MainCommands(
                                    context: context,
                                    ss: siteModel.SiteSettings,
                                    verType: siteModel.VerType,
                                    updateButton: true,
                                    copyButton: siteModel.ParentId > 0
                                        || Permissions.SiteTopPermission(context: context) == (Permissions.Types)Parameters.Permissions.Manager,
                                    mailButton: true,
                                    deleteButton: true))
                        .Hidden(
                            controlId: "MethodType",
                            value: siteModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Sites_Timestamp",
                            css: "control-hidden always-send",
                            value: siteModel.Timestamp))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Sites",
                    referenceId: siteModel.SiteId,
                    referenceVer: siteModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .DeleteSiteDialog(context: context)
                .Div(attributes: new HtmlAttributes()
                    .Id("GridColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("FilterColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("AggregationDetailsDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("EditorColumnDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("SearchEditorColumnDialog")
                    .Class("dialog")
                    .Title(Displays.Search(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("TabDialog")
                    .Class("dialog")
                    .Title(Displays.Tab(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("SummaryDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("FormulaDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ProcessDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ProcessValidateInputDialog")
                    .Class("dialog")
                    .Title(Displays.ValidateInput(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ViewDialog")
                    .Class("dialog")
                    .Title(Displays.DataView(context: context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ProcessDataChangeDialog")
                    .Class("dialog")
                    .Title(Displays.DataChanges(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ProcessNotificationDialog")
                        .Class("dialog")
                        .Title(Displays.Notifications(context: context)),
                    _using: context.ContractSettings.Notice != false)
                .Div(attributes: new HtmlAttributes()
                    .Id("StatusControlDialog")
                    .Class("dialog")
                    .Title(Displays.AdvancedSetting(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("NotificationDialog")
                        .Class("dialog")
                        .Title(Displays.Notifications(context: context)),
                    _using: context.ContractSettings.Notice != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ReminderDialog")
                        .Class("dialog")
                        .Title(Displays.Reminders(context: context)),
                    _using: context.ContractSettings.Remind != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ExportDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context)),
                    _using: context.ContractSettings.Export != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ExportColumnsDialog")
                        .Class("dialog")
                        .Title(Displays.AdvancedSetting(context: context)),
                    _using: context.ContractSettings.Export != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("StyleDialog")
                        .Class("dialog")
                        .Title(Displays.Style(context: context)),
                    _using: context.ContractSettings.Style != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ScriptDialog")
                        .Class("dialog")
                        .Title(Displays.Script(context: context)),
                    _using: context.ContractSettings.Script != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("HtmlDialog")
                        .Class("dialog")
                        .Title(Displays.Html(context: context)),
                    _using: context.ContractSettings.Html != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("DashboardPartDialog")
                        .Class("dialog")
                        .Title(Displays.Dashboards(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("DashboardPartTimeLineSitesDialog")
                        .Class("dialog")
                        .Title(Displays.SiteId(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("DashboardPartCalendarSitesDialog")
                        .Class("dialog")
                        .Title(Displays.SiteId(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("DashboardPartKambanSitesDialog")
                        .Class("dialog")
                        .Title(Displays.SiteId(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("DashboardPartIndexSitesDialog")
                        .Class("dialog")
                        .Title(Displays.SiteId(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("ServerScriptDialog")
                        .Class("dialog")
                        .Title(Displays.ServerScript(context: context)),
                    _using: context.ContractSettings.ServerScript != false
                        && Parameters.Script.ServerScript != false)
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("BulkUpdateColumnDialog")
                        .Class("dialog")
                        .Title(Displays.BulkUpdateColumnSettings(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("BulkUpdateColumnDetailDialog")
                        .Class("dialog")
                        .Title(Displays.AdvancedSetting(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("RelatingColumnDialog")
                        .Class("dialog")
                        .Title(Displays.RelatingColumnSettings(context: context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("SetNumericRangeDialog")
                        .Class("dialog")
                        .Title(Displays.NumericRange(context)))
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("SetDateRangeDialog")
                        .Class("dialog")
                        .Title(Displays.DateRange(context)))
                .Div(attributes: new HtmlAttributes()
                    .Id("ExportSelectorDialog")
                    .Class("dialog")
                    .Title(Displays.Export(context: context)),
                    _using: context.ContractSettings.Export != false)
                .PermissionsDialog(context: context)
                .PermissionForCreatingDialog(context: context)
                .PermissionForUpdatingDialog(context: context)
                .ColumnAccessControlDialog(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool EnableAdvancedPermissions(Context context, SiteModel siteModel)
        {
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                case "Results":
                    return context.CanManagePermission(ss: siteModel.SiteSettings);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb, Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var titleColumn = siteModel.SiteSettings.GetColumn(
                context: context,
                columnName: "Title");
            hb.TabsPanelField(id: "FieldSetGeneral", action: () =>
            {
                hb
                    .FieldText(
                        controlId: "Sites_SiteId",
                        labelText: Displays.Sites_SiteId(context: context),
                        text: siteModel.SiteId.ToString())
                    .FieldText(
                        controlId: "Sites_Ver",
                        controlCss: siteModel.SiteSettings?.GetColumn(
                            context: context,
                            columnName: "Ver").ControlCss,
                        labelText: Displays.Sites_Ver(context: context),
                        text: siteModel.Ver.ToString())
                    .FieldTextBox(
                        controlId: "Sites_Title",
                        fieldCss: "field-wide",
                        controlCss: " focus",
                        labelText: Displays.Sites_Title(context: context),
                        text: siteModel.Title.Value.ToString(),
                        validateRequired: titleColumn.ValidateRequired ?? false,
                        validateMaxLength: titleColumn.ValidateMaxLength ?? 0,
                        _using: siteModel.ReferenceType != "Wikis")
                    .FieldTextBox(
                        controlId: "Sites_SiteName",
                        fieldCss: "field-normal",
                        labelText: Displays.Sites_SiteName(context: context),
                        text: siteModel.SiteName,
                        validateMaxLength: ss.GetColumn(
                            context: context,
                            columnName: "SiteName")
                                ?.ValidateMaxLength ?? 0)
                    .FieldTextBox(
                        controlId: "Sites_SiteGroupName",
                        fieldCss: "field-normal",
                        labelText: Displays.Sites_SiteGroupName(context: context),
                        text: siteModel.SiteGroupName,
                        validateMaxLength: ss.GetColumn(
                            context: context,
                            columnName: "SiteGroupName")
                                ?.ValidateMaxLength ?? 0,
                        _using: siteModel.ReferenceType != "Wikis")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_Body",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_Body(context: context),
                        text: siteModel.Body,
                        mobile: context.Mobile,
                        _using: siteModel.ReferenceType != "Wikis")
                    .Field(
                        fieldCss: "field-normal",
                        labelText: Displays.Sites_ReferenceType(context: context),
                        controlAction: () => hb
                            .ReferenceType(
                                context: context,
                                referenceType: siteModel.ReferenceType,
                                methodType: siteModel.MethodType))
                    .FieldCheckBox(
                        controlId: "DisableSiteConditions",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.DisableSiteConditions(context: context),
                        _checked: Parameters.Site.DisableSiteConditions == true || ss.DisableSiteConditions == true,
                        disabled: Parameters.Site.DisableSiteConditions == true)
                    .VerUpCheckBox(
                        context: context,
                        ss: ss,
                        baseModel: siteModel);
            });
            hb.GuideEditor(
                context: context,
                ss: ss,
                siteModel: siteModel);
            if (siteModel.MethodType != BaseModel.MethodTypes.New)
            {
                hb.SiteImageSettingsEditor(
                    context: context,
                    ss: siteModel.SiteSettings);
                switch (siteModel.ReferenceType)
                {
                    case "Sites":
                        hb
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HtmlsSettingsEditor(context: context, ss: siteModel.SiteSettings);
                        break;
                    case "Dashboards":
                        hb
                            .DashboardPartSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ViewsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HtmlsSettingsEditor(context: context, ss: siteModel.SiteSettings);
                        break;
                    case "Wikis":
                        hb
                            .NotificationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .MailSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HtmlsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .PublishSettingsEditor(
                                context: context,
                                ss: siteModel.SiteSettings,
                                publish: siteModel.Publish);
                        break;
                    default:
                        hb
                            .GridSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .FiltersSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .AggregationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .EditorSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .LinksSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HistoriesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .MoveSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SummariesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .FormulasSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ProcessesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .StatusControlsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ViewsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .NotificationsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .RemindersSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ImportsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ExportsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .CalendarSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .CrosstabSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .GanttSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .BurnDownSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .TimeSeriesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .AnalySettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .KambanSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ImageLibSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SearchSettingsEditor(context: context, siteModel: siteModel)
                            .MailSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .SiteIntegrationEditor(context: context, ss: siteModel.SiteSettings)
                            .StylesSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .HtmlsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .ServerScriptsSettingsEditor(context: context, ss: siteModel.SiteSettings)
                            .PublishSettingsEditor(
                                context: context,
                                ss: siteModel.SiteSettings,
                                publish: siteModel.Publish);
                        break;
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GuideEditor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SiteModel siteModel)
        {
            return hb.TabsPanelField(id: "GuideEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.Guide(context: context),
                    action: () => hb
                    .FieldCheckBox(
                        controlId: "GuideAllowExpand",
                        fieldCss: "field-normal",
                        labelText: Displays.CommonAllowExpand(context: context),
                        _checked: ss.GuideAllowExpand == true)
                    .FieldDropDown(
                        context: context,
                        fieldId: "GuideExpandField",
                        controlId: "GuideExpand",
                        fieldCss: "field-auto-thin" + (ss.GuideExpand.IsNullOrEmpty()
                            ? " hidden"
                            : string.Empty),
                        labelText: Displays.Expand(context: context),
                        optionCollection: new Dictionary<string, string>
                            {
                                {
                                    "1",
                                    Displays.Open(context:context)
                                },
                                {
                                    "0",
                                    Displays.Close(context: context)
                                }
                            },
                        selectedValue: ss.GuideExpand)
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_GridGuide",
                        fieldCss: "field-wide",
                        labelText: ss.ReferenceType == "Sites"
                            ? Displays.MenuGuide(context: context)
                            : (ss.ReferenceType == "Dashboards"
                                ? Displays.DashboardGuide(context: context)
                                : Displays.Sites_GridGuide(context: context)),
                        text: siteModel.GridGuide,
                        mobile: context.Mobile,
                        _using: siteModel.ReferenceType != "Wikis")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_EditorGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_EditorGuide(context: context),
                        text: siteModel.EditorGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_CalendarGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_CalendarGuide(context: context),
                        text: siteModel.CalendarGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_CrosstabGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_CrosstabGuide(context: context),
                        text: siteModel.CrosstabGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_GanttGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_GanttGuide(context: context),
                        text: siteModel.GanttGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType == "Issues")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_BurnDownGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_BurnDownGuide(context: context),
                        text: siteModel.BurnDownGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType == "Issues")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_TimeSeriesGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_TimeSeriesGuide(context: context),
                        text: siteModel.TimeSeriesGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_AnalyGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_AnalyGuide(context: context),
                        text: siteModel.AnalyGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_KambanGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_KambanGuide(context: context),
                        text: siteModel.KambanGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "Sites_ImageLibGuide",
                        fieldCss: "field-wide",
                        labelText: Displays.Sites_ImageLibGuide(context: context),
                        text: siteModel.ImageLibGuide,
                        mobile: context.Mobile,
                        _using: ss.ReferenceType != "Sites"
                            && ss.ReferenceType != "Wikis"
                            && ss.ReferenceType != "Dashboards")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SiteImageSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "SiteImageSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.Icon(context: context),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.File,
                            controlId: "SiteImage",
                            fieldCss: "field-auto-thin",
                            controlCss: " w400",
                            labelText: Displays.File(context: context))
                        .Button(
                            controlId: "SetSiteImage",
                            controlCss: "button-icon button-positive",
                            text: Displays.Upload(context: context),
                            onClick: "$p.uploadSiteImage($(this));",
                            icon: "ui-icon-disk",
                            action: "binaries/updatesiteimage",
                            method: "post")
                        .Button(
                            controlCss: "button-icon button-negative",
                            text: Displays.Delete(context: context),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-trash",
                            action: "binaries/deletesiteimage",
                            method: "delete",
                            confirm: "ConfirmDelete",
                            _using: BinaryUtilities.ExistsSiteImage(
                                context: context,
                                ss: ss,
                                referenceId: ss.SiteId,
                                sizeType: Libraries.Images.ImageData.SizeTypes.Thumbnail))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "GridSettingsEditor", action: () => hb
                .GridColumns(context: context, ss: ss)
                .FieldSet(id: "BulkUpdateColumnsSettingsEditor",
                    css: " enclosed",
                    legendText: Displays.BulkUpdateColumnSettings(context: context),
                    action: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "MoveUpBulkUpdateColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveUp(context: context),
                            onClick: "$p.setAndSend('#EditBulkUpdateColumns', $(this));",
                            icon: "ui-icon-circle-triangle-n",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "MoveDownBulkUpdateColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveDown(context: context),
                            onClick: "$p.setAndSend('#EditBulkUpdateColumns', $(this));",
                            icon: "ui-icon-circle-triangle-s",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "NewBulkUpdateColumn",
                            text: Displays.New(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.openBulkUpdateColumnDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "put")
                        .Button(
                            controlId: "CopyBulkUpdateColumns",
                            text: Displays.Copy(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAndSend('#EditBulkUpdateColumns', $(this));",
                            icon: "ui-icon-copy",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "DeleteBulkUpdateColumns",
                            text: Displays.Delete(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAndSend('#EditBulkUpdateColumns', $(this));",
                            icon: "ui-icon-trash",
                            action: "SetSiteSettings",
                            method: "delete",
                            confirm: Displays.ConfirmDelete(context: context)))
                    .EditBulkUpdateColumns(
                        context: context,
                        ss: ss))
                .FieldSpinner(
                    controlId: "GridPageSize",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NumberPerPage(context: context),
                    value: ss.GridPageSize.ToDecimal(),
                    min: Parameters.General.GridPageSizeMin,
                    max: Parameters.General.GridPageSizeMax,
                    step: 1,
                    width: 25)
                .FieldDropDown(
                    context: context,
                    controlId: "GridView",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DefaultView(context: context),
                    optionCollection: ss.ViewSelectableOptions(),
                    selectedValue: ss.GridView?.ToString(),
                    insertBlank: true,
                    _using: ss.Views?.Any() == true)
                .FieldCheckBox(
                    controlId: "AllowViewReset",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowViewReset(context: context),
                    _checked: ss.AllowViewReset == true,
                    _using: ss.Views?.Any() == true)
                .FieldDropDown(
                    context: context,
                    controlId: "GridEditorType",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.GridEditorTypes(context: context),
                    optionCollection: new Dictionary<string, string>()
                    {
                        {
                            SiteSettings.GridEditorTypes.Grid.ToInt().ToString(),
                            Displays.EditInGrid(context: context)
                        },
                        {
                            SiteSettings.GridEditorTypes.Dialog.ToInt().ToString(),
                            Displays.EditInDialog(context: context)
                        },
                    },
                    selectedValue: ss.GridEditorType.ToInt().ToString(),
                    insertBlank: true)
                .FieldCheckBox(
                    controlId: "HistoryOnGrid",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.HistoryOnGrid(context: context),
                    _checked: ss.HistoryOnGrid == true)
                .FieldCheckBox(
                    controlId: "AlwaysRequestSearchCondition",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AlwaysRequestSearchCondition(context: context),
                    _checked: ss.AlwaysRequestSearchCondition == true)
                .FieldCheckBox(
                    controlId: "DisableLinkToEdit",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DisableLinkToEdit(context: context),
                    _checked: ss.DisableLinkToEdit == true)
                .FieldCheckBox(
                    controlId: "OpenEditInNewTab",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.OpenEditInNewTab(context: context),
                    _checked: ss.OpenEditInNewTab == true)
                .FieldCheckBox(
                    controlId: "EnableExpandLinkPath",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.ExpandLinkPath(context: context),
                    _checked: ss.EnableExpandLinkPath == true,
                    _using: Parameters.General.EnableExpandLinkPath == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GridColumns(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.FieldSet(
                css: " enclosed-thin",
                legendText: Displays.ListSettings(context: context),
                action: () => hb
                    .FieldSelectable(
                        controlId: "GridColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        controlCss: " always-send send-all",
                        labelText: Displays.CurrentSettings(context: context),
                        listItemCollection: ss.GridSelectableOptions(context: context),
                        selectedValueCollection: new List<string>(),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "MoveUpGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveUp(context: context),
                                    onClick: "$p.moveColumns(event, $(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-n")
                                .Button(
                                    controlId: "MoveDownGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.MoveDown(context: context),
                                    onClick: "$p.moveColumns(event, $(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-s")
                                .Button(
                                    controlId: "OpenGridColumnDialog",
                                    text: Displays.AdvancedSetting(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.openGridColumnDialog($(this));",
                                    icon: "ui-icon-gear",
                                    action: "SetSiteSettings",
                                    method: "put")
                                .Button(
                                    controlId: "ToDisableGridColumns",
                                    controlCss: "button-icon",
                                    text: Displays.ToDisable(context: context),
                                    onClick: "$p.moveColumns(event, $(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "GridSourceColumns",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h350",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: ss.GridSelectableOptions(
                            context: context, enabled: false),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ToEnableGridColumns",
                                    text: Displays.ToEnable(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.moveColumns(event, $(this),'Grid',false,true);",
                                    icon: "ui-icon-circle-triangle-w")
                                .FieldDropDown(
                                    context: context,
                                    controlId: "GridJoin",
                                    fieldCss: "w150",
                                    controlCss: " auto-postback always-send",
                                    optionCollection: ss.JoinOptions(),
                                    addSelectedValue: false,
                                    action: "SetSiteSettings",
                                    method: "post"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(Context context, SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("GridColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .GridColumnDialog(
                        context: context,
                        ss: ss,
                        column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder GridColumnDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column)
        {
            hb.FieldSet(
                css: " enclosed",
                legendText: column.LabelTextDefault,
                action: () =>
                {
                    hb.FieldTextBox(
                        controlId: "GridLabelText",
                        labelText: Displays.DisplayName(context: context),
                        text: column.GridLabelText,
                        validateRequired: true);
                    if (column.TypeName == "datetime")
                    {
                        hb
                            .FieldDropDown(
                                context: context,
                                controlId: "GridFormat",
                                labelText: Displays.GridFormat(context: context),
                                optionCollection: DateTimeOptions(context: context),
                                selectedValue: column.GridFormat);
                    }
                    hb
                        .FieldTextBox(
                            controlId: "ExtendedCellCss",
                            fieldCss: "field-normal",
                            labelText: Displays.ExtendedCellCss(context: context),
                            text: column.ExtendedCellCss)
                        .FieldCheckBox(
                            controlId: "CellSticky",
                            labelText: Displays.StickyOnLeftEdge(context: context),
                            _checked: column.CellSticky == true)
                        .FieldSpinner(
                            controlId: "CellWidth",
                            fieldCss: "field-normal",
                            labelText: Displays.SetCellWidth(context: context),
                            placeholder: Displays.CellWidthMinPx(context: context, data: "50"),
                            value: column.CellWidth >= 50 ? column.CellWidth : null,
                            unit: "px",
                            step: 10)
                        .FieldCheckBox(
                            controlId: "CellWordWrap",
                            labelText: Displays.WrapTextInCell(context: context),
                            _checked: column.CellWordWrap == true)
                        .FieldCheckBox(
                            controlId: "UseGridDesign",
                            labelText: Displays.UseCustomDesign(context: context),
                            _checked: !column.GridDesign.IsNullOrEmpty())
                        .FieldMarkDown(
                            context: context,
                            ss: ss,
                            fieldId: "GridDesignField",
                            controlId: "GridDesign",
                            fieldCss: "field-wide" + (!column.GridDesign.IsNullOrEmpty()
                                ? string.Empty
                                : " hidden"),
                            labelText: Displays.CustomDesign(context: context),
                            placeholder: Displays.CustomDesign(context: context),
                            text: ss.GridDesignEditorText(column),
                            allowImage: column.AllowImage == true,
                            mobile: context.Mobile);
                });
            return hb
                .Hidden(
                    controlId: "GridColumnName",
                    css: "always-send",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetGridColumn",
                        text: Displays.Change(context: context),
                        controlCss: "button-icon validate button-positive",
                        onClick: "$p.setGridColumn($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon button-neutral",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditBulkUpdateColumns(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditBulkUpdateColumns")
                .Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditBulkUpdateColumns",
                attributes: new HtmlAttributes()
                    .DataName("BulkUpdateColumnId")
                    .DataFunc("openBulkUpdateColumnDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditBulkUpdateColumnsHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditBulkUpdateColumnsBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditBulkUpdateColumnsHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: false))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Links(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditBulkUpdateColumnsBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .BulkUpdateColumns?.ForEach(relatingColumn => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(relatingColumn.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(relatingColumn.Id) == true))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Title))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Columns?
                                    .Select(o => GetClassLabelText(ss, o)).Join(", "))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FiltersSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "FiltersSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.FilterSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "FilterColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.FilterSelectableOptions(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: "$p.moveColumns(event, $(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: "$p.moveColumns(event, $(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "OpenFilterColumnDialog",
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openFilterColumnDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "ToDisableFilterColumns",
                                        controlCss: "button-icon",
                                        text: Displays.ToDisable(context: context),
                                        onClick: "$p.moveColumns(event, $(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "FilterSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.FilterSelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "ToEnableFilterColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'Filter',false,true);",
                                        icon: "ui-icon-circle-triangle-w")
                                    .FieldDropDown(
                                        context: context,
                                        controlId: "FilterJoin",
                                        fieldCss: "w150",
                                        controlCss: " auto-postback always-send",
                                        optionCollection: ss.JoinOptions(),
                                        addSelectedValue: false,
                                        action: "SetSiteSettings",
                                        method: "post"))))
                .FieldSpinner(
                    controlId: "NearCompletionTimeAfterDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeAfterDays(context: context),
                    value: ss.NearCompletionTimeAfterDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeAfterDaysMin,
                    max: Parameters.General.NearCompletionTimeAfterDaysMax,
                    step: 1,
                    width: 25)
                .FieldSpinner(
                    controlId: "NearCompletionTimeBeforeDays",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.NearCompletionTimeBeforeDays(context: context),
                    value: ss.NearCompletionTimeBeforeDays.ToDecimal(),
                    min: Parameters.General.NearCompletionTimeBeforeDaysMin,
                    max: Parameters.General.NearCompletionTimeBeforeDaysMax,
                    step: 1,
                    width: 25)
                .FieldCheckBox(
                    controlId: "UseFilterButton",
                    fieldCss: "field-auto-thin both",
                    labelText: Displays.UseFilterButton(context: context),
                    _checked: ss.UseFilterButton == true)
                .FieldCheckBox(
                    controlId: "UseFiltersArea",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseFiltersArea(context: context),
                    _checked: ss.UseFiltersArea == true)
                .FieldCheckBox(
                    controlId: "UseNegativeFilters",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseNegativeFilters(context: context),
                    _checked: ss.UseNegativeFilters == true)
                .FieldCheckBox(
                    controlId: "UseGridHeaderFilters",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseGridHeaderFilters(context: context),
                    _checked: ss.UseRelatingColumnsOnFilter == false && ss.UseGridHeaderFilters == true,
                    disabled: ss.UseRelatingColumnsOnFilter != false)
                .FieldCheckBox(
                    controlId: "UseRelatingColumnsOnFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseRelatingColumns(context: context),
                    _checked: ss.UseRelatingColumnsOnFilter == true)
                .FieldCheckBox(
                    controlId: "UseIncompleteFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseIncompleteFilter(context: context),
                    _checked: ss.UseIncompleteFilter == true)
                .FieldCheckBox(
                    controlId: "UseOwnFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseOwnFilter(context: context),
                    _checked: ss.UseOwnFilter == true)
                .FieldCheckBox(
                    controlId: "UseNearCompletionTimeFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseNearCompletionTimeFilter(context: context),
                    _checked: ss.UseNearCompletionTimeFilter == true)
                .FieldCheckBox(
                    controlId: "UseDelayFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseDelayFilter(context: context),
                    _checked: ss.UseDelayFilter == true)
                .FieldCheckBox(
                    controlId: "UseOverdueFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseOverdueFilter(context: context),
                    _checked: ss.UseOverdueFilter == true)
                .FieldCheckBox(
                    controlId: "UseSearchFilter",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UseSearchFilter(context: context),
                    _checked: ss.UseSearchFilter == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(
            Context context, SiteSettings ss, Column column)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FilterColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FilterColumnDialog(
                        context: context,
                        ss: ss,
                        column: column));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FilterColumnDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column)
        {
            var type = column.TypeName.CsTypeSummary();
            var noSettings = new string[]
            {
                "Owner",
                "Manager",
                "Status"
            }.Contains(column.ColumnName);
            hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.DateFilterSetMode(context: context),
                    action: () => hb.FieldDropDown(
                        context: context,
                        controlId: "DateFilterSetMode",
                        fieldCss: "field-auto-thin",
                        optionCollection: ColumnUtilities.DateFilterSetModeOptions(context),
                        selectedValue: (column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Range)
                            ? ColumnUtilities.DateFilterSetMode.Range.ToInt().ToString()
                            : ColumnUtilities.DateFilterSetMode.Default.ToInt().ToString()),
                    _using: (type == Types.CsDateTime
                        || type == Types.CsNumeric)
                            && !noSettings)
                .FieldSet(
                    id: "FilterColumnSettingField",
                    css: column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default
                        ? " enclosed"
                        : " enclosed hidden",
                    legendText: column.LabelText,
                    action: () =>
                    {
                        switch (type)
                        {
                            case Types.CsBool:
                                hb.FieldDropDown(
                                    context: context,
                                    controlId: "CheckFilterControlType",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.ControlType(context: context),
                                    optionCollection: ColumnUtilities
                                        .CheckFilterControlTypeOptions(context: context),
                                    selectedValue: column.CheckFilterControlType.ToInt().ToString());
                                break;
                            case Types.CsNumeric:
                                if (!noSettings)
                                {
                                    hb
                                        .FieldTextBox(
                                            controlId: "NumFilterMin",
                                            fieldCss: "field-auto-thin",
                                            labelText: Displays.Min(context: context),
                                            text: column.NumFilterMin.TrimEndZero(),
                                            validateRequired: true,
                                            validateNumber: true)
                                        .FieldTextBox(
                                            controlId: "NumFilterMax",
                                            fieldCss: "field-auto-thin",
                                            labelText: Displays.Max(context: context),
                                            text: column.NumFilterMax.TrimEndZero(),
                                            validateRequired: true,
                                            validateNumber: true)
                                        .FieldTextBox(
                                            controlId: "NumFilterStep",
                                            fieldCss: "field-auto-thin",
                                            labelText: Displays.Step(context: context),
                                            text: column.NumFilterStep.TrimEndZero(),
                                            validateRequired: true,
                                            validateNumber: true);
                                }
                                break;
                            case Types.CsDateTime:
                                hb
                                    .FieldTextBox(
                                        controlId: "DateFilterMinSpan",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Min(context: context),
                                        text: column.DateFilterMinSpan.ToString(),
                                        validateRequired: true,
                                        validateNumber: true)
                                    .FieldTextBox(
                                        controlId: "DateFilterMaxSpan",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Max(context: context),
                                        text: column.DateFilterMaxSpan.ToString(),
                                        validateRequired: true,
                                        validateNumber: true)
                                    .FieldCheckBox(
                                        controlId: "DateFilterFy",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.UseFy(context: context),
                                        _checked: column.DateFilterFy == true)
                                    .FieldCheckBox(
                                        controlId: "DateFilterHalf",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.UseHalf(context: context),
                                        _checked: column.DateFilterHalf == true)
                                    .FieldCheckBox(
                                        controlId: "DateFilterQuarter",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.UseQuarter(context: context),
                                        _checked: column.DateFilterQuarter == true)
                                    .FieldCheckBox(
                                        controlId: "DateFilterMonth",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.UseMonth(context: context),
                                        _checked: column.DateFilterMonth == true);
                                break;
                            case Types.CsString:
                                hb.FieldDropDown(
                                    context: context,
                                    controlId: "SearchTypes",
                                    fieldCss: "field-auto-thin",
                                    labelText: Displays.SearchTypes(context: context),
                                    optionCollection: ColumnUtilities.SearchTypeOptions(context),
                                    selectedValue: column.SearchType?.ToInt().ToString());
                                break;
                        }
                    });
            return hb
                .Hidden(
                    controlId: "FilterColumnName",
                    css: "always-send",
                    value: column.ColumnName)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "SetFilterColumn",
                        text: Displays.Change(context: context),
                        controlCss: "button-icon validate button-positive",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon button-neutral",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder AggregationsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "AggregationsSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.AggregationSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "AggregationDestination",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.AggregationDestination(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: "$p.moveColumnsById(event, $(this),'AggregationDestination','',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: "$p.moveColumnsById(event, $(this),'AggregationDestination','',false,true);",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "OpenAggregationDetailsDialog",
                                        controlCss: "button-icon open-dialog",
                                        text: Displays.AdvancedSetting(context: context),
                                        onClick: "$p.openAggregationDetailsDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "DeleteAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Delete(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-e",
                                        action: "SetSiteSettings",
                                        method: "put")))
                        .FieldSelectable(
                            controlId: "AggregationSource",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.AggregationSource(context: context),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "AddAggregations",
                                        controlCss: "button-icon",
                                        text: Displays.Add(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-w",
                                        action: "SetSiteSettings",
                                        method: "post")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder AggregationDetailsDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Aggregation aggregation)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("AggregationDetailsForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "AggregationType",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            { "Count", Displays.Count(context: context) },
                            { "Total", Displays.Total(context: context) },
                            { "Average", Displays.Average(context: context) }
                        },
                        selectedValue: aggregation.Type.ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "AggregationTarget",
                        fieldCss: " togglable" + (aggregation.Type == Aggregation.Types.Count
                            ? " hidden"
                            : string.Empty),
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == ss.ReferenceType)
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(
                                o => o.ColumnName,
                                o => ss.GetColumn(
                                    context: context,
                                    columnName: o.ColumnName).LabelText),
                        selectedValue: aggregation.Target)
                    .Hidden(
                        controlId: "SelectedAggregation",
                        value: aggregation.Id.ToString())
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetAggregationDetails",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon button-positive",
                            onClick: "$p.setAggregationDetails($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "EditorSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed editor-columns",
                    legendText: Displays.EditorSettings(context: context),
                    action: () => hb
                        .EditorSettings(
                            context: context,
                            ss: ss))
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.OtherColumnsSettings(context: context),
                    action: () => hb
                        .EditorOtherColumn(
                            context: context,
                            ss: ss)
                        .Button(
                            controlId: "OpenEditorOtherColumnDialog",
                            text: Displays.AdvancedSetting(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.openEditorColumnDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "put"))
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.TabSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "Tabs",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h200",
                            controlCss: " always-send send-all",
                            listItemCollection: ss.TabSelectableOptions(
                                context: context,
                                habGeneral: true),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpTabs",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-n",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "MoveDownTabs",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-circle-triangle-s",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "NewTabDialog",
                                        text: Displays.New(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openTabDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "EditTabDialog",
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openTabDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "DeleteTabs",
                                        text: Displays.Delete(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-trash",
                                        action: "SetSiteSettings",
                                        method: "put"))))
                .FieldSet(id: "RelatingColumnsSettingsEditor",
                    css: " enclosed",
                    legendText: Displays.RelatingColumnSettings(context: context),
                    action: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "MoveUpRelatingColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveUp(context: context),
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-circle-triangle-n",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "MoveDownRelatingColumns",
                            controlCss: "button-icon",
                            text: Displays.MoveDown(context: context),
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-circle-triangle-s",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "NewRelatingColumn",
                            text: Displays.New(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.openRelatingColumnDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "put")
                        .Button(
                            controlId: "DeleteRelatingColumns",
                            text: Displays.Delete(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAndSend('#EditRelatingColumns', $(this));",
                            icon: "ui-icon-trash",
                            action: "SetSiteSettings",
                            method: "delete",
                            confirm: Displays.ConfirmDelete(context: context)))
                    .EditRelatingColumns(
                        context: context,
                        ss: ss))
                .FieldDropDown(
                    context: context,
                    controlId: "AutoVerUpType",
                    fieldCss: "field-auto-thin both",
                    labelText: Displays.AutoVerUpType(context: context),
                    optionCollection: new Dictionary<string, string>
                    {
                        {
                            Versions.AutoVerUpTypes.Default.ToInt().ToString(),
                            Displays.Default(context: context)
                        },
                        {
                            Versions.AutoVerUpTypes.Always.ToInt().ToString(),
                            Displays.Always(context: context)
                        },
                        {
                            Versions.AutoVerUpTypes.Disabled.ToInt().ToString(),
                            Displays.Disabled(context: context)
                        }
                    },
                    selectedValue: ss.AutoVerUpType.ToInt().ToString())
                .FieldDropDown(
                    context: context,
                    controlId: "AfterCreateActionType",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AfterCreateActionType(context: context),
                    optionCollection: new Dictionary<string, string>
                    {
                        {
                            Versions.AfterCreateActionTypes.ReturnToList.ToInt().ToString(),
                            Displays.ReturnToList(context: context)
                        },
                        {
                            Versions.AfterCreateActionTypes.OpenNewEditor.ToInt().ToString(),
                            Displays.OpenNewEditor(context: context)
                        },
                    },
                    insertBlank: true,
                    selectedValue: ss.AfterCreateActionType.ToInt().ToString()
                )
                .FieldDropDown(
                    context: context,
                    controlId: "AfterUpdateActionType",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AfterUpdateActionType(context: context),
                    optionCollection: new Dictionary<string, string>
                    {
                        {
                            Versions.AfterUpdateActionTypes.ReturnToList.ToInt().ToString(),
                            Displays.ReturnToList(context: context)
                        },
                        {
                            Versions.AfterUpdateActionTypes.MoveToNextRecord.ToInt().ToString(),
                            Displays.MoveToNextRecord(context: context)
                        },
                    },
                    insertBlank: true,
                    selectedValue: ss.AfterUpdateActionType.ToInt().ToString()
                )
                .FieldCheckBox(
                    controlId: "AllowEditingComments",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowEditingComments(context: context),
                    _checked: ss.AllowEditingComments == true)
                .FieldCheckBox(
                    controlId: "AllowCopy",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowCopy(context: context),
                    _checked: ss.AllowCopy == true)
                .FieldCheckBox(
                    controlId: "AllowReferenceCopy",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowReferenceCopy(context: context),
                    _checked: ss.AllowReferenceCopy == true)
                .FieldTextBox(
                    controlId: "CharToAddWhenCopying",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.CharToAddWhenCopying(context: context),
                    text: ss.CharToAddWhenCopying)
                .FieldCheckBox(
                    controlId: "AllowSeparate",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowSeparate(context: context),
                    _checked: ss.AllowSeparate == true,
                    _using: ss.ReferenceType == "Issues")
                .FieldCheckBox(
                    controlId: "AllowLockTable",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowLockTable(context: context),
                    _checked: ss.AllowLockTable == true)
                .FieldCheckBox(
                    controlId: "HideLink",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.HideLink(context: context),
                    _checked: ss.HideLink == true)
                .FieldCheckBox(
                    controlId: "SwitchRecordWithAjax",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.SwitchRecordWithAjax(context: context),
                    _checked: ss.SwitchRecordWithAjax == true)
                .FieldCheckBox(
                    controlId: "SwitchCommandButtonsAutoPostBack",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.SwitchCommandButtonsAutoPostBack(context: context),
                    _checked: ss.SwitchCommandButtonsAutoPostBack == true)
                .FieldCheckBox(
                    controlId: "DeleteImageWhenDeleting",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DeleteImageWhenDeleting(context: context),
                    _checked: ss.DeleteImageWhenDeleting == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettings(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return context.ThemeVersionOver2_0()
                ? hb.EditorSettingsThemeVersion2_0(
                    context: context,
                    ss: ss)
                : hb.EditorSettingsThemeVersion1_0(
                    context: context,
                    ss: ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsThemeVersion1_0(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return hb
                .FieldSelectable(
                    controlId: "EditorColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h250",
                    controlCss: " always-send send-all",
                    labelText: Displays.CurrentSettings(context: context),
                    listItemCollection: ss.EditorSelectableOptions(
                        context: context,
                        tabId: 0),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(
                            css: "command-center",
                            action: () => hb
                                .Hidden(
                                    controlId: "EditorColumnsNessesaryMessage",
                                    value: Messages.CanNotDisabled(
                                        context: context,
                                        data: "COLUMNNAME").Text)
                                .Hidden(
                                    controlId: "EditorColumnsNessesaryColumns",
                                    value: Jsons.ToJson(
                                        ss.GetEditorColumnNames()
                                        ?.Where(o => ss.EditorColumn(o)?.Required == true)
                                        .Select(o => o)))
                                .Hidden(
                                    controlId: "EditorColumnsTabsTarget",
                                    css: " always-send",
                                    value: "0")
                                .FieldDropDown(
                                    context: context,
                                    controlId: "EditorColumnsTabs",
                                    fieldCss: "w300",
                                    controlCss: " auto-postback always-send",
                                    optionCollection: ss.TabSelectableOptions(
                                        context: context,
                                        habGeneral: true),
                                    addSelectedValue: false,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "MoveUpEditorColumns",
                                    text: Displays.MoveUp(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.moveColumns(event, $(this),'Editor');",
                                    icon: "ui-icon-circle-triangle-n")
                                .Button(
                                    controlId: "MoveDownEditorColumns",
                                    text: Displays.MoveDown(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.moveColumns(event, $(this),'Editor');",
                                    icon: "ui-icon-circle-triangle-s")
                                .Button(
                                    controlId: "OpenEditorColumnDialog",
                                    text: Displays.AdvancedSetting(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.openEditorColumnDialog($(this));",
                                    icon: "ui-icon-gear",
                                    action: "SetSiteSettings",
                                    method: "put")
                                .Button(
                                    controlId: "ToDisableEditorColumns",
                                    controlCss: "button-icon",
                                    text: Displays.ToDisable(context: context),
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-circle-triangle-e",
                                    action: "SetSiteSettings",
                                    method: "post")))
                .FieldSelectable(
                    controlId: "EditorSourceColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h250",
                    labelText: Displays.OptionList(context: context),
                    listItemCollection: ss.EditorSelectableOptions(
                        context: context,
                        enabled: false),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(
                            css: "command-center",
                            action: () => hb
                                .FieldDropDown(
                                    context: context,
                                    controlId: "EditorSourceColumnsType",
                                    fieldCss: "w300",
                                    controlCss: " auto-postback always-send",
                                    optionCollection: new Dictionary<string, ControlData>
                                    {
                                        {
                                            "Columns",
                                            new ControlData(
                                                text: Displays.Column(context: context))
                                        },
                                        {
                                            "Links",
                                            new ControlData(
                                                text: Displays.Links(context: context))
                                        },
                                        {
                                            "Others",
                                            new ControlData(
                                                text: Displays.Others(context: context),
                                                attributes: new Dictionary<string, string>
                                                {
                                                    { "data-type", "multiple"}
                                                })
                                        }
                                    },
                                    addSelectedValue: false,
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    controlId: "ToEnableEditorColumns",
                                    text: Displays.ToEnable(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.enableColumns(event, $(this),'Editor', 'EditorSourceColumnsType');",
                                    icon: "ui-icon-circle-triangle-w",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .SearchOptionButton(
                                    setSearchOptionButton: true,
                                    searchOptionId: "OpenSearchEditorColumnDialog",
                                    searchOptionFunction: "$p.openSearchEditorColumnDialog($(this));")))
                .Hidden(
                    controlId: "SearchEditorColumnDialogInput",
                    css: "always-send",
                    action: "SetSiteSettings",
                    method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorSettingsThemeVersion2_0(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return hb
                .EditorColumnsLeft(
                    context: context,
                    ss: ss)
                .EditorColumnsToastMenu(
                    id: "editor-columns-toast-menu")
                .EditorColumnsSpacer(
                    context: context)
                .EditorColumnsRight(
                    context: context,
                    ss: ss)
                .EditorColumnsOther(
                    context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnsLeft(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return hb
                .FieldSelectable(
                    controlId: "EditorColumns",
                    fieldCss: "field-vertical left",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h250",
                    controlCss: " always-send send-all",
                    labelText: Displays.CurrentSettings(context: context),
                    listItemCollection: ss.EditorSelectableOptions(
                        context: context,
                        tabId: 0),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(
                            css: "editor-columns-header",
                            action: () => hb
                                .Div(
                                    css: "command-center",
                                    action: () => hb
                                        .Hidden(
                                            controlId: "EditorColumnsNessesaryMessage",
                                            value: Messages.CanNotDisabled(
                                                context: context,
                                                data: "COLUMNNAME").Text)
                                        .Hidden(
                                            controlId: "EditorColumnsNessesaryColumns",
                                            value: Jsons.ToJson(
                                                ss.GetEditorColumnNames()
                                                ?.Where(o => ss.EditorColumn(o)?.Required == true)
                                                .Select(o => o)))
                                        .Hidden(
                                            controlId: "EditorColumnsTabsTarget",
                                            css: " always-send",
                                            value: "0")
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "EditorColumnsTabs",
                                            fieldCss: "w300",
                                            controlCss: " auto-postback always-send",
                                            optionCollection: ss.TabSelectableOptions(
                                                context: context,
                                                habGeneral: true),
                                            addSelectedValue: false,
                                            action: "SetSiteSettings",
                                            method: "post",
                                            labelText: Displays.Tab(context: context)))
                                .EditorColumnsFiltersButton(
                                    context: context,
                                    isLeft: true)),
                    setMaterialSymbols: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnsRight(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return hb
                .FieldSelectable(
                    controlId: "EditorSourceColumns",
                    fieldCss: "field-vertical right",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h250",
                    labelText: Displays.OptionList(context: context),
                    listItemCollection: ss.EditorSelectableOptions(
                        context: context,
                        enabled: false),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(
                            css: "editor-columns-header",
                            action: () => hb
                                .Div(
                                    css: "command-center",
                                    action: () => hb
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "EditorSourceColumnsType",
                                            fieldCss: "w300",
                                            controlCss: " auto-postback always-send",
                                            optionCollection: new Dictionary<string, ControlData>
                                            {
                                                {
                                                    "Columns",
                                                    new ControlData(
                                                        text: Displays.Column(context: context))
                                                },
                                                {
                                                    "Links",
                                                    new ControlData(
                                                        text: Displays.Links(context: context))
                                                },
                                                {
                                                    "Others",
                                                    new ControlData(
                                                        text: Displays.Others(context: context),
                                                        attributes: new Dictionary<string, string>
                                                        {
                                                            { "data-type", "multiple"}
                                                        })
                                                }
                                            },
                                            addSelectedValue: false,
                                            action: "SetSiteSettings",
                                            method: "post",
                                            labelText: Displays.Option(context: context)))
                                .EditorColumnsFiltersButton(
                                    context: context,
                                    isLeft: false)),
                    setMaterialSymbols: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnsToastMenu(
            this HtmlBuilder hb,
            string id = null,
            bool setOpenEditorColumnDialogButton = true,
            bool seResetAndDisableEditorColumnsButton = true)
        {
            return hb
                .Div(
                    id: id,
                    css: "toast-menu",
                    action: () => hb
                        .Div(
                            css: "toast-menu-buttons",
                            action: () => hb
                                .MaterialIconButton(
                                    id: "MoveUpEditorColumns",
                                    iconName: "arrow_upward")
                                .MaterialIconButton(
                                    id: "MoveDownEditorColumns",
                                    iconName: "arrow_downward")
                                .MaterialIconButton(
                                    id: "OpenEditorColumnDialog",
                                    iconName: "settings",
                                    action: "SetSiteSettings",
                                    method: "put",
                                    _using: setOpenEditorColumnDialogButton)
                                .MaterialIconButton(
                                    id: "EditorColumnsResetAndDisable",
                                    iconName: "reset_settings",
                                    action: "SetSiteSettings",
                                    method: "put",
                                    _using: seResetAndDisableEditorColumnsButton)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnsSpacer(
            this HtmlBuilder hb,
            Context context,
            string editorColumnsSpacerId = null)
        {
            return hb
                .Div(
                    css: "editor-columns-spacer",
                    action: () => hb
                        .Div(
                            css: "editor-columns-spacer-buttons",
                            action: () => hb
                                .MaterialIconButton(
                                    id: "ToEnableEditorColumns",
                                    iconName: "arrow_back",
                                    action: "SetSiteSettings",
                                    method: "post")
                                .MaterialIconButton(
                                    id: "ToDisableEditorColumns",
                                    iconName: "arrow_forward",
                                    action: "SetSiteSettings",
                                    method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnsOther(
            this HtmlBuilder hb,
            Context context)
        {
            var showLinkText = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var utmSource = Parameters.General.HtmlUrlPrefix switch
            {
                "ee" => "pleasanter-ee",
                "com" => "pleasanter-ce",
                "demo" => "pleasanter-demo",
                _ => "pleasanter-cloud"
            };
            return hb
                .Div(
                    id: "DoNotHaveEnoughColumnsField",
                    css: "fieldset-inner-bottom is-right")
                .Hidden(
                    controlId: "SearchEditorColumnDialogInput",
                    css: "always-send",
                    action: "SetSiteSettings",
                    method: "post")
                .Hidden(
                    controlId: "ThemeVersionOver2",
                    value: context.Theme())
                .Hidden(
                    controlId: "ShowLinkText",
                    value: showLinkText.ToString())
                .Hidden(
                    controlId: "utmSource",
                    value: utmSource);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorOtherColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string selectedValue = null)
        {
            return hb.FieldDropDown(
                context: context,
                controlId: "EditorOtherColumn",
                fieldCss: "field-auto-thin",
                controlCss: " always-send",
                controlOnly: true,
                optionCollection: new Dictionary<string, ControlData>
                {
                    {
                        "Creator",
                        new ControlData(text: ss.GetColumn(
                            context: context,
                            columnName: "Creator")?.LabelText)
                    },
                    {
                        "Updator",
                        new ControlData(text: ss.GetColumn(
                            context: context,
                            columnName: "Updator")?.LabelText)
                    },
                    {
                        "CreatedTime",
                        new ControlData(text: ss.GetColumn(
                            context: context,
                            columnName: "CreatedTime")?.LabelText)
                    },
                    {
                        "UpdatedTime",
                        new ControlData(text: ss.GetColumn(
                            context: context,
                            columnName: "UpdatedTime")?.LabelText)
                    }
                },
                selectedValue: selectedValue,
                addSelectedValue: false,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialog(
            Context context, SiteSettings ss, Column column, IEnumerable<string> titleColumns)
        {
            var hb = new HtmlBuilder();
            if (column.TypeName == "nvarchar"
                && column.ControlType != "Attachments")
            {
                return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("EditorColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .Div(
                        id: "EditorDetailTabsContainer",
                        css: "tab-container",
                        action: () => hb.Ul(
                            id: "EditorDetailsettingTabs",
                            action: () => hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorColumnDialogTab",
                                        text: Displays.General(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#AutoNumberingSettingTab",
                                            text: Displays.AutoNumbering(context: context)),
                                    _using: column.AutoNumberingColumn())
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#EditorDetailsettingTab",
                                            text: Displays.ValidateInput(context: context)),
                                    _using: !column.OtherColumn())
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExtendedHtmlSettingTab",
                                            text: Displays.ExtendedHtml(context: context)),
                                    _using: !column.OtherColumn()
                                        && column.ColumnName != "Comments"))
                        .EditorColumnDialogTab(
                            context: context,
                            ss: ss,
                            column: column,
                            titleColumns: titleColumns)
                        .AutoNumberingSettingTab(
                            context: context,
                            ss: ss,
                            column: column)
                        .ExtendedHtmlSettingTab(
                            context: context,
                            ss: ss,
                            column: column)
                        .EditorDetailsettingTab(
                            context: context,
                            ss: ss,
                            column: column))
                    .Hidden(
                        controlId: "EditorColumnName",
                        css: "always-send",
                        value: column.ColumnName)
                    .Hidden(
                        controlId: "ResetEditorColumnData",
                        css: "always-send",
                        value: "0")
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetEditorColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "ResetEditorColumn",
                            text: Displays.Reset(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.resetEditorColumn($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post",
                            confirm: "ConfirmReset")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
            }
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("EditorColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .Div(
                        id: "EditorDetailTabsContainer",
                        css: "tab-container",
                        action: () => hb
                            .Ul(id: "EditorDetailsettingTabs", action: () => hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#EditorColumnDialogTab",
                                        text: Displays.General(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExtendedHtmlSettingTab",
                                            text: Displays.ExtendedHtml(context: context)),
                                    _using: !column.OtherColumn()))
                            .EditorColumnDialogTab(
                                context: context,
                                ss: ss,
                                column: column,
                                titleColumns: titleColumns)
                            .ExtendedHtmlSettingTab(
                                context: context,
                                ss: ss,
                                column: column))
                    .Hidden(
                        controlId: "EditorColumnName",
                        css: "always-send",
                        value: column.ColumnName)
                    .Hidden(
                        controlId: "EditorColumnName",
                        css: "always-send",
                        value: column.ColumnName)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "SetEditorColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "ResetEditorColumn",
                            text: Displays.Reset(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.resetEditorColumn($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "post",
                            confirm: "ConfirmReset")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder AutoNumberingSettingTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column)
        {
            if (!column.AutoNumberingColumn())
            {
                return hb;
            }
            return hb.TabsPanelField(
                id: "AutoNumberingSettingTab",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: column.LabelTextDefault,
                        action: () => hb
                            .FieldTextBox(
                                controlId: "AutoNumberingFormat",
                                fieldCss: "field-wide",
                                labelText: Displays.Format(context: context),
                                text: ss.ColumnNameToLabelText(column.AutoNumberingFormat))
                            .FieldDropDown(
                                context: context,
                                controlId: "AutoNumberingResetType",
                                labelText: Displays.ResetType(context: context),
                                optionCollection: new Dictionary<string, string>
                                {
                                    {
                                        Column.AutoNumberingResetTypes.Year.ToInt().ToString(),
                                        Displays.Year(context: context)
                                    },
                                    {
                                        Column.AutoNumberingResetTypes.Month.ToInt().ToString(),
                                        Displays.Month(context: context)
                                    },
                                    {
                                        Column.AutoNumberingResetTypes.Day.ToInt().ToString(),
                                        Displays.Day(context: context)
                                    },
                                    {
                                        Column.AutoNumberingResetTypes.String.ToInt().ToString(),
                                        Displays.String(context: context)
                                    }
                                },
                                selectedValue: column.AutoNumberingResetType.ToInt().ToString(),
                                insertBlank: true)
                            .FieldTextBox(
                                controlId: "AutoNumberingDefault",
                                labelText: Displays.DefaultInput(context: context),
                                text: column.AutoNumberingDefault.ToString(),
                                validateNumber: true,
                                validateMinNumber: 0,
                                validateMaxNumber: 999999999999999)
                            .FieldTextBox(
                                controlId: "AutoNumberingStep",
                                labelText: Displays.Step(context: context),
                                text: column.AutoNumberingStep.ToString(),
                                validateNumber: true,
                                validateMinNumber: 1,
                                validateMaxNumber: 999999999999999)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ExtendedHtmlSettingTab(
            this HtmlBuilder hb,
            Column column,
            Context context,
            SiteSettings ss)
        {
            if (column.OtherColumn() || column.ColumnName == "Comments")
            {
                return hb;
            }
            return hb.TabsPanelField(
                id: "ExtendedHtmlSettingTab",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: column.LabelTextDefault,
                        action: () => hb
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ExtendedHtmlBeforeField",
                                fieldCss: "field-wide",
                                labelText: Displays.ExtendedHtmlBeforeField(context: context),
                                text: column.ExtendedHtmlBeforeField)
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ExtendedHtmlBeforeLabel",
                                fieldCss: "field-wide",
                                labelText: Displays.ExtendedHtmlBeforeLabel(context: context),
                                text: column.ExtendedHtmlBeforeLabel)
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ExtendedHtmlBetweenLabelAndControl",
                                fieldCss: "field-wide",
                                labelText: Displays.ExtendedHtmlBetweenLabelAndControl(context: context),
                                text: column.ExtendedHtmlBetweenLabelAndControl)
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ExtendedHtmlAfterControl",
                                fieldCss: "field-wide",
                                labelText: Displays.ExtendedHtmlAfterControl(context: context),
                                text: column.ExtendedHtmlAfterControl)
                            .FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                controlId: "ExtendedHtmlAfterField",
                                fieldCss: "field-wide",
                                labelText: Displays.ExtendedHtmlAfterField(context: context),
                                text: column.ExtendedHtmlAfterField)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorDetailsettingTab(
            this HtmlBuilder hb, Column column, Context context, SiteSettings ss)
        {
            if (column.OtherColumn())
            {
                return hb;
            }
            return hb.TabsPanelField(
                id: "EditorDetailsettingTab",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: column.LabelTextDefault,
                        action: () =>
                        {
                            hb
                                .FieldTextBox(
                                    controlId: "ClientRegexValidation",
                                    fieldCss: "field-wide",
                                    labelText: Displays.ClientRegexValidation(context: context),
                                    text: column.ClientRegexValidation)
                                .FieldTextBox(
                                    controlId: "ServerRegexValidation",
                                    fieldCss: "field-wide",
                                    labelText: Displays.ServerRegexValidation(context: context),
                                    text: column.ServerRegexValidation)
                                .FieldTextBox(
                                    controlId: "RegexValidationMessage",
                                    fieldCss: "field-wide",
                                    labelText: Displays.RegexValidationMessage(context: context),
                                    text: column.RegexValidationMessage);
                        }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditorColumnDialogTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            IEnumerable<string> titleColumns)
        {
            var type = column.TypeName.CsTypeSummary();
            return hb.TabsPanelField(
                id: "EditorColumnDialogTab",
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: column.LabelTextDefault,
                        action: () =>
                        {
                            hb
                                .FieldTextBox(
                                    controlId: "LabelText",
                                    labelText: Displays.DisplayName(context: context),
                                    text: column.LabelText,
                                    validateRequired: true)
                                .FieldDropDown(
                                    context: context,
                                    controlId: "TextAlign",
                                    labelText: Displays.TextAlign(context: context),
                                    optionCollection: new Dictionary<string, string>
                                    {
                                        {
                                            SiteSettings.TextAlignTypes.Left.ToInt().ToString(),
                                            Displays.LeftAlignment(context: context)
                                        },
                                        {
                                            SiteSettings.TextAlignTypes.Right.ToInt().ToString(),
                                            Displays.RightAlignment(context: context)
                                        },
                                        {
                                            SiteSettings.TextAlignTypes.Center.ToInt().ToString(),
                                            Displays.CenterAlignment(context: context)
                                        },
                                    },
                                    selectedValue: column.TextAlign.ToInt().ToString());
                            if (column.OtherColumn())
                            {
                                switch (column.ControlType)
                                {
                                    case "ChoicesText":
                                        hb
                                            .FieldTextBox(
                                                textType: HtmlTypes.TextTypes.MultiLine,
                                                controlId: "ChoicesText",
                                                fieldCss: "field-wide",
                                                labelText: Displays.OptionList(context: context),
                                                text: column.ChoicesText)
                                            .FieldCheckBox(
                                                controlId: "UseSearch",
                                                labelText: Displays.UseSearch(context: context),
                                                _checked: column.UseSearch == true);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                if (column.TypeName == "nvarchar"
                                    && column.ControlType != "Attachments")
                                {
                                    hb
                                        .FieldSpinner(
                                            fieldId: "MaxChars",
                                            controlId: "MaxLength",
                                            controlCss: " allow-blank",
                                            labelText: Displays.MaxLength(context: context),
                                            value: column.MaxLength > 0
                                                ? column.MaxLength
                                                : null,
                                            min: 1,
                                            max: !Parameters
                                                .Validation
                                                .MaxLength
                                                .ToString()
                                                .IsNullOrEmpty()
                                                    ? Math.Min(
                                                        Parameters
                                                            .Validation
                                                            .MaxLength
                                                            .ToDecimal(),
                                                        column.ValidateMaxLength.ToDecimal())
                                                    : column.ValidateMaxLength.ToDecimal(),
                                            step: column.Step.ToInt(),
                                            width: 50);
                                }
                                if (column.ColumnName != "Comments")
                                {
                                    var optionCollection = FieldCssOptions(
                                        context: context,
                                        column: column);
                                    hb
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "FieldCss",
                                            labelText: Displays.Style(context: context),
                                            optionCollection: optionCollection,
                                            selectedValue: column.FieldCss,
                                            _using: optionCollection?.Any() == true
                                                && column.ControlType != "Attachments")
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "ViewerSwitchingType",
                                            labelText: Displays.ViewerSwitchingType(context: context),
                                            optionCollection: new Dictionary<string, string>()
                                            {
                                            {
                                                Column.ViewerSwitchingTypes.Auto.ToInt().ToString(),
                                                Displays.Auto(context: context)
                                            },
                                            {
                                                Column.ViewerSwitchingTypes.Manual.ToInt().ToString(),
                                                Displays.Manual(context: context)
                                            },
                                            {
                                                Column.ViewerSwitchingTypes.Disabled.ToInt().ToString(),
                                                Displays.Disabled(context: context)
                                            }
                                            },
                                            selectedValue: column.ViewerSwitchingType.ToInt().ToString(),
                                            _using: column.ControlType == "MarkDown")
                                        .FieldCheckBox(
                                            controlId: "ValidateRequired",
                                            labelText: Displays.Required(context: context),
                                            _checked: column.ValidateRequired ?? false,
                                            disabled: column.Required,
                                            _using: !column.Id_Ver
                                                && !column.NotUpdate)
                                        .FieldCheckBox(
                                            controlId: "AllowBulkUpdate",
                                            labelText: Displays.AllowBulkUpdate(context: context),
                                            _checked: column.AllowBulkUpdate == true,
                                            _using: !column.Id_Ver
                                                && !column.NotUpdate
                                                && column.ControlType != "Attachments");
                                }
                                switch (type)
                                {
                                    case Types.CsNumeric:
                                    case Types.CsDateTime:
                                    case Types.CsString:
                                        if (!column.Id_Ver
                                            && !column.NotUpdate
                                            && column.ControlType != "Attachments"
                                            && column.ColumnName != "Comments")
                                        {
                                            hb
                                                .FieldCheckBox(
                                                    controlId: "NoDuplication",
                                                    labelText: Displays.NoDuplication(context: context),
                                                    _checked: column.NoDuplication == true)
                                                .FieldTextBox(
                                                    fieldId: "MessageWhenDuplicatedField",
                                                    controlId: "MessageWhenDuplicated",
                                                    fieldCss: "field-wide"
                                                        + (column.NoDuplication != true
                                                            ? " hidden"
                                                            : string.Empty),
                                                    labelText: Displays.MessageWhenDuplicated(context: context),
                                                    text: column.MessageWhenDuplicated);
                                        }
                                        break;
                                }
                                if ((column.Required == false
                                    || column.TypeName == "datetime")
                                        && !column.NotUpdate)
                                {
                                    hb
                                        .FieldCheckBox(
                                            controlId: "CopyByDefault",
                                            labelText: Displays.CopyByDefault(context: context),
                                            _checked: column.CopyByDefault == true,
                                            _using: column.TypeCs != "Attachments")
                                        .FieldCheckBox(
                                            controlId: "EditorReadOnly",
                                            labelText: Displays.ReadOnly(context: context),
                                            _checked: column.EditorReadOnly == true);
                                }
                                if (column.TypeName == "datetime")
                                {
                                    hb
                                        .FieldDropDown(
                                            context: context,
                                            controlId: "EditorFormat",
                                            labelText: Displays.EditorFormat(context: context),
                                            optionCollection: DateTimeOptions(
                                                context: context,
                                                editorFormat: true),
                                            selectedValue: column.EditorFormat,
                                            _using: !column.NotUpdate);
                                }
                                switch (type)
                                {
                                    case Types.CsBool:
                                        hb.FieldCheckBox(
                                            controlId: "DefaultInput",
                                            labelText: Displays.DefaultInput(context: context),
                                            _checked: column.DefaultInput.ToBool(),
                                            _using: !column.NotUpdate);
                                        break;
                                    case Types.CsNumeric:
                                        hb.FieldCheckBox(
                                            controlId: "ImportKey",
                                            labelText: Displays.ImportKey(context: context),
                                            _checked: column.ImportKey == true,
                                            _using: column.ColumnName == Rds.IdColumn(tableName: ss.ReferenceType)
                                                || Def.ExtendedColumnTypes.Get(column.ColumnName) == "Num");
                                        if (column.ControlType == "ChoicesText")
                                        {
                                            hb.FieldTextBox(
                                                controlId: "DefaultInput",
                                                labelText: Displays.DefaultInput(context: context),
                                                text: column.DefaultInput,
                                                _using: !column.Id_Ver);
                                        }
                                        else
                                        {
                                            var maxDecimalPlaces = MaxDecimalPlaces(column);
                                            hb
                                                .FieldTextBox(
                                                    controlId: "DefaultInput",
                                                    labelText: Displays.DefaultInput(context: context),
                                                    text: !column.DefaultInput.IsNullOrEmpty()
                                                        ? column.DefaultInput.ToDecimal().ToString()
                                                        : string.Empty,
                                                    validateNumber: true,
                                                    _using: !column.Id_Ver
                                                        && !column.NotUpdate)
                                                .EditorColumnFormatProperties(
                                                    context: context,
                                                    column: column)
                                                .FieldCheckBox(
                                                    controlId: "Nullable",
                                                    labelText: Displays.Nullable(context: context),
                                                    _checked: column.Nullable.ToBool(),
                                                    _using: !column.Id_Ver
                                                        && !column.NotUpdate
                                                        && column.ColumnName != "WorkValue"
                                                        && column.ColumnName != "ProgressRate")
                                                .FieldTextBox(
                                                    controlId: "Unit",
                                                    controlCss: " w50",
                                                    labelText: Displays.Unit(context: context),
                                                    text: column.Unit,
                                                    _using: !column.Id_Ver)
                                                .FieldSpinner(
                                                    controlId: "DecimalPlaces",
                                                    labelText: Displays.DecimalPlaces(context: context),
                                                    value: column.DecimalPlaces.ToDecimal(),
                                                    min: 0,
                                                    max: maxDecimalPlaces,
                                                    step: 1,
                                                    _using: maxDecimalPlaces > 0
                                                        && !column.Id_Ver)
                                                .FieldDropDown(
                                                    context: context,
                                                    controlId: "RoundingType",
                                                    labelText: Displays.RoundingType(context: context),
                                                    optionCollection: new Dictionary<string, string>
                                                    {
                                                    {
                                                        SiteSettings.RoundingTypes.AwayFromZero.ToInt().ToString(),
                                                        Displays.AwayFromZero(context:context)
                                                    },
                                                    {
                                                        SiteSettings.RoundingTypes.Ceiling.ToInt().ToString(),
                                                        Displays.Ceiling(context:context)
                                                    },
                                                    {
                                                        SiteSettings.RoundingTypes.Truncate.ToInt().ToString(),
                                                        Displays.Truncate(context: context)
                                                    },
                                                    {
                                                        SiteSettings.RoundingTypes.Floor.ToInt().ToString(),
                                                        Displays.Floor(context:context)
                                                    },
                                                    {
                                                        SiteSettings.RoundingTypes.ToEven.ToInt().ToString(),
                                                        Displays.ToEven(context:context)
                                                    }
                                                    },
                                                    selectedValue: column.RoundingType.ToInt().ToString(),
                                                    _using: !column.Id_Ver);
                                            if (!column.NotUpdate && !column.Id_Ver)
                                            {
                                                var hidden = column.ControlType != "Spinner"
                                                    ? " hidden"
                                                    : string.Empty;
                                                hb
                                                    .FieldDropDown(
                                                        context: context,
                                                        controlId: "ControlType",
                                                        labelText: Displays.ControlType(context: context),
                                                        optionCollection: new Dictionary<string, string>
                                                        {
                                                            { "Normal", Displays.Normal(context: context) },
                                                            { "Spinner", Displays.Spinner(context: context) }
                                                        },
                                                        selectedValue: column.ControlType)
                                                    .FieldTextBox(
                                                        fieldId: "MinField",
                                                        controlId: "Min",
                                                        fieldCss: " both",
                                                        labelText: Displays.Min(context: context),
                                                        text: column.Min.ToString())
                                                    .FieldTextBox(
                                                        fieldId: "MaxField",
                                                        controlId: "Max",
                                                        fieldCss: "",
                                                        labelText: Displays.Max(context: context),
                                                        text: column.Max.ToString())
                                                    .FieldTextBox(
                                                        fieldId: "StepField",
                                                        controlId: "Step",
                                                        fieldCss: hidden,
                                                        labelText: Displays.Step(context: context),
                                                        text: column.Step.ToString());
                                            }
                                        }
                                        break;
                                    case Types.CsDateTime:
                                        hb
                                            .FieldSpinner(
                                                controlId: "DefaultInput",
                                                controlCss: " allow-blank",
                                                labelText: Displays.DefaultInput(context: context),
                                                value: column.DefaultInput != string.Empty
                                                    ? column.DefaultInput.ToDecimal()
                                                    : (decimal?)null,
                                                min: column.Min.ToInt(),
                                                max: column.Max.ToInt(),
                                                step: column.Step.ToInt(),
                                                width: column.Width,
                                                _using: !column.NotUpdate)
                                            .FieldDropDown(
                                                context: context,
                                                fieldId: "DateTimeStepField",
                                                fieldCss: column.EditorFormat == "Ymdhm" ? null : " hidden",
                                                controlId: "DateTimeStep",
                                                labelText: Displays.MinutesStep(context),
                                                optionCollection: new[] { 1, 2, 3, 5, 6, 10, 15, 20, 30, 60 }
                                                    .Select(v => v.ToString()).ToDictionary(v => v),
                                                selectedValue: column.DateTimeStep?.ToString());
                                        break;
                                    case Types.CsString:
                                        switch (column.ControlType)
                                        {
                                            case "Attachments":
                                                var provider = BinaryUtilities.BinaryStorageProvider(column: column);
                                                var hiddenLocalFolder = provider == "DataBase"
                                                    ? " hidden"
                                                    : string.Empty;
                                                var hiddenDataBase = provider == "LocalFolder"
                                                    ? " hidden"
                                                    : string.Empty;
                                                var hiddenLocalFolderTotalSize = provider != "LocalFolder"
                                                    ? " hidden"
                                                    : string.Empty;
                                                hb
                                                    .FieldCheckBox(
                                                        controlId: "AllowDeleteAttachments",
                                                        labelText: Displays.AllowDeleteAttachments(context: context),
                                                        _checked: column.AllowDeleteAttachments == true)
                                                    .FieldCheckBox(
                                                        controlId: "NotDeleteExistHistory",
                                                        labelText: Displays.NotDeleteExistHistory(context: context),
                                                        _checked: column.NotDeleteExistHistory == true)
                                                    .FieldDropDown(
                                                        context: context,
                                                        controlId: "BinaryStorageProvider",
                                                        labelText: Displays.BinaryStorageProvider(context),
                                                        optionCollection: new Dictionary<string, string>
                                                        {
                                                        { "DataBase", Displays.Database(context) },
                                                        { "LocalFolder", Displays.LocalFolder(context) },
                                                        { "AutoDataBaseOrLocalFolder", Displays.AutoDataBaseOrLocalFolder(context) }
                                                        },
                                                        selectedValue: column.BinaryStorageProvider,
                                                        _using: Parameters.BinaryStorage.UseStorageSelect)
                                                    .FieldCheckBox(
                                                        controlId: "OverwriteSameFileName",
                                                        labelText: Displays.OverwriteSameFileName(context: context),
                                                        _checked: column.OverwriteSameFileName == true)
                                                    .FieldSpinner(
                                                        controlId: "LimitQuantity",
                                                        labelText: Displays.LimitQuantity(context: context),
                                                        value: column.LimitQuantity,
                                                        min: Parameters.BinaryStorage.MinQuantity,
                                                        max: Parameters.BinaryStorage.MaxQuantity,
                                                        step: column.Step.ToInt(),
                                                        width: 50)
                                                    .FieldSpinner(
                                                        fieldId: "LimitSizeField",
                                                        fieldCss: hiddenDataBase,
                                                        controlId: "LimitSize",
                                                        labelText: Displays.LimitSize(context: context),
                                                        value: column.LimitSize,
                                                        min: Parameters.BinaryStorage.MinSize,
                                                        max: Parameters.BinaryStorage.MaxSize,
                                                        step: column.Step.ToInt(),
                                                        width: 50)
                                                    .FieldSpinner(
                                                        fieldId: "LimitTotalSizeField",
                                                        fieldCss: hiddenDataBase,
                                                        controlId: "LimitTotalSize",
                                                        labelText: Displays.LimitTotalSize(context: context),
                                                        value: column.TotalLimitSize,
                                                        min: Parameters.BinaryStorage.TotalMinSize,
                                                        max: Parameters.BinaryStorage.TotalMaxSize,
                                                        step: column.Step.ToInt(),
                                                        width: 50)
                                                    .FieldSpinner(
                                                        fieldId: "LocalFolderLimitSizeField",
                                                        fieldCss: hiddenLocalFolder,
                                                        controlId: "LocalFolderLimitSize",
                                                        labelText: Displays.LocalFolder(context) +
                                                        Displays.LimitSize(context),
                                                        labelCss: "field-label-multiline",
                                                        value: column.LocalFolderLimitSize,
                                                        min: Parameters.BinaryStorage.LocalFolderMinSize,
                                                        max: Parameters.BinaryStorage.LocalFolderMaxSize,
                                                        step: column.Step.ToInt(),
                                                        width: 50)
                                                    .FieldSpinner(
                                                        fieldId: "LocalFolderLimitTotalSizeField",
                                                        fieldCss: hiddenLocalFolderTotalSize,
                                                        controlId: "LocalFolderLimitTotalSize",
                                                        labelText: Displays.LocalFolder(context) +
                                                        Displays.LimitTotalSize(context),
                                                        labelCss: "field-label-multiline",
                                                        value: column.LocalFolderTotalLimitSize,
                                                        min: Parameters.BinaryStorage.LocalFolderTotalMinSize,
                                                        max: Parameters.BinaryStorage.LocalFolderTotalMaxSize,
                                                        step: column.Step.ToInt(),
                                                        width: 50);
                                                break;
                                            default:
                                                hb
                                                    .FieldCheckBox(
                                                        controlId: "ImportKey",
                                                        labelText: Displays.ImportKey(context: context),
                                                        _checked: column.ImportKey == true,
                                                        _using: column.ColumnName != "Comments"
                                                            && !column.NotUpdate)
                                                    .FieldCheckBox(
                                                        controlId: "AllowImage",
                                                        labelText: Displays.AllowImage(context: context),
                                                        _checked: column.AllowImage == true,
                                                        _using: context.ContractSettings.Images()
                                                            && (column.ControlType == "MarkDown"
                                                            || column.ColumnName == "Comments"))
                                                    .FieldSpinner(
                                                        controlId: "ThumbnailLimitSize",
                                                        labelText: Displays.ThumbnailLimitSize(context: context),
                                                        value: column.ThumbnailLimitSize == 0
                                                               ? null
                                                               : column.ThumbnailLimitSize,
                                                        min: Parameters.BinaryStorage.ThumbnailMinSize,
                                                        max: Parameters.BinaryStorage.ThumbnailMaxSize,
                                                        step: column.Step.ToInt(),
                                                        width: 50,
                                                        _using: column.Max == -1)
                                                    .FieldTextBox(
                                                        textType: column.ControlType == "MarkDown"
                                                            ? HtmlTypes.TextTypes.MultiLine
                                                            : HtmlTypes.TextTypes.Normal,
                                                        controlId: "DefaultInput",
                                                        fieldCss: "field-wide",
                                                        labelText: Displays.DefaultInput(context: context),
                                                        text: column.DefaultInput,
                                                        _using: column.ColumnName != "Comments"
                                                            && !column.NotUpdate);
                                                break;
                                        }
                                        break;
                                }
                                hb
                                    .FieldTextBox(
                                        controlId: "Description",
                                        fieldCss: "field-wide",
                                        labelText: Displays.Description(context: context),
                                        text: column.Description)
                                    .FieldTextBox(
                                        controlId: "InputGuide",
                                        fieldCss: "field-wide",
                                        labelText: Displays.InputGuide(context: context),
                                        text: column.InputGuide,
                                        _using: type == Types.CsString
                                            || type == Types.CsDateTime
                                            || Def.ExtendedColumnTypes.Get(column?.Name) == "Num");
                                switch (column.ControlType)
                                {
                                    case "ChoicesText":
                                        hb
                                            .FieldCodeEditor(
                                                context: context,
                                                controlId: "ChoicesText",
                                                fieldCss: "field-wide",
                                                controlCss: " o-low",
                                                dataLang: "json",
                                                labelText: Displays.OptionList(context: context),
                                                text: column.ChoicesText)
                                            .FieldDropDown(
                                                context: context,
                                                controlId: "ChoicesControlType",
                                                labelText: Displays.ControlType(context: context),
                                                optionCollection: new Dictionary<string, string>
                                                {
                                                    { "DropDown", Displays.DropDownList(context: context) },
                                                    { "Radio", Displays.RadioButton(context: context) }
                                                },
                                                selectedValue: column.ChoicesControlType)
                                            .FieldCheckBox(
                                                controlId: "UseSearch",
                                                labelText: Displays.UseSearch(context: context),
                                                _checked: column.UseSearch == true)
                                            .FieldCheckBox(
                                                controlId: "MultipleSelections",
                                                labelText: Displays.MultipleSelections(context: context),
                                                _checked: column.MultipleSelections == true,
                                                _using: column.TypeName == "nvarchar")
                                            .FieldCheckBox(
                                                controlId: "NotInsertBlankChoice",
                                                labelText: Displays.NotInsertBlankChoice(context: context),
                                                _checked: column.NotInsertBlankChoice == true)
                                            .FieldCheckBox(
                                                controlId: "Anchor",
                                                labelText: Displays.Anchor(context: context),
                                                _checked: column.Anchor == true,
                                                _using: column.TypeName == "nvarchar"
                                                    && !column.NotUpdate)
                                            .FieldCheckBox(
                                                fieldId: "OpenAnchorNewTabField",
                                                controlId: "OpenAnchorNewTab",
                                                fieldCss: "field-normal"
                                                    + (column.Anchor != true
                                                        ? " hidden"
                                                        : string.Empty),
                                                labelText: Displays.OpenAnchorNewTab(context: context),
                                                _checked: column.OpenAnchorNewTab == true,
                                                _using: column.TypeName == "nvarchar"
                                                    && !column.NotUpdate)
                                            .FieldTextBox(
                                                fieldId: "AnchorFormatField",
                                                controlId: "AnchorFormat",
                                                fieldCss: "field-wide"
                                                    + (column.Anchor != true
                                                        ? " hidden"
                                                        : string.Empty),
                                                labelText: Displays.AnchorFormat(context: context),
                                                text: column.AnchorFormat,
                                                _using: column.TypeName == "nvarchar"
                                                    && !column.NotUpdate);
                                        break;
                                    default:
                                        break;
                                }
                                if (column.ColumnName == "Title")
                                {
                                    hb.EditorColumnTitleProperties(
                                        context: context,
                                        ss: ss,
                                        titleColumns: titleColumns);
                                }
                                if (column.ColumnName != "Comments")
                                {
                                    hb
                                        .FieldCheckBox(
                                            controlId: "AutoPostBack",
                                            labelText: Displays.AutoPostBack(context: context),
                                            _checked: column.AutoPostBack == true,
                                            _using: !column.Id_Ver
                                                && !column.NotUpdate)
                                        .FieldTextBox(
                                            fieldId: "ColumnsReturnedWhenAutomaticPostbackField",
                                            controlId: "ColumnsReturnedWhenAutomaticPostback",
                                            fieldCss: "field-wide"
                                                + (column.AutoPostBack != true
                                                    ? " hidden"
                                                    : string.Empty),
                                            labelText: Displays.ColumnsReturnedWhenAutomaticPostback(context: context),
                                            text: column.ColumnsReturnedWhenAutomaticPostback)
                                        .FieldCheckBox(
                                            controlId: "NoWrap",
                                            labelText: Displays.NoWrap(context: context),
                                            _checked: column.NoWrap == true)
                                        .FieldCheckBox(
                                            controlId: "Hide",
                                            labelText: Displays.Hide(context: context),
                                            _checked: column.Hide == true)
                                        .FieldTextBox(
                                            controlId: "ExtendedFieldCss",
                                            fieldCss: "field-normal",
                                            labelText: Displays.ExtendedFieldCss(context: context),
                                            text: column.ExtendedFieldCss)
                                        .FieldTextBox(
                                            controlId: "ExtendedControlCss",
                                            fieldCss: "field-normal",
                                            labelText: Displays.ExtendedControlCss(context: context),
                                            text: column.ExtendedControlCss);
                                }
                            }
                            hb.FieldDropDown(
                                context: context,
                                controlId: "FullTextTypes",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.FullTextTypes(context: context),
                                optionCollection: ColumnUtilities.FullTextTypeOptions(context),
                                selectedValue: column.FullTextType?.ToInt().ToString());
                        }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> FieldCssOptions(Context context, Column column)
        {
            switch (column.ControlType)
            {
                case "MarkDown":
                    return new Dictionary<string, string>
                    {
                        { "field-normal", Displays.Normal(context: context) },
                        { "field-wide", Displays.Wide(context: context) },
                        { "field-markdown", Displays.MarkDown(context: context) },
                        { "field-rte", Displays.RichTextEditor(context: context) }
                    };
                case "Attachment":
                    return null;
                default:
                    return new Dictionary<string, string>
                    {
                        { "field-normal", Displays.Normal(context: context) },
                        { "field-wide", Displays.Wide(context: context) }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnFormatProperties(
            this HtmlBuilder hb, Context context, Column column)
        {
            var formats = Formats();
            var custom = !column.Format.IsNullOrEmpty() && !formats.Keys.Contains(column.Format);
            return hb
                .FieldDropDown(
                    context: context,
                    controlId: "FormatSelector",
                    controlCss: " not-send",
                    labelText: Displays.Format(context: context),
                    optionCollection: formats
                        .ToDictionary(o => o.Key, o => Displays.Get(
                            context: context,
                            id: o.Value)),
                    selectedValue: custom
                        ? "\t"
                        : column.Format,
                    _using: !column.Id_Ver)
                .FieldTextBox(
                    fieldId: "FormatField",
                    controlId: "Format",
                    fieldCss: custom ? string.Empty : " hidden",
                    labelText: Displays.Custom(context: context),
                    text: custom
                        ? column.Format
                        : string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorColumnTitleProperties(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<string> titleColumns)
        {
            return hb
                .FieldSelectable(
                    controlId: "TitleColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    controlCss: " always-send send-all",
                    labelText: Displays.CurrentSettings(context: context),
                    listItemCollection: ss
                        .TitleSelectableOptions(
                            context: context,
                            titleColumns: titleColumns),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "MoveUpTitleColumns",
                                text: Displays.MoveUp(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns(event, $(this),'Title');",
                                icon: "ui-icon-circle-triangle-n")
                            .Button(
                                controlId: "MoveDownTitleColumns",
                                text: Displays.MoveDown(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns(event, $(this),'Title');",
                                icon: "ui-icon-circle-triangle-s")
                            .Button(
                                controlId: "ToDisableTitleColumns",
                                text: Displays.ToDisable(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns(event, $(this),'Title');",
                                icon: "ui-icon-circle-triangle-e")
                            .Button(
                                controlCss: "button-icon button-positive",
                                text: Displays.Synchronize(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-refresh",
                                action: "SynchronizeTitles",
                                method: "put",
                                confirm: Displays.ConfirmSynchronize(context: context))))
                .FieldSelectable(
                    controlId: "TitleSourceColumns",
                    fieldCss: "field-vertical",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h200",
                    labelText: Displays.OptionList(context: context),
                    listItemCollection: ss
                        .TitleSelectableOptions(
                            context: context,
                            titleColumns: titleColumns,
                            enabled: false),
                    commandOptionPositionIsTop: true,
                    commandOptionAction: () => hb
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "ToEnableTitleColumns",
                                text: Displays.ToEnable(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.moveColumns(event, $(this),'Title');",
                                icon: "ui-icon-circle-triangle-w")))
                .FieldTextBox(
                    controlId: "TitleSeparator",
                    fieldCss: " both",
                    labelText: Displays.TitleSeparator(context: context),
                    text: ss.TitleSeparator);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> Formats()
        {
            return new Dictionary<string, string>()
            {
                { string.Empty, "Standard" },
                { "C", "Currency" },
                { "N", "DigitGrouping"},
                { "\t", "Custom" },
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DateTimeOptions(
            Context context, bool editorFormat = false)
        {
            return editorFormat
                ? DisplayAccessor.Displays.DisplayHash
                    .Where(o => new string[] { "Ymd", "Ymdhm", "Ymdhms" }.Contains(o.Key))
                    .ToDictionary(o => o.Key, o => Displays.Get(
                        context: context,
                        id: o.Key))
                : DisplayAccessor.Displays.DisplayHash
                    .Where(o => o.Value.Type == DisplayAccessor.Displays.Types.Date)
                    .ToDictionary(o => o.Key, o => Displays.Get(
                        context: context,
                        id: o.Key));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int MaxDecimalPlaces(Column column)
        {
            return column.Size.Split_2nd().ToInt();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection EditorColumnsResponses(
            this ResponseCollection res,
            Context context,
            SiteSettings ss)
        {
            var tabId = (ss.Tabs?.Get(context.Forms.Int("EditorColumnsTabs"))?.Id ?? 0)
                .ToString();
            return res
                .Val(
                    "#EditorColumnsTabsTarget",
                    tabId)
                .Html(
                    "#EditorColumnsTabs",
                    new HtmlBuilder().OptionCollection(
                        context: context,
                        optionCollection: ss.TabSelectableOptions(
                            context: context,
                            habGeneral: true),
                        selectedValue: tabId))
                .Html(
                    "#EditorColumns",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: ss.EditorSelectableOptions(
                            context: context,
                            tabId: tabId.ToInt()),
                        setMaterialSymbols: context.ThemeVersionOver2_0()))
                .EditorSourceColumnsResponses(
                    context: context,
                    ss: ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection EditorSourceColumnsResponses(
            this ResponseCollection res,
            Context context,
            SiteSettings ss)
        {
            var setMaterialSymbols = context.ThemeVersionOver2_0();
            switch (context.Forms.Data("EditorSourceColumnsType"))
            {
                case "Columns":
                    FilterSourceColumnsSelectable(
                        res: res,
                        context: context,
                        ss: ss,
                        setMaterialSymbols: setMaterialSymbols);
                    break;
                case "Links":
                    res.Html(
                        "#EditorSourceColumns",
                        new HtmlBuilder().SelectableItems(
                            listItemCollection: ss
                                .EditorLinksSelectableOptions(
                                    context: context,
                                    enabled: false),
                            setMaterialSymbols: setMaterialSymbols));
                    break;
                case "Others":
                    res.Html(
                        "#EditorSourceColumns",
                        new HtmlBuilder()
                            .SelectableItems(
                                listItemCollection: new Dictionary<string, ControlData>
                                {
                                    {
                                        "_Section-0",
                                        new ControlData(Displays.Section(context: context))
                                    },
                                },
                                setMaterialSymbols: setMaterialSymbols));
                    break;
            }
            return res.SetData("#EditorSourceColumns");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection FilterSourceColumnsSelectable(
            ResponseCollection res,
            Context context,
            SiteSettings ss,
            bool setMaterialSymbols = false)
        {
            var dialogInput = context.Forms.Data("SearchEditorColumnDialogInput")
                .Deserialize<Dictionary<string, string>>();
            return res.Html(
                "#EditorSourceColumns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: ss
                        .EditorSelectableOptions(
                            context: context,
                            enabled: false,
                            selection: dialogInput.Get("selection"),
                            keyWord: dialogInput.Get("keyWord")),
                    setMaterialSymbols: setMaterialSymbols));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Tab(
            Context context,
            SiteSettings ss,
            string controlId,
            Tab tab)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes().Id("TabForm").Action(Locations.ItemAction(
                    context: context,
                    id: ss.SiteId)),
                action: () => hb
                    .Hidden(
                        controlId: "EditorColumnsTabs",
                        css: " always-send",
                        value: context.Forms.Int("EditorColumnsTabs").ToString())
                    .FieldText(
                        controlId: "TabId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: (tab?.Id).ToInt().ToString())
                    .FieldTextBox(
                        controlId: "LabelText",
                        labelText: Displays.DisplayName(context: context),
                        text: tab?.LabelText,
                        validateRequired: tab?.Id != 0)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddTab",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewTabDialog")
                        .Button(
                            controlId: "UpdateTab",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditTabDialog")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection TabResponses(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected = null)
        {
            return res.Html(
                "#Tabs",
                new HtmlBuilder()
                    .SelectableItems(
                        listItemCollection: ss
                            .TabSelectableOptions(
                                context: context,
                                habGeneral: true),
                        selectedValueTextCollection: selected
                            ?.Select(o => o.ToString())))
                .SetData("#Tabs");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SectionDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            Section section)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes().Id("SectionForm").Action(Locations.ItemAction(
                    context: context,
                    id: ss.SiteId)),
                action: () => hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Section(context: context),
                    action: () => hb
                        .FieldText(
                            controlId: "SectionId",
                            controlCss: " always-send",
                            labelText: Displays.Id(context: context),
                            text: section.Id.ToString())
                        .FieldTextBox(
                            controlId: "LabelText",
                            controlCss: " always-send",
                            labelText: Displays.DisplayName(context: context),
                            text: section.LabelText)
                        .FieldCheckBox(
                            controlId: "AllowExpand",
                            labelText: Displays.AllowExpand(context: context),
                            _checked: section.AllowExpand == true)
                        .FieldDropDown(
                            context: context,
                            controlId: "Expand",
                            labelText: Displays.Expand(context: context),
                            optionCollection: new Dictionary<string, string>
                            {
                                {
                                    "1",
                                    Displays.Open(context:context)
                                },
                                {
                                    "0",
                                    Displays.Close(context: context)
                                }
                            },
                            selectedValue: section.Expand != false
                                ? "1"
                                : "0"))
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "UpdateSection",
                                text: Displays.Change(context: context),
                                controlCss: "button-icon validate button-positive",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder LinkDialog(
            Context context,
            SiteSettings ss,
            string controlId)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes().Id("LinkForm").Action(Locations.ItemAction(
                    context: context,
                    id: ss.SiteId)),
                action: () => hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Links(context: context))
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "UpdateLink",
                                text: Displays.Change(context: context),
                                controlCss: "button-icon validate button-positive",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "SetSiteSettings",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder LinksSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "LinksSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "LinkColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.LinkSelectableOptions(context: context),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpLinkColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'Link');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownLinkColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'Link');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableLinkColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'Link');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "LinkSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.LinkSelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableLinkColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'Link');",
                                        icon: "ui-icon-circle-triangle-w"))))
                .FieldSpinner(
                    controlId: "LinkPageSize",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.LinkPageSize(context: context),
                    value: ss.LinkPageSize.ToInt(),
                    min: Parameters.General.LinkPageSizeMin.ToDecimal(),
                    max: Parameters.General.LinkPageSizeMax.ToDecimal(),
                    step: 1,
                    width: 25)
                .FieldDropDown(
                    context: context,
                    controlId: "LinkTableView",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DefaultView(context: context),
                    optionCollection: ss.ViewSelectableOptions(),
                    selectedValue: ss.LinkTableView?.ToString(),
                    insertBlank: true,
                    _using: ss.Views?.Any() == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder HistoriesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "HistoriesSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "HistoryColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.HistorySelectableOptions(context: context),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpHistoryColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'History');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownHistoryColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'History');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableHistoryColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'History');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "HistorySourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.HistorySelectableOptions(
                                context: context, enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableHistoryColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'History');",
                                        icon: "ui-icon-circle-triangle-w"))))
                .FieldCheckBox(
                    controlId: "AllowRestoreHistories",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowRestoreHistories(context: context),
                    _checked: ss.AllowRestoreHistories == true)
                .FieldCheckBox(
                    controlId: "AllowPhysicalDeleteHistories",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowPhysicalDeleteHistories(context: context),
                    _checked: ss.AllowPhysicalDeleteHistories == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MoveSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "MoveSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.MoveTargetsSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "MoveTargetsColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: SiteInfo.Sites(context: context)
                                .Where(o => ss.MoveTargets?.Contains(o.Key) == true)
                                .OrderBy(o => ss.MoveTargets.IndexOf(o.Key.ToLong()))
                                .ToDictionary(
                                    o => o.Key.ToString(),
                                    o => new ControlData($"[{o.Key}] {o.Value.String("Title")}")),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpMoveTargetsColumns",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'MoveTargets');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownMoveTargetsColumns",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'MoveTargets');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableMoveTargetsColumns",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'MoveTargets');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "MoveTargetsSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            action: "MoveTargetsSourceColumns",
                            method: "Post",
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableMoveTargetsColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'MoveTargets');",
                                        icon: "ui-icon-circle-triangle-w")
                                    .TextBox(
                                        controlId: "MoveTargetsSourceColumnsText",
                                        controlCss: " always-send auto-postback w100",
                                        placeholder: Displays.Search(context: context),
                                        action: "MoveTargetsSourceColumns",
                                        method: "post")
                                    .Button(
                                        text: Displays.Search(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($('#MoveTargetsSourceColumnsText'));",
                                        icon: "ui-icon-search")
                                    .Hidden(
                                        controlId: "MoveTargetsSourceColumnsOffset",
                                        value: "0",
                                        css: "always-send")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "SummariesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownSummaries",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewSummary",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openSummaryDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "CopySummaries",
                        controlCss: "button-icon",
                        text: Displays.Copy(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteSummaries",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "SynchronizeSummaries",
                        controlCss: "button-icon button-positive",
                        text: Displays.Synchronize(context: context),
                        onClick: "$p.setAndSend('#EditSummary', $(this));",
                        icon: "ui-icon-refresh",
                        action: "SynchronizeSummaries",
                        method: "put",
                        confirm: Displays.ConfirmSynchronize(context: context)))
                .EditSummary(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditSummary(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditSummary").Deserialize<IEnumerable<int>>();
            return hb
                .GridTable(
                    context: context,
                    id: "EditSummary",
                    attributes: new HtmlAttributes()
                        .DataName("SummaryId")
                        .DataFunc("openSummaryDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .SummariesHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .SummariesBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(attributes: new HtmlAttributes()
                            .Rowspan(2),
                        action: () => hb
                            .CheckBox(
                                controlCss: "select-all",
                                _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                    selected?.Contains(o.Id) == true) == true))
                    .Th(attributes: new HtmlAttributes()
                            .Rowspan(2),
                        action: () => hb
                            .Text(text: Displays.Id(context: context)))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(3),
                        action: () => hb
                            .Text(text: Displays.DataStorageDestination(context: context)))
                    .Th(attributes: new HtmlAttributes()
                            .Colspan(4),
                        action: () => hb
                            .Text(text: ss.Title)))
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .Text(text: Displays.Sites(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryLinkColumn(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummaryType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SummarySourceColumn(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SummariesBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            if (ss.Summaries?.Any() == true)
            {
                var dataRows = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn()
                            .SiteId()
                            .ReferenceType()
                            .Title()
                            .SiteSettings(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .SiteId_In(ss.Summaries?
                                .Select(o => o.SiteId)))).AsEnumerable();
                hb.TBody(action: () =>
                {
                    ss.Summaries?.ForEach(summary =>
                    {
                        var dataRow = dataRows.FirstOrDefault(o =>
                            o["SiteId"].ToLong() == summary.SiteId);
                        var destinationSs = SiteSettingsUtilities.Get(
                            context: context, dataRow: dataRow);
                        hb.Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(summary.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?.Contains(summary.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: summary.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: dataRow?["Title"].ToString()))
                                .Td(action: () => hb
                                    .Text(text: destinationSs?.GetColumn(
                                        context: context,
                                        columnName: summary.DestinationColumn)?.LabelText))
                                .Td(action: () => hb
                                    .Text(text: destinationSs?.Views?.Get(
                                        summary.DestinationCondition)?.Name))
                                .Td(action: () => hb
                                    .Text(text: ss.GetColumn(
                                        context: context,
                                        columnName: summary.LinkColumn)?.LabelText))
                                .Td(action: () => hb
                                    .Text(text: SummaryType(
                                        context: context,
                                        type: summary.Type)))
                                .Td(action: () => hb
                                    .Text(text: ss.GetColumn(
                                        context: context,
                                        columnName: summary.SourceColumn)?.LabelText))
                                .Td(action: () => hb
                                    .Text(text: ss.Views?.Get(
                                        summary.SourceCondition)?.Name)));
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDialog(
            Context context, SiteSettings ss, string controlId, Summary summary)
        {
            var hb = new HtmlBuilder();
            var destinationSiteHash = ss.Destinations
                ?.Values
                .ToDictionary(o => o.SiteId.ToString(), o => o.Title);
            var destinationSs = ss.Destinations?.Get(summary.SiteId);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("SummaryForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "SummaryId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: summary.Id.ToString(),
                        _using: controlId == "EditSummary")
                    .FieldSet(
                        css: "fieldset enclosed-half h250 both",
                        legendText: Displays.DataStorageDestination(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "SummarySiteId",
                                controlCss: " auto-postback always-send",
                                labelText: Displays.Sites(context: context),
                                optionCollection: destinationSiteHash,
                                selectedValue: summary.SiteId.ToString(),
                                action: "SetSiteSettings",
                                method: "post")
                            .SummaryDestinationColumn(
                                context: context,
                                destinationSs: destinationSs,
                                destinationColumn: summary.DestinationColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummaryDestinationCondition",
                                controlCss: " always-send",
                                labelText: Displays.Condition(context: context),
                                optionCollection: destinationSs?.ViewSelectableOptions(),
                                selectedValue: summary.DestinationCondition.ToString(),
                                insertBlank: true,
                                _using: destinationSs?.Views?.Any() == true)
                            .FieldCheckBox(
                                fieldId: "SummarySetZeroWhenOutOfConditionField",
                                controlId: "SummarySetZeroWhenOutOfCondition",
                                fieldCss: "field-auto-thin right" +
                                    (destinationSs?.Views?.Any(o =>
                                        o.Id == summary.DestinationCondition) == true
                                            ? null
                                            : " hidden"),
                                controlCss: " always-send",
                                labelText: Displays.SetZeroWhenOutOfCondition(context: context),
                                _checked: summary.SetZeroWhenOutOfCondition == true))
                    .FieldSet(
                        css: "fieldset enclosed-half h250",
                        legendText: ss.Title,
                        action: () => hb
                            .SummaryLinkColumn(
                                context: context,
                                ss: ss,
                                siteId: summary.SiteId,
                                linkColumn: summary.LinkColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummaryType",
                                controlCss: " auto-postback always-send",
                                labelText: Displays.SummaryType(context: context),
                                optionCollection: SummaryTypeCollection(context: context),
                                selectedValue: summary.Type,
                                action: "SetSiteSettings",
                                method: "post")
                            .SummarySourceColumn(
                                context: context,
                                ss: ss,
                                type: summary.Type,
                                sourceColumn: summary.SourceColumn)
                            .FieldDropDown(
                                context: context,
                                controlId: "SummarySourceCondition",
                                controlCss: " always-send",
                                labelText: Displays.Condition(context: context),
                                optionCollection: ss.ViewSelectableOptions(),
                                selectedValue: summary.SourceCondition.ToString(),
                                insertBlank: true,
                                _using: ss.Views?.Any() == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddSummary",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewSummary")
                        .Button(
                            controlId: "UpdateSummary",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setSummary($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditSummary")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryDestinationColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings destinationSs,
            string destinationColumn = null)
        {
            return hb.FieldDropDown(
                context: context,
                fieldId: "SummaryDestinationColumnField",
                controlId: "SummaryDestinationColumn",
                controlCss: " always-send",
                labelText: Displays.Column(context: context),
                optionCollection: destinationSs?.Columns?
                    .Where(o => o.Computable)
                    .Where(o => o.TypeName != "datetime")
                    .Where(o => !o.NotUpdate)
                    .OrderBy(o => o.No)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => o.LabelText),
                selectedValue: destinationColumn,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> SummaryTypeCollection(Context context)
        {
            return new Dictionary<string, string>
            {
                { "Count", Displays.Count(context: context) },
                { "Total", Displays.Total(context: context) },
                { "Average", Displays.Average(context: context) },
                { "Min", Displays.Min(context: context) },
                { "Max", Displays.Max(context: context) }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummaryLinkColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string linkColumn = null)
        {
            return hb.FieldDropDown(
                context: context,
                fieldId: "SummaryLinkColumnField",
                controlId: "SummaryLinkColumn",
                controlCss: " always-send",
                labelText: Displays.SummaryLinkColumn(context: context),
                optionCollection: ss.Links
                    ?.Where(o => o.SiteId > 0)
                    .Where(o => o.SiteId == siteId)
                    .ToDictionary(
                        o => o.ColumnName,
                        o => ss.GetColumn(
                            context: context,
                            columnName: o.ColumnName).LabelText),
                selectedValue: linkColumn,
                action: "SetSiteSettings",
                method: "post");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SummarySourceColumn(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string type = "Count",
            string sourceColumn = null)
        {
            switch (type)
            {
                case "Total":
                case "Average":
                case "Max":
                case "Min":
                    return hb.FieldDropDown(
                        context: context,
                        fieldId: "SummarySourceColumnField",
                        controlId: "SummarySourceColumn",
                        controlCss: " always-send",
                        labelText: Displays.SummarySourceColumn(context: context),
                        optionCollection: ss.Columns
                            .Where(o => o.Computable)
                            .Where(o => o.TypeName != "datetime")
                            .ToDictionary(o => o.ColumnName, o => o.LabelText),
                        selectedValue: sourceColumn,
                        action: "SetSiteSettings",
                        method: "post");
                default:
                    return hb.FieldContainer(
                        fieldId: "SummarySourceColumnField",
                        fieldCss: " hidden");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string SummaryType(Context context, string type)
        {
            switch (type)
            {
                case "Count": return Displays.Count(context: context);
                case "Total": return Displays.Total(context: context);
                case "Average": return Displays.Average(context: context);
                case "Min": return Displays.Min(context: context);
                case "Max": return Displays.Max(context: context);
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "FormulasSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpFormulas",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownFormulas",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewFormula",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openFormulaDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "CopyFormulas",
                        controlCss: "button-icon",
                        text: Displays.Copy(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteFormulas",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "SynchronizeFormulas",
                        controlCss: "button-icon button-positive",
                        text: Displays.Synchronize(context: context),
                        onClick: "$p.setAndSend('#EditFormula', $(this));",
                        icon: "ui-icon-refresh",
                        action: "SynchronizeFormulas",
                        method: "put",
                        confirm: Displays.ConfirmSynchronize(context: context)))
                .EditFormula(context: context, ss: ss)
                .FieldCheckBox(
                        controlId: "OutputFormulaLogs",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.OutputLog(context: context),
                        _checked: ss.OutputFormulaLogs == true,
                        labelPositionIsRight: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditFormula(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditFormula").Deserialize<IEnumerable<int>>();
            return hb
                .GridTable(
                    context: context,
                    id: "EditFormula",
                    attributes: new HtmlAttributes()
                        .DataName("FormulaId")
                        .DataFunc("openFormulaDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .FormulasHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .FormulasBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Formulas?.Any() == true && ss.Formulas?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                            .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.CalculationMethod(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Target(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Formulas(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.Condition(context: context)))
                    .Th(action: () => hb
                            .Text(text: Displays.OutOfCondition(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FormulasBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            if (ss.Formulas?.Any() == true)
            {
                hb.TBody(action: () =>
                {
                    var columnList = ss.FormulaColumnList();
                    ss.Formulas?.ForEach(formulaSet =>
                    {
                        if (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Extended.ToString())
                        {
                            if (formulaSet.NotUseDisplayName == true)
                            {
                                foreach (var column in columnList)
                                {
                                    formulaSet.FormulaScript = System.Text.RegularExpressions.Regex.Replace(
                                        input: formulaSet.FormulaScript,
                                        pattern: "(?<!\\$)"
                                            + System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                                            + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                                        replacement: $"[{column.ColumnName}]");
                                }
                                if (formulaSet.FormulaScriptOutOfCondition.IsNullOrEmpty() == false)
                                {
                                    foreach (var column in columnList)
                                    {
                                        formulaSet.FormulaScriptOutOfCondition = System.Text.RegularExpressions.Regex.Replace(
                                            input: formulaSet.FormulaScriptOutOfCondition,
                                            pattern: "(?<!\\$)"
                                                + System.Text.RegularExpressions.Regex.Escape(column.LabelText)
                                                + $"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                                            replacement: $"[{column.ColumnName}]");
                                    }
                                }
                            }
                            else
                            {
                                var columns1 = System.Text.RegularExpressions.Regex.Matches(formulaSet.FormulaScript, @"\[([^]]*)\]");
                                foreach (var column in columns1)
                                {
                                    var columnParam = column.ToString()[1..^1];
                                    if (ss.FormulaColumn(columnParam, formulaSet.CalculationMethod) != null)
                                    {
                                        formulaSet.FormulaScript = formulaSet.FormulaScript.Replace(
                                            oldValue: column.ToString(),
                                            newValue: columnList.SingleOrDefault(o => o.ColumnName == columnParam).LabelText);
                                    }
                                }
                                if (formulaSet.FormulaScriptOutOfCondition.IsNullOrEmpty() == false)
                                {
                                    var columns2 = System.Text.RegularExpressions.Regex.Matches(formulaSet.FormulaScriptOutOfCondition, @"\[([^]]*)\]");
                                    foreach (var column in columns2)
                                    {
                                        var columnParam = column.ToString()[1..^1];
                                        if (ss.FormulaColumn(columnParam, formulaSet.CalculationMethod) != null)
                                        {
                                            formulaSet.FormulaScriptOutOfCondition = formulaSet.FormulaScriptOutOfCondition.Replace(
                                                oldValue: column.ToString(),
                                                newValue: columnList.SingleOrDefault(o => o.ColumnName == columnParam).LabelText);
                                        }
                                    }
                                }
                            }
                            formulaSet = FormulaBuilder.UpdateColumnDisplayText(
                                ss: ss,
                                formulaSet: formulaSet);
                        }
                        hb.Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(formulaSet.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?.Contains(formulaSet.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: formulaSet.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: Displays.Get(
                                        context: context,
                                        id: formulaSet.CalculationMethod)))
                                .Td(action: () => hb
                                    .Text(text: ss.GetColumn(
                                        context: context,
                                        columnName: formulaSet.Target)?.LabelText))
                                .Td(action: () => hb
                                    .Text(text: (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                                        || string.IsNullOrEmpty(formulaSet.CalculationMethod))
                                            ? formulaSet.Formula?.ToString(ss: ss, notUseDisplayName: formulaSet.NotUseDisplayName)
                                            : formulaSet.FormulaScript))
                                .Td(action: () => hb
                                    .Text(text: ss.Views?.Get(formulaSet.Condition)?.Name))
                                .Td(action: () => hb
                                    .Text(text: (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                                        || string.IsNullOrEmpty(formulaSet.CalculationMethod))
                                            ? formulaSet.OutOfCondition?.ToString(ss)
                                            : formulaSet.FormulaScriptOutOfCondition)));
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FormulaDialog(
            Context context, SiteSettings ss, string controlId, FormulaSet formulaSet)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("FormulaForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "FormulaId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: formulaSet.Id.ToString(),
                        _using: controlId == "EditFormula")
                    .FieldDropDown(
                        context: context,
                        controlId: "FormulaCalculationMethod",
                        controlCss: " always-send",
                        labelText: Displays.CalculationMethod(context: context),
                        optionCollection: ss.FormulaCalculationMethodSelectableOptions(context: context),
                        selectedValue: formulaSet.CalculationMethod,
                        onChange: "$p.changeCalculationMethod($(this));")
                    .FieldDropDown(
                        context: context,
                        controlId: "FormulaTarget",
                        controlCss: " always-send",
                        labelText: Displays.Target(context: context),
                        optionCollection: ss.FormulaTargetSelectableOptions(formulaSet.CalculationMethod),
                        selectedValue: formulaSet.Target?.ToString())
                    .FieldTextBox(
                        controlId: "Formula",
                        controlCss: " always-send",
                        fieldCss: "field-wide",
                        labelText: Displays.Formulas(context: context),
                        text: (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                            || string.IsNullOrEmpty(formulaSet.CalculationMethod))
                                ? formulaSet.Formula?.ToString(ss, notUseDisplayName: formulaSet.NotUseDisplayName)
                                : FormulaBuilder.UpdateColumnDisplayText(
                                    ss: ss,
                                    formulaSet: formulaSet).FormulaScript,
                        validateRequired: true)
                    .FieldCheckBox(
                        controlId: "NotUseDisplayName",
                        controlCss: " always-send",
                        labelText: Displays.NotUseDisplayName(context: context),
                        _checked: formulaSet.NotUseDisplayName == true)
                    .FieldCheckBox(
                        controlId: "IsDisplayError",
                        controlCss: " always-send",
                        labelText: Displays.FormulaIsDisplayError(context: context),
                        _checked: formulaSet.IsDisplayError == true,
                        fieldCss: (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                            || formulaSet.CalculationMethod == null)
                                ? " hidden formula-display-error-check"
                                : " formula-display-error-check")
                    .FieldDropDown(
                        context: context,
                        controlId: "FormulaCondition",
                        controlCss: " always-send",
                        labelText: Displays.Condition(context: context),
                        optionCollection: ss.ViewSelectableOptions(),
                        selectedValue: formulaSet.Condition?.ToString(),
                        insertBlank: true,
                        _using: ss.Views?.Any() == true)
                    .FieldTextBox(
                        fieldId: "FormulaOutOfConditionField",
                        controlId: "FormulaOutOfCondition",
                        controlCss: " always-send",
                        fieldCss: "field-wide" + (ss.Views?
                            .Any(o => o.Id == formulaSet.Condition) == true
                                ? string.Empty
                                : " hidden"),
                        labelText: Displays.OutOfCondition(context: context),
                        text: (formulaSet.CalculationMethod == FormulaSet.CalculationMethods.Default.ToString()
                            || string.IsNullOrEmpty(formulaSet.CalculationMethod))
                                ? formulaSet.OutOfCondition?.ToString(ss)
                                : FormulaBuilder.UpdateColumnDisplayText(
                                    ss: ss,
                                    formulaSet: formulaSet).FormulaScriptOutOfCondition)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddFormula",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewFormula")
                        .Button(
                            controlId: "UpdateFormula",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditFormula")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "ProcessesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpProcesses",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditProcess', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownProcesses",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditProcess', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewProcess",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openProcessDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "CopyProcesses",
                        controlCss: "button-icon",
                        text: Displays.Copy(context: context),
                        onClick: "$p.setAndSend('#EditProcess', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteProcesses",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditProcess', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditProcess(
                    context: context,
                    ss: ss)
                .FieldCheckBox(
                    controlId: "ProcessOutputFormulaLogs",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.OutputLog(context: context),
                    _checked: ss.ProcessOutputFormulaLogs == true,
                    labelPositionIsRight: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcess(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditProcess").Deserialize<IEnumerable<int>>();
            return hb
                .GridTable(
                    context: context,
                    id: "EditProcess",
                    attributes: new HtmlAttributes()
                        .DataName("ProcessId")
                        .DataFunc("openProcessDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .ProcessesHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .ProcessesBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessesHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Processes?.Any() == true && ss.Processes?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Name(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.DisplayName(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Description(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Tooltip(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ScreenType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.CurrentStatus(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ChangedStatus(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ConfirmationMessage(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SuccessMessage(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.OnClick(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ExecutionTypes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ActionTypes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterProcessStatusChangeActionType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AllowBulkProcessing(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessesBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            if (ss.Processes?.Any() == true)
            {
                var statusColumn = ss.GetColumn(
                    context: context,
                    columnName: "Status");
                hb.TBody(action: () =>
                {
                    ss.Processes?.ForEach(process =>
                    {
                        hb.Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(process.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?.Contains(process.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: process.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: process.Name))
                                .Td(action: () => hb
                                    .Text(text: process.DisplayName))
                                .Td(action: () => hb
                                    .Text(text: process.Description))
                                .Td(action: () => hb
                                    .Text(text: process.Tooltip))
                                .Td(action: () => hb
                                    .Text(text: Displays.Get(
                                        context: context,
                                        id: process.ScreenType?.ToString()
                                            ?? Process.ScreenTypes.Edit.ToString())))
                                .Td(action: () => hb
                                    .Text(text: process.CurrentStatus != -1
                                        ? statusColumn?.ChoiceHash?.Get(process.CurrentStatus.ToString())?.Text
                                        : "*"))
                                .Td(action: () => hb
                                    .Text(text: process.ChangedStatus != -1
                                        ? statusColumn?.ChoiceHash?.Get(process.ChangedStatus.ToString())?.Text
                                        : "*"))
                                .Td(action: () => hb
                                    .Text(text: process.ConfirmationMessage))
                                .Td(action: () => hb
                                    .Text(text: ss.ColumnNameToLabelText(process.SuccessMessage)))
                                .Td(action: () => hb
                                    .Text(text: process.OnClick))
                                .Td(action: () => hb
                                    .Text(text: Displays.Get(
                                        context: context,
                                        id: process.ExecutionType?.ToString()
                                            ?? Process.ExecutionTypes.AddedButton.ToString())))
                                .Td(action: () => hb
                                    .Text(text: Displays.Get(
                                        context: context,
                                        id: process.ActionType?.ToString()
                                            ?? Process.ActionTypes.Save.ToString())))
                                .Td(action: () => hb
                                    .Text(text: process.AfterProcessStatusChangeActionType != null && process.AfterProcessStatusChangeActionType != Process.AfterProcessStatusChangeActionTypes.Default ?
                                        Displays.Get(
                                            context: context,
                                            id: process.AfterProcessStatusChangeActionType?.ToString()) : ""))
                                .Td(action: () => hb
                                    .Span(
                                        css: "ui-icon ui-icon-circle-check",
                                        _using: process.AllowBulkProcessing == true)));
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ProcessDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            Process process)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ProcessForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ProcessId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: process.Id.ToString(),
                        _using: controlId == "EditProcess")
                    .FieldTextBox(
                        controlId: "ProcessName",
                        controlCss: " always-send",
                        labelText: Displays.Name(context: context),
                        text: process.Name,
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ProcessDisplayName",
                        controlCss: " always-send",
                        labelText: Displays.DisplayName(context: context),
                        text: process.DisplayName)
                    .Div(
                        id: "ProcessTabsContainer",
                        css: "tab-container",
                        action: () => hb.Ul(
                            id: "ProcessTabs",
                            action: () => hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessGeneralTab",
                                        text: Displays.General(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessValidateInputsTab",
                                        text: Displays.ValidateInput(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessViewFiltersTab",
                                        text: Displays.Condition(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessAccessControlsTab",
                                        text: Displays.AccessControls(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessDataChangesTab",
                                        text: Displays.DataChanges(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ProcessAutoNumberingTab",
                                        text: Displays.AutoNumbering(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ProcessNotificationsTab",
                                            text: Displays.Notifications(context: context)),
                                    _using: context.ContractSettings.Notice != false))
                        .ProcessGeneralTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessValidateInputsTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessViewFiltersTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessAccessControlsTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessDataChangesTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessAutoNumberingTab(
                            context: context,
                            ss: ss,
                            process: process)
                        .ProcessNotificationsTab(
                            context: context,
                            ss: ss,
                            process: process))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddProcess",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewProcess")
                        .Button(
                            controlId: "UpdateProcess",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditProcess")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessGeneralTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            var status = ss.GetColumn(
                context: context,
                columnName: "Status");
            var optionCollection = $"-1,*\n{status.ChoicesText}".SplitReturn()
                .Select(o => new Choice(o))
                .GroupBy(o => o.Value)
                .ToDictionary(
                    o => o.Key,
                    o => new ControlData(
                        text: o.First().Text,
                        css: o.First().CssClass));
            return hb.TabsPanelField(id: "ProcessGeneralTab", action: () => hb
                .Div(css: "items", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessScreenType",
                        controlCss: " always-send",
                        labelText: Displays.ScreenType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Process.ScreenTypes.New.ToInt().ToString(),
                                Displays.New(context: context)
                            },
                            {
                                Process.ScreenTypes.Edit.ToInt().ToString(),
                                Displays.Edit(context: context)
                            }
                        },
                        selectedValue: process.ScreenType?.ToInt().ToString()
                            ?? Process.ScreenTypes.Edit.ToInt().ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessCurrentStatus",
                        controlCss: " always-send",
                        labelText: Displays.CurrentStatus(context: context),
                        optionCollection: optionCollection,
                        selectedValue: process.CurrentStatus.ToString(),
                        insertBlank: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessChangedStatus",
                        controlCss: " always-send",
                        labelText: Displays.ChangedStatus(context: context),
                        optionCollection: optionCollection,
                        selectedValue: ss.ColumnNameToLabelText(process.ChangedStatus.ToString()),
                        insertBlank: true)
                    .FieldTextBox(
                        controlId: "ProcessDescription",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Description(context: context),
                        text: process.Description)
                    .FieldTextBox(
                        controlId: "ProcessTooltip",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Tooltip(context: context),
                        text: process.Tooltip)
                    .FieldTextBox(
                        controlId: "ProcessIcon",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Icon(context: context),
                        text: process.Icon,
                        validateRegex: @"^[a-z\d-_]+$",
                        validateRegexErrorMessage: Displays.ValidationError(context: context))
                    .FieldTextBox(
                        controlId: "ProcessConfirmationMessage",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.ConfirmationMessage(context: context),
                        text: process.ConfirmationMessage)
                    .FieldTextBox(
                        controlId: "ProcessSuccessMessage",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.SuccessMessage(context: context),
                        text: ss.ColumnNameToLabelText(process.SuccessMessage))
                    .FieldTextBox(
                        controlId: "ProcessOnClick",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.OnClick(context: context),
                        text: process.OnClick)
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessExecutionType",
                        controlCss: " always-send",
                        labelText: Displays.ExecutionTypes(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Process.ExecutionTypes.AddedButton.ToInt().ToString(),
                                Displays.AddedButton(context: context)
                            },
                            {
                                Process.ExecutionTypes.CreateOrUpdate.ToInt().ToString(),
                                Displays.CreateOrUpdate(context: context)
                            },
                            {
                                Process.ExecutionTypes.AddedButtonOrCreateOrUpdate.ToInt().ToString(),
                                Displays.AddedButtonOrCreateOrUpdate(context: context)
                            }
                        },
                        selectedValue: process.ExecutionType.ToInt().ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessActionType",
                        controlCss: " always-send",
                        labelText: Displays.ActionTypes(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Process.ActionTypes.Save.ToInt().ToString(),
                                Displays.Save(context: context)
                            },
                            {
                                Process.ActionTypes.PostBack.ToInt().ToString(),
                                Displays.PostBack(context: context)
                            },
                            {
                                Process.ActionTypes.None.ToInt().ToString(),
                                Displays.None(context: context)
                            }
                        },
                        selectedValue: process.ActionType.ToInt().ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "AfterProcessStatusChangeActionType",
                        controlCss: " always-send",
                        labelText: Displays.AfterProcessStatusChangeActionType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Process.AfterProcessStatusChangeActionTypes.ReturnToList.ToInt().ToString(),
                                Displays.ReturnToList(context: context)
                            },
                        },
                        insertBlank: true,
                        selectedValue: process.AfterProcessStatusChangeActionType.ToInt().ToString())
                    .FieldCheckBox(
                        controlId: "ProcessAllowBulkProcessing",
                        controlCss: " always-send",
                        labelText: Displays.AllowBulkProcessing(context: context),
                        _checked: process.AllowBulkProcessing == true)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessValidateInputsTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            return hb.TabsPanelField(
                id: "ProcessValidateInputsTab",
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessValidationType",
                        controlCss: " always-send",
                        labelText: Displays.InputValidationTypes(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Process.ValidationTypes.Merge.ToInt().ToString(),
                                Displays.Merge(context: context)
                            },
                            {
                                Process.ValidationTypes.Replacement.ToInt().ToString(),
                                Displays.Replacement(context: context)
                            },
                            {
                                Process.ValidationTypes.None.ToInt().ToString(),
                                Displays.None(context: context)
                            }
                        },
                        selectedValue: process.ValidationType.ToInt().ToString())
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "MoveUpProcessValidateInputs",
                            controlCss: "button-icon",
                            text: Displays.MoveUp(context: context),
                            onClick: "$p.setAndSend('#EditProcessValidateInput', $(this));",
                            icon: "ui-icon-circle-triangle-n",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "MoveDownProcessValidateInputs",
                            controlCss: "button-icon",
                            text: Displays.MoveDown(context: context),
                            onClick: "$p.setAndSend('#EditProcessValidateInput', $(this));",
                            icon: "ui-icon-circle-triangle-s",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            controlId: "NewProcessValidateInput",
                            text: Displays.New(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.openProcessValidateInputDialog($(this));",
                            icon: "ui-icon-gear",
                            action: "SetSiteSettings",
                            method: "put")
                        .Button(
                            controlId: "DeleteProcessValidateInputs",
                            text: Displays.Delete(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.setAndSend('#EditProcessValidateInput', $(this));",
                            icon: "ui-icon-trash",
                            action: "SetSiteSettings",
                            method: "delete",
                            confirm: Displays.ConfirmDelete(context: context)))
                    .EditProcessValidateInput(
                        context: context,
                        ss: ss,
                        validateInputs: process.ValidateInputs)
                    .Hidden(
                        controlId: "ProcessValidateInputs",
                        css: "always-send",
                        value: process.ValidateInputs?.ToJson()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessValidateInput(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<ValidateInput> validateInputs)
        {
            var selected = context.Forms.Data("EditProcessValidateInput").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditProcessValidateInput",
                attributes: new HtmlAttributes()
                    .DataName("ProcessValidateInputId")
                    .DataFunc("openProcessValidateInputDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditProcessValidateInputHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditProcessValidateInputBody(
                        context: context,
                        ss: ss,
                        validateInputs: validateInputs,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditProcessValidateInputHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Required(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ClientRegexValidation(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ServerRegexValidation(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.RegexValidationMessage(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Min(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Max(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessValidateInputBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<ValidateInput> validateInputs,
            IEnumerable<int> selected)
        {
            return hb.TBody(action: () => validateInputs?.ForEach(validateInput =>
            {
                hb.Tr(
                    css: "grid-row",
                    attributes: new HtmlAttributes()
                        .DataId(validateInput.Id.ToString()),
                    action: () => hb
                        .Td(action: () => hb
                            .CheckBox(
                                controlCss: "select",
                                _checked: selected?
                                    .Contains(validateInput.Id) == true))
                        .Td(action: () => hb
                            .Text(text: validateInput.Id.ToString()))
                        .Td(action: () => hb
                            .Text(text: ss.GetColumn(
                                context: context,
                                columnName: validateInput.ColumnName)?.LabelText))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: validateInput.Required == true))
                        .Td(action: () => hb
                            .Text(text: validateInput.ClientRegexValidation))
                        .Td(action: () => hb
                            .Text(text: validateInput.ServerRegexValidation))
                        .Td(action: () => hb
                            .Text(text: validateInput.RegexValidationMessage))
                        .Td(action: () => hb
                            .Text(text: validateInput.Min?.TrimEndZero()))
                        .Td(action: () => hb
                            .Text(text: validateInput.Max?.TrimEndZero())));
            }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ProcessValidateInputDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            ValidateInput validateInput)
        {
            var hb = new HtmlBuilder();
            var column = ss.GetColumn(
                context: context,
                columnName: validateInput.ColumnName);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ProcessValidateInputForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ProcessValidateInputIdTemp",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: validateInput.Id.ToString(),
                        _using: controlId == "EditProcessValidateInput")
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessValidateInputColumnName",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.ProcessValidateInputSelectableOptions(),
                        selectedValue: validateInput.ColumnName)
                    .FieldCheckBox(
                        fieldId: "ProcessValidateInputRequiredField",
                        controlId: "ProcessValidateInputRequired",
                        controlCss: " always-send",
                        labelText: Displays.Required(context: context),
                        _checked: validateInput.Required ?? false)
                    .FieldTextBox(
                        fieldId: "ProcessValidateInputClientRegexValidationField",
                        controlId: "ProcessValidateInputClientRegexValidation",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.ClientRegexValidation(context: context),
                        text: validateInput.ClientRegexValidation)
                    .FieldTextBox(
                        fieldId: "ProcessValidateInputServerRegexValidationField",
                        controlId: "ProcessValidateInputServerRegexValidation",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.ServerRegexValidation(context: context),
                        text: validateInput.ServerRegexValidation)
                    .FieldTextBox(
                        fieldId: "ProcessValidateInputRegexValidationMessageField",
                        controlId: "ProcessValidateInputRegexValidationMessage",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.RegexValidationMessage(context: context),
                        text: validateInput.RegexValidationMessage)
                    .FieldTextBox(
                        fieldId: "ProcessValidateInputMinField",
                        controlId: "ProcessValidateInputMin",
                        fieldCss: " both",
                        controlCss: " always-send",
                        labelText: Displays.Min(context: context),
                        text: validateInput.Min?.TrimEndZero(),
                        validateNumber: true)
                    .FieldTextBox(
                        fieldId: "ProcessValidateInputMaxField",
                        controlId: "ProcessValidateInputMax",
                        controlCss: " always-send",
                        labelText: Displays.Max(context: context),
                        text: validateInput.Max?.TrimEndZero(),
                        validateNumber: true)
                    .Hidden(
                        controlId: "ProcessValidateInputsTemp",
                        css: "always-send",
                        value: context.Forms.Data("ProcessValidateInputs"))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddProcessValidateInput",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewProcessValidateInput")
                        .Button(
                            controlId: "UpdateProcessValidateInput",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditProcessValidateInput")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessViewFiltersTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            var view = process.View ?? new View();
            return hb.ViewFiltersTab(
                context: context,
                ss: ss,
                view: view,
                prefix: "Process",
                currentTableOnly: true,
                action: () => hb
                    .FieldTextBox(
                        controlId: "ProcessErrorMessage",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.ErrorMessage(context: context),
                        text: ss.ColumnNameToLabelText(process.ErrorMessage)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessAccessControlsTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process,
            int totalCount = 0)
        {
            var currentPermissions = process.GetPermissions(ss: ss);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: ss,
                searchText: context.Forms.Data("SearchProcessAccessControlElements"),
                currentPermissions: currentPermissions,
                allUsers: false);
            var offset = context.Forms.Int("SourceProcessAccessControlOffset");
            return hb.TabsPanelField(id: "ProcessAccessControlsTab", action: () => hb
                .Div(id: "ProcessAccessControlEditor", action: () => hb
                    .FieldSelectable(
                        controlId: "CurrentProcessAccessControl",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " always-send send-all",
                        labelText: Displays.Permissions(context: context),
                        listItemCollection: currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false)),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.DeletePermission(context: context),
                                    onClick: "$p.deleteProcessAccessControl();",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "SourceProcessAccessControl",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(
                                context: context,
                                ss: ss,
                                withType: false),
                        commandOptionPositionIsTop: true,
                        action: "Permissions",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.AddPermission(context: context),
                                    onClick: "$p.addProcessAccessControl();",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "SearchProcessAccessControl",
                                    controlCss: " auto-postback w100",
                                    placeholder: Displays.Search(context: context),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#SearchProcessAccessControl'));",
                                    icon: "ui-icon-search")))
                    .Hidden(
                        controlId: "SourceProcessAccessControlOffset",
                        css: "always-send",
                        value: Paging.NextOffset(
                            offset: offset,
                            totalCount: sourcePermissions.Count(),
                            pageSize: Parameters.Permissions.PageSize)
                                .ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessDataChangesTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            return hb.TabsPanelField(id: "ProcessDataChangesTab", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpProcessDataChanges",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditProcessDataChange', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownProcessDataChanges",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditProcessDataChange', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewProcessDataChange",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openProcessDataChangeDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyProcessDataChanges",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditProcessDataChange', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteProcessDataChanges",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditProcessDataChange', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditProcessDataChange(
                    context: context,
                    ss: ss,
                    dataChanges: process.DataChanges)
                .Hidden(
                    controlId: "ProcessDataChanges",
                    css: "always-send",
                    value: process.DataChanges?.ToJson()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessDataChange(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<DataChange> dataChanges)
        {
            var selected = context.Forms.Data("EditProcessDataChange").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditProcessDataChange",
                attributes: new HtmlAttributes()
                    .DataName("ProcessDataChangeId")
                    .DataFunc("openProcessDataChangeDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditProcessDataChangeHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditProcessDataChangeBody(
                        context: context,
                        ss: ss,
                        dataChanges: dataChanges,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditProcessDataChangeHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ChangeTypes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.CopyFrom(context: context)
                            + "/" + Displays.Value(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessDataChangeBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<DataChange> dataChanges,
            IEnumerable<int> selected)
        {
            return hb.TBody(action: () => dataChanges?.ForEach(dataChange =>
            {
                hb.Tr(
                    css: "grid-row",
                    attributes: new HtmlAttributes()
                        .DataId(dataChange.Id.ToString()),
                    action: () => hb
                        .Td(action: () => hb
                            .CheckBox(
                                controlCss: "select",
                                _checked: selected?
                                    .Contains(dataChange.Id) == true))
                        .Td(action: () => hb
                            .Text(text: dataChange.Id.ToString()))
                        .Td(action: () => hb
                            .Text(text: Displays.Get(
                                context: context,
                                id: dataChange.Type.ToString())))
                        .Td(action: () => hb
                            .Text(text: ss.GetColumn(
                                context: context,
                                columnName: dataChange.ColumnName)?.LabelText))
                        .Td(action: () => hb
                            .Text(text: dataChange.DisplayValue(
                                context: context,
                                ss: ss))));
            }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ProcessDataChangeDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            DataChange dataChange)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ProcessDataChangeForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ProcessDataChangeIdTemp",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: dataChange.Id.ToString(),
                        _using: controlId == "EditProcessDataChange")
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessDataChangeType",
                        controlCss: " always-send",
                        labelText: Displays.ChangeTypes(context: context),
                        optionCollection: DataChangeUtilities.Types(context: context),
                        selectedValue: dataChange.Type.ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessDataChangeColumnName",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: DataChangeUtilities.ColumnSelectableOptions(
                            context: context,
                            ss: ss),
                        selectedValue: dataChange.ColumnName)
                    .FieldDropDown(
                        context: context,
                        fieldId: "ProcessDataChangeValueColumnNameField",
                        controlId: "ProcessDataChangeValueColumnName",
                        fieldCss: "field-normal" + (!dataChange.Visible(type: "Column")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.CopyFrom(context: context),
                        optionCollection: DataChangeUtilities.ColumnSelectableOptions(
                            context: context,
                            ss: ss),
                        selectedValue: dataChange.Visible(type: "Column")
                            ? dataChange.Value
                            : string.Empty)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        fieldId: "ProcessDataChangeValueField",
                        controlId: "ProcessDataChangeValue",
                        fieldCss: "field-wide" + (!dataChange.Visible(type: "Value")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Value(context: context),
                        text: dataChange.Visible(type: "Value")
                            ? ss.ColumnNameToLabelText(dataChange.Value)
                            : string.Empty)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Normal,
                        fieldId: "ProcessDataChangeValueFormulaField",
                        controlId: "ProcessDataChangeValueFormula",
                        fieldCss: "field-wide" + (!dataChange.Visible(type: "ValueFormula")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Formulas(context: context),
                        text: dataChange.Visible(type: "ValueFormula")
                            ? dataChange.DisplayValue(context: context, ss: ss)
                            : string.Empty)
                    .FieldCheckBox(
                        fieldId: "ProcessDataChangeValueFormulaNotUseDisplayNameField",
                        controlId: "ProcessDataChangeValueFormulaNotUseDisplayName",
                        fieldCss: (!dataChange.Visible(type: "ValueFormula")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.NotUseDisplayName(context: context),
                        _checked: dataChange.ValueFormulaNotUseDisplayName)
                    .FieldCheckBox(
                        fieldId: "ProcessDataChangeValueFormulaIsDisplayErrorField",
                        controlId: "ProcessDataChangeValueFormulaIsDisplayError",
                        fieldCss: (!dataChange.Visible(type: "ValueFormula")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.FormulaIsDisplayError(context: context),
                        _checked: dataChange.ValueFormulaIsDisplayError)
                    .FieldDropDown(
                        context: context,
                        fieldId: "ProcessDataChangeBaseDateTimeField",
                        controlId: "ProcessDataChangeBaseDateTime",
                        fieldCss: "field-normal" + (!dataChange.Visible(type: "DateTime")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.BaseDateTime(context: context),
                        optionCollection: DataChangeUtilities.BaseDateTimeOptions(
                            context: context,
                            ss: ss),
                        selectedValue: dataChange.Visible(type: "DateTime")
                            ? dataChange.BaseDateTime ?? (dataChange.Type == DataChange.Types.InputDate
                                ? "CurrentDate"
                                : "CurrentTime")
                            : string.Empty)
                    .FieldTextBox(
                        fieldId: "ProcessDataChangeValueDateTimeField",
                        controlId: "ProcessDataChangeValueDateTime",
                        fieldCss: "field-normal" + (!dataChange.Visible(type: "DateTime")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Value(context: context),
                        text: dataChange.Visible(type: "DateTime")
                            ? dataChange.DateTimeNumber().ToString()
                            : "0",
                        validateNumber: true)
                    .FieldDropDown(
                        context: context,
                        fieldId: "ProcessDataChangeValueColumnNamePeriodField",
                        controlId: "ProcessDataChangeValueDateTimePeriod",
                        fieldCss: "field-normal" + (!dataChange.Visible(type: "DateTime")
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Period(context: context),
                        optionCollection: ss.PeriodOptions(context: context),
                        selectedValue: dataChange.Visible(type: "DateTime")
                            ? dataChange.DateTimePeriod()
                            : string.Empty)
                    .Hidden(
                        controlId: "ProcessDataChangesTemp",
                        css: "always-send",
                        value: context.Forms.Data("ProcessDataChanges"))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddProcessDataChange",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewProcessDataChange")
                        .Button(
                            controlId: "UpdateProcessDataChange",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditProcessDataChange")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessAutoNumberingTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            return hb.TabsPanelField(id: "ProcessAutoNumberingTab", action: () => hb
                .Div(css: "items", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessAutoNumberingColumnName",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.AutoNumberingColumnOptions(context: context),
                        selectedValue: process.AutoNumbering?.ColumnName,
                        insertBlank: true)
                    .FieldTextBox(
                        controlId: "ProcessAutoNumberingFormat",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Format(context: context),
                        text: ss.ColumnNameToLabelText(process.AutoNumbering?.Format))
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessAutoNumberingResetType",
                        controlCss: " always-send",
                        labelText: Displays.ResetType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Column.AutoNumberingResetTypes.Year.ToInt().ToString(),
                                Displays.Year(context: context)
                            },
                            {
                                Column.AutoNumberingResetTypes.Month.ToInt().ToString(),
                                Displays.Month(context: context)
                            },
                            {
                                Column.AutoNumberingResetTypes.Day.ToInt().ToString(),
                                Displays.Day(context: context)
                            },
                            {
                                Column.AutoNumberingResetTypes.String.ToInt().ToString(),
                                Displays.String(context: context)
                            }
                        },
                        selectedValue: process.AutoNumbering?.ResetType.ToInt().ToString(),
                        insertBlank: true)
                    .FieldTextBox(
                        controlId: "ProcessAutoNumberingDefault",
                        controlCss: " always-send",
                        labelText: Displays.DefaultInput(context: context),
                        text: process.AutoNumbering?.Default?.ToString() ?? "1",
                        validateNumber: true,
                        validateMinNumber: 0,
                        validateMaxNumber: 999999999999999)
                    .FieldTextBox(
                        controlId: "ProcessAutoNumberingStep",
                        controlCss: " always-send",
                        labelText: Displays.Step(context: context),
                        text: process.AutoNumbering?.Step?.ToString() ?? "1",
                        validateNumber: true,
                        validateMinNumber: 1,
                        validateMaxNumber: 999999999999999)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ProcessNotificationsTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Process process)
        {
            if (context.ContractSettings.Notice == false) return hb;
            return hb.TabsPanelField(id: "ProcessNotificationsTab", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpProcessNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditProcessNotification', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownProcessNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditProcessNotification', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewProcessNotification",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openProcessNotificationDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteProcessNotifications",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditProcessNotification', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditProcessNotification(
                    context: context,
                    ss: ss,
                    notifications: process.Notifications)
                .Hidden(
                    controlId: "ProcessNotifications",
                    css: "always-send",
                    value: process.Notifications?.ToJson()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessNotification(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<Notification> notifications)
        {
            var selected = context.Forms.Data("EditProcessNotification").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditProcessNotification",
                attributes: new HtmlAttributes()
                    .DataName("ProcessNotificationId")
                    .DataFunc("openProcessNotificationDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditProcessNotificationHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditProcessNotificationBody(
                        context: context,
                        ss: ss,
                        notifications: notifications,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditProcessNotificationHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotificationType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Address(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Subject(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditProcessNotificationBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            SettingList<Notification> notifications,
            IEnumerable<int> selected)
        {
            return hb.TBody(action: () => notifications?.ForEach(notification =>
            {
                var beforeCondition = ss.Views?.Get(notification.BeforeCondition);
                var afterCondition = ss.Views?.Get(notification.AfterCondition);
                hb.Tr(
                    css: "grid-row",
                    attributes: new HtmlAttributes()
                        .DataId(notification.Id.ToString()),
                    action: () => hb
                        .Td(action: () => hb
                            .CheckBox(
                                controlCss: "select",
                                _checked: selected?
                                    .Contains(notification.Id) == true))
                        .Td(action: () => hb
                            .Text(text: notification.Id.ToString()))
                        .Td(action: () => hb
                            .Text(text: Displays.Get(
                                context: context,
                                id: notification.Type.ToString())))
                        .Td(action: () => hb
                            .Text(text: ss.ColumnNameToLabelText(notification.Address)))
                        .Td(action: () => hb
                            .Text(text: ss.ColumnNameToLabelText(notification.Subject))));
            }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ProcessNotificationDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            Notification notification)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ProcessNotificationForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ProcessNotificationIdTemp",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: notification.Id.ToString(),
                        _using: controlId == "EditProcessNotification")
                    .FieldDropDown(
                        context: context,
                        controlId: "ProcessNotificationType",
                        controlCss: " always-send",
                        labelText: Displays.NotificationType(context: context),
                        optionCollection: Parameters.Notification.ListOrder == null
                            ? NotificationUtilities.Types(context: context, isProcessNotification: true)
                            : NotificationUtilities.OrderTypes(context: context, isProcessNotification: true),
                        selectedValue: notification.Type.ToInt().ToString())
                    .FieldTextBox(
                        controlId: "ProcessNotificationSubject",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Subject(context: context),
                        text: ss.ColumnNameToLabelText(notification.Subject),
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ProcessNotificationAddress",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Address(context: context),
                        text: ss.ColumnNameToLabelText(notification.Address),
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "ProcessNotificationCcAddressField",
                        controlId: "ProcessNotificationCcAddress",
                        fieldCss: "field-wide" + (notification.Type == Notification.Types.Mail
                            ? string.Empty
                            : " hidden"),
                        controlCss: " always-send",
                        labelText: Displays.Cc(context: context),
                        text: ss.ColumnNameToLabelText(notification.CcAddress),
                        validateRequired: false)
                    .FieldTextBox(
                        fieldId: "ProcessNotificationBccAddressField",
                        controlId: "ProcessNotificationBccAddress",
                        fieldCss: "field-wide" + (notification.Type == Notification.Types.Mail
                            ? string.Empty
                            : " hidden"),
                        controlCss: " always-send",
                        labelText: Displays.Bcc(context: context),
                        text: ss.ColumnNameToLabelText(notification.BccAddress),
                        validateRequired: false)
                    .FieldTextBox(
                        fieldId: "ProcessNotificationTokenField",
                        controlId: "ProcessNotificationToken",
                        fieldCss: "field-wide" + (!NotificationUtilities.RequireToken(notification)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Token(context: context),
                        text: notification.Token)
                    .Hidden(
                        controlId: "ProcessNotificationTokenEnableList",
                        value: NotificationUtilities.Tokens())
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        fieldId: "ProcessNotificationBody",
                        controlId: "ProcessNotificationBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Body(context: context),
                        text: ss.ColumnNameToLabelText(notification.Body),
                        validateRequired: true)
                    .Hidden(
                        controlId: "ProcessNotificationsTemp",
                        css: "always-send",
                        value: context.Forms.Data("ProcessNotifications"))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddProcessNotification",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewProcessNotification")
                        .Button(
                            controlId: "UpdateProcessNotification",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditProcessNotification")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "StatusControlsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpStatusControls",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditStatusControl', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownStatusControls",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditStatusControl', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewStatusControl",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openStatusControlDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "CopyStatusControls",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditStatusControl', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteStatusControls",
                        controlCss: "button-icon",
                        text: Displays.Delete(context: context),
                        onClick: "$p.setAndSend('#EditStatusControl', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditStatusControl(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditStatusControl(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditStatusControl").Deserialize<IEnumerable<int>>();
            return hb
                .GridTable(
                    context: context,
                    id: "EditStatusControl",
                    attributes: new HtmlAttributes()
                        .DataName("StatusControlId")
                        .DataFunc("openStatusControlDialog")
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"),
                    action: () => hb
                        .StatusControlsHeader(
                            context: context,
                            ss: ss,
                            selected: selected)
                        .StatusControlsBody(
                            context: context,
                            ss: ss,
                            selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlsHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.StatusControls?.Any() == true && ss.StatusControls?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Name(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Status(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Description(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ReadOnly(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlsBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            if (ss.StatusControls?.Any() == true)
            {
                var statusColumn = ss.GetColumn(
                    context: context,
                    columnName: "Status");
                hb.TBody(action: () =>
                {
                    ss.StatusControls?.ForEach(statusControl =>
                    {
                        hb.Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(statusControl.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?.Contains(statusControl.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: statusControl.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: statusControl.Name))
                                .Td(action: () => hb
                                    .Text(text: statusControl.Status != -1
                                        ? statusColumn?.ChoiceHash?.Get(statusControl.Status.ToString())?.Text
                                        : "*"))
                                .Td(action: () => hb
                                    .Text(text: statusControl.Description))
                                .Td(action: () => hb
                                    .Span(
                                        css: "ui-icon ui-icon-circle-check",
                                        _using: statusControl.ReadOnly == true)));
                    });
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder StatusControlDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            StatusControl statusControl)
        {
            var hb = new HtmlBuilder();
            var status = ss.GetColumn(
                context: context,
                columnName: "Status");
            var optionCollection = $"-1,*\n{status.ChoicesText}".SplitReturn()
                .Select(o => new Choice(o))
                .GroupBy(o => o.Value)
                .ToDictionary(
                    o => o.Key,
                    o => new ControlData(
                        text: o.First().Text,
                        css: o.First().CssClass));
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("StatusControlForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "StatusControlId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: statusControl.Id.ToString(),
                        _using: controlId == "EditStatusControl")
                    .FieldTextBox(
                        controlId: "StatusControlName",
                        controlCss: " always-send",
                        labelText: Displays.Name(context: context),
                        text: statusControl.Name,
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "StatusControlStatus",
                        controlCss: " always-send",
                        labelText: Displays.Status(context: context),
                        optionCollection: optionCollection,
                        selectedValue: statusControl.Status.ToString())
                    .FieldTextBox(
                        controlId: "StatusControlDescription",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Description(context: context),
                        text: statusControl.Description)
                    .Div(
                        id: "StatusControlTabsContainer",
                        css: "tab-container",
                        action: () => hb
                            .Ul(id: "StatusControlTabs", action: () => hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#StatusControlGeneralTab",
                                        text: Displays.General(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StatusControlViewFiltersTab",
                                        text: Displays.Condition(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#StatusControlAccessControlsTab",
                                        text: Displays.AccessControls(context: context))))
                            .StatusControlGeneralTab(
                                context: context,
                                ss: ss,
                                statusControl: statusControl)
                            .StatusControlViewFiltersTab(
                                context: context,
                                ss: ss,
                                statusControl: statusControl)
                            .StatusControlAccessControlsTab(
                                context: context,
                                ss: ss,
                                statusControl: statusControl))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddStatusControl",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewStatusControl")
                        .Button(
                            controlId: "UpdateStatusControl",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditStatusControl")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlGeneralTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            StatusControl statusControl)
        {
            return hb.TabsPanelField(id: "StatusControlGeneralTab", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.RecordControl(context: context),
                    action: () => hb
                        .FieldCheckBox(
                            fieldId: "StatusControlReadOnlyField",
                            controlId: "StatusControlReadOnly",
                            fieldCss: "field-auto-thin",
                            controlCss: " always-send",
                            labelText: Displays.ReadOnly(context: context),
                            _checked: statusControl.ReadOnly == true))
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.ColumnControl(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "StatusControlColumnHash",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h250",
                            controlCss: " always-send send-all",
                            listItemCollection: ss.StatusControlColumnHashOptions(
                                context: context,
                                columnHash: statusControl.ColumnHash),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "StatusControlColumnNone",
                                        text: Displays.None(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.setStatusControlColumnHash($(this));",
                                        icon: "ui-icon-gear",
                                        dataType: "None")
                                    .Button(
                                        controlId: "StatusControlColumnRequired",
                                        text: Displays.Required(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.setStatusControlColumnHash($(this));",
                                        icon: "ui-icon-gear",
                                        dataType: "Required")
                                    .Button(
                                        controlId: "StatusControlColumnReadOnly",
                                        text: Displays.ReadOnly(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.setStatusControlColumnHash($(this));",
                                        icon: "ui-icon-gear",
                                        dataType: "ReadOnly")
                                    .Button(
                                        controlId: "StatusControlColumnHidden",
                                        controlCss: "button-icon",
                                        text: Displays.Hidden(context: context),
                                        onClick: "$p.setStatusControlColumnHash($(this));",
                                        icon: "ui-icon-gear",
                                        dataType: "Hidden")))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlViewFiltersTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            StatusControl statusControl)
        {
            var view = statusControl.View ?? new View();
            return hb.ViewFiltersTab(
                context: context,
                ss: ss,
                view: view,
                prefix: "StatusControl",
                currentTableOnly: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StatusControlAccessControlsTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            StatusControl statusControl,
            int totalCount = 0)
        {
            var currentPermissions = statusControl.GetPermissions(ss: ss);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: ss,
                searchText: context.Forms.Data("SearchStatusControlAccessControlElements"),
                currentPermissions: currentPermissions,
                allUsers: false);
            var offset = context.Forms.Int("SourceStatusControlAccessControlOffset");
            return hb.TabsPanelField(id: "StatusControlAccessControlsTab", action: () => hb
                .Div(id: "StatusControlAccessControlEditor", action: () => hb
                    .FieldSelectable(
                        controlId: "CurrentStatusControlAccessControl",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " always-send send-all",
                        labelText: Displays.Permissions(context: context),
                        listItemCollection: currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false)),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.DeletePermission(context: context),
                                    onClick: "$p.deleteStatusControlAccessControl();",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "SourceStatusControlAccessControl",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(
                                context: context,
                                ss: ss,
                                withType: false),
                        commandOptionPositionIsTop: true,
                        action: "Permissions",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.AddPermission(context: context),
                                    onClick: "$p.addStatusControlAccessControl();",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "SearchStatusControlAccessControl",
                                    controlCss: " auto-postback w100",
                                    placeholder: Displays.Search(context: context),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#SearchStatusControlAccessControl'));",
                                    icon: "ui-icon-search")))
                    .Hidden(
                        controlId: "SourceStatusControlAccessControlOffset",
                        css: "always-send",
                        value: Paging.NextOffset(
                            offset: offset,
                            totalCount: sourcePermissions.Count(),
                            pageSize: Parameters.Permissions.PageSize)
                                .ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "ViewsSettingsEditor", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.ViewSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "Views",
                            fieldCss: "field-vertical w400",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            listItemCollection: ss.ViewSelectableOptions(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "MoveUpViews",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumnsById(event, $(this),'Views', '');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownViews",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumnsById(event, $(this),'Views', '');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "NewView",
                                        text: Displays.New(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openViewDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "EditView",
                                        text: Displays.AdvancedSetting(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openViewDialog($(this));",
                                        icon: "ui-icon-gear",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "CopyViews",
                                        text: Displays.Copy(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-copy",
                                        action: "SetSiteSettings",
                                        method: "put")
                                    .Button(
                                        controlId: "DeleteViews",
                                        text: Displays.Delete(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-trash",
                                        action: "SetSiteSettings",
                                        method: "put"))),
                    _using: ss.ReferenceType != "Dashboards")
                .FieldDropDown(
                    context: context,
                    controlId: "SaveViewType",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.SaveViewType(context: context),
                    optionCollection: new Dictionary<string, string>()
                    {
                        {
                            SiteSettings.SaveViewTypes.Session.ToInt().ToString(),
                            Displays.SaveViewSession(context: context)
                        },
                        {
                            SiteSettings.SaveViewTypes.User.ToInt().ToString(),
                            Displays.SaveViewUser(context: context)
                        },
                        {
                            SiteSettings.SaveViewTypes.None.ToInt().ToString(),
                            Displays.SaveViewNone(context: context)
                        },
                    },
                    selectedValue: ss.SaveViewType.ToInt().ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewDialog(
            Context context, SiteSettings ss, string controlId, View view)
        {
            var hb = new HtmlBuilder();
            var hasCalendar = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Calendar" && o.ReferenceType == ss.ReferenceType);
            var hasCrosstab = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Crosstab" && o.ReferenceType == ss.ReferenceType);
            var hasGantt = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Gantt" && o.ReferenceType == ss.ReferenceType);
            var hasTimeSeries = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "TimeSeries" && o.ReferenceType == ss.ReferenceType);
            var hasAnaly = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Analy" && o.ReferenceType == ss.ReferenceType);
            var hasKamban = Def.ViewModeDefinitionCollection
                .Any(o => o.Name == "Kamban" && o.ReferenceType == ss.ReferenceType);
            var displayTypeOptionCollection = GetDisplayTypeOptionCollection(context);
            var commandDisplayTypeOptionCollection = GetCommandDisplayTypeOptionCollection(context);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ViewForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ViewId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: view.Id.ToString())
                    .FieldTextBox(
                        controlId: "ViewName",
                        labelText: Displays.Name(context: context),
                        text: view.Name,
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "DefaultViewMode",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.DefaultViewMode(context: context),
                        optionCollection: GetViewTypeOptionCollection(
                            context: context,
                            ss: ss),
                        selectedValue: view.DefaultMode,
                        insertBlank: true)
                    .Div(
                        id: "ViewTabsContainer",
                        css: "tab-container",
                        action: () => hb.Ul(
                            id: "ViewTabs",
                            action: () => hb
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewGridTab",
                                        text: Displays.Grid(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewFiltersTab",
                                        text: Displays.Filters(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewSortersTab",
                                        text: Displays.Sorters(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewEditorTab",
                                            text: Displays.Editor(context: context)))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewCalendarTab",
                                            text: Displays.Calendar(context: context)),
                                    _using: hasCalendar)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewCrosstabTab",
                                            text: Displays.Crosstab(context: context)),
                                    _using: hasCrosstab)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewGanttTab",
                                            text: Displays.Gantt(context: context)),
                                    _using: hasGantt)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewTimeSeriesTab",
                                            text: Displays.TimeSeries(context: context)),
                                    _using: hasTimeSeries)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewAnalyTab",
                                            text: Displays.Analy(context: context)),
                                    _using: hasAnaly)
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ViewKambanTab",
                                            text: Displays.Kamban(context: context)),
                                    _using: hasKamban)
                                .Li(action: () => hb
                                    .A(
                                        href: "#ViewAccessControlTab",
                                        text: Displays.AccessControls(context: context))))
                        .ViewGridTab(
                            context: context,
                            ss: ss,
                            view: view,
                            displayTypeOptionCollection: displayTypeOptionCollection,
                            commandDisplayTypeOptionCollection: commandDisplayTypeOptionCollection)
                        .ViewFiltersTab(
                            context: context,
                            ss: ss,
                            view: view)
                        .ViewSortersTab(
                            context: context,
                            ss: ss,
                            view: view)
                        .ViewEditorTab(
                            context: context,
                            ss: ss,
                            view: view,
                            commandDisplayTypeOptionCollection: commandDisplayTypeOptionCollection)
                        .ViewCalendarTab(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: hasCalendar)
                        .ViewCrosstabTab(
                            context: context,
                            ss: ss,
                            view: view,
                            commandDisplayTypeOptionCollection: commandDisplayTypeOptionCollection,
                            _using: hasCrosstab)
                        .ViewGanttTab(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: hasGantt)
                        .ViewTimeSeriesTab(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: hasTimeSeries)
                        .ViewAnalyTab(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: hasAnaly)
                        .ViewKambanTab(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: hasKamban)
                        .ViewAccessControlTab(
                            context: context,
                            ss: ss,
                            view: view))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddView",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewView")
                        .Button(
                            controlId: "UpdateView",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditView")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewGridTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string prefix = "",
            bool hasNotInner = false,
            Action action = null,
            Dictionary<string, string> displayTypeOptionCollection = null,
            Dictionary<string, string> commandDisplayTypeOptionCollection = null)
        {
            return hb.TabsPanelField(id: $"{prefix}ViewGridTab", hasNotInner: hasNotInner, action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.ListSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: $"{prefix}ViewGridColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.ViewGridSelectableOptions(
                                context: context,
                                gridColumns: view.GridColumns ?? ss.GridColumns),
                            selectedValueCollection: new List<string>(),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveUp(context: context),
                                        onClick: $"$p.moveColumns(event, $(this),'{prefix}ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.MoveDown(context: context),
                                        onClick: $"$p.moveColumns(event, $(this),'{prefix}ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableViewGridColumns",
                                        controlCss: "button-icon",
                                        text: Displays.ToDisable(context: context),
                                        onClick: $"$p.moveColumns(event, $(this),'{prefix}ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: $"{prefix}ViewGridSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.ViewGridSelectableOptions(
                                context: context,
                                gridColumns: view.GridColumns ?? ss.GridColumns,
                                enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-left", action: () => hb
                                    .Button(
                                        controlId: "ToEnableViewGridColumns",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: $"$p.moveColumns(event, $(this),'{prefix}ViewGrid',false,true);",
                                        icon: "ui-icon-circle-triangle-w")
                                    .FieldDropDown(
                                        context: context,
                                        controlId: $"{prefix}ViewGridJoin",
                                        fieldCss: "w150",
                                        controlCss: " auto-postback always-send",
                                        optionCollection: ss.JoinOptions(),
                                        addSelectedValue: false,
                                        action: "SetSiteSettings",
                                        method: "post"))))
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.FilterAndAggregationSettings(context: context),
                    action: () => hb
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_FiltersDisplayType",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.FiltersDisplayType(context: context),
                            optionCollection: displayTypeOptionCollection,
                            selectedValue: view.FiltersDisplayType?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_AggregationsDisplayType",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.AggregationsDisplayType(context: context),
                            optionCollection: displayTypeOptionCollection,
                            selectedValue: view.AggregationsDisplayType?.ToInt().ToString()),
                    _using: prefix.IsNullOrEmpty())
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.CommandButtonsSettings(context: context),
                    action: () => hb
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_BulkMoveTargetsCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.BulkMove(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.BulkMoveTargetsCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_BulkDeleteCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.BulkDelete(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.BulkDeleteCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_EditImportSettings",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Import(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.EditImportSettings?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_OpenExportSelectorDialogCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Export(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.OpenExportSelectorDialogCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_OpenBulkUpdateSelectorDialogCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.BulkUpdate(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.OpenBulkUpdateSelectorDialogCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_EditOnGridCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.EditMode(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.EditOnGridCommand?.ToInt().ToString()),
                    _using: prefix.IsNullOrEmpty()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewFiltersTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string prefix = "",
            bool currentTableOnly = false,
            Action action = null)
        {
            return hb.TabsPanelField(id: $"{prefix}ViewFiltersTab", action: () =>
            {
                hb
                    .FieldSet(
                        id: "ViewFiltersFilterSettingsEditor",
                        action: () => hb.FieldSet(
                            css: " enclosed-thin",
                            legendText: Displays.FilterSettings(context: context),
                            action: () => hb
                                .FieldSelectable(
                                    controlId: "ViewFiltersFilterColumns",
                                    fieldCss: "field-vertical",
                                    controlContainerCss: "container-selectable",
                                    controlWrapperCss: " h350",
                                    controlCss: " always-send send-all",
                                    labelText: Displays.CurrentSettings(context: context),
                                    listItemCollection: ss.ViewFilterSelectableOptions(
                                        context: context,
                                        filterColumns: view.FilterColumns ?? ss.FilterColumns),
                                    selectedValueCollection: new List<string>(),
                                    commandOptionPositionIsTop: true,
                                    commandOptionAction: () => hb
                                        .Div(css: "command-center", action: () => hb
                                            .Button(
                                                controlId: "MoveUpViewFiltersFilterColumns",
                                                controlCss: "button-icon",
                                                text: Displays.MoveUp(context: context),
                                                onClick: "$p.moveColumns(event, $(this),'ViewFiltersFilter',false,true);",
                                                icon: "ui-icon-circle-triangle-n")
                                            .Button(
                                                controlId: "MoveDownViewFiltersFilterColumns",
                                                controlCss: "button-icon",
                                                text: Displays.MoveDown(context: context),
                                                onClick: "$p.moveColumns(event, $(this),'ViewFiltersFilter',false,true);",
                                                icon: "ui-icon-circle-triangle-s")
                                            .Button(
                                                controlId: "ToDisableViewFiltersFilterColumns",
                                                controlCss: "button-icon",
                                                text: Displays.ToDisable(context: context),
                                                onClick: "$p.moveColumns(event, $(this),'ViewFiltersFilter',false,true);",
                                                icon: "ui-icon-circle-triangle-e")))
                                .FieldSelectable(
                                    controlId: "ViewFiltersFilterSourceColumns",
                                    fieldCss: "field-vertical",
                                    controlContainerCss: "container-selectable",
                                    controlWrapperCss: " h350",
                                    labelText: Displays.OptionList(context: context),
                                    listItemCollection: ss.ViewFilterSelectableOptions(
                                        context: context,
                                        filterColumns: view.FilterColumns ?? ss.FilterColumns,
                                        enabled: false),
                                    commandOptionPositionIsTop: true,
                                    commandOptionAction: () => hb
                                        .Div(css: "command-left", action: () => hb
                                            .Button(
                                                controlId: "ToEnableViewFiltersFilterColumns",
                                                text: Displays.ToEnable(context: context),
                                                controlCss: "button-icon",
                                                onClick: "$p.moveColumns(event, $(this),'ViewFiltersFilter',false,true);",
                                                icon: "ui-icon-circle-triangle-w")
                                            .FieldDropDown(
                                                context: context,
                                                controlId: "ViewFiltersFilterJoin",
                                                fieldCss: "w150",
                                                controlCss: " auto-postback always-send",
                                                optionCollection: ss.JoinOptions(),
                                                addSelectedValue: false,
                                                action: "SetSiteSettings",
                                                method: "post")))),
                        _using: prefix.IsNullOrEmpty())
                    .FieldCheckBox(
                        controlId: $"{prefix}ViewFilters_KeepFilterState",
                        fieldCss: "field-auto-thin ",
                        labelText: Displays.KeepFilterState(context: context),
                        _checked: view.KeepFilterState == true,
                        labelPositionIsRight: true,
                        _using: prefix.IsNullOrEmpty())
                    .FieldSet(
                        id: $"{prefix}ViewFiltersFilterConditionSettingsEditor",
                        css: "fieldset cf both" + (view.KeepFilterState == true
                            ? " hidden"
                            : string.Empty),
                        action: () => hb
                        .FieldSet(
                            css: " enclosed-thin",
                            legendText: Displays.FilterCondition(context: context),
                            action: () => hb
                                .Div(css: "items", action: () => hb
                                    .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_Incomplete",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Incomplete(context: context),
                                        _checked: view.Incomplete == true,
                                        labelPositionIsRight: true,
                                        _using: view.HasIncompleteColumns(context: context, ss: ss))
                                    .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_Own",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Own(context: context),
                                        _checked: view.Own == true,
                                        labelPositionIsRight: true,
                                        _using: view.HasOwnColumns(context: context, ss: ss))
                                    .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_NearCompletionTime",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.NearCompletionTime(context: context),
                                        _checked: view.NearCompletionTime == true,
                                        labelPositionIsRight: true,
                                        _using: view.HasNearCompletionTimeColumns(context: context, ss: ss))
                                    .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_Delay",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Delay(context: context),
                                        _checked: view.Delay == true,
                                        labelPositionIsRight: true,
                                        _using: view.HasDelayColumns(context: context, ss: ss))
                                    .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_Overdue",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Overdue(context: context),
                                        _checked: view.Overdue == true,
                                        labelPositionIsRight: true,
                                        _using: view.HasOverdueColumns(context: context, ss: ss))
                                    .FieldTextBox(
                                        controlId: $"{prefix}ViewFilters_Search",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.Search(context: context),
                                        text: view.Search)
                                    .ViewColumnFilters(
                                        context: context,
                                        ss: ss,
                                        view: view,
                                        prefix: prefix))
                                .FieldCheckBox(
                                        controlId: $"{prefix}ViewFilters_ShowHistory",
                                        fieldCss: "field-auto-thin",
                                        labelText: Displays.ShowHistory(context: context),
                                        _checked: view.ShowHistory == true,
                                        labelPositionIsRight: true,
                                        _using: ss.HistoryOnGrid == true)
                                .Div(css: "both", action: () => hb
                                    .FieldDropDown(
                                        context: context,
                                        controlId: $"{prefix}ViewFilterSelector",
                                        fieldCss: "field-auto-thin",
                                        controlCss: " always-send",
                                        optionCollection: ss.ViewFilterOptions(
                                            context: context,
                                            view: view,
                                            currentTableOnly: currentTableOnly))
                                    .Button(
                                        controlId: $"Add{prefix}ViewFilter",
                                        controlCss: "button-icon",
                                        text: Displays.Add(context: context),
                                        onClick: "$p.send($(this));",
                                        icon: "ui-icon-plus",
                                        action: "SetSiteSettings",
                                        method: "post"))));
                action?.Invoke();
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewColumnFilters(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string prefix)
        {
            view.ColumnFilterHash?.ForEach(data => hb
                .ViewFilter(
                    context: context,
                    ss: ss,
                    column: ss.GetColumn(
                        context: context,
                        columnName: data.Key),
                    value: data.Value,
                    prefix: prefix));
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewFilter(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            string prefix,
            string value = null)
        {
            var labelTitle = ss.LabelTitle(column);
            var controlId = $"{prefix}ViewFilters__" + column.ColumnName;
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    switch (column.CheckFilterControlType)
                    {
                        case ColumnUtilities.CheckFilterControlTypes.OnOnly:
                            return hb.FieldCheckBox(
                                controlId: controlId,
                                fieldCss: "field-auto-thin",
                                labelText: column.LabelText,
                                labelTitle: labelTitle,
                                _checked: value.ToBool());
                        case ColumnUtilities.CheckFilterControlTypes.OnAndOff:
                            return hb.FieldDropDown(
                                context: context,
                                controlId: controlId,
                                fieldCss: "field-auto-thin",
                                labelText: column.LabelText,
                                labelTitle: labelTitle,
                                optionCollection: ColumnUtilities
                                    .CheckFilterTypeOptions(context: context),
                                selectedValue: value,
                                addSelectedValue: false,
                                insertBlank: true);
                        default:
                            return hb;
                    }
                case Types.CsDateTime:
                    return column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default
                        ? hb.FieldDropDown(
                            context: context,
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            optionCollection: column.DateFilterOptions(context: context),
                            selectedValue: value,
                            multiple: true,
                            addSelectedValue: false)
                        : hb.FieldTextBox(
                            controlId: controlId + "_DateRange",
                            fieldCss: "field-auto-thin",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            text: HtmlViewFilters.GetDisplayDateFilterRange(
                                context: context,
                                value: value,
                                timepicker: column.DateTimepicker()),
                            method: "put",
                            attributes: new Dictionary<string, string>
                            {
                                ["onfocus"] = $"$p.openSiteSetDateRangeDialog($(this)," + column.DateTimepicker().ToString().ToLower() + ")",
                                ["data-action"] = $"SetSiteSettings"
                            })
                            .Hidden(attributes: new HtmlAttributes()
                                .Id($"{prefix}ViewFilters__" + column.ColumnName)
                                .Value(value));
                case Types.CsNumeric:
                    return column.DateFilterSetMode == ColumnUtilities.DateFilterSetMode.Default
                        ? hb.FieldDropDown(
                            context: context,
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback" + (column.UseSearch == true
                                ? " search"
                                : string.Empty),
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            optionCollection: column.HasChoices()
                                ? HtmlFields.EditChoices(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    value: value,
                                    own: true,
                                    multiple: true,
                                    addNotSet: true)
                                : column.NumFilterOptions(context: context),
                            selectedValue: value,
                            multiple: true,
                            addSelectedValue: false)
                        : hb.FieldTextBox(
                            controlId: controlId + "_NumericRange",
                            fieldCss: "field-auto-thin",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            text: HtmlViewFilters.GetNumericFilterRange(value),
                            method: "put",
                            attributes: new Dictionary<string, string>
                            {
                                ["onfocus"] = $"$p.openSiteSetNumericRangeDialog($(this))",
                                ["data-action"] = $"SetSiteSettings"
                            })
                            .Hidden(attributes: new HtmlAttributes()
                                .Id($"{prefix}ViewFilters__" + column.ColumnName)
                                .Value(value));
                case Types.CsString:
                    if (column.HasChoices())
                    {
                        var currentSs = column.SiteSettings;
                        if (column.UseSearch == true
                            && currentSs.Links
                                ?.Where(o => o.SiteId > 0)
                                .Any(o => o.ColumnName == column.Name) == true)
                        {
                            currentSs.SetChoiceHash(
                                context: context,
                                columnName: column?.Name,
                                selectedValues: value.Deserialize<List<string>>());
                            column.ChoiceHash = currentSs.GetColumn(
                                context: context,
                                columnName: column.Name)?.ChoiceHash
                                    ?? new Dictionary<string, Choice>();
                        }
                        return hb.FieldDropDown(
                            context: context,
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            controlCss: " auto-postback" + (column.UseSearch == true
                                ? " search"
                                : string.Empty),
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            optionCollection: HtmlFields.EditChoices(
                                context: context,
                                ss: ss,
                                column: column,
                                value: value,
                                own: true,
                                multiple: true,
                                addNotSet: true),
                            selectedValue: value,
                            multiple: true,
                            addSelectedValue: false);
                    }
                    else
                    {
                        return hb.FieldTextBox(
                            controlId: controlId,
                            fieldCss: "field-auto-thin",
                            labelText: column.LabelText,
                            labelTitle: labelTitle,
                            text: value);
                    }
                default:
                    return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ViewSortersTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string prefix = "",
            bool usekeepSorterState = true,
            bool currentTableOnly = false)
        {
            return hb.TabsPanelField(id: $"{prefix}ViewSortersTab", action: () => hb
                .FieldCheckBox(
                    controlId: "KeepSorterState",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.KeepSorterState(context: context),
                    _checked: view.KeepSorterState == true,
                    labelPositionIsRight: true,
                    _using: usekeepSorterState)
                .FieldSet(
                    id: $"{prefix}ViewFiltersSorterConditionSettingsEditor",
                    css: "both" + (view.KeepSorterState == true
                        ? " hidden"
                        : string.Empty),
                    action: () => hb
                        .FieldSet(
                            css: " enclosed-thin",
                            legendText: Displays.SortCondition(context: context),
                            action: () => hb
                .FieldBasket(
                    controlId: $"{prefix}ViewSorters",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    listItemCollection: view.ColumnSorterHash?.ToDictionary(
                        o => $"{o.Key}&{o.Value}",
                        o => new ControlData(
                            $"{ss.LabelTitle(context: context, columnName: o.Key)}" +
                            $"({DisplayOrder(context: context, type: o)})")),
                    labelAction: () => hb
                        .Text(text: Displays.Sorters(context: context)))
                .FieldDropDown(
                    context: context,
                    controlId: $"{prefix}ViewSorterSelector",
                    fieldCss: "field-auto-thin",
                    controlCss: " always-send",
                    optionCollection: ss.ViewSorterOptions(
                        context: context,
                        currentTableOnly: currentTableOnly))
                .FieldDropDown(
                    context: context,
                    controlId: $"{prefix}ViewSorterOrderTypes",
                    fieldCss: "field-auto-thin",
                    controlCss: " always-send",
                    optionCollection: new Dictionary<string, string>
                    {
                        { "asc", Displays.OrderAsc(context: context) },
                        { "desc", Displays.OrderDesc(context: context) }
                    })
                .Button(
                    controlId: $"Add{prefix}ViewSorter",
                    controlCss: " add-view-sorter button-icon",
                    attributes: new Dictionary<string, string>() { { "data-prefix", prefix } },
                    text: Displays.Add(context: context),
                    icon: "ui-icon-plus"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        internal static string DisplayOrder(
            Context context, KeyValuePair<string, SqlOrderBy.Types> type)
        {
            return Displays.Get(
                context: context,
                id: "Order" + type.Value.ToString().ToUpperFirstChar());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewEditorTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Dictionary<string, string> commandDisplayTypeOptionCollection)
        {
            return hb.TabsPanelField(id: "ViewEditorTab", action: () => hb
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.CommandButtonsSettings(context: context),
                    action: () => hb
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_UpdateCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Update(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.UpdateCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_OpenCopyDialogCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Copy(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.OpenCopyDialogCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_ReferenceCopyCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.ReferenceCopy(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.ReferenceCopyCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_MoveTargetsCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Move(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.MoveTargetsCommand?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_EditOutgoingMail",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Mail(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.EditOutgoingMail?.ToInt().ToString())
                        .FieldDropDown(
                            context: context,
                            controlId: "ViewFilters_DeleteCommand",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Delete(context: context),
                            optionCollection: commandDisplayTypeOptionCollection,
                            selectedValue: view.DeleteCommand?.ToInt().ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewCalendarTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.TabsPanelField(id: "ViewCalendarTab", action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.CalendarSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "CalendarGroupBy",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupBy(context: context),
                                optionCollection: ss.CalendarGroupByOptions(context: context),
                                selectedValue: view.GetCalendarGroupBy(),
                                insertBlank: true)
                            .FieldDropDown(
                                context: context,
                                controlId: "CalendarTimePeriod",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Period(context: context),
                                optionCollection: ss.CalendarTimePeriodOptions(context: context),
                                selectedValue: view.GetCalendarTimePeriod(ss: ss),
                                _using: ss.CalendarType == SiteSettings.CalendarTypes.Standard)
                            .FieldDropDown(
                                context: context,
                                controlId: "CalendarViewType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Period(context: context),
                                optionCollection: ss.CalendarViewTypeOptions(context: context),
                                selectedValue: view.GetCalendarViewType(),
                                _using: ss.CalendarType == SiteSettings.CalendarTypes.FullCalendar)
                            .FieldDropDown(
                                context: context,
                                controlId: "CalendarFromTo",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Column(context: context),
                                optionCollection: ss.CalendarColumnOptions(context: context),
                                selectedValue: view.GetCalendarFromTo(ss: ss))
                            .FieldCheckBox(
                                controlId: "CalendarShowStatus",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.ShowStatus(context: context),
                                _checked: view.CalendarShowStatus == true,
                                labelPositionIsRight: true)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewCrosstabTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Dictionary<string, string> commandDisplayTypeOptionCollection,
            bool _using)
        {
            return _using
                ? hb.TabsPanelField(id: "ViewCrosstabTab", action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.CrosstabSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabGroupByX",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupByX(context: context),
                                optionCollection: ss.CrosstabGroupByXOptions(context: context),
                                selectedValue: view.GetCrosstabGroupByX(context: context, ss: ss))
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabGroupByY",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupByY(context: context),
                                optionCollection: ss.CrosstabGroupByYOptions(context: context),
                                selectedValue: view.GetCrosstabGroupByY(context: context, ss: ss))
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabColumns",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.NumericColumn(context: context),
                                optionCollection: ss.CrosstabColumnsOptions(context: context),
                                selectedValue: view.CrosstabColumns,
                                multiple: true)
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabAggregateType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationType(context: context),
                                optionCollection: ss.CrosstabAggregationTypeOptions(context: context),
                                selectedValue: view.GetCrosstabAggregateType(ss))
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabValue",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationTarget(context: context),
                                optionCollection: ss.CrosstabColumnsOptions(context: context),
                                selectedValue: view.GetCrosstabValue(
                                    context: context,
                                    ss: ss))
                            .FieldDropDown(
                                context: context,
                                controlId: "CrosstabTimePeriod",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Period(context: context),
                                optionCollection: ss.CrosstabTimePeriodOptions(context: context),
                                selectedValue: view.GetCrosstabTimePeriod(ss))
                            .FieldCheckBox(
                                controlId: "CrosstabNotShowZeroRows",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.NotShowZeroRows(context: context),
                                _checked: view.CrosstabNotShowZeroRows == true,
                                labelPositionIsRight: true))
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.CommandButtonsSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "ViewFilters_ExportCrosstabCommand",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.Export(context: context),
                                optionCollection: commandDisplayTypeOptionCollection,
                                selectedValue: view.ExportCrosstabCommand?.ToInt().ToString())))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewGanttTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.TabsPanelField(id: "ViewGanttTab", action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.GanttSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "GanttGroupBy",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupBy(context: context),
                                optionCollection: ss.GanttGroupByOptions(context: context),
                                selectedValue: view.GetGanttGroupBy(),
                                insertBlank: true)
                            .FieldDropDown(
                                context: context,
                                controlId: "GanttSortBy",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.SortBy(context: context),
                                optionCollection: ss.GanttSortByOptions(context: context),
                                selectedValue: view.GetGanttSortBy(),
                                insertBlank: true)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewTimeSeriesTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.TabsPanelField(id: "ViewTimeSeriesTab", action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.TimeSeriesSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "TimeSeriesGroupBy",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupBy(context: context),
                                optionCollection: ss.TimeSeriesGroupByOptions(context: context),
                                selectedValue: view.TimeSeriesGroupBy)
                            .FieldDropDown(
                                context: context,
                                controlId: "TimeSeriesAggregateType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationType(context: context),
                                optionCollection: ss.TimeSeriesAggregationTypeOptions(context: context),
                                selectedValue: view.TimeSeriesAggregateType)
                            .FieldDropDown(
                                context: context,
                                controlId: "TimeSeriesValue",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationTarget(context: context),
                                optionCollection: ss.TimeSeriesValueOptions(context: context),
                                selectedValue: view.TimeSeriesValue)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewAnalyTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.TabsPanelField(id: "ViewAnalyTab", action: () => hb
                    .FieldSet(
                        css: " enclosed-thin",
                        legendText: Displays.AnalySettings(context: context)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewKambanTab(
            this HtmlBuilder hb, Context context, SiteSettings ss, View view, bool _using)
        {
            return _using
                ? hb.FieldSet(id: "ViewKambanTab", action: () => hb
                    .TabsPanelField(
                        css: " enclosed-thin",
                        legendText: Displays.KambanSettings(context: context),
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "KambanGroupByX",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupByX(context: context),
                                optionCollection: ss.KambanGroupByOptions(context: context),
                                selectedValue: view.KambanGroupByX)
                            .FieldDropDown(
                                context: context,
                                controlId: "KambanGroupByY",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.GroupByY(context: context),
                                optionCollection: ss.KambanGroupByOptions(
                                    context: context,
                                    addNothing: true),
                                selectedValue: view.KambanGroupByY,
                                insertBlank: true)
                            .FieldDropDown(
                                context: context,
                                controlId: "KambanAggregateType",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationType(context: context),
                                optionCollection: ss.KambanAggregationTypeOptions(context: context),
                                selectedValue: view.GetKambanAggregationType(context: context, ss: ss))
                            .FieldDropDown(
                                context: context,
                                controlId: "KambanValue",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationTarget(context: context),
                                optionCollection: ss.KambanValueOptions(context: context),
                                selectedValue: view.KambanValue)
                            .FieldDropDown(
                                context: context,
                                controlId: "KambanColumns",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.MaxColumns(context: context),
                                optionCollection: Enumerable.Range(
                                    Parameters.General.KambanMinColumns,
                                    Parameters.General.KambanMaxColumns - Parameters.General.KambanMinColumns + 1)
                                        .ToDictionary(o => o.ToString(), o => o.ToString()),
                                selectedValue: view.GetKambanColumns().ToString())
                            .FieldCheckBox(
                                controlId: "KambanAggregationView",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.AggregationView(context: context),
                                _checked: view.KambanAggregationView == true,
                                labelPositionIsRight: true)
                            .FieldCheckBox(
                                controlId: "KambanShowStatus",
                                fieldCss: "field-auto-thin",
                                labelText: Displays.ShowStatus(context: context),
                                _checked: view.KambanShowStatus == true,
                                labelPositionIsRight: true)))
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ViewAccessControlTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            int totalCount = 0)
        {
            var currentPermissions = view.GetPermissions(ss: ss);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: ss,
                searchText: context.Forms.Data("SearchViewAccessControlElements"),
                currentPermissions: currentPermissions,
                allUsers: false);
            var offset = context.Forms.Int("SourceViewAccessControlOffset");
            return hb.TabsPanelField(id: "ViewAccessControlTab", action: () => hb
                .Div(id: "ViewAccessControlEditor", action: () => hb
                    .FieldSelectable(
                        controlId: "CurrentViewAccessControl",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " always-send send-all",
                        labelText: Displays.Permissions(context: context),
                        listItemCollection: currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false)),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-right", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.DeletePermission(context: context),
                                    onClick: "$p.deleteViewAccessControl();",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "SourceViewAccessControl",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(
                                context: context,
                                ss: ss,
                                withType: false),
                        commandOptionPositionIsTop: true,
                        action: "Permissions",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.AddPermission(context: context),
                                    onClick: "$p.addViewAccessControl();",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "SearchViewAccessControl",
                                    controlCss: " auto-postback w100",
                                    placeholder: Displays.Search(context: context),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#SearchViewAccessControl'));",
                                    icon: "ui-icon-search")))
                    .Hidden(
                        controlId: "SourceViewAccessControlOffset",
                        css: "always-send",
                        value: Paging.NextOffset(
                            offset: offset,
                            totalCount: sourcePermissions.Count(),
                            pageSize: Parameters.Permissions.PageSize)
                                .ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection ViewResponses(
            this ResponseCollection res,
            SiteSettings ss,
            IEnumerable<int> selected = null)
        {
            return res
                .Html("#Views", new HtmlBuilder().SelectableItems(
                    listItemCollection: ss.ViewSelectableOptions(),
                    selectedValueTextCollection: selected?.Select(o => o.ToString())))
                .SetData("#Views");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        internal static Dictionary<string, string> GetDisplayTypeOptionCollection(Context context)
        {
            return new Dictionary<string, string>()
            {
                {
                    View.DisplayTypes.Displayed.ToInt().ToString(),
                    Displays.Displayed(context: context)
                },
                {
                    View.DisplayTypes.Hidden.ToInt().ToString(),
                    Displays.Hidden(context: context)
                },
                {
                    View.DisplayTypes.AlwaysDisplayed.ToInt().ToString(),
                    Displays.AlwaysDisplayed(context: context)
                },
                {
                    View.DisplayTypes.AlwaysHidden.ToInt().ToString(),
                    Displays.AlwaysHidden(context: context)
                }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        internal static Dictionary<string, string> GetCommandDisplayTypeOptionCollection(Context context)
        {
            return new Dictionary<string, string>()
            {
                {
                    View.CommandDisplayTypes.Displayed.ToInt().ToString(),
                    Displays.Displayed(context: context)
                },
                {
                    View.CommandDisplayTypes.None.ToInt().ToString(),
                    Displays.None(context: context)
                },
                {
                    View.CommandDisplayTypes.Disabled.ToInt().ToString(),
                    Displays.Disabled(context: context)
                },
                {
                    View.CommandDisplayTypes.Hidden.ToInt().ToString(),
                    Displays.Hidden(context: context)
                }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        internal static Dictionary<string, string> GetViewTypeOptionCollection(Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.ReferenceType == ss.ReferenceType)
                .ToDictionary(o => o.Name, o => Displays.Get(
                    context: context,
                    id: o.Name));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder NotificationsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Notice == false) return hb;
            return hb.TabsPanelField(id: "NotificationsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownNotifications",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewNotification",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openNotificationDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyNotifications",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteNotifications",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditNotification', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditNotification(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditNotification(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditNotification").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditNotification",
                attributes: new HtmlAttributes()
                    .DataName("NotificationId")
                    .DataFunc("openNotificationDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditNotificationHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditNotificationBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditNotificationHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotificationType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Prefix(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Subject(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Address(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Notifications(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeCondition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Expression(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterCondition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterCreate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterUpdate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterCopy(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterBulkUpdate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterBulkDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterImport(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditNotificationBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss.Notifications?.ForEach(notification =>
            {
                var beforeCondition = ss.Views?.Get(notification.BeforeCondition);
                var afterCondition = ss.Views?.Get(notification.AfterCondition);
                hb.Tr(
                    css: "grid-row",
                    attributes: new HtmlAttributes()
                        .DataId(notification.Id.ToString()),
                    action: () => hb
                        .Td(action: () => hb
                            .CheckBox(
                                controlCss: "select",
                                _checked: selected?
                                    .Contains(notification.Id) == true))
                        .Td(action: () => hb
                            .Text(text: notification.Id.ToString()))
                        .Td(action: () => hb
                            .Text(text: Displays.Get(
                                context: context,
                                id: notification.Type.ToString())))
                        .Td(action: () => hb
                            .Text(text: notification.Prefix))
                        .Td(action: () => hb
                            .Text(text: ss.ColumnNameToLabelText(notification.Subject)))
                        .Td(action: () => hb
                            .Text(text: ss.ColumnNameToLabelText(notification.Address)))
                        .Td(action: () => hb
                            .Text(text: notification.MonitorChangesColumns?
                                .Select(columnName => ss.GetColumn(
                                    context: context,
                                    columnName: columnName)?.LabelText)
                                .Where(labelText => labelText != null)
                                .Join(", ")))
                        .Td(action: () => hb
                            .Text(text: beforeCondition?.Name))
                        .Td(action: () => hb
                            .Text(text: beforeCondition != null && afterCondition != null
                                ? Displays.Get(
                                    context: context,
                                    id: notification.Expression.ToString())
                                : null))
                        .Td(action: () => hb
                            .Text(text: afterCondition?.Name))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterCreate != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterUpdate != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterDelete != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterCopy != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterBulkUpdate != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterBulkDelete != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.AfterImport != false))
                        .Td(action: () => hb
                            .Span(
                                css: "ui-icon ui-icon-circle-check",
                                _using: notification.Disabled == true)));
            }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder NotificationDialog(
            Context context, SiteSettings ss, string controlId, Notification notification)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("NotificationForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "NotificationId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: notification.Id.ToString(),
                        _using: controlId == "EditNotification")
                    .FieldDropDown(
                        context: context,
                        controlId: "NotificationType",
                        controlCss: " always-send",
                        labelText: Displays.NotificationType(context: context),
                        optionCollection: Parameters.Notification.ListOrder == null
                            ? NotificationUtilities.Types(context: context)
                            : NotificationUtilities.OrderTypes(context: context),
                        selectedValue: notification.Type.ToInt().ToString())
                    .FieldTextBox(
                        controlId: "NotificationPrefix",
                        controlCss: " always-send",
                        labelText: Displays.Prefix(context: context),
                        text: notification.Prefix)
                    .FieldTextBox(
                        controlId: "NotificationSubject",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Subject(context: context),
                        text: ss.ColumnNameToLabelText(notification.Subject))
                    .FieldTextBox(
                        controlId: "NotificationAddress",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Address(context: context),
                        text: ss.ColumnNameToLabelText(notification.Address),
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "NotificationCcAddressField",
                        controlId: "NotificationCcAddress",
                        fieldCss: "field-wide" + (notification.Type == Notification.Types.Mail
                            ? string.Empty
                            : " hidden"),
                        controlCss: " always-send",
                        labelText: Displays.Cc(context: context),
                        text: ss.ColumnNameToLabelText(notification.CcAddress),
                        validateRequired: false)
                    .FieldTextBox(
                        fieldId: "NotificationBccAddressField",
                        controlId: "NotificationBccAddress",
                        fieldCss: "field-wide" + (notification.Type == Notification.Types.Mail
                            ? string.Empty
                            : " hidden"),
                        controlCss: " always-send",
                        labelText: Displays.Bcc(context: context),
                        text: ss.ColumnNameToLabelText(notification.BccAddress),
                        validateRequired: false)
                    .FieldTextBox(
                        fieldId: "NotificationTokenField",
                        controlId: "NotificationToken",
                        fieldCss: "field-wide" + (!NotificationUtilities.RequireToken(notification)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Token(context: context),
                        text: notification.Token)
                    .Hidden(
                        controlId: "NotificationTokenEnableList",
                        value: NotificationUtilities.Tokens())
                    .FieldDropDown(
                        context: context,
                        fieldId: "NotificationMethodTypeField",
                        fieldCss: "field-normal" + (!NotificationUtilities.NotificationType(notification)
                            ? " hidden"
                            : String.Empty),
                        controlId: "NotificationMethodType",
                        controlCss: " always-send",
                        labelText: Displays.MethodType(context: context),
                        optionCollection: NotificationUtilities.MethodTypes(context: context),
                        selectedValue: notification.MethodType?.ToInt().ToString())
                    .FieldDropDown(
                        context: context,
                        fieldId: "NotificationEncodingField",
                        fieldCss: "field-normal" + (!NotificationUtilities.NotificationType(notification)
                            ? " hidden"
                            : String.Empty),
                        controlId: "NotificationEncoding",
                        controlCss: " always-send",
                        labelText: Displays.Encoding(context: context),
                        optionCollection: NotificationUtilities.Encodings(context: context),
                        selectedValue: notification.Encoding)
                    .FieldTextBox(
                        fieldId: "NotificationMediaTypeField",
                        fieldCss: "field-normal" + (!NotificationUtilities.NotificationType(notification)
                            ? " hidden"
                            : String.Empty),
                        controlId: "NotificationMediaType",
                        controlCss: " always-send",
                        labelText: Displays.MediaType(context: context),
                        text: notification.MediaType)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        fieldId: "NotificationRequestHeadersField",
                        controlId: "NotificationRequestHeaders",
                        fieldCss: "field-wide" + (!NotificationUtilities.NotificationType(notification)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.HttpHeader(context: context),
                        text: notification.Headers)
                    .FieldCheckBox(
                        controlId: "NotificationUseCustomFormat",
                        controlCss: " always-send",
                        labelText: Displays.UseCustomDesign(context: context),
                        _checked: notification.UseCustomFormat == true)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        fieldId: "NotificationFormatField",
                        controlId: "NotificationFormat",
                        fieldCss: "field-wide" + (notification.UseCustomFormat != true
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Format(context: context),
                        text: ss.ColumnNameToLabelText(notification.GetFormat(
                            context: context,
                            ss: ss)))
                    .Div(
                        css: "both",
                        _using: ss.Views?.Any() == true,
                        action: () => hb
                            .FieldDropDown(
                                context: context,
                                controlId: "BeforeCondition",
                                controlCss: " always-send",
                                labelText: Displays.BeforeCondition(context: context),
                                optionCollection: ss.ViewSelectableOptions(),
                                selectedValue: notification.BeforeCondition.ToString(),
                                insertBlank: true)
                            .FieldDropDown(
                                context: context,
                                controlId: "Expression",
                                controlCss: " always-send",
                                labelText: Displays.Expression(context: context),
                                optionCollection: new Dictionary<string, string>
                                {
                                    {
                                        Notification.Expressions.Or.ToInt().ToString(),
                                        Displays.Or(context: context)
                                    },
                                    {
                                        Notification.Expressions.And.ToInt().ToString(),
                                        Displays.And(context: context)
                                    }
                                },
                                selectedValue: notification.Expression.ToInt().ToString())
                            .FieldDropDown(
                                context: context,
                                controlId: "AfterCondition",
                                controlCss: " always-send",
                                labelText: Displays.AfterCondition(context: context),
                                optionCollection: ss.ViewSelectableOptions(),
                                selectedValue: notification.AfterCondition.ToString(),
                                insertBlank: true))
                    .Div(
                        css: "both",
                        action: () => hb
                            .FieldCheckBox(
                                controlId: "NotificationAfterCreate",
                                controlCss: " always-send",
                                labelText: Displays.AfterCreate(context: context),
                                _checked: notification.AfterCreate != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterUpdate",
                                controlCss: " always-send",
                                labelText: Displays.AfterUpdate(context: context),
                                _checked: notification.AfterUpdate != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterDelete",
                                controlCss: " always-send",
                                labelText: Displays.AfterDelete(context: context),
                                _checked: notification.AfterDelete != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterCopy",
                                controlCss: " always-send",
                                labelText: Displays.AfterCopy(context: context),
                                _checked: notification.AfterCopy != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterBulkUpdate",
                                controlCss: " always-send",
                                labelText: Displays.AfterBulkUpdate(context: context),
                                _checked: notification.AfterBulkUpdate != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterBulkDelete",
                                controlCss: " always-send",
                                labelText: Displays.AfterBulkDelete(context: context),
                                _checked: notification.AfterBulkDelete != false)
                            .FieldCheckBox(
                                controlId: "NotificationAfterImport",
                                controlCss: " always-send",
                                labelText: Displays.AfterImport(context: context),
                                _checked: notification.AfterImport != false)
                            .FieldCheckBox(
                                controlId: "NotificationDisabled",
                                controlCss: " always-send",
                                labelText: Displays.Disabled(context: context),
                                _checked: notification.Disabled == true))
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.MonitorChangesColumns(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "MonitorChangesColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        context: context,
                                        monitorChangesColumns: notification.MonitorChangesColumns),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpMonitorChangesColumns",
                                            text: Displays.MoveUp(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-n")
                                        .Button(
                                            controlId: "MoveDownMonitorChangesColumns",
                                            text: Displays.MoveDown(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-s")
                                        .Button(
                                            controlId: "ToDisableMonitorChangesColumns",
                                            text: Displays.ToDisable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "MonitorChangesSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h200",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: ss
                                    .MonitorChangesSelectableOptions(
                                        context: context,
                                        monitorChangesColumns: notification.MonitorChangesColumns,
                                        enabled: false),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "ToEnableMonitorChangesColumns",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'MonitorChanges');",
                                            icon: "ui-icon-circle-triangle-w"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddNotification",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setNotification($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewNotification")
                        .Button(
                            controlId: "UpdateNotification",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setNotification($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditNotification")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder RemindersSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Remind == false) return hb;
            return hb.TabsPanelField(id: "RemindersSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpReminders",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownReminders",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewReminder",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openReminderDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyReminders",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "delete")
                    .Button(
                        controlId: "DeleteReminders",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .Button(
                        controlId: "TestReminders",
                        text: Displays.Test(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditReminder', $(this));",
                        icon: "ui-icon-mail-closed",
                        action: "SetSiteSettings",
                        method: "post",
                        confirm: Displays.ConfirmSendMail(context: context)))
                .EditReminder(context: context, ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditReminder(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditReminder").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditReminder",
                attributes: new HtmlAttributes()
                    .DataName("ReminderId")
                    .DataFunc("openReminderDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditReminderHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditReminderBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditReminderHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ReminderType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Subject(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Body(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Row(context: context)))
                    .Th(
                        action: () => hb
                            .Text(text: Displays.From(context: context)),
                        _using: Parameters.Mail.FixedFrom.IsNullOrEmpty())
                    .Th(action: () => hb
                        .Text(text: Displays.To(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Column(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.StartDateTime(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.PeriodType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Range(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.SendCompletedInPast(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotSendIfNotApplicable(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.NotSendHyperLink(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ExcludeOverdue(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Condition(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditReminderBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Reminders?.ForEach(reminder =>
                {
                    var condition = ss.Views?.Get(reminder.Condition);
                    hb.Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(reminder.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(reminder.Id) == true))
                            .Td(action: () => hb
                                .Text(text: reminder.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: Displays.Get(
                                    context: context,
                                    id: reminder.ReminderType.ToString())))
                            .Td(action: () => hb
                                .Text(text: ss.ColumnNameToLabelText(reminder.Subject)))
                            .Td(action: () => hb
                                .Text(text: ss.ColumnNameToLabelText(reminder.Body)))
                            .Td(action: () => hb
                                .Text(text: ss.ColumnNameToLabelText(reminder.Line)))
                            .Td(
                                action: () => hb
                                    .Text(text: reminder.From),
                                _using: Parameters.Mail.FixedFrom.IsNullOrEmpty())
                            .Td(action: () => hb
                                .Text(text: ss.ColumnNameToLabelText(reminder.To)))
                            .Td(action: () => hb
                                .Text(text: ss.GetColumn(
                                    context: context,
                                    columnName: reminder.Column)?.LabelText))
                            .Td(action: () => hb
                                .Text(text: reminder.StartDateTime
                                    .ToString(context.CultureInfo())))
                            .Td(action: () => hb
                                .Text(text: Displays.Get(
                                    context: context,
                                    id: reminder.Type.ToString())))
                            .Td(action: () => hb
                                .Text(text: reminder.Range.ToString()))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.SendCompletedInPast == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.NotSendIfNotApplicable == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.NotSendHyperLink == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.ExcludeOverdue == true))
                            .Td(action: () => hb
                                .Text(text: condition?.Name))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: reminder.Disabled == true)));
                }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ReminderDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            Reminder reminder)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ReminderForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ReminderId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: reminder.Id.ToString(),
                        _using: controlId == "EditReminder")
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderType",
                        controlCss: " always-send",
                        labelText: Displays.ReminderType(context: context),
                        optionCollection: ReminderUtilities.Types(context: context),
                        selectedValue: reminder.ReminderType.ToInt().ToString())
                    .FieldTextBox(
                        controlId: "ReminderSubject",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Subject(context: context),
                        text: ss.ColumnNameToLabelText(reminder.Subject),
                        validateRequired: true)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "ReminderBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Body(context: context),
                        text: ss.ColumnNameToLabelText(reminder.Body),
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ReminderLine",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Row(context: context),
                        text: ss.ColumnNameToLabelText(reminder.Line),
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "ReminderFromField",
                        controlId: "ReminderFrom",
                        fieldCss: "field-wide" + (!ReminderUtilities.RequireFrom(reminder)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.From(context: context),
                        text: reminder.From,
                        validateRequired: ReminderUtilities.RequireFrom(reminder))
                    .FieldTextBox(
                        controlId: "ReminderTo",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.To(context: context),
                        text: ss.ColumnNameToLabelText(reminder.To),
                        validateRequired: true)
                    .FieldTextBox(
                        fieldId: "ReminderTokenField",
                        controlId: "ReminderToken",
                        fieldCss: "field-wide" + (!ReminderUtilities.RequireToken(reminder)
                            ? " hidden"
                            : string.Empty),
                        controlCss: " always-send",
                        labelText: Displays.Token(context: context),
                        text: reminder.Token)
                    .Hidden(
                        controlId: "ReminderFromEnableList",
                        value: ReminderUtilities.FromList())
                    .Hidden(
                        controlId: "ReminderTokenEnableList",
                        value: ReminderUtilities.TokenList())
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderColumn",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.ReminderColumnOptions(),
                        selectedValue: reminder.GetColumn(ss))
                    .FieldTextBox(
                        context: context,
                        textType: HtmlTypes.TextTypes.DateTime,
                        controlId: "ReminderStartDateTime",
                        controlCss: " always-send",
                        labelText: Displays.StartDateTime(context: context),
                        format: Displays.YmdhmDatePickerFormat(context: context),
                        text: reminder.StartDateTime.InRange()
                            ? reminder.StartDateTime.ToString(Displays.Get(
                                context: context,
                                id: "YmdhmFormat"))
                            : null,
                        timepiker: true,
                        validateRequired: true,
                        validateDate: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderRepeatType",
                        controlCss: " always-send",
                        labelText: Displays.PeriodType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Times.RepeatTypes.Daily.ToInt().ToString(),
                                Displays.Daily(context: context)
                            },
                            {
                                Times.RepeatTypes.Weekly.ToInt().ToString(),
                                Displays.Weekly(context: context)
                            },
                            {
                                Times.RepeatTypes.NumberWeekly.ToInt().ToString(),
                                Displays.NumberWeekly(context: context)
                            },
                            {
                                Times.RepeatTypes.Monthly.ToInt().ToString(),
                                Displays.Monthly(context: context)
                            },
                            {
                                Times.RepeatTypes.EndOfMonth.ToInt().ToString(),
                                Displays.EndOfMonth(context: context)
                            },
                            {
                                Times.RepeatTypes.Yearly.ToInt().ToString(),
                                Displays.Yearly(context: context)
                            }
                        },
                        selectedValue: reminder.Type.ToInt().ToString())
                    .FieldSpinner(
                        controlId: "ReminderRange",
                        controlCss: " always-send",
                        labelText: Displays.Range(context: context),
                        value: reminder.Range,
                        min: Parameters.Reminder.MinRange,
                        max: Parameters.Reminder.MaxRange,
                        step: 1,
                        width: 25,
                        unit: Displays.Day(context: context))
                    .FieldCheckBox(
                        controlId: "ReminderSendCompletedInPast",
                        controlCss: " always-send",
                        labelText: Displays.SendCompletedInPast(context: context),
                        _checked: reminder.SendCompletedInPast == true)
                    .FieldCheckBox(
                        controlId: "ReminderNotSendIfNotApplicable",
                        controlCss: " always-send",
                        labelText: Displays.NotSendIfNotApplicable(context: context),
                        _checked: reminder.NotSendIfNotApplicable == true)
                    .FieldCheckBox(
                        controlId: "ReminderNotSendHyperLink",
                        controlCss: " always-send",
                        labelText: Displays.NotSendHyperLink(context: context),
                        _checked: reminder.NotSendHyperLink == true)
                    .FieldCheckBox(
                        controlId: "ReminderExcludeOverdue",
                        controlCss: " always-send",
                        labelText: Displays.ExcludeOverdue(context: context),
                        _checked: reminder.ExcludeOverdue == true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ReminderCondition",
                        controlCss: " always-send",
                        labelText: Displays.Condition(context: context),
                        optionCollection: conditions,
                        selectedValue: reminder.Condition.ToString(),
                        insertBlank: true,
                        _using: conditions?.Any() == true)
                    .FieldCheckBox(
                        controlId: "ReminderDisabled",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: reminder.Disabled == true)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddReminder",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setReminder($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewReminder")
                        .Button(
                            controlId: "UpdateReminder",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setReminder($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditReminder")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ImportsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false) return hb;
            return hb.TabsPanelField(id: "ImportsSettingsEditor", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "ImportEncoding",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.CharacterCode(context: context),
                    optionCollection: new Dictionary<string, ControlData>
                    {
                        { "Shift-JIS", new ControlData("Shift-JIS") },
                        { "UTF-8", new ControlData("UTF-8") },
                    },
                    selectedValue: ss.ImportEncoding)
                .FieldCheckBox(
                    controlId: "UpdatableImport",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.UpdatableImport(context: context),
                    _checked: ss.UpdatableImport == true)
                .FieldDropDown(
                    context: context,
                    controlId: "DefaultImportKey",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.DefaultImportKey(context: context),
                    optionCollection: ss.Columns?
                        .Where(o => o.ImportKey == true)
                        .OrderBy(o => o.No)
                        .ToDictionary(
                            o => o.ColumnName,
                            o => o.LabelText),
                    selectedValue: ss.DefaultImportKey)
                .FieldCheckBox(
                    controlId: "RejectNullImport",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.RejectNullImport(context: context),
                    _checked: ss.RejectNullImport == true,
                    controlCss: " always-send",
                    _using: context.Controller == "items")
                .FieldCheckBox(
                    controlId: "AllowMigrationMode",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowMigrationMode(context: context),
                    _checked: ss.AllowMigrationMode == true,
                    controlCss: " always-send"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ExportsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Export == false) return hb;
            return hb.TabsPanelField(id: "ExportsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpExports",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownExports",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewExport",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openExportDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyExports",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "DeleteExports",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditExport', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditExport(context: context, ss: ss)
                .FieldCheckBox(
                    controlId: "AllowStandardExport",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AllowStandardExport(context: context),
                    _checked: ss.AllowStandardExport == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditExport(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditExport").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditExport",
                attributes: new HtmlAttributes()
                    .DataName("ExportId")
                    .DataFunc("openExportDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditExportHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditExportBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditExportHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Summaries?.Any() == true && ss.Summaries?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Name(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ExportTypes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.OutputHeader(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.DelimiterTypes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.EncloseDoubleQuotes(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ExportExecutionType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.ExportCommentsJsonFormat(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditExportBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss.Exports?
                .ForEach(export => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(export.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(export.Id) == true))
                            .Td(action: () => hb
                                .Text(text: export.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: export.Name))
                            .Td(action: () => hb
                                .Text(text: Displays.Get(
                                    context: context,
                                    id: export.Type.ToString())))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: export.Header != false))
                            .Td(action: () => hb
                                 .Text(text: Displays.Get(
                                     context: context,
                                     id: export.DelimiterType.ToString())))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: export.EncloseDoubleQuotes != false))
                            .Td(action: () => hb
                                 .Text(text: Displays.Get(
                                     context: context,
                                     id: export.ExecutionType.ToString())))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: export.ExportCommentsJsonFormat)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ExportDialog(
            Context context, SiteSettings ss, string controlId, Export export)
        {
            var hb = new HtmlBuilder();
            export.SetColumns(
                context: context,
                ss: ss);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ExportForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .Div(
                        id: "ExportTabsContainer",
                        css: "tab-container",
                        action: () => hb.Ul(
                            id: "ExportTabs",
                            action: () => hb
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExportGeneralTab",
                                            text: Displays.General(context: context))
                                .Li(
                                    action: () => hb
                                        .A(
                                            href: "#ExportAccessControlTab",
                                            text: Displays.AccessControls(context: context)))))
                        .ExportGeneralTab(
                            context: context,
                            ss: ss,
                            controlId: controlId,
                            export: export)
                        .ExportAccessControlTab(
                            context: context,
                            ss: ss,
                            export: export)
                        )
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddExport",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setExport($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewExport")
                        .Button(
                            controlId: "UpdateExport",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setExport($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditExport")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ExportColumnsDialog(
            Context context, SiteSettings ss, string controlId, ExportColumn exportColumn)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ExportColumnsForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        labelText: Displays.Column(context: context),
                        text: exportColumn.GetColumnLabelText())
                    .FieldTextBox(
                        controlId: "ExportColumnLabelText",
                        controlCss: " always-send",
                        labelText: Displays.DisplayName(context: context),
                        text: exportColumn.GetLabelText(),
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportColumnType",
                        controlCss: " always-send",
                        labelText: Displays.Output(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                ExportColumn.Types.Text.ToInt().ToString(),
                                Displays.DisplayName(context: context)
                            },
                            {
                                ExportColumn.Types.TextMini.ToInt().ToString(),
                                Displays.ShortDisplayName(context: context)
                            },
                            {
                                ExportColumn.Types.Value.ToInt().ToString(),
                                Displays.Value(context: context)
                            }
                        },
                        selectedValue: exportColumn.GetType())
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportFormat",
                        controlCss: " always-send",
                        labelText: Displays.ExportFormat(context: context),
                        optionCollection: DateTimeOptions(context: context),
                        selectedValue: exportColumn.GetFormat(),
                        _using: exportColumn.Column.TypeName == "datetime")
                    .FieldCheckBox(
                        controlId: "ExportColumnOutputClassColumn",
                        controlCss: " always-send",
                        labelText: Displays.OutputClassColumn(context: context),
                        _checked: exportColumn.OutputClassColumn == true,
                        _using: exportColumn.Column.HasChoices())
                    .Hidden(
                        controlId: "ExportColumnId",
                        css: " always-send",
                        value: exportColumn.Id.ToString())
                    .P(id: "ExportColumnsMessage", css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateExportColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setExportColumn($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ExportGeneralTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string controlId,
            Export export)
        {
            return hb.TabsPanelField(id: "ExportGeneralTab", action: () => hb
                .Div(css: "items", action: () => hb
                    .FieldText(
                        controlId: "ExportId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: export.Id.ToString(),
                        _using: controlId == "EditExport")
                    .FieldTextBox(
                        controlId: "ExportName",
                        controlCss: " always-send",
                        labelText: Displays.Name(context: context),
                        text: export.Name,
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "ExportType",
                        controlCss: " always-send",
                        labelText: Displays.ExportTypes(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Export.Types.Csv.ToInt().ToString(),
                                Displays.Csv(context: context)
                            },
                            {
                                Export.Types.Json.ToInt().ToString(),
                                Displays.Json(context: context)
                            },
                        },
                        selectedValue: export.Type.ToInt().ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "DelimiterType",
                        controlCss: " always-send",
                        labelText: Displays.DelimiterTypes(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Export.DelimiterTypes.Comma.ToInt().ToString(),
                                Displays.Comma(context: context)
                            },
                            {
                                Export.DelimiterTypes.Tab.ToInt().ToString(),
                                Displays.Tab(context: context)
                            },
                        },
                        selectedValue: export.DelimiterType.ToInt().ToString())
                    .FieldCheckBox(
                        controlId: "EncloseDoubleQuotes",
                        controlCss: " always-send",
                        labelText: Displays.EncloseDoubleQuotes(context: context),
                        _checked: export.EncloseDoubleQuotes != false)
                    .FieldDropDown(
                        context: context,
                        controlId: "ExecutionType",
                        controlCss: " always-send",
                        labelText: Displays.ExportExecutionType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Export.ExecutionTypes.Direct.ToInt().ToString(),
                                Displays.Direct(context: context)
                            },
                            {
                                Export.ExecutionTypes.MailNotify.ToInt().ToString(),
                                Displays.MailNotify(context: context)
                            },
                        },
                        selectedValue: export.ExecutionType.ToInt().ToString())
                    .FieldCheckBox(
                        controlId: "ExportHeader",
                        controlCss: " always-send",
                        labelText: Displays.OutputHeader(context: context),
                        _checked: export.Header != false)
                    .FieldCheckBox(
                        controlId: "ExportCommentsJsonFormat",
                        controlCss: " always-send",
                        labelText: Displays.ExportCommentsJsonFormat(context: context),
                        _checked: export.ExportCommentsJsonFormat)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.ExportColumns(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "ExportColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: ExportUtilities
                                    .ColumnOptions(export.Columns),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpExportColumns",
                                            text: Displays.MoveUp(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-n")
                                        .Button(
                                            controlId: "MoveDownExportColumns",
                                            text: Displays.MoveDown(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-s")
                                        .Button(
                                            controlId: "OpenExportColumnsDialog",
                                            text: Displays.AdvancedSetting(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.openExportColumnsDialog($(this));",
                                            icon: "ui-icon-circle-triangle-s",
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ToDisableExportColumns",
                                            text: Displays.ToDisable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "ExportSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h300",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: ExportUtilities
                                    .ColumnOptions(ss.ExportColumns(context: context)),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-left", action: () => hb
                                        .Button(
                                            controlId: "ToEnableExportColumns",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'Export',true);",
                                            icon: "ui-icon-circle-triangle-w")
                                    .FieldDropDown(
                                        context: context,
                                        controlId: "ExportJoin",
                                        fieldCss: "w150",
                                        controlCss: " auto-postback always-send",
                                        optionCollection: ss.JoinOptions(),
                                        addSelectedValue: false,
                                        action: "SetSiteSettings",
                                        method: "post"))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ExportAccessControlTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Export export)
        {
            var currentPermissions = export.GetPermissions(ss: ss);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: ss,
                searchText: context.Forms.Data("SearchExportAccessControlElements"),
                currentPermissions: currentPermissions,
                allUsers: false);
            var offset = context.Forms.Int("SourceExportAccessControlOffset");
            return hb.TabsPanelField(id: "ExportAccessControlTab", action: () => hb
                .Div(id: "ExportAccessControlEditor", action: () => hb
                    .FieldSelectable(
                        controlId: "CurrentExportAccessControl",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " always-send send-all",
                        labelText: Displays.Permissions(context: context),
                        listItemCollection: currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false)),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-right", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.DeletePermission(context: context),
                                    onClick: "$p.deleteExportAccessControl();",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "SourceExportAccessControl",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(
                                context: context,
                                ss: ss,
                                withType: false),
                        commandOptionPositionIsTop: true,
                        action: "Permissions",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.AddPermission(context: context),
                                    onClick: "$p.addExportAccessControl();",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "SearchExportAccessControl",
                                    controlCss: " auto-postback w100",
                                    placeholder: Displays.Search(context: context),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#SearchExportAccessControl'));",
                                    icon: "ui-icon-search")))
                    .Hidden(
                        controlId: "SourceExportAccessControlOffset",
                        css: "always-send",
                        value: Paging.NextOffset(
                            offset: offset,
                            totalCount: sourcePermissions.Count(),
                            pageSize: Parameters.Permissions.PageSize)
                                .ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CalendarSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Calendar")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "CalendarSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableCalendar",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableCalendar == true)
                        .FieldDropDown(
                            context: context,
                            controlId: "CalendarType",
                            controlCss: " always-send",
                            labelText: Displays.CalendarType(context: context),
                            optionCollection: new Dictionary<string, string>
                            {
                                {
                                    SiteSettings.CalendarTypes.Standard.ToInt().ToString(),
                                    Displays.Standard(context: context)
                                },
                                {
                                    SiteSettings.CalendarTypes.FullCalendar.ToInt().ToString(),
                                    Displays.FullCalendar(context: context)
                                },
                            },
                            selectedValue: ss.CalendarType.ToInt().ToString()))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CrosstabSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Crosstab")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "CrosstabSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableCrosstab",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableCrosstab == true)
                        .FieldCheckBox(
                            controlId: "NoDisplayCrosstabGraph",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.NoDisplayGraph(context: context),
                            _checked: ss.NoDisplayCrosstabGraph == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder GanttSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Gantt")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "GanttSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableGantt",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableGantt == true)
                        .FieldCheckBox(
                            controlId: "ShowGanttProgressRate",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.ShowProgressRate(context: context),
                            _checked: ss.ShowGanttProgressRate == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder BurnDownSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "BurnDown")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "BurnDownSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableBurnDown",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableBurnDown == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TimeSeriesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "TimeSeries")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "TimeSeriesSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableTimeSeries",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableTimeSeries == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder AnalySettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Analy")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "AnalySettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableAnaly",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableAnaly == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder KambanSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "Kamban")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "KambanSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableKamban",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableKamban == true))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ImageLibSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Images() == false) return hb;
            return Def.ViewModeDefinitionCollection
                .Where(o => o.Name == "ImageLib")
                .Any(o => o.ReferenceType == ss.ReferenceType)
                    ? hb.TabsPanelField(id: "ImageLibSettingsEditor", action: () => hb
                        .FieldCheckBox(
                            controlId: "EnableImageLib",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Enabled(context: context),
                            _checked: ss.EnableImageLib == true)
                        .FieldSpinner(
                            controlId: "ImageLibPageSize",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.NumberPerPage(context: context),
                            value: ss.ImageLibPageSize.ToDecimal(),
                            min: Parameters.General.ImageLibPageSizeMin,
                            max: Parameters.General.ImageLibPageSizeMax,
                            step: 1,
                            width: 25))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SearchSettingsEditor(
            this HtmlBuilder hb,
            Context context,
            SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            return hb.TabsPanelField(id: "SearchSettingsEditor", action: () => hb
                .FieldSet(
                    id: "SearchSettingsEditorGeneral",
                    css: " enclosed",
                    legendText: Displays.SearchSettings(context: context),
                    action: () => hb
                        .FieldDropDown(
                            context: context,
                            controlId: "SearchType",
                            controlCss: " always-send",
                            labelText: Displays.SearchTypes(context: context),
                            optionCollection: new Dictionary<string, string>()
                            {
                                {
                                    SiteSettings.SearchTypes.FullText.ToInt().ToString(),
                                    Displays.FullText(context: context)
                                },
                                {
                                    SiteSettings.SearchTypes.PartialMatch.ToInt().ToString(),
                                    Displays.PartialMatch(context: context)
                                },
                                {
                                    SiteSettings.SearchTypes.MatchInFrontOfTitle.ToInt().ToString(),
                                    Displays.MatchInFrontOfTitle(context: context)
                                },
                                {
                                    SiteSettings.SearchTypes.BroadMatchOfTitle.ToInt().ToString(),
                                    Displays.BroadMatchOfTitle(context: context)
                                }
                            },
                            selectedValue: ss.SearchType.ToInt().ToString())
                        .FieldCheckBox(
                            controlId: "Sites_DisableCrossSearch",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.Sites_DisableCrossSearch(context: context),
                            _checked: siteModel.DisableCrossSearch))
                .FieldSet(
                    id: "SearchSettingsEditorFulltext",
                    css: " enclosed",
                    legendText: Displays.FullTextSettings(context: context),
                    action: () => hb
                        .FieldCheckBox(
                            controlId: "FullTextIncludeBreadcrumb",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.FullTextIncludeBreadcrumb(context: context),
                            _checked: ss.FullTextIncludeBreadcrumb == true)
                        .FieldCheckBox(
                            controlId: "FullTextIncludeSiteId",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.FullTextIncludeSiteId(context: context),
                            _checked: ss.FullTextIncludeSiteId == true)
                        .FieldCheckBox(
                            controlId: "FullTextIncludeSiteTitle",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.FullTextIncludeSiteTitle(context: context),
                            _checked: ss.FullTextIncludeSiteTitle == true)
                        .FieldSpinner(
                            controlId: "FullTextNumberOfMails",
                            fieldCss: "field-auto-thin",
                            labelText: Displays.FullTextNumberOfMails(context: context),
                            value: ss.FullTextNumberOfMails.ToDecimal(),
                            min: 0,
                            max: Parameters.Search.FullTextMaxNumberOfMails,
                            step: 1,
                            width: 25))
                .FieldSet(
                    id: "SearchSettingsEditorOperations",
                    css: " enclosed",
                    legendText: Displays.Operations(context: context),
                    action: () => hb
                        .Button(
                            controlId: "RebuildSearchIndexes",
                            controlCss: "button-icon button-positive",
                            text: Displays.RebuildSearchIndexes(context: context),
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-refresh",
                            action: "RebuildSearchIndexes",
                            method: "post",
                            confirm: Displays.ConfirmRebuildSearchIndex(context: context))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MailSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Mail == false) return hb;
            return hb.TabsPanelField(id: "MailSettingsEditor", action: () => hb
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "AddressBook",
                    fieldCss: "field-wide",
                    labelText: Displays.DefaultAddressBook(context: context),
                    text: ss.AddressBook.ToStr())
                .FieldSet(
                    css: " enclosed-thin",
                    legendText: Displays.DefaultDestinations(context: context),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailToDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_To(context: context),
                            text: ss.MailToDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailCcDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Cc(context: context),
                            text: ss.MailCcDefault.ToStr())
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.MultiLine,
                            controlId: "MailBccDefault",
                            fieldCss: "field-wide",
                            labelText: Displays.OutgoingMails_Bcc(context: context),
                            text: ss.MailBccDefault.ToStr())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SiteIntegrationEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "SiteIntegrationEditor", action: () => hb
                .FieldTextBox(
                    controlId: "IntegratedSites",
                    fieldCss: "field-wide",
                    labelText: Displays.SiteId(context: context),
                    text: ss.IntegratedSites?.Join()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder StylesSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Style == false) return hb;
            return hb.TabsPanelField(id: "StylesSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpStyles",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownStyles",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewStyle",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openStyleDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyStyles",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteStyles",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditStyle', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .FieldCheckBox(
                        controlId: "StylesAllDisabled",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AllDisabled(context: context),
                        _checked: ss.StylesAllDisabled == true))
                .EditStyle(
                    context: context,
                    ss: ss)
                .FieldCheckBox(
                    controlId: "Responsive",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.Responsive(context: context),
                    _checked: ss.Responsive == true,
                    _using: Parameters.Mobile.Responsive,
                    labelPositionIsRight: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditStyle(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditStyle").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditStyle",
                attributes: new HtmlAttributes()
                    .DataName("StyleId")
                    .DataFunc("openStyleDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditStyleHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditStyleBody(ss: ss, selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditStyleHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Styles?.Any() == true && ss.Styles?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))
                    .EditDestinationStyleHeader(
                        context: context,
                        _using: ss.ReferenceType != "Dashboards")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationStyleHeader(this HtmlBuilder hb, Context context, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Th(action: () => hb
                    .Text(text: Displays.All(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.New(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Edit(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Index(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Calendar(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Crosstab(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Gantt(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.BurnDown(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.TimeSeries(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Analy(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Kamban(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.ImageLib(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditStyleBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Styles?.ForEach(style => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(style.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(style.Id) == true))
                            .Td(action: () => hb
                                .Text(text: style.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: style.Title))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: style.Disabled == true))
                            .EditDestinationStyleBody(
                                style: style,
                                _using: ss.ReferenceType != "Dashboards"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationStyleBody(this HtmlBuilder hb, Style style, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.All == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.New == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Edit == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Index == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Calendar == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Crosstab == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Gantt == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.BurnDown == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.TimeSeries == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Analy == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.Kamban == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: style.ImageLib == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder StyleDialog(
            Context context, SiteSettings ss, string controlId, Style style)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-style" +
                (style.All == true
                    ? " hidden"
                    : string.Empty);
            var enclosedCss = " enclosed" +
                (ss.ReferenceType == "Sites"
                    ? " hidden"
                    : string.Empty);
            var showLinkText = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var hasLink = showLinkText
                ? " has-link"
                : string.Empty;
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("StyleForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "StyleId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: style.Id.ToString(),
                        _using: controlId == "EditStyle")
                    .FieldTextBox(
                        controlId: "StyleTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: style.Title,
                        validateRequired: true)
                    .FieldCodeEditor(
                        context: context,
                        controlId: "StyleBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        dataLang: "css",
                        labelText: Displays.Style(context: context),
                        text: style.Body)
                    .FieldCheckBox(
                        fieldId: "StyleDisabledField",
                        controlId: "StyleDisabled",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: style.Disabled == true)
                    .FieldSet(
                        css: enclosedCss,
                        legendText: Displays.OutputDestination(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                fieldId: "StyleAllField",
                                controlId: "StyleAll",
                                controlCss: " always-send",
                                labelText: Displays.All(context: context),
                                _checked: style.All == true)
                            .FieldCheckBox(
                                controlId: "StyleNew",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.New(context: context),
                                _checked: style.New == true)
                            .FieldCheckBox(
                                controlId: "StyleEdit",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Edit(context: context),
                                _checked: style.Edit == true)
                            .FieldCheckBox(
                                controlId: "StyleIndex",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Index(context: context),
                                _checked: style.Index == true)
                            .FieldCheckBox(
                                controlId: "StyleCalendar",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Calendar(context: context),
                                _checked: style.Calendar == true)
                            .FieldCheckBox(
                                controlId: "StyleCrosstab",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Crosstab(context: context),
                                _checked: style.Crosstab == true)
                            .FieldCheckBox(
                                controlId: "StyleGantt",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Gantt(context: context),
                                _checked: style.Gantt == true)
                            .FieldCheckBox(
                                controlId: "StyleBurnDown",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BurnDown(context: context),
                                _checked: style.BurnDown == true)
                            .FieldCheckBox(
                                controlId: "StyleTimeSeries",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.TimeSeries(context: context),
                                _checked: style.TimeSeries == true)
                            .FieldCheckBox(
                                controlId: "StyleAnaly",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Analy(context: context),
                                _checked: style.Analy == true)
                            .FieldCheckBox(
                                controlId: "StyleKamban",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Kamban(context: context),
                                _checked: style.Kamban == true)
                            .FieldCheckBox(
                                controlId: "StyleImageLib",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.ImageLib(context: context),
                                _checked: style.ImageLib == true),
                        _using: ss.ReferenceType != "Dashboards")
                    .Hidden(
                        controlId: "StyleAll",
                        css: " always-send",
                        value: "1",
                        _using: ss.ReferenceType == "Dashboards")
                    .P(css: "message-dialog")
                    .Div(css: "command-center" + hasLink, action: () => hb
                        .Div(
                            css: "link-item",
                            action: () => hb.A(
                                text: Displays.HowToDevelopEfficiently(context: context),
                                href: Parameters.General.RecommendUrl2.Params(
                                    Parameters.General.PleasanterSource,
                                    "code-assist",
                                    "styles-settings"),
                                target: "_blank"),
                            _using: showLinkText)
                        .Button(
                            controlId: "AddStyle",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setStyle($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewStyle")
                        .Button(
                            controlId: "UpdateStyle",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setStyle($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditStyle")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScriptsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Script == false) return hb;
            return hb.TabsPanelField(id: "ScriptsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewScript",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openScriptDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyScripts",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteScripts",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditScript', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .FieldCheckBox(
                        controlId: "ScriptsAllDisabled",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AllDisabled(context: context),
                        _checked: ss.ScriptsAllDisabled == true))
                .EditScript(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditScript(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditScript").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditScript",
                attributes: new HtmlAttributes()
                    .DataName("ScriptId")
                    .DataFunc("openScriptDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditScriptHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditScriptBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditScriptHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Scripts?.Any() == true && ss.Scripts?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))
                    .EditDestinationScriptHeader(
                        context: context,
                        _using: ss.ReferenceType != "Dashboards")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationScriptHeader(this HtmlBuilder hb, Context context, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Th(action: () => hb
                    .Text(text: Displays.All(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.New(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Edit(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Index(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Calendar(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Crosstab(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Gantt(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.BurnDown(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.TimeSeries(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Analy(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Kamban(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.ImageLib(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditScriptBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Scripts?.ForEach(script => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(script.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(script.Id) == true))
                            .Td(action: () => hb
                                .Text(text: script.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: script.Title))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Disabled == true))
                            .EditDestinationScriptBody(
                                script: script,
                                _using: ss.ReferenceType != "Dashboards"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationScriptBody(this HtmlBuilder hb, Script script, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.All == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.New == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Edit == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Index == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Calendar == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Crosstab == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Gantt == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.BurnDown == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.TimeSeries == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Analy == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.Kamban == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: script.ImageLib == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ScriptDialog(
            Context context, SiteSettings ss, string controlId, Script script)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-script" +
                (script.All == true
                    ? " hidden"
                    : string.Empty);
            var enclosedCss = " enclosed" +
                (ss.ReferenceType == "Sites"
                    ? " hidden"
                    : string.Empty);
            var showLinkText = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var hasLink = showLinkText
                ? " has-link"
                : string.Empty;
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ScriptForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ScriptId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: script.Id.ToString(),
                        _using: controlId == "EditScript")
                    .FieldTextBox(
                        controlId: "ScriptTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: script.Title,
                        validateRequired: true)
                    .FieldCodeEditor(
                        context: context,
                        controlId: "ScriptBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        dataLang: "javascript",
                        labelText: Displays.Script(context: context),
                        text: script.Body)
                    .FieldCheckBox(
                        fieldId: "ScriptDisabledField",
                        controlId: "ScriptDisabled",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: script.Disabled == true)
                    .FieldSet(
                        css: enclosedCss,
                        legendText: Displays.OutputDestination(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                fieldId: "ScriptAllField",
                                controlId: "ScriptAll",
                                controlCss: " always-send",
                                labelText: Displays.All(context: context),
                                _checked: script.All == true)
                            .FieldCheckBox(
                                controlId: "ScriptNew",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.New(context: context),
                                _checked: script.New == true)
                            .FieldCheckBox(
                                controlId: "ScriptEdit",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Edit(context: context),
                                _checked: script.Edit == true)
                            .FieldCheckBox(
                                controlId: "ScriptIndex",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Index(context: context),
                                _checked: script.Index == true)
                            .FieldCheckBox(
                                controlId: "ScriptCalendar",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Calendar(context: context),
                                _checked: script.Calendar == true)
                            .FieldCheckBox(
                                controlId: "ScriptCrosstab",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Crosstab(context: context),
                                _checked: script.Crosstab == true)
                            .FieldCheckBox(
                                controlId: "ScriptGantt",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Gantt(context: context),
                                _checked: script.Gantt == true)
                            .FieldCheckBox(
                                controlId: "ScriptBurnDown",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BurnDown(context: context),
                                _checked: script.BurnDown == true)
                            .FieldCheckBox(
                                controlId: "ScriptTimeSeries",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.TimeSeries(context: context),
                                _checked: script.TimeSeries == true)
                            .FieldCheckBox(
                                controlId: "ScriptAnaly",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Analy(context: context),
                                _checked: script.Analy == true)
                            .FieldCheckBox(
                                controlId: "ScriptKamban",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Kamban(context: context),
                                _checked: script.Kamban == true)
                            .FieldCheckBox(
                                controlId: "ScriptImageLib",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.ImageLib(context: context),
                                _checked: script.ImageLib == true),
                        _using: ss.ReferenceType != "Dashboards")
                    .Hidden(
                        controlId: "ScriptAll",
                        css: " always-send",
                        value: "1",
                        _using: ss.ReferenceType == "Dashboards")
                    .P(css: "message-dialog")
                    .Div(css: "command-center" + hasLink, action: () => hb
                        .Div(
                            css: "link-item",
                            action: () => hb.A(
                                text: Displays.HowToDevelopEfficiently(context: context),
                                href: Parameters.General.RecommendUrl2.Params(
                                    Parameters.General.PleasanterSource,
                                    "code-assist",
                                    "scripts-settings"),
                                target: "_blank"),
                            _using: showLinkText)
                        .Button(
                            controlId: "AddScript",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setScript($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewScript")
                        .Button(
                            controlId: "UpdateScript",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setScript($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditScript")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder HtmlsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.Html == false) return hb;
            return hb.TabsPanelField(id: "HtmlsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpHtmls",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditHtml', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownHtmls",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditHtml', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewHtml",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openHtmlDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyHtmls",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditHtml', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteHtmls",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditHtml', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .FieldCheckBox(
                        controlId: "HtmlsAllDisabled",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AllDisabled(context: context),
                        _checked: ss.HtmlsAllDisabled == true))
                .EditHtml(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditHtml(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditHtml").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditHtml",
                attributes: new HtmlAttributes()
                    .DataName("HtmlId")
                    .DataFunc("openHtmlDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditHtmlHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditHtmlBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditHtmlHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Htmls?.Any() == true && ss.Htmls?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.HtmlPositionType(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))
                    .EditDestinationHtmlHeader(
                        context: context,
                        _using: ss.ReferenceType != "Dashboards")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationHtmlHeader(this HtmlBuilder hb, Context context, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Th(action: () => hb
                    .Text(text: Displays.All(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.New(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Edit(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Index(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Calendar(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Crosstab(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Gantt(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.BurnDown(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.TimeSeries(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Analy(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.Kamban(context: context)))
                .Th(action: () => hb
                    .Text(text: Displays.ImageLib(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditHtmlBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .Htmls?.ForEach(html =>
                {
                    var positionType = string.Empty;
                    switch (html.PositionType)
                    {
                        case Html.PositionTypes.HeadTop:
                            positionType = Displays.HtmlHeadTop(context: context);
                            break;
                        case Html.PositionTypes.HeadBottom:
                            positionType = Displays.HtmlHeadBottom(context: context);
                            break;
                        case Html.PositionTypes.BodyScriptTop:
                            positionType = Displays.HtmlBodyScriptTop(context: context);
                            break;
                        case Html.PositionTypes.BodyScriptBottom:
                            positionType = Displays.HtmlBodyScriptBottom(context: context);
                            break;
                    }
                    hb
                        .Tr(
                            css: "grid-row",
                            attributes: new HtmlAttributes()
                                .DataId(html.Id.ToString()),
                            action: () => hb
                                .Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "select",
                                        _checked: selected?
                                            .Contains(html.Id) == true))
                                .Td(action: () => hb
                                    .Text(text: html.Id.ToString()))
                                .Td(action: () => hb
                                    .Text(text: html.Title))
                                .Td(action: () => hb
                                    .Text(text: positionType))
                                .Td(action: () => hb
                                    .Span(
                                        css: "ui-icon ui-icon-circle-check",
                                        _using: html.Disabled == true))
                                .EditDestinationHtmlBody(
                                    html: html,
                                    _using: ss.ReferenceType != "Dashboards"));
                }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDestinationHtmlBody(this HtmlBuilder hb, Html html, bool _using)
        {
            if (!_using) return hb;
            return hb
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.All == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.New == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Edit == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Index == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Calendar == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Crosstab == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Gantt == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.BurnDown == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.TimeSeries == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Analy == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.Kamban == true))
                .Td(action: () => hb
                    .Span(
                        css: "ui-icon ui-icon-circle-check",
                        _using: html.ImageLib == true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder HtmlDialog(
            Context context, SiteSettings ss, string controlId, Html html)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-html" +
                (html.All == true
                    ? " hidden"
                    : string.Empty);
            var enclosedCss = " enclosed" +
                (ss.ReferenceType == "Sites"
                    ? " hidden"
                    : string.Empty);
            var showLinkText = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var hasLink = showLinkText
                ? " has-link"
                : string.Empty;
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("HtmlForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "HtmlId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: html.Id.ToString(),
                        _using: controlId == "EditHtml")
                    .FieldTextBox(
                        controlId: "HtmlTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: html.Title,
                        validateRequired: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "HtmlPositionType",
                        controlCss: " always-send",
                        labelText: Displays.HtmlPositionType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                Html.PositionTypes.HeadTop.ToInt().ToString(),
                                Displays.HtmlHeadTop(context: context)
                            },
                            {
                                Html.PositionTypes.HeadBottom.ToInt().ToString(),
                                Displays.HtmlHeadBottom(context: context)
                            },
                            {
                                Html.PositionTypes.BodyScriptTop.ToInt().ToString(),
                                Displays.HtmlBodyScriptTop(context: context)
                            },
                            {
                                Html.PositionTypes.BodyScriptBottom.ToInt().ToString(),
                                Displays.HtmlBodyScriptBottom(context: context)
                            }
                        },
                        selectedValue: html.PositionType.ToInt().ToString(),
                        insertBlank: false)
                    .FieldCodeEditor(
                        context: context,
                        controlId: "HtmlBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        dataLang: "html",
                        labelText: Displays.Html(context: context),
                        text: html.Body)
                    .FieldCheckBox(
                        fieldId: "HtmlDisabledField",
                        controlId: "HtmlDisabled",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: html.Disabled == true)
                    .FieldSet(
                        css: enclosedCss,
                        legendText: Displays.OutputDestination(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                fieldId: "HtmlAllField",
                                controlId: "HtmlAll",
                                controlCss: " always-send",
                                labelText: Displays.All(context: context),
                                _checked: html.All == true)
                            .FieldCheckBox(
                                controlId: "HtmlNew",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.New(context: context),
                                _checked: html.New == true)
                            .FieldCheckBox(
                                controlId: "HtmlEdit",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Edit(context: context),
                                _checked: html.Edit == true)
                            .FieldCheckBox(
                                controlId: "HtmlIndex",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Index(context: context),
                                _checked: html.Index == true)
                            .FieldCheckBox(
                                controlId: "HtmlCalendar",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Calendar(context: context),
                                _checked: html.Calendar == true)
                            .FieldCheckBox(
                                controlId: "HtmlCrosstab",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Crosstab(context: context),
                                _checked: html.Crosstab == true)
                            .FieldCheckBox(
                                controlId: "HtmlGantt",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Gantt(context: context),
                                _checked: html.Gantt == true)
                            .FieldCheckBox(
                                controlId: "HtmlBurnDown",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BurnDown(context: context),
                                _checked: html.BurnDown == true)
                            .FieldCheckBox(
                                controlId: "HtmlTimeSeries",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.TimeSeries(context: context),
                                _checked: html.TimeSeries == true)
                            .FieldCheckBox(
                                controlId: "HtmlAnaly",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Analy(context: context),
                                _checked: html.Analy == true)
                            .FieldCheckBox(
                                controlId: "HtmlKamban",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Kamban(context: context),
                                _checked: html.Kamban == true)
                            .FieldCheckBox(
                                controlId: "HtmlImageLib",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.ImageLib(context: context),
                                _checked: html.ImageLib == true),
                        _using: ss.ReferenceType != "Dashboards")
                    .Hidden(
                        controlId: "HtmlAll",
                        css: " always-send",
                        value: "1",
                        _using: ss.ReferenceType == "Dashboards")
                    .P(css: "message-dialog")
                    .Div(css: "command-center" + hasLink, action: () => hb
                        .Div(
                            css: "link-item",
                            action: () => hb.A(
                                text: Displays.HowToDevelopEfficiently(context: context),
                                href: Parameters.General.RecommendUrl2.Params(
                                    Parameters.General.PleasanterSource,
                                    "code-assist",
                                    "htmls-settings"),
                                target: "_blank"),
                            _using: showLinkText)
                        .Button(
                            controlId: "AddHtml",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setHtml($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewHtml")
                        .Button(
                            controlId: "UpdateHtml",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setHtml($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditHtml")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ServerScriptsSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ServerScript == false
                || Parameters.Script.ServerScript == false) return hb;
            return hb.TabsPanelField(id: "ServerScriptsSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpServerScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditServerScript', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownServerScripts",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditServerScript', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewServerScript",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openServerScriptDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyServerScripts",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditServerScript', $(this));",
                        icon: "ui-icon-copy",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteServerScripts",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditServerScript', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context))
                    .FieldCheckBox(
                        controlId: "ServerScriptsAllDisabled",
                        fieldCss: "field-auto-thin",
                        labelText: Displays.AllDisabled(context: context),
                        _checked: ss.ServerScriptsAllDisabled == true))
                .EditServerScript(
                    context: context,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditServerScript(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditServerScript").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditServerScript",
                attributes: new HtmlAttributes()
                    .DataName("ServerScriptId")
                    .DataFunc("openServerScriptDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditServerScriptHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditServerScriptBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditServerScriptHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.ServerScripts?.Any() == true && ss.ServerScripts?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Name(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Disabled(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Functionalize(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.TryCatch(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.WhenloadingSiteSettings(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.WhenViewProcessing(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.WhenloadingRecord(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeFormulas(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterFormulas(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeCreate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterCreate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeUpdate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterUpdate(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeBulkDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.AfterBulkDelete(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeOpeningPage(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.BeforeOpeningRow(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Shared(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditServerScriptBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .ServerScripts?.ForEach(script => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(script.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(script.Id) == true))
                            .Td(action: () => hb
                                .Text(text: script.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: script.Title))
                            .Td(action: () => hb
                                .Text(text: script.Name))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Disabled == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Functionalize == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.TryCatch == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.WhenloadingSiteSettings == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.WhenViewProcessing == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.WhenloadingRecord == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeFormula == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.AfterFormula == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeCreate == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.AfterCreate == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeUpdate == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.AfterUpdate == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeDelete == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.AfterDelete == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeBulkDelete == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.AfterBulkDelete == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeOpeningPage == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.BeforeOpeningRow == true))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: script.Shared == true)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ServerScriptDialog(
            Context context, SiteSettings ss, string controlId, ServerScript script)
        {
            var hb = new HtmlBuilder();
            var conditions = ss.ViewSelectableOptions();
            var outputDestinationCss = " output-destination-script";
            var enclosedCss = " enclosed" +
                (ss.ReferenceType == "Sites"
                    ? " hidden"
                    : string.Empty);
            var showLinkText = !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
            var hasLink = showLinkText
                ? " has-link"
                : string.Empty;
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ServerScriptForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "ServerScriptId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: script.Id.ToString(),
                        _using: controlId == "EditServerScript")
                    .FieldTextBox(
                        controlId: "ServerScriptTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: script.Title,
                        validateRequired: true)
                    .FieldTextBox(
                        controlId: "ServerScriptName",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Name(context: context),
                        text: script.Name)
                    .FieldCodeEditor(
                        context: context,
                        controlId: "ServerScriptBody",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        dataLang: "javascript",
                        labelText: Displays.Script(context: context),
                        text: script.Body)
                    .FieldCheckBox(
                        controlId: "ServerScriptDisabled",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: script.Disabled == true)
                    .FieldSpinner(
                        controlId: "ServerScriptTimeOut",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.TimeOut(context: context),
                        value: script.TimeOut ?? Parameters.Script.ServerScriptTimeOut,
                        min: Parameters.Script.ServerScriptTimeOutMin,
                        max: Parameters.Script.ServerScriptTimeOutMax,
                        step: 1,
                        width: 75,
                        _using: Parameters.Script.ServerScriptTimeOutChangeable)
                    .FieldCheckBox(
                        controlId: "ServerScriptFunctionalize",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Functionalize(context: context),
                        _checked: script.Functionalize == true)
                    .FieldCheckBox(
                        controlId: "ServerScriptTryCatch",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.TryCatch(context: context),
                        _checked: script.TryCatch == true)
                    .FieldSet(
                        css: enclosedCss,
                        legendText: Displays.Condition(context: context),
                        action: () => hb
                            .FieldCheckBox(
                                controlId: "ServerScriptWhenloadingSiteSettings",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.WhenloadingSiteSettings(context: context),
                                _checked: script.WhenloadingSiteSettings == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptWhenViewProcessing",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.WhenViewProcessing(context: context),
                                _checked: script.WhenViewProcessing == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptWhenloadingRecord",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.WhenloadingRecord(context: context),
                                _checked: script.WhenloadingRecord == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeFormula",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeFormulas(context: context),
                                _checked: script.BeforeFormula == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptAfterFormula",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.AfterFormulas(context: context),
                                _checked: script.AfterFormula == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeCreate",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeCreate(context: context),
                                _checked: script.BeforeCreate == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptAfterCreate",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.AfterCreate(context: context),
                                _checked: script.AfterCreate == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeUpdate",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeUpdate(context: context),
                                _checked: script.BeforeUpdate == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptAfterUpdate",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.AfterUpdate(context: context),
                                _checked: script.AfterUpdate == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeDelete",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeDelete(context: context),
                                _checked: script.BeforeDelete == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptAfterDelete",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.AfterDelete(context: context),
                                _checked: script.AfterDelete == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeBulkDelete",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeBulkDelete(context: context),
                                _checked: script.BeforeBulkDelete == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptAfterBulkDelete",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.AfterBulkDelete(context: context),
                                _checked: script.AfterBulkDelete == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeOpeningPage",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeOpeningPage(context: context),
                                _checked: script.BeforeOpeningPage == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptBeforeOpeningRow",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.BeforeOpeningRow(context: context),
                                _checked: script.BeforeOpeningRow == true)
                            .FieldCheckBox(
                                controlId: "ServerScriptShared",
                                fieldCss: outputDestinationCss,
                                controlCss: " always-send",
                                labelText: Displays.Shared(context: context),
                                _checked: script.Shared == true))
                    .P(css: "message-dialog")
                    .Div(css: "command-center" + hasLink, action: () => hb
                        .Div(
                            css: "link-item",
                            action: () => hb.A(
                                text: Displays.HowToDevelopEfficiently(context: context),
                                href: Parameters.General.RecommendUrl2.Params(
                                    Parameters.General.PleasanterSource,
                                    "code-assist",
                                    "server-scripts-settings"),
                                target: "_blank"),
                            _using: showLinkText)
                        .Button(
                            controlId: "AddServerScript",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setServerScript($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewServerScript")
                        .Button(
                            controlId: "UpdateServerScript",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setServerScript($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditServerScript")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PublishSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool publish)
        {
            if (context.ContractSettings.Extensions?.Get("Publish") != true)
            {
                return hb;
            }
            return hb.TabsPanelField(id: "PublishSettingsEditor", action: () => hb
                .FieldCheckBox(
                    controlId: "Sites_Publish",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.PublishToAnonymousUsers(context: context),
                    _checked: publish));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DeleteSiteDialog(this HtmlBuilder hb, Context context)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("DeleteSiteDialog")
                    .Class("dialog")
                    .Title(Displays.ConfirmDeleteSite(context: context)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DeleteSiteTitle",
                        labelText: Displays.SiteTitle(context: context))
                    .FieldTextBox(
                        controlId: "Users_LoginId",
                        labelText: Displays.Users_LoginId(context: context),
                        _using: !Authentications.DisableDeletingSiteAuthentication(context: context))
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Password,
                        controlId: "Users_Password",
                        labelText: Displays.Users_Password(context: context),
                        _using: !Authentications.DisableDeletingSiteAuthentication(context: context))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.DeleteSite(context: context),
                            controlCss: "button-icon button-negative",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-trash",
                            action: "Delete",
                            method: "delete",
                            confirm: "ConfirmDelete")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SynchronizeTitles(Context context, SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            ItemUtilities.UpdateTitles(
                context: context,
                ss: ss,
                siteIdList: new List<long> { ss.SiteId });
            return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData SynchronizeSummaries(
            Context context,
            SiteModel siteModel,
            List<int> selected,
            Action watchdog = null)
        {
            siteModel.SetSiteSettingsPropertiesBySession(context: context);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid;
            }
            if (selected?.Any() != true)
            {
                return new ErrorData(type: Error.Types.SelectTargets);
            }
            else
            {
                selected.ForEach(id =>
                {
                    watchdog?.Invoke();
                    Summaries.Synchronize(
                        context: context,
                        ss: ss,
                        id: id);
                });
                return new ErrorData(type: Error.Types.None);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SynchronizeFormulas(Context context, SiteModel siteModel)
        {
            siteModel.SetSiteSettingsPropertiesBySession(context: context);
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context, siteModel: siteModel, referenceId: siteModel.SiteId);
            var ss = siteModel.SiteSettings;
            var invalid = SiteValidators.OnUpdating(
                context: context, ss: ss, siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selected = context.Forms.IntList("EditFormula");
            if (selected?.Any() != true)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            else
            {
                ss.SetChoiceHash(context: context);
                FormulaUtilities.Synchronize(
                    context: context,
                    siteModel: siteModel,
                    selected: selected);
                return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditRelatingColumns(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditRelatingColumns")
                .Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditRelatingColumns",
                attributes: new HtmlAttributes()
                    .DataName("RelatingColumnId")
                    .DataFunc("openRelatingColumnDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditRelatingColumnsHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditRelatingColumnsBody(
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditRelatingColumnsHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: false))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Links(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditRelatingColumnsBody(
            this HtmlBuilder hb, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .RelatingColumns?.ForEach(relatingColumn => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(relatingColumn.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(relatingColumn.Id) == true))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Title))
                            .Td(action: () => hb
                                .Text(text: relatingColumn.Columns?
                                    .Select(o => GetClassLabelText(ss, o)).Join(", "))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder DashboardPartSettingsEditor(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.TabsPanelField(id: "DashboardPartSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "MoveUpDashboardParts",
                        controlCss: "button-icon",
                        text: Displays.MoveUp(context: context),
                        onClick: "$p.setAndSend('#EditDashboardPart', $(this));",
                        icon: "ui-icon-circle-triangle-n",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "MoveDownDashboardParts",
                        controlCss: "button-icon",
                        text: Displays.MoveDown(context: context),
                        onClick: "$p.setAndSend('#EditDashboardPart', $(this));",
                        icon: "ui-icon-circle-triangle-s",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "NewDashboardPart",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openDashboardPartDialog($(this));",
                        icon: "ui-icon-gear",
                        action: "SetSiteSettings",
                        method: "put")
                    .Button(
                        controlId: "CopyDashboardParts",
                        text: Displays.Copy(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditDashboardPart', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "post")
                    .Button(
                        controlId: "DeleteDashboardParts",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditDashboardPart', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetSiteSettings",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditDashboardPart(
                    context: context,
                    ss: ss)
                .FieldCheckBox(
                    controlId: "AsynchronousLoadingDefault",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.AsynchronousLoading(context: context),
                    _checked: ss.DashboardPartsAsynchronousLoading ?? false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditDashboardPart(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var selected = context.Forms.Data("EditDashboardPart").Deserialize<IEnumerable<int>>();
            return hb.GridTable(
                context: context,
                id: "EditDashboardPart",
                attributes: new HtmlAttributes()
                    .DataName("DashboardPartId")
                    .DataFunc("openDashboardPartDialog")
                    .DataAction("SetSiteSettings")
                    .DataMethod("post"),
                action: () => hb
                    .EditDashboardPartHeader(
                        context: context,
                        ss: ss,
                        selected: selected)
                    .EditDashboardPartBody(
                        context: context,
                        ss: ss,
                        selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditDashboardPartHeader(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: ss.Scripts?.Any() == true && ss.Scripts?.All(o =>
                                selected?.Contains(o.Id) == true) == true))
                    .Th(action: () => hb
                        .Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.PartsType(context: context)))
                    .Th(action: () => hb
                        .Text(text: "X"))
                    .Th(action: () => hb
                        .Text(text: "Y"))
                    .Th(action: () => hb
                        .Text(text: Displays.Width(context: context)))
                    .Th(action: () => hb
                        .Text(text: Displays.Height(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditDashboardPartBody(
            this HtmlBuilder hb, Context context, SiteSettings ss, IEnumerable<int> selected)
        {
            return hb.TBody(action: () => ss
                .DashboardParts?.ForEach(dashboardPart => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(dashboardPart.Id.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?
                                        .Contains(dashboardPart.Id) == true))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.Id.ToString()))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.Title))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.PartTypeString(context: context)))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.X.ToString()))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.Y.ToString()))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.Width.ToString()))
                            .Td(action: () => hb
                                .Text(text: dashboardPart.Height.ToString())))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartTimeLineSitesDialog(
            Context context,
            SiteSettings ss,
            int dashboardPartId,
            string dashboardTimeLineSites)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DashboardPartTimeLineSitesEditForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DashboardPartTimeLineSitesEdit",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.SiteId(context: context),
                        text: dashboardTimeLineSites,
                        validateRequired: true)
                    .Hidden(
                        controlId: "DashboardPartId",
                        alwaysSend: true,
                        value: dashboardPartId.ToString())
                    .Hidden(
                        controlId: "SavedDashboardPartTimeLineSites",
                        alwaysSend: true,
                        value: dashboardTimeLineSites)
                    .Hidden(
                        controlId: "ClearDashboardView",
                        action: "SetSiteSettings",
                        method: "post")
                    .P(
                        id: "DashboardPartTimeLineSitesMessage",
                        css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateDashboardPartTimeLineSites",
                            text: Displays.OK(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-pencil",
                            onClick: "$p.updateDashboardPartTimeLineSites($(this));",
                            action: "SetSiteSettings",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartCalendarSitesDialog(
            Context context,
            SiteSettings ss,
            int dashboardPartId,
            string dashboardCalendarSites)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DashboardPartCalendarSitesEditForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DashboardPartCalendarSitesEdit",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.SiteId(context: context),
                        text: dashboardCalendarSites,
                        validateRequired: true)
                    .Hidden(
                        controlId: "DashboardPartId",
                        alwaysSend: true,
                        value: dashboardPartId.ToString())
                    .Hidden(
                        controlId: "SavedDashboardPartCalendarSites",
                        alwaysSend: true,
                        value: dashboardCalendarSites)
                    .Hidden(
                        controlId: "ClearDashboardCalendarView",
                        action: "SetSiteSettings",
                        method: "post")
                    .P(
                        id: "DashboardPartCalendarSitesMessage",
                        css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateDashboardPartCalendarSites",
                            text: Displays.OK(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-pencil",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartKambanSitesDialog(
        Context context,
        SiteSettings ss,
        int dashboardPartId,
        string dashboardKambanSites)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DashboardPartKambanSitesEditForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DashboardPartKambanSitesEdit",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.SiteId(context: context),
                        text: dashboardKambanSites,
                        validateRequired: true)
                    .Hidden(
                        controlId: "DashboardPartId",
                        alwaysSend: true,
                        value: dashboardPartId.ToString())
                    .Hidden(
                        controlId: "SavedDashboardPartKambanSites",
                        alwaysSend: true,
                        value: dashboardKambanSites)
                    .Hidden(
                        controlId: "ClearDashboardKambanView",
                        action: "SetSiteSettings",
                        method: "post")
                    .P(
                        id: "DashboardPartKambanSitesMessage",
                        css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateDashboardPartKambanSites",
                            text: Displays.OK(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-pencil",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartIndexSitesDialog(
            Context context,
            SiteSettings ss,
            int dashboardPartId,
            string dashboardIndexSites)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DashboardPartIndexSitesEditForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        controlId: "DashboardPartIndexSitesEdit",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.SiteId(context: context),
                        text: dashboardIndexSites,
                        validateRequired: true)
                    .Hidden(
                        controlId: "DashboardPartId",
                        alwaysSend: true,
                        value: dashboardPartId.ToString())
                    .Hidden(
                        controlId: "SavedDashboardPartIndexSites",
                        alwaysSend: true,
                        value: dashboardIndexSites)
                    .Hidden(
                        controlId: "ClearDashboardIndexView",
                        action: "SetSiteSettings",
                        method: "post")
                    .P(
                        id: "DashboardPartIndexSitesMessage",
                        css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateDashboardPartIndexSites",
                            text: Displays.OK(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-pencil",
                            onClick: "$p.send($(this));",
                            action: "SetSiteSettings",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartDialog(
            Context context,
            SiteSettings ss,
            string controlId,
            DashboardPart dashboardPart)
        {
            var filterVisible = false;
            var sorterVisible = false;
            var indexVisible = false;
            switch (dashboardPart.Type)
            {
                case DashboardPartType.TimeLine:
                    filterVisible = true;
                    sorterVisible = true;
                    break;
                case DashboardPartType.Calendar:
                    filterVisible = true;
                    break;
                case DashboardPartType.Kamban:
                    filterVisible = true;
                    break;
                case DashboardPartType.Index:
                    indexVisible = true;
                    filterVisible = true;
                    sorterVisible = true;
                    break;
            }
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DashboardPartForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .Div(
                        id: "ViewTabsContainer",
                        css: "tab-container",
                        action: () => hb.Ul(
                            id: "ViewTabs",
                            action: () => hb
                                .Li(
                                    id: "DashboardPartGeneralTabControl",
                                    action: () => hb
                                    .A(
                                        href: "#DashboardPartGeneralTabContainer",
                                        text: Displays.General(context: context)))
                                .Li(
                                    id: "DashboardPartViewIndexTabControl",
                                    css: indexVisible ? "" : "hidden",
                                    action: () => hb
                                        .A(
                                            href: "#DashboardPartViewIndexTabContainer",
                                            text: Displays.Index(context: context)))
                                .Li(
                                    id: "DashboardPartViewFiltersTabControl",
                                    css: filterVisible ? "" : "hidden",
                                    action: () => hb
                                        .A(
                                            href: "#DashboardPartViewFiltersTabContainer",
                                            text: Displays.Filters(context: context)))
                                .Li(
                                    id: "DashboardPartViewSortersTabControl",
                                    css: sorterVisible ? "" : "hidden",
                                    action: () => hb
                                        .A(
                                            href: "#DashboardPartViewSortersTabContainer",
                                            text: Displays.Sorters(context: context)))
                                .Li(action: () => hb
                                    .A(
                                        href: "#DashboardPartAccessControlsTab",
                                        text: Displays.AccessControls(context: context))))
                        .DashboardPartGeneralTab(
                            context: context,
                            ss: ss,
                            dashboardPart: dashboardPart,
                            controlId: controlId)
                        .DashboardPartViewIndexTab(
                            context: context,
                            dashboardPart: dashboardPart)
                        .DashboardPartViewFiltersTab(
                            context: context,
                            dashboardPart: dashboardPart)
                        .DashboardPartViewSortersTab(
                            context: context,
                            dashboardPart: dashboardPart)
                        .DashboardPartAccessControlsTab(
                            context: context,
                            ss: ss,
                            dashboardPart: dashboardPart))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddDashboardPart",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setDashboardPart($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewDashboardPart")
                        .Button(
                            controlId: "UpdateDashboardPart",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setDashboardPart($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditDashboardPart")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder DashboardPartGeneralTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string controlId,
            DashboardPart dashboardPart)
        {
            var currentSs = dashboardPart.SiteId > 0
                ? SiteSettingsUtilities.Get(
                    context: context,
                    siteId: dashboardPart.SiteId)
                : ss;
            string hiddenCss(bool hide) => hide ? " hidden" : "";
            return hb
                .TabsPanelField(id: $"DashboardPartGeneralTabContainer", action: () => hb
                    .FieldText(
                        controlId: "DashboardPartId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: dashboardPart.Id.ToString(),
                        _using: controlId == "EditDashboardPart")
                    .FieldTextBox(
                        controlId: "DashboardPartTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: dashboardPart.Title,
                        validateRequired: true)
                    .FieldCheckBox(
                        controlId: "DashboardPartShowTitle",
                        fieldCss: "field-wide",
                        controlContainerCss: "m-l50",
                        controlCss: " always-send",
                        labelText: Displays.DisplayTitle(context: context),
                        _checked: dashboardPart.ShowTitle ?? false)
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartType",
                        controlCss: " always-send",
                        labelText: Displays.PartsType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                DashboardPartType.QuickAccess.ToInt().ToString(),
                                Displays.QuickAccess(context: context)
                            },
                            {
                                DashboardPartType.TimeLine.ToInt().ToString(),
                                Displays.TimeLine(context: context)
                            },
                            {
                                DashboardPartType.Custom.ToInt().ToString(),
                                Displays.DashboardCustom(context: context)
                            },
                            {
                                DashboardPartType.CustomHtml.ToInt().ToString(),
                                Displays.DashboardCustomHtml(context: context)
                            },
                            {
                                DashboardPartType.Calendar.ToInt().ToString(),
                                Displays.Calendar(context: context)
                            },
                            {
                                DashboardPartType.Kamban.ToInt().ToString(),
                                Displays.Kamban(context: context)
                            },
                            {
                                DashboardPartType.Index.ToInt().ToString(),
                                Displays.Index(context: context)
                            }
                        },
                        selectedValue: dashboardPart.Type.ToInt().ToString(),
                        insertBlank: false)
                    .FieldTextBox(
                            controlId: "DashboardPartQuickAccessSites",
                            fieldId: "DashboardPartQuickAccessSitesField",
                            textType: HtmlTypes.TextTypes.MultiLine,
                            fieldCss: "field-wide"
                                + hiddenCss(dashboardPart.Type != DashboardPartType.QuickAccess),
                            controlCss: " always-send",
                            labelText: Displays.SiteId(context: context),
                            text: dashboardPart.QuickAccessSites)
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartQuickAccessLayout",
                        fieldId: "DashboardPartQuickAccessLayoutField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.QuickAccess),
                        labelText: Displays.QuickAccessLayout(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                QuickAccessLayout.Horizontal.ToInt().ToString(),
                                Displays.Horizontal(context: context)
                            },
                            {
                                QuickAccessLayout.Vertical.ToInt().ToString(),
                                Displays.Vertical(context: context)
                            }
                        },
                        selectedValue: dashboardPart.QuickAccessLayout.ToInt().ToString(),
                        insertBlank: false)
                    .Div(
                        id: "DashboardPartTimeLineSitesField",
                        css: "both" + hiddenCss(dashboardPart.Type != DashboardPartType.TimeLine),
                        action: () =>
                        {
                            var timeLineSites = dashboardPart.TimeLineSites;
                            var baseSiteId = DashboardPart.GetBaseSiteSettings(
                                context: context,
                                sitesString: timeLineSites)
                                    ?.SiteId;
                            hb
                                .FieldText(
                                    controlId: "DashboardPartTimeLineSitesValue",
                                    labelText: Displays.SiteId(context: context),
                                    text: timeLineSites)
                                .Hidden(
                                    controlId: "DashboardPartTimeLineSites",
                                    alwaysSend: true,
                                    value: timeLineSites)
                                .Hidden(
                                    controlId: "DashboardPartBaseSiteId",
                                    alwaysSend: true,
                                    value: baseSiteId == null
                                        ? null
                                        : baseSiteId.ToString())
                                .Button(
                                        controlId: "EditTimeLineSites",
                                        text: Displays.Edit(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openDashboardPartTimeLineSitesDialog($(this));",
                                        icon: "ui-icon-pencil",
                                        action: "SetSiteSettings",
                                        method: "post");
                        })
                    .FieldTextBox(
                        controlId: "DashboardPartTimeLineTitle",
                        fieldId: "DashboardPartTimeLineTitleField",
                        fieldCss: "field-wide" + hiddenCss(dashboardPart.Type != DashboardPartType.TimeLine),
                        controlCss: " always-send",
                        labelText: Displays.RecordTitle(context: context),
                        text: dashboardPart.TimeLineTitle.IsNullOrEmpty()
                            ? $"[Title]"
                            : SiteSettingsUtilities.Get(context: context, dashboardPart.SiteId)?
                                .ColumnNameToLabelText(dashboardPart.TimeLineTitle))
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.MultiLine,
                        controlId: "DashboardPartTimeLineBody",
                        fieldId: "DashboardPartTimeLineBodyField",
                        fieldCss: "field-wide" + hiddenCss(dashboardPart.Type != DashboardPartType.TimeLine),
                        controlCss: " always-send",
                        labelText: Displays.RecordBody(context: context),
                        text: dashboardPart.TimeLineBody.IsNullOrEmpty()
                            ? $"[Body]"
                            : SiteSettingsUtilities.Get(context: context, dashboardPart.SiteId)?
                                .ColumnNameToLabelText(dashboardPart.TimeLineBody))
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartTimeLineDisplayType",
                        fieldId: "DashboardPartTimeLineDisplayTypeField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.TimeLine),
                        labelText: Displays.TimeLineDisplayType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                TimeLineDisplayType.Simple.ToInt().ToString(),
                                Displays.TimeLineSimple(context: context)
                            },
                            {
                                TimeLineDisplayType.Standard.ToInt().ToString(),
                                Displays.TimeLineStandard(context: context)
                            },
                            {
                                TimeLineDisplayType.Detailed.ToInt().ToString(),
                                Displays.TimeLineDetailed(context: context)
                            }
                        },
                        selectedValue: dashboardPart.TimeLineDisplayType.ToInt().ToString(),
                        insertBlank: false)
                    .FieldSpinner(
                        controlId: "DashboardPartTimeLineItemCount",
                        fieldId: "DashboardPartTimeLineItemCountField",
                        fieldCss: "field-auto" + hiddenCss(dashboardPart.Type != DashboardPartType.TimeLine),
                        controlCss: " always-send",
                        labelText: Displays.DisplayCount(context: context),
                        value: dashboardPart.TimeLineItemCount == 0
                            ? Parameters.Dashboard.TimeLineItemCount
                            : dashboardPart.TimeLineItemCount,
                        min: Parameters.Dashboard.TimeLineItemCountMin,
                        max: Parameters.Dashboard.TimeLineItemCountMax,
                        step: 1)
                    .FieldMarkDown(
                        context: context,
                        ss: ss,
                        controlId: "DashboardPartContent",
                        fieldId: "DashboardPartContentField",
                        fieldCss: "field-wide"
                            + hiddenCss(dashboardPart.Type != DashboardPartType.Custom),
                        controlCss: " always-send",
                        labelText: Displays.Body(context: context),
                        text: dashboardPart.Content,
                        mobile: context.Mobile)
                    .FieldCodeEditor(
                        context: context,
                        controlId: "DashboardPartHtmlContent",
                        fieldId: "DashboardPartHtmlContentField",
                        fieldCss: "field-wide"
                            + hiddenCss(dashboardPart.Type != DashboardPartType.CustomHtml),
                        controlCss: " always-send",
                        dataLang: "html",
                        labelText: Displays.Body(context: context),
                        text: dashboardPart.HtmlContent)
                    .Div(
                        id: "DashboardPartCalendarSitesField",
                        css: "both" + hiddenCss(dashboardPart.Type != DashboardPartType.Calendar),
                        action: () =>
                        {
                            var calendarSites = dashboardPart.CalendarSites;
                            var baseSiteId = DashboardPart.GetBaseSiteSettings(
                                context: context,
                                sitesString: calendarSites)
                                    ?.SiteId;
                            hb
                                .FieldText(
                                    controlId: "DashboardPartCalendarSitesValue",
                                    labelText: Displays.SiteId(context: context),
                                    text: calendarSites)
                                .Hidden(
                                    controlId: "DashboardPartCalendarSites",
                                    alwaysSend: true,
                                    value: calendarSites)
                                .Hidden(
                                    controlId: "DashboardPartCalendarBaseSiteId",
                                    alwaysSend: true,
                                    value: baseSiteId == null
                                        ? null
                                        : baseSiteId.ToString())
                                .Button(
                                        controlId: "EditCalendarSites",
                                        text: Displays.Edit(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openDashboardPartCalendarSitesDialog($(this));",
                                        icon: "ui-icon-pencil",
                                        action: "SetSiteSettings",
                                        method: "post");
                        })
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartCalendarType",
                        fieldId: "DashboardPartCalendarTypeField",
                        controlCss: " always-send",
                        fieldCss: "both field-normal" + hiddenCss(dashboardPart.Type != DashboardPartType.Calendar),
                        labelText: Displays.CalendarType(context: context),
                        optionCollection: new Dictionary<string, string>
                        {
                            {
                                SiteSettings.CalendarTypes.Standard.ToInt().ToString(),
                                Displays.Standard(context: context)
                            },
                            {
                                SiteSettings.CalendarTypes.FullCalendar.ToInt().ToString(),
                                Displays.FullCalendar(context: context)
                            }
                        },
                        selectedValue: dashboardPart.CalendarType?.ToInt().ToString() ?? Parameters.General.DefaultCalendarType.ToString(),
                        insertBlank: false)
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartCalendarGroupBy",
                        fieldId: "DashboardPartCalendarGroupByField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Calendar || dashboardPart.CalendarType == SiteSettings.CalendarTypes.FullCalendar),
                        labelText: Displays.GroupBy(context: context),
                        optionCollection: currentSs.CalendarGroupByOptions(context: context),
                        selectedValue: dashboardPart.CalendarGroupBy?.ToString(),
                        insertBlank: true)
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartCalendarTimePeriod",
                        fieldId: "DashboardPartCalendarTimePeriodField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Calendar || dashboardPart.CalendarType == SiteSettings.CalendarTypes.FullCalendar),
                        labelText: Displays.Period(context: context),
                        optionCollection: currentSs.CalendarTimePeriodOptions(context: context),
                        selectedValue: !dashboardPart.CalendarTimePeriod.IsNullOrEmpty()
                            ? dashboardPart.CalendarTimePeriod.ToString()
                            : "Monthly",
                        insertBlank: false)
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartCalendarFromTo",
                        fieldId: "DashboardPartCalendarFromToField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Calendar),
                        labelText: Displays.Column(context: context),
                        optionCollection: currentSs.CalendarColumnOptions(context: context),
                        selectedValue: !dashboardPart.CalendarFromTo.IsNullOrEmpty()
                            ? dashboardPart.CalendarFromTo.ToString()
                            : "StartTime-CompletionTime",
                        insertBlank: false)
                    .FieldCheckBox(
                        controlId: "CalendarShowStatus",
                        fieldId: "DashboardPartCalendarShowStatusField",
                        controlCss: " always-send",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Calendar),
                        labelText: Displays.ShowStatus(context: context),
                        _checked: dashboardPart.CalendarShowStatus == true)
                    .Div(
                        id: "DashboardPartKambanSitesField",
                        css: "both" + hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        action: () =>
                        {
                            var kambanSites = dashboardPart.KambanSites;
                            var baseSiteId = DashboardPart.GetBaseSiteSettings(
                                context: context,
                                sitesString: kambanSites)
                                    ?.SiteId;
                            hb
                                .FieldText(
                                    controlId: "DashboardPartKambanSitesValue",
                                    labelText: Displays.SiteId(context: context),
                                    text: kambanSites)
                                .Hidden(
                                    controlId: "DashboardPartKambanSites",
                                    alwaysSend: true,
                                    value: kambanSites)
                                .Hidden(
                                    controlId: "DashboardPartKambanBaseSiteId",
                                    alwaysSend: true,
                                    value: baseSiteId == null
                                        ? null
                                        : baseSiteId.ToString())
                                .Button(
                                        controlId: "EditKambanSites",
                                        text: Displays.Edit(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openDashboardPartKambanSitesDialog($(this));",
                                        icon: "ui-icon-pencil",
                                        action: "SetSiteSettings",
                                        method: "post");
                        })
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartKambanGroupByX",
                        fieldId: "DashboardPartKambanGroupByXField",
                        fieldCss: "both field-normal" + hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.GroupByX(context: context),
                        optionCollection: currentSs.KambanGroupByOptions(context: context),
                        selectedValue: !dashboardPart.KambanGroupByX.IsNullOrEmpty()
                            ? dashboardPart.KambanGroupByX
                            : "Status")
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartKambanGroupByY",
                        fieldId: "DashboardPartKambanGroupByYField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.GroupByY(context: context),
                        optionCollection: currentSs.KambanGroupByOptions(
                            context: context,
                            addNothing: true),
                        selectedValue: !dashboardPart.KambanGroupByY.IsNullOrEmpty()
                            ? dashboardPart.KambanGroupByY
                            : "Owner")
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartKambanAggregateType",
                        fieldId: "DashboardPartKambanAggregateTypeField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: currentSs.KambanAggregationTypeOptions(context: context),
                        selectedValue: !dashboardPart.KambanAggregateType.IsNullOrEmpty()
                            ? dashboardPart.KambanAggregateType
                            : "Total")
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartKambanValue",
                        fieldId: "DashboardPartKambanValueField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban || dashboardPart.KambanAggregateType == "Count"),
                        controlCss: " always-send",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: currentSs.KambanValueOptions(context: context),
                        selectedValue: !dashboardPart.KambanValue.IsNullOrEmpty()
                            ? dashboardPart.KambanValue
                            : ss.ReferenceType == "Issues" ? "RemainingWorkValue" : "NumA")
                    .FieldDropDown(
                        context: context,
                        controlId: "DashboardPartKambanColumns",
                        fieldId: "DashboardPartKambanColumnsField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.MaxColumns(context: context),
                        optionCollection: Enumerable.Range(
                            Parameters.General.KambanMinColumns,
                            Parameters.General.KambanMaxColumns - Parameters.General.KambanMinColumns + 1)
                                .ToDictionary(o => o.ToString(), o => o.ToString()),
                        selectedValue: !dashboardPart.KambanColumns.IsNullOrEmpty()
                            ? dashboardPart.KambanColumns
                            : "10")
                    .FieldCheckBox(
                        controlId: "DashboardPartKambanAggregationView",
                        fieldId: "DashboardPartKambanAggregationViewField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.AggregationView(context: context),
                        _checked: dashboardPart.KambanAggregationView == true)
                    .FieldCheckBox(
                        controlId: "DashboardPartKambanShowStatus",
                        fieldId: "DashboardPartKambanShowStatusField",
                        fieldCss: hiddenCss(dashboardPart.Type != DashboardPartType.Kamban),
                        controlCss: " always-send",
                        labelText: Displays.ShowStatus(context: context),
                        _checked: dashboardPart.KambanShowStatus == true)
                    .Div(
                        id: "DashboardPartIndexSitesField",
                        css: "both" + hiddenCss(dashboardPart.Type != DashboardPartType.Index),
                        action: () =>
                        {
                            var indexSites = dashboardPart.IndexSites;
                            var baseSiteId = DashboardPart.GetBaseSiteSettings(
                                context: context,
                                sitesString: indexSites)
                                    ?.SiteId;
                            hb
                                .FieldText(
                                    controlId: "DashboardPartIndexSitesValue",
                                    labelText: Displays.SiteId(context: context),
                                    text: indexSites)
                                .Hidden(
                                    controlId: "DashboardPartIndexSites",
                                    alwaysSend: true,
                                    value: indexSites)
                                .Hidden(
                                    controlId: "DashboardPartIndexBaseSiteId",
                                    alwaysSend: true,
                                    value: baseSiteId == null
                                        ? null
                                        : baseSiteId.ToString())
                                .Button(
                                        controlId: "EditIndexSites",
                                        text: Displays.Edit(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.openDashboardPartIndexSitesDialog($(this));",
                                        icon: "ui-icon-pencil",
                                        action: "SetSiteSettings",
                                        method: "post");
                        })
                    .FieldCheckBox(
                        controlId: "DisableAsynchronousLoading",
                        fieldCss: " both",
                        controlCss: " always-send control-checkbox",
                        labelText: Displays.DisableAsynchronousLoading(context: context),
                        _checked: dashboardPart.DisableAsynchronousLoading == true)
                    .FieldTextBox(
                        controlId: "DashboardPartExtendedCss",
                        controlCss: " always-send",
                        labelText: "CSS",
                        text: dashboardPart.ExtendedCss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartViewIndexTab(
            this HtmlBuilder hb,
            Context context,
            DashboardPart dashboardPart,
            bool clearView = false,
            bool _using = true)
        {
            if (_using == false) return hb;
            var view = clearView
                ? new View()
                : (dashboardPart.View ?? new View());
            var currentSs = SiteSettingsUtilities.Get(
                context: context,
                dashboardPart.SiteId);
            if (currentSs == null)
            {
                return hb.TabsPanelField(
                    id: "DashboardPartViewIndexTabContainer",
                    innerId: "DashboardPartViewGridTabContainer"
                );
            }
            return hb.TabsPanelField(
                id: "DashboardPartViewIndexTabContainer",
                innerId: "DashboardPartViewGridTabContainer",
                action: () => hb.ViewGridTab(
                    context: context,
                    ss: currentSs,
                    view: view,
                    prefix: "DashboardPart",
                    hasNotInner: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartViewFiltersTab(
            this HtmlBuilder hb,
            Context context,
            DashboardPart dashboardPart,
            bool clearView = false,
            bool _using = true)
        {
            if (_using == false) return hb;
            var view = clearView
                ? new View()
                : (dashboardPart.View ?? new View());
            var currentSs = SiteSettingsUtilities.Get(
                context: context,
                dashboardPart.SiteId);
            if (currentSs == null)
            {
                return hb.TabsPanelField(id: "DashboardPartViewFiltersTabContainer");
            }
            return hb.TabsPanelField(id: "DashboardPartViewFiltersTabContainer",
                action: () => hb.ViewFiltersTab(
                    context: context,
                    ss: currentSs,
                    view: view,
                    prefix: "DashboardPart",
                    currentTableOnly: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder DashboardPartViewSortersTab(
            this HtmlBuilder hb,
            Context context,
            DashboardPart dashboardPart,
            bool clearView = false,
            bool _using = true)
        {
            if (_using == false) return hb;
            var view = clearView
                ? new View()
                : (dashboardPart.View ?? new View());
            var ss = SiteSettingsUtilities.Get(
                context: context,
                dashboardPart.SiteId);
            if (ss == null)
            {
                return hb.TabsPanelField(id: "DashboardPartViewSortersTabContainer");
            }
            return hb.TabsPanelField(id: "DashboardPartViewSortersTabContainer",
                action: () => hb.ViewSortersTab(
                    context: context,
                    ss: ss,
                    view: view,
                    prefix: "DashboardPart",
                    usekeepSorterState: false,
                    currentTableOnly: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder DashboardPartAccessControlsTab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DashboardPart dashboardPart)
        {
            var currentPermissions = dashboardPart.GetPermissions(ss: ss);
            var sourcePermissions = PermissionUtilities.SourceCollection(
                context: context,
                ss: ss,
                searchText: context.Forms.Data("SearchDashboardPartAccessControlElements"),
                currentPermissions: currentPermissions,
                allUsers: false);
            var offset = context.Forms.Int("SourceDashboardPartAccessControlOffset");
            return hb.TabsPanelField(id: "DashboardPartAccessControlsTab", action: () => hb
                .Div(id: "DashboardPartAccessControlEditor", action: () => hb
                    .FieldSelectable(
                        controlId: "CurrentDashboardPartAccessControl",
                        fieldCss: "field-vertical both",
                        controlContainerCss: "container-selectable",
                        controlCss: " always-send send-all",
                        labelText: Displays.Permissions(context: context),
                        listItemCollection: currentPermissions.ToDictionary(
                            o => o.Key(), o => o.ControlData(
                                context: context,
                                ss: ss,
                                withType: false)),
                        commandOptionPositionIsTop: true,
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.DeletePermission(context: context),
                                    onClick: "$p.deleteDashboardPartAccessControl();",
                                    icon: "ui-icon-circle-triangle-e")))
                    .FieldSelectable(
                        controlId: "SourceDashboardPartAccessControl",
                        fieldCss: "field-vertical",
                        controlContainerCss: "container-selectable",
                        controlWrapperCss: " h300",
                        labelText: Displays.OptionList(context: context),
                        listItemCollection: sourcePermissions
                            .Page(offset)
                            .ListItemCollection(
                                context: context,
                                ss: ss,
                                withType: false),
                        commandOptionPositionIsTop: true,
                        action: "Permissions",
                        method: "post",
                        commandOptionAction: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlCss: "button-icon",
                                    text: Displays.AddPermission(context: context),
                                    onClick: "$p.addDashboardPartAccessControl();",
                                    icon: "ui-icon-circle-triangle-w")
                                .TextBox(
                                    controlId: "SearchDashboardPartAccessControl",
                                    controlCss: " auto-postback w100",
                                    placeholder: Displays.Search(context: context),
                                    action: "SetSiteSettings",
                                    method: "post")
                                .Button(
                                    text: Displays.Search(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.send($('#SearchDashboardPartAccessControl'));",
                                    icon: "ui-icon-search")))
                    .Hidden(
                        controlId: "SourceDashboardPartAccessControlOffset",
                        css: "always-send",
                        value: Paging.NextOffset(
                            offset: offset,
                            totalCount: sourcePermissions.Count(),
                            pageSize: Parameters.Permissions.PageSize)
                                .ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string GetClassLabelText(SiteSettings ss, string className)
        {
            return (ss?.ColumnHash?.FirstOrDefault(o => o.Key == className))?.Value?.LabelText ?? className;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder BulkUpdateColumnDialog(
            Context context, SiteSettings ss, string controlId, BulkUpdateColumn bulkUpdateColumn)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("BulkUpdateColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "BulkUpdateColumnId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: bulkUpdateColumn.Id.ToString(),
                        _using: controlId == "EditBulkUpdateColumns")
                    .FieldTextBox(
                        controlId: "BulkUpdateColumnTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: bulkUpdateColumn.Title,
                        validateRequired: true)
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.BulkUpdateColumnSettings(context: context),
                        action: () => hb
                            .FieldSelectable(
                                controlId: "BulkUpdateColumnColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h350",
                                controlCss: " always-send send-all",
                                labelText: Displays.CurrentSettings(context: context),
                                listItemCollection: ss.BulkUpdateColumnSelectableOptions(
                                    context: context,
                                    id: bulkUpdateColumn.Id),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "MoveUpBulkUpdateColumnColumnsLocal",
                                            text: Displays.MoveUp(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'BulkUpdateColumn');",
                                            icon: "ui-icon-circle-triangle-n")
                                        .Button(
                                            controlId: "MoveDownBulkUpdateColumnColumnsLocal",
                                            text: Displays.MoveDown(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'BulkUpdateColumn');",
                                            icon: "ui-icon-circle-triangle-s")
                                        .Button(
                                            controlId: "OpenBulkUpdateColumnDetailDialog",
                                            text: Displays.AdvancedSetting(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.openBulkUpdateColumnDetailDialog($(this));",
                                            icon: "ui-icon-circle-triangle-s",
                                            action: "SetSiteSettings",
                                            method: "post")
                                        .Button(
                                            controlId: "ToDisableBulkUpdateColumnColumnsLocal",
                                            text: Displays.ToDisable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'BulkUpdateColumn');",
                                            icon: "ui-icon-circle-triangle-e")))
                            .FieldSelectable(
                                controlId: "BulkUpdateColumnSourceColumns",
                                fieldCss: "field-vertical",
                                controlContainerCss: "container-selectable",
                                controlWrapperCss: " h350",
                                labelText: Displays.OptionList(context: context),
                                listItemCollection: ss.BulkUpdateColumnSelectableOptions(
                                    context: context,
                                    id: bulkUpdateColumn.Id,
                                    enabled: false),
                                commandOptionPositionIsTop: true,
                                commandOptionAction: () => hb
                                    .Div(css: "command-center", action: () => hb
                                        .Button(
                                            controlId: "ToEnableBulkUpdateColumnColumnsLocal",
                                            text: Displays.ToEnable(context: context),
                                            controlCss: "button-icon",
                                            onClick: "$p.moveColumns(event, $(this),'BulkUpdateColumn');",
                                            icon: "ui-icon-circle-triangle-w"))))
                    .Hidden(
                        controlId: "BulkUpdateColumnDetails",
                        css: " always-send",
                        value: bulkUpdateColumn.Details.ToJson())
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddBulkUpdateColumn",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setBulkUpdateColumn($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewBulkUpdateColumn")
                        .Button(
                            controlId: "UpdateBulkUpdateColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setBulkUpdateColumn($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditBulkUpdateColumns")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder BulkUpdateColumnDetailDialog(
            Context context,
            SiteSettings ss,
            Column column,
            BulkUpdateColumnDetail detail)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("BulkUpdateColumnDetailForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldCheckBox(
                        controlId: "BulkUpdateColumnDetailValidateRequired",
                        controlCss: " always-send",
                        labelText: Displays.Required(context: context),
                        _checked: detail.ValidateRequired ?? column.ValidateRequired ?? false,
                        disabled: column.Required,
                        _using: !column.Id_Ver
                            && !column.NotUpdate
                            && column.TypeName != "bit")
                    .FieldCheckBox(
                        controlId: "BulkUpdateColumnDetailEditorReadOnly",
                        labelText: Displays.ReadOnly(context: context),
                        _checked: detail.EditorReadOnly ?? column.EditorReadOnly ?? false)
                    .BulkUpdateColumnDetailDefaultInput(
                        context: context,
                        column: column,
                        detail: detail)
                    .Hidden(
                        controlId: "BulkUpdateColumnDetailColumnName",
                        css: " always-send",
                        value: column.ColumnName)
                    .Hidden(
                        controlId: "BulkUpdateColumnDetailsTemp",
                        css: " always-send",
                        value: context.Forms.Data("BulkUpdateColumnDetails"))
                    .P(id: "BulkUpdateColumnDetailMessage", css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "UpdateBulkUpdateColumnDetail",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setBulkUpdateColumnDetail($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder BulkUpdateColumnDetailDefaultInput(
            this HtmlBuilder hb,
            Context context,
            Column column,
            BulkUpdateColumnDetail detail)
        {
            var type = column.TypeName.CsTypeSummary();
            var controlId = "BulkUpdateColumnDetailDefaultInput";
            switch (type)
            {
                case Types.CsBool:
                    hb.FieldCheckBox(
                        controlId: controlId,
                        controlCss: " always-send",
                        labelText: Displays.DefaultInput(context: context),
                        _checked: detail.DefaultInput.ToBool(),
                        _using: !column.NotUpdate);
                    break;
                case Types.CsNumeric:
                    if (column.ControlType == "ChoicesText")
                    {
                        hb.FieldTextBox(
                            controlId: controlId,
                            controlCss: " always-send",
                            labelText: Displays.DefaultInput(context: context),
                            text: detail.DefaultInput,
                            _using: !column.Id_Ver);
                    }
                    else
                    {
                        hb.FieldTextBox(
                            controlId: controlId,
                            controlCss: " always-send",
                            labelText: Displays.DefaultInput(context: context),
                            text: !detail.DefaultInput.IsNullOrEmpty()
                                ? detail.DefaultInput.ToLong().ToString()
                                : string.Empty,
                            validateNumber: true,
                            _using: !column.Id_Ver
                                && !column.NotUpdate);
                    }
                    break;
                case Types.CsString:
                    hb.FieldTextBox(
                        textType: column.ControlType == "MarkDown"
                            ? HtmlTypes.TextTypes.MultiLine
                            : HtmlTypes.TextTypes.Normal,
                        controlId: controlId,
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.DefaultInput(context: context),
                        text: detail.DefaultInput,
                        _using: column.ColumnName != "Comments"
                            && column.ControlType != "Attachments"
                            && !column.NotUpdate);
                    break;
                case Types.CsDateTime:
                    hb.FieldSpinner(
                        controlId: controlId,
                        controlCss: " allow-blank always-send",
                        labelText: Displays.DefaultInput(context: context),
                        value: !detail.DefaultInput.IsNullOrEmpty()
                            ? detail.DefaultInput.ToDecimal()
                            : (decimal?)null,
                        min: column.Min.ToInt(),
                        max: column.Max.ToInt(),
                        step: column.Step.ToInt(),
                        width: column.Width,
                        _using: !column.NotUpdate);
                    break;
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder RelatingColumnDialog(
            Context context, SiteSettings ss, string controlId, RelatingColumn relatingColumn)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("RelatingColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "RelatingColumnId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: relatingColumn.Id.ToString(),
                        _using: controlId == "EditRelatingColumns")
                    .FieldTextBox(
                        controlId: "RelatingColumnTitle",
                        fieldCss: "field-wide",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context),
                        text: relatingColumn.Title,
                        validateRequired: true)
                .FieldSet(
                    css: " enclosed",
                    legendText: Displays.RelatingColumnSettings(context: context),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "RelatingColumnColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            controlCss: " always-send send-all",
                            labelText: Displays.CurrentSettings(context: context),
                            listItemCollection: ss.RelatingColumnSelectableOptions(
                                context: context,
                                id: relatingColumn.Id),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "MoveUpRelatingColumnColumnsLocal",
                                        text: Displays.MoveUp(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-n")
                                    .Button(
                                        controlId: "MoveDownRelatingColumnColumnsLocal",
                                        text: Displays.MoveDown(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-s")
                                    .Button(
                                        controlId: "ToDisableRelatingColumnColumnsLocal",
                                        text: Displays.ToDisable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-e")))
                        .FieldSelectable(
                            controlId: "RelatingColumnSourceColumns",
                            fieldCss: "field-vertical",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h350",
                            labelText: Displays.OptionList(context: context),
                            listItemCollection: ss.RelatingColumnSelectableOptions(
                                context: context,
                                id: relatingColumn.Id,
                                enabled: false),
                            commandOptionPositionIsTop: true,
                            commandOptionAction: () => hb
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        controlId: "ToEnableRelatingColumnColumnsLocal",
                                        text: Displays.ToEnable(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.moveColumns(event, $(this),'RelatingColumn');",
                                        icon: "ui-icon-circle-triangle-w"))))
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddRelatingColumn",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setRelatingColumn($(this));",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "NewRelatingColumn")
                        .Button(
                            controlId: "UpdateRelatingColumn",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.setRelatingColumn($(this));",
                            icon: "ui-icon-disk",
                            action: "SetSiteSettings",
                            method: "post",
                            _using: controlId == "EditRelatingColumns")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string LockTable(Context context, SiteSettings ss)
        {
            if (ss.AllowLockTable != true)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var invalid = SiteValidators.OnLockTable(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateSites(
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId),
                    param: Rds.SitesParam()
                        .LockedTime(DateTime.Now)
                        .LockedUser(context.UserId)));
            return new ResponseCollection(context: context)
                .Href(Locations.ItemIndex(
                    context: context,
                    id: ss.SiteId))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string UnlockTable(Context context, SiteSettings ss)
        {
            var invalid = SiteValidators.OnUnlockTable(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateSites(
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId),
                    param: Rds.SitesParam()
                        .LockedTime(DateTime.Now)
                        .LockedUser(raw: "null")));
            return new ResponseCollection(context: context)
                .Href(Locations.ItemIndex(
                    context: context,
                    id: ss.SiteId))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ForceUnlockTable(Context context, SiteSettings ss)
        {
            var invalid = SiteValidators.OnForceUnlockTable(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateSites(
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId),
                    param: Rds.SitesParam()
                        .LockedTime(DateTime.Now)
                        .LockedUser(raw: "null")));
            return new ResponseCollection(context: context)
                .Href(Locations.ItemIndex(
                    context: context,
                    id: ss.SiteId))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FormulaCalculationMethod(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string target)
        {
            hb.FieldDropDown(
                context: context,
                controlId: "FormulaTarget",
                controlCss: " always-send",
                labelText: Displays.Target(context: context),
                optionCollection: ss.FormulaTargetSelectableOptions(target));
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder SearchEditorColumnDialog(
            Context context,
            SiteSettings ss)
        {
            var dialogInput = context.Forms.Data("SearchEditorColumnDialogInput")
                .Deserialize<Dictionary<string, string>>();
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("SearchEditorColumnForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId))
                    .DataEnter("#ShowTargetColumnKeyWord"),
                action: () => hb
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.Search(context: context),
                        action: () => hb
                            .FieldTextBox(
                                controlId: "TargetColumnKeyWord",
                                text: dialogInput == null ? "" : dialogInput.Get("keyWord"),
                                controlCss: "control-textbox always-send")
                            .Button(
                                controlId: "ShowTargetColumnKeyWord",
                                text: Displays.Search(context: context),
                                controlCss: "button-icon button-positive",
                                onClick: "$p.selectSearchEditorColumn('KeyWord');"))
                    .FieldSet(
                        css: " enclosed",
                        legendText: Displays.UseSearchFilter(context: context),
                        action: () => hb
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ShowTargetColumnBasic",
                                    text: Displays.Basic(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Basic');")
                                .Button(
                                    controlId: "ShowTargetColumnClass",
                                    text: Displays.Class(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Class');")
                                .Button(
                                    controlId: "ShowTargetColumnNum",
                                    text: Displays.Num(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Num');"))
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ShowTargetColumnDate",
                                    text: Displays.Date(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Date');")
                                .Button(
                                    controlId: "ShowTargetColumnDescription",
                                    text: Displays.Description(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Description');")
                                .Button(
                                    controlId: "ShowTargetColumnCheck",
                                    text: Displays.Check(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Check');"))
                            .Div(css: "command-left", action: () => hb
                                .Button(
                                    controlId: "ShowTargetColumnAttachments",
                                    text: Displays.Attachments(context: context),
                                    controlCss: "button-icon w150",
                                    onClick: "$p.selectSearchEditorColumn('Attachments');")))
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ShowTargetColumnDefault",
                            text: Displays.Reset(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.selectSearchEditorColumn('');",
                            icon: "ui-icon-gear")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetClosestSiteIdByApi(Context context, long id)
        {
            var findSiteNames = context.RequestDataString.Deserialize<SiteApiModel>()?.FindSiteNames;
            if (findSiteNames == null)
            {
                return ApiResults.BadRequest(context: context);
            }
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var resultCollection = new List<object>();
            var startSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: id,
                referenceId: id);
            var startCanRead = context.CanRead(ss: startSs, site: true)
                || context.CanCreate(ss: startSs, site: true);
            var tenantCache = SiteInfo.TenantCaches[context.TenantId];
            foreach (var siteName in findSiteNames)
            {
                if (siteName.IsNullOrEmpty() || startCanRead == false)
                {
                    resultCollection.Add(new { SiteName = siteName, SiteId = -1 });
                }
                else
                {
                    var foundId = tenantCache.SiteNameTree.Find(
                        startId: id,
                        name: siteName);
                    if (foundId != -1)
                    {
                        var findSs = SiteSettingsUtilities.Get(
                            context: context,
                            siteId: foundId,
                            referenceId: foundId);
                        var findCanRead = context.CanRead(ss: findSs, site: true)
                            || context.CanCreate(ss: findSs, site: true);
                        if (findCanRead == false)
                        {
                            foundId = -1;
                        }
                    }
                    resultCollection.Add(new { SiteName = siteName, SiteId = foundId });
                }
            }
            return ApiResults.Get(apiResponse: new
            {
                // 正しいAPIの戻り値はResponseの中にSiteId,Dataを入れるべきだが、既にリリースしている為にStatusCodeのみを追加する。
                StatusCode = 200,
                SiteId = id,
                Data = resultCollection
            }.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ImportUserTemplateDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, TemplateDefinition template = null)
        {
            var type = template == null ? "Import" : "Update";
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id(type + "UserTemplateForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.File,
                        controlId: type + "UserTemplate_Import",
                        fieldCss: "field-wide",
                        labelText: Displays.SitePackage(context: context),
                        validateRequired: true,
                        _using: template == null)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Normal,
                        controlId: type + "UserTemplate_Title",
                        fieldCss: "field-wide",
                        labelText: Displays.Title(context: context),
                        text: template?.Title,
                        alwaysSend: true,
                        validateRequired: true)
                    .FieldMarkDown(
                        context: context,
                        ss: null,
                        controlId: type + "UserTemplate_Description",
                        fieldCss: "field-wide",
                        labelText: Displays.Description(context: context),
                        text: template?.Description,
                        alwaysSend: true,
                        allowImage: false)
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.Import(context: context),
                                controlCss: "button-icon button-positive validate",
                                onClick: "$p.importUserTemplate($(this));",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                action: "ImportUserTemplate",
                                method: "post",
                                _using: template == null)
                            .Button(
                                text: Displays.Update(context: context),
                                controlCss: "button-icon button-positive validate",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-copy",
                                action: "UpdateUserTemplate",
                                method: "post",
                                _using: template != null)
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"))
                    .Hidden(
                        controlId: type + "UserTemplate_Id",
                        css: "always-send",
                        value: template?.Id,
                        _using: template != null)
                    .Hidden(
                        controlId: type + "UserTemplate_SearchText",
                        css: "always-send",
                        value: context.Forms.Data("SearchText"),
                        _using: template != null));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder CreateUserTemplateDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("CreateUserTemplateDialog")
                    .Class("dialog")
                    .Title(Displays.CustomApps(context: context)),
                action: () => hb
                    .Div(
                        action: () => hb
                            .FieldText(
                                controlId: "CreateUserTemplate_Title",
                                controlCss: " focus always-send",
                                labelText: Displays.Title(context: context))
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "CreateUserTemplate",
                                    text: Displays.Create(context: context),
                                    controlCss: "button-icon validate button-positive",
                                    onClick: "$p.send($(this), 'SiteTitleForm');",
                                    icon: "ui-icon-gear",
                                    action: "CreateByTemplate",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon button-neutral",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }
    }
}
