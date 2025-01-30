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
    public static class UserUtilities
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
            var invalid = UserValidators.OnEntry(
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
                referenceType: "Users",
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            return new GridData(
                context: context,
                ss: ss,
                view: view,
                join: Rds.UsersJoinDefault(),
                where: Rds.UsersWhere().TenantId(context.TenantId),
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
                    value: gridData.DataRows.Select(g => g.Long("UserId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("UserId")).ToJson())
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
                ? Rds.UsersWhere().UserId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.UsersWhere().UserId_In(
                    value: recordSelector.Selected?.Select(o => o.ToInt()) ?? new List<int>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long userId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.UsersWhere().UserId(userId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: userId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{userId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + userId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("UserId")}\"][data-latest]",
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
                            idColumn: "UserId"))
                    .Messages(context.Messages)
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            UserModel userModel,
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
                    userModel: userModel);
            }
            else
            {
                var mine = userModel.Mine(context: context);
                switch (column.Name)
                {
                    case "UserId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserId,
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
                                    value: userModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LoginId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LoginId,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Name":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Name,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "UserCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserCode,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Birthday":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Birthday,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Gender":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Gender,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Language":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Language,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "TimeZoneInfo":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TimeZoneInfo,
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
                                    value: userModel.DeptCode,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Dept,
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
                                    value: userModel.Manager,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Theme":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Theme,
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
                                    value: userModel.Body,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LastLoginTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LastLoginTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "PasswordExpirationTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordExpirationTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "PasswordChangeTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordChangeTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "NumberOfLogins":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfLogins,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "NumberOfDenial":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfDenial,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "TenantManager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TenantManager,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AllowCreationAtTopSite":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.AllowCreationAtTopSite,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AllowGroupAdministration":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.AllowGroupAdministration,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AllowGroupCreation":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.AllowGroupCreation,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AllowApi":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.AllowApi,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "AllowMovingFromTopSite":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.AllowMovingFromTopSite,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "EnableSecondaryAuthentication":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.EnableSecondaryAuthentication,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "DisableSecondaryAuthentication":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DisableSecondaryAuthentication,
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
                                    value: userModel.Disabled,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Lockout":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Lockout,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LockoutCounter":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LockoutCounter,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LdapSearchRoot":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LdapSearchRoot,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "SynchronizedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.SynchronizedTime,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LoginExpirationLimit":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LoginExpirationLimit,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "EnableSecretKey":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.EnableSecretKey,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "LoginExpirationPeriod":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LoginExpirationPeriod,
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
                                    value: userModel.Comments,
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
                                    value: userModel.Creator,
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
                                    value: userModel.Updator,
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
                                    value: userModel.CreatedTime,
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
                                    value: userModel.UpdatedTime,
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
                                            value: userModel.GetClass(columnName: column.Name),
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
                                            value: userModel.GetNum(columnName: column.Name),
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
                                            value: userModel.GetDate(columnName: column.Name),
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
                                            value: userModel.GetDescription(columnName: column.Name),
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
                                            value: userModel.GetCheck(columnName: column.Name),
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
                                            value: userModel.GetAttachments(columnName: column.Name),
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
            UserModel userModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "UserId": value = userModel.UserId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = userModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "LoginId": value = userModel.LoginId.GridText(
                        context: context,
                        column: column); break;
                    case "Name": value = userModel.Name.GridText(
                        context: context,
                        column: column); break;
                    case "UserCode": value = userModel.UserCode.GridText(
                        context: context,
                        column: column); break;
                    case "Birthday": value = userModel.Birthday.GridText(
                        context: context,
                        column: column); break;
                    case "Gender": value = userModel.Gender.GridText(
                        context: context,
                        column: column); break;
                    case "Language": value = userModel.Language.GridText(
                        context: context,
                        column: column); break;
                    case "TimeZoneInfo": value = userModel.TimeZoneInfo.GridText(
                        context: context,
                        column: column); break;
                    case "DeptCode": value = userModel.DeptCode.GridText(
                        context: context,
                        column: column); break;
                    case "Dept": value = userModel.Dept.GridText(
                        context: context,
                        column: column); break;
                    case "Manager": value = userModel.Manager.GridText(
                        context: context,
                        column: column); break;
                    case "Theme": value = userModel.Theme.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = userModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "LastLoginTime": value = userModel.LastLoginTime.GridText(
                        context: context,
                        column: column); break;
                    case "PasswordExpirationTime": value = userModel.PasswordExpirationTime.GridText(
                        context: context,
                        column: column); break;
                    case "PasswordChangeTime": value = userModel.PasswordChangeTime.GridText(
                        context: context,
                        column: column); break;
                    case "NumberOfLogins": value = userModel.NumberOfLogins.GridText(
                        context: context,
                        column: column); break;
                    case "NumberOfDenial": value = userModel.NumberOfDenial.GridText(
                        context: context,
                        column: column); break;
                    case "TenantManager": value = userModel.TenantManager.GridText(
                        context: context,
                        column: column); break;
                    case "AllowCreationAtTopSite": value = userModel.AllowCreationAtTopSite.GridText(
                        context: context,
                        column: column); break;
                    case "AllowGroupAdministration": value = userModel.AllowGroupAdministration.GridText(
                        context: context,
                        column: column); break;
                    case "AllowGroupCreation": value = userModel.AllowGroupCreation.GridText(
                        context: context,
                        column: column); break;
                    case "AllowApi": value = userModel.AllowApi.GridText(
                        context: context,
                        column: column); break;
                    case "AllowMovingFromTopSite": value = userModel.AllowMovingFromTopSite.GridText(
                        context: context,
                        column: column); break;
                    case "EnableSecondaryAuthentication": value = userModel.EnableSecondaryAuthentication.GridText(
                        context: context,
                        column: column); break;
                    case "DisableSecondaryAuthentication": value = userModel.DisableSecondaryAuthentication.GridText(
                        context: context,
                        column: column); break;
                    case "Disabled": value = userModel.Disabled.GridText(
                        context: context,
                        column: column); break;
                    case "Lockout": value = userModel.Lockout.GridText(
                        context: context,
                        column: column); break;
                    case "LockoutCounter": value = userModel.LockoutCounter.GridText(
                        context: context,
                        column: column); break;
                    case "LdapSearchRoot": value = userModel.LdapSearchRoot.GridText(
                        context: context,
                        column: column); break;
                    case "SynchronizedTime": value = userModel.SynchronizedTime.GridText(
                        context: context,
                        column: column); break;
                    case "LoginExpirationLimit": value = userModel.LoginExpirationLimit.GridText(
                        context: context,
                        column: column); break;
                    case "EnableSecretKey": value = userModel.EnableSecretKey.GridText(
                        context: context,
                        column: column); break;
                    case "LoginExpirationPeriod": value = userModel.LoginExpirationPeriod.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = userModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = userModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = userModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = userModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = userModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = userModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = userModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = userModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = userModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = userModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = userModel.GetAttachments(columnName: column.Name).GridText(
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
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.UsersLimit));
            }
            return Editor(context: context, ss: ss, userModel: new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int userId, bool clearSessions)
        {
            var userModel = new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            userModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userModel.UserId);
            return Editor(context: context, ss: ss, userModel: userModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, UserModel userModel)
        {
            var invalid = UserValidators.OnEditing(
                context: context,
                ss: ss,
                userModel: userModel);
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
                referenceType: "Users",
                title: userModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Users(context: context) + " - " + Displays.New(context: context)
                    : userModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        userModel: userModel)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, UserModel userModel)
        {
            var commentsColumn = ss.GetColumn(
                context: context,
                columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: userModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(userModel.UserId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "Users",
                                id: userModel.UserId)
                            : Locations.Action(
                                context: context,
                                controller: "Users")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: userModel,
                            tableName: "Users")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: userModel.Comments,
                                    column: commentsColumn,
                                    verType: userModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(context: context, userModel: userModel)
                                .FieldSetGeneral(
                                    context: context,
                                    ss: ss,
                                    userModel: userModel)
                                .FieldSetMailAddresses(
                                    context: context,
                                    userModel: userModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: userModel.MethodType != BaseModel.MethodTypes.New)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: userModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            userModel: userModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: userModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: userModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Users_Timestamp",
                            css: "always-send",
                            value: userModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: userModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Users",
                    referenceId: userModel.UserId,
                    referenceVer: userModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    ss: ss,
                    userModel: userModel));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMailAddresses",
                        text: Displays.MailAddresses(context: context),
                        _using: userModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: userModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () => hb.FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    userModel: userModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    userModel: userModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: userModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = userModel.ControlValue(
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
                        baseModel: userModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this UserModel userModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "UserId":
                    return userModel.UserId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return userModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LoginId":
                    return userModel.LoginId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Name":
                    return userModel.Name
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "UserCode":
                    return userModel.UserCode
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Password":
                    return userModel.Password
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "PasswordValidate":
                    return userModel.PasswordValidate
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "PasswordDummy":
                    return userModel.PasswordDummy
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "RememberMe":
                    return userModel.RememberMe
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Birthday":
                    return userModel.Birthday
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Gender":
                    return userModel.Gender
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Language":
                    return userModel.Language
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "TimeZone":
                    return userModel.TimeZone
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "DeptId":
                    return userModel.DeptId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Manager":
                    return userModel.Manager
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Theme":
                    return userModel.Theme
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return userModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LastLoginTime":
                    return userModel.LastLoginTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "PasswordExpirationTime":
                    return userModel.PasswordExpirationTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "PasswordChangeTime":
                    return userModel.PasswordChangeTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "NumberOfLogins":
                    return userModel.NumberOfLogins
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "NumberOfDenial":
                    return userModel.NumberOfDenial
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "TenantManager":
                    return userModel.TenantManager
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AllowCreationAtTopSite":
                    return userModel.AllowCreationAtTopSite
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AllowGroupAdministration":
                    return userModel.AllowGroupAdministration
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AllowGroupCreation":
                    return userModel.AllowGroupCreation
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AllowApi":
                    return userModel.AllowApi
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AllowMovingFromTopSite":
                    return userModel.AllowMovingFromTopSite
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "EnableSecondaryAuthentication":
                    return userModel.EnableSecondaryAuthentication
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "DisableSecondaryAuthentication":
                    return userModel.DisableSecondaryAuthentication
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Disabled":
                    return userModel.Disabled
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Lockout":
                    return userModel.Lockout
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LockoutCounter":
                    return userModel.LockoutCounter
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "OldPassword":
                    return userModel.OldPassword
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ChangedPassword":
                    return userModel.ChangedPassword
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "ChangedPasswordValidator":
                    return userModel.ChangedPasswordValidator
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AfterResetPassword":
                    return userModel.AfterResetPassword
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "AfterResetPasswordValidator":
                    return userModel.AfterResetPasswordValidator
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "DemoMailAddress":
                    return userModel.DemoMailAddress
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LdapSearchRoot":
                    return userModel.LdapSearchRoot
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "SynchronizedTime":
                    return userModel.SynchronizedTime
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LoginExpirationLimit":
                    return userModel.LoginExpirationLimit
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "EnableSecretKey":
                    return userModel.EnableSecretKey
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LoginExpirationPeriod":
                    return userModel.LoginExpirationPeriod
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return userModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return userModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return userModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return userModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return userModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return userModel.GetAttachments(columnName: column.Name)
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
            UserModel userModel)
        {
            if (userModel.VerType == Versions.VerTypes.Latest &&
                userModel.MethodType != BaseModel.MethodTypes.New &&
                Repository.ExecuteScalar_bool(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId(userModel.UserId)
                            .Password(_operator: "is not null"))))
            {
                if (userModel.Self(context: context))
                {
                    hb.Button(
                        controlId: "OpenChangePasswordDialog",
                        text: Displays.ChangePassword(context: context),
                        controlCss: "button-icon button-positive",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ChangePasswordDialog");
                }
                if (context.User?.TenantManager == true)
                {
                    hb.Button(
                        text: Displays.ResetPassword(context: context),
                        controlCss: "button-icon button-negative",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ResetPasswordDialog");
                }
            }
            if (context.HasPrivilege
               && context.User.Id != userModel.UserId
               && !userModel.Disabled
               && userModel.MethodType != BaseModel.MethodTypes.New)
            {
                hb.Button(
                    text: Displays.SwitchUser(context: context),
                    controlCss: "button-icon",
                    onClick: "$p.send($(this));",
                    icon: "ui-icon-person",
                    action: "SwitchUser",
                    method: "post",
                    confirm: "ConfirmSwitchUser");
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            UserModel userModel)
        {
            return hb
                .ChangePasswordDialog(
                    context: context,
                    ss: ss,
                    userId: userModel.UserId)
                .ResetPasswordDialog(
                    context: context,
                    ss: ss,
                    userId: userModel.UserId);
        }

        public static string EditorJson(Context context, SiteSettings ss, int userId)
        {
            return EditorResponse(context, ss, new UserModel(
                context, ss, userId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            Message message = null,
            string switchTargets = null)
        {
            userModel.MethodType = userModel.UserId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new UsersResponseCollection(
                context: context,
                userModel: userModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, userModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int userId)
        {
            if (Permissions.CanManageTenant(context: context))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss,
                    setSession: false);
                var where = view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.UsersWhere().TenantId(context.TenantId));
                var switchTargets = Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: where)) <= Parameters.General.SwitchTargetsLimit
                            ? Repository.ExecuteTable(
                                context: context,
                                statements: Rds.SelectUsers(
                                    column: Rds.UsersColumn().UserId(),
                                    join: Rds.UsersJoinDefault(),
                                    where: where,
                                    orderBy: view.OrderBy(
                                        context: context,
                                        ss: ss,
                                        orderBy: Rds.UsersOrderBy()
                                            .UpdatedTime(SqlOrderBy.Types.desc))))
                                            .AsEnumerable()
                                            .Select(dataRow => dataRow.Int("UserId"))
                                            .ToList()
                            : new List<int>();
                if (!switchTargets.Contains(userId))
                {
                    switchTargets.Add(userId);
                }
                return switchTargets;
            }
            else
            {
                return new List<int> { userId };
            }
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            UserModel userModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: userModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = userModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Users_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                userModel: userModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "UserId":
                                res.Val(
                                    target: "#Users_UserId" + idSuffix,
                                    value: userModel.UserId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LoginId":
                                res.Val(
                                    target: "#Users_LoginId" + idSuffix,
                                    value: userModel.LoginId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "GlobalId":
                                res.Val(
                                    target: "#Users_GlobalId" + idSuffix,
                                    value: userModel.GlobalId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Name":
                                res.Val(
                                    target: "#Users_Name" + idSuffix,
                                    value: userModel.Name.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserCode":
                                res.Val(
                                    target: "#Users_UserCode" + idSuffix,
                                    value: userModel.UserCode.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Password":
                                res.Val(
                                    target: "#Users_Password" + idSuffix,
                                    value: userModel.Password.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LastName":
                                res.Val(
                                    target: "#Users_LastName" + idSuffix,
                                    value: userModel.LastName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "FirstName":
                                res.Val(
                                    target: "#Users_FirstName" + idSuffix,
                                    value: userModel.FirstName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Birthday":
                                res.Val(
                                    target: "#Users_Birthday" + idSuffix,
                                    value: userModel.Birthday.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Gender":
                                res.Val(
                                    target: "#Users_Gender" + idSuffix,
                                    value: userModel.Gender.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Language":
                                res.Val(
                                    target: "#Users_Language" + idSuffix,
                                    value: userModel.Language.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TimeZone":
                                res.Val(
                                    target: "#Users_TimeZone" + idSuffix,
                                    value: userModel.TimeZone.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DeptId":
                                res.Val(
                                    target: "#Users_DeptId" + idSuffix,
                                    value: userModel.DeptId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Manager":
                                res.Val(
                                    target: "#Users_Manager" + idSuffix,
                                    value: userModel.Manager.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Theme":
                                res.Val(
                                    target: "#Users_Theme" + idSuffix,
                                    value: userModel.Theme.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "FirstAndLastNameOrder":
                                res.Val(
                                    target: "#Users_FirstAndLastNameOrder" + idSuffix,
                                    value: userModel.FirstAndLastNameOrder.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Users_Body" + idSuffix,
                                    value: userModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LastLoginTime":
                                res.Val(
                                    target: "#Users_LastLoginTime" + idSuffix,
                                    value: userModel.LastLoginTime.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "PasswordExpirationTime":
                                res.Val(
                                    target: "#Users_PasswordExpirationTime" + idSuffix,
                                    value: userModel.PasswordExpirationTime.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "PasswordChangeTime":
                                res.Val(
                                    target: "#Users_PasswordChangeTime" + idSuffix,
                                    value: userModel.PasswordChangeTime.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "NumberOfLogins":
                                res.Val(
                                    target: "#Users_NumberOfLogins" + idSuffix,
                                    value: userModel.NumberOfLogins.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "NumberOfDenial":
                                res.Val(
                                    target: "#Users_NumberOfDenial" + idSuffix,
                                    value: userModel.NumberOfDenial.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "TenantManager":
                                res.Val(
                                    target: "#Users_TenantManager" + idSuffix,
                                    value: userModel.TenantManager,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AllowCreationAtTopSite":
                                res.Val(
                                    target: "#Users_AllowCreationAtTopSite" + idSuffix,
                                    value: userModel.AllowCreationAtTopSite,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AllowGroupAdministration":
                                res.Val(
                                    target: "#Users_AllowGroupAdministration" + idSuffix,
                                    value: userModel.AllowGroupAdministration,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AllowGroupCreation":
                                res.Val(
                                    target: "#Users_AllowGroupCreation" + idSuffix,
                                    value: userModel.AllowGroupCreation,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AllowApi":
                                res.Val(
                                    target: "#Users_AllowApi" + idSuffix,
                                    value: userModel.AllowApi,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "AllowMovingFromTopSite":
                                res.Val(
                                    target: "#Users_AllowMovingFromTopSite" + idSuffix,
                                    value: userModel.AllowMovingFromTopSite,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "EnableSecondaryAuthentication":
                                res.Val(
                                    target: "#Users_EnableSecondaryAuthentication" + idSuffix,
                                    value: userModel.EnableSecondaryAuthentication,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DisableSecondaryAuthentication":
                                res.Val(
                                    target: "#Users_DisableSecondaryAuthentication" + idSuffix,
                                    value: userModel.DisableSecondaryAuthentication,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Disabled":
                                res.Val(
                                    target: "#Users_Disabled" + idSuffix,
                                    value: userModel.Disabled,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Lockout":
                                res.Val(
                                    target: "#Users_Lockout" + idSuffix,
                                    value: userModel.Lockout,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LockoutCounter":
                                res.Val(
                                    target: "#Users_LockoutCounter" + idSuffix,
                                    value: userModel.LockoutCounter.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "ApiKey":
                                res.Val(
                                    target: "#Users_ApiKey" + idSuffix,
                                    value: userModel.ApiKey.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SecondaryAuthenticationCode":
                                res.Val(
                                    target: "#Users_SecondaryAuthenticationCode" + idSuffix,
                                    value: userModel.SecondaryAuthenticationCode.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SecondaryAuthenticationCodeExpirationTime":
                                res.Val(
                                    target: "#Users_SecondaryAuthenticationCodeExpirationTime" + idSuffix,
                                    value: userModel.SecondaryAuthenticationCodeExpirationTime.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LdapSearchRoot":
                                res.Val(
                                    target: "#Users_LdapSearchRoot" + idSuffix,
                                    value: userModel.LdapSearchRoot.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SynchronizedTime":
                                res.Val(
                                    target: "#Users_SynchronizedTime" + idSuffix,
                                    value: userModel.SynchronizedTime.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LoginExpirationLimit":
                                res.Val(
                                    target: "#Users_LoginExpirationLimit" + idSuffix,
                                    value: userModel.LoginExpirationLimit.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "SecretKey":
                                res.Val(
                                    target: "#Users_SecretKey" + idSuffix,
                                    value: userModel.SecretKey.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "EnableSecretKey":
                                res.Val(
                                    target: "#Users_EnableSecretKey" + idSuffix,
                                    value: userModel.EnableSecretKey,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LoginExpirationPeriod":
                                res.Val(
                                    target: "#Users_LoginExpirationPeriod" + idSuffix,
                                    value: userModel.LoginExpirationPeriod.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Users_{column.Name}{idSuffix}",
                                            value: userModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Users_{column.Name}{idSuffix}",
                                            value: userModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Users_{column.Name}{idSuffix}",
                                            value: userModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Users_{column.Name}{idSuffix}",
                                            value: userModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Users_{column.Name}{idSuffix}",
                                            value: userModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Users_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Users_{column.Name}Field",
                                                    controlId: $"Users_{column.Name}",
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
                                                    value: userModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: userModel)
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
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return Error.Types.UsersLimit.MessageJson(context: context);
            }
            if (Parameters.Security.JoeAccountCheck
                && context.Forms.Get("Users_Password") == context.Forms.Get("Users_LoginId"))
            {
                return Error.Types.JoeAccountCheck.MessageJson(context: context);
            }
            ﻿//Issues、Results以外は参照コピーを使用しないためcopyFromを0にする
            var copyFrom = 0;
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: copyFrom,
                formData: context.Forms);
            userModel.UserId = 0;
            userModel.Ver = 1;
            var invalid = UserValidators.OnCreating(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Users_Password").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            List<Process> processes = null;
            var errorData = userModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            userModel: userModel,
                            process: processes?.FirstOrDefault(o => o.MatchConditions)));
                    return new ResponseCollection(
                        context: context,
                        id: userModel.UserId)
                            .SetMemory("formChanged", false)
                            .Messages(context.Messages)
                            .Href(Locations.Edit(
                                context: context,
                                controller: context.Controller,
                                id: ss.Columns.Any(o => o.Linking)
                                    ? context.Forms.Long("LinkId")
                                    : userModel.UserId)
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
            UserModel userModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: userModel.Title.Value);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = userModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                formData: context.Forms);
            var invalid = UserValidators.OnUpdating(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                case Error.Types.PermissionNotSelfChange:
                    return Messages.ResponsePermissionNotSelfChange(context: context)
                        .Val("#Users_TenantManager", userModel.SavedTenantManager)
                        .ClearFormData("Users_TenantManager")
                        .ToJson();
                default: return invalid.MessageJson(context: context);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = userModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new UsersResponseCollection(
                        context: context,
                        userModel: userModel);
                    return ResponseByUpdate(res, context, ss, userModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: userModel.Comments,
                            verType: userModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: userModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            UsersResponseCollection res,
            Context context,
            SiteSettings ss,
            UserModel userModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: userModel);
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
                    where: Rds.UsersWhere().UserId(userModel.UserId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{userModel.UserId}\"][data-latest]",
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
                        userModel: userModel,
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
                    .FieldResponse(context: context, ss: ss, userModel: userModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", userModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(userModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: userModel,
                        tableName: "Users"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: userModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: userModel.Comments,
                        deleteCommentId: userModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: userModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = userModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context, ss, userId);
            var invalid = UserValidators.OnDeleting(
                context: context,
                ss: ss,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = userModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: userModel.Title.MessageDisplay(context: context)));
                    var res = new UsersResponseCollection(
                        context: context,
                        userModel: userModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int userId, Message message = null)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: userId);
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
                                    userModel: userModel))));
            return new UsersResponseCollection(
                context: context,
                userModel: userModel)
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
            UserModel userModel)
        {
            new UserCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.UsersWhere().UserId(userModel.UserId),
                orderBy: Rds.UsersOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(userModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(userModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: userModelHistory.Ver == userModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: userModelHistory.Ver.ToString(),
                                            _using: userModelHistory.Ver < userModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        userModel: userModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.UsersColumnCollection()
                .UserId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.UsersColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: userId);
            userModel.Get(
                context: context,
                ss: ss,
                where: Rds.UsersWhere()
                    .UserId(userModel.UserId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            userModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, userModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        userId.ToString() 
                            + (userModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
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
                if (Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: BulkDeleteWhere(
                            context: context,
                            ss: ss,
                            selected: selector.Selected,
                            negative: selector.All)
                                .Users_UserId(context.UserId))) == 1)
                {
                    return Messages.ResponseUserNotSelfDelete(context: context).ToJson();
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
            var where = Rds.UsersWhere()
                .UserId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    negative: negative,
                    _using: selected.Any())
                .UserId_In(
                    sub: Rds.SelectUsers(
                        column: Rds.UsersColumn().UserId(),
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectUsers(
                column: Rds.UsersColumn().UserId(),
                where: where);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteGroupMembers(
                        factory: context,
                        where: Rds.GroupMembersWhere()
                            .UserId_In(sub: sub)),
                    Rds.DeletePermissions(
                        factory: context,
                        where: Rds.PermissionsWhere()
                            .UserId_In(sub: sub)),
                    Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.DeleteMailAddresses(
                        factory: context,
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(sub: sub)
                            .OwnerType("Users")),
                    Rds.DeleteUsers(
                        factory: context,
                        where: Rds.UsersWhere()
                            .UserId_In(sub: sub)),
                    Rds.RowCount(),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.UsersUpdated),
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
                where: Rds.UsersWhere()
                    .TenantId(context.TenantId)
                    .UserId_In(
                        value: selected.Select(o => o.ToInt()),
                        negative: negative,
                        _using: selected.Any()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Import(Context context)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(
                context: context,
                setAllChoices: true);
            if (context.ContractSettings.Import == false)
            {
                return Messages.ResponseRestricted(context: context).ToJson();
            }
            var invalid = UserValidators.OnImporting(
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
                var idColumn = -1;
                var columnHash = new Dictionary<int, Column>();
                var mailAddressHash = new Dictionary<string, string>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .Where(o => o.ColumnName != "DemoMailAddress")
                        .Where(o => o.TypeCs != "Attachments")
                        .FirstOrDefault();
                    if (column?.ColumnName == "LoginId")
                    {
                        idColumn = data.Index;
                    }
                    //海外言語設定のユーザでは、columnはTimeZoneInfoのLabelTextを参照して抽出しているため、
                    //ColumnNameがTimeZoneのものを抽出する。
                    if (column?.ColumnName == "TimeZoneInfo")
                    {
                        column = ss.Columns
                        .FirstOrDefault(o => o.ColumnName == "TimeZone");
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalidColumn = Imports.ColumnValidate(
                    context: context,
                    ss: ss,
                    headers: columnHash.Values.Select(o => o.ColumnName),
                    columnNames: new string[]
                    {
                        "LoginId",
                        "Name"
                    });
                if (invalidColumn != null) return invalidColumn;
                var userHash = CreateUserHash(
                    context: context,
                    ss: ss,
                    csv: csv,
                    columnHash: columnHash,
                    idColumn: idColumn);
                var errorRowNo = 1;
                foreach (var userModel in userHash.Values)
                {
                    var badMailAddress = Libraries.Mails.Addresses.BadAddress(
                        addresses: userModel.MailAddresses.Join());
                    if (!badMailAddress.IsNullOrEmpty())
                    {
                        return Messages.ResponseBadMailAddress(
                            context: context,
                            data: badMailAddress).ToJson();
                    }
                    if (!userModel.PasswordValidate.IsNullOrEmpty())
                    {
                        foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
                        {
                            if (!userModel.PasswordValidate.RegexExists(policy.Regex))
                            {
                                var badPassword = policy.Languages?.Any() == true
                                    ? policy.Display(context: context)
                                    : Displays.PasswordPolicyViolation(
                                        context: context,
                                        data: null);
                                var badPasswordParam = new string[]
                                {
                                    errorRowNo.ToString(),
                                    badPassword
                                };
                                return Messages.ResponseBadPasswordWhenImporting(
                                    context: context,
                                    data: badPasswordParam).ToJson();
                            }
                        }
                    }
                    errorRowNo++;
                }
                var insertCount = 0;
                var updateCount = 0;
                foreach (var userModel in userHash.Values)
                {
                    if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        var mailAddressUpdated = UpdateMailAddresses(context, userModel);
                        if (userModel.Updated(context: context, ss: ss))
                        {
                            userModel.VerUp = Versions.MustVerUp(
                                context: context,
                                ss: ss,
                                baseModel: userModel);
                            var errorData = userModel.Update(
                                context: context,
                                ss: ss,
                                updateMailAddresses: false,
                                refreshSiteInfo: false,
                                get: false);
                            switch (errorData.Type)
                            {
                                case Error.Types.None:
                                    break;
                                case Error.Types.UpdateConflicts:
                                    return new ResponseCollection(context: context)
                                        .Message(Messages.ImportInvalidUserIdAndLoginId(
                                            context: context,
                                            data: [userModel.UserId.ToString(), userModel.LoginId]))
                                        .ToJson();
                                default:
                                    return errorData.MessageJson(context: context);
                            }
                            updateCount++;
                        }
                        else if (mailAddressUpdated)
                        {
                            updateCount++;
                        }
                    }
                    else
                    {
                        var errorData = userModel.Create(
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
                        UpdateMailAddresses(context, userModel);
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
                var idColumn = -1;
                var columnHash = new Dictionary<int, Column>();
                var mailAddressHash = new Dictionary<string, string>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .Where(o => o.ColumnName != "DemoMailAddress")
                        .Where(o => o.TypeCs != "Attachments")
                        .FirstOrDefault();
                    if (column?.ColumnName == "LoginId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalidColumn = Imports.ApiColumnValidate(
                    context: context,
                    ss: ss,
                    headers: columnHash.Values.Select(o => o.ColumnName),
                    columnNames: new string[]
                    {
                        "LoginId",
                        "Name"
                    });
                if (invalidColumn != null)
                {
                    return ApiResults.Get(new ApiResponse(
                        id: context.Id,
                        statusCode: 500,
                        message: invalidColumn));
                }
                var userHash = CreateUserHash(
                    context: context,
                    ss: ss,
                    csv: csv,
                    columnHash: columnHash,
                    idColumn: idColumn);
                var errorRowNo = 1;
                foreach (var userModel in userHash.Values)
                {
                    var badMailAddress = Libraries.Mails.Addresses.BadAddress(
                        addresses: userModel.MailAddresses.Join());
                    if (!badMailAddress.IsNullOrEmpty())
                    {
                        return ApiResults.Get(new ApiResponse(
                            id: context.Id,
                            statusCode: 500,
                            message: Messages.BadMailAddress(
                                context: context,
                                data: badMailAddress).Text));
                    }
                    if (!userModel.PasswordValidate.IsNullOrEmpty())
                    {
                        foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
                        {
                            if (!userModel.PasswordValidate.RegexExists(policy.Regex))
                            {
                                var badPassword = policy.Languages?.Any() == true
                                    ? policy.Display(context: context)
                                    : Displays.PasswordPolicyViolation(
                                        context: context,
                                        data: null);
                                var badPasswordParam = new string[]
                                {
                                    errorRowNo.ToString(),
                                    badPassword
                                };
                                return ApiResults.Get(new ApiResponse(
                                    id: context.Id,
                                    statusCode: 500,
                                    message: Messages.BadPasswordWhenImporting(
                                        context: context,
                                        data: badPasswordParam).Text));
                            }
                        }
                    }
                    errorRowNo++;
                }
                var insertCount = 0;
                var updateCount = 0;
                foreach (var userModel in userHash.Values)
                {
                    if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        var mailAddressUpdated = UpdateMailAddresses(context, userModel);
                        if (userModel.Updated(context: context, ss: ss))
                        {
                            userModel.VerUp = Versions.MustVerUp(
                                context: context,
                                ss: ss,
                                baseModel: userModel);
                            var errorData = userModel.Update(
                                context: context,
                                ss: ss,
                                updateMailAddresses: false,
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
                        else if (mailAddressUpdated)
                        {
                            updateCount++;
                        }
                    }
                    else
                    {
                        var errorData = userModel.Create(
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
                        UpdateMailAddresses(context, userModel);
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
        private static bool UpdateMailAddresses(Context context, UserModel userModel)
        {
            if (userModel.UserId > 0 && userModel.MailAddresses.Any())
            {
                var mailAddresses = userModel.MailAddresses
                    .OrderBy(o => o)
                    .Join();
                var where = Rds.MailAddressesWhere()
                    .OwnerId(userModel.UserId)
                    .OwnerType("Users");
                if (new MailAddressCollection(
                    context: context,
                    where: where)
                        .Select(o => o.MailAddress)
                        .OrderBy(o => o)
                        .Join() != mailAddresses)
                {
                    var statements = new List<SqlStatement>()
                    {
                        Rds.PhysicalDeleteMailAddresses(where: where)
                    };
                    userModel.MailAddresses
                        .Where(mailAddress => !Libraries.Mails.Addresses.GetBody(mailAddress).IsNullOrEmpty())
                        .ForEach(mailAddress =>
                            statements.Add(Rds.InsertMailAddresses(param: Rds.MailAddressesParam()
                                .OwnerId(userModel.UserId)
                                .OwnerType("Users")
                                .MailAddress(mailAddress))));
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: statements.ToArray());
                    return true;
                }
            }
            return false;
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
        public static Dictionary<int, UserModel> CreateUserHash(
            Context context,
            SiteSettings ss,
            Csv csv,
            Dictionary<int, Column> columnHash,
            int idColumn)
        {
            var userHash = new Dictionary<int, UserModel>();
            var tenantModel = new TenantModel(
                context: context,
                ss: ss,
                tenantId: context.TenantId);
            csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
            {
                var userModel = new UserModel();
                if (idColumn > -1)
                {
                    var model = new UserModel(
                        context: context,
                        ss: ss,
                        loginId: data.Row[idColumn]);
                    if (model.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        userModel = model;
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
                        case "TenantId":
                            userModel.TenantId = recordingData.ToInt();
                            break;
                        case "UserId":
                            userModel.UserId = recordingData.ToInt();
                            break;
                        case "LoginId":
                            userModel.LoginId = recordingData;
                            break;
                        case "GlobalId":
                            userModel.GlobalId = recordingData.ToString();
                            break;
                        case "Name":
                            userModel.Name = recordingData.ToString();
                            break;
                        case "UserCode":
                            userModel.UserCode = recordingData.ToString();
                            break;
                        case "Password":
                            userModel.Password = recordingData.IsNullOrEmpty()
                                ? userModel.Password
                                : recordingData.Sha512Cng();
                            userModel.PasswordValidate = recordingData.ToString();
                            break;
                        case "LastName":
                            userModel.LastName = recordingData.ToString();
                            break;
                        case "FirstName":
                            userModel.FirstName = recordingData.ToString();
                            break;
                        case "Birthday":
                            userModel.Birthday.Value = recordingData.ToDateTime();
                            break;
                        case "Gender":
                            userModel.Gender = recordingData.ToString();
                            break;
                        case "Language":
                            userModel.Language = recordingData.ToString();
                            if (userModel.Language.IsNullOrEmpty())
                            {
                                userModel.Language = tenantModel.Language.IsNullOrEmpty()
                                ? Parameters.Service.DefaultLanguage
                                : tenantModel.Language;
                            }
                            break;
                        case "TimeZone":
                            userModel.TimeZone = recordingData.ToString();
                            if (userModel.TimeZone.IsNullOrEmpty())
                            {
                                userModel.TimeZone = tenantModel.TimeZone.IsNullOrEmpty()
                                ? Parameters.Service.TimeZoneDefault
                                : tenantModel.TimeZone;
                            }
                            break;
                        case "DeptId":
                            userModel.DeptId = recordingData.ToInt();
                            break;
                        case "DeptCode":
                            userModel.DeptId = SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptCode: recordingData).Id;
                            break;
                        case "Manager":
                            userModel.Manager = SiteInfo.User(
                                context: context,
                                userId: recordingData.ToInt());
                            break;
                        case "Theme":
                            userModel.Theme = recordingData.ToString();
                            break;
                        case "EnableSecretKey":
                            userModel.EnableSecretKey = recordingData.ToBool();
                            break;
                        case "Body":
                            userModel.Body = recordingData.ToString();
                            break;
                        case "PasswordExpirationTime":
                            userModel.PasswordExpirationTime.Value = recordingData.ToDateTime();
                            break;
                        case "TenantManager":
                            userModel.TenantManager = recordingData.ToBool();
                            break;
                        case "ServiceManager":
                            userModel.ServiceManager = recordingData.ToBool();
                            break;
                        case "AllowCreationAtTopSite":
                            userModel.AllowCreationAtTopSite = recordingData.ToBool();
                            break;
                        case "AllowGroupAdministration":
                            userModel.AllowGroupAdministration = recordingData.ToBool();
                            break;
                        case "AllowGroupCreation":
                            userModel.AllowGroupCreation = recordingData.ToBool();
                            break;
                        case "AllowApi":
                            userModel.AllowApi = recordingData.ToBool();
                            break;
                        case "EnableSecondaryAuthentication":
                            userModel.EnableSecondaryAuthentication = recordingData.ToBool();
                            break;
                        case "DisableSecondaryAuthentication":
                            userModel.DisableSecondaryAuthentication = recordingData.ToBool();
                            break;
                        case "Disabled":
                            userModel.Disabled = recordingData.ToBool();
                            break;
                        case "Lockout":
                            userModel.Lockout = recordingData.ToBool();
                            break;
                        case "ApiKey":
                            userModel.ApiKey = recordingData.ToString();
                            break;
                        case "MailAddresses":
                            userModel.MailAddresses = recordingData.Split(',').ToList();
                            break;
                        case "LdapSearchRoot":
                            userModel.LdapSearchRoot = recordingData.ToString();
                            break;
                        case "SynchronizedTime":
                            userModel.SynchronizedTime = recordingData.ToDateTime();
                            break;
                        case "Comments":
                            if (userModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                !data.Row[column.Key].IsNullOrEmpty())
                            {
                                userModel.Comments.Prepend(
                                    context: context,
                                    ss: ss,
                                    body: data.Row[column.Key]);
                            }
                            break;
                        default:
                            userModel.SetValue(
                                context: context,
                                column: column.Value,
                                value: recordingData);
                            break;
                    }
                });
                userHash.Add(data.Index, userModel);
            });
            return userHash;
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
            var invalid = UserValidators.OnExporting(
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
            var invalid = UserValidators.OnExporting(
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
                    ",",
                    $"\"{Displays.MailAddress(context: context)}\"",
                    ",",
                    $"\"{Displays.Password(context: context)}\"",
                    "\n");
            }
            new UserCollection(
                context: context,
                ss: ss,
                where: view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.UsersWhere().TenantId(context.TenantId)),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss))
                        .ForEach(userModel =>
                            csv.Append(
                                export.Columns.Select(exportColumn =>
                                    userModel.CsvData(
                                        context: context,
                                        ss: ss,
                                        column: ss.GetColumn(
                                            context: context,
                                            columnName: exportColumn.ColumnName),
                                        exportColumn: exportColumn,
                                        mine: userModel.Mine(context: context),
                                        encloseDoubleQuotes: true)).Join(),
                                ",",
                                "\"" + Repository.ExecuteTable(
                                    context: context,
                                    statements: Rds.SelectMailAddresses(
                                        column: Rds.MailAddressesColumn().MailAddress(),
                                        where: Rds.MailAddressesWhere()
                                            .OwnerId(userModel.UserId)
                                            .OwnerType("Users")))
                                                .AsEnumerable()
                                                .Select(dataRow => dataRow.String("MailAddress"))
                                                .Join() + "\"",
                                ",\"\"\n"));
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
                    name: Displays.Users(context: context)),
                encoding: context.Forms.Data("ExportEncoding"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenChangePasswordDialog(Context context, SiteSettings ss)
        {
            var invalid = UserValidators.OnPasswordChange(context: context);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return new ResponseCollection(context: context)
                .Html(
                    "#ChangePasswordDialog",
                    new HtmlBuilder().ChangePasswordDialog(
                        context: context,
                        ss: ss))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePassword(Context context, int userId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                formData: context.Forms);
            var invalid = UserValidators.OnPasswordChanging(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (Parameters.Security.JoeAccountCheck
                && context.Forms.Data("Users_ChangedPassword") == userModel.LoginId)
            {
                return Error.Types.JoeAccountCheck.MessageJson(context: context);
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Users_ChangedPassword").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            var error = userModel.ChangePassword(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : new UsersResponseCollection(
                    context: context,
                    userModel: userModel)
                        .OldPassword(context: context, value: string.Empty)
                        .ChangedPassword(context: context, value: string.Empty)
                        .ChangedPasswordValidator(context: context, value: string.Empty)
                        .Ver(context: context, ss: ss)
                        .Timestamp(context: context, ss: ss)
                        .Val("#VerUp", false)
                        .Disabled("#VerUp", false)
                        .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                            context: context,
                            baseModel: userModel,
                            tableName: "Users"))
                        .CloseDialog()
                        .ClearFormData()
                        .Message(Messages.ChangingPasswordComplete(context: context))
                        .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangePasswordAtLogin(Context context)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                loginId: context.Forms.Data("Users_LoginId"),
                formData: context.Forms);
            var invalid = UserValidators.OnPasswordChangingAtLogin(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                case Error.Types.IncorrectCurrentPassword:
                    userModel.DenyLog(context: context);
                    return invalid.MessageJson(context: context);
                default:
                    return invalid.MessageJson(context: context);
            }
            if (Parameters.Security.JoeAccountCheck
                && context.Forms.Data("Users_ChangedPassword") == userModel.LoginId)
            {
                return Error.Types.JoeAccountCheck.MessageJson(context: context);
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Users_ChangedPassword").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            var error = userModel.ChangePasswordAtLogin(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : userModel.Allow(
                    context: context,
                    returnUrl: context.Forms.Data("ReturnUrl"),
                    atLogin: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ResetPassword(Context context, int userId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                formData: context.Forms);
            var invalid = UserValidators.OnPasswordResetting(context: context);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                case Error.Types.IncorrectCurrentPassword:
                    userModel.DenyLog(context: context);
                    return invalid.MessageJson(context: context);
                default:
                    return invalid.MessageJson(context: context);
            }
            if (Parameters.Security.JoeAccountCheck
                && context.Forms.Data("Users_AfterResetPassword") == userModel.LoginId)
            {
                return Error.Types.JoeAccountCheck.MessageJson(context: context);
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Users_AfterResetPassword").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            var error = userModel.ResetPassword(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : new UsersResponseCollection(
                    context: context,
                    userModel: userModel)
                        .PasswordExpirationTime(context: context, ss: ss)
                        .PasswordChangeTime(context: context, ss: ss)
                        .AfterResetPassword(context: context, value: string.Empty)
                        .AfterResetPasswordValidator(context: context, value: string.Empty)
                        .Ver(context: context, ss: ss)
                        .Timestamp(context: context, ss: ss)
                        .Val("#VerUp", false)
                        .Disabled("#VerUp", false)
                        .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                            context: context,
                            baseModel: userModel,
                            tableName: "Users"))
                        .CloseDialog()
                        .ClearFormData()
                        .Message(Messages.PasswordResetCompleted(context: context))
                        .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AddMailAddresses(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId);
            var mailAddress = context.Forms.Data("MailAddress").Trim();
            var selected = context.Forms.List("MailAddresses");
            var invalid = UserValidators.OnAddingMailAddress(
                context: context,
                userModel: userModel,
                mailAddress: mailAddress);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                case Error.Types.BadMailAddress:
                    return invalid.MessageJson(context: context);
                default:
                    return invalid.MessageJson(context: context);
            }
            var error = userModel.AddMailAddress(
                context: context,
                mailAddress: mailAddress,
                selected: selected);
            return error.Has()
                ? error.MessageJson(context: context)
                : ResponseMailAddresses(
                    context: context,
                    userModel: userModel,
                    selected: selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteMailAddresses(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId);
            var invalid = UserValidators.OnUpdating(
                context: context, ss: ss, userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var selected = context.Forms.List("MailAddresses");
            var error = userModel.DeleteMailAddresses(
                context: context,
                selected: selected);
            return error.Has()
                ? error.MessageJson(context: context)
                : ResponseMailAddresses(
                    context: context,
                    userModel: userModel,
                    selected: selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ResponseMailAddresses(
            Context context,
            UserModel userModel,
            IEnumerable<string> selected)
        {
            return new ResponseCollection(context: context)
                .Html(
                    "#MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: userModel.MailAddresses.ToDictionary(
                            o => o, o => new ControlData(o)),
                        selectedValueTextCollection: selected))
                .Val("#MailAddress", string.Empty)
                .SetMemory("formChanged", true)
                .Focus("#MailAddress")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, long userId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword(context: context)),
                action: () => hb.ChangePasswordDialog(
                    context: context,
                    ss: ss,
                    userId: userId,
                    content: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool content = true)
        {
            return hb.ChangePasswordDialog(
                context: context,
                ss: ss,
                userId: context.UserId,
                content: content);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, long userId, bool content)
        {
            hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordForm")
                    .Action(Locations.Action(
                        context: context,
                        controller: "Users",
                        id: userId))
                    .DataEnter("#ChangePassword"),
                action: content
                    ? () => hb
                        .Field(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(
                                context: context,
                                columnName: "OldPassword"))
                        .Field(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(
                                context: context,
                                columnName: "ChangedPassword"))
                        .Field(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(
                                context: context,
                                columnName: "ChangedPasswordValidator"))
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                controlId: "ChangePassword",
                                text: Displays.Change(context: context),
                                controlCss: "button-icon validate button-positive",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "ChangePassword",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"))
                    : (Action)null);
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ResetPasswordDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, long userId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ResetPasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ResetPassword(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ResetPasswordForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users",
                                id: userId))
                            .DataEnter("#ResetPassword"),
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "AfterResetPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "AfterResetPasswordValidator"))
                            .P(css: "hidden", action: () => hb
                                .TextBox(
                                    textType: HtmlTypes.TextTypes.Password,
                                    controlCss: " dummy not-send"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ResetPassword",
                                    text: Displays.Reset(context: context),
                                    controlCss: "button-icon validate button-negative",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ResetPassword",
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
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                offset: context.Forms.Int("GridOffset"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string HtmlLogin(Context context, string returnUrl, string message)
        {
            var hb = new HtmlBuilder();
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                useBreadcrumb: false,
                useTitle: false,
                useSearch: false,
                useNavigationMenu: false,
                referenceType: "Users",
                title: string.Empty,
                action: () => hb
                    .Div(id: "PortalLink", action: () => hb
                        .A(href: Parameters.General.HtmlPortalUrl, action: () => hb
                            .Text(Displays.Portal(context: context))))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("MainForm")
                            .Class("main-form")
                            .Action(Locations.Raw(
                                context: context,
                                parts: new string[]
                                {
                                    "users",
                                    "_action_?ReturnUrl=" + System.Web.HttpUtility.UrlEncode(returnUrl)
                                }))
                            .DataEnter("#Login"),
                        action: () => hb
                            .Div(
                                id: "LoginLogo",
                                action: context.ThemeVersionOver2_0()
                                    ? () => hb
                                        .HeaderLogo(
                                            context: context,
                                            ss: ss)
                                    : (Action)null)
                            .FieldSet(id: "LoginFieldSet", action: () => hb
                                .Div(
                                    id: "Logins",
                                    action: () => hb
                                        .Div(
                                            id: "LoginGuideTop",
                                            action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                                context: context,
                                                id: "LoginGuideTop")))
                                        .Div(action: () => hb
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "LoginId"),
                                                fieldCss: "field-wide",
                                                controlCss: " always-send focus")
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "Password"),
                                                fieldCss: "field-wide",
                                                controlCss: " always-send")
                                            .Div(
                                                id: "Tenants",
                                                css: "field-wide")
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "RememberMe")))
                                        .Div(id: "LoginCommands", action: () => hb
                                            .Button(
                                                controlId: "Login",
                                                controlCss: "button-icon button-right-justified validate button-positive",
                                                text: Displays.Login(context: context),
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-unlocked",
                                                action: "Authenticate",
                                                method: "post",
                                                type: "submit"))
                                        .Div(id: "SsoLogin",
                                            css: " command-center",
                                            action: () => hb
                                                .P(
                                                    css: "ssoLoginMessage",
                                                    action: () => hb
                                                        .Text(Displays.SsoLoginMessage(context: context)))
                                                .Button(
                                                    controlId: "SSOLogin",
                                                    controlCss: "button-icon button-right-justified button-positive",
                                                    text: Displays.SsoLogin(context: context),
                                                    action: "Challenge",
                                                    onClick: "$p.ssoLogin($(this))",
                                                    icon: "ui-icon-unlocked"),
                                            _using: Parameters.Authentication.Provider == "SAML")
                                        .Div(
                                            id: "LoginGuideBottom",
                                            action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                                context: context,
                                                id: "LoginGuideBottom"))))
                                .Div(id: "SecondaryAuthentications")
                                .Div(id: "TotpRegister")))
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("DemoForm")
                            .Action(Locations.Get(
                                context: context,
                                parts: new string[]
                                {
                                    "demos",
                                    "_action_"
                                })),
                        _using: Parameters.Service.Demo && !Parameters.Service.DemoApi,
                        action: () => hb
                            .Div(id: "Demo", action: () => hb
                                .FieldSet(
                                    css: " enclosed-thin",
                                    legendText: Displays.ViewDemoEnvironment(context: context),
                                    action: () => hb
                                        .Div(id: "DemoFields", action: () => hb
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "DemoMailAddress"))
                                            .Button(
                                                text: Displays.Register(context: context),
                                                controlCss: "button-icon validate button-positive",
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-mail-closed",
                                                action: "Register",
                                                method: "post")))))
                    .P(
                        id: "LoginMessage",
                        css: "message-form-bottom",
                        action: () => hb.Raw(message))
                    .ChangePasswordAtLoginDialog(
                        context: context,
                        ss: ss)
                    .Hidden(
                        controlId: "ReturnUrl",
                        value: context.QueryStrings.Data("ReturnUrl")))
                    .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Expired(Context context)
        {
            return HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.Expired));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ChangePasswordAtLoginDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ChangePasswordDialog")
                    .Class("dialog")
                    .Title(Displays.ChangePassword(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ChangePasswordForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users"))
                            .DataEnter("#ChangePassword"),
                        action: () => hb
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPassword"))
                            .Field(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "ChangedPasswordValidator"))
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ChangePassword",
                                    text: Displays.Change(context: context),
                                    controlCss: "button-icon validate button-positive",
                                    onClick: "$p.changePasswordAtLogin($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePasswordAtLogin",
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
        public static HtmlBuilder FieldSetMailAddresses(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            if (userModel.MethodType == BaseModel.MethodTypes.New) return hb;
            var listItemCollection = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectMailAddresses(
                    column: Rds.MailAddressesColumn()
                        .MailAddressId()
                        .MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userModel.UserId)
                        .OwnerType("Users")))
                            .AsEnumerable()
                            .ToDictionary(
                                o => o["MailAddress"].ToString(),
                                o => new ControlData(o["MailAddress"].ToString()));
            userModel.Session_MailAddresses(
                context: context,
                value: listItemCollection.Keys.ToList().ToJson());
            return hb.TabsPanelField(
                id: "FieldSetMailAddresses",
                action: () => hb.FieldSelectable(
                    controlId: "MailAddresses",
                    fieldCss: "field-vertical w500",
                    controlContainerCss: "container-selectable",
                    controlWrapperCss: " h350",
                    labelText: Displays.MailAddresses(context: context),
                    listItemCollection: listItemCollection,
                    commandOptionAction: () => hb
                        .Div(css: "command-left", action: () => hb
                            .TextBox(
                                controlId: "MailAddress",
                                controlCss: " w200")
                            .Button(
                                text: Displays.Add(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "AddMailAddress",
                                method: "post")
                            .Button(
                                controlId: "DeleteMailAddresses",
                                controlCss: "button-icon",
                                text: Displays.Delete(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-image",
                                action: "DeleteMailAddresses",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SyncByLdap(Context context)
        {
            Ldap.Sync(context: context);
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ApiEditor(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiEditing(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                referenceType: "Users",
                title: Displays.ApiSettings(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("MainForm")
                            .Class("main-form")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Users")),
                        action: () => hb
                            .ApiEditor(
                                context: context,
                                userModel: userModel)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")))
                                .ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ApiEditor(
            this HtmlBuilder hb, Context context, UserModel userModel)
        {
            return hb.Div(
                id: "EditorTabsContainer",
                css: "tab-container max",
                action: () => hb
                    .Ul(id: "EditorTabs", action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetGeneral",
                                text: Displays.General(context: context))))
                    .TabsPanelField(
                        id: "FieldSetGeneral",
                        action: () => hb.FieldText(
                            controlId: "ApiKey",
                            fieldCss: "field-wide",
                            labelText: Displays.ApiKey(context: context),
                            text: userModel.ApiKey))
                .Div(
                        id: "ApiEditorCommands",
                        action: () => hb
                            .Button(
                                controlId: "CreateApiKey",
                                controlCss: "button-icon button-positive",
                                text: userModel.ApiKey.IsNullOrEmpty()
                                    ? Displays.Create(context: context)
                                    : Displays.ReCreate(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "CreateApiKey",
                                method: "post")
                            .Button(
                                controlId: "DeleteApiKey",
                                controlCss: "button-icon button-negative",
                                text: Displays.Delete(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-trash",
                                action: "DeleteApiKey",
                                method: "post",
                                confirm: "ConfirmDelete",
                                _using: !userModel.ApiKey.IsNullOrEmpty())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CreateApiKey(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiCreating(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var error = userModel.CreateApiKey(context: context, ss: ss);
            if (error.Type.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                    .ReplaceAll(
                        "#EditorTabsContainer",
                        new HtmlBuilder().ApiEditor(
                            context: context,
                            userModel: userModel))
                    .Message(Messages.ApiKeyCreated(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string DeleteApiKey(Context context, SiteSettings ss)
        {
            var userModel = new UserModel(context: context, ss: ss, userId: context.UserId);
            var invalid = UserValidators.OnApiDeleting(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var error = userModel.DeleteApiKey(context: context, ss: ss);
            if (error.Type.Has())
            {
                return error.MessageJson(context: context);
            }
            else
            {
                return new ResponseCollection(context: context)
                    .ReplaceAll(
                        "#EditorTabsContainer",
                        new HtmlBuilder().ApiEditor(
                            context: context,
                            userModel: userModel))
                    .Message(Messages.ApiKeyDeleted(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            int userId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var invalid = UserValidators.OnEntry(
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
                    default:
                        return ApiResults.Error(
                        context: context,
                        errorData: invalidOnReading);
                }
            }
            var siteUsers = siteModel != null
                ? SiteInfo.SiteUsers(context, siteModel.InheritPermission)?
                .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = api?.PageSize > 0 && api?.PageSize <= Parameters.Api.PageSize
                ? api.PageSize
                : Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (userId > 0)
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash.Add("UserId", userId.ToString());
            }
            switch (view.ApiDataType)
            {
                case View.ApiDataTypes.KeyValues:
                    var gridData = new GridData(
                        context: context,
                        ss: ss,
                        view: view,
                        tableType: tableType,
                        join: Rds.UsersJoinDefault(),
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
                    var userCollection = new UserCollection(
                        context: context,
                        ss: ss,
                        where: view.Where(
                            context: context,
                            ss: ss)
                                .Users_TenantId(context.TenantId)
                                .SqlWhereLike(
                                    tableName: "\"Users\"",
                                    name: "SearchText",
                                    searchText: view.ColumnFilterHash
                                        ?.Where(f => f.Key == "SearchText")
                                        ?.Select(f => f.Value)
                                        ?.FirstOrDefault(),
                                    clauseCollection: new List<string>()
                                    {
                                        Rds.Users_LoginId_WhereLike(factory: context),
                                        Rds.Users_Name_WhereLike(factory: context),
                                        Rds.Users_UserCode_WhereLike(factory: context),
                                        Rds.Users_Body_WhereLike(factory: context),
                                        Rds.Depts_DeptCode_WhereLike(factory: context),
                                        Rds.Depts_DeptName_WhereLike(factory: context),
                                        Rds.Depts_Body_WhereLike(factory: context)
                                    }),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss),
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize,
                        tableType: tableType);
                    var users = siteUsers == null
                        ? userCollection
                        : userCollection.Join(siteUsers, c => c.UserId, s => s, (c, s) => c);
                    return ApiResults.Get(new
                    {
                        StatusCode = 200,
                        Response = new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = userCollection.TotalCount,
                            Data = users.Select(o => o.GetByApi(
                                context: context,
                                ss: ss,
                                getMailAddresses: view.ApiGetMailAddresses))
                        }
                    }.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SwitchUser(Context context)
        {
            var userModel = new UserModel(
                context: context,
                ss: null,
                userId: context.Controller == "users"
                    ? context.Id.ToInt()
                    : 0);
            var invalid = UserValidators.OnSwitchUser(
                context: context,
                userModel: userModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                SessionUtilities.Set(
                    context: context,
                    key: "SwitchLoginId",
                    value: userModel.LoginId);
                context = new Context();
                return new ResponseCollection(context: context)
                    .ReplaceAll("#Warnings", new HtmlBuilder().Warnings(
                        context: context,
                        ss: null))
                    .Message(Messages.UserSwitched(
                        context: context,
                        data: context.User.Name))
                    .ToJson();
            }
            else
            {
                SessionUtilities.Remove(
                    context: context,
                    key: "SwitchLoginId",
                    page: false);
                return new ResponseCollection(context: context)
                    .Message(Messages.InvalidRequest(context: context))
                    .ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ReturnOriginalUser(Context context)
        {
            var invalid = UserValidators.OnReturnSwitchUser(context: context);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            SessionUtilities.Remove(
                context: context,
                key: "SwitchLoginId",
                page: false);
            context = new Context();
            return new ResponseCollection(context: context)
                .ReplaceAll("#Warnings", new HtmlBuilder().Warnings(
                    context: context,
                    ss: null))
                .Message(Messages.UserSwitched(
                    context: context,
                    data: context.User.Name))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SetStartGuide(Context context)
        {
            context.UserSettings.DisableStartGuide = context.Forms.Bool("DisableStartGuide");
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .UserId(context.UserId),
                    param: Rds.UsersParam()
                        .UserSettings(context.UserSettings.RecordingJson()),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            return new ResponseCollection(context: context).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string CloseAnnouncement(Context context)
        {
            var siteId = Parameters.Service.AnnouncementSiteId;
            if (siteId == 0)
            {
                return new ResponseCollection(context: context)
                    .Message(Messages.InvalidRequest(context: context))
                    .ToJson();
            }
            var announcementId = context.Forms.Int("AnnouncementId");
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            var issueCollection = new IssueCollection(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .SiteId(siteId)
                    .IssueId(announcementId));
            var issueModel = issueCollection.FirstOrDefault();
            if (issueModel == null)
            {
                return new ResponseCollection(context: context)
                    .Message(Messages.InvalidRequest(context: context))
                    .ToJson();
            }
            var allowClose = issueModel.CheckHash.TryGetValue("CheckD", out bool checkD)
                ? checkD
                : false;
            if (allowClose)
            {
                SessionUtilities.Set(
                    context: context,
                    key: $"ClosedAnnouncement:{announcementId}",
                    value: "true");
                return new ResponseCollection(context: context)
                    .Remove($"#AnnouncementContainer_{announcementId}")
                    .ToJson();
            }
            else
            {
                return new ResponseCollection(context: context).ToJson();
            }
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
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.UsersLimit));
            }
            var userApiModel = context.RequestDataString.Deserialize<UserApiModel>();
            if (userApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: 0,
                userApiModel: userApiModel,
                methodType: BaseModel.MethodTypes.New);
            var invalid = UserValidators.OnCreating(
                context: context,
                ss: ss,
                userModel: userModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            if (Parameters.Security.JoeAccountCheck
                && (context.RequestDataString.Deserialize<UserApiModel>()?.Password ?? string.Empty) == userModel.LoginId)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.JoeAccountCheck));
            }
            if (!Parameters.Security.DisableCheckPasswordPolicyIfApi)
            {
                foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
                {
                    if (!(context.RequestDataString.Deserialize<UserApiModel>()?.Password ?? string.Empty).RegexExists(policy.Regex))
                    {
                        return ApiResults.Error(
                            context: context,
                            errorData: new ErrorData(type: Error.Types.PasswordPolicyViolation));
                    }
                }
            }
            foreach (var column in ss.Columns
                .Where(o => o.ValidateRequired ?? false)
                .Where(o => typeof(UserApiModel).GetField(o.ColumnName) != null))
            {
                if (userModel.GetType().GetField(column.ColumnName).GetValue(userModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            var errorData = userModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        userModel.UserId,
                        Displays.Created(
                            context: context,
                            data: userModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateByApi(Context context, SiteSettings ss, int userId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var userApiModel = context.RequestDataString.Deserialize<UserApiModel>();
            if (userApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                userApiModel: userApiModel);
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = UserValidators.OnUpdating(
                context: context,
                ss: ss,
                userModel: userModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            if (Parameters.Security.JoeAccountCheck
                && (context.RequestDataString.Deserialize<UserApiModel>()?.Password ?? string.Empty) == userModel.LoginId)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.JoeAccountCheck));
            }
            if (!Parameters.Security.DisableCheckPasswordPolicyIfApi)
            {
                foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
                {
                    if (userModel.Password_Updated(context: context) && !(context.RequestDataString.Deserialize<UserApiModel>()?.Password ?? string.Empty).RegexExists(policy.Regex))
                    {
                        return ApiResults.Error(
                            context: context,
                            errorData: new ErrorData(type: Error.Types.PasswordPolicyViolation));
                    }
                }
            }
            if (!(context.RequestDataString.Deserialize<UserApiModel>()?.Password).IsNullOrEmpty())
            {
                userModel.ChangedPassword = (context.RequestDataString.Deserialize<UserApiModel>()?.Password ?? string.Empty).Sha512Cng();
                var error = userModel.ChangePassword(context: context);
                if (error.Has())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: error));
                }
                userModel.SetByApi(
                    context: context,
                    ss: ss,
                    data: userApiModel);
            }
            foreach (var column in ss.Columns
                .Where(o => o.ValidateRequired ?? false)
                .Where(o => typeof(UserApiModel).GetField(o.ColumnName) != null))
            {
                if (userModel.GetType().GetField(column.ColumnName).GetValue(userModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            userModel.VerUp = Versions.MustVerUp(
                context: context,
                ss: ss,
                baseModel: userModel);
            var errorData = userModel.Update(
                context: context,
                ss: ss,
                updateMailAddresses: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        userModel.UserId,
                        Displays.Updated(
                            context: context,
                            data: userModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance DeleteByApi(Context context, SiteSettings ss, int userId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var userApiModel = context.RequestDataString.Deserialize<UserApiModel>();
            if (userApiModel == null)
            {
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            }
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: userId,
                userApiModel: userApiModel);
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = UserValidators.OnDeleting(
                context: context,
                ss: ss,
                userModel: userModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            var errorData = userModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        userModel.UserId,
                        Displays.Deleted(
                            context: context,
                            data: userModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool ExcessLicense(Context context)
        {
            return Parameters.CommercialLicense()
                && Parameters.LicensedUsers() > 0
                && Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .Disabled(false))) > Parameters.LicensedUsers();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static (int TenantId, ContractSettings ContractSettings) GetContractSettingsBySsoCode(Context context, string ssocode)
        {
            var dataRow = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn()
                        .TenantId()
                        .ContractSettings(),
                    where: Rds.TenantsWhere()
                        .Add(raw: $"(\"Tenants\".\"ContractDeadline\" is null or \"Tenants\".\"ContractDeadline\" >= {context.Sqls.CurrentDateTime})")
                        .Comments(ssocode)))
                            .AsEnumerable()
                            .FirstOrDefault();
            if (dataRow == null)
            {
                return (0, null);
            }
            var tenantId = dataRow.Int("TenantId");
            var contractSettings = dataRow.String("ContractSettings").Deserialize<ContractSettings>();
            return (tenantId, contractSettings);
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
            else if (Permissions.CanManageUser(context: context))
            {
                var selector = new RecordSelector(context: context);
                var count = 0;
                try
                {
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
                }
                catch (System.Data.Common.DbException e)
                {
                    if (context.SqlErrors.ErrorCode(e) == context.SqlErrors.ErrorCodeDuplicateKey)
                    {
                        return new ErrorData(type: Error.Types.LoginIdAlreadyUse).MessageJson(context: context);
                    }
                    else
                    {
                        throw;
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
            var where = Rds.UsersWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Users_Deleted")
                .UserId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    tableName: "Users_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .UserId_In(
                    tableName: "Users_Deleted",
                    sub: Rds.SelectUsers(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.UsersColumn().UserId(),
                        join: ss.Join(
                            context: context,
                            join: new IJoin[]
                            {
                                subWhere
                            }),
                        where: subWhere));
            var sub = Rds.SelectUsers(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Users_Deleted",
                column: Rds.UsersColumn()
                    .UserId(tableName: "Users_Deleted"),
                where: where);
            var count = Repository.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreGroupMembers(
                        factory: context,
                        where: Rds.GroupMembersWhere()
                            .UserId_In(sub: sub)),
                    Rds.RestorePermissions(
                        factory: context,
                        where: Rds.PermissionsWhere()
                            .UserId_In(sub: sub)),
                    Rds.RestoreBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.RestoreMailAddresses(
                        factory: context,
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(sub: sub)
                            .OwnerType("Users")),
                    Rds.RestoreUsers(
                        factory: context,
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId_In(sub: sub)),
                    Rds.RowCount(),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.UsersUpdated)
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
            if (Permissions.CanManageUser(context: context))
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
            where = where ?? Rds.UsersWhere()
                .TenantId(
                    value: context.TenantId,
                    tableName: "Users" + tableName)
                .UserId_In(
                    value: selected.Select(o => o.ToInt()).ToList(),
                    tableName: "Users" + tableName,
                    negative: negative,
                    _using: selected.Any())
                .UserId_In(
                    tableName: "Users" + tableName,
                    sub: Rds.SelectUsers(
                        tableType: tableType,
                        column: Rds.UsersColumn().UserId(),
                        where: Views.GetBySession(
                            context: context,
                            ss: ss)
                                .Where(
                                    context: context,
                                    ss: ss,
                                    itemJoin: false)));
            var sub = Rds.SelectUsers(
                tableType: tableType,
                _as: "Users" + tableName,
                column: Rds.UsersColumn()
                    .UserId(tableName: "Users" + tableName),
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
                    Rds.PhysicalDeleteGroupMembers(
                        tableType: tableType,
                        where: Rds.GroupMembersWhere()
                            .UserId_In(sub: sub)),
                    Rds.PhysicalDeletePermissions(
                        tableType: tableType,
                        where: Rds.PermissionsWhere()
                            .UserId_In(sub: sub)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: tableType,
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .ReferenceId_In(sub: sub)
                            .BinaryType(value: "TenantManagementImages")),
                    Rds.PhysicalDeleteMailAddresses(
                        tableType: tableType,
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(sub: sub)
                            .OwnerType("Users")),
                    Rds.PhysicalDeleteUsers(
                        tableType: tableType,
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId_In(sub: sub)),
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
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .UsersCount(),
                    where: Rds.UsersWhere()
                        .UserId_In(value: ids)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GeneratePassword(string passwordObject, string passwordValidateObject)
        {
            var password = "";
            var regex = "";
            var defaultRegex = Parameters.Security.PasswordPolicies[0].Enabled
                ? Parameters.Security.PasswordPolicies[0].Regex
                : "[!-~]{ 6,}";
            foreach (var policy in Parameters.Security.PasswordPolicies.Skip(1).Where(o => o.Enabled))
            {
                regex += "(?=.*?" + policy.Regex + ")";
            }
            regex += defaultRegex;
            var xeger = new Fare.Xeger(defaultRegex, new Random());
            while (!System.Text.RegularExpressions.Regex.IsMatch(password, $"^{regex}$"))
            {
                password = xeger.Generate();
            }
            return new ResponseCollection()
                .Val(
                    target: passwordObject,
                    value: password)
                .Val(
                    target: passwordValidateObject,
                    value: password)
                .SetData(target: passwordObject)
                .SetData(target: passwordValidateObject)
                .ToJson();
        }
    }
}
