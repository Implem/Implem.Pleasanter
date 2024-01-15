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
    public static class RegistrationUtilities
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
            var suffix = view.GetIndexSuffix();
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
            var invalid = RegistrationValidators.OnEntry(
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
                referenceType: "Registrations",
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
                        .Title(Displays.BulkUpdate(context: context))))
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
                where: Rds.RegistrationsWhere().TenantId(context.TenantId),
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
                .Table(
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
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.TotalCount)
                            .ToString())
                .Hidden(
                    controlId: "GridRowIds",
                    value: gridData.DataRows.Select(g => g.Long("RegistrationId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("RegistrationId")).ToJson())
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
                        checkRow: checkRow));
        }

        private static SqlWhereCollection SelectedWhere(
            Context context,
            SiteSettings ss)
        {
            var selector = new RecordSelector(context: context);
            return !selector.Nothing
                ? Rds.RegistrationsWhere().RegistrationId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.RegistrationsWhere().RegistrationId_In(
                    value: recordSelector.Selected?.Select(o => o.ToInt()) ?? new List<int>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long registrationId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.RegistrationsWhere().RegistrationId(registrationId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: registrationId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{registrationId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + registrationId)
                    .Messages(context.Messages)
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("RegistrationId")}\"][data-latest]",
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
                            idColumn: "RegistrationId"))
                    .Messages(context.Messages)
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            RegistrationModel registrationModel,
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
                    registrationModel: registrationModel);
            }
            else
            {
                var mine = registrationModel.Mine(context: context);
                switch (column.Name)
                {
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "MailAddress":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.MailAddress,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "InviteeName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.InviteeName,
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
                                    value: registrationModel.LoginId,
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
                                    value: registrationModel.Name,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "Invitingflg":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.Invitingflg,
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
                                    value: registrationModel.Comments,
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
                                    value: registrationModel.Creator,
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
                                    value: registrationModel.Updator,
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
                                    value: registrationModel.CreatedTime,
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
                                    value: registrationModel.UpdatedTime,
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
                                            value: registrationModel.GetClass(columnName: column.Name),
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
                                            value: registrationModel.GetNum(columnName: column.Name),
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
                                            value: registrationModel.GetDate(columnName: column.Name),
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
                                            value: registrationModel.GetDescription(columnName: column.Name),
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
                                            value: registrationModel.GetCheck(columnName: column.Name),
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
                                            value: registrationModel.GetAttachments(columnName: column.Name),
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
            RegistrationModel registrationModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "Ver": value = registrationModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "MailAddress": value = registrationModel.MailAddress.GridText(
                        context: context,
                        column: column); break;
                    case "InviteeName": value = registrationModel.InviteeName.GridText(
                        context: context,
                        column: column); break;
                    case "LoginId": value = registrationModel.LoginId.GridText(
                        context: context,
                        column: column); break;
                    case "Name": value = registrationModel.Name.GridText(
                        context: context,
                        column: column); break;
                    case "Invitingflg": value = registrationModel.Invitingflg.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = registrationModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = registrationModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = registrationModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = registrationModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = registrationModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = registrationModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = registrationModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = registrationModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = registrationModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = registrationModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = registrationModel.GetAttachments(columnName: column.Name).GridText(
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
            return Editor(context: context, ss: ss, registrationModel: new RegistrationModel(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int registrationId, bool clearSessions)
        {
            var registrationModel = new RegistrationModel(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: registrationId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            registrationModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: registrationModel.RegistrationId);
            return Editor(context: context, ss: ss, registrationModel: registrationModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, RegistrationModel registrationModel)
        {
            var invalid = RegistrationValidators.OnEditing(
                context: context,
                ss: ss,
                registrationModel: registrationModel);
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
                referenceType: "Registrations",
                title: registrationModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Registrations(context: context) + " - " + Displays.New(context: context)
                    : registrationModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        registrationModel: registrationModel)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, RegistrationModel registrationModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: registrationModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form confirm-unload")
                        .Action(registrationModel.RegistrationId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "Registrations",
                                id: registrationModel.RegistrationId)
                            : Locations.Action(
                                context: context,
                                controller: "Registrations")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: registrationModel,
                            tableName: "Registrations")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: registrationModel.Comments,
                                    column: commentsColumn,
                                    verType: registrationModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    registrationModel: registrationModel)
                                .FieldSetGeneral(context: context, ss: ss, registrationModel: registrationModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: registrationModel.MethodType != BaseModel.MethodTypes.New
                                        && !context.Publish)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: registrationModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            registrationModel: registrationModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "Registrations_Invitee",
                            value: registrationModel.Invitee.ToString())
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: registrationModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: registrationModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Registrations_Timestamp",
                            css: "always-send",
                            value: registrationModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: registrationModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Registrations",
                    referenceId: registrationModel.RegistrationId,
                    referenceVer: registrationModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    registrationModel: registrationModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, RegistrationModel registrationModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(
                    _using: registrationModel.MethodType != BaseModel.MethodTypes.New
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
            RegistrationModel registrationModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, registrationModel: registrationModel));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            value: registrationModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: column,
                                baseModel: registrationModel),
                            preview: preview);
                        break;
                    case "MailAddress":
                        if (context.Action != "new")
                        {
                            column.EditorReadOnly = true;
                        }
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            value: registrationModel.MailAddress
                                .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                baseModel: registrationModel),
                            preview: preview);
                        break;
                    case "InviteeName":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            value: registrationModel.InviteeName
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: column,
                                baseModel: registrationModel),
                            preview: preview);
                        break;
                    case "LoginId":
                        if (context.Action == "login")
                        {
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                value: registrationModel.LoginId
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    baseModel: registrationModel),
                                preview: preview);
                        }
                        break;
                    case "Name":
                        if (context.Action != "new")
                        {
                            if (context.Action != "login")
                            {
                                column.EditorReadOnly = true;
                            }
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                value: registrationModel.Name
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    baseModel: registrationModel),
                                preview: preview);
                        }
                        break;
                    case "Password":
                        if (context.Action == "login")
                        {
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                value: registrationModel.Password
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    baseModel: registrationModel),
                                preview: preview);
                        }
                        break;
                    case "PasswordValidate":
                        if (context.Action == "login")
                        {
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                value: registrationModel.PasswordValidate
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    baseModel: registrationModel),
                                preview: preview);
                        }
                        break;
                    case "Language":
                        if (context.Action == "login")
                        {
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                value: registrationModel.Language
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: Permissions.ColumnPermissionType(
                                    context: context,
                                    ss: ss,
                                    column: column,
                                    baseModel: registrationModel),
                                preview: preview);
                        }
                        break;
                    case "Invitingflg":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            value: registrationModel.Invitingflg
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: Permissions.ColumnPermissionType(
                                context: context,
                                ss: ss,
                                column: column,
                                baseModel: registrationModel),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: registrationModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = registrationModel.ControlValue(
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
                        baseModel: registrationModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this RegistrationModel registrationModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "Ver":
                    return registrationModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "MailAddress":
                    return registrationModel.MailAddress
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "InviteeName":
                    return registrationModel.InviteeName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "LoginId":
                    return registrationModel.LoginId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Name":
                    return registrationModel.Name
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Password":
                    return registrationModel.Password
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "PasswordValidate":
                    return registrationModel.PasswordValidate
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Language":
                    return registrationModel.Language
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Invitingflg":
                    return registrationModel.Invitingflg
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return registrationModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return registrationModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return registrationModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return registrationModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return registrationModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return registrationModel.GetAttachments(columnName: column.Name)
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
            RegistrationModel registrationModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, int registrationId)
        {
            return EditorResponse(context, ss, new RegistrationModel(
                context, ss, registrationId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            Message message = null,
            string switchTargets = null)
        {
            registrationModel.MethodType = registrationModel.RegistrationId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new RegistrationsResponseCollection(
                context: context,
                registrationModel: registrationModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, registrationModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .Messages(context.Messages)
                    .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"));
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int registrationId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.RegistrationsWhere().TenantId(context.TenantId));
            var param = view.Param(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Registrations_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectRegistrations(
                        column: Rds.RegistrationsColumn().RegistrationsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectRegistrations(
                            column: Rds.RegistrationsColumn().RegistrationId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["RegistrationId"].ToInt())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(registrationId))
            {
                switchTargets.Add(registrationId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: registrationModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = registrationModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Registrations_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                registrationModel: registrationModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "RegistrationId":
                                res.Val(
                                    target: "#Registrations_RegistrationId" + idSuffix,
                                    value: registrationModel.RegistrationId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "MailAddress":
                                res.Val(
                                    target: "#Registrations_MailAddress" + idSuffix,
                                    value: registrationModel.MailAddress.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Invitee":
                                res.Val(
                                    target: "#Registrations_Invitee" + idSuffix,
                                    value: registrationModel.Invitee.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "InviteeName":
                                res.Val(
                                    target: "#Registrations_InviteeName" + idSuffix,
                                    value: registrationModel.InviteeName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "LoginId":
                                res.Val(
                                    target: "#Registrations_LoginId" + idSuffix,
                                    value: registrationModel.LoginId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Name":
                                res.Val(
                                    target: "#Registrations_Name" + idSuffix,
                                    value: registrationModel.Name.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Password":
                                res.Val(
                                    target: "#Registrations_Password" + idSuffix,
                                    value: registrationModel.Password.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Language":
                                res.Val(
                                    target: "#Registrations_Language" + idSuffix,
                                    value: registrationModel.Language.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Passphrase":
                                res.Val(
                                    target: "#Registrations_Passphrase" + idSuffix,
                                    value: registrationModel.Passphrase.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Invitingflg":
                                res.Val(
                                    target: "#Registrations_Invitingflg" + idSuffix,
                                    value: registrationModel.Invitingflg.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "UserId":
                                res.Val(
                                    target: "#Registrations_UserId" + idSuffix,
                                    value: registrationModel.UserId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "DeptId":
                                res.Val(
                                    target: "#Registrations_DeptId" + idSuffix,
                                    value: registrationModel.DeptId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "GroupId":
                                res.Val(
                                    target: "#Registrations_GroupId" + idSuffix,
                                    value: registrationModel.GroupId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Registrations_{column.Name}{idSuffix}",
                                            value: registrationModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Registrations_{column.Name}{idSuffix}",
                                            value: registrationModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Registrations_{column.Name}{idSuffix}",
                                            value: registrationModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Registrations_{column.Name}{idSuffix}",
                                            value: registrationModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Registrations_{column.Name}{idSuffix}",
                                            value: registrationModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Registrations_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Registrations_{column.Name}Field",
                                                    controlId: $"Registrations_{column.Name}",
                                                    columnName: column.ColumnName,
                                                    fieldCss: column.FieldCss
                                                        + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                                            ? " right-align"
                                                            : string.Empty),
                                                    fieldDescription: column.Description,
                                                    labelText: column.LabelText,
                                                    value: registrationModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: registrationModel)
                                                            != Permissions.ColumnPermissionTypes.Update,
                                                    allowDelete: column.AllowDeleteAttachments != false,
                                                    validateRequired: column.ValidateRequired != false),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                }
                                break;
                        }
                    }
                });
            return res;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Create(Context context, SiteSettings ss)
        {
            var registrationModel = new RegistrationModel(context, ss, registrationId : 0, formData: context.Forms);
            var invalid = RegistrationValidators.OnCreating(
                context: context,
                ss: ss,
                registrationModel: registrationModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var mailAddress = Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectMailAddresses(
                column: Rds.MailAddressesColumn().MailAddress(),
                where: Rds.MailAddressesWhere().OwnerId(context.UserId)));
            if (mailAddress.IsNullOrEmpty()) { return Messages.ResponseMailAddressHasNotSet(context: context, data: null).ToJson();}
            var username = Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectUsers(
                column: Rds.UsersColumn().Name(),
                where: Rds.UsersWhere().UserId(context.UserId)));
            var errorData = registrationModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.InviteMessage(
                            context: context,
                            data: registrationModel.Title.Value));
                    var passphrase = Strings.NewGuid();
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateRegistrations(
                            param: Rds.RegistrationsParam()
                                .Passphrase(passphrase)
                                .LoginId(registrationModel.MailAddress)
                                .Invitingflg("0")
                                .Invitee(context.UserId)
                                .InviteeName(username),
                            where: Rds.RegistrationsWhere().RegistrationId(registrationModel.RegistrationId)));
                    var outgoingMailModel = new OutgoingMailModel()
                    {
                        Title = new Title(Displays.InvitationMailTitle(
                            context: context, 
                            data: new string[] { context.TenantTitle
                            })),
                        Body = Displays.InvitationMailBody(
                            context: context,
                            data: new string[]
                            {
                                context.TenantTitle,
                                Locations.RegistrationUri(
                                    context: context,
                                    passphrase: passphrase)
                            }),
                        From = MimeKit.MailboxAddress.Parse(mailAddress),
                        To = registrationModel.MailAddress,
                        Bcc = Parameters.Mail.SupportFrom
                    };
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        outgoingMailModel.Send(context: context, ss: new SiteSettings());
                    });
                    return new ResponseCollection(context: context)
                        .Response("id", registrationModel.RegistrationId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : registrationModel.RegistrationId))
                        .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static Message CreatedMessage(
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: registrationModel.Title.Value);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = registrationModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, int registrationId)
        {
            var registrationModel = new RegistrationModel(
                context: context,
                ss: ss,
                registrationId: registrationId,
                formData: context.Forms);
            var invalid = RegistrationValidators.OnUpdating(
                context: context,
                ss: ss,
                registrationModel: registrationModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (registrationModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = registrationModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new RegistrationsResponseCollection(
                        context: context,
                        registrationModel: registrationModel);
                    return ResponseByUpdate(res, context, ss, registrationModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: registrationModel.Comments,
                            verType: registrationModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: registrationModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            RegistrationsResponseCollection res,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: registrationModel);
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
                    where: Rds.RegistrationsWhere().RegistrationId(registrationModel.RegistrationId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{registrationModel.RegistrationId}\"][data-latest]",
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
                        registrationModel: registrationModel,
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
                    .FieldResponse(context: context, ss: ss, registrationModel: registrationModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", registrationModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(registrationModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: registrationModel,
                        tableName: "Registrations"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: registrationModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: registrationModel.Comments,
                        deleteCommentId: registrationModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            List<Process> processes)
        {
            var process = processes?.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty()
                && o.MatchConditions);
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: registrationModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = registrationModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int registrationId)
        {
            var registrationModel = new RegistrationModel(context, ss, registrationId);
            var invalid = RegistrationValidators.OnDeleting(
                context: context,
                ss: ss,
                registrationModel: registrationModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = registrationModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: registrationModel.Title.MessageDisplay(context: context)));
                    var res = new RegistrationsResponseCollection(
                        context: context,
                        registrationModel: registrationModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int registrationId, Message message = null)
        {
            var registrationModel = new RegistrationModel(context: context, ss: ss, registrationId: registrationId);
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
                                registrationModel: registrationModel)));
            return new RegistrationsResponseCollection(
                context: context,
                registrationModel: registrationModel)
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
            RegistrationModel registrationModel)
        {
            new RegistrationCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.RegistrationsWhere().RegistrationId(registrationModel.RegistrationId),
                orderBy: Rds.RegistrationsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(registrationModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(registrationModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: registrationModelHistory.Ver == registrationModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: registrationModelHistory.Ver.ToString(),
                                            _using: registrationModelHistory.Ver < registrationModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        registrationModel: registrationModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.RegistrationsColumnCollection()
                .RegistrationId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.RegistrationsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int registrationId)
        {
            var registrationModel = new RegistrationModel(context: context, ss: ss, registrationId: registrationId);
            registrationModel.Get(
                context: context,
                ss: ss,
                where: Rds.RegistrationsWhere()
                    .RegistrationId(registrationModel.RegistrationId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            registrationModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, registrationModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        registrationId.ToString() 
                            + (registrationModel.VerType == Versions.VerTypes.History
                                ? "?ver=" + context.Forms.Int("Ver") 
                                : string.Empty)
                    }))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ResponseCollection ResponseByApproval(
            RegistrationsResponseCollection res,
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel)
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
                    where: Rds.RegistrationsWhere().RegistrationId(registrationModel.RegistrationId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{registrationModel.RegistrationId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            view: view,
                            dataRows: gridData.DataRows,
                            columns: columns))
                    .CloseDialog()
                    .Message(Messages.ApprovalMessage(
                        context: context,
                        data: registrationModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, registrationModel: registrationModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(registrationModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: registrationModel,
                        tableName: "Registrations"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.ApprovalMessage(
                        context: context,
                        data: registrationModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: registrationModel.Comments,
                        deleteCommentId: registrationModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ResponseCollection ResponseByApprovalRequest(
            Context context,
            SiteSettings ss,
            RegistrationsResponseCollection res,
            RegistrationModel registrationModel)
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
                    where: Rds.RegistrationsWhere().RegistrationId(registrationModel.RegistrationId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{registrationModel.RegistrationId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            view: view,
                            dataRows: gridData.DataRows,
                            columns: columns))
                    .CloseDialog()
                    .Message(Messages.ApprovalRequestMessage(
                        context: context,
                        data: registrationModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, registrationModel: registrationModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(registrationModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: registrationModel,
                        tableName: "Registrations"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.ApprovalRequestMessage(
                        context: context,
                        data: registrationModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: registrationModel.Comments,
                        deleteCommentId: registrationModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Login(Context context, SiteSettings ss)
        {
            if (!Parameters.Registration.Enabled)
            {
                return string.Empty;
            }
            var registrationModel = new RegistrationModel().Get(
                context: context,
                ss: ss,
                where: Rds.RegistrationsWhere()
                    .Passphrase(context.QueryStrings.Data("passphrase"), _operator: context.Sqls.Like));
            if (registrationModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                registrationModel.MethodType = BaseModel.MethodTypes.Edit;
                return Editor(
                    context: context,
                    ss: ss,
                    registrationModel: registrationModel);
            }
            else
            {
                return Editor(
                    context: context,
                    ss: ss,
                    registrationModel: registrationModel);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ApprovalRequest(Context context, SiteSettings ss, int registrationId)
        {
            if (!Parameters.Registration.Enabled)
            {
                return string.Empty;
            }
            var registrationModel = new RegistrationModel().Get(
                context: context,
                ss: ss,
                where: Rds.RegistrationsWhere()
                    .RegistrationId(registrationId));
            registrationModel.SetByForm(
                context: context,
                ss: ss,
                formData: context.Forms);
            if (registrationModel.Invitingflg == "1")
            {
                return Messages.ResponseApprovalRequestMessageRequesting(
                    context: context,
                    data: registrationModel.MailAddress)
                        .ToJson();
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Registrations_Password").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            var existsId = Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                column: Rds.UsersColumn().UserId(),
                where: Rds.UsersWhere().LoginId(
                    value: context.Sqls.EscapeValue(registrationModel.LoginId),
                    _operator: context.Sqls.LikeWithEscape)));
            if (existsId != 0)
            {
                return Messages.ResponseLoginIdAlreadyUse(context: context).ToJson();
            }
            if (registrationModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            registrationModel.Invitingflg = "1";
            var errorData = registrationModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var tenantTitle = Repository.ExecuteScalar_string(
                        context: context,
                        statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn().Title(),
                        where: Rds.TenantsWhere().TenantId(registrationModel.TenantId)));
                    var from = Libraries.Mails.Addresses.From(MimeKit.MailboxAddress.Parse(registrationModel.MailAddress));
                    new OutgoingMailModel()
                    {
                        Title = new Title(Displays.ApprovalRequestMailTitle(
                            context: context,
                            data: new string[] { tenantTitle })),
                        Body = Displays.ApprovalRequestMailBody(
                            context: context,
                            data: new string[]
                            {
                                tenantTitle,
                                Locations.RegistrationEditUri(
                                    context: context,
                                    id: registrationId.ToString())
                            }),
                        From = from,
                        To = Parameters.Registration.ApprovalRequestTo,
                        Bcc = Parameters.Mail.SupportFrom
                    }.Send(
                        context: context,
                        ss: new SiteSettings());
                    var res = new RegistrationsResponseCollection(
                        context: context,
                        registrationModel: registrationModel);
                    return ResponseByApprovalRequest(
                        context: context,
                        ss: ss,
                        res: res,
                        registrationModel: registrationModel)
                            .PrependComment(
                                context: context,
                                ss: ss,
                                column: ss.GetColumn(
                                    context: context,
                                    columnName: "Comments"),
                                comments: registrationModel.Comments,
                                verType: registrationModel.VerType)
                            .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: registrationModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Approval(Context context, SiteSettings ss, int registrationId)
        {
            if (!Parameters.Registration.Enabled)
            {
                return string.Empty;
            }
            var registrationModel = new RegistrationModel(
                context: context,
                ss: ss,
                registrationId: registrationId);
            switch (registrationModel.Invitingflg)
            {
                case "0":
                    return Messages.ResponseApprovalMessageInviting(
                        context: context,
                        data: registrationModel.MailAddress)
                            .ToJson();
                case "2":
                    return Messages.ResponseApprovalMessageInvited(
                        context: context,
                        data: registrationModel.MailAddress)
                            .ToJson();
                default:
                    break;
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateRegistrations(
                    param: Rds.RegistrationsParam().Invitingflg("2"),
                    where: Rds.RegistrationsWhere().RegistrationId(registrationId)));
            var mailAddress = Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectMailAddresses(
                column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere().OwnerId(context.UserId)));
            if (mailAddress.IsNullOrEmpty())
            {
                return Messages.ResponseMailAddressHasNotSet(context: context).ToJson();
            }
            new OutgoingMailModel()
            {
                Title = new Title(Displays.ApprovalMailTitle(
                    context: context,
                    data: new string[] { context.TenantTitle })),
                Body = Displays.ApprovalMailBody(
                    context: context,
                    data: new string[]
                    {
                        context.TenantTitle,
                        Locations.ApprovaUri(context: context)
                    }),
                From = MimeKit.MailboxAddress.Parse(mailAddress),
                To = registrationModel.MailAddress,
                Bcc = Parameters.Mail.SupportFrom
            }.Send(
                context: context,
                ss: new SiteSettings());
            registrationModel.Approval(context: context);
            var res = new RegistrationsResponseCollection(
                context: context,
                registrationModel: registrationModel);
            return ResponseByApproval(res, context, ss, registrationModel)
                .PrependComment(
                    context: context,
                    ss: ss,
                    column: ss.GetColumn(
                        context: context,
                        columnName: "Comments"),
                    comments: registrationModel.Comments,
                    verType: registrationModel.VerType)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                offset: context.Forms.Int("GridOffset"));
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
            var where = BulkDeleteWhere(
                context: context,
                ss: ss,
                selected: selected,
                negative: negative);
            return Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteRegistrations(
                        factory: context,
                        where: where),
                    Rds.RowCount()
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
                where: Rds.RegistrationsWhere()
                    .RegistrationId_In(
                        value: selected.Select(o => o.ToInt()),
                        negative: negative,
                        _using: selected.Any()));
        }
    }
}
