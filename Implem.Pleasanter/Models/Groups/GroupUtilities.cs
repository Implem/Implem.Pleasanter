﻿using Implem.DefinitionAccessor;
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
    public static class GroupUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Index(Context context, SiteSettings ss)
        {
            var invalid = GroupValidators.OnEntry(
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
            var view = Views.GetBySession(
                context: context,
                ss: ss);
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
            var invalid = GroupValidators.OnEntry(
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
                referenceType: "Groups",
                title: Displays.Groups(context: context) + " - " + Displays.List(context: context),
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
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setGrid",
                    editOnGrid: context.Forms.Bool("EditOnGrid"),
                    serverScriptModelRow: serverScriptModelRow,
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            serverScriptModelRow: serverScriptModelRow))
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
                    value: gridData.DataRows.Select(g => g.Long("GroupId")).ToJson())
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
                .Val("#GridRowIds", gridData.DataRows.Select(g => g.Long("GroupId")).ToJson())
                .Val("#GridColumns", columns.Select(o => o.ColumnName).ToJson())
                .Paging("#Grid")
                .Message(message)
                .Messages(context.Messages)
                .Log(context.GetLog())
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
                ? Rds.GroupsWhere().GroupId_In(
                    value: selector.Selected.Select(o => o.ToInt()),
                    negative: selector.All)
                : null;
        }

        private static SqlWhereCollection SelectedWhereByApi(
            SiteSettings ss,
            RecordSelector recordSelector)
        {
            return !recordSelector.Nothing
                ? Rds.GroupsWhere().GroupId_In(
                    value: recordSelector.Selected?.Select(o => o.ToInt()) ?? new List<int>(),
                    negative: recordSelector.All)
                : null;
        }

        public static string ReloadRow(Context context, SiteSettings ss, long groupId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var dataRow = new GridData(
                context: context,
                ss: ss,
                view: view,
                tableType: Sqls.TableTypes.Normal,
                where: Rds.GroupsWhere().GroupId(groupId))
                    .DataRows
                    .FirstOrDefault();
            var res = ItemUtilities.ClearItemDataResponse(
                context: context,
                ss: ss,
                id: groupId);
            return dataRow == null
                ? res
                    .Remove($"[data-id=\"{groupId}\"][data-latest]")
                    .Message(
                        message: Messages.NotFound(context: context),
                        target: "row_" + groupId)
                    .Messages(context.Messages)
                    .Log(context.GetLog())
                    .ToJson()
                : res
                    .ReplaceAll(
                        $"[data-id=\"{dataRow.Long("GroupId")}\"][data-latest]",
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
                            idColumn: "GroupId"))
                    .Messages(context.Messages)
                    .Log(context.GetLog())
                    .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            GroupModel groupModel,
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
                    groupModel: groupModel);
            }
            else
            {
                var mine = groupModel.Mine(context: context);
                switch (column.Name)
                {
                    case "GroupId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.GroupId,
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
                                    value: groupModel.Ver,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty,
                                    tabIndex: tabIndex,
                                    serverScriptModelColumn: serverScriptModelColumn);
                    case "GroupName":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: groupModel.GroupName,
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
                                    value: groupModel.Body,
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
                                    value: groupModel.Disabled,
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
                                    value: groupModel.Comments,
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
                                    value: groupModel.Creator,
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
                                    value: groupModel.Updator,
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
                                    value: groupModel.CreatedTime,
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
                                    value: groupModel.UpdatedTime,
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
                                            value: groupModel.GetClass(columnName: column.Name),
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
                                            value: groupModel.GetNum(columnName: column.Name),
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
                                            value: groupModel.GetDate(columnName: column.Name),
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
                                            value: groupModel.GetDescription(columnName: column.Name),
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
                                            value: groupModel.GetCheck(columnName: column.Name),
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
                                            value: groupModel.GetAttachments(columnName: column.Name),
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
            GroupModel groupModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "GroupId": value = groupModel.GroupId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = groupModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "GroupName": value = groupModel.GroupName.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = groupModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "Disabled": value = groupModel.Disabled.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = groupModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = groupModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = groupModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = groupModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = groupModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                value = groupModel.GetClass(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Num":
                                value = groupModel.GetNum(columnName: column.Name)?.Value?.GridText(
                                    context: context,
                                    column: column) ?? string.Empty;
                                break;
                            case "Date":
                                value = groupModel.GetDate(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Description":
                                value = groupModel.GetDescription(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Check":
                                value = groupModel.GetCheck(columnName: column.Name).GridText(
                                    context: context,
                                    column: column);
                                break;
                            case "Attachments":
                                value = groupModel.GetAttachments(columnName: column.Name).GridText(
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
            return Editor(context: context, ss: ss, groupModel: new GroupModel(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(
            Context context, SiteSettings ss, int groupId, bool clearSessions)
        {
            var groupModel = new GroupModel(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: groupId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            groupModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: groupModel.GroupId);
            return Editor(context: context, ss: ss, groupModel: groupModel);
        }

        public static string Editor(
            Context context, SiteSettings ss, GroupModel groupModel)
        {
            var invalid = GroupValidators.OnEditing(
                context: context,
                ss: ss,
                groupModel: groupModel);
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
                referenceType: "Groups",
                title: groupModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.Groups(context: context) + " - " + Displays.New(context: context)
                    : groupModel.Title.MessageDisplay(context: context),
                action: () => hb
                    .Editor(
                        context: context,
                        ss: ss,
                        groupModel: groupModel)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, Context context, SiteSettings ss, GroupModel groupModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = Permissions.ColumnPermissionType(
                context: context,
                ss: ss,
                column: commentsColumn,
                baseModel: groupModel);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("GroupForm")
                        .Class("main-form confirm-unload")
                        .Action(groupModel.GroupId != 0
                            ? Locations.Action(
                                context: context,
                                controller: "Groups",
                                id: groupModel.GroupId)
                            : Locations.Action(
                                context: context,
                                controller: "Groups")),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: groupModel,
                            tableName: "Groups")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: groupModel.Comments,
                                    column: commentsColumn,
                                    verType: groupModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container " + tabsCss,
                            action: () => hb
                                .EditorTabs(
                                    context: context,
                                    groupModel: groupModel)
                                .FieldSetGeneral(
                                    context: context,
                                    ss: ss,
                                    groupModel: groupModel)
                                .FieldSetMembers(
                                    context: context,
                                    groupModel: groupModel)
                                .FieldSet(
                                    attributes: new HtmlAttributes()
                                        .Id("FieldSetHistories")
                                        .DataAction("Histories")
                                        .DataMethod("post"),
                                    _using: groupModel.MethodType != BaseModel.MethodTypes.New)
                                .MainCommands(
                                    context: context,
                                    ss: ss,
                                    verType: groupModel.VerType,
                                    updateButton: true,
                                    mailButton: true,
                                    deleteButton: true,
                                    extensions: () => hb
                                        .MainCommandExtensions(
                                            context: context,
                                            groupModel: groupModel,
                                            ss: ss)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "Ver",
                            value: groupModel.Ver.ToString())
                        .Hidden(
                            controlId: "MethodType",
                            value: groupModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Groups_Timestamp",
                            css: "always-send",
                            value: groupModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: groupModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Groups",
                    referenceId: groupModel.GroupId,
                    referenceVer: groupModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    ss: ss)
                .OutgoingMailDialog()
                .EditorExtensions(
                    context: context,
                    groupModel: groupModel,
                    ss: ss));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetMembers",
                        text: Displays.Members(context: context),
                        _using: groupModel.MethodType != BaseModel.MethodTypes.New))
                .Li(
                    _using: groupModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context, ss: ss, groupModel: groupModel));
        }

        private static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            bool preview = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
                hb.Field(
                    context: context,
                    ss: ss,
                    groupModel: groupModel,
                    column: column,
                    preview: preview));
            if (!preview)
            {
                hb.VerUpCheckBox(
                    context: context,
                    ss: ss,
                    baseModel: groupModel);
            }
            return hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            Column column,
            bool controlOnly = false,
            bool alwaysSend = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false)
        {
            var value = groupModel.ControlValue(
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
                        baseModel: groupModel),
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    idSuffix: idSuffix,
                    preview: preview,
                    disableSection: disableSection);
            }
            return hb;
        }

        public static string ControlValue(
            this GroupModel groupModel,
            Context context,
            SiteSettings ss,
            Column column)
        {
            switch (column.Name)
            {
                case "GroupId":
                    return groupModel.GroupId
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Ver":
                    return groupModel.Ver
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "GroupName":
                    return groupModel.GroupName
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Body":
                    return groupModel.Body
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                case "Disabled":
                    return groupModel.Disabled
                        .ToControl(
                            context: context,
                            ss: ss,
                            column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return groupModel.GetClass(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Num":
                            return groupModel.GetNum(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Date":
                            return groupModel.GetDate(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Description":
                            return groupModel.GetDescription(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Check":
                            return groupModel.GetCheck(columnName: column.Name)
                                .ToControl(
                                    context: context,
                                    ss: ss,
                                    column: column);
                        case "Attachments":
                            return groupModel.GetAttachments(columnName: column.Name)
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
            GroupModel groupModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GroupModel groupModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, int groupId)
        {
            return EditorResponse(context, ss, new GroupModel(
                context, ss, groupId,
                formData: context.Forms)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            Message message = null,
            string switchTargets = null)
        {
            groupModel.MethodType = groupModel.GroupId == 0
                ? BaseModel.MethodTypes.New
                : BaseModel.MethodTypes.Edit;
            return new GroupsResponseCollection(groupModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(context, ss, groupModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .Messages(context.Messages)
                .ClearFormData(_using: !context.QueryStrings.Bool("control-auto-postback"))
                .Log(context.GetLog());
        }

        private static List<int> GetSwitchTargets(Context context, SiteSettings ss, int groupId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss, where: Rds.GroupsWhere().TenantId(context.TenantId));
            var param = view.Param(
                context: context,
                ss: ss);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss)
                    .Groups_UpdatedTime(SqlOrderBy.Types.desc);
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
                    statements: Rds.SelectGroups(
                        column: Rds.GroupsColumn().GroupsCount(),
                        join: join,
                        where: where,
                        param: param)) <= Parameters.General.SwitchTargetsLimit)
                {
                    switchTargets = Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectGroups(
                            column: Rds.GroupsColumn().GroupId(),
                            join: join,
                            where: where,
                            param: param,
                            orderBy: orderBy))
                                .AsEnumerable()
                                .Select(o => o["GroupId"].ToInt())
                                .ToList();
                }
            }
            if (!switchTargets.Contains(groupId))
            {
                switchTargets.Add(groupId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            string idSuffix = null)
        {
            var replaceFieldColumns = ss.ReplaceFieldColumns(
                context: context,
                serverScriptModelRow: groupModel.ServerScriptModelRow);
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
                    var serverScriptModelColumn = groupModel
                        ?.ServerScriptModelRow
                        ?.Columns.Get(column.ColumnName);
                    if (replaceFieldColumns?.Contains(column.ColumnName) == true)
                    {
                        res.ReplaceAll(
                            target: $"#Groups_{column.Name}Field" + idSuffix,
                            value: new HtmlBuilder().Field(
                                context: context,
                                ss: ss,
                                groupModel: groupModel,
                                column: column,
                                idSuffix: idSuffix));
                    }
                    else
                    {
                        switch (column.Name)
                        {
                            case "TenantId":
                                res.Val(
                                    target: "#Groups_TenantId" + idSuffix,
                                    value: groupModel.TenantId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "GroupId":
                                res.Val(
                                    target: "#Groups_GroupId" + idSuffix,
                                    value: groupModel.GroupId.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "GroupName":
                                res.Val(
                                    target: "#Groups_GroupName" + idSuffix,
                                    value: groupModel.GroupName.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Body":
                                res.Val(
                                    target: "#Groups_Body" + idSuffix,
                                    value: groupModel.Body.ToResponse(context: context, ss: ss, column: column),
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            case "Disabled":
                                res.Val(
                                    target: "#Groups_Disabled" + idSuffix,
                                    value: groupModel.Disabled,
                                    options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                break;
                            default:
                                switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                                {
                                    case "Class":
                                        res.Val(
                                            target: $"#Groups_{column.Name}{idSuffix}",
                                            value: groupModel.GetClass(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Num":
                                        res.Val(
                                            target: $"#Groups_{column.Name}{idSuffix}",
                                            value: groupModel.GetNum(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Date":
                                        res.Val(
                                            target: $"#Groups_{column.Name}{idSuffix}",
                                            value: groupModel.GetDate(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Description":
                                        res.Val(
                                            target: $"#Groups_{column.Name}{idSuffix}",
                                            value: groupModel.GetDescription(columnName: column.Name).ToResponse(
                                                context: context,
                                                ss: ss,
                                                column: column),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Check":
                                        res.Val(
                                            target: $"#Groups_{column.Name}{idSuffix}",
                                            value: groupModel.GetCheck(columnName: column.Name),
                                            options: column.ResponseValOptions(serverScriptModelColumn: serverScriptModelColumn));
                                        break;
                                    case "Attachments":
                                        res.ReplaceAll(
                                            target: $"#Groups_{column.Name}Field",
                                            value: new HtmlBuilder()
                                                .FieldAttachments(
                                                    context: context,
                                                    fieldId: $"Groups_{column.Name}Field",
                                                    controlId: $"Groups_{column.Name}",
                                                    columnName: column.ColumnName,
                                                    fieldCss: column.FieldCss
                                                        + (column.TextAlign == SiteSettings.TextAlignTypes.Right
                                                            ? " right-align"
                                                            : string.Empty),
                                                    fieldDescription: column.Description,
                                                    labelText: column.LabelText,
                                                    value: groupModel.GetAttachments(columnName: column.Name).ToJson(),
                                                    readOnly: Permissions.ColumnPermissionType(
                                                        context: context,
                                                        ss: ss,
                                                        column: column,
                                                        baseModel: groupModel)
                                                            != Permissions.ColumnPermissionTypes.Update,
                                                    allowDelete: column.AllowDeleteAttachments != false),
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
            var groupModel = new GroupModel(
                context: context,
                ss: ss,
                groupId: 0,
                formData: context.Forms);
            var invalid = GroupValidators.OnCreating(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            List<Process> processes = null;
            var errorData = groupModel.Create(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: CreatedMessage(
                            context: context,
                            ss: ss,
                            groupModel: groupModel,
                            process: processes?.FirstOrDefault()));
                    return new ResponseCollection()
                        .Response("id", groupModel.GroupId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : groupModel.GroupId)
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
            GroupModel groupModel,
            Process process)
        {
            if (process == null)
            {
                return Messages.Created(
                    context: context,
                    data: groupModel.Title.Value);
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = groupModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Update(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(
                context: context,
                ss: ss,
                groupId: groupId,
                formData: context.Forms);
            var invalid = GroupValidators.OnUpdating(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            List<Process> processes = null;
            var errorData = groupModel.Update(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    var res = new GroupsResponseCollection(groupModel);
                    return ResponseByUpdate(res, context, ss, groupModel, processes)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: groupModel.Comments,
                            verType: groupModel.VerType)
                        .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: groupModel.Updator.Name)
                            .ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            GroupsResponseCollection res,
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            List<Process> processes)
        {
            ss.ClearColumnAccessControlCaches(baseModel: groupModel);
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
                    where: Rds.GroupsWhere().GroupId(groupModel.GroupId));
                var columns = ss.GetGridColumns(
                    context: context,
                    view: view,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{groupModel.GroupId}\"][data-latest]",
                        new HtmlBuilder().GridRows(
                            context: context,
                            ss: ss,
                            dataRows: gridData.DataRows,
                            columns: columns))
                    .CloseDialog()
                    .Message(message: UpdatedMessage(
                        context: context,
                        ss: ss,
                        groupModel: groupModel,
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
                    .FieldResponse(context: context, ss: ss, groupModel: groupModel)
                    .Val("#VerUp", verUp)
                    .Val("#Ver", groupModel.Ver)
                    .Disabled("#VerUp", verUp)
                    .Html("#HeaderTitle", HttpUtility.HtmlEncode(groupModel.Title.Value))
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: groupModel,
                        tableName: "Groups"))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: groupModel.Title.Value))
                    .Messages(context.Messages)
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: groupModel.Comments,
                        deleteCommentId: groupModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            List<Process> processes)
        {
            var process = processes.FirstOrDefault(o => !o.SuccessMessage.IsNullOrEmpty());
            if (process == null)
            {
                return Messages.Updated(
                    context: context,
                    data: groupModel.Title.MessageDisplay(context: context));
            }
            else
            {
                var message = process.GetSuccessMessage(context: context);
                message.Text = groupModel.ReplacedDisplayValues(
                    context: context,
                    ss: ss,
                    value: message.Text);
                return message;
            }
        }

        public static string Delete(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(context, ss, groupId);
            var invalid = GroupValidators.OnDeleting(
                context: context,
                ss: ss,
                groupModel: groupModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var errorData = groupModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: groupModel.Title.MessageDisplay(context: context)));
                    var res = new GroupsResponseCollection(groupModel);
                    res
                        .SetMemory("formChanged", false)
                        .Invoke("back");
                    return res.ToJson();
                default:
                    return errorData.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, int groupId, Message message = null)
        {
            var groupModel = new GroupModel(context: context, ss: ss, groupId: groupId);
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
                                groupModel: groupModel)));
            return new GroupsResponseCollection(groupModel)
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
            GroupModel groupModel)
        {
            new GroupCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.GroupsWhere().GroupId(groupModel.GroupId),
                orderBy: Rds.GroupsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(groupModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(groupModelHistory.Ver)
                                .DataLatest(
                                    value: 1,
                                    _using: groupModelHistory.Ver == groupModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: groupModelHistory.Ver.ToString(),
                                            _using: groupModelHistory.Ver < groupModel.Ver));
                                columns.ForEach(column => hb
                                    .TdValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        groupModel: groupModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.GroupsColumnCollection()
                .GroupId()
                .Ver();
            columns.ForEach(column =>
                sqlColumn.GroupsColumn(columnName: column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, int groupId)
        {
            var groupModel = new GroupModel(context: context, ss: ss, groupId: groupId);
            groupModel.Get(
                context: context,
                ss: ss,
                where: Rds.GroupsWhere()
                    .GroupId(groupModel.GroupId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            groupModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, groupModel)
                .PushState("History", Locations.Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        groupId.ToString() 
                            + (groupModel.VerType == Versions.VerTypes.History
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
            var ss = SiteSettingsUtilities.GroupsSiteSettings(context: context);
            if (context.ContractSettings.Import == false)
            {
                return Messages.ResponseRestricted(context: context).ToJson();
            }
            var invalid = GroupValidators.OnImporting(
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
                var columnHash = ImportUtilities.GetColumnHash(ss, csv);
                var idColumn = columnHash
                    .Where(o =>
                        o.Value.Column.ColumnName == "GroupId")
                    .Select(o =>
                        new { Id = o.Key })
                    .FirstOrDefault()?.Id ?? -1;
                var invalidColumn = Imports.ColumnValidate(
                    context,
                    ss,
                    columnHash.Values.Select(o => o.Column.ColumnName),
                    columnNames: "GroupName");
                if (invalidColumn != null) return invalidColumn;
                var groups = new List<GroupModel>();
                foreach (var data in csv.Rows.Select((o, i) =>
                    new { Row = o, Index = i }))
                {
                    var groupModel = new GroupModel(
                        context: context,
                        ss: ss);
                    if (idColumn > -1)
                    {
                        var model = new GroupModel(
                            context: context,
                            ss: ss,
                            groupId: data.Row[idColumn].ToInt());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            groupModel = model;
                        }
                    }
                    foreach (var column in columnHash)
                    {
                        var recordingData = ImportRecordingData(
                            context: context,
                            column: column.Value.Column,
                            value: data.Row[column.Key],
                            inheritPermission: ss.InheritPermission);
                        switch (column.Value.Column.ColumnName)
                        {
                            case "GroupId":
                                groupModel.GroupId = recordingData.ToInt();
                                break;
                            case "GroupName":
                                groupModel.GroupName = recordingData;
                                break;
                            case "Body":
                                groupModel.Body = recordingData;
                                break;
                            case "Comments":
                                if (groupModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    groupModel.Comments.Prepend(
                                        context: context,
                                        ss: ss,
                                        body: data.Row[column.Key]);
                                }
                                break;
                            case "Disabled":
                                groupModel.Disabled = recordingData.ToBool();
                                break;
                            case "MemberType":
                                groupModel.MemberType = recordingData.ToString();
                                break;
                            case "MemberKey":
                                groupModel.MemberKey = recordingData.ToString();
                                break;
                            case "MemberName":
                                groupModel.MemberName = recordingData.ToString();
                                break;
                            case "MemberIsAdmin":
                                groupModel.MemberIsAdmin = recordingData.ToBool();
                                break;
                            default:
                                groupModel.SetValue(
                                    context: context,
                                    column: column.Value.Column,
                                    value: recordingData);
                                break;
                        }
                    }
                    var csvRowForError = data.Index + 2;
                    if (!ValidateMemberType(memberType: groupModel.MemberType))
                    {
                        return InvalidMemberTypeError(
                            context: context,
                            errorCsvRow: csvRowForError);
                    }
                    if (!ValidateMemberKey(
                        context: context,
                        memberType: groupModel.MemberType,
                        memberKey: groupModel.MemberKey))
                    {
                        return InvalidMemberKeyError(
                            context: context,
                            errorCsvRow: csvRowForError);
                    }
                    groups.Add(groupModel);
                };
                if (context.Forms.Bool("ReplaceAllGroupMembers") == true)
                {
                    groups
                        .Select(o => o.GroupId)
                        .Distinct()
                        .ForEach(groupId =>
                            PhysicalDeleteGroupMembers(
                                context: context,
                                groupId: groupId));
                }
                var insertGroupCount = 0;
                var updateGroupCount = 0;
                var insertGroupMemberCount = 0;
                var updateGroupMemberCount = 0;
                var newGroups = new Dictionary<string, GroupModel>();
                foreach (var groupModel in groups)
                {
                    if (groupModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        var errorData = UpdateGroup(
                            context: context,
                            ss: ss,
                            groupModel: groupModel,
                            updateGroupCount: ref updateGroupCount);
                        switch (errorData.Type)
                        {
                            case Error.Types.None:
                                break;
                            default:
                                return errorData.MessageJson(context: context);
                        }
                    }
                    else
                    {
                        if (!newGroups.ContainsKey(groupModel.GroupName))
                        {
                            var errorData = groupModel.Create(
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
                            insertGroupCount++;
                            newGroups.Add(groupModel.GroupName, groupModel);
                        }
                        else
                        {
                            groupModel.GroupId = newGroups[groupModel.GroupName].GroupId;
                            var errorData = UpdateGroup(
                                context: context,
                                ss: ss,
                                groupModel: groupModel,
                                updateGroupCount: ref updateGroupCount);
                            switch (errorData.Type)
                            {
                                case Error.Types.None:
                                    break;
                                default:
                                    return errorData.MessageJson(context: context);
                            }
                        }
                    }
                    if (!groupModel.MemberType.IsNullOrEmpty())
                    {
                        UpdateOrInsertGroupMember(
                            context: context,
                            groupModel: groupModel,
                            insertGroupMemberCount: ref insertGroupMemberCount,
                            updateGroupMemberCount: ref updateGroupMemberCount);
                    }
                }
                SiteInfo.Reflesh(
                    context: context,
                    force: true);
                return GridRows(
                    context: context,
                    ss: ss,
                    res: res.WindowScrollTop(),
                    message: Messages.GroupImported(
                        context: context,
                        data: new string[]
                        {
                            ss.Title,
                            insertGroupCount.ToString(),
                            updateGroupCount.ToString(),
                            insertGroupMemberCount.ToString(),
                            updateGroupMemberCount.ToString()
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
        private static ErrorData UpdateGroup(
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            ref int updateGroupCount)
        {
            if (!groupModel.Updated(context: context))
            {
                return new ErrorData(type: Error.Types.None);
            }
            groupModel.Timestamp = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectGroups(
                    column: Rds.GroupsColumn()
                        .UpdatedTime(),
                    where: Rds.GroupsWhere()
                        .TenantId(context.TenantId)
                        .GroupId(groupModel.GroupId)))
                            .AsEnumerable()
                            .FirstOrDefault()
                            ?.Field<DateTime>("UpdatedTime")
                            .ToString("yyyy/M/d H:m:s.fff");
            var errorData = groupModel.Update(
                context: context,
                ss: ss,
                refleshSiteInfo: false,
                updateGroupMembers: false,
                get: false);
            if (errorData.Type == Error.Types.None)
            {
                updateGroupCount++;
            }
            return errorData;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string InvalidMemberTypeError(Context context, int errorCsvRow)
        {
            return Error.Types.InvalidMemberType.MessageJson(
                context: context,
                data: errorCsvRow.ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string InvalidMemberKeyError(Context context, int errorCsvRow)
        {
            return Error.Types.InvalidMemberKey.MessageJson(
                context: context,
                data: errorCsvRow.ToString());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidateMemberType(string memberType)
        {
            return memberType.IsNullOrEmpty()
                || memberType == "Dept"
                || memberType == "User";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool ValidateMemberKey(Context context, string memberType, string memberKey)
        {
            switch (memberType)
            {
                case null:
                case "":
                    return true;
                case "Dept":
                    return GetDeptId(
                        context: context,
                        deptCode: memberKey) > 0;
                case "User":
                    return GetUserId(
                        context: context,
                        loginId: memberKey) > 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int GetUserId(Context context, string loginId)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .UserId(),
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .LoginId(loginId))
                );
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int GetDeptId(Context context, string deptCode)
        {
            return Repository.ExecuteScalar_int (
                context: context,
                statements: Rds.SelectDepts(
                    column: Rds.DeptsColumn()
                        .DeptId(),
                    where: Rds.DeptsWhere()
                        .TenantId(context.TenantId)
                        .DeptCode(deptCode)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void UpdateOrInsertGroupMember(
            Context context,
            GroupModel groupModel,
            ref int insertGroupMemberCount,
            ref int updateGroupMemberCount)
        {
            var deptId = GetDeptId(
                context: context,
                deptCode: groupModel.MemberKey);
            var userId = GetUserId(
                context: context,
                loginId: groupModel.MemberKey);
            if (Repository.ExecuteScalar_bool(
                    context: context,
                    statements: Rds.SelectGroupMembers(
                        column: Rds.GroupMembersColumn().GroupMembersCount(),
                        where: Rds.GroupMembersWhere()
                            .GroupId(groupModel.GroupId)
                            .DeptId(deptId)
                            .UserId(userId))))
            {
                if (groupModel.MemberIsAdmin != null)
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Rds.UpdateGroupMembers(
                            where: Rds.GroupMembersWhere()
                                .GroupId(groupModel.GroupId)
                                .DeptId(deptId)
                                .UserId(userId),
                            param: Rds.GroupMembersParam()
                                .Admin(groupModel.MemberIsAdmin)));
                    updateGroupMemberCount++;
                }
            }
            else
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: Rds.InsertGroupMembers(
                        param: Rds.GroupMembersParam()
                        .GroupId(groupModel.GroupId)
                        .DeptId(deptId)
                        .UserId(userId)
                        .Admin(groupModel.MemberIsAdmin ?? false)));
                insertGroupMemberCount++;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void PhysicalDeleteGroupMembers(Context context, int groupId)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(groupId)));
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
            var invalid = GroupValidators.OnExporting(
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
            var invalid = GroupValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var export = ss.GetExport(context: context);
            var view = Views.GetBySession(
                context: context,
                ss: ss);
            var csv = new System.Text.StringBuilder();
            if (export.Header == true)
            {
                csv.Append(export.Columns.Select(column =>
                    "\"" + column.GetLabelText() + "\"").Join(","),
                    $",\"{Displays.Groups_MemberType(context: context)}\"",
                    $",\"{Displays.Groups_MemberKey(context: context)}\"",
                    $",\"{Displays.Groups_MemberName(context: context)}\"",
                    $",\"{Displays.Groups_MemberIsAdmin(context: context)}\"",
                    "\n");
            }
            new GroupCollection(
                context: context,
                ss: ss,
                where: view.Where(
                    context: context,
                    ss: ss,
                    where: Rds.GroupsWhere().TenantId(context.TenantId)),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss))
                        .ForEach(groupModel =>
                        {
                            var members = GroupMembers(
                                context: context,
                                groupId: groupModel.GroupId);
                            if (members.Count() == 0)
                            {
                                AppendGroupColumnsToCsv(
                                    context: context,
                                    ss: ss,
                                    groupModel: groupModel,
                                    export: export,
                                    csv: csv);
                                AppendBlankGroupMemberColumnsToCsv(csv: csv);
                            }
                            else
                            {
                                members.ForEach(dataRow =>
                                {
                                    AppendGroupColumnsToCsv(
                                        context: context,
                                        ss: ss,
                                        groupModel: groupModel,
                                        export: export,
                                        csv: csv);
                                    AppendGroupMemberColumnsToCsv(
                                        context: context,
                                        dataRow: dataRow,
                                        csv: csv);
                                });
                            }
                        });
            return new ResponseFile(
                fileContent: csv.ToString(),
                fileDownloadName: ExportUtilities.FileName(
                    context: context,
                    title: ss.Title,
                    name: Displays.Groups(context: context)),
                encoding: context.QueryStrings.Data("encoding"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AppendGroupMemberColumnsToCsv(Context context, DataRow dataRow, System.Text.StringBuilder csv)
        {
            var dept = SiteInfo.Dept(
                tenantId: context.TenantId,
                deptId: dataRow.Int("DeptId"));
            var user = SiteInfo.User(
                context: context,
                userId: dataRow.Int("UserId"));
            var admin = dataRow.Bool("Admin");
            if (dept.Id > 0)
            {
                csv.Append(
                    $",\"Dept\"",
                    $",\"{dept.Code}\"",
                    $",\"{dept.Name}\"",
                    $",\"{admin}\"",
                    "\n");
            }
            else if (!user.Anonymous())
            {
                csv.Append(
                    $",\"User\"",
                    $",\"{user.LoginId}\"",
                    $",\"{user.Name}\"",
                    $",\"{admin}\"",
                    "\n");
            }
            else
            {
                AppendBlankGroupMemberColumnsToCsv(csv: csv);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AppendGroupColumnsToCsv(
            Context context,
            SiteSettings ss,
            GroupModel groupModel,
            Export export,
            System.Text.StringBuilder csv)
        {
            csv.Append(export.Columns.Select(exportColumn =>
                groupModel.CsvData(
                    context: context,
                    ss: ss,
                    column: ss.GetColumn(
                        context: context,
                        columnName: exportColumn.ColumnName),
                    exportColumn: exportColumn,
                    mine: groupModel.Mine(context: context),
                    encloseDoubleQuotes: true)).Join());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AppendBlankGroupMemberColumnsToCsv(System.Text.StringBuilder csv)
        {
            csv.Append(",\"\",\"\",\"\",\"\"", "\n");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Rds.GroupsWhereCollection AdditionalWhere(Context context, View view)
        {
            var deptsSearchText = view.ColumnFilterHash.Get("Depts");
            var usersSearchText = view.ColumnFilterHash.Get("Users");
            return Rds.GroupsWhere()
                .TenantId(context.TenantId)
                .GroupId_In(sub: Rds.SelectGroupMembers(
                    distinct: true,
                    column: Rds.GroupMembersColumn().GroupId(),
                    where: Rds.GroupMembersWhere()
                        .Add(raw: Permissions.DeptOrUser("GroupMembers"))),
                    _using: !Permissions.CanManageTenant(context: context))
                .GroupId_In(sub: Rds.SelectGroupMembers(
                    distinct: true,
                    column: Rds.GroupMembersColumn().GroupId(),
                    join: Rds.GroupMembersJoin()
                        .Add(new SqlJoin(
                            tableBracket: "\"Depts\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"GroupMembers\".\"DeptId\"=\"Depts\".\"DeptId\"")),
                    where: new SqlWhereCollection()
                        .Or(or: new SqlWhereCollection()
                            .SqlWhereLike(
                                tableName: "Depts",
                                name: "DeptsSearchText",
                                searchText: deptsSearchText,
                                clauseCollection: new List<string>()
                                {
                        Rds.Depts_DeptCode_WhereLike(
                            factory: context,
                            name: "DeptsSearchText"),
                        Rds.Depts_DeptName_WhereLike(
                            factory: context,
                            name: "DeptsSearchText"),
                        Rds.Depts_Body_WhereLike(
                            factory: context,
                            name: "DeptsSearchText")
                                }))),
                    _using: !deptsSearchText.IsNullOrEmpty())
                .GroupId_In(sub: Rds.SelectGroupMembers(
                    distinct: true,
                    column: Rds.GroupMembersColumn().GroupId(),
                    join: Rds.GroupMembersJoin()
                        .Add(new SqlJoin(
                            tableBracket: "\"Depts\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "\"GroupMembers\".\"DeptId\"=\"Depts\".\"DeptId\""))
                        .Add(new SqlJoin(
                            tableBracket: "\"Users\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "\"Depts\".\"DeptId\"=\"DeptUsers\".\"DeptId\"",
                            _as: "DeptUsers"))
                        .Add(new SqlJoin(
                            tableBracket: "\"Users\"",
                            joinType: SqlJoin.JoinTypes.LeftOuter,
                            joinExpression: "\"GroupMembers\".\"UserId\"=\"Users\".\"UserId\"")),
                    where: new SqlWhereCollection()
                        .Or(or: new SqlWhereCollection()
                            .SqlWhereLike(
                                tableName: "DeptUsers",
                                name: "UsersSearchText",
                                searchText: usersSearchText,
                                clauseCollection: new List<string>()
                                {
                        Rds.Users_LoginId_WhereLike(
                            tableName: "DeptUsers",
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_Name_WhereLike(
                            tableName: "DeptUsers",
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_UserCode_WhereLike(
                            tableName: "DeptUsers",
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_Body_WhereLike(
                            tableName: "DeptUsers",
                            factory: context,
                            name: "UsersSearchText"),
                                })
                            .SqlWhereLike(
                                tableName: "Users",
                                name: "UsersSearchText",
                                searchText: usersSearchText,
                                clauseCollection: new List<string>()
                                {
                        Rds.Users_LoginId_WhereLike(
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_Name_WhereLike(
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_UserCode_WhereLike(
                            factory: context,
                            name: "UsersSearchText"),
                        Rds.Users_Body_WhereLike(
                            factory: context,
                            name: "UsersSearchText")
                                }))),
                    _using: !usersSearchText.IsNullOrEmpty());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GridRows(Context context)
        {
            return GridRows(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                offset: context.Forms.Int("GridOffset"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetMembers(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            if (groupModel.MethodType == BaseModel.MethodTypes.New) return hb;
            return hb.FieldSet(id: "FieldSetMembers", action: () => hb
                .CurrentMembers(
                    context: context,
                    groupModel: groupModel)
                .SelectableMembers(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder CurrentMembers(
            this HtmlBuilder hb, Context context, GroupModel groupModel)
        {
            return hb.FieldSelectable(
                controlId: "CurrentMembers",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                controlCss: " always-send send-all",
                labelText: Displays.CurrentMembers(context: context),
                listItemCollection: CurrentMembers(
                    context: context,
                    groupModel: groupModel),
                selectedValueCollection: null,
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "GeneralUser",
                            controlCss: "button-icon post",
                            text: Displays.GeneralUser(context: context),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlId: "Manager",
                            controlCss: "button-icon post",
                            text: Displays.Manager(context: context),
                            onClick: "$p.setGroup($(this));",
                            icon: "ui-icon-person")
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Delete(context: context),
                            onClick: "$p.deleteSelected($(this));",
                            icon: "ui-icon-circle-triangle-e",
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> CurrentMembers(
            Context context, GroupModel groupModel)
        {
            var data = new Dictionary<string, ControlData>();
            GroupMembers(
                context: context,
                groupId: groupModel.GroupId)
                    .ForEach(dataRow =>
                        data.AddMember(
                            context: context,
                            dataRow: dataRow));
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static EnumerableRowCollection<DataRow> GroupMembers(Context context, int groupId)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn()
                        .DeptId()
                        .UserId()
                        .Admin(),
                    where: Rds.GroupMembersWhere()
                        .GroupId(groupId)
                        .Add(or: Rds.GroupMembersWhere()
                            .Sub(sub: Rds.ExistsDepts(where: Rds.DeptsWhere()
                                .DeptId(raw: "\"GroupMembers\".\"DeptId\"")))
                            .Sub(sub: Rds.ExistsUsers(where: Rds.UsersWhere()
                                .UserId(raw: "\"GroupMembers\".\"UserId\""))))))
                                    .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Contains(Context context, int groupId, Dept dept)
        {
            return Contains(
                context: context,
                groupId: groupId,
                deptId: dept?.Id ?? 0,
                userId: 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Contains(Context context, int groupId, User user)
        {
            return Contains(
                context: context,
                groupId: groupId,
                deptId: user?.DeptId ?? 0,
                userId: user?.Id ?? 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool Contains(Context context, int groupId, int deptId, int userId)
        {
            return deptId > 0 || userId > 0
                ? Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectGroupMembers(
                        column: Rds.GroupMembersColumn().GroupId(),
                        where: Rds.GroupMembersWhere()
                            .GroupId(groupId)
                            .Add(or: Rds.GroupMembersWhere()
                                .DeptId(deptId, _using: deptId > 0)
                                .UserId(userId, _using: userId > 0))
                            .Add(or: Rds.GroupMembersWhere()
                                .Sub(sub: Rds.ExistsDepts(where: Rds.DeptsWhere()
                                    .DeptId(raw: "\"GroupMembers\".\"DeptId\"")))
                                .Sub(sub: Rds.ExistsUsers(where: Rds.UsersWhere()
                                    .UserId(raw: "\"GroupMembers\".\"UserId\"")))),
                        top: 1))
                            .AsEnumerable()
                            .Count() == 1
                : false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SelectableMembersJson(Context context)
        {
            return new ResponseCollection().Html(
                "#SelectableMembers",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: SelectableMembers(
                        context: context)))
                            .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder SelectableMembers(
            this HtmlBuilder hb, Context context)
        {
            return hb.FieldSelectable(
                controlId: "SelectableMembers",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlWrapperCss: " h300",
                labelText: Displays.SelectableMembers(context: context),
                listItemCollection: SelectableMembers(context: context),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlCss: "button-icon post",
                            text: Displays.Add(context: context),
                            onClick: "$p.addSelected($(this), $('#CurrentMembers'));",
                            icon: "ui-icon-circle-triangle-w",
                            action: "SelectableMembers",
                            method: "post")
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchMemberText",
                            controlCss: " always-send auto-postback w100",
                            placeholder: Displays.Search(context: context),
                            action: "SelectableMembers",
                            method: "post")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SelectableMembers(Context context)
        {
            var data = new Dictionary<string, ControlData>();
            var searchText = context.Forms.Data("SearchMemberText");
            if (!searchText.IsNullOrEmpty())
            {
                var currentMembers = context.Forms.List("CurrentMembersAll");
                Repository.ExecuteTable(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectDepts(
                            column: Rds.DeptsColumn()
                                .DeptId()
                                .DeptCode()
                                .Add("0 as \"UserId\"")
                                .Add("'' as \"UserCode\"")
                                .Add("0 as \"IsUser\""),
                            where: Rds.DeptsWhere()
                                .TenantId(context.TenantId)
                                .DeptId_In(
                                    currentMembers?
                                        .Where(o => o.StartsWith("Dept,"))
                                        .Select(o => o.Split_2nd().ToInt()),
                                    negative: true)
                                .SqlWhereLike(
                                    tableName: "Depts",
                                    name: "SearchText",
                                    searchText: searchText,
                                    clauseCollection: new List<string>()
                                    {
                                        Rds.Depts_DeptCode_WhereLike(factory: context),
                                        Rds.Depts_DeptName_WhereLike(factory: context)
                                    })),
                        Rds.SelectUsers(
                            unionType: Sqls.UnionTypes.Union,
                            column: Rds.UsersColumn()
                                .Add("0 as \"DeptId\"")
                                .Add("'' as \"DeptCode\"")
                                .UserId()
                                .UserCode()
                                .Add("1 as \"IsUser\""),
                            join: Rds.UsersJoin()
                                .Add(new SqlJoin(
                                    tableBracket: "\"Depts\"",
                                    joinType: SqlJoin.JoinTypes.LeftOuter,
                                    joinExpression: "\"Users\".\"DeptId\"=\"Depts\".\"DeptId\"")),
                            where: Rds.UsersWhere()
                                .TenantId(context.TenantId)
                                .UserId_In(
                                    currentMembers?
                                        .Where(o => o.StartsWith("User,"))
                                        .Select(o => o.Split_2nd().ToInt()),
                                    negative: true)
                                .SqlWhereLike(
                                    tableName: "Users",
                                    name: "SearchText",
                                    searchText: searchText,
                                    clauseCollection: new List<string>()
                                    {
                                        Rds.Users_LoginId_WhereLike(factory: context),
                                        Rds.Users_Name_WhereLike(factory: context),
                                        Rds.Users_UserCode_WhereLike(factory: context),
                                        Rds.Users_Body_WhereLike(factory: context),
                                        Rds.Depts_DeptCode_WhereLike(factory: context),
                                        Rds.Depts_DeptName_WhereLike(factory: context),
                                        Rds.Depts_Body_WhereLike(factory: context)
                                    })
                                .Users_Disabled(false))
                    })
                        .AsEnumerable()
                        .OrderBy(dataRow => dataRow.Bool("IsUser"))
                        .ThenBy(dataRow => dataRow.String("DeptCode"))
                        .ThenBy(dataRow => dataRow.Int("DeptId"))
                        .ThenBy(dataRow => dataRow.String("UserCode"))
                        .ForEach(dataRow =>
                            data.AddMember(
                                context: context,
                                dataRow: dataRow));
            }
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AddMember(
            this Dictionary<string, ControlData> data, Context context, DataRow dataRow)
        {
            var deptId = dataRow.Int("DeptId");
            var userId = dataRow.Int("UserId");
            var admin = dataRow.Table.Columns.Contains("Admin")
                ? dataRow.Bool("Admin")
                : false;
            var manager = admin
                ? $"({Displays.Manager(context: context)})"
                : string.Empty;
            if (deptId > 0)
            {
                var dept = SiteInfo.Dept(
                    tenantId: context.TenantId,
                    deptId: deptId);
                data.Add(
                    $"Dept,{deptId}," + admin,
                    new ControlData(
                        text: dept.SelectableText(
                            context: context,
                            format: Parameters.GroupMembers.DeptFormat) + manager,
                        title: dept?.Tooltip()));
            }
            else if (userId > 0)
            {
                var user = SiteInfo.User(context, userId);
                data.Add(
                    $"User,{userId},{admin}",
                    new ControlData(
                        text: user.SelectableText(
                            context: context,
                            format: Parameters.GroupMembers.UserFormat) + manager,
                        title: user?.Tooltip(context: context)));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            int groupId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var invalid = GroupValidators.OnEntry(
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
            var siteGroups = siteModel != null
                ? SiteInfo.SiteGroups(context, siteModel.InheritPermission)?
                .Where(o => !SiteInfo.User(context, o).Disabled).ToArray()
                : null;
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (groupId > 0)
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash.Add("GroupId", groupId.ToString());
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
                    var groupCollection = new GroupCollection(
                        context: context,
                        ss: ss,
                        where: view.Where(
                            context: context,
                            ss: ss)
                                .Groups_TenantId(context.TenantId)
                                .SqlWhereLike(
                                    tableName: "Groups",
                                    name: "SearchText",
                                    searchText: view.ColumnFilterHash
                                    ?.Where(f => f.Key == "SearchText")
                                    ?.Select(f => f.Value)
                                    ?.FirstOrDefault(),
                                    clauseCollection: new List<string>()
                                    {
                                        Rds.Groups_GroupId_WhereLike(factory: context),
                                        Rds.Groups_GroupName_WhereLike(factory: context),
                                        Rds.Groups_Body_WhereLike(factory: context)
                                    })
                                .Add(
                                    tableName: "Groups",
                                    subLeft: Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn().GroupMembersCount(),
                                    where: Rds.GroupMembersWhere().UserId(userId).GroupId(raw: "\"Groups\".\"GroupId\"").Add(raw: "\"Groups\".\"GroupId\">0")),
                                    _operator: ">0",
                                    _using: userId.HasValue),
                        orderBy: view.OrderBy(
                            context: context,
                            ss: ss),
                        offset: api?.Offset ?? 0,
                        pageSize: pageSize,
                        tableType: tableType);
                    groupCollection.ForEach(groupModel =>
                    {
                        var data = new Dictionary<string, ControlData>();
                        GroupMembers(
                            context: context,
                            groupId: groupModel.GroupId)
                                .ForEach(datarow =>
                                    data.AddMember(
                                        context: context,
                                        dataRow: datarow));
                        groupModel.GroupMembers = data.Select(
                            groupMember => groupMember.Key).ToList<string>();
                    });
                    var groups = siteGroups == null
                        ? groupCollection
                        : groupCollection.Join(siteGroups, c => c.GroupId, s => s, (c, s) => c);
                    return ApiResults.Get(new
                    {
                        StatusCode = 200,
                        Response = new
                        {
                            Offset = api?.Offset ?? 0,
                            PageSize = pageSize,
                            TotalCount = groups.Count(),
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
        public static ContentResultInheritance CreateByApi(Context context, SiteSettings ss)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var groupModel = new GroupModel(context, ss, 0, setByApi: true);
            var invalid = GroupValidators.OnCreating(
                context: context,
                ss: ss,
                groupModel: groupModel,
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
                if (groupModel.GetType().GetField(column.ColumnName).GetValue(groupModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            var errorData = groupModel.Create(
                context: context,
                ss: ss,
                setByApi: true);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        groupModel.GroupId,
                        Displays.Created(
                            context: context,
                            data: groupModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateByApi(Context context, SiteSettings ss, int groupId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var groupModel = new GroupModel(context, ss, groupId: groupId, setByApi: true);
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = GroupValidators.OnUpdating(
                context: context,
                ss: ss,
                groupModel: groupModel,
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
                if (groupModel.GetType().GetField(column.ColumnName).GetValue(groupModel).ToString().IsNullOrEmpty())
                {
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotRequiredColumn),
                        data: column.ColumnName);
                }
            }
            var errorData = groupModel.Update(
                context: context,
                ss: ss,
                setByApi: true,
                get: false);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        groupModel.GroupId,
                        Displays.Updated(
                            context: context,
                            data: groupModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance DeleteByApi(Context context, SiteSettings ss, int groupId)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var groupModel = new GroupModel(context, ss, groupId: groupId, setByApi: true);
            if (groupModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = GroupValidators.OnDeleting(
                context: context,
                ss: ss,
                groupModel: groupModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                       context: context,
                       errorData: invalid);
            }
            var errorData = groupModel.Delete(context: context, ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        groupModel.GroupId,
                        Displays.Deleted(
                            context: context,
                            data: groupModel.Title.Value));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static List<ParameterAccessor.Parts.ExtendedField> GetExtendedFields(Context context)
        {
            return new List<ParameterAccessor.Parts.ExtendedField>()
            {
                new ParameterAccessor.Parts.ExtendedField()
                {
                    Name = "Depts",
                    FieldType = "Filter",
                    TypeName = "nvarchar",
                    LabelText = Displays.Depts(context: context),
                    After = "Disabled"
                },
                new ParameterAccessor.Parts.ExtendedField()
                {
                    Name = "Users",
                    FieldType = "Filter",
                    TypeName = "nvarchar",
                    LabelText = Displays.Users(context: context),
                    After = "Depts"
                }
            };
        }
    }
}
