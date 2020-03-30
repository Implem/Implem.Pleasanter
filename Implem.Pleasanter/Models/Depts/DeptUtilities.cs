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
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                referenceType: "Depts",
                script: JavaScripts.ViewMode(viewMode),
                title: Displays.Depts(context: context) + " - " + Displays.List(context: context),
                action: () =>
                {
                    hb
                        .Form(
                            attributes: new HtmlAttributes()
                                .Id("MainForm")
                                .Class("main-form")
                                .Action(Locations.Action(
                                    context: context,
                                    controller: "Depts")),
                            action: () => hb
                                .ViewFilters(context: context, ss: ss, view: view)
                                .Aggregations(
                                    context: context,
                                    ss: ss,
                                    view: view)
                                .Div(id: "ViewModeContainer", action: () => hb
                                    .Grid(
                                        context: context,
                                        ss: ss,
                                        gridData: gridData,
                                        view: view))
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: Versions.VerTypes.Latest)
                                .Div(css: "margin-bottom")
                                .Hidden(
                                    controlId: "BaseUrl",
                                    value: Locations.BaseUrl(context: context)))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ImportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.Import(context: context)))
                        .Div(attributes: new HtmlAttributes()
                            .Id("ExportSettingsDialog")
                            .Class("dialog")
                            .Title(Displays.ExportSettings(context: context)));
                }).ToString();
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            string viewMode,
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
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Depts",
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
                                action: ()=> hb
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("DeptId")).ToJson())
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
            var checkRow = !ss.GridColumnsHasSources();
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
                ? Rds.DeptsWhere().DeptId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long deptId)
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
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("DeptId")}\"][data-latest]",
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
                            idColumn: "DeptId"))
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            DeptModel deptModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
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
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: deptModel.DeptId)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: deptModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: deptModel.DeptCode)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: deptModel.Dept)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
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
                                    value: deptModel.Body)
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
                                    value: deptModel.Comments)
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
                                    value: deptModel.Creator)
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
                                    value: deptModel.Updator)
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
                                    value: deptModel.CreatedTime)
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
                                    value: deptModel.UpdatedTime)
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
                                            value: deptModel.Class(columnName: column.Name))
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
                                            value: deptModel.Num(columnName: column.Name))
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
                                            value: deptModel.Date(columnName: column.Name))
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
                                            value: deptModel.Description(columnName: column.Name))
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
                                            value: deptModel.Check(columnName: column.Name))
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
                                            value: deptModel.Attachments(columnName: column.Name))
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
                    case "Dept": value = deptModel.Dept.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = deptModel.Body.GridText(
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
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                value = deptModel.Class(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = deptModel.Num(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Date":
                                value = deptModel.Date(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = deptModel.Description(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = deptModel.Check(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = deptModel.Attachments(columnName: column.Name).GridText(
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
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                verType: deptModel.VerType,
                methodType: deptModel.MethodType,
                referenceType: "Depts",
                title: deptModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Depts(context: context) + " - " + Displays.New(context: context)
                    : deptModel.Title.Value,
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
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
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
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
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
                .CopyDialog(
                    context: context,
                    referenceType: "Depts",
                    id: deptModel.DeptId)
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
            var mine = deptModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, deptModel: deptModel));
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
                    methodType: deptModel.MethodType,
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
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            return deptModel.Class(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return deptModel.Num(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return deptModel.Date(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return deptModel.Description(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return deptModel.Check(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return deptModel.Attachments(columnName: column.Name)
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
                context, ss, deptId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            DeptModel deptModel,
            Message message = null,
            string switchTargets = null)
        {
            deptModel.MethodType = BaseModel.MethodTypes.Edit;
            return new DeptsResponseCollection(deptModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, deptModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int deptId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.DeptsWhere().TenantId(context.TenantId));
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
                if (Rds.ExecuteScalar_long(
                    context: context,
                    statements: Rds.SelectDepts(
                        column: Rds.DeptsColumn().DeptsCount(),
                        join: join,
                        where: where)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Rds.ExecuteTable(
                        context: context,
                        statements: Rds.SelectDepts(
                            column: Rds.DeptsColumn().DeptId(),
                            join: join,
                            where: where,
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
            var mine = deptModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "DeptId":
                            res.Val(
                                "#Depts_DeptId" + idSuffix,
                                deptModel.DeptId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptCode":
                            res.Val(
                                "#Depts_DeptCode" + idSuffix,
                                deptModel.DeptCode.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DeptName":
                            res.Val(
                                "#Depts_DeptName" + idSuffix,
                                deptModel.DeptName.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Depts_Body" + idSuffix,
                                deptModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    res.Val(
                                        $"#Depts_{column.Name}{idSuffix}",
                                        deptModel.Class(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Num":
                                    res.Val(
                                        $"#Depts_{column.Name}{idSuffix}",
                                        deptModel.Num(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Date":
                                    res.Val(
                                        $"#Depts_{column.Name}{idSuffix}",
                                        deptModel.Date(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Description":
                                    res.Val(
                                        $"#Depts_{column.Name}{idSuffix}",
                                        deptModel.Description(columnName: column.Name).ToResponse(
                                            context: context,
                                            ss: ss,
                                            column: column));
                                    break;
                                case "Check":
                                    res.Val(
                                        $"#Depts_{column.Name}{idSuffix}",
                                        deptModel.Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    res.ReplaceAll(
                                        $"#Depts_{column.Name}Field",
                                        new HtmlBuilder()
                                            .FieldAttachments(
                                                context: context,
                                                fieldId: $"Depts_{column.Name}Field",
                                                controlId: $"Depts_{column.Name}",
                                                columnName: column.ColumnName,
                                                fieldCss: column.FieldCss,
                                                fieldDescription: column.Description,
                                                labelText: column.LabelText,
                                                value: deptModel.Attachments(columnName: column.Name).ToJson(),
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
            var deptModel = new DeptModel(
                context: context,
                ss: ss,
                deptId: 0,
                formData: context.Forms);
            var invalid = DeptValidators.OnCreating(
                context: context,
                ss: ss,
                deptModel: deptModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = deptModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: deptModel.Title.Value));
                    return new ResponseCollection()
                        .Response("id", deptModel.DeptId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : deptModel.DeptId) + "?new=1")
                        .ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
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
                default: return invalid.Type.MessageJson(context: context);
            }
            if (deptModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var errorData = deptModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new DeptsResponseCollection(deptModel);
                    return ResponseByUpdate(res, context, ss, deptModel)
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
                    return errorData.Type.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            DeptsResponseCollection res,
            Context context,
            SiteSettings ss,
            DeptModel deptModel)
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
                            dataRows: gridData.DataRows,
                            columns: columns,
                            gridSelector: null))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: deptModel.Title.DisplayValue));
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
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", deptModel.Title.Value)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: deptModel,
                        tableName: "Depts"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: deptModel.Title.Value))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: deptModel.Comments,
                        deleteCommentId: deptModel.DeleteCommentId)
                    .ClearFormData();
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
                default: return invalid.Type.MessageJson(context: context);
            }
            var errorData = deptModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: deptModel.Title.Value));
                    var res = new DeptsResponseCollection(deptModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.Type.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int deptId, Message message = null)
        {
            var deptModel = new DeptModel(context: context, ss: ss, deptId: deptId);
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
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
                                deptModel: deptModel)));
            return new DeptsResponseCollection(deptModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
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
                                .DataLatest(1, _using:
                                    deptModelHistory.Ver == deptModel.Ver),
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
                                columns
                                    .ForEach(column => hb
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
            ss.SetColumnAccessControls(
                context: context,
                mine: deptModel.Mine(context: context));
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
                        "Items",
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
            var siteDepts = siteModel != null
                ? SiteInfo.SiteDepts(context, siteModel.InheritPermission)?
                    .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var deptCollection = new DeptCollection(
                context: context,
                ss: ss,
                where: view.Where(context: context, ss: ss)
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
                        Rds.Depts_DeptId_WhereLike(),
                        Rds.Depts_DeptName_WhereLike(),
                        Rds.Depts_Body_WhereLike()
                    })
                .Add(
                    tableName: "Users",
                    subLeft: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere()
                        .UserId(userId)
                        .DeptId(raw: "[Depts].[DeptId]")
                        .Add(raw: "[Depts].[DeptId]>0")),
                    _operator: ">0",
                    _using: userId.HasValue),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss),
                offset: api.Offset,
                pageSize: pageSize);
            var groups = siteDepts == null
                ? deptCollection
                : deptCollection.Join(siteDepts, c => c.DeptId, s => s, (c, s) => c);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Offset = api.Offset,
                    PageSize = pageSize,
                    TotalCount = groups.Count(),
                    Data = groups.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }
    }
}
