using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
    public static class ResultUtilities
    {
        public static string Index(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
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
            Action viewModeBody,
            Aggregations aggregations = null)
        {
            var invalid = ResultValidators.OnEntry(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            return hb.Template(
                context: context,
                ss: ss,
                view: view,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Results",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ResultsForm")
                            .Class("main-form")
                            .Action(Locations.Action(
                                context: context,
                                controller: context.Controller,
                                id: ss.SiteId)),
                        action: () => hb
                            .ViewSelector(
                                context: context,
                                ss: ss,
                                view: view)
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
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest,
                                backButton: !context.Publish)
                            .Div(css: "margin-bottom")
                            .Hidden(
                                controlId: "TableName",
                                value: "Results")
                            .Hidden(
                                controlId: "BaseUrl",
                                value: Locations.BaseUrl(context: context)))
                    .EditorDialog(context: context, ss: ss)
                    .DropDownSearchDialog(
                        context: context,
                        id: ss.SiteId)
                    .MoveDialog(context: context, bulk: true)
                    .ImportSettingsDialog(context: context)
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export(context: context))))
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
                    body: new HtmlBuilder()
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view))
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
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class(ss.GridCss())
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction(action)
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: action))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        gridData.DataRows.Count(),
                        gridData.TotalCount)
                            .ToString())
                .Button(
                    controlId: "ViewSorter",
                    controlCss: "hidden",
                    action: action,
                    method: "post")
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
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
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
                .Append("#Grid", new HtmlBuilder().GridRows(
                    context: context,
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck,
                    action: action))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    gridData.DataRows.Count(),
                    gridData.TotalCount))
                .Paging("#Grid")
                .Message(message)
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false,
            string action = "GridRows")
        {
            var checkAll = clearCheck
                ? false
                : context.Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            context: context,
                            columns: columns, 
                            view: view,
                            checkAll: checkAll,
                            action: action))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    context: context,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.ResultsColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.ResultsColumn(column));
            return sqlColumnCollection;
        }

        public static string TrashBox(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TrashBoxCommands(context: context, ss: ss)
                    .Grid(
                        context: context,
                        ss: ss,
                        gridData: gridData,
                        view: view,
                        action: "TrashBoxGridRows"));
        }

        public static string TrashBoxJson(Context context, SiteSettings ss)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .TrashBoxCommands(context: context, ss: ss)
                        .Grid(
                            context: context,
                            ss: ss,
                            gridData: gridData,
                            view: view,
                            action: "TrashBoxGridRows"))
                .ToJson();
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            ResultModel resultModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    resultModel: resultModel);
            }
            else
            {
                var mine = resultModel.Mine(context: context);
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.SiteId)
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
                                    value: resultModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ResultId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ResultId)
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
                                    value: resultModel.Ver)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Title)
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
                                    value: resultModel.Body)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.TitleBody)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Status":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Status)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Manager":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Manager)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "Owner":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.Owner)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ClassZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.ClassZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "NumZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.NumZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DateZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DateZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "DescriptionZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.DescriptionZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CheckZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.CheckZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsA":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsA)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsB":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsB)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsC":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsC)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsD":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsD)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsE":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsE)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsF":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsF)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsG":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsG)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsH":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsH)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsI":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsI)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsJ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsJ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsK":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsK)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsL":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsL)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsM":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsM)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsN":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsN)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsO":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsO)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsP":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsP)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsQ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsQ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsR":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsR)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsS":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsS)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsT":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsT)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsU":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsU)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsV":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsV)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsW":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsW)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsX":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsX)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsY":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsY)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "AttachmentsZ":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.AttachmentsZ)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "SiteTitle":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: resultModel.SiteTitle)
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
                                    value: resultModel.Comments)
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
                                    value: resultModel.Creator)
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
                                    value: resultModel.Updator)
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
                                    value: resultModel.CreatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string gridDesign,
            ResultModel resultModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = resultModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = resultModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "ResultId": value = resultModel.ResultId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = resultModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = resultModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = resultModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = resultModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = resultModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "Manager": value = resultModel.Manager.GridText(
                        context: context,
                        column: column); break;
                    case "Owner": value = resultModel.Owner.GridText(
                        context: context,
                        column: column); break;
                    case "ClassA": value = resultModel.ClassA.GridText(
                        context: context,
                        column: column); break;
                    case "ClassB": value = resultModel.ClassB.GridText(
                        context: context,
                        column: column); break;
                    case "ClassC": value = resultModel.ClassC.GridText(
                        context: context,
                        column: column); break;
                    case "ClassD": value = resultModel.ClassD.GridText(
                        context: context,
                        column: column); break;
                    case "ClassE": value = resultModel.ClassE.GridText(
                        context: context,
                        column: column); break;
                    case "ClassF": value = resultModel.ClassF.GridText(
                        context: context,
                        column: column); break;
                    case "ClassG": value = resultModel.ClassG.GridText(
                        context: context,
                        column: column); break;
                    case "ClassH": value = resultModel.ClassH.GridText(
                        context: context,
                        column: column); break;
                    case "ClassI": value = resultModel.ClassI.GridText(
                        context: context,
                        column: column); break;
                    case "ClassJ": value = resultModel.ClassJ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassK": value = resultModel.ClassK.GridText(
                        context: context,
                        column: column); break;
                    case "ClassL": value = resultModel.ClassL.GridText(
                        context: context,
                        column: column); break;
                    case "ClassM": value = resultModel.ClassM.GridText(
                        context: context,
                        column: column); break;
                    case "ClassN": value = resultModel.ClassN.GridText(
                        context: context,
                        column: column); break;
                    case "ClassO": value = resultModel.ClassO.GridText(
                        context: context,
                        column: column); break;
                    case "ClassP": value = resultModel.ClassP.GridText(
                        context: context,
                        column: column); break;
                    case "ClassQ": value = resultModel.ClassQ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassR": value = resultModel.ClassR.GridText(
                        context: context,
                        column: column); break;
                    case "ClassS": value = resultModel.ClassS.GridText(
                        context: context,
                        column: column); break;
                    case "ClassT": value = resultModel.ClassT.GridText(
                        context: context,
                        column: column); break;
                    case "ClassU": value = resultModel.ClassU.GridText(
                        context: context,
                        column: column); break;
                    case "ClassV": value = resultModel.ClassV.GridText(
                        context: context,
                        column: column); break;
                    case "ClassW": value = resultModel.ClassW.GridText(
                        context: context,
                        column: column); break;
                    case "ClassX": value = resultModel.ClassX.GridText(
                        context: context,
                        column: column); break;
                    case "ClassY": value = resultModel.ClassY.GridText(
                        context: context,
                        column: column); break;
                    case "ClassZ": value = resultModel.ClassZ.GridText(
                        context: context,
                        column: column); break;
                    case "NumA": value = resultModel.NumA.GridText(
                        context: context,
                        column: column); break;
                    case "NumB": value = resultModel.NumB.GridText(
                        context: context,
                        column: column); break;
                    case "NumC": value = resultModel.NumC.GridText(
                        context: context,
                        column: column); break;
                    case "NumD": value = resultModel.NumD.GridText(
                        context: context,
                        column: column); break;
                    case "NumE": value = resultModel.NumE.GridText(
                        context: context,
                        column: column); break;
                    case "NumF": value = resultModel.NumF.GridText(
                        context: context,
                        column: column); break;
                    case "NumG": value = resultModel.NumG.GridText(
                        context: context,
                        column: column); break;
                    case "NumH": value = resultModel.NumH.GridText(
                        context: context,
                        column: column); break;
                    case "NumI": value = resultModel.NumI.GridText(
                        context: context,
                        column: column); break;
                    case "NumJ": value = resultModel.NumJ.GridText(
                        context: context,
                        column: column); break;
                    case "NumK": value = resultModel.NumK.GridText(
                        context: context,
                        column: column); break;
                    case "NumL": value = resultModel.NumL.GridText(
                        context: context,
                        column: column); break;
                    case "NumM": value = resultModel.NumM.GridText(
                        context: context,
                        column: column); break;
                    case "NumN": value = resultModel.NumN.GridText(
                        context: context,
                        column: column); break;
                    case "NumO": value = resultModel.NumO.GridText(
                        context: context,
                        column: column); break;
                    case "NumP": value = resultModel.NumP.GridText(
                        context: context,
                        column: column); break;
                    case "NumQ": value = resultModel.NumQ.GridText(
                        context: context,
                        column: column); break;
                    case "NumR": value = resultModel.NumR.GridText(
                        context: context,
                        column: column); break;
                    case "NumS": value = resultModel.NumS.GridText(
                        context: context,
                        column: column); break;
                    case "NumT": value = resultModel.NumT.GridText(
                        context: context,
                        column: column); break;
                    case "NumU": value = resultModel.NumU.GridText(
                        context: context,
                        column: column); break;
                    case "NumV": value = resultModel.NumV.GridText(
                        context: context,
                        column: column); break;
                    case "NumW": value = resultModel.NumW.GridText(
                        context: context,
                        column: column); break;
                    case "NumX": value = resultModel.NumX.GridText(
                        context: context,
                        column: column); break;
                    case "NumY": value = resultModel.NumY.GridText(
                        context: context,
                        column: column); break;
                    case "NumZ": value = resultModel.NumZ.GridText(
                        context: context,
                        column: column); break;
                    case "DateA": value = resultModel.DateA.GridText(
                        context: context,
                        column: column); break;
                    case "DateB": value = resultModel.DateB.GridText(
                        context: context,
                        column: column); break;
                    case "DateC": value = resultModel.DateC.GridText(
                        context: context,
                        column: column); break;
                    case "DateD": value = resultModel.DateD.GridText(
                        context: context,
                        column: column); break;
                    case "DateE": value = resultModel.DateE.GridText(
                        context: context,
                        column: column); break;
                    case "DateF": value = resultModel.DateF.GridText(
                        context: context,
                        column: column); break;
                    case "DateG": value = resultModel.DateG.GridText(
                        context: context,
                        column: column); break;
                    case "DateH": value = resultModel.DateH.GridText(
                        context: context,
                        column: column); break;
                    case "DateI": value = resultModel.DateI.GridText(
                        context: context,
                        column: column); break;
                    case "DateJ": value = resultModel.DateJ.GridText(
                        context: context,
                        column: column); break;
                    case "DateK": value = resultModel.DateK.GridText(
                        context: context,
                        column: column); break;
                    case "DateL": value = resultModel.DateL.GridText(
                        context: context,
                        column: column); break;
                    case "DateM": value = resultModel.DateM.GridText(
                        context: context,
                        column: column); break;
                    case "DateN": value = resultModel.DateN.GridText(
                        context: context,
                        column: column); break;
                    case "DateO": value = resultModel.DateO.GridText(
                        context: context,
                        column: column); break;
                    case "DateP": value = resultModel.DateP.GridText(
                        context: context,
                        column: column); break;
                    case "DateQ": value = resultModel.DateQ.GridText(
                        context: context,
                        column: column); break;
                    case "DateR": value = resultModel.DateR.GridText(
                        context: context,
                        column: column); break;
                    case "DateS": value = resultModel.DateS.GridText(
                        context: context,
                        column: column); break;
                    case "DateT": value = resultModel.DateT.GridText(
                        context: context,
                        column: column); break;
                    case "DateU": value = resultModel.DateU.GridText(
                        context: context,
                        column: column); break;
                    case "DateV": value = resultModel.DateV.GridText(
                        context: context,
                        column: column); break;
                    case "DateW": value = resultModel.DateW.GridText(
                        context: context,
                        column: column); break;
                    case "DateX": value = resultModel.DateX.GridText(
                        context: context,
                        column: column); break;
                    case "DateY": value = resultModel.DateY.GridText(
                        context: context,
                        column: column); break;
                    case "DateZ": value = resultModel.DateZ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionA": value = resultModel.DescriptionA.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionB": value = resultModel.DescriptionB.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionC": value = resultModel.DescriptionC.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionD": value = resultModel.DescriptionD.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionE": value = resultModel.DescriptionE.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionF": value = resultModel.DescriptionF.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionG": value = resultModel.DescriptionG.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionH": value = resultModel.DescriptionH.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionI": value = resultModel.DescriptionI.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionJ": value = resultModel.DescriptionJ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionK": value = resultModel.DescriptionK.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionL": value = resultModel.DescriptionL.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionM": value = resultModel.DescriptionM.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionN": value = resultModel.DescriptionN.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionO": value = resultModel.DescriptionO.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionP": value = resultModel.DescriptionP.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionQ": value = resultModel.DescriptionQ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionR": value = resultModel.DescriptionR.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionS": value = resultModel.DescriptionS.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionT": value = resultModel.DescriptionT.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionU": value = resultModel.DescriptionU.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionV": value = resultModel.DescriptionV.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionW": value = resultModel.DescriptionW.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionX": value = resultModel.DescriptionX.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionY": value = resultModel.DescriptionY.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionZ": value = resultModel.DescriptionZ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckA": value = resultModel.CheckA.GridText(
                        context: context,
                        column: column); break;
                    case "CheckB": value = resultModel.CheckB.GridText(
                        context: context,
                        column: column); break;
                    case "CheckC": value = resultModel.CheckC.GridText(
                        context: context,
                        column: column); break;
                    case "CheckD": value = resultModel.CheckD.GridText(
                        context: context,
                        column: column); break;
                    case "CheckE": value = resultModel.CheckE.GridText(
                        context: context,
                        column: column); break;
                    case "CheckF": value = resultModel.CheckF.GridText(
                        context: context,
                        column: column); break;
                    case "CheckG": value = resultModel.CheckG.GridText(
                        context: context,
                        column: column); break;
                    case "CheckH": value = resultModel.CheckH.GridText(
                        context: context,
                        column: column); break;
                    case "CheckI": value = resultModel.CheckI.GridText(
                        context: context,
                        column: column); break;
                    case "CheckJ": value = resultModel.CheckJ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckK": value = resultModel.CheckK.GridText(
                        context: context,
                        column: column); break;
                    case "CheckL": value = resultModel.CheckL.GridText(
                        context: context,
                        column: column); break;
                    case "CheckM": value = resultModel.CheckM.GridText(
                        context: context,
                        column: column); break;
                    case "CheckN": value = resultModel.CheckN.GridText(
                        context: context,
                        column: column); break;
                    case "CheckO": value = resultModel.CheckO.GridText(
                        context: context,
                        column: column); break;
                    case "CheckP": value = resultModel.CheckP.GridText(
                        context: context,
                        column: column); break;
                    case "CheckQ": value = resultModel.CheckQ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckR": value = resultModel.CheckR.GridText(
                        context: context,
                        column: column); break;
                    case "CheckS": value = resultModel.CheckS.GridText(
                        context: context,
                        column: column); break;
                    case "CheckT": value = resultModel.CheckT.GridText(
                        context: context,
                        column: column); break;
                    case "CheckU": value = resultModel.CheckU.GridText(
                        context: context,
                        column: column); break;
                    case "CheckV": value = resultModel.CheckV.GridText(
                        context: context,
                        column: column); break;
                    case "CheckW": value = resultModel.CheckW.GridText(
                        context: context,
                        column: column); break;
                    case "CheckX": value = resultModel.CheckX.GridText(
                        context: context,
                        column: column); break;
                    case "CheckY": value = resultModel.CheckY.GridText(
                        context: context,
                        column: column); break;
                    case "CheckZ": value = resultModel.CheckZ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsA": value = resultModel.AttachmentsA.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsB": value = resultModel.AttachmentsB.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsC": value = resultModel.AttachmentsC.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsD": value = resultModel.AttachmentsD.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsE": value = resultModel.AttachmentsE.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsF": value = resultModel.AttachmentsF.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsG": value = resultModel.AttachmentsG.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsH": value = resultModel.AttachmentsH.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsI": value = resultModel.AttachmentsI.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsJ": value = resultModel.AttachmentsJ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsK": value = resultModel.AttachmentsK.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsL": value = resultModel.AttachmentsL.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsM": value = resultModel.AttachmentsM.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsN": value = resultModel.AttachmentsN.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsO": value = resultModel.AttachmentsO.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsP": value = resultModel.AttachmentsP.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsQ": value = resultModel.AttachmentsQ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsR": value = resultModel.AttachmentsR.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsS": value = resultModel.AttachmentsS.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsT": value = resultModel.AttachmentsT.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsU": value = resultModel.AttachmentsU.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsV": value = resultModel.AttachmentsV.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsW": value = resultModel.AttachmentsW.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsX": value = resultModel.AttachmentsX.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsY": value = resultModel.AttachmentsY.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsZ": value = resultModel.AttachmentsZ.GridText(
                        context: context,
                        column: column); break;
                    case "SiteTitle": value = resultModel.SiteTitle.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = resultModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = resultModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = resultModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = resultModel.CreatedTime.GridText(
                        context: context,
                        column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return HtmlTemplates.Error(context, Error.Types.ItemsLimit);
            }
            return Editor(
                context: context,
                ss: ss,
                resultModel: new ResultModel(
                    context: context,
                    ss: ss,
                    methodType: BaseModel.MethodTypes.New,
                    setByForm: true));
        }

        public static string Editor(
            Context context, SiteSettings ss, long resultId, bool clearSessions)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            resultModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: ss,
                resultId: resultModel.ResultId,
                siteId: resultModel.SiteId);
            return Editor(
                context: context,
                ss: ss,
                resultModel: resultModel);
        }

        public static string Editor(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool editInDialog = false)
        {
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    siteId: resultModel.SiteId,
                    referenceId: resultModel.ResultId,
                    action: () => hb
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            editInDialog: editInDialog))
                                .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: resultModel.VerType,
                    methodType: resultModel.MethodType,
                    siteId: resultModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Results",
                    title: resultModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : resultModel.Title.DisplayValue,
                    useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: resultModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: resultModel.MethodType),
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            resultModel: resultModel)
                        .Hidden(controlId: "TableName", value: "Results")
                        .Hidden(controlId: "Id", value: resultModel.ResultId.ToString())
                        .Hidden(controlId: "TriggerRelatingColumns", value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("ResultForm")
                        .Class("main-form confirm-reload")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: resultModel.ResultId != 0 
                                ? resultModel.ResultId
                                : resultModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: resultModel,
                            tableName: "Results")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: resultModel.Comments,
                                    column: commentsColumn,
                                    verType: resultModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(
                                context: context,
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New
                                    && !context.Publish)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: context.CanManagePermission(ss: ss) &&
                                    resultModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: resultModel.SiteId,
                                verType: resultModel.VerType,
                                referenceId: resultModel.ResultId,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
                                        ss: ss,
                                        resultModel: resultModel)))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "FromSiteId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("FromSiteId"),
                            _using: context.QueryStrings.Long("FromSiteId") > 0)
                        .Hidden(
                            controlId: "LinkId",
                            css: "control-hidden always-send",
                            value: context.QueryStrings.Data("LinkId"),
                            _using: context.QueryStrings.Long("LinkId") > 0)
                        .Hidden(
                            controlId: "MethodType",
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "always-send",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: resultModel.SwitchTargets?.Join(),
                            _using: !context.Ajax))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Results",
                    referenceId: resultModel.ResultId,
                    referenceVer: resultModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    referenceType: "items",
                    id: resultModel.ResultId)
                .MoveDialog(context: context)
                .OutgoingMailDialog()
                .PermissionsDialog(context: context)
                .EditorExtensions(
                    context: context,
                    resultModel: resultModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New
                        && !context.Publish,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(_using: context.CanManagePermission(ss: ss) &&
                        resultModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl(context: context))));
        }

        public static string PreviewTemplate(Context context, SiteSettings ss)
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
                                text: Displays.Editor(context: context)))
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Grid",
                                text: Displays.Grid(context: context))))
                    .FieldSet(
                        id: name + "Editor",
                        action: () => hb
                            .FieldSetGeneralColumns(
                                context: context,
                                ss: ss,
                                resultModel: new ResultModel(),
                                preview: true))
                    .FieldSet(
                        id: name + "Grid",
                        action: () => hb
                            .Table(css: "grid", action: () => hb
                                .THead(action: () => hb
                                    .GridHeader(
                                        context: context,
                                        columns: ss.GetGridColumns(context: context),
                                        view: new View(context: context, ss: ss),
                                        sort: false,
                                        checkRow: false)))))
                                            .ToString();
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool editInDialog = false)
        {
            var mine = resultModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    resultModel: resultModel,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool preview = false,
            bool editInDialog = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "ResultId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ResultId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Title
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Body
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Status":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Status
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Manager":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Manager
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Owner":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.Owner
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.ClassZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.NumZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DateZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.DescriptionZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.CheckZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: resultModel.MethodType,
                            value: resultModel.AttachmentsZ
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
                    baseModel: resultModel);
                if (!editInDialog)
                {
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkId: resultModel.ResultId,
                                methodType: resultModel.MethodType))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: resultModel.ResultId));
                }
            }
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            return hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long resultId)
        {
            return EditorResponse(context, ss, new ResultModel(
                context, ss, resultId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            Message message = null,
            string switchTargets = null)
        {
            resultModel.MethodType = BaseModel.MethodTypes.Edit;
            var editInDialog = context.Forms.Bool("EditInDialog");
            return editInDialog
                ? new ResultsResponseCollection(resultModel)
                    .Response("id", resultModel.ResultId.ToString())
                    .Html("#EditInDialogBody", Editor(
                        context: context,
                        ss: ss,
                        resultModel: resultModel,
                        editInDialog: editInDialog))
                    .Invoke("openEditorDialog")
                    .Events("on_editor_load")
                : new ResultsResponseCollection(resultModel)
                    .Response("id", resultModel.ResultId.ToString())
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, resultModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .ClearFormData()
                    .Events("on_editor_load");
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long resultId, long siteId)
        {
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                setSession: false);
            var where = view.Where(context: context, ss: ss);
            var join = ss.Join(context: context);
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectResults(
                                column: Rds.ResultsColumn().ResultId(),
                                join: join,
                                where: where,
                                orderBy: view.OrderBy(context: context, ss: ss)
                                    .Results_UpdatedTime(SqlOrderBy.Types.desc)))
                                        .AsEnumerable()
                                        .Select(o => o["ResultId"].ToLong())
                                        .ToList()
                        : new List<long>();
            if (!switchTargets.Contains(resultId))
            {
                switchTargets.Add(resultId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this ResultsResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var mine = resultModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "ResultId":
                            res.Val(
                                "#Results_ResultId",
                                resultModel.ResultId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Results_Title",
                                resultModel.Title.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Results_Body",
                                resultModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Status":
                            res.Val(
                                "#Results_Status",
                                resultModel.Status.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Results_Manager",
                                resultModel.Manager.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Results_Owner",
                                resultModel.Owner.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassA":
                            res.Val(
                                "#Results_ClassA",
                                resultModel.ClassA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassB":
                            res.Val(
                                "#Results_ClassB",
                                resultModel.ClassB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassC":
                            res.Val(
                                "#Results_ClassC",
                                resultModel.ClassC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassD":
                            res.Val(
                                "#Results_ClassD",
                                resultModel.ClassD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassE":
                            res.Val(
                                "#Results_ClassE",
                                resultModel.ClassE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassF":
                            res.Val(
                                "#Results_ClassF",
                                resultModel.ClassF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassG":
                            res.Val(
                                "#Results_ClassG",
                                resultModel.ClassG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassH":
                            res.Val(
                                "#Results_ClassH",
                                resultModel.ClassH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassI":
                            res.Val(
                                "#Results_ClassI",
                                resultModel.ClassI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassJ":
                            res.Val(
                                "#Results_ClassJ",
                                resultModel.ClassJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassK":
                            res.Val(
                                "#Results_ClassK",
                                resultModel.ClassK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassL":
                            res.Val(
                                "#Results_ClassL",
                                resultModel.ClassL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassM":
                            res.Val(
                                "#Results_ClassM",
                                resultModel.ClassM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassN":
                            res.Val(
                                "#Results_ClassN",
                                resultModel.ClassN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassO":
                            res.Val(
                                "#Results_ClassO",
                                resultModel.ClassO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassP":
                            res.Val(
                                "#Results_ClassP",
                                resultModel.ClassP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassQ":
                            res.Val(
                                "#Results_ClassQ",
                                resultModel.ClassQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassR":
                            res.Val(
                                "#Results_ClassR",
                                resultModel.ClassR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassS":
                            res.Val(
                                "#Results_ClassS",
                                resultModel.ClassS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassT":
                            res.Val(
                                "#Results_ClassT",
                                resultModel.ClassT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassU":
                            res.Val(
                                "#Results_ClassU",
                                resultModel.ClassU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassV":
                            res.Val(
                                "#Results_ClassV",
                                resultModel.ClassV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassW":
                            res.Val(
                                "#Results_ClassW",
                                resultModel.ClassW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassX":
                            res.Val(
                                "#Results_ClassX",
                                resultModel.ClassX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassY":
                            res.Val(
                                "#Results_ClassY",
                                resultModel.ClassY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassZ":
                            res.Val(
                                "#Results_ClassZ",
                                resultModel.ClassZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumA":
                            res.Val(
                                "#Results_NumA",
                                resultModel.NumA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumB":
                            res.Val(
                                "#Results_NumB",
                                resultModel.NumB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumC":
                            res.Val(
                                "#Results_NumC",
                                resultModel.NumC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumD":
                            res.Val(
                                "#Results_NumD",
                                resultModel.NumD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumE":
                            res.Val(
                                "#Results_NumE",
                                resultModel.NumE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumF":
                            res.Val(
                                "#Results_NumF",
                                resultModel.NumF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumG":
                            res.Val(
                                "#Results_NumG",
                                resultModel.NumG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumH":
                            res.Val(
                                "#Results_NumH",
                                resultModel.NumH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumI":
                            res.Val(
                                "#Results_NumI",
                                resultModel.NumI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumJ":
                            res.Val(
                                "#Results_NumJ",
                                resultModel.NumJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumK":
                            res.Val(
                                "#Results_NumK",
                                resultModel.NumK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumL":
                            res.Val(
                                "#Results_NumL",
                                resultModel.NumL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumM":
                            res.Val(
                                "#Results_NumM",
                                resultModel.NumM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumN":
                            res.Val(
                                "#Results_NumN",
                                resultModel.NumN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumO":
                            res.Val(
                                "#Results_NumO",
                                resultModel.NumO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumP":
                            res.Val(
                                "#Results_NumP",
                                resultModel.NumP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumQ":
                            res.Val(
                                "#Results_NumQ",
                                resultModel.NumQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumR":
                            res.Val(
                                "#Results_NumR",
                                resultModel.NumR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumS":
                            res.Val(
                                "#Results_NumS",
                                resultModel.NumS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumT":
                            res.Val(
                                "#Results_NumT",
                                resultModel.NumT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumU":
                            res.Val(
                                "#Results_NumU",
                                resultModel.NumU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumV":
                            res.Val(
                                "#Results_NumV",
                                resultModel.NumV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumW":
                            res.Val(
                                "#Results_NumW",
                                resultModel.NumW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumX":
                            res.Val(
                                "#Results_NumX",
                                resultModel.NumX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumY":
                            res.Val(
                                "#Results_NumY",
                                resultModel.NumY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumZ":
                            res.Val(
                                "#Results_NumZ",
                                resultModel.NumZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateA":
                            res.Val(
                                "#Results_DateA",
                                resultModel.DateA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateB":
                            res.Val(
                                "#Results_DateB",
                                resultModel.DateB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateC":
                            res.Val(
                                "#Results_DateC",
                                resultModel.DateC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateD":
                            res.Val(
                                "#Results_DateD",
                                resultModel.DateD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateE":
                            res.Val(
                                "#Results_DateE",
                                resultModel.DateE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateF":
                            res.Val(
                                "#Results_DateF",
                                resultModel.DateF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateG":
                            res.Val(
                                "#Results_DateG",
                                resultModel.DateG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateH":
                            res.Val(
                                "#Results_DateH",
                                resultModel.DateH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateI":
                            res.Val(
                                "#Results_DateI",
                                resultModel.DateI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateJ":
                            res.Val(
                                "#Results_DateJ",
                                resultModel.DateJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateK":
                            res.Val(
                                "#Results_DateK",
                                resultModel.DateK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateL":
                            res.Val(
                                "#Results_DateL",
                                resultModel.DateL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateM":
                            res.Val(
                                "#Results_DateM",
                                resultModel.DateM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateN":
                            res.Val(
                                "#Results_DateN",
                                resultModel.DateN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateO":
                            res.Val(
                                "#Results_DateO",
                                resultModel.DateO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateP":
                            res.Val(
                                "#Results_DateP",
                                resultModel.DateP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateQ":
                            res.Val(
                                "#Results_DateQ",
                                resultModel.DateQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateR":
                            res.Val(
                                "#Results_DateR",
                                resultModel.DateR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateS":
                            res.Val(
                                "#Results_DateS",
                                resultModel.DateS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateT":
                            res.Val(
                                "#Results_DateT",
                                resultModel.DateT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateU":
                            res.Val(
                                "#Results_DateU",
                                resultModel.DateU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateV":
                            res.Val(
                                "#Results_DateV",
                                resultModel.DateV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateW":
                            res.Val(
                                "#Results_DateW",
                                resultModel.DateW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateX":
                            res.Val(
                                "#Results_DateX",
                                resultModel.DateX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateY":
                            res.Val(
                                "#Results_DateY",
                                resultModel.DateY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateZ":
                            res.Val(
                                "#Results_DateZ",
                                resultModel.DateZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionA":
                            res.Val(
                                "#Results_DescriptionA",
                                resultModel.DescriptionA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionB":
                            res.Val(
                                "#Results_DescriptionB",
                                resultModel.DescriptionB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionC":
                            res.Val(
                                "#Results_DescriptionC",
                                resultModel.DescriptionC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionD":
                            res.Val(
                                "#Results_DescriptionD",
                                resultModel.DescriptionD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionE":
                            res.Val(
                                "#Results_DescriptionE",
                                resultModel.DescriptionE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionF":
                            res.Val(
                                "#Results_DescriptionF",
                                resultModel.DescriptionF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionG":
                            res.Val(
                                "#Results_DescriptionG",
                                resultModel.DescriptionG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionH":
                            res.Val(
                                "#Results_DescriptionH",
                                resultModel.DescriptionH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionI":
                            res.Val(
                                "#Results_DescriptionI",
                                resultModel.DescriptionI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionJ":
                            res.Val(
                                "#Results_DescriptionJ",
                                resultModel.DescriptionJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionK":
                            res.Val(
                                "#Results_DescriptionK",
                                resultModel.DescriptionK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionL":
                            res.Val(
                                "#Results_DescriptionL",
                                resultModel.DescriptionL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionM":
                            res.Val(
                                "#Results_DescriptionM",
                                resultModel.DescriptionM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionN":
                            res.Val(
                                "#Results_DescriptionN",
                                resultModel.DescriptionN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionO":
                            res.Val(
                                "#Results_DescriptionO",
                                resultModel.DescriptionO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionP":
                            res.Val(
                                "#Results_DescriptionP",
                                resultModel.DescriptionP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionQ":
                            res.Val(
                                "#Results_DescriptionQ",
                                resultModel.DescriptionQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionR":
                            res.Val(
                                "#Results_DescriptionR",
                                resultModel.DescriptionR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionS":
                            res.Val(
                                "#Results_DescriptionS",
                                resultModel.DescriptionS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionT":
                            res.Val(
                                "#Results_DescriptionT",
                                resultModel.DescriptionT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionU":
                            res.Val(
                                "#Results_DescriptionU",
                                resultModel.DescriptionU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionV":
                            res.Val(
                                "#Results_DescriptionV",
                                resultModel.DescriptionV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionW":
                            res.Val(
                                "#Results_DescriptionW",
                                resultModel.DescriptionW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionX":
                            res.Val(
                                "#Results_DescriptionX",
                                resultModel.DescriptionX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionY":
                            res.Val(
                                "#Results_DescriptionY",
                                resultModel.DescriptionY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionZ":
                            res.Val(
                                "#Results_DescriptionZ",
                                resultModel.DescriptionZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "CheckA":
                            res.Val(
                                "#Results_CheckA",
                                resultModel.CheckA);
                            break;
                        case "CheckB":
                            res.Val(
                                "#Results_CheckB",
                                resultModel.CheckB);
                            break;
                        case "CheckC":
                            res.Val(
                                "#Results_CheckC",
                                resultModel.CheckC);
                            break;
                        case "CheckD":
                            res.Val(
                                "#Results_CheckD",
                                resultModel.CheckD);
                            break;
                        case "CheckE":
                            res.Val(
                                "#Results_CheckE",
                                resultModel.CheckE);
                            break;
                        case "CheckF":
                            res.Val(
                                "#Results_CheckF",
                                resultModel.CheckF);
                            break;
                        case "CheckG":
                            res.Val(
                                "#Results_CheckG",
                                resultModel.CheckG);
                            break;
                        case "CheckH":
                            res.Val(
                                "#Results_CheckH",
                                resultModel.CheckH);
                            break;
                        case "CheckI":
                            res.Val(
                                "#Results_CheckI",
                                resultModel.CheckI);
                            break;
                        case "CheckJ":
                            res.Val(
                                "#Results_CheckJ",
                                resultModel.CheckJ);
                            break;
                        case "CheckK":
                            res.Val(
                                "#Results_CheckK",
                                resultModel.CheckK);
                            break;
                        case "CheckL":
                            res.Val(
                                "#Results_CheckL",
                                resultModel.CheckL);
                            break;
                        case "CheckM":
                            res.Val(
                                "#Results_CheckM",
                                resultModel.CheckM);
                            break;
                        case "CheckN":
                            res.Val(
                                "#Results_CheckN",
                                resultModel.CheckN);
                            break;
                        case "CheckO":
                            res.Val(
                                "#Results_CheckO",
                                resultModel.CheckO);
                            break;
                        case "CheckP":
                            res.Val(
                                "#Results_CheckP",
                                resultModel.CheckP);
                            break;
                        case "CheckQ":
                            res.Val(
                                "#Results_CheckQ",
                                resultModel.CheckQ);
                            break;
                        case "CheckR":
                            res.Val(
                                "#Results_CheckR",
                                resultModel.CheckR);
                            break;
                        case "CheckS":
                            res.Val(
                                "#Results_CheckS",
                                resultModel.CheckS);
                            break;
                        case "CheckT":
                            res.Val(
                                "#Results_CheckT",
                                resultModel.CheckT);
                            break;
                        case "CheckU":
                            res.Val(
                                "#Results_CheckU",
                                resultModel.CheckU);
                            break;
                        case "CheckV":
                            res.Val(
                                "#Results_CheckV",
                                resultModel.CheckV);
                            break;
                        case "CheckW":
                            res.Val(
                                "#Results_CheckW",
                                resultModel.CheckW);
                            break;
                        case "CheckX":
                            res.Val(
                                "#Results_CheckX",
                                resultModel.CheckX);
                            break;
                        case "CheckY":
                            res.Val(
                                "#Results_CheckY",
                                resultModel.CheckY);
                            break;
                        case "CheckZ":
                            res.Val(
                                "#Results_CheckZ",
                                resultModel.CheckZ);
                            break;
                        case "AttachmentsA":
                            res.ReplaceAll(
                                "#Results_AttachmentsAField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsA.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsB":
                            res.ReplaceAll(
                                "#Results_AttachmentsBField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsB.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsC":
                            res.ReplaceAll(
                                "#Results_AttachmentsCField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsC.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsD":
                            res.ReplaceAll(
                                "#Results_AttachmentsDField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsD.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsE":
                            res.ReplaceAll(
                                "#Results_AttachmentsEField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsE.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsF":
                            res.ReplaceAll(
                                "#Results_AttachmentsFField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsF.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsG":
                            res.ReplaceAll(
                                "#Results_AttachmentsGField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsG.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsH":
                            res.ReplaceAll(
                                "#Results_AttachmentsHField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsH.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsI":
                            res.ReplaceAll(
                                "#Results_AttachmentsIField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsI.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsJ":
                            res.ReplaceAll(
                                "#Results_AttachmentsJField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsJ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsK":
                            res.ReplaceAll(
                                "#Results_AttachmentsKField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsK.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsL":
                            res.ReplaceAll(
                                "#Results_AttachmentsLField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsL.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsM":
                            res.ReplaceAll(
                                "#Results_AttachmentsMField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsM.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsN":
                            res.ReplaceAll(
                                "#Results_AttachmentsNField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsN.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsO":
                            res.ReplaceAll(
                                "#Results_AttachmentsOField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsO.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsP":
                            res.ReplaceAll(
                                "#Results_AttachmentsPField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsP.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsQ":
                            res.ReplaceAll(
                                "#Results_AttachmentsQField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsQ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsR":
                            res.ReplaceAll(
                                "#Results_AttachmentsRField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsR.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsS":
                            res.ReplaceAll(
                                "#Results_AttachmentsSField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsS.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsT":
                            res.ReplaceAll(
                                "#Results_AttachmentsTField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsT.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsU":
                            res.ReplaceAll(
                                "#Results_AttachmentsUField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsU.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsV":
                            res.ReplaceAll(
                                "#Results_AttachmentsVField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsV.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsW":
                            res.ReplaceAll(
                                "#Results_AttachmentsWField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsW.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsX":
                            res.ReplaceAll(
                                "#Results_AttachmentsXField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsX.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsY":
                            res.ReplaceAll(
                                "#Results_AttachmentsYField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsY.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsZ":
                            res.ReplaceAll(
                                "#Results_AttachmentsZField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsZ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static System.Web.Mvc.ContentResult GetByApi(Context context, SiteSettings ss)
        {
            var invalid = IssueValidators.OnEntry(
                context: context,
                ss: ss,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var resultCollection = new ResultCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    "[Items]",
                    SqlJoin.JoinTypes.Inner,
                    "[Results].[ResultId]=[Items].[ReferenceId]")),
                where: view.Where(context: context, ss: ss),
                orderBy: view.OrderBy(context: context, ss: ss, pageSize: pageSize),
                offset: api.Offset,
                pageSize: pageSize,
                countRecord: true);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    api.Offset,
                    PageSize = pageSize,
                    resultCollection.TotalCount,
                    Data = resultCollection.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnEditing(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = resultModel.GetByApi(
                        context: context,
                        ss: ss).ToSingleList()
                }
            }.ToJson());
        }

        public static string Create(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var resultModel = new ResultModel(context, ss, 0, setByForm: true);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = resultModel.Create(context: context, ss: ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: resultModel.Title.DisplayValue));
                    return new ResponseCollection()
                        .Response("id", resultModel.ResultId.ToString())
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : resultModel.ResultId))
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: ss.DuplicatedColumn)?.LabelText)
                                .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult CreateByApi(Context context, SiteSettings ss)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return ApiResults.Error(
                    context: context,
                    type: Error.Types.ItemsLimit);
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: 0,
                setByApi: true);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(context: context, ss: ss);
            var error = resultModel.Create(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Created(
                            context: context,
                            data: resultModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        context: context,
                        type: error,
                        data: ss.GetColumn(
                            context: context,
                            columnName: ss.DuplicatedColumn)?.LabelText);
                default:
                    return ApiResults.Error(
                        context: context,
                        type: error);
            }
        }

        public static string Update(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context, ss: ss, resultId: resultId, setByForm: true);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var error = resultModel.Update(
                context: context,
                ss: ss,
                notice: true,
                permissions: context.Forms.List("CurrentPermissionsAll"),
                permissionChanged: context.Forms.Exists("CurrentPermissionsAll"));
            switch (error)
            {
                case Error.Types.None:
                    var res = new ResultsResponseCollection(resultModel);
                    return ResponseByUpdate(res, context, ss, resultModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: resultModel.Comments,
                            verType: resultModel.VerType)
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: ss.DuplicatedColumn)?.LabelText)
                                .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        context: context,
                        data: resultModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            ResultsResponseCollection res,
            Context context,
            SiteSettings ss,
            ResultModel resultModel)
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
                    where: Rds.ResultsWhere().ResultId(resultModel.ResultId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{resultModel.ResultId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: resultModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, resultModel: resultModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", resultModel.Title.DisplayValue)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: resultModel,
                        tableName: "Results"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: resultModel.ResultId))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: resultModel.Title.DisplayValue))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: resultModel.Comments,
                        deleteCommentId: resultModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                setByApi: true);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(context: context, ss: ss);
            var error = resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Updated(
                            context: context,
                            data: resultModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        context: context,
                        type: error,
                        data: ss.GetColumn(
                            context: context,
                            columnName: ss.DuplicatedColumn)?.LabelText);
                default:
                    return ApiResults.Error(
                        context: context,
                        type: error);
            }
        }

        public static string Copy(Context context, SiteSettings ss, long resultId)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var resultModel = new ResultModel(
                context: context, ss: ss, resultId: resultId, setByForm: true);
            var invalid = ResultValidators.OnCreating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            resultModel.ResultId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                resultModel.Title.Value += Displays.SuffixCopy(context: context);
            }
            if (!context.Forms.Bool("CopyWithComments"))
            {
                resultModel.Comments.Clear();
            }
            ss.Columns
                .Where(column => column.CopyByDefault == true
                    || column.TypeCs == "Attachments")
                .ForEach(column => resultModel.SetDefault(
                    context: context,
                    ss: ss,
                    column: column));
            var error = resultModel.Create(
                context, ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    if (ss.SwitchRecordWithAjax == true)
                    {
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            message: Messages.Copied(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                resultId: resultModel.ResultId,
                                siteId: resultModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Copied(context: context));
                        return new ResponseCollection()
                            .Response("id", resultModel.ResultId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: resultModel.ResultId))
                            .ToJson();
                    }
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: ss.GetColumn(
                            context: context,
                            columnName: ss.DuplicatedColumn)?.LabelText)
                                .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Move(Context context, SiteSettings ss, long resultId)
        {
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(context: context, siteId: siteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId);
            var invalid = ResultValidators.OnMoving(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: resultId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var targetSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            var error = resultModel.Move(
                context: context,
                ss: ss,
                targetSs: targetSs);
            switch (error)
            {
                case Error.Types.None:
                    if (ss.SwitchRecordWithAjax == true)
                    {
                        return EditorResponse(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            message: Messages.Moved(context: context),
                            switchTargets: GetSwitchTargets(
                                context: context,
                                ss: ss,
                                resultId: resultModel.ResultId,
                                siteId: resultModel.SiteId).Join())
                                    .ToJson();
                    }
                    else
                    {
                        SessionUtilities.Set(
                            context: context,
                            message: Messages.Moved(
                                context: context,
                                data: resultModel.Title.DisplayValue));
                        return new ResponseCollection()
                            .Response("id", resultModel.ResultId.ToString())
                            .Href(Locations.ItemEdit(
                                context: context,
                                id: resultModel.ResultId))
                            .ToJson();
                    }
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        context: context,
                        data: targetSs.GetColumn(
                            context: context,
                            columnName: targetSs.DuplicatedColumn)?.LabelText)
                                .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Delete(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(context, ss, resultId);
            var invalid = ResultValidators.OnDeleting(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = resultModel.Delete(context: context, ss: ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: resultModel.Title.Value));
                    var res = new ResultsResponseCollection(resultModel);
                    res
                        .SetMemory("formChanged", false)
                        .Href(Locations.Get(
                            context: context,
                            parts: new string[]
                            {
                                "Items",
                                ss.SiteId.ToString(),
                                ViewModes.GetSessionData(
                                    context: context,
                                    siteId: ss.SiteId)
                            }));
                    return res.ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static System.Web.Mvc.ContentResult DeleteByApi(
            Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: resultId,
                methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = ResultValidators.OnDeleting(
                context: context,
                ss: ss,
                resultModel: resultModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(context: context, ss: ss);
            var error = resultModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        resultModel.ResultId,
                        Displays.Deleted(
                            context: context,
                            data: resultModel.Title.DisplayValue));
                default:
                    return ApiResults.Error(
                        context: context,
                        type: error);
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
                var selector = new GridSelector(context: context);
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

        public static int Restore(
            Context context, SiteSettings ss, List<long> selected, bool negative = false)
        {
            var where = Rds.ResultsWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Results_Deleted")
                .ResultId_In(
                    value: selected,
                    tableName: "Results_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .ResultId_In(
                    tableName: "Results_Deleted",
                    sub: Rds.SelectResults(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.ResultsColumn().ResultId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(context: context, ss: ss)));
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(where: Rds.ItemsWhere().ReferenceId_In(sub:
                        Rds.SelectResults(
                            tableType: Sqls.TableTypes.Deleted,
                            _as: "Results_Deleted",
                            column: Rds.ResultsColumn()
                                .ResultId(tableName: "Results_Deleted"),
                            where: where))),
                    Rds.RestoreResults(where: where, countRecord: true)
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long resultId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var resultModel = new ResultModel(context, ss, resultId);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
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
            resultModel.SetByModel(new ResultModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId(resultId)
                    .Ver(ver.First())));
            resultModel.VerUp = true;
            var error = resultModel.Update(
                context: context,
                ss: ss,
                otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.RestoredFromHistory(
                            context: context,
                            data: ver.First().ToString()));
                    return  new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: resultId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long resultId, Message message = null)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
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
                                columns: columns,
                                sort: false,
                                checkRow: true))
                        .TBody(action: () => hb
                            .HistoriesTableBody(
                                context: context,
                                ss: ss,
                                columns: columns,
                                resultModel: resultModel)));
            return new ResultsResponseCollection(resultModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            ResultModel resultModel)
        {
            new ResultCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.ResultsWhere().ResultId(resultModel.ResultId),
                orderBy: Rds.ResultsOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(resultModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(resultModelHistory.Ver)
                                .DataLatest(1, _using:
                                    resultModelHistory.Ver == resultModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: resultModelHistory.Ver.ToString(),
                                            _using: resultModelHistory.Ver < resultModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            resultModel: resultModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.ResultsColumnCollection()
                .ResultId()
                .Ver();
            columns.ForEach(column => sqlColumn.ResultsColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(context: context, ss: ss, resultId: resultId);
            ss.SetColumnAccessControls(
                context: context,
                mine: resultModel.Mine(context: context));
            resultModel.Get(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .ResultId(resultModel.ResultId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            resultModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, resultModel).ToJson();
        }

        public static string BulkMove(Context context, SiteSettings ss)
        {
            var siteId = context.Forms.Long("MoveTargets");
            var selector = new GridSelector(context: context);
            var count = BulkMoveCount(
                context: context,
                ss: ss,
                siteId: siteId,
                selector: selector);
            if (context.ContractSettings.ItemsLimit(context: context, siteId: siteId, number: count))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            if (Permissions.CanMove(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: siteId)))
            {
                if (selector.All)
                {
                    count = BulkMove(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        selector: selector);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = BulkMove(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            selector: selector);
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
                    message: Messages.BulkMoved(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int BulkMoveCount(
            Context context,
            SiteSettings ss,
            long siteId,
            GridSelector selector)
        {
            return Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: ss.Join(context: context),
                    where: Views.GetBySession(context: context, ss: ss).Where(
                        context: context,
                        ss: ss,
                        where: Rds.ResultsWhere()
                            .SiteId(ss.SiteId)
                            .ResultId_In(
                                value: selector.Selected,
                                negative: selector.All,
                                _using: selector.Selected.Any()))));
        }

        private static int BulkMove(
            Context context,
            SiteSettings ss,
            long siteId,
            GridSelector selector)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateResults(
                        where: Views.GetBySession(context: context, ss: ss).Where(
                            context: context,
                            ss: ss,
                            where: Rds.ResultsWhere()
                                .SiteId(ss.SiteId)
                                .ResultId_In(
                                    value: selector.Selected,
                                    negative: selector.All,
                                    _using: selector.Selected.Any())),
                        param: Rds.ResultsParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: Rds.ResultsWhere().SiteId(siteId)))
                            .SiteId(siteId, _operator: "<>"),
                        param: Rds.ItemsParam().SiteId(siteId))
                }).Count.ToInt();
        }

        public static string BulkDelete(Context context, SiteSettings ss)
        {
            if (context.CanDelete(ss: ss))
            {
                var selector = new GridSelector(context: context);
                var count = 0;
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
                    if (selector.Selected.Any())
                    {
                        count = BulkDelete(
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
                    message: Messages.BulkDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int BulkDelete(
            Context context,
            SiteSettings ss,
            IEnumerable<long> selected,
            bool negative = false)
        {
            var where = Views.GetBySession(context: context, ss: ss).Where(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(
                        value: selected,
                        negative: negative,
                        _using: selected.Any()));
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                where: where);
            var statements = new List<SqlStatement>();
            statements.OnBulkDeletingExtendedSqls(ss.SiteId);
            statements.Add(Rds.DeleteItems(
                where: Rds.ItemsWhere()
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteBinaries(
                where: Rds.BinariesWhere()
                    .TenantId(context.TenantId)
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteResults(
                where: where, 
                countRecord: true));
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long resultId)
        {
            if (!Parameters.History.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector(context: context);
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
                        resultId: resultId,
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
                            resultId: resultId,
                            selected: selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets(context: context).ToJson();
                    }
                }
                return Histories(
                    context: context,
                    ss: ss,
                    resultId: resultId,
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
            long resultId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteResults(
                    tableType: Sqls.TableTypes.History,
                    where: Rds.ResultsWhere()
                        .SiteId(
                            value: ss.SiteId,
                            tableName: "Results_History")
                        .ResultId(
                            value: resultId,
                            tableName: "Results_History")
                        .Ver_In(
                            value: selected,
                            tableName: "Results_History",
                            negative: negative,
                            _using: selected.Any())
                        .ResultId_In(
                            tableName: "Results_History",
                            sub: Rds.SelectResults(
                                tableType: Sqls.TableTypes.History,
                                column: Rds.ResultsColumn().ResultId(),
                                where: Views.GetBySession(context: context, ss: ss).Where(
                                    context: context, ss: ss))),
                    countRecord: true)).Count.ToInt();
        }

        public static string PhysicalDelete(Context context, SiteSettings ss)
        {
            if (!Parameters.Deleted.PhysicalDelete)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            if (context.CanManageSite(ss: ss))
            {
                var selector = new GridSelector(context: context);
                var count = 0;
                if (selector.All)
                {
                    count = PhysicalDelete(
                        context: context,
                        ss: ss,
                        selected: selector.Selected,
                        negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = PhysicalDelete(
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
                    message: Messages.PhysicalDeleted(
                        context: context,
                        data: count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
        }

        private static int PhysicalDelete(
            Context context,
            SiteSettings ss,
            List<long> selected,
            bool negative = false)
        {
            var where = Rds.ResultsWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Results_Deleted")
                .ResultId_In(
                    value: selected,
                    tableName: "Results_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .ResultId_In(
                    tableName: "Results_Deleted",
                    sub: Rds.SelectResults(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.ResultsColumn().ResultId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(
                            context: context, ss: ss)));
            var sub = Rds.SelectResults(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Results_Deleted",
                column: Rds.ResultsColumn()
                    .ResultId(tableName: "Results_Deleted"),
                where: where);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.PhysicalDeleteItems(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteBinaries(
                        tableType: Sqls.TableTypes.Deleted,
                        where: Rds.ItemsWhere().ReferenceId_In(sub: sub)),
                    Rds.PhysicalDeleteResults(
                        tableType: Sqls.TableTypes.Deleted,
                        where: where,
                        countRecord: true)
                }).Count.ToInt();
        }

        public static string Import(Context context, SiteModel siteModel)
        {
            var ss = siteModel.ResultsSiteSettings(
                context: context,
                referenceId: siteModel.SiteId,
                setAllChoices: true);
            if (context.ContractSettings.Import == false)
            {
                return Messages.ResponseRestricted(context: context).ToJson();
            }
            if (!context.CanCreate(ss: ss))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
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
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column?.ColumnName == "ResultId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalid = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.ColumnName));
                if (invalid != null) return invalid;
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var resultHash = new Dictionary<int, ResultModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var resultModel = new ResultModel() { SiteId = ss.SiteId };
                    if (context.Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
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
                            case "Title":
                                resultModel.Title.Value = recordingData.ToString();
                                break;
                            case "Body":
                                resultModel.Body = recordingData.ToString();
                                break;
                            case "Status":
                                resultModel.Status.Value = recordingData.ToInt();
                                break;
                            case "ClassA":
                                resultModel.ClassA = recordingData.ToString();
                                break;
                            case "ClassB":
                                resultModel.ClassB = recordingData.ToString();
                                break;
                            case "ClassC":
                                resultModel.ClassC = recordingData.ToString();
                                break;
                            case "ClassD":
                                resultModel.ClassD = recordingData.ToString();
                                break;
                            case "ClassE":
                                resultModel.ClassE = recordingData.ToString();
                                break;
                            case "ClassF":
                                resultModel.ClassF = recordingData.ToString();
                                break;
                            case "ClassG":
                                resultModel.ClassG = recordingData.ToString();
                                break;
                            case "ClassH":
                                resultModel.ClassH = recordingData.ToString();
                                break;
                            case "ClassI":
                                resultModel.ClassI = recordingData.ToString();
                                break;
                            case "ClassJ":
                                resultModel.ClassJ = recordingData.ToString();
                                break;
                            case "ClassK":
                                resultModel.ClassK = recordingData.ToString();
                                break;
                            case "ClassL":
                                resultModel.ClassL = recordingData.ToString();
                                break;
                            case "ClassM":
                                resultModel.ClassM = recordingData.ToString();
                                break;
                            case "ClassN":
                                resultModel.ClassN = recordingData.ToString();
                                break;
                            case "ClassO":
                                resultModel.ClassO = recordingData.ToString();
                                break;
                            case "ClassP":
                                resultModel.ClassP = recordingData.ToString();
                                break;
                            case "ClassQ":
                                resultModel.ClassQ = recordingData.ToString();
                                break;
                            case "ClassR":
                                resultModel.ClassR = recordingData.ToString();
                                break;
                            case "ClassS":
                                resultModel.ClassS = recordingData.ToString();
                                break;
                            case "ClassT":
                                resultModel.ClassT = recordingData.ToString();
                                break;
                            case "ClassU":
                                resultModel.ClassU = recordingData.ToString();
                                break;
                            case "ClassV":
                                resultModel.ClassV = recordingData.ToString();
                                break;
                            case "ClassW":
                                resultModel.ClassW = recordingData.ToString();
                                break;
                            case "ClassX":
                                resultModel.ClassX = recordingData.ToString();
                                break;
                            case "ClassY":
                                resultModel.ClassY = recordingData.ToString();
                                break;
                            case "ClassZ":
                                resultModel.ClassZ = recordingData.ToString();
                                break;
                            case "NumA":
                                resultModel.NumA = recordingData.ToDecimal();
                                break;
                            case "NumB":
                                resultModel.NumB = recordingData.ToDecimal();
                                break;
                            case "NumC":
                                resultModel.NumC = recordingData.ToDecimal();
                                break;
                            case "NumD":
                                resultModel.NumD = recordingData.ToDecimal();
                                break;
                            case "NumE":
                                resultModel.NumE = recordingData.ToDecimal();
                                break;
                            case "NumF":
                                resultModel.NumF = recordingData.ToDecimal();
                                break;
                            case "NumG":
                                resultModel.NumG = recordingData.ToDecimal();
                                break;
                            case "NumH":
                                resultModel.NumH = recordingData.ToDecimal();
                                break;
                            case "NumI":
                                resultModel.NumI = recordingData.ToDecimal();
                                break;
                            case "NumJ":
                                resultModel.NumJ = recordingData.ToDecimal();
                                break;
                            case "NumK":
                                resultModel.NumK = recordingData.ToDecimal();
                                break;
                            case "NumL":
                                resultModel.NumL = recordingData.ToDecimal();
                                break;
                            case "NumM":
                                resultModel.NumM = recordingData.ToDecimal();
                                break;
                            case "NumN":
                                resultModel.NumN = recordingData.ToDecimal();
                                break;
                            case "NumO":
                                resultModel.NumO = recordingData.ToDecimal();
                                break;
                            case "NumP":
                                resultModel.NumP = recordingData.ToDecimal();
                                break;
                            case "NumQ":
                                resultModel.NumQ = recordingData.ToDecimal();
                                break;
                            case "NumR":
                                resultModel.NumR = recordingData.ToDecimal();
                                break;
                            case "NumS":
                                resultModel.NumS = recordingData.ToDecimal();
                                break;
                            case "NumT":
                                resultModel.NumT = recordingData.ToDecimal();
                                break;
                            case "NumU":
                                resultModel.NumU = recordingData.ToDecimal();
                                break;
                            case "NumV":
                                resultModel.NumV = recordingData.ToDecimal();
                                break;
                            case "NumW":
                                resultModel.NumW = recordingData.ToDecimal();
                                break;
                            case "NumX":
                                resultModel.NumX = recordingData.ToDecimal();
                                break;
                            case "NumY":
                                resultModel.NumY = recordingData.ToDecimal();
                                break;
                            case "NumZ":
                                resultModel.NumZ = recordingData.ToDecimal();
                                break;
                            case "DateA":
                                resultModel.DateA = recordingData.ToDateTime();
                                break;
                            case "DateB":
                                resultModel.DateB = recordingData.ToDateTime();
                                break;
                            case "DateC":
                                resultModel.DateC = recordingData.ToDateTime();
                                break;
                            case "DateD":
                                resultModel.DateD = recordingData.ToDateTime();
                                break;
                            case "DateE":
                                resultModel.DateE = recordingData.ToDateTime();
                                break;
                            case "DateF":
                                resultModel.DateF = recordingData.ToDateTime();
                                break;
                            case "DateG":
                                resultModel.DateG = recordingData.ToDateTime();
                                break;
                            case "DateH":
                                resultModel.DateH = recordingData.ToDateTime();
                                break;
                            case "DateI":
                                resultModel.DateI = recordingData.ToDateTime();
                                break;
                            case "DateJ":
                                resultModel.DateJ = recordingData.ToDateTime();
                                break;
                            case "DateK":
                                resultModel.DateK = recordingData.ToDateTime();
                                break;
                            case "DateL":
                                resultModel.DateL = recordingData.ToDateTime();
                                break;
                            case "DateM":
                                resultModel.DateM = recordingData.ToDateTime();
                                break;
                            case "DateN":
                                resultModel.DateN = recordingData.ToDateTime();
                                break;
                            case "DateO":
                                resultModel.DateO = recordingData.ToDateTime();
                                break;
                            case "DateP":
                                resultModel.DateP = recordingData.ToDateTime();
                                break;
                            case "DateQ":
                                resultModel.DateQ = recordingData.ToDateTime();
                                break;
                            case "DateR":
                                resultModel.DateR = recordingData.ToDateTime();
                                break;
                            case "DateS":
                                resultModel.DateS = recordingData.ToDateTime();
                                break;
                            case "DateT":
                                resultModel.DateT = recordingData.ToDateTime();
                                break;
                            case "DateU":
                                resultModel.DateU = recordingData.ToDateTime();
                                break;
                            case "DateV":
                                resultModel.DateV = recordingData.ToDateTime();
                                break;
                            case "DateW":
                                resultModel.DateW = recordingData.ToDateTime();
                                break;
                            case "DateX":
                                resultModel.DateX = recordingData.ToDateTime();
                                break;
                            case "DateY":
                                resultModel.DateY = recordingData.ToDateTime();
                                break;
                            case "DateZ":
                                resultModel.DateZ = recordingData.ToDateTime();
                                break;
                            case "DescriptionA":
                                resultModel.DescriptionA = recordingData.ToString();
                                break;
                            case "DescriptionB":
                                resultModel.DescriptionB = recordingData.ToString();
                                break;
                            case "DescriptionC":
                                resultModel.DescriptionC = recordingData.ToString();
                                break;
                            case "DescriptionD":
                                resultModel.DescriptionD = recordingData.ToString();
                                break;
                            case "DescriptionE":
                                resultModel.DescriptionE = recordingData.ToString();
                                break;
                            case "DescriptionF":
                                resultModel.DescriptionF = recordingData.ToString();
                                break;
                            case "DescriptionG":
                                resultModel.DescriptionG = recordingData.ToString();
                                break;
                            case "DescriptionH":
                                resultModel.DescriptionH = recordingData.ToString();
                                break;
                            case "DescriptionI":
                                resultModel.DescriptionI = recordingData.ToString();
                                break;
                            case "DescriptionJ":
                                resultModel.DescriptionJ = recordingData.ToString();
                                break;
                            case "DescriptionK":
                                resultModel.DescriptionK = recordingData.ToString();
                                break;
                            case "DescriptionL":
                                resultModel.DescriptionL = recordingData.ToString();
                                break;
                            case "DescriptionM":
                                resultModel.DescriptionM = recordingData.ToString();
                                break;
                            case "DescriptionN":
                                resultModel.DescriptionN = recordingData.ToString();
                                break;
                            case "DescriptionO":
                                resultModel.DescriptionO = recordingData.ToString();
                                break;
                            case "DescriptionP":
                                resultModel.DescriptionP = recordingData.ToString();
                                break;
                            case "DescriptionQ":
                                resultModel.DescriptionQ = recordingData.ToString();
                                break;
                            case "DescriptionR":
                                resultModel.DescriptionR = recordingData.ToString();
                                break;
                            case "DescriptionS":
                                resultModel.DescriptionS = recordingData.ToString();
                                break;
                            case "DescriptionT":
                                resultModel.DescriptionT = recordingData.ToString();
                                break;
                            case "DescriptionU":
                                resultModel.DescriptionU = recordingData.ToString();
                                break;
                            case "DescriptionV":
                                resultModel.DescriptionV = recordingData.ToString();
                                break;
                            case "DescriptionW":
                                resultModel.DescriptionW = recordingData.ToString();
                                break;
                            case "DescriptionX":
                                resultModel.DescriptionX = recordingData.ToString();
                                break;
                            case "DescriptionY":
                                resultModel.DescriptionY = recordingData.ToString();
                                break;
                            case "DescriptionZ":
                                resultModel.DescriptionZ = recordingData.ToString();
                                break;
                            case "CheckA":
                                resultModel.CheckA = recordingData.ToBool();
                                break;
                            case "CheckB":
                                resultModel.CheckB = recordingData.ToBool();
                                break;
                            case "CheckC":
                                resultModel.CheckC = recordingData.ToBool();
                                break;
                            case "CheckD":
                                resultModel.CheckD = recordingData.ToBool();
                                break;
                            case "CheckE":
                                resultModel.CheckE = recordingData.ToBool();
                                break;
                            case "CheckF":
                                resultModel.CheckF = recordingData.ToBool();
                                break;
                            case "CheckG":
                                resultModel.CheckG = recordingData.ToBool();
                                break;
                            case "CheckH":
                                resultModel.CheckH = recordingData.ToBool();
                                break;
                            case "CheckI":
                                resultModel.CheckI = recordingData.ToBool();
                                break;
                            case "CheckJ":
                                resultModel.CheckJ = recordingData.ToBool();
                                break;
                            case "CheckK":
                                resultModel.CheckK = recordingData.ToBool();
                                break;
                            case "CheckL":
                                resultModel.CheckL = recordingData.ToBool();
                                break;
                            case "CheckM":
                                resultModel.CheckM = recordingData.ToBool();
                                break;
                            case "CheckN":
                                resultModel.CheckN = recordingData.ToBool();
                                break;
                            case "CheckO":
                                resultModel.CheckO = recordingData.ToBool();
                                break;
                            case "CheckP":
                                resultModel.CheckP = recordingData.ToBool();
                                break;
                            case "CheckQ":
                                resultModel.CheckQ = recordingData.ToBool();
                                break;
                            case "CheckR":
                                resultModel.CheckR = recordingData.ToBool();
                                break;
                            case "CheckS":
                                resultModel.CheckS = recordingData.ToBool();
                                break;
                            case "CheckT":
                                resultModel.CheckT = recordingData.ToBool();
                                break;
                            case "CheckU":
                                resultModel.CheckU = recordingData.ToBool();
                                break;
                            case "CheckV":
                                resultModel.CheckV = recordingData.ToBool();
                                break;
                            case "CheckW":
                                resultModel.CheckW = recordingData.ToBool();
                                break;
                            case "CheckX":
                                resultModel.CheckX = recordingData.ToBool();
                                break;
                            case "CheckY":
                                resultModel.CheckY = recordingData.ToBool();
                                break;
                            case "CheckZ":
                                resultModel.CheckZ = recordingData.ToBool();
                                break;
                            case "Manager":
                                resultModel.Manager = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Owner":
                                resultModel.Owner = SiteInfo.User(
                                    context: context,
                                    userId: recordingData.ToInt());
                                break;
                            case "Comments":
                                if (resultModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    resultModel.Comments.Prepend(
                                        context: context,
                                        ss: ss,
                                        body: data.Row[column.Key]);
                                }
                                break;
                        }
                    });
                    resultHash.Add(data.Index, resultModel);
                });
                var insertCount = 0;
                var updateCount = 0;
                foreach (var resultModel in resultHash.Values)
                {
                    resultModel.SetByFormula(context: context, ss: ss);
                    resultModel.SetTitle(context: context, ss: ss);
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        resultModel.VerUp = Versions.MustVerUp(
                            context: context, baseModel: resultModel);
                        if (resultModel.Updated(context: context))
                        {
                            var error = resultModel.Update(
                                context: context,
                                ss: ss,
                                extendedSqls: false,
                                get: false);
                            switch (error)
                            {
                                case Error.Types.None:
                                    break;
                                case Error.Types.Duplicated:
                                    return Messages.ResponseDuplicated(
                                        context: context,
                                        data: ss.GetColumn(
                                            context: context,
                                            columnName: ss.DuplicatedColumn)?.LabelText)
                                                .ToJson();
                                default:
                                    return error.MessageJson(context: context);
                            }
                            updateCount++;
                        }
                    }
                    else
                    {
                        var error = resultModel.Create(
                            context: context,
                            ss: ss,
                            extendedSqls: false,
                            get: false);
                        switch (error)
                        {
                            case Error.Types.None:
                                break;
                            case Error.Types.Duplicated:
                                return Messages.ResponseDuplicated(
                                    context: context,
                                    data: ss.GetColumn(
                                        context: context,
                                        columnName: ss.DuplicatedColumn)?.LabelText)
                                            .ToJson();
                            default:
                                return error.MessageJson(context: context);
                        }
                        insertCount++;
                    }
                }
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportedExtendedSqls(ss.SiteId).ToArray());
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

        private static string ImportRecordingData(
            Context context, Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(
                context: context,
                value: value,
                siteId: inheritPermission);
            return recordingData;
        }

        public static string OpenExportSelectorDialog(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return HtmlTemplates.Error(context, Error.Types.InvalidRequest);
            }
            var invalid = ResultValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid)
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

        public static ResponseFile Export(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return null;
            }
            var invalid = ResultValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return ExportUtilities.Csv(
                context: context,
                ss: ss,
                export: ss.GetExport(
                    context: context,
                    id: context.QueryStrings.Int("id")));
        }

        public static ResponseFile ExportCrosstab(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false) return null;
            var invalid = ResultValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context, columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "ResultId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            return new ResponseFile(
                Libraries.ViewModes.CrosstabUtilities.Csv(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    columns: columns,
                    aggregateType: aggregateType,
                    value: value,
                    timePeriod: timePeriod,
                    month: month,
                    dataRows: dataRows),
                ExportUtilities.FileName(
                    context: context,
                    ss: ss,
                    name: Displays.Crosstab(context: context)));
        }

        public static string Calendar(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Calendar"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(
                context: context,
                date: month);
            var end = Calendars.EndDate(
                context: context,
                date: month);
            var dataRows = CalendarDataRows(
                context: context,
                ss: ss,
                view: view,
                fromColumn: fromColumn,
                toColumn: toColumn,
                begin: Calendars.BeginDate(
                    context: context,
                    date: month),
                end: Calendars.EndDate(
                    context: context,
                    date: month));
            var inRange = dataRows.Count() <= Parameters.General.CalendarLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.CalendarLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Calendar(
                        context: context,
                        ss: ss,
                        fromColumn: fromColumn,
                        toColumn: toColumn,
                        month: month,
                        begin: begin,
                        dataRows: dataRows,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string UpdateByCalendar(Context context, SiteSettings ss)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: context.Forms.Long("Id"),
                setByForm: true);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: resultModel);
            resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return CalendarJson(
                context: context,
                ss: ss,
                changedItemId: resultModel.ResultId,
                update: true,
                message: Messages.Updated(
                    context: context,
                    data: resultModel.Title.DisplayValue));
        }

        public static string CalendarJson(
            Context context,
            SiteSettings ss,
            long changedItemId = 0,
            bool update = false,
            Message message = null)
        {
            if (!ss.EnableViewMode(context: context, name: "Calendar"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Calendar");
            var fromColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(
                context: context,
                columnName: view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(
                context: context,
                date: month);
            var end = Calendars.EndDate(
                context: context,
                date: month);
            var dataRows = CalendarDataRows(
                context: context,
                ss: ss,
                view: view,
                fromColumn: fromColumn,
                toColumn: toColumn,
                begin: Calendars.BeginDate(
                    context: context,
                    date: month),
                end: Calendars.EndDate(
                    context: context,
                    date: month));
            return dataRows.Count() <= Parameters.General.CalendarLimit
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "setCalendar",
                        message: message,
                        loadScroll: update,
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
                                context: context,
                                ss: ss,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                month: month,
                                begin: begin,
                                dataRows: dataRows,
                                bodyOnly: bodyOnly,
                                inRange: true,
                                changedItemId: changedItemId))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.CalendarLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
                                context: context,
                                ss: ss,
                                fromColumn: fromColumn,
                                toColumn: toColumn,
                                month: month,
                                begin: begin,
                                dataRows: dataRows,
                                bodyOnly: bodyOnly,
                                inRange: false,
                                changedItemId: changedItemId))
                    .ToJson();
        }

        private static EnumerableRowCollection<DataRow> CalendarDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column fromColumn,
            Column toColumn,
            DateTime begin,
            DateTime end)
        {
            var where = Rds.ResultsWhere();
            if (toColumn == null)
            {
                where.Add(
                    raw: $"[Results].[{fromColumn.ColumnName}] between '{begin}' and '{end}'");
            }
            else
            {
                where.Or(or: Rds.ResultsWhere()
                    .Add(raw: $"[Results].[{fromColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Results].[{toColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Results].[{fromColumn.ColumnName}]<='{begin}' and [Results].[{toColumn.ColumnName}]>='{end}'"));
            }
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsTitleColumn(context: context, ss: ss)
                        .ResultId(_as: "Id")
                        .ResultsColumn(fromColumn.ColumnName, _as: "From")
                        .ResultsColumn(toColumn?.ColumnName, _as: "To")
                        .UpdatedTime()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType)),
                    join: ss.Join(context: context),
                    where: view.Where(context: context, ss: ss, where: where)))
                        .AsEnumerable();
        }

        private static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column fromColumn,
            Column toColumn,
            DateTime month,
            DateTime begin,
            EnumerableRowCollection<DataRow> dataRows,
            bool bodyOnly,
            bool inRange,
            long changedItemId = 0)
        {
            return !bodyOnly
                ? hb.Calendar(
                    context: context,
                    ss: ss,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId)
                : hb.CalendarBody(
                    context: context,
                    ss: ss,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId);
        }

        public static string Crosstab(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Crosstab"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "ResultId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(
                context: context,
                dataRows: dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns"
                    || Libraries.ViewModes.CrosstabUtilities.InRangeY(
                        context: context,
                        dataRows: dataRows);
            if (!inRangeX)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyColumnCases(
                        context: context,
                        data: Parameters.General.CrosstabXLimit.ToString()));
            }
            else if (!inRangeY)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyColumnCases(
                        context: context,
                        data: Parameters.General.CrosstabYLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Crosstab(
                        context: context,
                        ss: ss,
                        view: view,
                        groupByX: groupByX,
                        groupByY: groupByY,
                        columns: columns,
                        aggregateType: aggregateType,
                        value: value,
                        timePeriod: timePeriod,
                        month: month,
                        dataRows: dataRows,
                        inRange: inRangeX && inRangeY));
        }

        public static string CrosstabJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Crosstab"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByX(context: context, ss: ss));
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabGroupByY(context: context, ss: ss));
            var columns = CrosstabColumns(context: context, ss: ss, view: view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn(context: context, columnName: "ResultId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                columns: columns,
                value: value,
                aggregateType: aggregateType,
                timePeriod: timePeriod,
                month: month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(
                context: context,
                dataRows: dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns"
                    || Libraries.ViewModes.CrosstabUtilities.InRangeY(
                        context: context,
                        dataRows: dataRows);
            var bodyOnly = context.Forms.ControlId().StartsWith("Crosstab");
            return inRangeX && inRangeY
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        invoke: "setCrosstab",
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows)
                            : new HtmlBuilder().CrosstabBody(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        message: !inRangeX
                            ? Messages.TooManyColumnCases(
                                context: context,
                                data: Parameters.General.CrosstabXLimit.ToString())
                            : Messages.TooManyRowCases(
                                context: context,
                                data: Parameters.General.CrosstabYLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows,
                                inRange: false)
                            : new HtmlBuilder())
                    .ToJson();
        }

        private static List<Column> CrosstabColumns(
            Context context, SiteSettings ss, View view)
        {
            return Libraries.ViewModes.CrosstabUtilities.GetColumns(
                context: context,
                ss: ss,
                columns: view.CrosstabColumns?.Deserialize<IEnumerable<string>>()?
                    .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                    .ToList());
        }

        private static EnumerableRowCollection<DataRow> CrosstabDataRows(
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            List<Column> columns,
            Column value,
            string aggregateType,
            string timePeriod,
            DateTime month)
        {
            EnumerableRowCollection<DataRow> dataRows;
            var join = ss.Join(
                context: context,
                columns: Libraries.ViewModes.CrosstabUtilities
                    .JoinColumns(view, groupByX, groupByY, columns, value));
            if (groupByX?.TypeName != "datetime")
            {
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn()
                            .Add(ss, groupByX)
                            .CrosstabColumns(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByY: groupByY,
                                columns: columns,
                                value: value,
                                aggregateType: aggregateType),
                        join: join,
                        where: view.Where(context: context, ss: ss),
                        groupBy: Rds.ResultsGroupBy()
                            .Add(ss, groupByX)
                            .Add(ss, groupByY)))
                                .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    context: context,
                    ss: ss,
                    column: groupByX,
                    timePeriod: timePeriod);
                dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
                        column: Rds.ResultsColumn()
                            .Add(dateGroup, _as: groupByX.ColumnName)
                            .CrosstabColumns(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByY: groupByY,
                                columns: columns,
                                value: value,
                                aggregateType: aggregateType),
                        join: join,
                        where: view.Where(
                            context: context,
                            ss: ss,
                            where: Libraries.ViewModes.CrosstabUtilities.Where(
                                context: context,
                                ss: ss,
                                column: groupByX,
                                timePeriod: timePeriod,
                                month: month)),
                        groupBy: Rds.ResultsGroupBy()
                            .Add(dateGroup)
                            .Add(ss, groupByY)))
                                .AsEnumerable();
            }
            ss.SetChoiceHash(dataRows);
            return dataRows;
        }

        private static SqlColumnCollection CrosstabColumns(
            this SqlColumnCollection self,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByY,
            List<Column> columns,
            Column value,
            string aggregateType)
        {
            if (view.CrosstabGroupByY != "Columns")
            {
                return self
                    .Add(
                        ss: ss,
                        column: groupByY)
                    .Add(
                        ss: ss,
                        column: value,
                        _as: "Value",
                        function: Sqls.Function(aggregateType));
            }
            else
            {
                columns.ForEach(column =>
                    self.Add(
                        ss: ss,
                        column: column,
                        _as: column.ColumnName,
                        function: Sqls.Function(aggregateType)));
                return self;
            }
        }

        public static string TimeSeries(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "TimeSeries"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.TimeSeriesLimit);
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.TimeSeriesLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TimeSeries(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string TimeSeriesJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "TimeSeries"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("TimeSeries");
            return InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.TimeSeriesLimit)
                    ? new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "drawTimeSeries",
                            bodyOnly: bodyOnly,
                            bodySelector: "#TimeSeriesBody",
                            body: new HtmlBuilder()
                                .TimeSeries(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: true))
                        .ToJson()
                    : new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.TimeSeriesLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: "#TimeSeriesBody",
                            body: new HtmlBuilder()
                                .TimeSeries(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: false))
                        .ToJson();
        }

        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            bool inRange)
        {
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesGroupBy(ss));
            var aggregationType = view.GetTimeSeriesAggregationType(
                context: context,
                ss: ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetTimeSeriesValue(ss));
            var dataRows = TimeSeriesDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                value: value);
            if (groupBy == null)
            {
                return hb;
            }
            return !bodyOnly
                ? hb.TimeSeries(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.TimeSeriesBody(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    aggregationType: aggregationType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            Context context, SiteSettings ss, View view, Column groupBy, Column value)
        {
            if (groupBy != null && value != null)
            {
                var dataRows = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectResults(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.ResultsColumn()
                            .ResultId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .Add(ss: ss, column: groupBy)
                            .Add(ss: ss, column: value),
                        join: ss.Join(context: context),
                        where: view.Where(context: context, ss: ss)))
                            .AsEnumerable();
                ss.SetChoiceHash(dataRows);
                return dataRows;
            }
            else
            {
                return null;
            }
        }

        public static string Kamban(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Kamban"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.KambanLimit);
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.KambanLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Kamban(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string KambanJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Kamban"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("Kamban");
            return InRange(
                context: context,
                ss: ss,
                view: view,
                limit: Parameters.General.KambanLimit)
                    ? new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            invoke: "setKamban",
                            bodyOnly: bodyOnly,
                            bodySelector: "#KambanBody",
                            body: new HtmlBuilder()
                                .Kamban(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    changedItemId: context.Forms.Long("KambanId")))
                        .ToJson()
                    : new ResponseCollection()
                        .ViewMode(
                            context: context,
                            ss: ss,
                            view: view,
                            message: Messages.TooManyCases(
                                context: context,
                                data: Parameters.General.KambanLimit.ToString()),
                            bodyOnly: bodyOnly,
                            bodySelector: "#KambanBody",
                            body: new HtmlBuilder()
                                .Kamban(
                                    context: context,
                                    ss: ss,
                                    view: view,
                                    bodyOnly: bodyOnly,
                                    inRange: false))
                        .ToJson();
        }

        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            long changedItemId = 0,
            bool inRange = true)
        {
            var groupByX = ss.GetColumn(
                context: context,
                columnName: view.GetKambanGroupByX(
                    context: context,
                    ss: ss));
            if (groupByX == null)
            {
                return hb;
            }
            var groupByY = ss.GetColumn(
                context: context,
                columnName: view.GetKambanGroupByY(
                    context: context,
                    ss: ss));
            var aggregateType = view.GetKambanAggregationType(
                context: context,
                ss: ss);
            var value = ss.GetColumn(
                context: context,
                columnName: view.GetKambanValue(ss));
            var aggregationView = view.KambanAggregationView ?? false;
            var data = KambanData(
                context: context,
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                value: value);
            return !bodyOnly
                ? hb.Kamban(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    columns: view.KambanColumns,
                    aggregationView: aggregationView,
                    data: data,
                    inRange: inRange)
                : hb.KambanBody(
                    context: context,
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    columns: view.KambanColumns,
                    aggregationView: aggregationView,
                    data: data,
                    changedItemId: changedItemId,
                    inRange: inRange);
        }

        private static IEnumerable<Libraries.ViewModes.KambanElement> KambanData(
            Context context, 
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            Column value)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn()
                        .ResultId()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType))
                        .Add(ss: ss, column: groupByX)
                        .Add(ss: ss, column: groupByY)
                        .Add(ss: ss, column: value),
                    where: view.Where(context: context, ss: ss)))
                        .AsEnumerable()
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Long("ResultId"),
                            Title = o.String("ItemTitle"),
                            GroupX = groupByX.ConvertIfUserColumn(o),
                            GroupY = groupByY?.ConvertIfUserColumn(o),
                            Value = o.Decimal(value.ColumnName)
                        });
        }

        public static string UpdateByKamban(Context context, SiteSettings ss)
        {
            var resultModel = new ResultModel(
                context: context,
                ss: ss,
                resultId: context.Forms.Long("KambanId"),
                setByForm: true);
            var invalid = ResultValidators.OnUpdating(
                context: context,
                ss: ss,
                resultModel: resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: resultModel);
            resultModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return KambanJson(context: context, ss: ss);
        }

        public static string ImageLib(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "ImageLib"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .ImageLib(
                        context: context,
                        ss: ss,
                        view: view,
                        bodyOnly: false));
        }

        public static string ImageLibJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "ImageLib"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var bodyOnly = context.Forms.ControlId().StartsWith("ImageLib");
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    invoke: "setImageLib",
                    bodyOnly: bodyOnly,
                    bodySelector: "#ImageLibBody",
                    body: new HtmlBuilder()
                        .ImageLib(
                            context: context,
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly))
                .ToJson();
        }

        private static HtmlBuilder ImageLib(
            this HtmlBuilder hb,
            Context context, 
            SiteSettings ss,
            View view,
            bool bodyOnly,
            int offset = 0)
        {
            return !bodyOnly
                ? hb.ImageLib(
                    ss: ss,
                    context: context,
                    imageLibData: new ImageLibData(
                        context: context,
                        ss: ss,
                        view: view,
                        offset: offset,
                        pageSize: ss.ImageLibPageSize.ToInt()))
                : hb.ImageLibBody(
                    ss: ss,
                    context: context,
                    imageLibData: new ImageLibData(
                        context: context,
                        ss: ss,
                        view: view,
                        offset: offset,
                        pageSize: ss.ImageLibPageSize.ToInt()));
        }

        public static string ImageLibNext(Context context, SiteSettings ss, int offset)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var imageLibData = new ImageLibData(
                context: context,
                ss: ss,
                view: view,
                offset: offset,
                pageSize: ss.ImageLibPageSize.ToInt());
            var hb = new HtmlBuilder();
            new ImageLibData(
                context: context,
                ss: ss,
                view: view,
                offset: offset,
                pageSize: Parameters.General.ImageLibPageSize)
                    .DataRows
                    .ForEach(dataRow => hb
                        .ImageLibItem(
                            context: context,
                            ss: ss,
                            dataRow: dataRow));
            return (new ResponseCollection())
                .Append("#ImageLib", hb)
                .Val("#ImageLibOffset", ss.ImageLibNextOffset(
                    offset,
                    imageLibData.DataRows.Count(),
                    imageLibData.TotalCount))
                .Paging("#ImageLib")
                .ToJson();
        }

        public static void SetLinks(
            this List<ResultModel> results, Context context, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks(context: context);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    context: context,
                    columnName: link.ColumnName,
                    selectedValues: results
                        .Select(o => o.PropertyValue(
                            context: context,
                            name: link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel => resultModel
                    .SetTitle(context: context, ss: ss));
            }
        }

        private static bool InRange(Context context, SiteSettings ss, View view, int limit)
        {
            return Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: view.Where(context: context, ss: ss))) <= limit;
        }
    }
}
