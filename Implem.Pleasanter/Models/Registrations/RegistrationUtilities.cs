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
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Registrations",
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("RegistrationId")).ToJson())
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
            var checkRow = ss.CheckRow(context: context);
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
                        gridSelector: null,
                        checkRow: checkRow));
        }

        private static SqlWhereCollection SelectedWhere(
            Context context, SiteSettings ss)
        {
            var selector = new GridSelector(context: context);
            return !selector.Nothing
                ? Rds.RegistrationsWhere().RegistrationId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long registrationId)
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
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("RegistrationId")}\"][data-latest]",
                        new HtmlBuilder().Tr(
                            context: context,
                            ss: ss,
                            dataRow: dataRow,
                            columns: ss.GetGridColumns(
                                context: context,
                                view: view,
                                checkPermission: true),
                            gridSelector: null,
                            editRow: true,
                            checkRow: false,
                            idColumn: "RegistrationId"))
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            RegistrationModel registrationModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
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
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "MailAddress":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.MailAddress)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "InviteeName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.InviteeName)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.LoginId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.Name)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Invitingflg":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: registrationModel.Invitingflg)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.Comments)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.Creator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.Updator)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: registrationModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                            value: registrationModel.Class(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                                            value: registrationModel.Num(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                                            value: registrationModel.Date(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                                            value: registrationModel.Description(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                                            value: registrationModel.Check(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                                            value: registrationModel.Attachments(columnName: column.Name))
                                        : hb.Td(
                                            context: context,
                                            column: column,
                                            value: string.Empty);
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
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = registrationModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = registrationModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = registrationModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = registrationModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = registrationModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = registrationModel.Attachments(columnName: column.Name).GridText(
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
            ss.SetColumnAccessControls(
                context: context,
                mine: registrationModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: registrationModel.VerType,
                methodType: registrationModel.MethodType,
                referenceType: "Registrations",
                title: registrationModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Registrations(context: context) + " - " + Displays.New(context: context)
                    : registrationModel.Title.Value,
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
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
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
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
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
                .CopyDialog(
                    context: context,
                    referenceType: "Registrations",
                    id: registrationModel.RegistrationId)
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
            var mine = registrationModel.Mine(context: context);
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
                            methodType: registrationModel.MethodType,
                            value: registrationModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
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
                        methodType: registrationModel.MethodType,
                        value: registrationModel.MailAddress
                            .ToControl(context: context, ss: ss, column: column),
                        columnPermissionType: column.ColumnPermissionType(context: context),
                        preview: preview);
                        break;
                    case "InviteeName":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: registrationModel.MethodType,
                            value: registrationModel.InviteeName
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "LoginId":
                        if (context.Action == "login")
                        {
                            hb.Field(
                                context: context,
                                ss: ss,
                                column: column,
                                methodType: registrationModel.MethodType,
                                value: registrationModel.LoginId
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: column.ColumnPermissionType(context: context),
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
                                methodType: registrationModel.MethodType,
                                value: registrationModel.Name
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: column.ColumnPermissionType(context: context),
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
                                methodType: registrationModel.MethodType,
                                value: registrationModel.Password
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: column.ColumnPermissionType(context: context),
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
                                methodType: registrationModel.MethodType,
                                value: registrationModel.PasswordValidate
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: column.ColumnPermissionType(context: context),
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
                                methodType: registrationModel.MethodType,
                                value: registrationModel.Language
                                    .ToControl(context: context, ss: ss, column: column),
                                columnPermissionType: column.ColumnPermissionType(context: context),
                                preview: preview);
                        }
                        break;
                    case "Invitingflg":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: registrationModel.MethodType,
                            value: registrationModel.Invitingflg
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
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
                    methodType: registrationModel.MethodType,
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
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return registrationModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return registrationModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return registrationModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return registrationModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return registrationModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return registrationModel.Attachments(columnName: column.Name)
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
                context, ss, registrationId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            RegistrationModel registrationModel,
            Message message = null,
            string switchTargets = null)
        {
            registrationModel.MethodType = BaseModel.MethodTypes.Edit;
            return new RegistrationsResponseCollection(registrationModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, registrationModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int registrationId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.RegistrationsWhere().TenantId(context.TenantId));
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
                if (Rds.ExecuteScalar_long(
                    context: context,
                    statements: Rds.SelectRegistrations(
                        column: Rds.RegistrationsColumn().RegistrationsCount(),
                        join: join,
                        where: where)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Rds.ExecuteTable(
                        context: context,
                        statements: Rds.SelectRegistrations(
                            column: Rds.RegistrationsColumn().RegistrationId(),
                            join: join,
                            where: where,
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
            var mine = registrationModel.Mine(context: context);
            ss.GetEditorColumnNames()
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "RegistrationId":
                            res.Val(
                                "#Registrations_RegistrationId" + idSuffix,
                                registrationModel.RegistrationId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "MailAddress":
                            res.Val(
                                "#Registrations_MailAddress" + idSuffix,
                                registrationModel.MailAddress.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Invitee":
                            res.Val(
                                "#Registrations_Invitee" + idSuffix,
                                registrationModel.Invitee.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "InviteeName":
                            res.Val(
                                "#Registrations_InviteeName" + idSuffix,
                                registrationModel.InviteeName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "LoginId":
                            res.Val(
                                "#Registrations_LoginId" + idSuffix,
                                registrationModel.LoginId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Name":
                            res.Val(
                                "#Registrations_Name" + idSuffix,
                                registrationModel.Name.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Password":
                            res.Val(
                                "#Registrations_Password" + idSuffix,
                                registrationModel.Password.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Language":
                            res.Val(
                                "#Registrations_Language" + idSuffix,
                                registrationModel.Language.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Passphrase":
                            res.Val(
                                "#Registrations_Passphrase" + idSuffix,
                                registrationModel.Passphrase.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Invitingflg":
                            res.Val(
                                "#Registrations_Invitingflg" + idSuffix,
                                registrationModel.Invitingflg.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "UserId":
                            res.Val(
                                "#Registrations_UserId" + idSuffix,
                                registrationModel.UserId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptId":
                            res.Val(
                                "#Registrations_DeptId" + idSuffix,
                                registrationModel.DeptId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "GroupId":
                            res.Val(
                                "#Registrations_GroupId" + idSuffix,
                                registrationModel.GroupId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Registrations_{column.Name}{idSuffix}",
                                        registrationModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Registrations_{column.Name}{idSuffix}",
                                        registrationModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Registrations_{column.Name}{idSuffix}",
                                        registrationModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Registrations_{column.Name}{idSuffix}",
                                        registrationModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Registrations_{column.Name}{idSuffix}",
                                        registrationModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Registrations_{column.Name}Field",
                                        new HtmlBuilder()
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
                                                value: registrationModel.Attachments(columnName: column.Name).ToJson(),
                                                readOnly: column.ColumnPermissionType(context: context)
                                                    != Permissions.ColumnPermissionTypes.Update));
                                    break;
                            }
                            break;
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
            var mailAddress = Rds.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectMailAddresses(
                column: Rds.MailAddressesColumn().MailAddress(),
                where: Rds.MailAddressesWhere().OwnerId(context.UserId)));
            if (mailAddress.IsNullOrEmpty()) { return Messages.ResponseMailAddressHasNotSet(context: context, data: null).ToJson();}
            var username = Rds.ExecuteScalar_string(
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
                    Rds.ExecuteNonQuery(
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
                        From = new System.Net.Mail.MailAddress(mailAddress),
                        To = registrationModel.MailAddress,
                        Bcc = Parameters.Mail.SupportFrom
                    };
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        outgoingMailModel.Send(context: context, ss: new SiteSettings());
                    });
                    return new ResponseCollection()
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
            var errorData = registrationModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new RegistrationsResponseCollection(registrationModel);
                    return ResponseByUpdate(res, context, ss, registrationModel)
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: registrationModel.Title.DisplayValue));
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
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", registrationModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: registrationModel,
                        tableName: "Registrations"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
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
                            data: registrationModel.Title.Value));
                    var res = new RegistrationsResponseCollection(registrationModel);
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
            ss.SetColumnAccessControls(
                context: context,
                mine: registrationModel.Mine(context: context));
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
            return new RegistrationsResponseCollection(registrationModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
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
            ss.SetColumnAccessControls(
                context: context,
                mine: registrationModel.Mine(context: context));
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
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
                    .Html("#HeaderTitle", registrationModel.Title.Value)
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
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
                    .Html("#HeaderTitle", registrationModel.Title.Value)
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
                    .Passphrase(context.QueryStrings.Data("passphrase")));
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
            var existsId = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                column: Rds.UsersColumn().UserId(),
                where: Rds.UsersWhere().LoginId(registrationModel.LoginId)));
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
                    var tenantTitle = Rds.ExecuteScalar_string(
                        context: context,
                        statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn().Title(),
                        where: Rds.TenantsWhere().TenantId(registrationModel.TenantId)));
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
                        From = new System.Net.Mail.MailAddress(registrationModel.MailAddress),
                        To = Parameters.Registration.ApprovalRequestTo,
                        Bcc = Parameters.Mail.SupportFrom
                    }.Send(
                        context: context,
                        ss: new SiteSettings());
                    var res = new RegistrationsResponseCollection(registrationModel);
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
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateRegistrations(
                    param: Rds.RegistrationsParam().Invitingflg("2"),
                    where: Rds.RegistrationsWhere().RegistrationId(registrationId)));
            var mailAddress = Rds.ExecuteScalar_string(
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
                From = new System.Net.Mail.MailAddress(mailAddress),
                To = registrationModel.MailAddress,
                Bcc = Parameters.Mail.SupportFrom
            }.Send(
                context: context,
                ss: new SiteSettings());
            registrationModel.Approval(context: context);
            var res = new RegistrationsResponseCollection(registrationModel);
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
                var selector = new GridSelector(context: context);
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
                    Rds.DeleteRegistrations(where: where),
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
