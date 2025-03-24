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
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
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
            var invalid = DeptValidators.OnEntry(
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
                referenceType: "Depts",
                title: Displays.Depts(context: context) + " - " + Displays.List(context: context),
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
                    value: gridData.DataRows.Select(g => g.Long("DeptId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("DeptId")).ToJson())
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
                ? Rds.DeptsWhere().DeptId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.DeptsWhere().DeptId_In(
                    value: recordSelector.Selected?.Select(o => o.ToInt()) ?? new List<int>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long deptId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.DeptsWhere().DeptId(deptId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: deptId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{deptId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + deptId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("DeptId")}\"][data-latest]",
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
                            idColumn: "DeptId"))
                    .Messages(context.Messages)
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            DeptModel deptModel,
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
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptId,
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
                                    value: deptModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptCode,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "DeptName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptName,
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
                                    value: deptModel.Body,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Disabled":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.Disabled,
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
                                    value: deptModel.Comments,
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
                                    value: deptModel.Creator,
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
                                    value: deptModel.Updator,
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
                                    value: deptModel.CreatedTime,
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
                                    value: deptModel.UpdatedTime,
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
                                            value: deptModel.GetClass(columnName: column.Name),
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
                                            value: deptModel.GetNum(columnName: column.Name),
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
                                            value: deptModel.GetDate(columnName: column.Name),
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
                                            value: deptModel.GetDescription(columnName: column.Name),
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
                                            value: deptModel.GetCheck(columnName: column.Name),
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
                                            value: deptModel.GetAttachments(columnName: column.Name),
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
                    case "DeptName": value = deptModel.DeptName.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = deptModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "Disabled": value = deptModel.Disabled.GridText(
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
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = deptModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = deptModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = deptModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = deptModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = deptModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = deptModel.GetAttachments(columnName: column.Name).GridText(
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
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts(context: context) + " - " + Displays.New(context: context)
                    : deptModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        deptModel: deptModel)).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, DeptModel deptModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType =  Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: deptModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
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
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
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
                                    verType: deptModel.VerType,
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
                            controlId: "Ver",
                            value: deptModel.Ver.ToString())
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
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
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
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb.FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    deptModel: deptModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    deptModel: deptModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: deptModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = deptModel.ControlValue(
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
                        baseModel: deptModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this DeptModel deptModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "DeptId":
                    return deptModel.DeptId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return deptModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "DeptCode":
                    return deptModel.DeptCode
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "DeptName":
                    return deptModel.DeptName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return deptModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Disabled":
                    return deptModel.Disabled
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return deptModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return deptModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return deptModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return deptModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return deptModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return deptModel.GetAttachments(columnName: column.Name)
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
                context, ss, deptId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            Message message = null,
            string switchTargets = null)
        {
            deptModel.MethodType = deptModel.DeptId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new DeptsResponseCollection(
                context: context,
                deptModel: deptModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, deptModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int deptId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.DeptsWhere().TenantId(context.TenantId));
            var param = view.Param(
                context: context,
                ss: ss);
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
            var switchTargets = new List<int>();
            if (Parameters.General.SwitchTargetsLimit > 0)
            {
                if (Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectDepts(
                            column: Rds.DeptsColumn().DeptId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["DeptId"].ToInt())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(deptId))
            {
                switchTargets.Add(deptId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: deptModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = deptModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Depts_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                deptModel: deptModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "DeptId":
                                res.Val(
                                    target: "#Depts_DeptId" + idSuffix,
                                    value: deptModel.DeptId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DeptCode":
                                res.Val(
                                    target: "#Depts_DeptCode" + idSuffix,
                                    value: deptModel.DeptCode.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DeptName":
                                res.Val(
                                    target: "#Depts_DeptName" + idSuffix,
                                    value: deptModel.DeptName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Depts_Body" + idSuffix,
                                    value: deptModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Disabled":
                                res.Val(
                                    target: "#Depts_Disabled" + idSuffix,
                                    value: deptModel.Disabled,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Depts_{column.Name}{idSuffix}",
                                            value: deptModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Depts_{column.Name}{idSuffix}",
                                            value: deptModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Depts_{column.Name}{idSuffix}",
                                            value: deptModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Depts_{column.Name}{idSuffix}",
                                            value: deptModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Depts_{column.Name}{idSuffix}",
                                            value: deptModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Depts_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Depts_{column.Name}Field",
                                                    controlId: $"Depts_{column.Name}",
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
                                                    value: deptModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: deptModel)
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
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: copyFrom,
                formData: context.Forms);
            deptModel.DeptId = 0;
            deptModel.Ver = 1;
            var invalid = DeptValidators.OnCreating(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            List<Process> processes = null;
            var errorData = deptModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            deptModel: deptModel,
                            processes: processes));
                    return new ResponseCollection(
                        context: context,
                        id: deptModel.DeptId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .Href(Locations.Edit(
                                context: context,
                                controller: context.Controller,
                                id: ss.Columns.Any(o => o.Linking)
                                    ? context.Forms.Long("LinkId")
                                    : deptModel.DeptId)
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
            DeptModel deptModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: deptModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = deptModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: deptId,
                formData: context.Forms);
            var invalid = DeptValidators.OnUpdating(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = deptModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new DeptsResponseCollection(
                        context: context,
                        deptModel: deptModel);
                    return ResponseByUpdate(res, context, ss, deptModel, processes)
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
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            DeptsResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: deptModel);
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
                    where: Rds.DeptsWhere().DeptId(deptModel.DeptId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{deptModel.DeptId}\"][data-latest]",
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
                        deptModel: deptModel,
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
                    .FieldResponse(context: context, ss: ss, deptModel: deptModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", deptModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(deptModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: deptModel,
                        tableName: "Depts"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: deptModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: deptModel.Comments,
                        deleteCommentId: deptModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: deptModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = deptModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(context, ss, deptId);
            var invalid = DeptValidators.OnDeleting(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = deptModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: deptModel.Title.MessageDisplay(context: context)));
                    var res = new DeptsResponseCollection(
                        context: context,
                        deptModel: deptModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int deptId, Message message = null)
        {
            var deptModel = new DeptModel(context: context, ss: ss, deptId: deptId);
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
                                    deptModel: deptModel))));
            return new DeptsResponseCollection(
                context: context,
                deptModel: deptModel)
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
                                .DataLatest(
                                    value: 1,
                                    _using: deptModelHistory.Ver == deptModel.Ver),
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
                                columns.ForEach(column => hb
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
            columns.ForEach(column =>
                sqlColumn.DeptsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int deptId)
        {
            var deptModel = new DeptModel(context: context, ss: ss, deptId: deptId);
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
            return EditorResponse(context, ss, deptModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        context.Controller,
                        deptId.ToString() 
                            + (deptModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Import(Context context)
        {
            var ss = SiteSettingsUtilities.DeptsSiteSettings(context: context);
            if (context.ContractSettings.Import == false)
            {
                return Messages.ResponseRestricted(context: context).ToJson();
            }
            var invalid = DeptValidators.OnImporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
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
                var columnHash = ImportUtilities.GetColumnHash(ss, csv);
                var idColumn = columnHash
                    .Where(o =>
                        o.Value.Column.ColumnName == "DeptCode")
                    .Select(o =>
                        new { Id = o.Key })
                    .FirstOrDefault()?.Id ?? -1;
                var invalidColumn = Imports.ColumnValidate(
                    context,
                    ss,
                    columnHash.Values.Select(o => o.Column.ColumnName),
                    columnNames: new string[]
                    {
                        "DeptCode",
                        "DeptName"
                    });
                if (invalidColumn != null) return invalidColumn;
                var deptHash = CreateDeptHash(
                    context: context,
                    ss: ss,
                    csv: csv,
                    columnHash: columnHash,
                    idColumn: idColumn);
                var insertCount = 0;
                var updateCount = 0;
                foreach (var deptModel in deptHash.Values)
                {
                    if (deptModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        if (deptModel.Updated(context: context, ss: ss))
                        {
                            deptModel.VerUp = Versions.MustVerUp(
                                context: context,
                                ss: ss,
                                baseModel: deptModel);
                            var errorData = deptModel.Update(
                                context: context,
                                ss: ss,
                                refreshSiteInfo: false,
                                get: false);
                            switch (errorData.Type)
                            {
                                case Error.Types.None:
                                    break;
                                default:
                                    return errorData.MessageJson(context: context);
                            }
                            updateCount++;
                        }
                    }
                    else
                    {
                        var errorData = deptModel.Create(
                            context: context,
                            ss: ss,
                            get: false);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            default:
                                return errorData.MessageJson(context: context);
                        }
                        insertCount++;
                    }
                }
                SiteInfo.Refresh(
                    context: context,
                    force: true);
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance ImportByApi(Context context, SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType, multipart: true))
            {
                return ApiResults.BadRequest(context: context);
            }
            if (context.ContractSettings.Import == false)
            {
                return ApiResults.Get(new ApiResponse(
                    id: context.Id,
                    statusCode: 500,
                    message: Messages.Restricted(context: context).Text));
            }
            var invalid = UserValidators.OnImporting(
                context: context,
                ss: ss,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            var api = context.RequestDataString.Deserialize<ImportApi>();
            var encoding = api.Encoding;
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
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId, number: count))
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
                    .Where(o =>
                        o.Value.Column.ColumnName == "DeptCode")
                    .Select(o =>
                        new { Id = o.Key })
                    .FirstOrDefault()?.Id ?? -1;
                var invalidColumn = Imports.ApiColumnValidate(
                    context,
                    ss,
                    columnHash.Values.Select(o => o.Column.ColumnName),
                    columnNames: new string[]
                    {
                        "DeptCode",
                        "DeptName"
                    });
                if (invalidColumn != null)
                {
                    return ApiResults.Get(new ApiResponse(
                        id: context.Id,
                        statusCode: 500,
                        message: invalidColumn));
                }
                var deptHash = CreateDeptHash(
                    context: context,
                    ss: ss,
                    csv: csv,
                    columnHash: columnHash,
                    idColumn: idColumn);
                var insertCount = 0;
                var updateCount = 0;
                foreach (var deptModel in deptHash.Values)
                {
                    if (deptModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        if (deptModel.Updated(context: context, ss: ss))
                        {
                            deptModel.VerUp = Versions.MustVerUp(
                                context: context,
                                ss: ss,
                                baseModel: deptModel);
                            var errorData = deptModel.Update(
                                context: context,
                                ss: ss,
                                refreshSiteInfo: false,
                                get: false);
                            switch (errorData.Type)
                            {
                                case Error.Types.None:
                                    break;
                                default:
                                    return ApiResults.Error(
                                        context: context,
                                        errorData: errorData);
                            }
                            updateCount++;
                        }
                    }
                    else
                    {
                        var errorData = deptModel.Create(
                            context: context,
                            ss: ss,
                            get: false);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            default:
                                return ApiResults.Error(
                                    context: context,
                                    errorData: errorData);
                        }
                        insertCount++;
                    }
                }
                SiteInfo.Refresh(
                    context: context,
                    force: true);
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
            return recordingData;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<int, DeptModel> CreateDeptHash(
            Context context,
            SiteSettings ss,
            Csv csv,
            Dictionary<int, ImportColumn> columnHash,
            int idColumn)
        {
            var deptHash = new Dictionary<int, DeptModel>();
            csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
            {
                var deptModel = new DeptModel();
                if (idColumn > -1)
                {
                    var model = new DeptModel(
                        context: context,
                        ss: ss,
                        deptCode: data.Row[idColumn]);
                    if (model.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        deptModel = model;
                    }
                }
                columnHash.ForEach(column =>
                {
                    var recordingData = ImportRecordingData(
                        context: context,
                        column: column.Value.Column,
                        value: data.Row[column.Key],
                        inheritPermission: ss.InheritPermission);
                    switch (column.Value.Column.ColumnName)
                    {
                        case "DeptId":
                            deptModel.DeptId = recordingData.ToInt();
                            break;
                        case "DeptCode":
                            deptModel.DeptCode = recordingData;
                            break;
                        case "DeptName":
                            deptModel.DeptName = recordingData;
                            break;
                        case "Body":
                            deptModel.Body = recordingData;
                            break;
                        case "Comments":
                            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                !data.Row[column.Key].IsNullOrEmpty())
                            {
                                deptModel.Comments.Prepend(
                                    context: context,
                                    ss: ss,
                                    body: data.Row[column.Key]);
                            }
                            break;
                        case "Disabled":
                            deptModel.Disabled = recordingData.ToBool();
                            break;
                        default:
                            deptModel.SetValue(
                                context: context,
                                column: column.Value.Column,
                                value: recordingData);
                            break;
                    }
                });
                deptHash.Add(data.Index, deptModel);
            });
            return deptHash;
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
            var invalid = DeptValidators.OnExporting(
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
            var invalid = DeptValidators.OnExporting(
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
            new DeptCollection(
                context: context,
                ss: ss,
                where: view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.DeptsWhere().TenantId(context.TenantId)),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss))
                        .ForEach(deptModel =>
                            csv.Append(
                                export.Columns.Select(exportColumn =>
                                    deptModel.CsvData(
                                        context: context,
                                        ss: ss,
                                        column: ss.GetColumn(
                                            context: context,
                                            columnName: exportColumn.ColumnName),
                                        exportColumn: exportColumn,
                                        mine: deptModel.Mine(context: context),
                                        encloseDoubleQuotes: true)).Join(),
                                "\n"));
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
                    name: Displays.Depts(context: context)),
                encoding: context.Forms.Data("ExportEncoding"));
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
        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            int deptId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var invalid = DeptValidators.OnEntry(
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
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null && !context.RequestDataString.IsNullOrEmpty())
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
                var invalidOnReading = SiteValidators.OnReading(
                    context,
                    siteModel.SitesSiteSettings(context, siteId.Value),
                    siteModel);
                switch (invalidOnReading.Type)
                {
                    case Error.Types.None: break;
                    default: return ApiResults.Error(
                        context: context,
                        errorData: invalidOnReading);
                }
            }
            var siteDepts = siteModel != null
                ? SiteInfo.SiteDepts(context, siteModel.InheritPermission)?
                    .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = api?.PageSize > 0 && api?.PageSize <= Parameters.Api.PageSize
                ? api.PageSize
                : Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (deptId > 0)
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash.Add("DeptId", deptId.ToString());
            }
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
                    return ApiResults.Get(new
                    {
                        statusCode = 200,
                        response = new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = gridData.TotalCount,
                            Data = gridData.KeyValues(
                                context: context,
                                ss: ss,
                                view: view)
                        }
                    }.ToJson());
                default:
                    var deptCollection = new DeptCollection(
                        context: context,
                        ss: ss,
                        where: view.Where(
                            context: context,
                            ss: ss)
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
                                        Rds.Depts_DeptId_WhereLike(factory: context),
                                        Rds.Depts_DeptName_WhereLike(factory: context),
                                        Rds.Depts_Body_WhereLike(factory: context)
                                    })
                                .Add(
                                    tableName: "Users",
                                    subLeft: Rds.SelectUsers(
                                    column: Rds.UsersColumn().UsersCount(),
                                    where: Rds.UsersWhere()
                                        .UserId(userId)
                                        .DeptId(raw: "\"Depts\".\"DeptId\"")
                                        .Add(raw: "\"Depts\".\"DeptId\">0")),
                                    _operator: ">0",
                                    _using: userId.HasValue),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss),
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize,
                        tableType: tableType);
                    var groups = siteDepts == null
                        ? deptCollection
                        : deptCollection.Join(siteDepts, c => c.DeptId, s => s, (c, s) => c);
                    return ApiResults.Get(new
                    {
                        StatusCode = 200,
                        Response = new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = deptCollection.TotalCount,
                            Data = groups.Select(o => o.GetByApi(
                                context: context,
                                ss: ss))
                        }
                    }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static EnumerableRowCollection<DataRow> Users(Context context, int deptId)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .UserId()
                        .DeptId()
                        .LoginId()
                        .Name()
                        .UserCode()
                        .Body()
                        .TenantManager()
                        .ServiceManager()
                        .Disabled(),
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .DeptId(deptId)))
                            .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance CreateByApi(Context context, SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var deptApiModel = context.RequestDataString.Deserialize<DeptApiModel>();
            if (deptApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: 0,
                deptApiModel: deptApiModel);
            var invalid = DeptValidators.OnCreating(
                context: context,
                ss: ss,
                deptModel: deptModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            foreach (var column in ss.Columns
                .Where(o => o.ValidateRequired ?? false)
                .Where(o => typeof(DeptApiModel).GetField(o.ColumnName) != null))
            {
                if (deptModel.GetType().GetField(column.ColumnName).GetValue(deptModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            var errorData = deptModel.Create(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        deptModel.DeptId,
                        Displays.Created(
                            context: context,
                            data: deptModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateByApi(Context context, SiteSettings ss, int deptId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var deptApiModel = context.RequestDataString.Deserialize<DeptApiModel>();
            if (deptApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: deptId,
                deptApiModel: deptApiModel);
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = DeptValidators.OnUpdating(
                context: context,
                ss: ss,
                deptModel: deptModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            foreach (var column in ss.Columns
                .Where(o => o.ValidateRequired ?? false)
                .Where(o => typeof(GroupApiModel).GetField(o.ColumnName) != null))
            {
                if (deptModel.GetType().GetField(column.ColumnName).GetValue(deptModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            var errorData = deptModel.Update(
                context: context,
                ss: ss,
                get: false);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        deptModel.DeptId,
                        Displays.Updated(
                            context: context,
                            data: deptModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance DeleteByApi(Context context, SiteSettings ss, int deptId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var deptApiModel = context.RequestDataString.Deserialize<DeptApiModel>();
            if (deptApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: deptId,
                deptApiModel: deptApiModel);
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = DeptValidators.OnDeleting(
                context: context,
                ss: ss,
                deptModel: deptModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            var errorData = deptModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        deptModel.DeptId,
                        Displays.Deleted(
                            context: context,
                            data: deptModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BulkDelete(Context context, SiteSettings ss)
        {
            if (context.CanDelete(ss: ss))
            {
                var selector = new RecordSelector(context: context);
                var count = 0;
                if (selector.All == false && selector.Selected.Any() == false)
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
                if (selector.All)
                {
                    count = BulkDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    count = BulkDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected);
                }
                Summaries.Synchronize(context: context, ss: ss);
                var data = new string[]
                {
                    ss.Title,
                    count.ToString()
                };
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int BulkDelete(
            Context context,
            SiteSettings ss,
            IEnumerable<long> selected,
            bool negative = false)
        {
            var subWhere = Views.GetBySession(
                context: context,
                ss: ss)
                    .Where(
                        context: context,
                        ss: ss,
                        itemJoin: false);
            var where = Rds.DeptsWhere()
                .TenantId(context.TenantId)
                .DeptId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    negative: negative,
                    _using: selected.Any())
                .DeptId_In(
                    sub: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptId(),
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectDepts(
                column: Rds.DeptsColumn().DeptId(),
                where: where);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.DeleteDepts(
                        factory: context,
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptId_In(sub: sub)),
                    Rds.RowCount(),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.DeptsUpdated),
                }).Count.ToInt();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlWhereCollection BulkDeleteWhere(
            Context context,
            SiteSettings ss,
            IEnumerable<long> selected,
            bool negative)
        {
            return Views.GetBySession(context: context, ss: ss).Where(
                context: context,
                ss: ss,
                where: Rds.DeptsWhere()
                    .DeptId_In(
                        value: selected.Select(o => o.ToInt()),
                        negative: negative,
                        _using: selected.Any()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
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

        /// <summary>
        /// Fixed:
        /// </summary>
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Restore(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            else if (Permissions.CanManageTenant(context: context))
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

        /// <summary>
        /// Fixed:
        /// </summary>
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
            var where = Rds.DeptsWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Depts_Deleted")
                .DeptId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    tableName: "Depts_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .DeptId_In(
                    tableName: "Depts_Deleted",
                    sub: Rds.SelectDepts(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.DeptsColumn().DeptId(),
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectDepts(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Depts_Deleted",
                column: Rds.DeptsColumn()
                    .DeptId(tableName: "Depts_Deleted"),
                where: where);
            var count = Repository.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.RestoreDepts(
                        factory: context,
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptId_In(sub: sub)),
                    Rds.RowCount(),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.DeptsUpdated)
                }).Count.ToInt();
            return count;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string PhysicalBulkDelete(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (Permissions.CanManageTenant(context: context))
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

        /// <summary>
        /// Fixed:
        /// </summary>
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
            where = where ?? Rds.DeptsWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Depts" + tableName)
                .DeptId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    tableName: "Depts" + tableName,
                    negative: negative,
                    _using: selected.Any())
                .DeptId_In(
                    tableName: "Depts" + tableName,
                    sub: Rds.SelectDepts(
                        tableType: tableType,
                        column: Rds.DeptsColumn().DeptId(),
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
            var sub = Rds.SelectDepts(
                tableType: tableType,
                _as: "Depts" + tableName,
                column: Rds.DeptsColumn()
                    .DeptId(tableName: "Depts" + tableName),
                where: where,
                param: param);
            var dataRows = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    tableType: tableType,
                    column: Rds.BinariesColumn().Guid().BinaryType(),
                    where: Rds.BinariesWhere().ReferenceId_In(sub: sub)))
                        .AsEnumerable();
            var count = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteBinaries(
                        tableType: tableType,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.PhysicalDeleteGroupMembers(
                        tableType: tableType,
                        where: Rds.GroupMembersWhere()
                            .DeptId_In(sub: sub)),
                    Rds.PhysicalDeletePermissions(
                        tableType: tableType,
                        where: Rds.PermissionsWhere()
                            .DeptId_In(sub: sub)),
                    Rds.PhysicalDeleteDepts(
                        tableType: tableType,
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptId_In(sub: sub)),
                    Rds.RowCount()
                }).Count.ToInt();
            return count;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static int CountByIds(Context context, SiteSettings ss, List<int> ids)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectDepts(
                    column: Rds.DeptsColumn()
                        .DeptsCount(),
                    where: Rds.DeptsWhere()
                        .DeptId_In(value: ids)));
        }
    }
}
