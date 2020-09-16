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
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Users",
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
                    .MoveDialog(context: context, bulk: true)
                    .ImportSettingsDialog(context: context)
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static GridData GetGridData(
            Context context, SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls(context: context);
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
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridOffset")
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
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
            string action = "GridRows")
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
                            action: action))
                .TBody(action: () => hb
                    .GridRows(
                        context: context,
                        ss: ss,
                        dataRows: gridData.DataRows,
                        columns: columns,
                        recordSelector: null,
                        checkRow: checkRow));
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
            ss.SetColumnAccessControls(context: context);
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
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("UserId")}\"][data-latest]",
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
                            idColumn: "UserId"))
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            UserModel userModel,
            int? tabIndex = null)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
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
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserId,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.Ver,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "LoginId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LoginId,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Name":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Name,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "UserCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.UserCode,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Birthday":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Birthday,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Gender":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Gender,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Language":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Language,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "TimeZoneInfo":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TimeZoneInfo,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "DeptCode":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.DeptCode,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Dept":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Dept,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.Body,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "LastLoginTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LastLoginTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "PasswordExpirationTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordExpirationTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "PasswordChangeTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.PasswordChangeTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "NumberOfLogins":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfLogins,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "NumberOfDenial":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.NumberOfDenial,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "TenantManager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.TenantManager,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Disabled":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Disabled,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "Lockout":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.Lockout,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "LockoutCounter":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LockoutCounter,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "LdapSearchRoot":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.LdapSearchRoot,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
                    case "SynchronizedTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: userModel.SynchronizedTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.Comments,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.Creator,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.Updator,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.CreatedTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                    value: userModel.UpdatedTime,
                                    tabIndex: tabIndex)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex);
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
                                            value: userModel.Class(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                                            value: userModel.Num(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                                            value: userModel.Date(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                                            value: userModel.Description(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                                            value: userModel.Check(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                                            value: userModel.Attachments(columnName: column.Name),
                                            tabIndex: tabIndex)
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty,
                                            tabIndex: tabIndex);
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
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = userModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = userModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = userModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = userModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = userModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = userModel.Attachments(columnName: column.Name).GridText(
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
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: userModel.VerType,
                methodType: userModel.MethodType,
                referenceType: "Users",
                title: userModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Users(context: context) + " - " + Displays.New(context: context)
                    : userModel.Title.Value,
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
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
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
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
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
                .CopyDialog(
                    context: context,
                    referenceType: "Users",
                    id: userModel.UserId)
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
            var mine = userModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, userModel: userModel));
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
                    methodType: userModel.MethodType,
                    value: value,
                    columnPermissionType: column.ColumnPermissionType(context: context),
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
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return userModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return userModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return userModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return userModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return userModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return userModel.Attachments(columnName: column.Name)
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
                Rds.ExecuteScalar_bool(
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
                        text: Displays.ChangePassword(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ChangePasswordDialog");
                }
                if (context.User?.TenantManager == true)
                {
                    hb.Button(
                        text: Displays.ResetPassword(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openDialog($(this));",
                        icon: "ui-icon-person",
                        selector: "#ResetPasswordDialog");
                }
            }
            if (context.HasPrivilege
               && context.User.Id != userModel.UserId
               && !userModel.Disabled)
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
                context, ss, userId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            Message message = null,
            string switchTargets = null)
        {
            userModel.MethodType = BaseModel.MethodTypes.Edit;
            return new UsersResponseCollection(userModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, userModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
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
                var switchTargets = Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: where)) <= Parameters.General.SwitchTargetsLimit
                            ? Rds.ExecuteTable(
                                context: context,
                                statements: Rds.SelectUsers(
                                    column: Rds.UsersColumn().UserId(),
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
            var mine = userModel.Mine(context: context);
            ss.GetEditorColumnNames()
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "UserId":
                            res.Val(
                                "#Users_UserId" + idSuffix,
                                userModel.UserId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "LoginId":
                            res.Val(
                                "#Users_LoginId" + idSuffix,
                                userModel.LoginId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "GlobalId":
                            res.Val(
                                "#Users_GlobalId" + idSuffix,
                                userModel.GlobalId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Name":
                            res.Val(
                                "#Users_Name" + idSuffix,
                                userModel.Name.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "UserCode":
                            res.Val(
                                "#Users_UserCode" + idSuffix,
                                userModel.UserCode.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Password":
                            res.Val(
                                "#Users_Password" + idSuffix,
                                userModel.Password.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "LastName":
                            res.Val(
                                "#Users_LastName" + idSuffix,
                                userModel.LastName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "FirstName":
                            res.Val(
                                "#Users_FirstName" + idSuffix,
                                userModel.FirstName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Birthday":
                            res.Val(
                                "#Users_Birthday" + idSuffix,
                                userModel.Birthday.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Gender":
                            res.Val(
                                "#Users_Gender" + idSuffix,
                                userModel.Gender.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Language":
                            res.Val(
                                "#Users_Language" + idSuffix,
                                userModel.Language.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "TimeZone":
                            res.Val(
                                "#Users_TimeZone" + idSuffix,
                                userModel.TimeZone.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptId":
                            res.Val(
                                "#Users_DeptId" + idSuffix,
                                userModel.DeptId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "FirstAndLastNameOrder":
                            res.Val(
                                "#Users_FirstAndLastNameOrder" + idSuffix,
                                userModel.FirstAndLastNameOrder.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Users_Body" + idSuffix,
                                userModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "LastLoginTime":
                            res.Val(
                                "#Users_LastLoginTime" + idSuffix,
                                userModel.LastLoginTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "PasswordExpirationTime":
                            res.Val(
                                "#Users_PasswordExpirationTime" + idSuffix,
                                userModel.PasswordExpirationTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "PasswordChangeTime":
                            res.Val(
                                "#Users_PasswordChangeTime" + idSuffix,
                                userModel.PasswordChangeTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumberOfLogins":
                            res.Val(
                                "#Users_NumberOfLogins" + idSuffix,
                                userModel.NumberOfLogins.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumberOfDenial":
                            res.Val(
                                "#Users_NumberOfDenial" + idSuffix,
                                userModel.NumberOfDenial.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "TenantManager":
                            res.Val(
                                "#Users_TenantManager" + idSuffix,
                                userModel.TenantManager);
                            break;
                        case "Disabled":
                            res.Val(
                                "#Users_Disabled" + idSuffix,
                                userModel.Disabled);
                            break;
                        case "Lockout":
                            res.Val(
                                "#Users_Lockout" + idSuffix,
                                userModel.Lockout);
                            break;
                        case "LockoutCounter":
                            res.Val(
                                "#Users_LockoutCounter" + idSuffix,
                                userModel.LockoutCounter.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ApiKey":
                            res.Val(
                                "#Users_ApiKey" + idSuffix,
                                userModel.ApiKey.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "SecondaryAuthenticationCode":
                            res.Val(
                                "#Users_SecondaryAuthenticationCode" + idSuffix,
                                userModel.SecondaryAuthenticationCode.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "SecondaryAuthenticationCodeExpirationTime":
                            res.Val(
                                "#Users_SecondaryAuthenticationCodeExpirationTime" + idSuffix,
                                userModel.SecondaryAuthenticationCodeExpirationTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "LdapSearchRoot":
                            res.Val(
                                "#Users_LdapSearchRoot" + idSuffix,
                                userModel.LdapSearchRoot.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "SynchronizedTime":
                            res.Val(
                                "#Users_SynchronizedTime" + idSuffix,
                                userModel.SynchronizedTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Users_{column.Name}{idSuffix}",
                                        userModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Users_{column.Name}{idSuffix}",
                                        userModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Users_{column.Name}{idSuffix}",
                                        userModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Users_{column.Name}{idSuffix}",
                                        userModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Users_{column.Name}{idSuffix}",
                                        userModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Users_{column.Name}Field",
                                        new HtmlBuilder()
                                            .FieldAttachments(
                                                context: context,
                                                fieldId: $"Users_{column.Name}Field",
                                                controlId: $"Users_{column.Name}",
                                                columnName: column.ColumnName,
                                                fieldCss: column.FieldCss
                                                    + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                                        ? " right-align"
                                                        : string.Empty),
                                                fieldDescription: column.Description,
                                                labelText: column.LabelText,
                                                value: userModel.Attachments(columnName: column.Name).ToJson(),
                                                readOnly: column.ColumnPermissionType(context: context)
                                                    != Permissions.ColumnPermissionTypes.Update));
                                    break;
                            }
                            break;
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
            var userModel = new UserModel(
                context: context,
                ss: ss,
                userId: 0,
                formData: context.Forms);
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
            var errorData = userModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: userModel.Title.Value));
                    return new ResponseCollection()
                        .Response("id", userModel.UserId.ToString())
                        .SetMemory("formChanged", false)
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
            var errorData = userModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new UsersResponseCollection(userModel);
                    return ResponseByUpdate(res, context, ss, userModel)
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
            UserModel userModel)
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            recordSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: userModel.Title.DisplayValue));
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
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", userModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: userModel,
                        tableName: "Users"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: userModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: userModel.Comments,
                        deleteCommentId: userModel.DeleteCommentId)
                    .ClearFormData();
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
                            data: userModel.Title.Value));
                    var res = new UsersResponseCollection(userModel);
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
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
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
                                userModel: userModel)));
            return new UsersResponseCollection(userModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
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
            ss.SetColumnAccessControls(
                context: context,
                mine: userModel.Mine(context: context));
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
                if(selector.All == false && selector.Selected.Any() == false)
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
                if (Rds.ExecuteScalar_int(
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
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteMailAddresses(
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(sub: Rds.SelectUsers(
                                column: Rds.UsersColumn().UserId(),
                                where: where))
                            .OwnerType("Users")),
                    Rds.DeleteUsers(where: where),
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
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
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
                var mailAddressHash = new Dictionary<string, string>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .Where(o => o.TypeCs != "Attachments")
                        .FirstOrDefault();
                    if (column?.ColumnName == "LoginId")
                    {
                        idColumn = data.Index;
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
                var userHash = new Dictionary<int, UserModel>();
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
                                break;
                            case "TimeZone":
                                userModel.TimeZone = recordingData.ToString();
                                break;
                            case "DeptId":
                                userModel.DeptId = recordingData.ToInt();
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
                            case "Disabled":
                                userModel.Disabled = recordingData.ToBool();
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
                        }
                    });
                    userHash.Add(data.Index, userModel);
                });
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
                }
                var insertCount = 0;
                var updateCount = 0;
                foreach (var userModel in userHash.Values)
                {
                    if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        var mailAddressUpdated = UpdateMailAddresses(context, userModel);
                        if (userModel.Updated(context: context))
                        {
                            var errorData = userModel.Update(
                                context: context,
                                ss: ss,
                                updateMailAddresses: false,
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
                    Rds.ExecuteNonQuery(
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
            return new ResponseCollection()
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
                    Displays.MailAddress(context: context),
                    ",",
                    Displays.Password(context: context),
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
                                        mine: userModel.Mine(context: context))).Join(),
                                ",",
                                "\"" + Rds.ExecuteTable(
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
                encoding: context.QueryStrings.Data("encoding"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenChangePasswordDialog(Context context, SiteSettings ss)
        {
            var invalid = UserValidators.OnPasswordChange(
                context: context);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return new ResponseCollection()
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
                : new UsersResponseCollection(userModel)
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
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (Parameters.Security.JoeAccountCheck
                && context.Forms.Data("Users_AfterResetPassword") == userModel.LoginId)
            {
                return Error.Types.JoeAccountCheck.MessageJson(context: context);
            }
            foreach(var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!context.Forms.Data("Users_AfterResetPassword").RegexExists(policy.Regex))
                {
                    return policy.ResponseMessage(context: context).ToJson();
                }
            }
            var error = userModel.ResetPassword(context: context);
            return error.Has()
                ? error.MessageJson(context: context)
                : new UsersResponseCollection(userModel)
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
                : ResponseMailAddresses(userModel, selected);
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
                : ResponseMailAddresses(userModel, selected);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ResponseMailAddresses(
            UserModel userModel, IEnumerable<string> selected)
        {
            return new ResponseCollection()
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
                                controlCss: "button-icon validate",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "ChangePassword",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon",
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
                                    controlCss: "button-icon validate",
                                    onClick: "$p.send($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ResetPassword",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
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
                verType: Versions.VerTypes.Latest,
                useBreadcrumb: false,
                useTitle: false,
                useSearch: false,
                useNavigationMenu: false,
                methodType: BaseModel.MethodTypes.Edit,
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
                                            .Div(id: "Tenants")
                                            .Field(
                                                context: context,
                                                ss: ss,
                                                column: ss.GetColumn(
                                                    context: context,
                                                    columnName: "RememberMe")))
                                        .Div(id: "LoginCommands", action: () => hb
                                            .Button(
                                                controlId: "Login",
                                                controlCss: "button-icon button-right-justified validate",
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
                                                    css:"ssoLoginMessage",
                                                    action: ()=>hb
                                                        .Text(Displays.SsoLoginMessage(context: context)))
                                                .Button(
                                                    controlId: "SSOLogin",
                                                    controlCss: "button-icon button-right-justified",
                                                    text: Displays.SsoLogin(context: context),
                                                    href: "../Saml2/SignIn",
                                                    onClick: "",
                                                    icon: "ui-icon-unlocked"),
                                            _using: Parameters.Authentication.Provider == "SAML")
                                        .Div(
                                            id: "LoginGuideBottom",
                                            action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                                context:context,
                                                id: "LoginGuideBottom"))))
                                .Div(id: "SecondaryAuthentications")))
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
                        _using: Parameters.Service.Demo,
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
                                                controlCss: "button-icon validate",
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
                                    controlCss: "button-icon validate",
                                    onClick: "$p.changePasswordAtLogin($(this));",
                                    icon: "ui-icon-disk",
                                    action: "ChangePasswordAtLogin",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
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
            var listItemCollection = Rds.ExecuteTable(
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
            return hb.FieldSet(id: "FieldSetMailAddresses", action: () => hb
                .FieldSelectable(
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
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
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
            return hb
                .Div(id: "EditorTabsContainer", css: "max", action: () => hb
                    .Ul(id: "EditorTabs", action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#FieldSetGeneral",
                                text: Displays.General(context: context))))
                    .FieldSet(id: "FieldSetGeneral", action: () => hb
                        .FieldText(
                            controlId: "ApiKey",
                            fieldCss: "field-wide",
                            labelText: Displays.ApiKey(context: context),
                            text: userModel.ApiKey))
                    .Div(
                        id: "ApiEditorCommands",
                        action: () => hb
                            .Button(
                                controlId: "CreateApiKey",
                                controlCss: "button-icon",
                                text: userModel.ApiKey.IsNullOrEmpty()
                                    ? Displays.Create(context: context)
                                    : Displays.ReCreate(context: context),
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "CreateApiKey",
                                method: "post")
                            .Button(
                                controlId: "DeleteApiKey",
                                controlCss: "button-icon",
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
                return new ResponseCollection()
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
                return new ResponseCollection()
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
        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss)
        {
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null)
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
                var invalid = SiteValidators.OnReading(
                    context,
                    siteModel.SitesSiteSettings(context, siteId.Value),
                    siteModel);
                switch (invalid.Type)
                {
                    case Error.Types.None: break;
                    default: return ApiResults.Error(
                        context: context,
                        errorData: invalid);
                }
            }
            var siteUsers = siteModel != null
                ? SiteInfo.SiteUsers(context, siteModel.InheritPermission)?
                .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var userCollection = new UserCollection(
                context: context,
                ss: ss,
                where: view.Where(context: context, ss: ss)
                .Users_TenantId(context.TenantId)
                .SqlWhereLike(
                    tableName: "Users",
                    name: "SearchText",
                    searchText: view.ColumnFilterHash
                        ?.Where(f => f.Key == "SearchText")
                        ?.Select(f => f.Value)
                        ?.FirstOrDefault(),
                    clauseCollection: new List<string>()
                    {
                        Rds.Users_LoginId_WhereLike(),
                        Rds.Users_Name_WhereLike(),
                        Rds.Users_UserCode_WhereLike(),
                        Rds.Users_Body_WhereLike(),
                        Rds.Depts_DeptCode_WhereLike(),
                        Rds.Depts_DeptName_WhereLike(),
                        Rds.Depts_Body_WhereLike()
                    }),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api.Offset,
                pageSize: pageSize);
            var users = siteUsers == null
                ? userCollection
                : userCollection.Join(siteUsers, c => c.UserId, s => s, (c, s) => c);
            ss.SetColumnAccessControls(context: context);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Offset = api.Offset,
                    PageSize = pageSize,
                    TotalCount = users.Count(),
                    Data = users.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
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
                default: return HtmlTemplates.Error(
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
                return new ResponseCollection()
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
                return new ResponseCollection()
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
                default: return HtmlTemplates.Error(
                    context: context,
                    errorData: invalid);
            }
            SessionUtilities.Remove(
                context: context,
                key: "SwitchLoginId",
                page: false);
            context = new Context();
            return new ResponseCollection()
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
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .UserId(context.UserId),
                    param: Rds.UsersParam()
                        .UserSettings(context.UserSettings.RecordingJson()),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            return new ResponseCollection().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult CreateByApi(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.UsersLimit(context: context))
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.UsersLimit));
            }
            var userModel = new UserModel(context, ss, 0, setByApi: true);
            var invalid = UserValidators.OnCreating(
                context: context,
                ss: ss,
                userModel: userModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (!(context.RequestDataString.Deserialize<UserApiModel>().Password ?? "").RegexExists(policy.Regex))
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.PasswordPolicyViolation));
                }
            }
            foreach(var column in ss.Columns
                .Where(o => o.ValidateRequired ?? false )
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
        public static System.Web.Mvc.ContentResult UpdateByApi(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context, ss, userId: userId, setByApi: true);
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
            foreach (var policy in Parameters.Security.PasswordPolicies.Where(o => o.Enabled))
            {
                if (userModel.Password_Updated(context: context) && !userModel.Password.RegexExists(policy.Regex))
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.PasswordPolicyViolation));
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
            var errorData = userModel.Update(
                context: context,
                ss: ss,
                updateMailAddresses: false);
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
        public static System.Web.Mvc.ContentResult DeleteByApi(Context context, SiteSettings ss, int userId)
        {
            var userModel = new UserModel(context, ss, userId: userId, setByApi: true);
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
    }
}
