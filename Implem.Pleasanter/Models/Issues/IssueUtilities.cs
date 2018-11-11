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
    public static class IssueUtilities
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
                gridData: gridData,
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
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = IssueValidators.OnEntry(
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
                referenceType: "Issues",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(context: context),
                userStyle: ss.ViewModeStyles(context: context),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("IssuesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(
                                context: context,
                                id: ss.SiteId)),
                        action: () => hb
                            .ViewSelector(context: context, ss: ss, view: view)
                            .ViewFilters(context: context, ss: ss, view: view)
                            .Aggregations(
                                context: context,
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(
                                controlId: "TableName",
                                value: "Issues")
                            .Hidden(
                                controlId: "BaseUrl",
                                value: Locations.BaseUrl(context: context)))
                    .EditorDialog(context: context, ss: ss)
                    .DropDownSearchDialog(
                        context: context,
                        controller: "items",
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
                    gridData: gridData,
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
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
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
                        gridData.Aggregations.TotalCount)
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
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    context: context,
                    ss: ss,
                    aggregations: gridData.Aggregations),
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
                    gridData.Aggregations.TotalCount))
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
            var sqlColumnCollection = Rds.IssuesColumn();
            new List<string> { "SiteId", "IssueId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.IssuesColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(
            Context context, SiteSettings ss)
        {
            var sqlColumnCollection = Rds.IssuesColumn();
            new List<string> { "SiteId", "IssueId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks(context: context).Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.IssuesColumn(column));
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
                gridData: gridData,
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
                    gridData: gridData,
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
            IssueModel issueModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    context: context,
                    ss: ss,
                    gridDesign: column.GridDesign,
                    issueModel: issueModel);
            }
            else
            {
                var mine = issueModel.Mine(context: context);
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
                                    value: issueModel.SiteId)
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
                                    value: issueModel.UpdatedTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "IssueId":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.IssueId)
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
                                    value: issueModel.Ver)
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
                                    value: issueModel.Title)
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
                                    value: issueModel.Body)
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
                                    value: issueModel.TitleBody)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "StartTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.StartTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "CompletionTime":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.CompletionTime)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "WorkValue":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.WorkValue)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "ProgressRate":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.ProgressRate)
                                : hb.Td(
                                    context: context,
                                    column: column,
                                    value: string.Empty);
                    case "RemainingWorkValue":
                        return ss.ReadColumnAccessControls.Allowed(
                            context: context,
                            ss: ss,
                            column: column,
                            type: ss.PermissionType,
                            mine: mine)
                                ? hb.Td(
                                    context: context,
                                    column: column,
                                    value: issueModel.RemainingWorkValue)
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
                                    value: issueModel.Status)
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
                                    value: issueModel.Manager)
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
                                    value: issueModel.Owner)
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
                                    value: issueModel.ClassA)
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
                                    value: issueModel.ClassB)
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
                                    value: issueModel.ClassC)
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
                                    value: issueModel.ClassD)
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
                                    value: issueModel.ClassE)
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
                                    value: issueModel.ClassF)
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
                                    value: issueModel.ClassG)
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
                                    value: issueModel.ClassH)
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
                                    value: issueModel.ClassI)
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
                                    value: issueModel.ClassJ)
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
                                    value: issueModel.ClassK)
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
                                    value: issueModel.ClassL)
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
                                    value: issueModel.ClassM)
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
                                    value: issueModel.ClassN)
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
                                    value: issueModel.ClassO)
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
                                    value: issueModel.ClassP)
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
                                    value: issueModel.ClassQ)
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
                                    value: issueModel.ClassR)
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
                                    value: issueModel.ClassS)
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
                                    value: issueModel.ClassT)
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
                                    value: issueModel.ClassU)
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
                                    value: issueModel.ClassV)
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
                                    value: issueModel.ClassW)
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
                                    value: issueModel.ClassX)
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
                                    value: issueModel.ClassY)
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
                                    value: issueModel.ClassZ)
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
                                    value: issueModel.NumA)
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
                                    value: issueModel.NumB)
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
                                    value: issueModel.NumC)
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
                                    value: issueModel.NumD)
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
                                    value: issueModel.NumE)
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
                                    value: issueModel.NumF)
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
                                    value: issueModel.NumG)
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
                                    value: issueModel.NumH)
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
                                    value: issueModel.NumI)
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
                                    value: issueModel.NumJ)
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
                                    value: issueModel.NumK)
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
                                    value: issueModel.NumL)
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
                                    value: issueModel.NumM)
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
                                    value: issueModel.NumN)
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
                                    value: issueModel.NumO)
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
                                    value: issueModel.NumP)
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
                                    value: issueModel.NumQ)
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
                                    value: issueModel.NumR)
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
                                    value: issueModel.NumS)
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
                                    value: issueModel.NumT)
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
                                    value: issueModel.NumU)
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
                                    value: issueModel.NumV)
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
                                    value: issueModel.NumW)
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
                                    value: issueModel.NumX)
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
                                    value: issueModel.NumY)
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
                                    value: issueModel.NumZ)
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
                                    value: issueModel.DateA)
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
                                    value: issueModel.DateB)
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
                                    value: issueModel.DateC)
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
                                    value: issueModel.DateD)
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
                                    value: issueModel.DateE)
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
                                    value: issueModel.DateF)
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
                                    value: issueModel.DateG)
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
                                    value: issueModel.DateH)
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
                                    value: issueModel.DateI)
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
                                    value: issueModel.DateJ)
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
                                    value: issueModel.DateK)
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
                                    value: issueModel.DateL)
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
                                    value: issueModel.DateM)
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
                                    value: issueModel.DateN)
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
                                    value: issueModel.DateO)
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
                                    value: issueModel.DateP)
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
                                    value: issueModel.DateQ)
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
                                    value: issueModel.DateR)
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
                                    value: issueModel.DateS)
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
                                    value: issueModel.DateT)
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
                                    value: issueModel.DateU)
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
                                    value: issueModel.DateV)
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
                                    value: issueModel.DateW)
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
                                    value: issueModel.DateX)
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
                                    value: issueModel.DateY)
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
                                    value: issueModel.DateZ)
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
                                    value: issueModel.DescriptionA)
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
                                    value: issueModel.DescriptionB)
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
                                    value: issueModel.DescriptionC)
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
                                    value: issueModel.DescriptionD)
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
                                    value: issueModel.DescriptionE)
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
                                    value: issueModel.DescriptionF)
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
                                    value: issueModel.DescriptionG)
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
                                    value: issueModel.DescriptionH)
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
                                    value: issueModel.DescriptionI)
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
                                    value: issueModel.DescriptionJ)
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
                                    value: issueModel.DescriptionK)
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
                                    value: issueModel.DescriptionL)
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
                                    value: issueModel.DescriptionM)
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
                                    value: issueModel.DescriptionN)
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
                                    value: issueModel.DescriptionO)
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
                                    value: issueModel.DescriptionP)
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
                                    value: issueModel.DescriptionQ)
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
                                    value: issueModel.DescriptionR)
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
                                    value: issueModel.DescriptionS)
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
                                    value: issueModel.DescriptionT)
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
                                    value: issueModel.DescriptionU)
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
                                    value: issueModel.DescriptionV)
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
                                    value: issueModel.DescriptionW)
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
                                    value: issueModel.DescriptionX)
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
                                    value: issueModel.DescriptionY)
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
                                    value: issueModel.DescriptionZ)
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
                                    value: issueModel.CheckA)
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
                                    value: issueModel.CheckB)
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
                                    value: issueModel.CheckC)
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
                                    value: issueModel.CheckD)
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
                                    value: issueModel.CheckE)
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
                                    value: issueModel.CheckF)
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
                                    value: issueModel.CheckG)
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
                                    value: issueModel.CheckH)
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
                                    value: issueModel.CheckI)
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
                                    value: issueModel.CheckJ)
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
                                    value: issueModel.CheckK)
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
                                    value: issueModel.CheckL)
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
                                    value: issueModel.CheckM)
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
                                    value: issueModel.CheckN)
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
                                    value: issueModel.CheckO)
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
                                    value: issueModel.CheckP)
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
                                    value: issueModel.CheckQ)
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
                                    value: issueModel.CheckR)
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
                                    value: issueModel.CheckS)
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
                                    value: issueModel.CheckT)
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
                                    value: issueModel.CheckU)
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
                                    value: issueModel.CheckV)
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
                                    value: issueModel.CheckW)
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
                                    value: issueModel.CheckX)
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
                                    value: issueModel.CheckY)
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
                                    value: issueModel.CheckZ)
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
                                    value: issueModel.AttachmentsA)
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
                                    value: issueModel.AttachmentsB)
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
                                    value: issueModel.AttachmentsC)
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
                                    value: issueModel.AttachmentsD)
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
                                    value: issueModel.AttachmentsE)
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
                                    value: issueModel.AttachmentsF)
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
                                    value: issueModel.AttachmentsG)
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
                                    value: issueModel.AttachmentsH)
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
                                    value: issueModel.AttachmentsI)
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
                                    value: issueModel.AttachmentsJ)
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
                                    value: issueModel.AttachmentsK)
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
                                    value: issueModel.AttachmentsL)
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
                                    value: issueModel.AttachmentsM)
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
                                    value: issueModel.AttachmentsN)
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
                                    value: issueModel.AttachmentsO)
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
                                    value: issueModel.AttachmentsP)
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
                                    value: issueModel.AttachmentsQ)
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
                                    value: issueModel.AttachmentsR)
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
                                    value: issueModel.AttachmentsS)
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
                                    value: issueModel.AttachmentsT)
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
                                    value: issueModel.AttachmentsU)
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
                                    value: issueModel.AttachmentsV)
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
                                    value: issueModel.AttachmentsW)
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
                                    value: issueModel.AttachmentsX)
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
                                    value: issueModel.AttachmentsY)
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
                                    value: issueModel.AttachmentsZ)
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
                                    value: issueModel.SiteTitle)
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
                                    value: issueModel.Comments)
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
                                    value: issueModel.Creator)
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
                                    value: issueModel.Updator)
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
                                    value: issueModel.CreatedTime)
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
            IssueModel issueModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = issueModel.SiteId.GridText(
                        context: context,
                        column: column); break;
                    case "UpdatedTime": value = issueModel.UpdatedTime.GridText(
                        context: context,
                        column: column); break;
                    case "IssueId": value = issueModel.IssueId.GridText(
                        context: context,
                        column: column); break;
                    case "Ver": value = issueModel.Ver.GridText(
                        context: context,
                        column: column); break;
                    case "Title": value = issueModel.Title.GridText(
                        context: context,
                        column: column); break;
                    case "Body": value = issueModel.Body.GridText(
                        context: context,
                        column: column); break;
                    case "TitleBody": value = issueModel.TitleBody.GridText(
                        context: context,
                        column: column); break;
                    case "StartTime": value = issueModel.StartTime.GridText(
                        context: context,
                        column: column); break;
                    case "CompletionTime": value = issueModel.CompletionTime.GridText(
                        context: context,
                        column: column); break;
                    case "WorkValue": value = issueModel.WorkValue.GridText(
                        context: context,
                        column: column); break;
                    case "ProgressRate": value = issueModel.ProgressRate.GridText(
                        context: context,
                        column: column); break;
                    case "RemainingWorkValue": value = issueModel.RemainingWorkValue.GridText(
                        context: context,
                        column: column); break;
                    case "Status": value = issueModel.Status.GridText(
                        context: context,
                        column: column); break;
                    case "Manager": value = issueModel.Manager.GridText(
                        context: context,
                        column: column); break;
                    case "Owner": value = issueModel.Owner.GridText(
                        context: context,
                        column: column); break;
                    case "ClassA": value = issueModel.ClassA.GridText(
                        context: context,
                        column: column); break;
                    case "ClassB": value = issueModel.ClassB.GridText(
                        context: context,
                        column: column); break;
                    case "ClassC": value = issueModel.ClassC.GridText(
                        context: context,
                        column: column); break;
                    case "ClassD": value = issueModel.ClassD.GridText(
                        context: context,
                        column: column); break;
                    case "ClassE": value = issueModel.ClassE.GridText(
                        context: context,
                        column: column); break;
                    case "ClassF": value = issueModel.ClassF.GridText(
                        context: context,
                        column: column); break;
                    case "ClassG": value = issueModel.ClassG.GridText(
                        context: context,
                        column: column); break;
                    case "ClassH": value = issueModel.ClassH.GridText(
                        context: context,
                        column: column); break;
                    case "ClassI": value = issueModel.ClassI.GridText(
                        context: context,
                        column: column); break;
                    case "ClassJ": value = issueModel.ClassJ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassK": value = issueModel.ClassK.GridText(
                        context: context,
                        column: column); break;
                    case "ClassL": value = issueModel.ClassL.GridText(
                        context: context,
                        column: column); break;
                    case "ClassM": value = issueModel.ClassM.GridText(
                        context: context,
                        column: column); break;
                    case "ClassN": value = issueModel.ClassN.GridText(
                        context: context,
                        column: column); break;
                    case "ClassO": value = issueModel.ClassO.GridText(
                        context: context,
                        column: column); break;
                    case "ClassP": value = issueModel.ClassP.GridText(
                        context: context,
                        column: column); break;
                    case "ClassQ": value = issueModel.ClassQ.GridText(
                        context: context,
                        column: column); break;
                    case "ClassR": value = issueModel.ClassR.GridText(
                        context: context,
                        column: column); break;
                    case "ClassS": value = issueModel.ClassS.GridText(
                        context: context,
                        column: column); break;
                    case "ClassT": value = issueModel.ClassT.GridText(
                        context: context,
                        column: column); break;
                    case "ClassU": value = issueModel.ClassU.GridText(
                        context: context,
                        column: column); break;
                    case "ClassV": value = issueModel.ClassV.GridText(
                        context: context,
                        column: column); break;
                    case "ClassW": value = issueModel.ClassW.GridText(
                        context: context,
                        column: column); break;
                    case "ClassX": value = issueModel.ClassX.GridText(
                        context: context,
                        column: column); break;
                    case "ClassY": value = issueModel.ClassY.GridText(
                        context: context,
                        column: column); break;
                    case "ClassZ": value = issueModel.ClassZ.GridText(
                        context: context,
                        column: column); break;
                    case "NumA": value = issueModel.NumA.GridText(
                        context: context,
                        column: column); break;
                    case "NumB": value = issueModel.NumB.GridText(
                        context: context,
                        column: column); break;
                    case "NumC": value = issueModel.NumC.GridText(
                        context: context,
                        column: column); break;
                    case "NumD": value = issueModel.NumD.GridText(
                        context: context,
                        column: column); break;
                    case "NumE": value = issueModel.NumE.GridText(
                        context: context,
                        column: column); break;
                    case "NumF": value = issueModel.NumF.GridText(
                        context: context,
                        column: column); break;
                    case "NumG": value = issueModel.NumG.GridText(
                        context: context,
                        column: column); break;
                    case "NumH": value = issueModel.NumH.GridText(
                        context: context,
                        column: column); break;
                    case "NumI": value = issueModel.NumI.GridText(
                        context: context,
                        column: column); break;
                    case "NumJ": value = issueModel.NumJ.GridText(
                        context: context,
                        column: column); break;
                    case "NumK": value = issueModel.NumK.GridText(
                        context: context,
                        column: column); break;
                    case "NumL": value = issueModel.NumL.GridText(
                        context: context,
                        column: column); break;
                    case "NumM": value = issueModel.NumM.GridText(
                        context: context,
                        column: column); break;
                    case "NumN": value = issueModel.NumN.GridText(
                        context: context,
                        column: column); break;
                    case "NumO": value = issueModel.NumO.GridText(
                        context: context,
                        column: column); break;
                    case "NumP": value = issueModel.NumP.GridText(
                        context: context,
                        column: column); break;
                    case "NumQ": value = issueModel.NumQ.GridText(
                        context: context,
                        column: column); break;
                    case "NumR": value = issueModel.NumR.GridText(
                        context: context,
                        column: column); break;
                    case "NumS": value = issueModel.NumS.GridText(
                        context: context,
                        column: column); break;
                    case "NumT": value = issueModel.NumT.GridText(
                        context: context,
                        column: column); break;
                    case "NumU": value = issueModel.NumU.GridText(
                        context: context,
                        column: column); break;
                    case "NumV": value = issueModel.NumV.GridText(
                        context: context,
                        column: column); break;
                    case "NumW": value = issueModel.NumW.GridText(
                        context: context,
                        column: column); break;
                    case "NumX": value = issueModel.NumX.GridText(
                        context: context,
                        column: column); break;
                    case "NumY": value = issueModel.NumY.GridText(
                        context: context,
                        column: column); break;
                    case "NumZ": value = issueModel.NumZ.GridText(
                        context: context,
                        column: column); break;
                    case "DateA": value = issueModel.DateA.GridText(
                        context: context,
                        column: column); break;
                    case "DateB": value = issueModel.DateB.GridText(
                        context: context,
                        column: column); break;
                    case "DateC": value = issueModel.DateC.GridText(
                        context: context,
                        column: column); break;
                    case "DateD": value = issueModel.DateD.GridText(
                        context: context,
                        column: column); break;
                    case "DateE": value = issueModel.DateE.GridText(
                        context: context,
                        column: column); break;
                    case "DateF": value = issueModel.DateF.GridText(
                        context: context,
                        column: column); break;
                    case "DateG": value = issueModel.DateG.GridText(
                        context: context,
                        column: column); break;
                    case "DateH": value = issueModel.DateH.GridText(
                        context: context,
                        column: column); break;
                    case "DateI": value = issueModel.DateI.GridText(
                        context: context,
                        column: column); break;
                    case "DateJ": value = issueModel.DateJ.GridText(
                        context: context,
                        column: column); break;
                    case "DateK": value = issueModel.DateK.GridText(
                        context: context,
                        column: column); break;
                    case "DateL": value = issueModel.DateL.GridText(
                        context: context,
                        column: column); break;
                    case "DateM": value = issueModel.DateM.GridText(
                        context: context,
                        column: column); break;
                    case "DateN": value = issueModel.DateN.GridText(
                        context: context,
                        column: column); break;
                    case "DateO": value = issueModel.DateO.GridText(
                        context: context,
                        column: column); break;
                    case "DateP": value = issueModel.DateP.GridText(
                        context: context,
                        column: column); break;
                    case "DateQ": value = issueModel.DateQ.GridText(
                        context: context,
                        column: column); break;
                    case "DateR": value = issueModel.DateR.GridText(
                        context: context,
                        column: column); break;
                    case "DateS": value = issueModel.DateS.GridText(
                        context: context,
                        column: column); break;
                    case "DateT": value = issueModel.DateT.GridText(
                        context: context,
                        column: column); break;
                    case "DateU": value = issueModel.DateU.GridText(
                        context: context,
                        column: column); break;
                    case "DateV": value = issueModel.DateV.GridText(
                        context: context,
                        column: column); break;
                    case "DateW": value = issueModel.DateW.GridText(
                        context: context,
                        column: column); break;
                    case "DateX": value = issueModel.DateX.GridText(
                        context: context,
                        column: column); break;
                    case "DateY": value = issueModel.DateY.GridText(
                        context: context,
                        column: column); break;
                    case "DateZ": value = issueModel.DateZ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionA": value = issueModel.DescriptionA.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionB": value = issueModel.DescriptionB.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionC": value = issueModel.DescriptionC.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionD": value = issueModel.DescriptionD.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionE": value = issueModel.DescriptionE.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionF": value = issueModel.DescriptionF.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionG": value = issueModel.DescriptionG.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionH": value = issueModel.DescriptionH.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionI": value = issueModel.DescriptionI.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionJ": value = issueModel.DescriptionJ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionK": value = issueModel.DescriptionK.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionL": value = issueModel.DescriptionL.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionM": value = issueModel.DescriptionM.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionN": value = issueModel.DescriptionN.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionO": value = issueModel.DescriptionO.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionP": value = issueModel.DescriptionP.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionQ": value = issueModel.DescriptionQ.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionR": value = issueModel.DescriptionR.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionS": value = issueModel.DescriptionS.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionT": value = issueModel.DescriptionT.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionU": value = issueModel.DescriptionU.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionV": value = issueModel.DescriptionV.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionW": value = issueModel.DescriptionW.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionX": value = issueModel.DescriptionX.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionY": value = issueModel.DescriptionY.GridText(
                        context: context,
                        column: column); break;
                    case "DescriptionZ": value = issueModel.DescriptionZ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckA": value = issueModel.CheckA.GridText(
                        context: context,
                        column: column); break;
                    case "CheckB": value = issueModel.CheckB.GridText(
                        context: context,
                        column: column); break;
                    case "CheckC": value = issueModel.CheckC.GridText(
                        context: context,
                        column: column); break;
                    case "CheckD": value = issueModel.CheckD.GridText(
                        context: context,
                        column: column); break;
                    case "CheckE": value = issueModel.CheckE.GridText(
                        context: context,
                        column: column); break;
                    case "CheckF": value = issueModel.CheckF.GridText(
                        context: context,
                        column: column); break;
                    case "CheckG": value = issueModel.CheckG.GridText(
                        context: context,
                        column: column); break;
                    case "CheckH": value = issueModel.CheckH.GridText(
                        context: context,
                        column: column); break;
                    case "CheckI": value = issueModel.CheckI.GridText(
                        context: context,
                        column: column); break;
                    case "CheckJ": value = issueModel.CheckJ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckK": value = issueModel.CheckK.GridText(
                        context: context,
                        column: column); break;
                    case "CheckL": value = issueModel.CheckL.GridText(
                        context: context,
                        column: column); break;
                    case "CheckM": value = issueModel.CheckM.GridText(
                        context: context,
                        column: column); break;
                    case "CheckN": value = issueModel.CheckN.GridText(
                        context: context,
                        column: column); break;
                    case "CheckO": value = issueModel.CheckO.GridText(
                        context: context,
                        column: column); break;
                    case "CheckP": value = issueModel.CheckP.GridText(
                        context: context,
                        column: column); break;
                    case "CheckQ": value = issueModel.CheckQ.GridText(
                        context: context,
                        column: column); break;
                    case "CheckR": value = issueModel.CheckR.GridText(
                        context: context,
                        column: column); break;
                    case "CheckS": value = issueModel.CheckS.GridText(
                        context: context,
                        column: column); break;
                    case "CheckT": value = issueModel.CheckT.GridText(
                        context: context,
                        column: column); break;
                    case "CheckU": value = issueModel.CheckU.GridText(
                        context: context,
                        column: column); break;
                    case "CheckV": value = issueModel.CheckV.GridText(
                        context: context,
                        column: column); break;
                    case "CheckW": value = issueModel.CheckW.GridText(
                        context: context,
                        column: column); break;
                    case "CheckX": value = issueModel.CheckX.GridText(
                        context: context,
                        column: column); break;
                    case "CheckY": value = issueModel.CheckY.GridText(
                        context: context,
                        column: column); break;
                    case "CheckZ": value = issueModel.CheckZ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsA": value = issueModel.AttachmentsA.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsB": value = issueModel.AttachmentsB.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsC": value = issueModel.AttachmentsC.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsD": value = issueModel.AttachmentsD.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsE": value = issueModel.AttachmentsE.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsF": value = issueModel.AttachmentsF.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsG": value = issueModel.AttachmentsG.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsH": value = issueModel.AttachmentsH.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsI": value = issueModel.AttachmentsI.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsJ": value = issueModel.AttachmentsJ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsK": value = issueModel.AttachmentsK.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsL": value = issueModel.AttachmentsL.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsM": value = issueModel.AttachmentsM.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsN": value = issueModel.AttachmentsN.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsO": value = issueModel.AttachmentsO.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsP": value = issueModel.AttachmentsP.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsQ": value = issueModel.AttachmentsQ.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsR": value = issueModel.AttachmentsR.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsS": value = issueModel.AttachmentsS.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsT": value = issueModel.AttachmentsT.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsU": value = issueModel.AttachmentsU.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsV": value = issueModel.AttachmentsV.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsW": value = issueModel.AttachmentsW.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsX": value = issueModel.AttachmentsX.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsY": value = issueModel.AttachmentsY.GridText(
                        context: context,
                        column: column); break;
                    case "AttachmentsZ": value = issueModel.AttachmentsZ.GridText(
                        context: context,
                        column: column); break;
                    case "SiteTitle": value = issueModel.SiteTitle.GridText(
                        context: context,
                        column: column); break;
                    case "Comments": value = issueModel.Comments.GridText(
                        context: context,
                        column: column); break;
                    case "Creator": value = issueModel.Creator.GridText(
                        context: context,
                        column: column); break;
                    case "Updator": value = issueModel.Updator.GridText(
                        context: context,
                        column: column); break;
                    case "CreatedTime": value = issueModel.CreatedTime.GridText(
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
                issueModel: new IssueModel(
                    context: context,
                    ss: ss,
                    methodType: BaseModel.MethodTypes.New,
                    setByForm: true));
        }

        public static string Editor(
            Context context, SiteSettings ss, long issueId, bool clearSessions)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            issueModel.SwitchTargets = GetSwitchTargets(
                context: context,
                ss: ss,
                issueId: issueModel.IssueId,
                siteId: issueModel.SiteId);
            return Editor(
                context: context,
                ss: ss,
                issueModel: issueModel);
        }

        public static string Editor(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool editInDialog = false)
        {
            var invalid = IssueValidators.OnEditing(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(context, invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
            return editInDialog
                ? hb.DialogEditorForm(
                    context: context,
                    siteId: issueModel.SiteId,
                    referenceId: issueModel.IssueId,
                    action: () => hb
                        .FieldSetGeneral(
                            context: context,
                            ss: ss,
                            issueModel: issueModel,
                            editInDialog: editInDialog))
                                .ToString()
                : hb.Template(
                    context: context,
                    ss: ss,
                    view: null,
                    verType: issueModel.VerType,
                    methodType: issueModel.MethodType,
                    siteId: issueModel.SiteId,
                    parentId: ss.ParentId,
                    referenceType: "Issues",
                    title: issueModel.MethodType == BaseModel.MethodTypes.New
                        ? Displays.New(context: context)
                        : issueModel.Title.DisplayValue,
                    useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                    userScript: ss.EditorScripts(
                        context: context, methodType: issueModel.MethodType),
                    userStyle: ss.EditorStyles(
                        context: context, methodType: issueModel.MethodType),
                    action: () => hb
                        .Editor(
                            context: context,
                            ss: ss,
                            issueModel: issueModel)
                        .Hidden(controlId: "TableName", value: "Issues")
                        .Hidden(controlId: "Id", value: issueModel.IssueId.ToString())
                        .Hidden(controlId: "TriggerRelatingColumns", value: Jsons.ToJson(ss.RelatingColumns))
                        .Hidden(controlId: "DropDownSearchPageSize", value: Parameters.General.DropDownSearchPageSize.ToString()))
                            .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var commentsColumn = ss.GetColumn(context: context, columnName: "Comments");
            var commentsColumnPermissionType = commentsColumn
                .ColumnPermissionType(context: context);
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("IssueForm")
                        .Class("main-form confirm-reload")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: issueModel.IssueId != 0 
                                ? issueModel.IssueId
                                : issueModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            context: context,
                            ss: ss,
                            baseModel: issueModel,
                            tableName: "Issues")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    context: context,
                                    ss: ss,
                                    comments: issueModel.Comments,
                                    column: commentsColumn,
                                    verType: issueModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(
                                context: context,
                                ss: ss,
                                issueModel: issueModel)
                            .FieldSetGeneral(
                                context: context,
                                ss: ss,
                                issueModel: issueModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: issueModel.MethodType != BaseModel.MethodTypes.New)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: context.CanManagePermission(ss: ss) &&
                                    issueModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                context: context,
                                ss: ss,
                                siteId: issueModel.SiteId,
                                verType: issueModel.VerType,
                                referenceId: issueModel.IssueId,
                                updateButton: true,
                                copyButton: true,
                                moveButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        context: context,
                                        ss: ss,
                                        issueModel: issueModel)))
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
                            value: issueModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Issues_Timestamp",
                            css: "always-send",
                            value: issueModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: issueModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm(
                    context: context,
                    ss: ss,
                    referenceType: "Issues",
                    referenceId: issueModel.IssueId,
                    referenceVer: issueModel.Ver)
                .DropDownSearchDialog(
                    context: context,
                    controller: "items",
                    id: ss.SiteId)
                .CopyDialog(
                    context: context,
                    referenceType: "items",
                    id: issueModel.IssueId)
                .MoveDialog(context: context)
                .OutgoingMailDialog()
                .PermissionsDialog(context: context)
                .EditorExtensions(
                    context: context,
                    issueModel: issueModel,
                    ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context)))
                .Li(_using: issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList(context: context)))
                .Li(_using: context.CanManagePermission(ss: ss) &&
                        issueModel.MethodType != BaseModel.MethodTypes.New,
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
                                issueModel: new IssueModel(),
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
            IssueModel issueModel,
            bool editInDialog = false)
        {
            var mine = issueModel.Mine(context: context);
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    editInDialog: editInDialog));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            bool preview = false,
            bool editInDialog = false)
        {
            ss.GetEditorColumns(context: context).ForEach(column =>
            {
                switch (column.Name)
                {
                    case "IssueId":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.IssueId
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Ver
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Title
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Body
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "StartTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.StartTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CompletionTime":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CompletionTime
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "WorkValue":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.WorkValue
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ProgressRate":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ProgressRate
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "RemainingWorkValue":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.RemainingWorkValue
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Status":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Status
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Manager":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Manager
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "Owner":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.Owner
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.ClassZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.NumZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DateZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.DescriptionZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.CheckZ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsA":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsA
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsB":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsB
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsC":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsC
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsD":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsD
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsE":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsE
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsF":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsF
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsG":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsG
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsH":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsH
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsI":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsI
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsJ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsJ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsK":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsK
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsL":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsL
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsM":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsM
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsN":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsN
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsO":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsO
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsP":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsP
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsQ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsQ
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsR":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsR
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsS":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsS
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsT":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsT
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsU":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsU
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsV":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsV
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsW":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsW
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsX":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsX
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsY":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsY
                                .ToControl(context: context, ss: ss, column: column),
                            columnPermissionType: column.ColumnPermissionType(context: context),
                            preview: preview);
                        break;
                    case "AttachmentsZ":
                        hb.Field(
                            context: context,
                            ss: ss,
                            column: column,
                            methodType: issueModel.MethodType,
                            value: issueModel.AttachmentsZ
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
                    baseModel: issueModel);
                if (!editInDialog)
                {
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                context: context,
                                ss: ss,
                                linkId: issueModel.IssueId,
                                methodType: issueModel.MethodType))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(
                                context: context,
                                ss: ss,
                                id: issueModel.IssueId));
                }
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.Button(
                        text: Displays.Separate(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openSeparateSettingsDialog($(this));",
                        icon: "ui-icon-extlink",
                        action: "EditSeparateSettings",
                        method: "post",
                        _using: context.CanUpdate(ss: ss))
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.SeparateSettingsDialog(context: context)
                    : hb;
        }

        public static string EditorJson(Context context, SiteSettings ss, long issueId)
        {
            return EditorResponse(context, ss, new IssueModel(
                context, ss, issueId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            Message message = null,
            string switchTargets = null)
        {
            issueModel.MethodType = BaseModel.MethodTypes.Edit;
            var editInDialog = context.Forms.Bool("EditInDialog");
            return editInDialog
                ? new IssuesResponseCollection(issueModel)
                    .Html("#EditInDialogBody", Editor(
                        context: context,
                        ss: ss,
                        issueModel: issueModel,
                        editInDialog: editInDialog))
                    .Invoke("openEditorDialog")
                : new IssuesResponseCollection(issueModel)
                    .Invoke("clearDialogs")
                    .ReplaceAll("#MainContainer", Editor(context, ss, issueModel))
                    .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                    .SetMemory("formChanged", false)
                    .Invoke("setCurrentIndex")
                    .Message(message)
                    .ClearFormData();
        }

        private static List<long> GetSwitchTargets(Context context, SiteSettings ss, long issueId, long siteId)
        {
            var view = Views.GetBySession(context: context, ss: ss);
            var where = view.Where(context: context, ss: ss);
            var join = ss.Join(context: context);
            var switchTargets = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(
                            context: context,
                            statements: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                join: join,
                                where: where,
                                orderBy: view.OrderBy(context: context, ss: ss)
                                    .Issues_UpdatedTime(SqlOrderBy.Types.desc)))
                                        .AsEnumerable()
                                        .Select(o => o["IssueId"].ToLong())
                                        .ToList()
                        : new List<long>();
            if (!switchTargets.Contains(issueId))
            {
                switchTargets.Add(issueId);
            }
            return switchTargets;
        }

        public static ResponseCollection FieldResponse(
            this IssuesResponseCollection res,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var mine = issueModel.Mine(context: context);
            ss.EditorColumns
                .Select(columnName => ss.GetColumn(context: context, columnName: columnName))
                .Where(column => column != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            res.Val(
                                "#Issues_SiteId",
                                issueModel.SiteId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "UpdatedTime":
                            res.Val(
                                "#Issues_UpdatedTime",
                                issueModel.UpdatedTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "IssueId":
                            res.Val(
                                "#Issues_IssueId",
                                issueModel.IssueId.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Ver":
                            res.Val(
                                "#Issues_Ver",
                                issueModel.Ver.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Title":
                            res.Val(
                                "#Issues_Title",
                                issueModel.Title.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Body":
                            res.Val(
                                "#Issues_Body",
                                issueModel.Body.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "StartTime":
                            res.Val(
                                "#Issues_StartTime",
                                issueModel.StartTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "CompletionTime":
                            res.Val(
                                "#Issues_CompletionTime",
                                issueModel.CompletionTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "WorkValue":
                            res.Val(
                                "#Issues_WorkValue",
                                issueModel.WorkValue.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ProgressRate":
                            res.Val(
                                "#Issues_ProgressRate",
                                issueModel.ProgressRate.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Status":
                            res.Val(
                                "#Issues_Status",
                                issueModel.Status.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Issues_Manager",
                                issueModel.Manager.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Issues_Owner",
                                issueModel.Owner.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassA":
                            res.Val(
                                "#Issues_ClassA",
                                issueModel.ClassA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassB":
                            res.Val(
                                "#Issues_ClassB",
                                issueModel.ClassB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassC":
                            res.Val(
                                "#Issues_ClassC",
                                issueModel.ClassC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassD":
                            res.Val(
                                "#Issues_ClassD",
                                issueModel.ClassD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassE":
                            res.Val(
                                "#Issues_ClassE",
                                issueModel.ClassE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassF":
                            res.Val(
                                "#Issues_ClassF",
                                issueModel.ClassF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassG":
                            res.Val(
                                "#Issues_ClassG",
                                issueModel.ClassG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassH":
                            res.Val(
                                "#Issues_ClassH",
                                issueModel.ClassH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassI":
                            res.Val(
                                "#Issues_ClassI",
                                issueModel.ClassI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassJ":
                            res.Val(
                                "#Issues_ClassJ",
                                issueModel.ClassJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassK":
                            res.Val(
                                "#Issues_ClassK",
                                issueModel.ClassK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassL":
                            res.Val(
                                "#Issues_ClassL",
                                issueModel.ClassL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassM":
                            res.Val(
                                "#Issues_ClassM",
                                issueModel.ClassM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassN":
                            res.Val(
                                "#Issues_ClassN",
                                issueModel.ClassN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassO":
                            res.Val(
                                "#Issues_ClassO",
                                issueModel.ClassO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassP":
                            res.Val(
                                "#Issues_ClassP",
                                issueModel.ClassP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassQ":
                            res.Val(
                                "#Issues_ClassQ",
                                issueModel.ClassQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassR":
                            res.Val(
                                "#Issues_ClassR",
                                issueModel.ClassR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassS":
                            res.Val(
                                "#Issues_ClassS",
                                issueModel.ClassS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassT":
                            res.Val(
                                "#Issues_ClassT",
                                issueModel.ClassT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassU":
                            res.Val(
                                "#Issues_ClassU",
                                issueModel.ClassU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassV":
                            res.Val(
                                "#Issues_ClassV",
                                issueModel.ClassV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassW":
                            res.Val(
                                "#Issues_ClassW",
                                issueModel.ClassW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassX":
                            res.Val(
                                "#Issues_ClassX",
                                issueModel.ClassX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassY":
                            res.Val(
                                "#Issues_ClassY",
                                issueModel.ClassY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "ClassZ":
                            res.Val(
                                "#Issues_ClassZ",
                                issueModel.ClassZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumA":
                            res.Val(
                                "#Issues_NumA",
                                issueModel.NumA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumB":
                            res.Val(
                                "#Issues_NumB",
                                issueModel.NumB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumC":
                            res.Val(
                                "#Issues_NumC",
                                issueModel.NumC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumD":
                            res.Val(
                                "#Issues_NumD",
                                issueModel.NumD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumE":
                            res.Val(
                                "#Issues_NumE",
                                issueModel.NumE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumF":
                            res.Val(
                                "#Issues_NumF",
                                issueModel.NumF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumG":
                            res.Val(
                                "#Issues_NumG",
                                issueModel.NumG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumH":
                            res.Val(
                                "#Issues_NumH",
                                issueModel.NumH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumI":
                            res.Val(
                                "#Issues_NumI",
                                issueModel.NumI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumJ":
                            res.Val(
                                "#Issues_NumJ",
                                issueModel.NumJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumK":
                            res.Val(
                                "#Issues_NumK",
                                issueModel.NumK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumL":
                            res.Val(
                                "#Issues_NumL",
                                issueModel.NumL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumM":
                            res.Val(
                                "#Issues_NumM",
                                issueModel.NumM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumN":
                            res.Val(
                                "#Issues_NumN",
                                issueModel.NumN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumO":
                            res.Val(
                                "#Issues_NumO",
                                issueModel.NumO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumP":
                            res.Val(
                                "#Issues_NumP",
                                issueModel.NumP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumQ":
                            res.Val(
                                "#Issues_NumQ",
                                issueModel.NumQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumR":
                            res.Val(
                                "#Issues_NumR",
                                issueModel.NumR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumS":
                            res.Val(
                                "#Issues_NumS",
                                issueModel.NumS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumT":
                            res.Val(
                                "#Issues_NumT",
                                issueModel.NumT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumU":
                            res.Val(
                                "#Issues_NumU",
                                issueModel.NumU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumV":
                            res.Val(
                                "#Issues_NumV",
                                issueModel.NumV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumW":
                            res.Val(
                                "#Issues_NumW",
                                issueModel.NumW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumX":
                            res.Val(
                                "#Issues_NumX",
                                issueModel.NumX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumY":
                            res.Val(
                                "#Issues_NumY",
                                issueModel.NumY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "NumZ":
                            res.Val(
                                "#Issues_NumZ",
                                issueModel.NumZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateA":
                            res.Val(
                                "#Issues_DateA",
                                issueModel.DateA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateB":
                            res.Val(
                                "#Issues_DateB",
                                issueModel.DateB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateC":
                            res.Val(
                                "#Issues_DateC",
                                issueModel.DateC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateD":
                            res.Val(
                                "#Issues_DateD",
                                issueModel.DateD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateE":
                            res.Val(
                                "#Issues_DateE",
                                issueModel.DateE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateF":
                            res.Val(
                                "#Issues_DateF",
                                issueModel.DateF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateG":
                            res.Val(
                                "#Issues_DateG",
                                issueModel.DateG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateH":
                            res.Val(
                                "#Issues_DateH",
                                issueModel.DateH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateI":
                            res.Val(
                                "#Issues_DateI",
                                issueModel.DateI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateJ":
                            res.Val(
                                "#Issues_DateJ",
                                issueModel.DateJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateK":
                            res.Val(
                                "#Issues_DateK",
                                issueModel.DateK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateL":
                            res.Val(
                                "#Issues_DateL",
                                issueModel.DateL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateM":
                            res.Val(
                                "#Issues_DateM",
                                issueModel.DateM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateN":
                            res.Val(
                                "#Issues_DateN",
                                issueModel.DateN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateO":
                            res.Val(
                                "#Issues_DateO",
                                issueModel.DateO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateP":
                            res.Val(
                                "#Issues_DateP",
                                issueModel.DateP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateQ":
                            res.Val(
                                "#Issues_DateQ",
                                issueModel.DateQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateR":
                            res.Val(
                                "#Issues_DateR",
                                issueModel.DateR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateS":
                            res.Val(
                                "#Issues_DateS",
                                issueModel.DateS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateT":
                            res.Val(
                                "#Issues_DateT",
                                issueModel.DateT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateU":
                            res.Val(
                                "#Issues_DateU",
                                issueModel.DateU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateV":
                            res.Val(
                                "#Issues_DateV",
                                issueModel.DateV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateW":
                            res.Val(
                                "#Issues_DateW",
                                issueModel.DateW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateX":
                            res.Val(
                                "#Issues_DateX",
                                issueModel.DateX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateY":
                            res.Val(
                                "#Issues_DateY",
                                issueModel.DateY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DateZ":
                            res.Val(
                                "#Issues_DateZ",
                                issueModel.DateZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionA":
                            res.Val(
                                "#Issues_DescriptionA",
                                issueModel.DescriptionA.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionB":
                            res.Val(
                                "#Issues_DescriptionB",
                                issueModel.DescriptionB.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionC":
                            res.Val(
                                "#Issues_DescriptionC",
                                issueModel.DescriptionC.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionD":
                            res.Val(
                                "#Issues_DescriptionD",
                                issueModel.DescriptionD.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionE":
                            res.Val(
                                "#Issues_DescriptionE",
                                issueModel.DescriptionE.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionF":
                            res.Val(
                                "#Issues_DescriptionF",
                                issueModel.DescriptionF.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionG":
                            res.Val(
                                "#Issues_DescriptionG",
                                issueModel.DescriptionG.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionH":
                            res.Val(
                                "#Issues_DescriptionH",
                                issueModel.DescriptionH.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionI":
                            res.Val(
                                "#Issues_DescriptionI",
                                issueModel.DescriptionI.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionJ":
                            res.Val(
                                "#Issues_DescriptionJ",
                                issueModel.DescriptionJ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionK":
                            res.Val(
                                "#Issues_DescriptionK",
                                issueModel.DescriptionK.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionL":
                            res.Val(
                                "#Issues_DescriptionL",
                                issueModel.DescriptionL.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionM":
                            res.Val(
                                "#Issues_DescriptionM",
                                issueModel.DescriptionM.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionN":
                            res.Val(
                                "#Issues_DescriptionN",
                                issueModel.DescriptionN.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionO":
                            res.Val(
                                "#Issues_DescriptionO",
                                issueModel.DescriptionO.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionP":
                            res.Val(
                                "#Issues_DescriptionP",
                                issueModel.DescriptionP.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionQ":
                            res.Val(
                                "#Issues_DescriptionQ",
                                issueModel.DescriptionQ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionR":
                            res.Val(
                                "#Issues_DescriptionR",
                                issueModel.DescriptionR.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionS":
                            res.Val(
                                "#Issues_DescriptionS",
                                issueModel.DescriptionS.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionT":
                            res.Val(
                                "#Issues_DescriptionT",
                                issueModel.DescriptionT.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionU":
                            res.Val(
                                "#Issues_DescriptionU",
                                issueModel.DescriptionU.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionV":
                            res.Val(
                                "#Issues_DescriptionV",
                                issueModel.DescriptionV.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionW":
                            res.Val(
                                "#Issues_DescriptionW",
                                issueModel.DescriptionW.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionX":
                            res.Val(
                                "#Issues_DescriptionX",
                                issueModel.DescriptionX.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionY":
                            res.Val(
                                "#Issues_DescriptionY",
                                issueModel.DescriptionY.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "DescriptionZ":
                            res.Val(
                                "#Issues_DescriptionZ",
                                issueModel.DescriptionZ.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "CheckA":
                            res.Val(
                                "#Issues_CheckA",
                                issueModel.CheckA);
                            break;
                        case "CheckB":
                            res.Val(
                                "#Issues_CheckB",
                                issueModel.CheckB);
                            break;
                        case "CheckC":
                            res.Val(
                                "#Issues_CheckC",
                                issueModel.CheckC);
                            break;
                        case "CheckD":
                            res.Val(
                                "#Issues_CheckD",
                                issueModel.CheckD);
                            break;
                        case "CheckE":
                            res.Val(
                                "#Issues_CheckE",
                                issueModel.CheckE);
                            break;
                        case "CheckF":
                            res.Val(
                                "#Issues_CheckF",
                                issueModel.CheckF);
                            break;
                        case "CheckG":
                            res.Val(
                                "#Issues_CheckG",
                                issueModel.CheckG);
                            break;
                        case "CheckH":
                            res.Val(
                                "#Issues_CheckH",
                                issueModel.CheckH);
                            break;
                        case "CheckI":
                            res.Val(
                                "#Issues_CheckI",
                                issueModel.CheckI);
                            break;
                        case "CheckJ":
                            res.Val(
                                "#Issues_CheckJ",
                                issueModel.CheckJ);
                            break;
                        case "CheckK":
                            res.Val(
                                "#Issues_CheckK",
                                issueModel.CheckK);
                            break;
                        case "CheckL":
                            res.Val(
                                "#Issues_CheckL",
                                issueModel.CheckL);
                            break;
                        case "CheckM":
                            res.Val(
                                "#Issues_CheckM",
                                issueModel.CheckM);
                            break;
                        case "CheckN":
                            res.Val(
                                "#Issues_CheckN",
                                issueModel.CheckN);
                            break;
                        case "CheckO":
                            res.Val(
                                "#Issues_CheckO",
                                issueModel.CheckO);
                            break;
                        case "CheckP":
                            res.Val(
                                "#Issues_CheckP",
                                issueModel.CheckP);
                            break;
                        case "CheckQ":
                            res.Val(
                                "#Issues_CheckQ",
                                issueModel.CheckQ);
                            break;
                        case "CheckR":
                            res.Val(
                                "#Issues_CheckR",
                                issueModel.CheckR);
                            break;
                        case "CheckS":
                            res.Val(
                                "#Issues_CheckS",
                                issueModel.CheckS);
                            break;
                        case "CheckT":
                            res.Val(
                                "#Issues_CheckT",
                                issueModel.CheckT);
                            break;
                        case "CheckU":
                            res.Val(
                                "#Issues_CheckU",
                                issueModel.CheckU);
                            break;
                        case "CheckV":
                            res.Val(
                                "#Issues_CheckV",
                                issueModel.CheckV);
                            break;
                        case "CheckW":
                            res.Val(
                                "#Issues_CheckW",
                                issueModel.CheckW);
                            break;
                        case "CheckX":
                            res.Val(
                                "#Issues_CheckX",
                                issueModel.CheckX);
                            break;
                        case "CheckY":
                            res.Val(
                                "#Issues_CheckY",
                                issueModel.CheckY);
                            break;
                        case "CheckZ":
                            res.Val(
                                "#Issues_CheckZ",
                                issueModel.CheckZ);
                            break;
                        case "Comments":
                            res.Val(
                                "#Issues_Comments",
                                issueModel.Comments.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Creator":
                            res.Val(
                                "#Issues_Creator",
                                issueModel.Creator.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "Updator":
                            res.Val(
                                "#Issues_Updator",
                                issueModel.Updator.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "CreatedTime":
                            res.Val(
                                "#Issues_CreatedTime",
                                issueModel.CreatedTime.ToResponse(context: context, ss: ss, column: column));
                            break;
                        case "AttachmentsA":
                            res.ReplaceAll(
                                "#Issues_AttachmentsAField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsA.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsB":
                            res.ReplaceAll(
                                "#Issues_AttachmentsBField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsB.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsC":
                            res.ReplaceAll(
                                "#Issues_AttachmentsCField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsC.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsD":
                            res.ReplaceAll(
                                "#Issues_AttachmentsDField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsD.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsE":
                            res.ReplaceAll(
                                "#Issues_AttachmentsEField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsE.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsF":
                            res.ReplaceAll(
                                "#Issues_AttachmentsFField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsF.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsG":
                            res.ReplaceAll(
                                "#Issues_AttachmentsGField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsG.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsH":
                            res.ReplaceAll(
                                "#Issues_AttachmentsHField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsH.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsI":
                            res.ReplaceAll(
                                "#Issues_AttachmentsIField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsI.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsJ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsJField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsJ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsK":
                            res.ReplaceAll(
                                "#Issues_AttachmentsKField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsK.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsL":
                            res.ReplaceAll(
                                "#Issues_AttachmentsLField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsL.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsM":
                            res.ReplaceAll(
                                "#Issues_AttachmentsMField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsM.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsN":
                            res.ReplaceAll(
                                "#Issues_AttachmentsNField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsN.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsO":
                            res.ReplaceAll(
                                "#Issues_AttachmentsOField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsO.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsP":
                            res.ReplaceAll(
                                "#Issues_AttachmentsPField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsP.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsQ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsQField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsQ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsR":
                            res.ReplaceAll(
                                "#Issues_AttachmentsRField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsR.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsS":
                            res.ReplaceAll(
                                "#Issues_AttachmentsSField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsS.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsT":
                            res.ReplaceAll(
                                "#Issues_AttachmentsTField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsT.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsU":
                            res.ReplaceAll(
                                "#Issues_AttachmentsUField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsU.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsV":
                            res.ReplaceAll(
                                "#Issues_AttachmentsVField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsV.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsW":
                            res.ReplaceAll(
                                "#Issues_AttachmentsWField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsW.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsX":
                            res.ReplaceAll(
                                "#Issues_AttachmentsXField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsX.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsY":
                            res.ReplaceAll(
                                "#Issues_AttachmentsYField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsY.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType(
                                            context: context)));
                            break;
                        case "AttachmentsZ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsZField",
                                new HtmlBuilder()
                                    .Field(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsZ.ToJson(),
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
            var api = context.Forms.String().Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var issueCollection = new IssueCollection(
                context: context,
                ss: ss,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    "[Items]",
                    SqlJoin.JoinTypes.Inner,
                    "[Issues].[IssueId]=[Items].[ReferenceId]")),
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
                    issueCollection.TotalCount,
                    Data = issueCollection.Select(o => o.GetByApi(
                        context: context,
                        ss: ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnEditing(
                context: context,
                ss: ss,
                issueModel: issueModel,
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
                mine: issueModel.Mine(context: context));
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = issueModel.GetByApi(
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
            var issueModel = new IssueModel(context, ss, 0, setByForm: true);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = issueModel.Create(context: context, ss: ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Created(
                            context: context,
                            data: issueModel.Title.DisplayValue));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            context: context,
                            controller: context.Controller,
                            id: ss.Columns.Any(o => o.Linking)
                                ? context.Forms.Long("LinkId")
                                : issueModel.IssueId))
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
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: 0,
                setByApi: true);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(context: context, ss: ss);
            var error = issueModel.Create(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Created(
                            context: context,
                            data: issueModel.Title.DisplayValue));
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

        public static string Update(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context, ss: ss, issueId: issueId, setByForm: true);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            var error = issueModel.Update(
                context: context,
                ss: ss,
                notice: true,
                permissions: context.Forms.List("CurrentPermissionsAll"),
                permissionChanged: context.Forms.Exists("CurrentPermissionsAll"));
            switch (error)
            {
                case Error.Types.None:
                    var res = new IssuesResponseCollection(issueModel);
                    res.Val(
                        "#Issues_RemainingWorkValue",
                        ss.GetColumn(context: context, columnName: "RemainingWorkValue")
                            .Display(
                                context: context,
                                ss: ss,
                                value: issueModel.RemainingWorkValue));
                    return ResponseByUpdate(res, context, ss, issueModel)
                        .PrependComment(
                            context: context,
                            ss: ss,
                            column: ss.GetColumn(context: context, columnName: "Comments"),
                            comments: issueModel.Comments,
                            verType: issueModel.VerType)
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
                        data: issueModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        private static ResponseCollection ResponseByUpdate(
            IssuesResponseCollection res,
            Context context,
            SiteSettings ss,
            IssueModel issueModel)
        {
            if (context.Forms.Bool("IsDialogEditorForm"))
            {
                var view = Views.GetBySession(
                    context: context,
                    ss: ss);
                var gridData = new GridData(
                    context: context,
                    ss: ss,
                    view: view,
                    where: Rds.IssuesWhere().IssueId(issueModel.IssueId));
                var columns = ss.GetGridColumns(
                    context: context,
                    checkPermission: true);
                return res
                    .ReplaceAll(
                        $"[data-id=\"{issueModel.IssueId}\"]",
                        gridData.TBody(
                            hb: new HtmlBuilder(),
                            context: context,
                            ss: ss,
                            columns: columns,
                            checkAll: false))
                    .CloseDialog()
                    .Message(Messages.Updated(
                        context: context,
                        data: issueModel.Title.DisplayValue));
            }
            else
            {
                return res
                    .Ver(context: context, ss: ss)
                    .Timestamp(context: context, ss: ss)
                    .FieldResponse(context: context, ss: ss, issueModel: issueModel)
                    .Val("#VerUp", false)
                    .Disabled("#VerUp", false)
                    .Html("#HeaderTitle", issueModel.Title.DisplayValue)
                    .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                        context: context,
                        baseModel: issueModel,
                        tableName: "Issues"))
                    .Html("#Links", new HtmlBuilder().Links(
                        context: context,
                        ss: ss,
                        id: issueModel.IssueId))
                    .SetMemory("formChanged", false)
                    .Message(Messages.Updated(
                        context: context,
                        data: issueModel.Title.DisplayValue))
                    .Comment(
                        context: context,
                        ss: ss,
                        column: ss.GetColumn(context: context, columnName: "Comments"),
                        comments: issueModel.Comments,
                        deleteCommentId: issueModel.DeleteCommentId)
                    .ClearFormData();
            }
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                setByApi: true);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(context: context, ss: ss);
            var error = issueModel.Update(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Updated(
                            context: context,
                            data: issueModel.Title.DisplayValue));
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

        public static string Copy(Context context, SiteSettings ss, long issueId)
        {
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context, ss: ss, issueId: issueId, setByForm: true);
            var invalid = IssueValidators.OnCreating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            issueModel.IssueId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                issueModel.Title.Value += Displays.SuffixCopy(context: context);
            }
            if (!context.Forms.Bool("CopyWithComments"))
            {
                issueModel.Comments.Clear();
            }
            ss.Columns
                .Where(column => column.CopyByDefault == true
                    || column.TypeCs == "Attachments")
                .ForEach(column => issueModel.SetDefault(
                    context: context,
                    ss: ss,
                    column: column));
            var error = issueModel.Create(
                context, ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    return EditorResponse(
                        context: context,
                        ss: ss,
                        issueModel: issueModel,
                        message: Messages.Copied(context: context),
                        switchTargets: GetSwitchTargets(
                            context: context,
                            ss: ss,
                            issueId: issueModel.IssueId,
                            siteId: issueModel.SiteId).Join())
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

        public static string Move(Context context, SiteSettings ss, long issueId)
        {
            var siteId = context.Forms.Long("MoveTargets");
            if (context.ContractSettings.ItemsLimit(context: context, siteId: siteId))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnMoving(
                context: context,
                source: ss,
                destination: SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: issueId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var targetSs = SiteSettingsUtilities.Get(context: context, siteId: siteId);
            var error = issueModel.Move(
                context: context,
                ss: ss,
                targetSs: targetSs);
            switch (error)
            {
                case Error.Types.None:
                    return new ResponseCollection()
                        .Href(Locations.ItemEdit(
                            context: context,
                            id: issueModel.IssueId))
                        .ToJson();
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

        public static string Delete(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(context, ss, issueId);
            var invalid = IssueValidators.OnDeleting(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var error = issueModel.Delete(context: context, ss: ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    SessionUtilities.Set(
                        context: context,
                        message: Messages.Deleted(
                            context: context,
                            data: issueModel.Title.Value));
                    var res = new IssuesResponseCollection(issueModel);
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
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId,
                methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            }
            var invalid = IssueValidators.OnDeleting(
                context: context,
                ss: ss,
                issueModel: issueModel,
                api: true);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(
                    context: context,
                    type: invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(context: context, ss: ss);
            var error = issueModel.Delete(
                context: context,
                ss: ss,
                notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Deleted(
                            context: context,
                            data: issueModel.Title.DisplayValue));
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
            var where = Rds.IssuesWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Issues_Deleted")
                .IssueId_In(
                    value: selected,
                    tableName: "Issues_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .IssueId_In(
                    tableName: "Issues_Deleted",
                    sub: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.IssuesColumn().IssueId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(context: context, ss: ss)));
            return Rds.ExecuteScalar_response(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreItems(where: Rds.ItemsWhere().ReferenceId_In(sub:
                        Rds.SelectIssues(
                            tableType: Sqls.TableTypes.Deleted,
                            _as: "Issues_Deleted",
                            column: Rds.IssuesColumn()
                                .IssueId(tableName: "Issues_Deleted"),
                            where: where))),
                    Rds.RestoreIssues(where: where, countRecord: true)
                }).Count.ToInt();
        }

        public static string RestoreFromHistory(
            Context context, SiteSettings ss, long issueId)
        {
            if (!Parameters.History.Restore)
            {
                return Error.Types.InvalidRequest.MessageJson(context: context);
            }
            var issueModel = new IssueModel(context, ss, issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
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
            issueModel.SetByModel(new IssueModel().Get(
                context: context,
                ss: ss,
                tableType: Sqls.TableTypes.History,
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId(issueId)
                    .Ver(ver.First())));
            issueModel.VerUp = true;
            var error = issueModel.Update(
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
                            id: issueId))
                        .ToJson();
                default:
                    return error.MessageJson(context: context);
            }
        }

        public static string Histories(
            Context context, SiteSettings ss, long issueId, Message message = null)
        {
            var issueModel = new IssueModel(context: context, ss: ss, issueId: issueId);
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
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
                                issueModel: issueModel)));
            return new IssuesResponseCollection(issueModel)
                .Html("#FieldSetHistories", hb)
                .Message(message)
                .ToJson();
        }

        private static void HistoriesTableBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<Column> columns,
            IssueModel issueModel)
        {
            new IssueCollection(
                context: context,
                ss: ss,
                column: HistoryColumn(columns),
                where: Rds.IssuesWhere().IssueId(issueModel.IssueId),
                orderBy: Rds.IssuesOrderBy().Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.NormalAndHistory)
                    .ForEach(issueModelHistory => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataAction("History")
                                .DataMethod("post")
                                .DataVer(issueModelHistory.Ver)
                                .DataLatest(1, _using:
                                    issueModelHistory.Ver == issueModel.Ver),
                            action: () =>
                            {
                                hb.Td(
                                    css: "grid-check-td",
                                    action: () => hb
                                        .CheckBox(
                                            controlCss: "grid-check",
                                            _checked: false,
                                            dataId: issueModelHistory.Ver.ToString(),
                                            _using: issueModelHistory.Ver < issueModel.Ver));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            issueModel: issueModelHistory));
                            }));
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.IssuesColumnCollection()
                .IssueId()
                .Ver();
            columns.ForEach(column => sqlColumn.IssuesColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(context: context, ss: ss, issueId: issueId);
            ss.SetColumnAccessControls(
                context: context,
                mine: issueModel.Mine(context: context));
            issueModel.Get(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .IssueId(issueModel.IssueId)
                    .Ver(context.Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            issueModel.VerType = context.Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(context, ss, issueModel).ToJson();
        }

        public static string EditSeparateSettings(
            Context context, SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            return new ResponseCollection()
                .Html(
                    "#SeparateSettingsDialog",
                    new HtmlBuilder().SeparateSettings(
                        context: context,
                        ss: ss,
                        title: issueModel.Title.Value,
                        workValue: issueModel.WorkValue.Value,
                        mine: issueModel.Mine(context: context)))
                .Invoke("separateSettings")
                .ToJson();
        }

        public static string Separate(Context context, SiteSettings ss, long issueId)
        {
            var number = context.Forms.Int("SeparateNumber");
            if (context.ContractSettings.ItemsLimit(context: context, siteId: ss.SiteId, number: number - 1))
            {
                return Error.Types.ItemsLimit.MessageJson(context: context);
            }
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: issueId);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (number >= 2)
            {
                var idHash = new Dictionary<int, long> { { 1, issueModel.IssueId } };
                var ver = issueModel.Ver;
                var timestampHash = new Dictionary<int, string> { { 1, issueModel.Timestamp } };
                var comments = issueModel.Comments.ToJson();
                for (var index = 2; index <= number; index++)
                {
                    issueModel.IssueId = 0;
                    issueModel.Create(
                        context: context,
                        ss: ss,
                        otherInitValue: true);
                    idHash.Add(index, issueModel.IssueId);
                    timestampHash.Add(index, issueModel.Timestamp);
                }
                var addCommentCollection = new List<string>();
                addCommentCollection.AddRange(idHash.Select(o => "[{0}]({1}{2})  ".Params(
                    context.Forms.Data("SeparateTitle_" + o.Key),
                    context.Server,
                    Locations.ItemEdit(
                        context: context,
                        id: o.Value))));
                var addComment = "[md]\n{0}  \n{1}".Params(
                    Displays.Separated(context: context),
                    addCommentCollection.Join("\n"));
                for (var index = number; index >= 1; index--)
                {
                    var source = index == 1;
                    issueModel.IssueId = idHash[index];
                    issueModel.Ver = source
                        ? ver
                        : 1;
                    issueModel.Timestamp = timestampHash[index];
                    issueModel.Title.Value = context.Forms.Data("SeparateTitle_" + index);
                    issueModel.WorkValue.Value = source
                        ? context.Forms.Decimal(
                            context: context,
                            key: "SourceWorkValue")
                        : context.Forms.Decimal(
                            context: context,
                            key: "SeparateWorkValue_" + index);
                    issueModel.Comments.Clear();
                    if (source || context.Forms.Bool("SeparateCopyWithComments"))
                    {
                        issueModel.Comments = comments.Deserialize<Comments>();
                    }
                    issueModel.Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: addComment);
                    issueModel.Update(
                        context: context,
                        ss: ss,
                        forceSynchronizeSourceSummary: true,
                        otherInitValue: true);
                }
                return EditorResponse(
                    context: context,
                    ss: ss,
                    issueModel: issueModel,
                    message: Messages.Separated(context: context)).ToJson();
            }
            else
            {
                return Messages.ResponseInvalidRequest(context: context).ToJson();
            }
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
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: ss.Join(context: context),
                    where: Views.GetBySession(context: context, ss: ss).Where(
                        context: context,
                        ss: ss,
                        where: Rds.IssuesWhere()
                            .SiteId(ss.SiteId)
                            .IssueId_In(
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
                    Rds.UpdateIssues(
                        where: Views.GetBySession(context: context, ss: ss).Where(
                            context: context,
                            ss: ss,
                            where: Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId_In(
                                    value: selector.Selected,
                                    negative: selector.All,
                                    _using: selector.Selected.Any())),
                        param: Rds.IssuesParam().SiteId(siteId),
                        countRecord: true),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectIssues(
                                    column: Rds.IssuesColumn().IssueId(),
                                    where: Rds.IssuesWhere().SiteId(siteId)))
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
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(
                        value: selected,
                        negative: negative,
                        _using: selected.Any()));
            var sub = Rds.SelectIssues(
                column: Rds.IssuesColumn().IssueId(),
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
            statements.Add(Rds.DeleteIssues(
                where: where, 
                countRecord: true));
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string DeleteHistory(Context context, SiteSettings ss, long issueId)
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
                        issueId: issueId,
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
                            issueId: issueId,
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
                    issueId: issueId,
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
            long issueId,
            List<int> selected,
            bool negative = false)
        {
            return Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: Sqls.TableTypes.History,
                    where: Rds.IssuesWhere()
                        .SiteId(
                            value: ss.SiteId,
                            tableName: "Issues_History")
                        .IssueId(
                            value: issueId,
                            tableName: "Issues_History")
                        .Ver_In(
                            value: selected,
                            tableName: "Issues_History",
                            negative: negative,
                            _using: selected.Any())
                        .IssueId_In(
                            tableName: "Issues_History",
                            sub: Rds.SelectIssues(
                                tableType: Sqls.TableTypes.History,
                                column: Rds.IssuesColumn().IssueId(),
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
            var where = Rds.IssuesWhere()
                .SiteId(
                    value: ss.SiteId,
                    tableName: "Issues_Deleted")
                .IssueId_In(
                    value: selected,
                    tableName: "Issues_Deleted",
                    negative: negative,
                    _using: selected.Any())
                .IssueId_In(
                    tableName: "Issues_Deleted",
                    sub: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        column: Rds.IssuesColumn().IssueId(),
                        where: Views.GetBySession(context: context, ss: ss).Where(
                            context: context, ss: ss)));
            var sub = Rds.SelectIssues(
                tableType: Sqls.TableTypes.Deleted,
                _as: "Issues_Deleted",
                column: Rds.IssuesColumn()
                    .IssueId(tableName: "Issues_Deleted"),
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
                    Rds.PhysicalDeleteIssues(
                        tableType: Sqls.TableTypes.Deleted,
                        where: where,
                        countRecord: true)
                }).Count.ToInt();
        }

        public static string Import(Context context, SiteModel siteModel)
        {
            var ss = siteModel.IssuesSiteSettings(
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
                    if (column?.ColumnName == "IssueId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var invalid = Imports.ColumnValidate(context, ss, columnHash.Values.Select(o => o.ColumnName), "CompletionTime");
                if (invalid != null) return invalid;
                Rds.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var issueHash = new Dictionary<int, IssueModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var issueModel = new IssueModel() { SiteId = ss.SiteId };
                    if (context.Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            issueModel = model;
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
                                issueModel.Title.Value = recordingData.ToString();
                                break;
                            case "Body":
                                issueModel.Body = recordingData.ToString();
                                break;
                            case "StartTime":
                                issueModel.StartTime = recordingData.ToDateTime();
                                break;
                            case "CompletionTime":
                                issueModel.CompletionTime.Value = recordingData.ToDateTime();
                                break;
                            case "WorkValue":
                                issueModel.WorkValue.Value = recordingData.ToDecimal();
                                break;
                            case "ProgressRate":
                                issueModel.ProgressRate.Value = recordingData.ToDecimal();
                                break;
                            case "Status":
                                issueModel.Status.Value = recordingData.ToInt();
                                break;
                            case "Manager":
                                issueModel.Manager.Id = recordingData.ToInt();
                                break;
                            case "Owner":
                                issueModel.Owner.Id = recordingData.ToInt();
                                break;
                            case "ClassA":
                                issueModel.ClassA = recordingData.ToString();
                                break;
                            case "ClassB":
                                issueModel.ClassB = recordingData.ToString();
                                break;
                            case "ClassC":
                                issueModel.ClassC = recordingData.ToString();
                                break;
                            case "ClassD":
                                issueModel.ClassD = recordingData.ToString();
                                break;
                            case "ClassE":
                                issueModel.ClassE = recordingData.ToString();
                                break;
                            case "ClassF":
                                issueModel.ClassF = recordingData.ToString();
                                break;
                            case "ClassG":
                                issueModel.ClassG = recordingData.ToString();
                                break;
                            case "ClassH":
                                issueModel.ClassH = recordingData.ToString();
                                break;
                            case "ClassI":
                                issueModel.ClassI = recordingData.ToString();
                                break;
                            case "ClassJ":
                                issueModel.ClassJ = recordingData.ToString();
                                break;
                            case "ClassK":
                                issueModel.ClassK = recordingData.ToString();
                                break;
                            case "ClassL":
                                issueModel.ClassL = recordingData.ToString();
                                break;
                            case "ClassM":
                                issueModel.ClassM = recordingData.ToString();
                                break;
                            case "ClassN":
                                issueModel.ClassN = recordingData.ToString();
                                break;
                            case "ClassO":
                                issueModel.ClassO = recordingData.ToString();
                                break;
                            case "ClassP":
                                issueModel.ClassP = recordingData.ToString();
                                break;
                            case "ClassQ":
                                issueModel.ClassQ = recordingData.ToString();
                                break;
                            case "ClassR":
                                issueModel.ClassR = recordingData.ToString();
                                break;
                            case "ClassS":
                                issueModel.ClassS = recordingData.ToString();
                                break;
                            case "ClassT":
                                issueModel.ClassT = recordingData.ToString();
                                break;
                            case "ClassU":
                                issueModel.ClassU = recordingData.ToString();
                                break;
                            case "ClassV":
                                issueModel.ClassV = recordingData.ToString();
                                break;
                            case "ClassW":
                                issueModel.ClassW = recordingData.ToString();
                                break;
                            case "ClassX":
                                issueModel.ClassX = recordingData.ToString();
                                break;
                            case "ClassY":
                                issueModel.ClassY = recordingData.ToString();
                                break;
                            case "ClassZ":
                                issueModel.ClassZ = recordingData.ToString();
                                break;
                            case "NumA":
                                issueModel.NumA = recordingData.ToDecimal();
                                break;
                            case "NumB":
                                issueModel.NumB = recordingData.ToDecimal();
                                break;
                            case "NumC":
                                issueModel.NumC = recordingData.ToDecimal();
                                break;
                            case "NumD":
                                issueModel.NumD = recordingData.ToDecimal();
                                break;
                            case "NumE":
                                issueModel.NumE = recordingData.ToDecimal();
                                break;
                            case "NumF":
                                issueModel.NumF = recordingData.ToDecimal();
                                break;
                            case "NumG":
                                issueModel.NumG = recordingData.ToDecimal();
                                break;
                            case "NumH":
                                issueModel.NumH = recordingData.ToDecimal();
                                break;
                            case "NumI":
                                issueModel.NumI = recordingData.ToDecimal();
                                break;
                            case "NumJ":
                                issueModel.NumJ = recordingData.ToDecimal();
                                break;
                            case "NumK":
                                issueModel.NumK = recordingData.ToDecimal();
                                break;
                            case "NumL":
                                issueModel.NumL = recordingData.ToDecimal();
                                break;
                            case "NumM":
                                issueModel.NumM = recordingData.ToDecimal();
                                break;
                            case "NumN":
                                issueModel.NumN = recordingData.ToDecimal();
                                break;
                            case "NumO":
                                issueModel.NumO = recordingData.ToDecimal();
                                break;
                            case "NumP":
                                issueModel.NumP = recordingData.ToDecimal();
                                break;
                            case "NumQ":
                                issueModel.NumQ = recordingData.ToDecimal();
                                break;
                            case "NumR":
                                issueModel.NumR = recordingData.ToDecimal();
                                break;
                            case "NumS":
                                issueModel.NumS = recordingData.ToDecimal();
                                break;
                            case "NumT":
                                issueModel.NumT = recordingData.ToDecimal();
                                break;
                            case "NumU":
                                issueModel.NumU = recordingData.ToDecimal();
                                break;
                            case "NumV":
                                issueModel.NumV = recordingData.ToDecimal();
                                break;
                            case "NumW":
                                issueModel.NumW = recordingData.ToDecimal();
                                break;
                            case "NumX":
                                issueModel.NumX = recordingData.ToDecimal();
                                break;
                            case "NumY":
                                issueModel.NumY = recordingData.ToDecimal();
                                break;
                            case "NumZ":
                                issueModel.NumZ = recordingData.ToDecimal();
                                break;
                            case "DateA":
                                issueModel.DateA = recordingData.ToDateTime();
                                break;
                            case "DateB":
                                issueModel.DateB = recordingData.ToDateTime();
                                break;
                            case "DateC":
                                issueModel.DateC = recordingData.ToDateTime();
                                break;
                            case "DateD":
                                issueModel.DateD = recordingData.ToDateTime();
                                break;
                            case "DateE":
                                issueModel.DateE = recordingData.ToDateTime();
                                break;
                            case "DateF":
                                issueModel.DateF = recordingData.ToDateTime();
                                break;
                            case "DateG":
                                issueModel.DateG = recordingData.ToDateTime();
                                break;
                            case "DateH":
                                issueModel.DateH = recordingData.ToDateTime();
                                break;
                            case "DateI":
                                issueModel.DateI = recordingData.ToDateTime();
                                break;
                            case "DateJ":
                                issueModel.DateJ = recordingData.ToDateTime();
                                break;
                            case "DateK":
                                issueModel.DateK = recordingData.ToDateTime();
                                break;
                            case "DateL":
                                issueModel.DateL = recordingData.ToDateTime();
                                break;
                            case "DateM":
                                issueModel.DateM = recordingData.ToDateTime();
                                break;
                            case "DateN":
                                issueModel.DateN = recordingData.ToDateTime();
                                break;
                            case "DateO":
                                issueModel.DateO = recordingData.ToDateTime();
                                break;
                            case "DateP":
                                issueModel.DateP = recordingData.ToDateTime();
                                break;
                            case "DateQ":
                                issueModel.DateQ = recordingData.ToDateTime();
                                break;
                            case "DateR":
                                issueModel.DateR = recordingData.ToDateTime();
                                break;
                            case "DateS":
                                issueModel.DateS = recordingData.ToDateTime();
                                break;
                            case "DateT":
                                issueModel.DateT = recordingData.ToDateTime();
                                break;
                            case "DateU":
                                issueModel.DateU = recordingData.ToDateTime();
                                break;
                            case "DateV":
                                issueModel.DateV = recordingData.ToDateTime();
                                break;
                            case "DateW":
                                issueModel.DateW = recordingData.ToDateTime();
                                break;
                            case "DateX":
                                issueModel.DateX = recordingData.ToDateTime();
                                break;
                            case "DateY":
                                issueModel.DateY = recordingData.ToDateTime();
                                break;
                            case "DateZ":
                                issueModel.DateZ = recordingData.ToDateTime();
                                break;
                            case "DescriptionA":
                                issueModel.DescriptionA = recordingData.ToString();
                                break;
                            case "DescriptionB":
                                issueModel.DescriptionB = recordingData.ToString();
                                break;
                            case "DescriptionC":
                                issueModel.DescriptionC = recordingData.ToString();
                                break;
                            case "DescriptionD":
                                issueModel.DescriptionD = recordingData.ToString();
                                break;
                            case "DescriptionE":
                                issueModel.DescriptionE = recordingData.ToString();
                                break;
                            case "DescriptionF":
                                issueModel.DescriptionF = recordingData.ToString();
                                break;
                            case "DescriptionG":
                                issueModel.DescriptionG = recordingData.ToString();
                                break;
                            case "DescriptionH":
                                issueModel.DescriptionH = recordingData.ToString();
                                break;
                            case "DescriptionI":
                                issueModel.DescriptionI = recordingData.ToString();
                                break;
                            case "DescriptionJ":
                                issueModel.DescriptionJ = recordingData.ToString();
                                break;
                            case "DescriptionK":
                                issueModel.DescriptionK = recordingData.ToString();
                                break;
                            case "DescriptionL":
                                issueModel.DescriptionL = recordingData.ToString();
                                break;
                            case "DescriptionM":
                                issueModel.DescriptionM = recordingData.ToString();
                                break;
                            case "DescriptionN":
                                issueModel.DescriptionN = recordingData.ToString();
                                break;
                            case "DescriptionO":
                                issueModel.DescriptionO = recordingData.ToString();
                                break;
                            case "DescriptionP":
                                issueModel.DescriptionP = recordingData.ToString();
                                break;
                            case "DescriptionQ":
                                issueModel.DescriptionQ = recordingData.ToString();
                                break;
                            case "DescriptionR":
                                issueModel.DescriptionR = recordingData.ToString();
                                break;
                            case "DescriptionS":
                                issueModel.DescriptionS = recordingData.ToString();
                                break;
                            case "DescriptionT":
                                issueModel.DescriptionT = recordingData.ToString();
                                break;
                            case "DescriptionU":
                                issueModel.DescriptionU = recordingData.ToString();
                                break;
                            case "DescriptionV":
                                issueModel.DescriptionV = recordingData.ToString();
                                break;
                            case "DescriptionW":
                                issueModel.DescriptionW = recordingData.ToString();
                                break;
                            case "DescriptionX":
                                issueModel.DescriptionX = recordingData.ToString();
                                break;
                            case "DescriptionY":
                                issueModel.DescriptionY = recordingData.ToString();
                                break;
                            case "DescriptionZ":
                                issueModel.DescriptionZ = recordingData.ToString();
                                break;
                            case "CheckA":
                                issueModel.CheckA = recordingData.ToBool();
                                break;
                            case "CheckB":
                                issueModel.CheckB = recordingData.ToBool();
                                break;
                            case "CheckC":
                                issueModel.CheckC = recordingData.ToBool();
                                break;
                            case "CheckD":
                                issueModel.CheckD = recordingData.ToBool();
                                break;
                            case "CheckE":
                                issueModel.CheckE = recordingData.ToBool();
                                break;
                            case "CheckF":
                                issueModel.CheckF = recordingData.ToBool();
                                break;
                            case "CheckG":
                                issueModel.CheckG = recordingData.ToBool();
                                break;
                            case "CheckH":
                                issueModel.CheckH = recordingData.ToBool();
                                break;
                            case "CheckI":
                                issueModel.CheckI = recordingData.ToBool();
                                break;
                            case "CheckJ":
                                issueModel.CheckJ = recordingData.ToBool();
                                break;
                            case "CheckK":
                                issueModel.CheckK = recordingData.ToBool();
                                break;
                            case "CheckL":
                                issueModel.CheckL = recordingData.ToBool();
                                break;
                            case "CheckM":
                                issueModel.CheckM = recordingData.ToBool();
                                break;
                            case "CheckN":
                                issueModel.CheckN = recordingData.ToBool();
                                break;
                            case "CheckO":
                                issueModel.CheckO = recordingData.ToBool();
                                break;
                            case "CheckP":
                                issueModel.CheckP = recordingData.ToBool();
                                break;
                            case "CheckQ":
                                issueModel.CheckQ = recordingData.ToBool();
                                break;
                            case "CheckR":
                                issueModel.CheckR = recordingData.ToBool();
                                break;
                            case "CheckS":
                                issueModel.CheckS = recordingData.ToBool();
                                break;
                            case "CheckT":
                                issueModel.CheckT = recordingData.ToBool();
                                break;
                            case "CheckU":
                                issueModel.CheckU = recordingData.ToBool();
                                break;
                            case "CheckV":
                                issueModel.CheckV = recordingData.ToBool();
                                break;
                            case "CheckW":
                                issueModel.CheckW = recordingData.ToBool();
                                break;
                            case "CheckX":
                                issueModel.CheckX = recordingData.ToBool();
                                break;
                            case "CheckY":
                                issueModel.CheckY = recordingData.ToBool();
                                break;
                            case "CheckZ":
                                issueModel.CheckZ = recordingData.ToBool();
                                break;
                            case "Comments":
                                if (issueModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    issueModel.Comments.Prepend(
                                        context: context,
                                        ss: ss,
                                        body: data.Row[column.Key]);
                                }
                                break;
                        }
                    });
                    issueHash.Add(data.Index, issueModel);
                });
                var errorCompletionTime = Imports.Validate(
                    context: context,
                    hash: issueHash.ToDictionary(
                        o => o.Key,
                        o => o.Value.CompletionTime.Value.ToString()),
                    column: ss.GetColumn(context: context, columnName: "CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                var insertCount = 0;
                var updateCount = 0;
                foreach (var issueModel in issueHash.Values)
                {
                    issueModel.SetByFormula(context: context, ss: ss);
                    issueModel.SetTitle(context: context, ss: ss);
                    if (issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        issueModel.VerUp = Versions.MustVerUp(
                            context: context, baseModel: issueModel);
                        if (issueModel.Updated(context: context))
                        {
                            var error = issueModel.Update(
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
                        var error = issueModel.Create(
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
            switch (column.ColumnName)
            {
                case "CompletionTime":
                    recordingData = recordingData
                        .ToDateTime()
                        .AddDifferenceOfDates(column.EditorFormat)
                        .ToString();
                    break;
            }
            return recordingData;
        }

        public static string OpenExportSelectorDialog(
            Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (context.ContractSettings.Export == false)
            {
                return HtmlTemplates.Error(context, Error.Types.InvalidRequest);
            }
            var invalid = IssueValidators.OnExporting(
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
            var invalid = IssueValidators.OnExporting(
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
            var invalid = IssueValidators.OnExporting(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
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
                value = ss.GetColumn(context: context, columnName: "IssueId");
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
            var gridData = GetGridData(
                context: context, ss: ss, view: view);
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
                gridData: gridData,
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
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: context.Forms.Long("Id"),
                setByForm: true);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: issueModel);
            issueModel.Update(
                context: context,
                ss: ss,
                notice: true);
            return CalendarJson(
                context: context,
                ss: ss,
                changedItemId: issueModel.IssueId,
                update: true,
                message: Messages.Updated(
                    context: context,
                    data: issueModel.Title.DisplayValue));
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
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
                        gridData: gridData,
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
                        gridData: gridData,
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
            var where = Rds.IssuesWhere();
            if (toColumn == null)
            {
                where.Add(
                    raw: $"[Issues].[{fromColumn.ColumnName}] between '{begin}' and '{end}'");
            }
            else
            {
                where.Or(or: Rds.IssuesWhere()
                    .Add(raw: $"[Issues].[{fromColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Issues].[{toColumn.ColumnName}] between '{begin}' and '{end}'")
                    .Add(raw: $"[Issues].[{fromColumn.ColumnName}]<='{begin}' and [Issues].[{toColumn.ColumnName}]>='{end}'"));
            }
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(context: context, ss: ss)
                        .IssueId(_as: "Id")
                        .IssuesColumn(fromColumn.ColumnName, _as: "From")
                        .IssuesColumn(toColumn?.ColumnName, _as: "To")
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
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
                value = ss.GetColumn(context: context, columnName: "IssueId");
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
                gridData: gridData,
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
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
                value = ss.GetColumn(context: context, columnName: "IssueId");
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
                        gridData: gridData,
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
                        gridData: gridData,
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
                    statements: Rds.SelectIssues(
                        column: Rds.IssuesColumn()
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
                        groupBy: Rds.IssuesGroupBy()
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
                    statements: Rds.SelectIssues(
                        column: Rds.IssuesColumn()
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
                        groupBy: Rds.IssuesGroupBy()
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

        public static string Gantt(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Gantt"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttSortBy());
            var range = new Libraries.ViewModes.GanttRange(
                context: context,
                ss: ss,
                view: view);
            var dataRows = GanttDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                sortBy: sortBy);
            var inRange = dataRows.Count() <= Parameters.General.GanttLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.GanttLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Gantt(
                        context: context,
                        ss: ss,
                        view: view,
                        dataRows: dataRows,
                        groupBy: groupBy,
                        sortBy: sortBy,
                        period: view.GanttPeriod.ToInt(),
                        startDate: view.GanttStartDate.ToDateTime(),
                        range: range,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string GanttJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "Gantt"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var bodyOnly = context.Forms.ControlId().StartsWith("Gantt");
            var range = new Libraries.ViewModes.GanttRange(
                context: context,
                ss: ss,
                view: view);
            var groupBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(
                context: context,
                columnName: view.GetGanttSortBy());
            var period = view.GanttPeriod.ToInt();
            var startDate = view.GanttStartDate.ToDateTime();
            var dataRows = GanttDataRows(
                context: context,
                ss: ss,
                view: view,
                groupBy: groupBy,
                sortBy: sortBy);
            if (dataRows.Count() <= Parameters.General.GanttLimit)
            {
                return new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "drawGantt",
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
                                context: context,
                                ss: ss,
                                view: view,
                                dataRows: dataRows,
                                groupBy: groupBy,
                                sortBy: sortBy,
                                period: period,
                                startDate: startDate,
                                range: range,
                                bodyOnly: bodyOnly,
                                inRange: true))
                    .ToJson();
            }
            else
            {
                return new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.GanttLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
                                context: context,
                                ss: ss,
                                view: view,
                                dataRows: dataRows,
                                groupBy: groupBy,
                                sortBy: sortBy,
                                period: view.GanttPeriod.ToInt(),
                                startDate: view.GanttStartDate.ToDateTime(),
                                range: range,
                                bodyOnly: bodyOnly,
                                inRange: false))
                    .ToJson();
            }
        }

        private static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            EnumerableRowCollection<DataRow> dataRows,
            Column groupBy,
            Column sortBy,
            int period,
            DateTime startDate,
            Libraries.ViewModes.GanttRange range,
            bool bodyOnly,
            bool inRange = true)
        {
            return !bodyOnly
                ? hb.Gantt(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    period: period,
                    startDate: startDate,
                    range: range,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.GanttBody(
                    context: context,
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    period: period,
                    startDate: startDate,
                    range: range,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> GanttDataRows(
            Context context, SiteSettings ss, View view, Column groupBy, Column sortBy)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(context: context, ss: ss)
                        .IssueId()
                        .WorkValue()
                        .StartTime()
                        .CompletionTime()
                        .ProgressRate()
                        .Status()
                        .Owner()
                        .Updator()
                        .CreatedTime()
                        .UpdatedTime()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType))
                        .Add(
                            ss: ss,
                            column: groupBy,
                            function: Sqls.Functions.SingleColumn)
                        .Add(
                            ss: ss,
                            column: sortBy,
                            function: Sqls.Functions.SingleColumn),
                    join: ss.Join(context: context),
                    where: view.Where(
                        context: context,
                        ss: ss,
                        where: Libraries.ViewModes.GanttUtilities.Where(
                            context: context, ss: ss, view: view))))
                                .AsEnumerable();
        }

        public static string BurnDown(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "BurnDown"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.BurnDownLimit;
            if (!inRange)
            {
                SessionUtilities.Set(
                    context: context,
                    message: Messages.TooManyCases(
                        context: context,
                        data: Parameters.General.BurnDownLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.BurnDown(
                            context: context,
                            ss: ss,
                            dataRows: BurnDownDataRows(context: context, ss: ss, view: view),
                            ownerLabelText: ss.GetColumn(context: context, columnName: "Owner").GridLabelText,
                            column: ss.GetColumn(context: context, columnName: "WorkValue"));
                    }
                });
        }

        public static string BurnDownJson(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "BurnDown"))
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            return gridData.Aggregations.TotalCount <= Parameters.General.BurnDownLimit
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "drawBurnDown",
                        body: new HtmlBuilder()
                            .BurnDown(
                                context: context,
                                ss: ss,
                                dataRows: BurnDownDataRows(context: context, ss: ss, view: view),
                                ownerLabelText: ss.GetColumn(context: context, columnName: "Owner").GridLabelText,
                                column: ss.GetColumn(context: context, columnName: "WorkValue")))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            context: context,
                            data: Parameters.General.BurnDownLimit.ToString()),
                        body: new HtmlBuilder())
                    .ToJson();
        }

        public static string BurnDownRecordDetails(Context context, SiteSettings ss)
        {
            var date = context.Forms.DateTime("BurnDownDate");
            return new ResponseCollection()
                .After(string.Empty, new HtmlBuilder().BurnDownRecordDetails(
                    context: context,
                    elements: new Libraries.ViewModes.BurnDown(
                        context: context,
                        ss: ss,
                        dataRows: BurnDownDataRows(
                            context: context,
                            ss: ss,
                            view: Views.GetBySession(context: context, ss: ss)))
                                .Where(o => o.UpdatedTime == date),
                    progressRateColumn: ss.GetColumn(context: context, columnName: "ProgressRate"),
                    statusColumn: ss.GetColumn(context: context, columnName: "Status"),
                    colspan: context.Forms.Int("BurnDownColspan"),
                    unit: ss.GetColumn(context: context, columnName: "WorkValue").Unit)).ToJson();
        }

        private static EnumerableRowCollection<DataRow> BurnDownDataRows(
            Context context, SiteSettings ss, View view)
        {
            var where = view.Where(context: context, ss: ss);
            var join = ss.Join(context: context);
            return Rds.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectIssues(
                        column: Rds.IssuesTitleColumn(context: context, ss: ss)
                            .IssueId()
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: join,
                        where: where),
                    Rds.SelectIssues(
                        unionType: Sqls.UnionTypes.Union,
                        tableType: Sqls.TableTypes.HistoryWithoutFlag,
                        column: Rds.IssuesTitleColumn(context: context, ss: ss)
                            .IssueId(_as: "Id")
                            .Ver()
                            .Title()
                            .WorkValue()
                            .StartTime()
                            .CompletionTime()
                            .ProgressRate()
                            .Status()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: join,
                        where: Rds.IssuesWhere()
                            .IssueId_In(sub: Rds.SelectIssues(
                                column: Rds.IssuesColumn().IssueId(),
                                where: where)),
                        orderBy: Rds.IssuesOrderBy()
                            .IssueId()
                            .Ver())
                }).AsEnumerable();
        }

        public static string TimeSeries(Context context, SiteSettings ss)
        {
            if (!ss.EnableViewMode(context: context, name: "TimeSeries"))
            {
                return HtmlTemplates.Error(context, Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(context: context, ss: ss);
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.TimeSeriesLimit;
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
                gridData: gridData,
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var bodyOnly = context.Forms.ControlId().StartsWith("TimeSeries");
            return gridData.Aggregations.TotalCount <= Parameters.General.TimeSeriesLimit
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
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
                        gridData: gridData,
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
                    statements: Rds.SelectIssues(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.IssuesColumn()
                            .IssueId(_as: "Id")
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.KambanLimit;
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
                gridData: gridData,
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var bodyOnly = context.Forms.ControlId().StartsWith("Kamban");
            return gridData.Aggregations.TotalCount <= Parameters.General.KambanLimit
                ? new ResponseCollection()
                    .ViewMode(
                        context: context,
                        ss: ss,
                        view: view,
                        gridData: gridData,
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
                        gridData: gridData,
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
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .IssueId()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType))
                        .Add(ss: ss, column: groupByX)
                        .Add(ss: ss, column: groupByY)
                        .Add(ss: ss, column: value),
                    where: view.Where(context: context, ss: ss)))
                        .AsEnumerable()
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Long("IssueId"),
                            Title = o.String("ItemTitle"),
                            GroupX = groupByX.ConvertIfUserColumn(o),
                            GroupY = groupByY?.ConvertIfUserColumn(o),
                            Value = o.Decimal(value.ColumnName)
                        });
        }

        public static string UpdateByKamban(Context context, SiteSettings ss)
        {
            var issueModel = new IssueModel(
                context: context,
                ss: ss,
                issueId: context.Forms.Long("KambanId"),
                setByForm: true);
            var invalid = IssueValidators.OnUpdating(
                context: context,
                ss: ss,
                issueModel: issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts(context: context).ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(
                context: context, baseModel: issueModel);
            issueModel.Update(
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var viewMode = ViewModes.GetSessionData(
                context: context,
                siteId: ss.SiteId);
            return hb.ViewModeTemplate(
                context: context,
                ss: ss,
                gridData: gridData,
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
            var gridData = GetGridData(context: context, ss: ss, view: view);
            var bodyOnly = context.Forms.ControlId().StartsWith("ImageLib");
            return new ResponseCollection()
                .ViewMode(
                    context: context,
                    ss: ss,
                    view: view,
                    gridData: gridData,
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
            this List<IssueModel> issues, Context context, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks(context: context);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    context: context,
                    columnName: link.ColumnName,
                    selectedValues: issues
                        .Select(o => o.PropertyValue(
                            context: context,
                            name: link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issues.ForEach(issueModel => issueModel
                    .SetTitle(context: context, ss: ss));
            }
        }
    }
}
