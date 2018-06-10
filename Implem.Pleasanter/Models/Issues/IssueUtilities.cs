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
        public static string Index(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Grid(
                   gridData: gridData,
                   ss: ss,
                   view: view));
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view,
            string viewMode,
            Action viewModeBody)
        {
            var invalid = IssueValidators.OnEntry(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Issues",
                script: JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(Routes.Action()),
                userStyle: ss.ViewModeStyles(Routes.Action()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("IssuesForm")
                            .Class("main-form")
                            .Action(Locations.ItemAction(ss.SiteId)),
                        action: () => hb
                            .ViewSelector(ss: ss, view: view)
                            .ViewFilters(ss: ss, view: view)
                            .Aggregations(
                                ss: ss,
                                aggregations: gridData.Aggregations)
                            .Div(id: "ViewModeContainer", action: () => viewModeBody())
                            .MainCommands(
                                ss: ss,
                                siteId: ss.SiteId,
                                verType: Versions.VerTypes.Latest)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Issues")
                            .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl()))
                    .DropDownSearchDialog("items", ss.SiteId)
                    .MoveDialog(bulk: true)
                    .ImportSettingsDialog()
                    .Div(attributes: new HtmlAttributes()
                        .Id("ExportSelectorDialog")
                        .Class("dialog")
                        .Title(Displays.Export())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            return new ResponseCollection()
                .ViewMode(
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setGrid",
                    body: new HtmlBuilder()
                        .Grid(
                            ss: ss,
                            gridData: gridData,
                            view: view))
                .ToJson();
        }

        private static GridData GetGridData(SiteSettings ss, View view, int offset = 0)
        {
            ss.SetColumnAccessControls();
            return new GridData(
                ss: ss,
                view: view,
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregations: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            GridData gridData,
            View view)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("Grid")
                        .Class("grid")
                        .DataValue("back", _using: ss?.IntegratedSites?.Any() == true)
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            ss: ss,
                            gridData: gridData,
                            view: view))
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
                    action: "GridRows",
                    method: "post")
                .Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: "GridRows",
                    method: "post");
        }

        public static string GridRows(
            SiteSettings ss,
            ResponseCollection res = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view, offset: offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .CloseDialog()
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: gridData.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    gridData: gridData,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
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
            SiteSettings ss,
            GridData gridData,
            View view,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(checkPermission: true);
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columns: columns, 
                            view: view,
                            checkAll: checkAll))
                .TBody(action: () => gridData.TBody(
                    hb: hb,
                    ss: ss,
                    columns: columns,
                    checkAll: checkAll));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.IssuesColumn();
            new List<string> { "SiteId", "IssueId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.IssuesColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.IssuesColumn();
            new List<string> { "SiteId", "IssueId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.IssuesColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, IssueModel issueModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    issueModel: issueModel);
            }
            else
            {
                var mine = issueModel.Mine();
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.SiteId)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "IssueId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.IssueId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Title)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.TitleBody)
                            : hb.Td(column: column, value: string.Empty);
                    case "StartTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.StartTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "CompletionTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CompletionTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "WorkValue":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.WorkValue)
                            : hb.Td(column: column, value: string.Empty);
                    case "ProgressRate":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ProgressRate)
                            : hb.Td(column: column, value: string.Empty);
                    case "RemainingWorkValue":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.RemainingWorkValue)
                            : hb.Td(column: column, value: string.Empty);
                    case "Status":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Status)
                            : hb.Td(column: column, value: string.Empty);
                    case "Manager":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Manager)
                            : hb.Td(column: column, value: string.Empty);
                    case "Owner":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Owner)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassA)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassB)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassC)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassD)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassE)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassF)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassG)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassH)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassI)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassK)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassL)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassM)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassN)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassO)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassP)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassR)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassS)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassT)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassU)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassV)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassW)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassX)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassY)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.ClassZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumA)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumB)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumC)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumD)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumE)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumF)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumG)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumH)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumI)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumK)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumL)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumM)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumN)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumO)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumP)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumR)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumS)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumT)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumU)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumV)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumW)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumX)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumY)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.NumZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DateZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.DescriptionZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckA)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckB)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckC)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckD)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckE)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckF)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckG)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckH)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckI)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckK)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckL)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckM)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckN)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckO)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckP)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckR)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckS)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckT)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckU)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckV)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckW)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckX)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckY)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CheckZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsA)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsB)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsC)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsD)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsE)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsF)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsG)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsH)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsI)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsK)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsL)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsM)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsN)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsO)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsP)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsR)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsS)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsT)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsU)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsV)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsW)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsX)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsY)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.AttachmentsZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "SiteTitle":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.SiteTitle)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: issueModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, IssueModel issueModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = issueModel.SiteId.GridText(column: column); break;
                    case "UpdatedTime": value = issueModel.UpdatedTime.GridText(column: column); break;
                    case "IssueId": value = issueModel.IssueId.GridText(column: column); break;
                    case "Ver": value = issueModel.Ver.GridText(column: column); break;
                    case "Title": value = issueModel.Title.GridText(column: column); break;
                    case "Body": value = issueModel.Body.GridText(column: column); break;
                    case "TitleBody": value = issueModel.TitleBody.GridText(column: column); break;
                    case "StartTime": value = issueModel.StartTime.GridText(column: column); break;
                    case "CompletionTime": value = issueModel.CompletionTime.GridText(column: column); break;
                    case "WorkValue": value = issueModel.WorkValue.GridText(column: column); break;
                    case "ProgressRate": value = issueModel.ProgressRate.GridText(column: column); break;
                    case "RemainingWorkValue": value = issueModel.RemainingWorkValue.GridText(column: column); break;
                    case "Status": value = issueModel.Status.GridText(column: column); break;
                    case "Manager": value = issueModel.Manager.GridText(column: column); break;
                    case "Owner": value = issueModel.Owner.GridText(column: column); break;
                    case "ClassA": value = issueModel.ClassA.GridText(column: column); break;
                    case "ClassB": value = issueModel.ClassB.GridText(column: column); break;
                    case "ClassC": value = issueModel.ClassC.GridText(column: column); break;
                    case "ClassD": value = issueModel.ClassD.GridText(column: column); break;
                    case "ClassE": value = issueModel.ClassE.GridText(column: column); break;
                    case "ClassF": value = issueModel.ClassF.GridText(column: column); break;
                    case "ClassG": value = issueModel.ClassG.GridText(column: column); break;
                    case "ClassH": value = issueModel.ClassH.GridText(column: column); break;
                    case "ClassI": value = issueModel.ClassI.GridText(column: column); break;
                    case "ClassJ": value = issueModel.ClassJ.GridText(column: column); break;
                    case "ClassK": value = issueModel.ClassK.GridText(column: column); break;
                    case "ClassL": value = issueModel.ClassL.GridText(column: column); break;
                    case "ClassM": value = issueModel.ClassM.GridText(column: column); break;
                    case "ClassN": value = issueModel.ClassN.GridText(column: column); break;
                    case "ClassO": value = issueModel.ClassO.GridText(column: column); break;
                    case "ClassP": value = issueModel.ClassP.GridText(column: column); break;
                    case "ClassQ": value = issueModel.ClassQ.GridText(column: column); break;
                    case "ClassR": value = issueModel.ClassR.GridText(column: column); break;
                    case "ClassS": value = issueModel.ClassS.GridText(column: column); break;
                    case "ClassT": value = issueModel.ClassT.GridText(column: column); break;
                    case "ClassU": value = issueModel.ClassU.GridText(column: column); break;
                    case "ClassV": value = issueModel.ClassV.GridText(column: column); break;
                    case "ClassW": value = issueModel.ClassW.GridText(column: column); break;
                    case "ClassX": value = issueModel.ClassX.GridText(column: column); break;
                    case "ClassY": value = issueModel.ClassY.GridText(column: column); break;
                    case "ClassZ": value = issueModel.ClassZ.GridText(column: column); break;
                    case "NumA": value = issueModel.NumA.GridText(column: column); break;
                    case "NumB": value = issueModel.NumB.GridText(column: column); break;
                    case "NumC": value = issueModel.NumC.GridText(column: column); break;
                    case "NumD": value = issueModel.NumD.GridText(column: column); break;
                    case "NumE": value = issueModel.NumE.GridText(column: column); break;
                    case "NumF": value = issueModel.NumF.GridText(column: column); break;
                    case "NumG": value = issueModel.NumG.GridText(column: column); break;
                    case "NumH": value = issueModel.NumH.GridText(column: column); break;
                    case "NumI": value = issueModel.NumI.GridText(column: column); break;
                    case "NumJ": value = issueModel.NumJ.GridText(column: column); break;
                    case "NumK": value = issueModel.NumK.GridText(column: column); break;
                    case "NumL": value = issueModel.NumL.GridText(column: column); break;
                    case "NumM": value = issueModel.NumM.GridText(column: column); break;
                    case "NumN": value = issueModel.NumN.GridText(column: column); break;
                    case "NumO": value = issueModel.NumO.GridText(column: column); break;
                    case "NumP": value = issueModel.NumP.GridText(column: column); break;
                    case "NumQ": value = issueModel.NumQ.GridText(column: column); break;
                    case "NumR": value = issueModel.NumR.GridText(column: column); break;
                    case "NumS": value = issueModel.NumS.GridText(column: column); break;
                    case "NumT": value = issueModel.NumT.GridText(column: column); break;
                    case "NumU": value = issueModel.NumU.GridText(column: column); break;
                    case "NumV": value = issueModel.NumV.GridText(column: column); break;
                    case "NumW": value = issueModel.NumW.GridText(column: column); break;
                    case "NumX": value = issueModel.NumX.GridText(column: column); break;
                    case "NumY": value = issueModel.NumY.GridText(column: column); break;
                    case "NumZ": value = issueModel.NumZ.GridText(column: column); break;
                    case "DateA": value = issueModel.DateA.GridText(column: column); break;
                    case "DateB": value = issueModel.DateB.GridText(column: column); break;
                    case "DateC": value = issueModel.DateC.GridText(column: column); break;
                    case "DateD": value = issueModel.DateD.GridText(column: column); break;
                    case "DateE": value = issueModel.DateE.GridText(column: column); break;
                    case "DateF": value = issueModel.DateF.GridText(column: column); break;
                    case "DateG": value = issueModel.DateG.GridText(column: column); break;
                    case "DateH": value = issueModel.DateH.GridText(column: column); break;
                    case "DateI": value = issueModel.DateI.GridText(column: column); break;
                    case "DateJ": value = issueModel.DateJ.GridText(column: column); break;
                    case "DateK": value = issueModel.DateK.GridText(column: column); break;
                    case "DateL": value = issueModel.DateL.GridText(column: column); break;
                    case "DateM": value = issueModel.DateM.GridText(column: column); break;
                    case "DateN": value = issueModel.DateN.GridText(column: column); break;
                    case "DateO": value = issueModel.DateO.GridText(column: column); break;
                    case "DateP": value = issueModel.DateP.GridText(column: column); break;
                    case "DateQ": value = issueModel.DateQ.GridText(column: column); break;
                    case "DateR": value = issueModel.DateR.GridText(column: column); break;
                    case "DateS": value = issueModel.DateS.GridText(column: column); break;
                    case "DateT": value = issueModel.DateT.GridText(column: column); break;
                    case "DateU": value = issueModel.DateU.GridText(column: column); break;
                    case "DateV": value = issueModel.DateV.GridText(column: column); break;
                    case "DateW": value = issueModel.DateW.GridText(column: column); break;
                    case "DateX": value = issueModel.DateX.GridText(column: column); break;
                    case "DateY": value = issueModel.DateY.GridText(column: column); break;
                    case "DateZ": value = issueModel.DateZ.GridText(column: column); break;
                    case "DescriptionA": value = issueModel.DescriptionA.GridText(column: column); break;
                    case "DescriptionB": value = issueModel.DescriptionB.GridText(column: column); break;
                    case "DescriptionC": value = issueModel.DescriptionC.GridText(column: column); break;
                    case "DescriptionD": value = issueModel.DescriptionD.GridText(column: column); break;
                    case "DescriptionE": value = issueModel.DescriptionE.GridText(column: column); break;
                    case "DescriptionF": value = issueModel.DescriptionF.GridText(column: column); break;
                    case "DescriptionG": value = issueModel.DescriptionG.GridText(column: column); break;
                    case "DescriptionH": value = issueModel.DescriptionH.GridText(column: column); break;
                    case "DescriptionI": value = issueModel.DescriptionI.GridText(column: column); break;
                    case "DescriptionJ": value = issueModel.DescriptionJ.GridText(column: column); break;
                    case "DescriptionK": value = issueModel.DescriptionK.GridText(column: column); break;
                    case "DescriptionL": value = issueModel.DescriptionL.GridText(column: column); break;
                    case "DescriptionM": value = issueModel.DescriptionM.GridText(column: column); break;
                    case "DescriptionN": value = issueModel.DescriptionN.GridText(column: column); break;
                    case "DescriptionO": value = issueModel.DescriptionO.GridText(column: column); break;
                    case "DescriptionP": value = issueModel.DescriptionP.GridText(column: column); break;
                    case "DescriptionQ": value = issueModel.DescriptionQ.GridText(column: column); break;
                    case "DescriptionR": value = issueModel.DescriptionR.GridText(column: column); break;
                    case "DescriptionS": value = issueModel.DescriptionS.GridText(column: column); break;
                    case "DescriptionT": value = issueModel.DescriptionT.GridText(column: column); break;
                    case "DescriptionU": value = issueModel.DescriptionU.GridText(column: column); break;
                    case "DescriptionV": value = issueModel.DescriptionV.GridText(column: column); break;
                    case "DescriptionW": value = issueModel.DescriptionW.GridText(column: column); break;
                    case "DescriptionX": value = issueModel.DescriptionX.GridText(column: column); break;
                    case "DescriptionY": value = issueModel.DescriptionY.GridText(column: column); break;
                    case "DescriptionZ": value = issueModel.DescriptionZ.GridText(column: column); break;
                    case "CheckA": value = issueModel.CheckA.GridText(column: column); break;
                    case "CheckB": value = issueModel.CheckB.GridText(column: column); break;
                    case "CheckC": value = issueModel.CheckC.GridText(column: column); break;
                    case "CheckD": value = issueModel.CheckD.GridText(column: column); break;
                    case "CheckE": value = issueModel.CheckE.GridText(column: column); break;
                    case "CheckF": value = issueModel.CheckF.GridText(column: column); break;
                    case "CheckG": value = issueModel.CheckG.GridText(column: column); break;
                    case "CheckH": value = issueModel.CheckH.GridText(column: column); break;
                    case "CheckI": value = issueModel.CheckI.GridText(column: column); break;
                    case "CheckJ": value = issueModel.CheckJ.GridText(column: column); break;
                    case "CheckK": value = issueModel.CheckK.GridText(column: column); break;
                    case "CheckL": value = issueModel.CheckL.GridText(column: column); break;
                    case "CheckM": value = issueModel.CheckM.GridText(column: column); break;
                    case "CheckN": value = issueModel.CheckN.GridText(column: column); break;
                    case "CheckO": value = issueModel.CheckO.GridText(column: column); break;
                    case "CheckP": value = issueModel.CheckP.GridText(column: column); break;
                    case "CheckQ": value = issueModel.CheckQ.GridText(column: column); break;
                    case "CheckR": value = issueModel.CheckR.GridText(column: column); break;
                    case "CheckS": value = issueModel.CheckS.GridText(column: column); break;
                    case "CheckT": value = issueModel.CheckT.GridText(column: column); break;
                    case "CheckU": value = issueModel.CheckU.GridText(column: column); break;
                    case "CheckV": value = issueModel.CheckV.GridText(column: column); break;
                    case "CheckW": value = issueModel.CheckW.GridText(column: column); break;
                    case "CheckX": value = issueModel.CheckX.GridText(column: column); break;
                    case "CheckY": value = issueModel.CheckY.GridText(column: column); break;
                    case "CheckZ": value = issueModel.CheckZ.GridText(column: column); break;
                    case "AttachmentsA": value = issueModel.AttachmentsA.GridText(column: column); break;
                    case "AttachmentsB": value = issueModel.AttachmentsB.GridText(column: column); break;
                    case "AttachmentsC": value = issueModel.AttachmentsC.GridText(column: column); break;
                    case "AttachmentsD": value = issueModel.AttachmentsD.GridText(column: column); break;
                    case "AttachmentsE": value = issueModel.AttachmentsE.GridText(column: column); break;
                    case "AttachmentsF": value = issueModel.AttachmentsF.GridText(column: column); break;
                    case "AttachmentsG": value = issueModel.AttachmentsG.GridText(column: column); break;
                    case "AttachmentsH": value = issueModel.AttachmentsH.GridText(column: column); break;
                    case "AttachmentsI": value = issueModel.AttachmentsI.GridText(column: column); break;
                    case "AttachmentsJ": value = issueModel.AttachmentsJ.GridText(column: column); break;
                    case "AttachmentsK": value = issueModel.AttachmentsK.GridText(column: column); break;
                    case "AttachmentsL": value = issueModel.AttachmentsL.GridText(column: column); break;
                    case "AttachmentsM": value = issueModel.AttachmentsM.GridText(column: column); break;
                    case "AttachmentsN": value = issueModel.AttachmentsN.GridText(column: column); break;
                    case "AttachmentsO": value = issueModel.AttachmentsO.GridText(column: column); break;
                    case "AttachmentsP": value = issueModel.AttachmentsP.GridText(column: column); break;
                    case "AttachmentsQ": value = issueModel.AttachmentsQ.GridText(column: column); break;
                    case "AttachmentsR": value = issueModel.AttachmentsR.GridText(column: column); break;
                    case "AttachmentsS": value = issueModel.AttachmentsS.GridText(column: column); break;
                    case "AttachmentsT": value = issueModel.AttachmentsT.GridText(column: column); break;
                    case "AttachmentsU": value = issueModel.AttachmentsU.GridText(column: column); break;
                    case "AttachmentsV": value = issueModel.AttachmentsV.GridText(column: column); break;
                    case "AttachmentsW": value = issueModel.AttachmentsW.GridText(column: column); break;
                    case "AttachmentsX": value = issueModel.AttachmentsX.GridText(column: column); break;
                    case "AttachmentsY": value = issueModel.AttachmentsY.GridText(column: column); break;
                    case "AttachmentsZ": value = issueModel.AttachmentsZ.GridText(column: column); break;
                    case "SiteTitle": value = issueModel.SiteTitle.GridText(column: column); break;
                    case "Comments": value = issueModel.Comments.GridText(column: column); break;
                    case "Creator": value = issueModel.Creator.GridText(column: column); break;
                    case "Updator": value = issueModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = issueModel.CreatedTime.GridText(column: column); break;
                }
                gridDesign = gridDesign.Replace("[" + column.ColumnName + "]", value);
            });
            return hb.Td(action: () => hb
                .Div(css: "markup", action: () => hb
                    .Text(text: gridDesign)));
        }

        public static string EditorNew(SiteSettings ss)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return HtmlTemplates.Error(Error.Types.ItemsLimit);
            }
            return Editor(ss, new IssueModel(ss, methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, long issueId, bool clearSessions)
        {
            var issueModel = new IssueModel(
                ss: ss,
                issueId: issueId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            issueModel.SwitchTargets = GetSwitchTargets(
                ss, issueModel.IssueId, issueModel.SiteId);
            return Editor(ss, issueModel);
        }

        public static string Editor(SiteSettings ss, IssueModel issueModel)
        {
            var invalid = IssueValidators.OnEditing(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(issueModel.Mine());
            return hb.Template(
                ss: ss,
                verType: issueModel.VerType,
                methodType: issueModel.MethodType,
                siteId: issueModel.SiteId,
                parentId: ss.ParentId,
                referenceType: "Issues",
                title: issueModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.New()
                    : issueModel.Title.DisplayValue,
                useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                userScript: ss.EditorScripts(issueModel.MethodType),
                userStyle: ss.EditorStyles(issueModel.MethodType),
                action: () => hb
                    .Editor(
                        ss: ss,
                        issueModel: issueModel)
                    .Hidden(controlId: "TableName", value: "Issues")
                    .Hidden(controlId: "Id", value: issueModel.IssueId.ToString()))
                        .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("IssueForm")
                        .Class("main-form confirm-reload")
                        .Action(Locations.ItemAction(issueModel.IssueId != 0 
                            ? issueModel.IssueId
                            : issueModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: issueModel,
                            tableName: "Issues")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: issueModel.Comments,
                                    column: commentsColumn,
                                    verType: issueModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(issueModel: issueModel, ss: ss)
                            .FieldSetGeneral(
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
                                _using: ss.CanManagePermission() &&
                                    issueModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
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
                                        issueModel: issueModel,
                                        ss: ss)))
                        .Hidden(controlId: "BaseUrl", value: Locations.BaseUrl())
                        .Hidden(
                            controlId: "FromSiteId",
                            css: "control-hidden always-send",
                            value: QueryStrings.Data("FromSiteId"),
                            _using: QueryStrings.Long("FromSiteId") > 0)
                        .Hidden(
                            controlId: "LinkId",
                            css: "control-hidden always-send",
                            value: QueryStrings.Data("LinkId"),
                            _using: QueryStrings.Long("LinkId") > 0)
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
                .OutgoingMailsForm("Issues", issueModel.IssueId, issueModel.Ver)
                .DropDownSearchDialog("items", ss.SiteId)
                .CopyDialog("items", issueModel.IssueId)
                .MoveDialog()
                .OutgoingMailDialog()
                .PermissionsDialog()
                .EditorExtensions(issueModel: issueModel, ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, SiteSettings ss, IssueModel issueModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General()))
                .Li(_using: issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList()))
                .Li(_using: ss.CanManagePermission() &&
                        issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl())));
        }

        public static string PreviewTemplate(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var name = Strings.NewGuid();
            return hb
                .Div(css: "samples-displayed", action: () => hb
                    .Text(text: Displays.SamplesDisplayed()))
                .Div(css: "template-tab-container", action: () => hb
                    .Ul(action: () => hb
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Editor",
                                text: Displays.Editor()))
                        .Li(action: () => hb
                            .A(
                                href: "#" + name + "Grid",
                                text: Displays.Grid())))
                    .FieldSet(
                        id: name + "Editor",
                        action: () => hb
                            .FieldSetGeneralColumns(
                                ss: ss, issueModel: new IssueModel(), preview: true))
                    .FieldSet(
                        id: name + "Grid",
                        action: () => hb
                            .Table(css: "grid", action: () => hb
                                .THead(action: () => hb
                                    .GridHeader(
                                        columns: ss.GetGridColumns(),
                                        view: new View(ss),
                                        sort: false,
                                        checkRow: false)))))
                                            .ToString();
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var mine = issueModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    ss: ss, issueModel: issueModel));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel,
            bool preview = false)
        {
            ss.GetEditorColumns().ForEach(column =>
            {
                switch (column.Name)
                {
                    case "IssueId":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.IssueId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Ver.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Title.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Body.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "StartTime":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.StartTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CompletionTime":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CompletionTime.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "WorkValue":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.WorkValue.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ProgressRate":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ProgressRate.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "RemainingWorkValue":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.RemainingWorkValue.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Status":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Status.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Manager":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Manager.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Owner":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.Owner.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.ClassZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.NumZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DateZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.DescriptionZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.CheckZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsA":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsB":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsC":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsD":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsE":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsF":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsG":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsH":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsI":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsJ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsK":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsL":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsM":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsN":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsO":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsP":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsQ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsR":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsS":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsT":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsU":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsV":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsW":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsX":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsY":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsZ":
                        hb.Field(
                            ss,
                            column,
                            issueModel.MethodType,
                            issueModel.AttachmentsZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(issueModel);
                hb
                    .Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            ss: ss,
                            linkId: issueModel.IssueId,
                            methodType: issueModel.MethodType))
                    .Div(id: "Links", css: "links", action: () => hb
                        .Links(ss: ss, id: issueModel.IssueId));
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.Button(
                        text: Displays.Separate(),
                        controlCss: "button-icon",
                        onClick: "$p.openSeparateSettingsDialog($(this));",
                        icon: "ui-icon-extlink",
                        action: "EditSeparateSettings",
                        method: "post",
                        _using: ss.CanUpdate())
                    : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, SiteSettings ss, IssueModel issueModel)
        {
            return
                issueModel.MethodType != BaseModel.MethodTypes.New &&
                issueModel.VerType == Versions.VerTypes.Latest
                    ? hb.SeparateSettingsDialog()
                    : hb;
        }

        public static string EditorJson(SiteSettings ss, long issueId)
        {
            return EditorResponse(ss, new IssueModel(ss, issueId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss,
            IssueModel issueModel,
            Message message = null,
            string switchTargets = null)
        {
            issueModel.MethodType = BaseModel.MethodTypes.Edit;
            return new IssuesResponseCollection(issueModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, issueModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<long> GetSwitchTargets(SiteSettings ss, long issueId, long siteId)
        {
            var view = Views.GetBySession(ss);
            var where = view.Where(ss: ss);
            var join = ss.Join();
            var switchTargets = Rds.ExecuteScalar_int(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(statements: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssueId(),
                            join: join,
                            where: where,
                            orderBy: view.OrderBy(ss).Issues_UpdatedTime(SqlOrderBy.Types.desc)))
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
            this ResponseCollection res,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var mine = issueModel.Mine();
            ss.EditorColumns
                .Select(o => ss.GetColumn(o))
                .Where(o => o != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            res.Val(
                                "#Issues_SiteId",
                                issueModel.SiteId.ToControl(ss, column));
                            break;
                        case "UpdatedTime":
                            res.Val(
                                "#Issues_UpdatedTime",
                                issueModel.UpdatedTime.ToControl(ss, column));
                            break;
                        case "IssueId":
                            res.Val(
                                "#Issues_IssueId",
                                issueModel.IssueId.ToControl(ss, column));
                            break;
                        case "Ver":
                            res.Val(
                                "#Issues_Ver",
                                issueModel.Ver.ToControl(ss, column));
                            break;
                        case "Title":
                            res.Val(
                                "#Issues_Title",
                                issueModel.Title.ToControl(ss, column));
                            break;
                        case "Body":
                            res.Val(
                                "#Issues_Body",
                                issueModel.Body.ToControl(ss, column));
                            break;
                        case "StartTime":
                            res.Val(
                                "#Issues_StartTime",
                                issueModel.StartTime.ToControl(ss, column));
                            break;
                        case "CompletionTime":
                            res.Val(
                                "#Issues_CompletionTime",
                                issueModel.CompletionTime.ToControl(ss, column));
                            break;
                        case "WorkValue":
                            res.Val(
                                "#Issues_WorkValue",
                                issueModel.WorkValue.ToControl(ss, column));
                            break;
                        case "ProgressRate":
                            res.Val(
                                "#Issues_ProgressRate",
                                issueModel.ProgressRate.ToControl(ss, column));
                            break;
                        case "Status":
                            res.Val(
                                "#Issues_Status",
                                issueModel.Status.ToControl(ss, column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Issues_Manager",
                                issueModel.Manager.ToControl(ss, column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Issues_Owner",
                                issueModel.Owner.ToControl(ss, column));
                            break;
                        case "ClassA":
                            res.Val(
                                "#Issues_ClassA",
                                issueModel.ClassA.ToControl(ss, column));
                            break;
                        case "ClassB":
                            res.Val(
                                "#Issues_ClassB",
                                issueModel.ClassB.ToControl(ss, column));
                            break;
                        case "ClassC":
                            res.Val(
                                "#Issues_ClassC",
                                issueModel.ClassC.ToControl(ss, column));
                            break;
                        case "ClassD":
                            res.Val(
                                "#Issues_ClassD",
                                issueModel.ClassD.ToControl(ss, column));
                            break;
                        case "ClassE":
                            res.Val(
                                "#Issues_ClassE",
                                issueModel.ClassE.ToControl(ss, column));
                            break;
                        case "ClassF":
                            res.Val(
                                "#Issues_ClassF",
                                issueModel.ClassF.ToControl(ss, column));
                            break;
                        case "ClassG":
                            res.Val(
                                "#Issues_ClassG",
                                issueModel.ClassG.ToControl(ss, column));
                            break;
                        case "ClassH":
                            res.Val(
                                "#Issues_ClassH",
                                issueModel.ClassH.ToControl(ss, column));
                            break;
                        case "ClassI":
                            res.Val(
                                "#Issues_ClassI",
                                issueModel.ClassI.ToControl(ss, column));
                            break;
                        case "ClassJ":
                            res.Val(
                                "#Issues_ClassJ",
                                issueModel.ClassJ.ToControl(ss, column));
                            break;
                        case "ClassK":
                            res.Val(
                                "#Issues_ClassK",
                                issueModel.ClassK.ToControl(ss, column));
                            break;
                        case "ClassL":
                            res.Val(
                                "#Issues_ClassL",
                                issueModel.ClassL.ToControl(ss, column));
                            break;
                        case "ClassM":
                            res.Val(
                                "#Issues_ClassM",
                                issueModel.ClassM.ToControl(ss, column));
                            break;
                        case "ClassN":
                            res.Val(
                                "#Issues_ClassN",
                                issueModel.ClassN.ToControl(ss, column));
                            break;
                        case "ClassO":
                            res.Val(
                                "#Issues_ClassO",
                                issueModel.ClassO.ToControl(ss, column));
                            break;
                        case "ClassP":
                            res.Val(
                                "#Issues_ClassP",
                                issueModel.ClassP.ToControl(ss, column));
                            break;
                        case "ClassQ":
                            res.Val(
                                "#Issues_ClassQ",
                                issueModel.ClassQ.ToControl(ss, column));
                            break;
                        case "ClassR":
                            res.Val(
                                "#Issues_ClassR",
                                issueModel.ClassR.ToControl(ss, column));
                            break;
                        case "ClassS":
                            res.Val(
                                "#Issues_ClassS",
                                issueModel.ClassS.ToControl(ss, column));
                            break;
                        case "ClassT":
                            res.Val(
                                "#Issues_ClassT",
                                issueModel.ClassT.ToControl(ss, column));
                            break;
                        case "ClassU":
                            res.Val(
                                "#Issues_ClassU",
                                issueModel.ClassU.ToControl(ss, column));
                            break;
                        case "ClassV":
                            res.Val(
                                "#Issues_ClassV",
                                issueModel.ClassV.ToControl(ss, column));
                            break;
                        case "ClassW":
                            res.Val(
                                "#Issues_ClassW",
                                issueModel.ClassW.ToControl(ss, column));
                            break;
                        case "ClassX":
                            res.Val(
                                "#Issues_ClassX",
                                issueModel.ClassX.ToControl(ss, column));
                            break;
                        case "ClassY":
                            res.Val(
                                "#Issues_ClassY",
                                issueModel.ClassY.ToControl(ss, column));
                            break;
                        case "ClassZ":
                            res.Val(
                                "#Issues_ClassZ",
                                issueModel.ClassZ.ToControl(ss, column));
                            break;
                        case "NumA":
                            res.Val(
                                "#Issues_NumA",
                                issueModel.NumA.ToControl(ss, column));
                            break;
                        case "NumB":
                            res.Val(
                                "#Issues_NumB",
                                issueModel.NumB.ToControl(ss, column));
                            break;
                        case "NumC":
                            res.Val(
                                "#Issues_NumC",
                                issueModel.NumC.ToControl(ss, column));
                            break;
                        case "NumD":
                            res.Val(
                                "#Issues_NumD",
                                issueModel.NumD.ToControl(ss, column));
                            break;
                        case "NumE":
                            res.Val(
                                "#Issues_NumE",
                                issueModel.NumE.ToControl(ss, column));
                            break;
                        case "NumF":
                            res.Val(
                                "#Issues_NumF",
                                issueModel.NumF.ToControl(ss, column));
                            break;
                        case "NumG":
                            res.Val(
                                "#Issues_NumG",
                                issueModel.NumG.ToControl(ss, column));
                            break;
                        case "NumH":
                            res.Val(
                                "#Issues_NumH",
                                issueModel.NumH.ToControl(ss, column));
                            break;
                        case "NumI":
                            res.Val(
                                "#Issues_NumI",
                                issueModel.NumI.ToControl(ss, column));
                            break;
                        case "NumJ":
                            res.Val(
                                "#Issues_NumJ",
                                issueModel.NumJ.ToControl(ss, column));
                            break;
                        case "NumK":
                            res.Val(
                                "#Issues_NumK",
                                issueModel.NumK.ToControl(ss, column));
                            break;
                        case "NumL":
                            res.Val(
                                "#Issues_NumL",
                                issueModel.NumL.ToControl(ss, column));
                            break;
                        case "NumM":
                            res.Val(
                                "#Issues_NumM",
                                issueModel.NumM.ToControl(ss, column));
                            break;
                        case "NumN":
                            res.Val(
                                "#Issues_NumN",
                                issueModel.NumN.ToControl(ss, column));
                            break;
                        case "NumO":
                            res.Val(
                                "#Issues_NumO",
                                issueModel.NumO.ToControl(ss, column));
                            break;
                        case "NumP":
                            res.Val(
                                "#Issues_NumP",
                                issueModel.NumP.ToControl(ss, column));
                            break;
                        case "NumQ":
                            res.Val(
                                "#Issues_NumQ",
                                issueModel.NumQ.ToControl(ss, column));
                            break;
                        case "NumR":
                            res.Val(
                                "#Issues_NumR",
                                issueModel.NumR.ToControl(ss, column));
                            break;
                        case "NumS":
                            res.Val(
                                "#Issues_NumS",
                                issueModel.NumS.ToControl(ss, column));
                            break;
                        case "NumT":
                            res.Val(
                                "#Issues_NumT",
                                issueModel.NumT.ToControl(ss, column));
                            break;
                        case "NumU":
                            res.Val(
                                "#Issues_NumU",
                                issueModel.NumU.ToControl(ss, column));
                            break;
                        case "NumV":
                            res.Val(
                                "#Issues_NumV",
                                issueModel.NumV.ToControl(ss, column));
                            break;
                        case "NumW":
                            res.Val(
                                "#Issues_NumW",
                                issueModel.NumW.ToControl(ss, column));
                            break;
                        case "NumX":
                            res.Val(
                                "#Issues_NumX",
                                issueModel.NumX.ToControl(ss, column));
                            break;
                        case "NumY":
                            res.Val(
                                "#Issues_NumY",
                                issueModel.NumY.ToControl(ss, column));
                            break;
                        case "NumZ":
                            res.Val(
                                "#Issues_NumZ",
                                issueModel.NumZ.ToControl(ss, column));
                            break;
                        case "DateA":
                            res.Val(
                                "#Issues_DateA",
                                issueModel.DateA.ToControl(ss, column));
                            break;
                        case "DateB":
                            res.Val(
                                "#Issues_DateB",
                                issueModel.DateB.ToControl(ss, column));
                            break;
                        case "DateC":
                            res.Val(
                                "#Issues_DateC",
                                issueModel.DateC.ToControl(ss, column));
                            break;
                        case "DateD":
                            res.Val(
                                "#Issues_DateD",
                                issueModel.DateD.ToControl(ss, column));
                            break;
                        case "DateE":
                            res.Val(
                                "#Issues_DateE",
                                issueModel.DateE.ToControl(ss, column));
                            break;
                        case "DateF":
                            res.Val(
                                "#Issues_DateF",
                                issueModel.DateF.ToControl(ss, column));
                            break;
                        case "DateG":
                            res.Val(
                                "#Issues_DateG",
                                issueModel.DateG.ToControl(ss, column));
                            break;
                        case "DateH":
                            res.Val(
                                "#Issues_DateH",
                                issueModel.DateH.ToControl(ss, column));
                            break;
                        case "DateI":
                            res.Val(
                                "#Issues_DateI",
                                issueModel.DateI.ToControl(ss, column));
                            break;
                        case "DateJ":
                            res.Val(
                                "#Issues_DateJ",
                                issueModel.DateJ.ToControl(ss, column));
                            break;
                        case "DateK":
                            res.Val(
                                "#Issues_DateK",
                                issueModel.DateK.ToControl(ss, column));
                            break;
                        case "DateL":
                            res.Val(
                                "#Issues_DateL",
                                issueModel.DateL.ToControl(ss, column));
                            break;
                        case "DateM":
                            res.Val(
                                "#Issues_DateM",
                                issueModel.DateM.ToControl(ss, column));
                            break;
                        case "DateN":
                            res.Val(
                                "#Issues_DateN",
                                issueModel.DateN.ToControl(ss, column));
                            break;
                        case "DateO":
                            res.Val(
                                "#Issues_DateO",
                                issueModel.DateO.ToControl(ss, column));
                            break;
                        case "DateP":
                            res.Val(
                                "#Issues_DateP",
                                issueModel.DateP.ToControl(ss, column));
                            break;
                        case "DateQ":
                            res.Val(
                                "#Issues_DateQ",
                                issueModel.DateQ.ToControl(ss, column));
                            break;
                        case "DateR":
                            res.Val(
                                "#Issues_DateR",
                                issueModel.DateR.ToControl(ss, column));
                            break;
                        case "DateS":
                            res.Val(
                                "#Issues_DateS",
                                issueModel.DateS.ToControl(ss, column));
                            break;
                        case "DateT":
                            res.Val(
                                "#Issues_DateT",
                                issueModel.DateT.ToControl(ss, column));
                            break;
                        case "DateU":
                            res.Val(
                                "#Issues_DateU",
                                issueModel.DateU.ToControl(ss, column));
                            break;
                        case "DateV":
                            res.Val(
                                "#Issues_DateV",
                                issueModel.DateV.ToControl(ss, column));
                            break;
                        case "DateW":
                            res.Val(
                                "#Issues_DateW",
                                issueModel.DateW.ToControl(ss, column));
                            break;
                        case "DateX":
                            res.Val(
                                "#Issues_DateX",
                                issueModel.DateX.ToControl(ss, column));
                            break;
                        case "DateY":
                            res.Val(
                                "#Issues_DateY",
                                issueModel.DateY.ToControl(ss, column));
                            break;
                        case "DateZ":
                            res.Val(
                                "#Issues_DateZ",
                                issueModel.DateZ.ToControl(ss, column));
                            break;
                        case "DescriptionA":
                            res.Val(
                                "#Issues_DescriptionA",
                                issueModel.DescriptionA.ToControl(ss, column));
                            break;
                        case "DescriptionB":
                            res.Val(
                                "#Issues_DescriptionB",
                                issueModel.DescriptionB.ToControl(ss, column));
                            break;
                        case "DescriptionC":
                            res.Val(
                                "#Issues_DescriptionC",
                                issueModel.DescriptionC.ToControl(ss, column));
                            break;
                        case "DescriptionD":
                            res.Val(
                                "#Issues_DescriptionD",
                                issueModel.DescriptionD.ToControl(ss, column));
                            break;
                        case "DescriptionE":
                            res.Val(
                                "#Issues_DescriptionE",
                                issueModel.DescriptionE.ToControl(ss, column));
                            break;
                        case "DescriptionF":
                            res.Val(
                                "#Issues_DescriptionF",
                                issueModel.DescriptionF.ToControl(ss, column));
                            break;
                        case "DescriptionG":
                            res.Val(
                                "#Issues_DescriptionG",
                                issueModel.DescriptionG.ToControl(ss, column));
                            break;
                        case "DescriptionH":
                            res.Val(
                                "#Issues_DescriptionH",
                                issueModel.DescriptionH.ToControl(ss, column));
                            break;
                        case "DescriptionI":
                            res.Val(
                                "#Issues_DescriptionI",
                                issueModel.DescriptionI.ToControl(ss, column));
                            break;
                        case "DescriptionJ":
                            res.Val(
                                "#Issues_DescriptionJ",
                                issueModel.DescriptionJ.ToControl(ss, column));
                            break;
                        case "DescriptionK":
                            res.Val(
                                "#Issues_DescriptionK",
                                issueModel.DescriptionK.ToControl(ss, column));
                            break;
                        case "DescriptionL":
                            res.Val(
                                "#Issues_DescriptionL",
                                issueModel.DescriptionL.ToControl(ss, column));
                            break;
                        case "DescriptionM":
                            res.Val(
                                "#Issues_DescriptionM",
                                issueModel.DescriptionM.ToControl(ss, column));
                            break;
                        case "DescriptionN":
                            res.Val(
                                "#Issues_DescriptionN",
                                issueModel.DescriptionN.ToControl(ss, column));
                            break;
                        case "DescriptionO":
                            res.Val(
                                "#Issues_DescriptionO",
                                issueModel.DescriptionO.ToControl(ss, column));
                            break;
                        case "DescriptionP":
                            res.Val(
                                "#Issues_DescriptionP",
                                issueModel.DescriptionP.ToControl(ss, column));
                            break;
                        case "DescriptionQ":
                            res.Val(
                                "#Issues_DescriptionQ",
                                issueModel.DescriptionQ.ToControl(ss, column));
                            break;
                        case "DescriptionR":
                            res.Val(
                                "#Issues_DescriptionR",
                                issueModel.DescriptionR.ToControl(ss, column));
                            break;
                        case "DescriptionS":
                            res.Val(
                                "#Issues_DescriptionS",
                                issueModel.DescriptionS.ToControl(ss, column));
                            break;
                        case "DescriptionT":
                            res.Val(
                                "#Issues_DescriptionT",
                                issueModel.DescriptionT.ToControl(ss, column));
                            break;
                        case "DescriptionU":
                            res.Val(
                                "#Issues_DescriptionU",
                                issueModel.DescriptionU.ToControl(ss, column));
                            break;
                        case "DescriptionV":
                            res.Val(
                                "#Issues_DescriptionV",
                                issueModel.DescriptionV.ToControl(ss, column));
                            break;
                        case "DescriptionW":
                            res.Val(
                                "#Issues_DescriptionW",
                                issueModel.DescriptionW.ToControl(ss, column));
                            break;
                        case "DescriptionX":
                            res.Val(
                                "#Issues_DescriptionX",
                                issueModel.DescriptionX.ToControl(ss, column));
                            break;
                        case "DescriptionY":
                            res.Val(
                                "#Issues_DescriptionY",
                                issueModel.DescriptionY.ToControl(ss, column));
                            break;
                        case "DescriptionZ":
                            res.Val(
                                "#Issues_DescriptionZ",
                                issueModel.DescriptionZ.ToControl(ss, column));
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
                                issueModel.Comments.ToControl(ss, column));
                            break;
                        case "Creator":
                            res.Val(
                                "#Issues_Creator",
                                issueModel.Creator.ToControl(ss, column));
                            break;
                        case "Updator":
                            res.Val(
                                "#Issues_Updator",
                                issueModel.Updator.ToControl(ss, column));
                            break;
                        case "CreatedTime":
                            res.Val(
                                "#Issues_CreatedTime",
                                issueModel.CreatedTime.ToControl(ss, column));
                            break;
                        case "AttachmentsA":
                            res.ReplaceAll(
                                "#Issues_AttachmentsAField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsA.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsB":
                            res.ReplaceAll(
                                "#Issues_AttachmentsBField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsB.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsC":
                            res.ReplaceAll(
                                "#Issues_AttachmentsCField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsC.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsD":
                            res.ReplaceAll(
                                "#Issues_AttachmentsDField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsD.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsE":
                            res.ReplaceAll(
                                "#Issues_AttachmentsEField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsE.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsF":
                            res.ReplaceAll(
                                "#Issues_AttachmentsFField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsF.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsG":
                            res.ReplaceAll(
                                "#Issues_AttachmentsGField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsG.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsH":
                            res.ReplaceAll(
                                "#Issues_AttachmentsHField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsH.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsI":
                            res.ReplaceAll(
                                "#Issues_AttachmentsIField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsI.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsJ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsJField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsJ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsK":
                            res.ReplaceAll(
                                "#Issues_AttachmentsKField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsK.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsL":
                            res.ReplaceAll(
                                "#Issues_AttachmentsLField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsL.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsM":
                            res.ReplaceAll(
                                "#Issues_AttachmentsMField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsM.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsN":
                            res.ReplaceAll(
                                "#Issues_AttachmentsNField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsN.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsO":
                            res.ReplaceAll(
                                "#Issues_AttachmentsOField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsO.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsP":
                            res.ReplaceAll(
                                "#Issues_AttachmentsPField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsP.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsQ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsQField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsQ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsR":
                            res.ReplaceAll(
                                "#Issues_AttachmentsRField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsR.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsS":
                            res.ReplaceAll(
                                "#Issues_AttachmentsSField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsS.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsT":
                            res.ReplaceAll(
                                "#Issues_AttachmentsTField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsT.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsU":
                            res.ReplaceAll(
                                "#Issues_AttachmentsUField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsU.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsV":
                            res.ReplaceAll(
                                "#Issues_AttachmentsVField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsV.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsW":
                            res.ReplaceAll(
                                "#Issues_AttachmentsWField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsW.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsX":
                            res.ReplaceAll(
                                "#Issues_AttachmentsXField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsX.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsY":
                            res.ReplaceAll(
                                "#Issues_AttachmentsYField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsY.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsZ":
                            res.ReplaceAll(
                                "#Issues_AttachmentsZField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: issueModel.AttachmentsZ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static System.Web.Mvc.ContentResult GetByApi(SiteSettings ss)
        {
            var api = Forms.String().Deserialize<Api>();
            if (api == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest());
            }
            var view = api.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var issueCollection = new IssueCollection(
                ss: ss,
                where: view.Where(ss: ss),
                orderBy: view.OrderBy(ss: ss, pageSize: pageSize),
                offset: api.Offset,
                pageSize: pageSize,
                countRecord: true);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Offset = api.Offset,
                    PageSize = pageSize,
                    TotalCount = issueCollection.TotalCount,
                    Data = issueCollection.Select(o => o.GetByApi(ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId, methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = IssueValidators.OnEditing(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            ss.SetColumnAccessControls(issueModel.Mine());
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = issueModel.GetByApi(ss).ToSingleList()
                }
            }.ToJson());
        }

        public static string Create(SiteSettings ss)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var issueModel = new IssueModel(ss, 0, setByForm: true);
            var invalid = IssueValidators.OnCreating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Create(ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Created(issueModel.Title.DisplayValue));
                    return new ResponseCollection()
                        .SetMemory("formChanged", false)
                        .Href(Locations.Edit(
                            controller: Routes.Controller(),
                            id: ss.Columns.Any(o => o.Linking)
                                ? Forms.Long("LinkId")
                                : issueModel.IssueId))
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static System.Web.Mvc.ContentResult CreateByApi(SiteSettings ss)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return ApiResults.Error(Error.Types.ItemsLimit);
            }
            var issueModel = new IssueModel(ss, 0, setByApi: true);
            var invalid = IssueValidators.OnCreating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(ss);
            var error = issueModel.Create(ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Created(issueModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        error,
                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText);
                default:
                    return ApiResults.Error(error);
            }
        }

        public static string Update(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId, setByForm: true);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = issueModel.Update(
                ss: ss,
                notice: true,
                permissions: Forms.List("CurrentPermissionsAll"),
                permissionChanged: Forms.Exists("CurrentPermissionsAll"));
            switch (error)
            {
                case Error.Types.None:
                    var res = new IssuesResponseCollection(issueModel);
                    res.Val(
                        "#Issues_RemainingWorkValue",
                        ss.GetColumn("RemainingWorkValue")
                            .Display(ss, issueModel.RemainingWorkValue));
                    return ResponseByUpdate(res, ss, issueModel)
                        .PrependComment(
                            ss,
                            ss.GetColumn("Comments"),
                            issueModel.Comments,
                            issueModel.VerType)
                        .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText)
                            .ToJson();
                case Error.Types.UpdateConflicts:
                    return Messages.ResponseUpdateConflicts(
                        issueModel.Updator.Name)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            IssuesResponseCollection res,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FieldResponse(ss, issueModel)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", issueModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: issueModel, tableName: "Issues"))
                .Html("#Links", new HtmlBuilder().Links(
                    ss: ss, id: issueModel.IssueId))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(issueModel.Title.DisplayValue))
                .Comment(
                    ss,
                    ss.GetColumn("Comments"),
                    issueModel.Comments,
                    issueModel.DeleteCommentId)
                .ClearFormData();
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId, setByApi: true);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(ss);
            var error = issueModel.Update(ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Updated(issueModel.Title.DisplayValue));
                case Error.Types.Duplicated:
                    return ApiResults.Error(
                        error,
                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText);
                default:
                    return ApiResults.Error(error);
            }
        }

        public static string Copy(SiteSettings ss, long issueId)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var issueModel = new IssueModel(ss, issueId, setByForm: true);
            var invalid = IssueValidators.OnCreating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            issueModel.IssueId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                issueModel.Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Bool("CopyWithComments"))
            {
                issueModel.Comments.Clear();
            }
            var error = issueModel.Create(
                ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
            switch (error)
            {
                case Error.Types.None:
                    return EditorResponse(
                        ss,
                        issueModel,
                        Messages.Copied(),
                        GetSwitchTargets(
                            ss, issueModel.IssueId, issueModel.SiteId).Join())
                                .ToJson();
                case Error.Types.Duplicated:
                    return Messages.ResponseDuplicated(
                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText)
                            .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Move(SiteSettings ss, long issueId)
        {
            var siteId = Forms.Long("MoveTargets");
            if (Contract.ItemsLimit(siteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnMoving(ss, SiteSettingsUtilities.Get(siteId, issueId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Move(ss, siteId);
            switch (error)
            {
                case Error.Types.None:
                    ss = SiteSettingsUtilities.Get(siteId, issueModel.IssueId);
                    return EditorResponse(ss, issueModel)
                        .Message(Messages.Moved(issueModel.Title.Value))
                        .Val("#BackUrl", Locations.ItemIndex(siteId))
                        .ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Delete(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnDeleting(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Delete(ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    Sessions.Set("Message", Messages.Deleted(issueModel.Title.Value));
                    var res = new IssuesResponseCollection(issueModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Get(
                        "Items", ss.SiteId.ToString(), ViewModes.GetBySession(ss.SiteId)));
                    return res.ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static System.Web.Mvc.ContentResult DeleteByApi(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId, methodType: BaseModel.MethodTypes.Edit);
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = IssueValidators.OnDeleting(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            issueModel.SiteId = ss.SiteId;
            issueModel.SetTitle(ss);
            var error = issueModel.Delete(ss, notice: true);
            switch (error)
            {
                case Error.Types.None:
                    return ApiResults.Success(
                        issueModel.IssueId,
                        Displays.Deleted(issueModel.Title.DisplayValue));
                default:
                    return ApiResults.Error(error);
            }
        }

        public static string Restore(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel();
            var invalid = IssueValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Restore(ss, issueId);
            switch (error)
            {
                case Error.Types.None:
                    var res = new IssuesResponseCollection(issueModel);
                    return res.ToJson();
                default:
                    return error.MessageJson();
            }
        }

        public static string Histories(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            ss.SetColumnAccessControls(issueModel.Mine());
            var columns = ss.GetHistoryColumns(checkPermission: true);
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission.MessageJson();
            }
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columns: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new IssueCollection(
                            ss: ss,
                            column: HistoryColumn(columns),
                            where: Rds.IssuesWhere().IssueId(issueModel.IssueId),
                            orderBy: Rds.IssuesOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(issueModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(issueModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                issueModelHistory.Ver == issueModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    issueModel: issueModelHistory))))));
            return new IssuesResponseCollection(issueModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        private static SqlColumnCollection HistoryColumn(List<Column> columns)
        {
            var sqlColumn = new Rds.IssuesColumnCollection()
                .IssueId()
                .Ver();
            columns.ForEach(column => sqlColumn.IssuesColumn(column.ColumnName));
            return sqlColumn;
        }

        public static string History(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            ss.SetColumnAccessControls(issueModel.Mine());
            issueModel.Get(
                ss, 
                where: Rds.IssuesWhere()
                    .IssueId(issueModel.IssueId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            issueModel.VerType = Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, issueModel).ToJson();
        }

        public static string EditSeparateSettings(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            return new ResponseCollection()
                .Html(
                    "#SeparateSettingsDialog",
                    new HtmlBuilder().SeparateSettings(
                        ss: ss,
                        title: issueModel.Title.Value,
                        workValue: issueModel.WorkValue.Value,
                        mine: issueModel.Mine()))
                .Invoke("separateSettings")
                .ToJson();
        }

        public static string Separate(SiteSettings ss, long issueId)
        {
            var number = Forms.Int("SeparateNumber");
            if (Contract.ItemsLimit(ss.SiteId, number - 1))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
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
                    issueModel.Create(ss, otherInitValue: true);
                    idHash.Add(index, issueModel.IssueId);
                    timestampHash.Add(index, issueModel.Timestamp);
                }
                var addCommentCollection = new List<string>();
                addCommentCollection.AddRange(idHash.Select(o => "[{0}]({1}{2})  ".Params(
                    Forms.Data("SeparateTitle_" + o.Key),
                    Url.Server(),
                    Locations.ItemEdit(o.Value))));
                var addComment = "[md]\n{0}  \n{1}".Params(
                    Displays.Separated(), addCommentCollection.Join("\n"));
                for (var index = number; index >= 1; index--)
                {
                    var source = index == 1;
                    issueModel.IssueId = idHash[index];
                    issueModel.Ver = source
                        ? ver
                        : 1;
                    issueModel.Timestamp = timestampHash[index];
                    issueModel.Title.Value = Forms.Data("SeparateTitle_" + index);
                    issueModel.WorkValue.Value = source
                        ? Forms.Decimal("SourceWorkValue")
                        : Forms.Decimal("SeparateWorkValue_" + index);
                    issueModel.Comments.Clear();
                    if (source || Forms.Bool("SeparateCopyWithComments"))
                    {
                        issueModel.Comments = comments.Deserialize<Comments>();
                    }
                    issueModel.Comments.Prepend(addComment);
                    issueModel.Update(ss, forceSynchronizeSourceSummary: true, otherInitValue: true);
                }
                return EditorResponse(ss, issueModel, Messages.Separated()).ToJson();
            }
            else
            {
                return Messages.ResponseInvalidRequest().ToJson();
            }
        }

        public static string BulkMove(SiteSettings ss)
        {
            var siteId = Forms.Long("MoveTargets");
            var selector = new GridSelector();
            var count = BulkMoveCount(siteId, ss, selector);
            if (Contract.ItemsLimit(siteId, count))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            if (Permissions.CanMove(ss, SiteSettingsUtilities.Get(siteId, siteId)))
            {
                if (selector.All)
                {
                    count = BulkMove(siteId, ss, selector);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = BulkMove(siteId, ss, selector);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                Summaries.Synchronize(ss);
                return GridRows(
                    ss,
                    clearCheck: true,
                    message: Messages.BulkMoved(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkMoveCount(
            long siteId,
            SiteSettings ss,
            GridSelector selector)
        {
            return Rds.ExecuteScalar_int(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    join: ss.Join(),
                    where: Views.GetBySession(ss).Where(
                        ss, Rds.IssuesWhere()
                            .SiteId(ss.SiteId)
                            .IssueId_In(
                                value: selector.Selected,
                                negative: selector.All,
                                _using: selector.Selected.Any()))));
        }

        private static int BulkMove(
            long siteId,
            SiteSettings ss,
            GridSelector selector)
        {
            return Rds.ExecuteScalar_response(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateIssues(
                        where: Views.GetBySession(ss).Where(
                            ss, Rds.IssuesWhere()
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

        public static string BulkDelete(SiteSettings ss)
        {
            if (ss.CanDelete())
            {
                var selector = new GridSelector();
                var count = 0;
                if (selector.All)
                {
                    count = BulkDelete(ss, selector.Selected, negative: true);
                }
                else
                {
                    if (selector.Selected.Any())
                    {
                        count = BulkDelete(ss, selector.Selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
                Summaries.Synchronize(ss);
                return GridRows(
                    ss,
                    clearCheck: true,
                    message: Messages.BulkDeleted(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkDelete(
            SiteSettings ss,
            IEnumerable<long> checkedItems,
            bool negative = false)
        {
            var where = Views.GetBySession(ss).Where(
                ss, Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(
                        value: checkedItems,
                        negative: negative,
                        _using: checkedItems.Any()));
            var sub = Rds.SelectIssues(
                column: Rds.IssuesColumn().IssueId(),
                where: where);
            var statements = new List<SqlStatement>();
            statements.OnBulkDeletingExtendedSqls(ss.SiteId);
            statements.Add(Rds.PhysicalDeleteItems(
                where: Rds.ItemsWhere()
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere()
                    .Or(or: Rds.LinksWhere()
                        .DestinationId_In(sub: sub)
                        .SourceId_In(sub: sub))));
            statements.Add(Rds.DeleteBinaries(
                where: Rds.BinariesWhere()
                    .TenantId(Sessions.TenantId())
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.DeleteIssues(
                where: where, 
                countRecord: true));
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_response(
                transactional: true,
                statements: statements.ToArray())
                    .Count.ToInt();
        }

        public static string Import(SiteModel siteModel)
        {
            if (!Contract.Import()) return null;
            var ss = siteModel.IssuesSiteSettings(siteModel.SiteId, setAllChoices: true);
            if (!ss.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var res = new ResponseCollection();
            Csv csv;
            try
            {
                csv = new Csv(Forms.File("Import"), Forms.Data("Encoding"));
            }
            catch
            {
                return Messages.ResponseFailedReadFile().ToJson();
            }
            var count = csv.Rows.Count();
            if (Parameters.General.ImportMax > 0 && Parameters.General.ImportMax < count)
            {
                return Error.Types.ImportMax.MessageJson(Parameters.General.ImportMax.ToString());
            }
            if (Contract.ItemsLimit(ss.SiteId, count))
            {
                return Error.Types.ItemsLimit.MessageJson();
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
                var invalid = Imports.ColumnValidate(ss, columnHash.Values
                    .Select(o => o.ColumnName), "CompletionTime");
                if (invalid != null) return invalid;
                Rds.ExecuteNonQuery(
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var issueHash = new Dictionary<int, IssueModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var issueModel = new IssueModel() { SiteId = ss.SiteId };
                    if (Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new IssueModel(ss, data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            issueModel = model;
                        }
                    }
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], ss.InheritPermission);
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
                                    issueModel.Comments.Prepend(data.Row[column.Key]);
                                }
                                break;
                        }
                    });
                    issueHash.Add(data.Index, issueModel);
                });
                var errorCompletionTime = Imports.Validate(
                    issueHash.ToDictionary(
                        o => o.Key,
                        o => o.Value.CompletionTime.Value.ToString()),
                    ss.GetColumn("CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                var insertCount = 0;
                var updateCount = 0;
                foreach (var issueModel in issueHash.Values)
                {
                    issueModel.SetTitle(ss);
                    if (issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        issueModel.VerUp = Versions.MustVerUp(issueModel);
                        if (issueModel.Updated())
                        {
                            var error = issueModel.Update(
                                ss: ss, extendedSqls: false, get: false);
                            switch (error)
                            {
                                case Error.Types.None:
                                    break;
                                case Error.Types.Duplicated:
                                    return Messages.ResponseDuplicated(
                                        ss.GetColumn(ss.DuplicatedColumn)?.LabelText)
                                            .ToJson();
                                default:
                                    return error.MessageJson();
                            }
                            updateCount++;
                        }
                    }
                    else
                    {
                        var error = issueModel.Create(
                            ss: ss, extendedSqls: false, get: false);
                        switch (error)
                        {
                            case Error.Types.None:
                                break;
                            case Error.Types.Duplicated:
                                return Messages.ResponseDuplicated(
                                    ss.GetColumn(ss.DuplicatedColumn)?.LabelText)
                                        .ToJson();
                            default:
                                return error.MessageJson();
                        }
                        insertCount++;
                    }
                }
                Rds.ExecuteNonQuery(
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportedExtendedSqls(ss.SiteId).ToArray());
                return GridRows(
                    ss: ss,
                    res: res.WindowScrollTop(),
                    message: Messages.Imported(insertCount.ToString(), updateCount.ToString()));
            }
            else
            {
                return Messages.ResponseFileNotFound().ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
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

        public static string OpenExportSelectorDialog(SiteSettings ss, SiteModel siteModel)
        {
            if (!Contract.Export())
            {
                return HtmlTemplates.Error(Error.Types.InvalidRequest);
            }
            var invalid = IssueValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            return new ResponseCollection()
                .Html(
                    "#ExportSelectorDialog",
                    new HtmlBuilder().ExportSelectorDialog(ss: ss))
                .ToJson();
        }

        public static ResponseFile Export(SiteSettings ss, SiteModel siteModel)
        {
            if (!Contract.Export()) return null;
            var invalid = IssueValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return ExportUtilities.Csv(ss, ss.GetExport(QueryStrings.Int("id")));
        }

        public static ResponseFile ExportCrosstab(SiteSettings ss, SiteModel siteModel)
        {
            if (!Contract.Export()) return null;
            var invalid = IssueValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = ss.GetColumn(view.GetCrosstabGroupByX(ss));
            var groupByY = ss.GetColumn(view.GetCrosstabGroupByY(ss));
            var columns = CrosstabColumns(ss, view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn("IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                ss, view, groupByX, groupByY, columns, value, aggregateType, timePeriod, month);
            return new ResponseFile(
                Libraries.ViewModes.CrosstabUtilities.Csv(
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
                ExportUtilities.FileName(ss, Displays.Crosstab()));
        }

        public static string Calendar(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Calendar"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var fromColumn = ss.GetColumn(view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(month);
            var end = Calendars.EndDate(month);
            var dataRows = CalendarDataRows(
                ss,
                view,
                fromColumn,
                toColumn,
                Calendars.BeginDate(month),
                Calendars.EndDate(month));
            var inRange = dataRows.Count() <= Parameters.General.CalendarLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.CalendarLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Calendar(
                        ss: ss,
                        fromColumn: fromColumn,
                        toColumn: toColumn,
                        month: month,
                        begin: begin,
                        dataRows: dataRows,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string UpdateByCalendar(SiteSettings ss)
        {
            var issueModel = new IssueModel(ss, Forms.Long("Id"), setByForm: true);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(issueModel);
            issueModel.Update(ss, notice: true);
            return CalendarJson(
                ss: ss,
                changedItemId: issueModel.IssueId,
                update: true,
                message: Messages.Updated(issueModel.Title.DisplayValue));
        }

        public static string CalendarJson(
            SiteSettings ss, long changedItemId = 0, bool update = false, Message message = null)
        {
            if (!ss.EnableViewMode("Calendar"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Calendar");
            var fromColumn = ss.GetColumn(view.GetCalendarFromColumn(ss));
            var toColumn = ss.GetColumn(view.GetCalendarToColumn(ss));
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(month);
            var end = Calendars.EndDate(month);
            var dataRows = CalendarDataRows(
                ss,
                view,
                fromColumn,
                toColumn,
                Calendars.BeginDate(month),
                Calendars.EndDate(month));
            return dataRows.Count() <= Parameters.General.CalendarLimit
                ? new ResponseCollection()
                    .ViewMode(
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
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            Parameters.General.CalendarLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CalendarBody",
                        body: new HtmlBuilder()
                            .Calendar(
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
            return Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(ss)
                        .IssueId(_as: "Id")
                        .IssuesColumn(fromColumn.ColumnName, _as: "From")
                        .IssuesColumn(toColumn?.ColumnName, _as: "To")
                        .UpdatedTime()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType)),
                    join: ss.Join(),
                    where: view.Where(ss: ss, where: where)))
                        .AsEnumerable();
        }

        private static HtmlBuilder Calendar(
            this HtmlBuilder hb,
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
                    ss: ss,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId)
                : hb.CalendarBody(
                    ss: ss,
                    fromColumn: fromColumn,
                    toColumn: toColumn,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange,
                    changedItemId: changedItemId);
        }

        public static string Crosstab(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Crosstab"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = ss.GetColumn(view.GetCrosstabGroupByX(ss));
            var groupByY = ss.GetColumn(view.GetCrosstabGroupByY(ss));
            var columns = CrosstabColumns(ss, view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn("IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                ss, view, groupByX, groupByY, columns, value, aggregateType, timePeriod, month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns" ||
                Libraries.ViewModes.CrosstabUtilities.InRangeY(dataRows);
            if (!inRangeX)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyColumnCases(
                        Parameters.General.CrosstabXLimit.ToString()));
            }
            else if (!inRangeY)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyColumnCases(
                        Parameters.General.CrosstabYLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Crosstab(
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

        public static string CrosstabJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Crosstab"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = ss.GetColumn(view.GetCrosstabGroupByX(ss));
            var groupByY = ss.GetColumn(view.GetCrosstabGroupByY(ss));
            var columns = CrosstabColumns(ss, view);
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = ss.GetColumn(view.GetCrosstabValue(ss));
            if (value == null)
            {
                value = ss.GetColumn("IssueId");
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                ss, view, groupByX, groupByY, columns, value, aggregateType, timePeriod, month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(dataRows);
            var inRangeY =
                view.CrosstabGroupByY == "Columns" ||
                Libraries.ViewModes.CrosstabUtilities.InRangeY(dataRows);
            var bodyOnly = Forms.ControlId().StartsWith("Crosstab");
            return inRangeX && inRangeY
                ? new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "setCrosstab",
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
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
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: !inRangeX
                            ? Messages.TooManyColumnCases(
                                Parameters.General.CrosstabXLimit.ToString())
                            : Messages.TooManyRowCases(
                                Parameters.General.CrosstabYLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#CrosstabBody",
                        body: !bodyOnly
                            ? new HtmlBuilder().Crosstab(
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

        private static List<Column> CrosstabColumns(SiteSettings ss, View view)
        {
            return Libraries.ViewModes.CrosstabUtilities.GetColumns(
                ss,
                view.CrosstabColumns?.Deserialize<IEnumerable<string>>()?
                    .Select(o => ss.GetColumn(o))
                    .ToList());
        }

        private static EnumerableRowCollection<DataRow> CrosstabDataRows(
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
            var join = ss.Join(columns: Libraries.ViewModes.CrosstabUtilities
                .JoinColumns(view, groupByX, groupByY, columns));
            if (groupByX?.TypeName != "datetime")
            {
                dataRows = Rds.ExecuteTable(statements:
                    Rds.SelectIssues(
                        column: Rds.IssuesColumn()
                            .Add(ss, groupByX)
                            .CrosstabColumns(
                                ss: ss,
                                view: view,
                                groupByY: groupByY,
                                columns: columns,
                                value: value,
                                aggregateType: aggregateType),
                        join: join,
                        where: view.Where(ss: ss),
                        groupBy: Rds.IssuesGroupBy()
                            .Add(ss, groupByX)
                            .Add(ss, groupByY)))
                                .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    ss, groupByX, timePeriod);
                dataRows = Rds.ExecuteTable(statements:
                    Rds.SelectIssues(
                        column: Rds.IssuesColumn()
                            .Add(dateGroup, _as: groupByX.ColumnName)
                            .CrosstabColumns(
                                ss: ss,
                                view: view,
                                groupByY: groupByY,
                                columns: columns,
                                value: value,
                                aggregateType: aggregateType),
                        join: join,
                        where: view.Where(
                            ss: ss,
                            where: Libraries.ViewModes.CrosstabUtilities.Where(
                                ss, groupByX, timePeriod, month)),
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

        public static string Gantt(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Gantt"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupBy = ss.GetColumn(view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(view.GetGanttSortBy());
            var range = new Libraries.ViewModes.GanttRange(ss, view);
            var dataRows = GanttDataRows(ss, view, groupBy, sortBy);
            var inRange = dataRows.Count() <= Parameters.General.GanttLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.GanttLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Gantt(
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

        public static string GanttJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Gantt"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Gantt");
            var range = new Libraries.ViewModes.GanttRange(ss, view);
            var groupBy = ss.GetColumn(view.GetGanttGroupBy());
            var sortBy = ss.GetColumn(view.GetGanttSortBy());
            var period = view.GanttPeriod.ToInt();
            var startDate = view.GanttStartDate.ToDateTime();
            var dataRows = GanttDataRows(ss, view, groupBy, sortBy);
            if (dataRows.Count() <= Parameters.General.GanttLimit)
            {
                return new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "drawGantt",
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
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
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            Parameters.General.GanttLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#GanttBody",
                        body: new HtmlBuilder()
                            .Gantt(
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
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    period: period,
                    startDate: startDate,
                    range: range,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.GanttBody(
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
            SiteSettings ss, View view, Column groupBy, Column sortBy)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(ss)
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
                    join: ss.Join(),
                    where: view.Where(
                        ss: ss, where: Libraries.ViewModes.GanttUtilities.Where(ss, view))))
                            .AsEnumerable();
        }

        public static string BurnDown(SiteSettings ss)
        {
            if (!ss.EnableViewMode("BurnDown"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.BurnDownLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.BurnDownLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.BurnDown(
                            ss: ss,
                            dataRows: BurnDownDataRows(ss: ss, view: view),
                            ownerLabelText: ss.GetColumn("Owner").GridLabelText,
                            column: ss.GetColumn("WorkValue"));
                    }
                });
        }

        public static string BurnDownJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("BurnDown"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            return gridData.Aggregations.TotalCount <= Parameters.General.BurnDownLimit
                ? new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "drawBurnDown",
                        body: new HtmlBuilder()
                            .BurnDown(
                                ss: ss,
                                dataRows: BurnDownDataRows(ss, view),
                                ownerLabelText: ss.GetColumn("Owner").GridLabelText,
                                column: ss.GetColumn("WorkValue")))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            Parameters.General.BurnDownLimit.ToString()),
                        body: new HtmlBuilder())
                    .ToJson();
        }

        public static string BurnDownRecordDetails(SiteSettings ss)
        {
            var date = Forms.DateTime("BurnDownDate");
            return new ResponseCollection()
                .After(string.Empty, new HtmlBuilder().BurnDownRecordDetails(
                    elements: new Libraries.ViewModes.BurnDown(ss, BurnDownDataRows(
                        ss: ss,
                        view: Views.GetBySession(ss)))
                            .Where(o => o.UpdatedTime == date),
                    progressRateColumn: ss.GetColumn("ProgressRate"),
                    statusColumn: ss.GetColumn("Status"),
                    colspan: Forms.Int("BurnDownColspan"),
                    unit: ss.GetColumn("WorkValue").Unit)).ToJson();
        }

        private static EnumerableRowCollection<DataRow> BurnDownDataRows(
            SiteSettings ss, View view)
        {
            var where = view.Where(ss: ss);
            var join = ss.Join();
            return Rds.ExecuteTable(statements: new SqlStatement[]
            {
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(ss)
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
                    column: Rds.IssuesTitleColumn(ss)
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

        public static string TimeSeries(SiteSettings ss)
        {
            if (!ss.EnableViewMode("TimeSeries"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.TimeSeriesLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .TimeSeries(
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string TimeSeriesJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("TimeSeries"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("TimeSeries");
            return gridData.Aggregations.TotalCount <= Parameters.General.TimeSeriesLimit
                ? new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "drawTimeSeries",
                        bodyOnly: bodyOnly,
                        bodySelector: "#TimeSeriesBody",
                        body: new HtmlBuilder()
                            .TimeSeries(
                                ss: ss,
                                view: view,
                                bodyOnly: bodyOnly,
                                inRange: true))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            Parameters.General.TimeSeriesLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#TimeSeriesBody",
                        body: new HtmlBuilder()
                            .TimeSeries(
                                ss: ss,
                                view: view,
                                bodyOnly: bodyOnly,
                                inRange: false))
                    .ToJson();
        }

        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            bool inRange)
        {
            var groupBy = ss.GetColumn(view.GetTimeSeriesGroupBy(ss));
            var aggregateType = view.GetTimeSeriesAggregationType(ss);
            var value = ss.GetColumn(view.GetTimeSeriesValue(ss));
            var dataRows = TimeSeriesDataRows(
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
                    ss: ss,
                    groupBy: groupBy,
                    aggregateType: aggregateType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.TimeSeriesBody(
                    ss: ss,
                    groupBy: groupBy,
                    aggregateType: aggregateType,
                    value: value,
                    dataRows: dataRows,
                    inRange: inRange);
        }

        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            SiteSettings ss, View view, Column groupBy, Column value)
        {
            if (groupBy != null && value != null)
            {
                var dataRows = Rds.ExecuteTable(statements:
                    Rds.SelectIssues(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.IssuesColumn()
                            .IssueId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .Add(ss: ss, column: groupBy)
                            .Add(ss: ss, column: value),
                        join: ss.Join(),
                        where: view.Where(ss: ss)))
                            .AsEnumerable();
                ss.SetChoiceHash(dataRows);
                return dataRows;
            }
            else
            {
                return null;
            }
        }

        public static string Kamban(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Kamban"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = gridData.Aggregations.TotalCount <=
                Parameters.General.KambanLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.KambanLimit.ToString()));
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Kamban(
                        ss: ss,
                        view: view,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string KambanJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Kamban"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Kamban");
            return gridData.Aggregations.TotalCount <= Parameters.General.KambanLimit
                ? new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        invoke: "setKamban",
                        bodyOnly: bodyOnly,
                        bodySelector: "#KambanBody",
                        body: new HtmlBuilder()
                            .Kamban(
                                ss: ss,
                                view: view,
                                bodyOnly: bodyOnly,
                                changedItemId: Forms.Long("KambanId")))
                    .ToJson()
                : new ResponseCollection()
                    .ViewMode(
                        ss: ss,
                        view: view,
                        gridData: gridData,
                        message: Messages.TooManyCases(
                            Parameters.General.KambanLimit.ToString()),
                        bodyOnly: bodyOnly,
                        bodySelector: "#KambanBody",
                        body: new HtmlBuilder()
                            .Kamban(
                                ss: ss,
                                view: view,
                                bodyOnly: bodyOnly,
                                inRange: false))
                    .ToJson();
        }

        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            long changedItemId = 0,
            bool inRange = true)
        {
            var groupByX = ss.GetColumn(view.GetKambanGroupByX(ss));
            if (groupByX == null)
            {
                return hb;
            }
            var groupByY = ss.GetColumn(view.GetKambanGroupByY(ss));
            var aggregateType = view.GetKambanAggregationType(ss);
            var value = ss.GetColumn(view.GetKambanValue(ss));
            var aggregationView = view.KambanAggregationView ?? false;
            var data = KambanData(
                ss: ss,
                view: view,
                groupByX: groupByX,
                groupByY: groupByY,
                value: value);
            return !bodyOnly
                ? hb.Kamban(
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
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            Column value)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .IssueId()
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType))
                        .Add(ss: ss, column: groupByX)
                        .Add(ss: ss, column: groupByY)
                        .Add(ss: ss, column: value),
                    where: view.Where(ss: ss)))
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

        public static string UpdateByKamban(SiteSettings ss)
        {
            var issueModel = new IssueModel(ss, Forms.Long("KambanId"), setByForm: true);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (issueModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            issueModel.VerUp = Versions.MustVerUp(issueModel);
            issueModel.Update(ss, notice: true);
            return KambanJson(ss);
        }

        public static string ImageLib(SiteSettings ss)
        {
            if (!ss.EnableViewMode("ImageLib"))
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .ImageLib(
                        ss: ss,
                        view: view,
                        bodyOnly: false));
        }

        public static string ImageLibJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("ImageLib"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("ImageLib");
            return new ResponseCollection()
                .ViewMode(
                    ss: ss,
                    view: view,
                    gridData: gridData,
                    invoke: "setImageLib",
                    bodyOnly: bodyOnly,
                    bodySelector: "#ImageLibBody",
                    body: new HtmlBuilder()
                        .ImageLib(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly))
                .ToJson();
        }

        private static HtmlBuilder ImageLib(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            int offset = 0)
        {
            return !bodyOnly
                ? hb.ImageLib(ss: ss, imageLibData: new ImageLibData(
                    ss, view, offset: offset, pageSize: ss.ImageLibPageSize.ToInt()))
                : hb.ImageLibBody(ss: ss, imageLibData: new ImageLibData(
                    ss, view, offset: offset, pageSize: ss.ImageLibPageSize.ToInt()));
        }

        public static string ImageLibNext(SiteSettings ss, int offset)
        {
            var view = Views.GetBySession(ss);
            var imageLibData = new ImageLibData(
                ss, view, offset: offset, pageSize: ss.ImageLibPageSize.ToInt());
            var hb = new HtmlBuilder();
            new ImageLibData(ss, view, offset, Parameters.General.ImageLibPageSize)
                .DataRows
                .ForEach(dataRow => hb
                    .ImageLibItem(
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

        public static void SetLinks(this List<IssueModel> issues, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: issues
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issues.ForEach(issueModel => issueModel.SetTitle(ss));
            }
        }
    }
}
