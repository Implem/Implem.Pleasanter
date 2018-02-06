using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
            var invalid = ResultValidators.OnEntry(ss);
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
                referenceType: "Results",
                script: Libraries.Scripts.JavaScripts.ViewMode(viewMode),
                userScript: ss.ViewModeScripts(Routes.Action()),
                userStyle: ss.ViewModeStyles(Routes.Action()),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("ResultsForm")
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
                                verType: Versions.VerTypes.Latest,
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Results")
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
                .Html("#ViewModeContainer", new HtmlBuilder().Grid(
                    ss: ss,
                    gridData: gridData,
                    view: view))
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: gridData.Aggregations))
                .Paging("#Grid")
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
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.ResultsColumn(column));
            return sqlColumnCollection;
        }

        private static SqlColumnCollection DefaultSqlColumns(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(ss.IncludedColumns())
                .Concat(ss.GetUseSearchLinks().Select(o => o.ColumnName))
                .Concat(ss.TitleColumns)
                    .Distinct().ForEach(column =>
                        sqlColumnCollection.ResultsColumn(column));
            return sqlColumnCollection;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, SiteSettings ss, Column column, ResultModel resultModel)
        {
            if (!column.GridDesign.IsNullOrEmpty())
            {
                return hb.TdCustomValue(
                    ss: ss,
                    gridDesign: column.GridDesign,
                    resultModel: resultModel);
            }
            else
            {
                var mine = resultModel.Mine();
                switch (column.Name)
                {
                    case "SiteId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.SiteId)
                            : hb.Td(column: column, value: string.Empty);
                    case "UpdatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.UpdatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    case "ResultId":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ResultId)
                            : hb.Td(column: column, value: string.Empty);
                    case "Ver":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Ver)
                            : hb.Td(column: column, value: string.Empty);
                    case "Title":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Title)
                            : hb.Td(column: column, value: string.Empty);
                    case "Body":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Body)
                            : hb.Td(column: column, value: string.Empty);
                    case "TitleBody":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.TitleBody)
                            : hb.Td(column: column, value: string.Empty);
                    case "Status":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Status)
                            : hb.Td(column: column, value: string.Empty);
                    case "Manager":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Manager)
                            : hb.Td(column: column, value: string.Empty);
                    case "Owner":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Owner)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassA)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassB)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassC)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassD)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassE)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassF)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassG)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassH)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassI)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassK)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassL)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassM)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassN)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassO)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassP)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassR)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassS)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassT)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassU)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassV)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassW)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassX)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassY)
                            : hb.Td(column: column, value: string.Empty);
                    case "ClassZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.ClassZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumA)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumB)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumC)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumD)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumE)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumF)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumG)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumH)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumI)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumK)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumL)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumM)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumN)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumO)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumP)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumR)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumS)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumT)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumU)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumV)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumW)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumX)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumY)
                            : hb.Td(column: column, value: string.Empty);
                    case "NumZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.NumZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DateZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DateZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionA)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionB)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionC)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionD)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionE)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionF)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionG)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionH)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionI)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionK)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionL)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionM)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionN)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionO)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionP)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionR)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionS)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionT)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionU)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionV)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionW)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionX)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionY)
                            : hb.Td(column: column, value: string.Empty);
                    case "DescriptionZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.DescriptionZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckA)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckB)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckC)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckD)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckE)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckF)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckG)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckH)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckI)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckK)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckL)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckM)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckN)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckO)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckP)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckR)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckS)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckT)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckU)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckV)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckW)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckX)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckY)
                            : hb.Td(column: column, value: string.Empty);
                    case "CheckZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CheckZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsA":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsA)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsB":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsB)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsC":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsC)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsD":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsD)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsE":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsE)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsF":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsF)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsG":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsG)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsH":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsH)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsI":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsI)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsJ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsJ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsK":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsK)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsL":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsL)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsM":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsM)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsN":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsN)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsO":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsO)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsP":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsP)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsQ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsQ)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsR":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsR)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsS":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsS)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsT":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsT)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsU":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsU)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsV":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsV)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsW":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsW)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsX":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsX)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsY":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsY)
                            : hb.Td(column: column, value: string.Empty);
                    case "AttachmentsZ":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.AttachmentsZ)
                            : hb.Td(column: column, value: string.Empty);
                    case "SiteTitle":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.SiteTitle)
                            : hb.Td(column: column, value: string.Empty);
                    case "Comments":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Comments)
                            : hb.Td(column: column, value: string.Empty);
                    case "Creator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Creator)
                            : hb.Td(column: column, value: string.Empty);
                    case "Updator":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.Updator)
                            : hb.Td(column: column, value: string.Empty);
                    case "CreatedTime":
                        return ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                            ? hb.Td(column: column, value: resultModel.CreatedTime)
                            : hb.Td(column: column, value: string.Empty);
                    default: return hb;
                }
            }
        }

        private static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, ResultModel resultModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.Name)
                {
                    case "SiteId": value = resultModel.SiteId.GridText(column: column); break;
                    case "UpdatedTime": value = resultModel.UpdatedTime.GridText(column: column); break;
                    case "ResultId": value = resultModel.ResultId.GridText(column: column); break;
                    case "Ver": value = resultModel.Ver.GridText(column: column); break;
                    case "Title": value = resultModel.Title.GridText(column: column); break;
                    case "Body": value = resultModel.Body.GridText(column: column); break;
                    case "TitleBody": value = resultModel.TitleBody.GridText(column: column); break;
                    case "Status": value = resultModel.Status.GridText(column: column); break;
                    case "Manager": value = resultModel.Manager.GridText(column: column); break;
                    case "Owner": value = resultModel.Owner.GridText(column: column); break;
                    case "ClassA": value = resultModel.ClassA.GridText(column: column); break;
                    case "ClassB": value = resultModel.ClassB.GridText(column: column); break;
                    case "ClassC": value = resultModel.ClassC.GridText(column: column); break;
                    case "ClassD": value = resultModel.ClassD.GridText(column: column); break;
                    case "ClassE": value = resultModel.ClassE.GridText(column: column); break;
                    case "ClassF": value = resultModel.ClassF.GridText(column: column); break;
                    case "ClassG": value = resultModel.ClassG.GridText(column: column); break;
                    case "ClassH": value = resultModel.ClassH.GridText(column: column); break;
                    case "ClassI": value = resultModel.ClassI.GridText(column: column); break;
                    case "ClassJ": value = resultModel.ClassJ.GridText(column: column); break;
                    case "ClassK": value = resultModel.ClassK.GridText(column: column); break;
                    case "ClassL": value = resultModel.ClassL.GridText(column: column); break;
                    case "ClassM": value = resultModel.ClassM.GridText(column: column); break;
                    case "ClassN": value = resultModel.ClassN.GridText(column: column); break;
                    case "ClassO": value = resultModel.ClassO.GridText(column: column); break;
                    case "ClassP": value = resultModel.ClassP.GridText(column: column); break;
                    case "ClassQ": value = resultModel.ClassQ.GridText(column: column); break;
                    case "ClassR": value = resultModel.ClassR.GridText(column: column); break;
                    case "ClassS": value = resultModel.ClassS.GridText(column: column); break;
                    case "ClassT": value = resultModel.ClassT.GridText(column: column); break;
                    case "ClassU": value = resultModel.ClassU.GridText(column: column); break;
                    case "ClassV": value = resultModel.ClassV.GridText(column: column); break;
                    case "ClassW": value = resultModel.ClassW.GridText(column: column); break;
                    case "ClassX": value = resultModel.ClassX.GridText(column: column); break;
                    case "ClassY": value = resultModel.ClassY.GridText(column: column); break;
                    case "ClassZ": value = resultModel.ClassZ.GridText(column: column); break;
                    case "NumA": value = resultModel.NumA.GridText(column: column); break;
                    case "NumB": value = resultModel.NumB.GridText(column: column); break;
                    case "NumC": value = resultModel.NumC.GridText(column: column); break;
                    case "NumD": value = resultModel.NumD.GridText(column: column); break;
                    case "NumE": value = resultModel.NumE.GridText(column: column); break;
                    case "NumF": value = resultModel.NumF.GridText(column: column); break;
                    case "NumG": value = resultModel.NumG.GridText(column: column); break;
                    case "NumH": value = resultModel.NumH.GridText(column: column); break;
                    case "NumI": value = resultModel.NumI.GridText(column: column); break;
                    case "NumJ": value = resultModel.NumJ.GridText(column: column); break;
                    case "NumK": value = resultModel.NumK.GridText(column: column); break;
                    case "NumL": value = resultModel.NumL.GridText(column: column); break;
                    case "NumM": value = resultModel.NumM.GridText(column: column); break;
                    case "NumN": value = resultModel.NumN.GridText(column: column); break;
                    case "NumO": value = resultModel.NumO.GridText(column: column); break;
                    case "NumP": value = resultModel.NumP.GridText(column: column); break;
                    case "NumQ": value = resultModel.NumQ.GridText(column: column); break;
                    case "NumR": value = resultModel.NumR.GridText(column: column); break;
                    case "NumS": value = resultModel.NumS.GridText(column: column); break;
                    case "NumT": value = resultModel.NumT.GridText(column: column); break;
                    case "NumU": value = resultModel.NumU.GridText(column: column); break;
                    case "NumV": value = resultModel.NumV.GridText(column: column); break;
                    case "NumW": value = resultModel.NumW.GridText(column: column); break;
                    case "NumX": value = resultModel.NumX.GridText(column: column); break;
                    case "NumY": value = resultModel.NumY.GridText(column: column); break;
                    case "NumZ": value = resultModel.NumZ.GridText(column: column); break;
                    case "DateA": value = resultModel.DateA.GridText(column: column); break;
                    case "DateB": value = resultModel.DateB.GridText(column: column); break;
                    case "DateC": value = resultModel.DateC.GridText(column: column); break;
                    case "DateD": value = resultModel.DateD.GridText(column: column); break;
                    case "DateE": value = resultModel.DateE.GridText(column: column); break;
                    case "DateF": value = resultModel.DateF.GridText(column: column); break;
                    case "DateG": value = resultModel.DateG.GridText(column: column); break;
                    case "DateH": value = resultModel.DateH.GridText(column: column); break;
                    case "DateI": value = resultModel.DateI.GridText(column: column); break;
                    case "DateJ": value = resultModel.DateJ.GridText(column: column); break;
                    case "DateK": value = resultModel.DateK.GridText(column: column); break;
                    case "DateL": value = resultModel.DateL.GridText(column: column); break;
                    case "DateM": value = resultModel.DateM.GridText(column: column); break;
                    case "DateN": value = resultModel.DateN.GridText(column: column); break;
                    case "DateO": value = resultModel.DateO.GridText(column: column); break;
                    case "DateP": value = resultModel.DateP.GridText(column: column); break;
                    case "DateQ": value = resultModel.DateQ.GridText(column: column); break;
                    case "DateR": value = resultModel.DateR.GridText(column: column); break;
                    case "DateS": value = resultModel.DateS.GridText(column: column); break;
                    case "DateT": value = resultModel.DateT.GridText(column: column); break;
                    case "DateU": value = resultModel.DateU.GridText(column: column); break;
                    case "DateV": value = resultModel.DateV.GridText(column: column); break;
                    case "DateW": value = resultModel.DateW.GridText(column: column); break;
                    case "DateX": value = resultModel.DateX.GridText(column: column); break;
                    case "DateY": value = resultModel.DateY.GridText(column: column); break;
                    case "DateZ": value = resultModel.DateZ.GridText(column: column); break;
                    case "DescriptionA": value = resultModel.DescriptionA.GridText(column: column); break;
                    case "DescriptionB": value = resultModel.DescriptionB.GridText(column: column); break;
                    case "DescriptionC": value = resultModel.DescriptionC.GridText(column: column); break;
                    case "DescriptionD": value = resultModel.DescriptionD.GridText(column: column); break;
                    case "DescriptionE": value = resultModel.DescriptionE.GridText(column: column); break;
                    case "DescriptionF": value = resultModel.DescriptionF.GridText(column: column); break;
                    case "DescriptionG": value = resultModel.DescriptionG.GridText(column: column); break;
                    case "DescriptionH": value = resultModel.DescriptionH.GridText(column: column); break;
                    case "DescriptionI": value = resultModel.DescriptionI.GridText(column: column); break;
                    case "DescriptionJ": value = resultModel.DescriptionJ.GridText(column: column); break;
                    case "DescriptionK": value = resultModel.DescriptionK.GridText(column: column); break;
                    case "DescriptionL": value = resultModel.DescriptionL.GridText(column: column); break;
                    case "DescriptionM": value = resultModel.DescriptionM.GridText(column: column); break;
                    case "DescriptionN": value = resultModel.DescriptionN.GridText(column: column); break;
                    case "DescriptionO": value = resultModel.DescriptionO.GridText(column: column); break;
                    case "DescriptionP": value = resultModel.DescriptionP.GridText(column: column); break;
                    case "DescriptionQ": value = resultModel.DescriptionQ.GridText(column: column); break;
                    case "DescriptionR": value = resultModel.DescriptionR.GridText(column: column); break;
                    case "DescriptionS": value = resultModel.DescriptionS.GridText(column: column); break;
                    case "DescriptionT": value = resultModel.DescriptionT.GridText(column: column); break;
                    case "DescriptionU": value = resultModel.DescriptionU.GridText(column: column); break;
                    case "DescriptionV": value = resultModel.DescriptionV.GridText(column: column); break;
                    case "DescriptionW": value = resultModel.DescriptionW.GridText(column: column); break;
                    case "DescriptionX": value = resultModel.DescriptionX.GridText(column: column); break;
                    case "DescriptionY": value = resultModel.DescriptionY.GridText(column: column); break;
                    case "DescriptionZ": value = resultModel.DescriptionZ.GridText(column: column); break;
                    case "CheckA": value = resultModel.CheckA.GridText(column: column); break;
                    case "CheckB": value = resultModel.CheckB.GridText(column: column); break;
                    case "CheckC": value = resultModel.CheckC.GridText(column: column); break;
                    case "CheckD": value = resultModel.CheckD.GridText(column: column); break;
                    case "CheckE": value = resultModel.CheckE.GridText(column: column); break;
                    case "CheckF": value = resultModel.CheckF.GridText(column: column); break;
                    case "CheckG": value = resultModel.CheckG.GridText(column: column); break;
                    case "CheckH": value = resultModel.CheckH.GridText(column: column); break;
                    case "CheckI": value = resultModel.CheckI.GridText(column: column); break;
                    case "CheckJ": value = resultModel.CheckJ.GridText(column: column); break;
                    case "CheckK": value = resultModel.CheckK.GridText(column: column); break;
                    case "CheckL": value = resultModel.CheckL.GridText(column: column); break;
                    case "CheckM": value = resultModel.CheckM.GridText(column: column); break;
                    case "CheckN": value = resultModel.CheckN.GridText(column: column); break;
                    case "CheckO": value = resultModel.CheckO.GridText(column: column); break;
                    case "CheckP": value = resultModel.CheckP.GridText(column: column); break;
                    case "CheckQ": value = resultModel.CheckQ.GridText(column: column); break;
                    case "CheckR": value = resultModel.CheckR.GridText(column: column); break;
                    case "CheckS": value = resultModel.CheckS.GridText(column: column); break;
                    case "CheckT": value = resultModel.CheckT.GridText(column: column); break;
                    case "CheckU": value = resultModel.CheckU.GridText(column: column); break;
                    case "CheckV": value = resultModel.CheckV.GridText(column: column); break;
                    case "CheckW": value = resultModel.CheckW.GridText(column: column); break;
                    case "CheckX": value = resultModel.CheckX.GridText(column: column); break;
                    case "CheckY": value = resultModel.CheckY.GridText(column: column); break;
                    case "CheckZ": value = resultModel.CheckZ.GridText(column: column); break;
                    case "AttachmentsA": value = resultModel.AttachmentsA.GridText(column: column); break;
                    case "AttachmentsB": value = resultModel.AttachmentsB.GridText(column: column); break;
                    case "AttachmentsC": value = resultModel.AttachmentsC.GridText(column: column); break;
                    case "AttachmentsD": value = resultModel.AttachmentsD.GridText(column: column); break;
                    case "AttachmentsE": value = resultModel.AttachmentsE.GridText(column: column); break;
                    case "AttachmentsF": value = resultModel.AttachmentsF.GridText(column: column); break;
                    case "AttachmentsG": value = resultModel.AttachmentsG.GridText(column: column); break;
                    case "AttachmentsH": value = resultModel.AttachmentsH.GridText(column: column); break;
                    case "AttachmentsI": value = resultModel.AttachmentsI.GridText(column: column); break;
                    case "AttachmentsJ": value = resultModel.AttachmentsJ.GridText(column: column); break;
                    case "AttachmentsK": value = resultModel.AttachmentsK.GridText(column: column); break;
                    case "AttachmentsL": value = resultModel.AttachmentsL.GridText(column: column); break;
                    case "AttachmentsM": value = resultModel.AttachmentsM.GridText(column: column); break;
                    case "AttachmentsN": value = resultModel.AttachmentsN.GridText(column: column); break;
                    case "AttachmentsO": value = resultModel.AttachmentsO.GridText(column: column); break;
                    case "AttachmentsP": value = resultModel.AttachmentsP.GridText(column: column); break;
                    case "AttachmentsQ": value = resultModel.AttachmentsQ.GridText(column: column); break;
                    case "AttachmentsR": value = resultModel.AttachmentsR.GridText(column: column); break;
                    case "AttachmentsS": value = resultModel.AttachmentsS.GridText(column: column); break;
                    case "AttachmentsT": value = resultModel.AttachmentsT.GridText(column: column); break;
                    case "AttachmentsU": value = resultModel.AttachmentsU.GridText(column: column); break;
                    case "AttachmentsV": value = resultModel.AttachmentsV.GridText(column: column); break;
                    case "AttachmentsW": value = resultModel.AttachmentsW.GridText(column: column); break;
                    case "AttachmentsX": value = resultModel.AttachmentsX.GridText(column: column); break;
                    case "AttachmentsY": value = resultModel.AttachmentsY.GridText(column: column); break;
                    case "AttachmentsZ": value = resultModel.AttachmentsZ.GridText(column: column); break;
                    case "SiteTitle": value = resultModel.SiteTitle.GridText(column: column); break;
                    case "Comments": value = resultModel.Comments.GridText(column: column); break;
                    case "Creator": value = resultModel.Creator.GridText(column: column); break;
                    case "Updator": value = resultModel.Updator.GridText(column: column); break;
                    case "CreatedTime": value = resultModel.CreatedTime.GridText(column: column); break;
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
            return Editor(ss, new ResultModel(ss, methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(SiteSettings ss, long resultId, bool clearSessions)
        {
            var resultModel = new ResultModel(
                ss: ss,
                resultId: resultId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            resultModel.SwitchTargets = GetSwitchTargets(
                ss, resultModel.ResultId, resultModel.SiteId);
            return Editor(ss, resultModel);
        }

        public static string Editor(SiteSettings ss, ResultModel resultModel)
        {
            var invalid = ResultValidators.OnEditing(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return HtmlTemplates.Error(invalid);
            }
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(resultModel.Mine());
            return hb.Template(
                ss: ss,
                verType: resultModel.VerType,
                methodType: resultModel.MethodType,
                siteId: resultModel.SiteId,
                parentId: ss.ParentId,
                referenceType: "Results",
                title: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.New()
                    : resultModel.Title.DisplayValue,
                useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
                userScript: ss.EditorScripts(resultModel.MethodType),
                userStyle: ss.EditorStyles(resultModel.MethodType),
                action: () => hb
                    .Editor(
                        ss: ss,
                        resultModel: resultModel)
                    .Hidden(controlId: "TableName", value: "Results")
                    .Hidden(controlId: "Id", value: resultModel.ResultId.ToString()))
                        .ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.ShowComments(commentsColumnPermissionType);
            var tabsCss = showComments ? null : "max";
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("ResultForm")
                        .Class("main-form confirm-reload")
                        .Action(Locations.ItemAction(resultModel.ResultId != 0 
                            ? resultModel.ResultId
                            : resultModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: resultModel,
                            tableName: "Results")
                        .Div(
                            id: "EditorComments", action: () => hb
                                .Comments(
                                    comments: resultModel.Comments,
                                    column: commentsColumn,
                                    verType: resultModel.VerType,
                                    columnPermissionType: commentsColumnPermissionType),
                            _using: showComments)
                        .Div(id: "EditorTabsContainer", css: tabsCss, action: () => hb
                            .EditorTabs(resultModel: resultModel, ss: ss)
                            .FieldSetGeneral(
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("post"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetRecordAccessControl")
                                    .DataAction("Permissions")
                                    .DataMethod("post"),
                                _using: ss.CanManagePermission() &&
                                    resultModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
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
                                        resultModel: resultModel,
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
                            value: resultModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Results_Timestamp",
                            css: "always-send",
                            value: resultModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "always-send",
                            value: resultModel.SwitchTargets?.Join(),
                            _using: !Request.IsAjax()))
                .OutgoingMailsForm("Results", resultModel.ResultId, resultModel.Ver)
                .DropDownSearchDialog("items", ss.SiteId)
                .CopyDialog("items", resultModel.ResultId)
                .MoveDialog()
                .OutgoingMailDialog()
                .PermissionsDialog()
                .EditorExtensions(resultModel: resultModel, ss: ss));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb, SiteSettings ss, ResultModel resultModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.General()))
                .Li(_using: resultModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList()))
                .Li(_using: ss.CanManagePermission() &&
                        resultModel.MethodType != BaseModel.MethodTypes.New,
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
                                ss: ss, resultModel: new ResultModel(), preview: true))
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
            ResultModel resultModel)
        {
            var mine = resultModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () => hb
                .FieldSetGeneralColumns(
                    ss: ss, resultModel: resultModel));
        }

        public static HtmlBuilder FieldSetGeneralColumns(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultModel resultModel,
            bool preview = false)
        {
            ss.GetEditorColumns().ForEach(column =>
            {
                switch (column.Name)
                {
                    case "ResultId":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ResultId.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Ver":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Ver.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Title":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Title.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Body":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Body.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Status":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Status.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Manager":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Manager.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "Owner":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.Owner.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "ClassZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.ClassZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "NumZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.NumZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DateZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DateZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "DescriptionZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.DescriptionZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "CheckZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.CheckZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsA":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsA.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsB":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsB.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsC":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsC.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsD":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsD.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsE":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsE.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsF":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsF.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsG":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsG.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsH":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsH.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsI":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsI.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsJ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsJ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsK":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsK.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsL":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsL.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsM":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsM.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsN":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsN.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsO":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsO.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsP":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsP.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsQ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsQ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsR":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsR.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsS":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsS.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsT":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsT.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsU":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsU.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsV":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsV.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsW":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsW.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsX":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsX.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsY":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsY.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                    case "AttachmentsZ":
                        hb.Field(
                            ss,
                            column,
                            resultModel.MethodType,
                            resultModel.AttachmentsZ.ToControl(ss, column),
                            column.ColumnPermissionType(),
                            preview: preview);
                        break;
                }
            });
            if (!preview)
            {
                hb.VerUpCheckBox(resultModel);
                    hb
                        .Div(id: "LinkCreations", css: "links", action: () => hb
                            .LinkCreations(
                                ss: ss,
                                linkId: resultModel.ResultId,
                                methodType: resultModel.MethodType))
                        .Div(id: "Links", css: "links", action: () => hb
                            .Links(ss: ss, id: resultModel.ResultId));
            }
            return hb;
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb, SiteSettings ss, ResultModel resultModel)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb, SiteSettings ss, ResultModel resultModel)
        {
            return hb;
        }

        public static string EditorJson(SiteSettings ss, long resultId)
        {
            return EditorResponse(ss, new ResultModel(ss, resultId)).ToJson();
        }

        private static ResponseCollection EditorResponse(
            SiteSettings ss, 
            ResultModel resultModel,
            Message message = null,
            string switchTargets = null)
        {
            resultModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ResultsResponseCollection(resultModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, resultModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<long> GetSwitchTargets(SiteSettings ss, long resultId, long siteId)
        {
            var view = Views.GetBySession(ss);
            var where = view.Where(ss: ss);
            var join = ss.Join();
            var switchTargets = Rds.ExecuteScalar_int(statements:
                Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: join,
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultId(),
                            join: join,
                            where: where,
                            orderBy: view.OrderBy(ss).Results_UpdatedTime(SqlOrderBy.Types.desc)))
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
            this ResponseCollection res,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var mine = resultModel.Mine();
            ss.EditorColumns
                .Select(o => ss.GetColumn(o))
                .Where(o => o != null)
                .ForEach(column =>
                {
                    switch (column.Name)
                    {
                        case "SiteId":
                            res.Val(
                                "#Results_SiteId",
                                resultModel.SiteId.ToControl(ss, column));
                            break;
                        case "UpdatedTime":
                            res.Val(
                                "#Results_UpdatedTime",
                                resultModel.UpdatedTime.ToControl(ss, column));
                            break;
                        case "ResultId":
                            res.Val(
                                "#Results_ResultId",
                                resultModel.ResultId.ToControl(ss, column));
                            break;
                        case "Ver":
                            res.Val(
                                "#Results_Ver",
                                resultModel.Ver.ToControl(ss, column));
                            break;
                        case "Title":
                            res.Val(
                                "#Results_Title",
                                resultModel.Title.ToControl(ss, column));
                            break;
                        case "Body":
                            res.Val(
                                "#Results_Body",
                                resultModel.Body.ToControl(ss, column));
                            break;
                        case "Status":
                            res.Val(
                                "#Results_Status",
                                resultModel.Status.ToControl(ss, column));
                            break;
                        case "Manager":
                            res.Val(
                                "#Results_Manager",
                                resultModel.Manager.ToControl(ss, column));
                            break;
                        case "Owner":
                            res.Val(
                                "#Results_Owner",
                                resultModel.Owner.ToControl(ss, column));
                            break;
                        case "ClassA":
                            res.Val(
                                "#Results_ClassA",
                                resultModel.ClassA.ToControl(ss, column));
                            break;
                        case "ClassB":
                            res.Val(
                                "#Results_ClassB",
                                resultModel.ClassB.ToControl(ss, column));
                            break;
                        case "ClassC":
                            res.Val(
                                "#Results_ClassC",
                                resultModel.ClassC.ToControl(ss, column));
                            break;
                        case "ClassD":
                            res.Val(
                                "#Results_ClassD",
                                resultModel.ClassD.ToControl(ss, column));
                            break;
                        case "ClassE":
                            res.Val(
                                "#Results_ClassE",
                                resultModel.ClassE.ToControl(ss, column));
                            break;
                        case "ClassF":
                            res.Val(
                                "#Results_ClassF",
                                resultModel.ClassF.ToControl(ss, column));
                            break;
                        case "ClassG":
                            res.Val(
                                "#Results_ClassG",
                                resultModel.ClassG.ToControl(ss, column));
                            break;
                        case "ClassH":
                            res.Val(
                                "#Results_ClassH",
                                resultModel.ClassH.ToControl(ss, column));
                            break;
                        case "ClassI":
                            res.Val(
                                "#Results_ClassI",
                                resultModel.ClassI.ToControl(ss, column));
                            break;
                        case "ClassJ":
                            res.Val(
                                "#Results_ClassJ",
                                resultModel.ClassJ.ToControl(ss, column));
                            break;
                        case "ClassK":
                            res.Val(
                                "#Results_ClassK",
                                resultModel.ClassK.ToControl(ss, column));
                            break;
                        case "ClassL":
                            res.Val(
                                "#Results_ClassL",
                                resultModel.ClassL.ToControl(ss, column));
                            break;
                        case "ClassM":
                            res.Val(
                                "#Results_ClassM",
                                resultModel.ClassM.ToControl(ss, column));
                            break;
                        case "ClassN":
                            res.Val(
                                "#Results_ClassN",
                                resultModel.ClassN.ToControl(ss, column));
                            break;
                        case "ClassO":
                            res.Val(
                                "#Results_ClassO",
                                resultModel.ClassO.ToControl(ss, column));
                            break;
                        case "ClassP":
                            res.Val(
                                "#Results_ClassP",
                                resultModel.ClassP.ToControl(ss, column));
                            break;
                        case "ClassQ":
                            res.Val(
                                "#Results_ClassQ",
                                resultModel.ClassQ.ToControl(ss, column));
                            break;
                        case "ClassR":
                            res.Val(
                                "#Results_ClassR",
                                resultModel.ClassR.ToControl(ss, column));
                            break;
                        case "ClassS":
                            res.Val(
                                "#Results_ClassS",
                                resultModel.ClassS.ToControl(ss, column));
                            break;
                        case "ClassT":
                            res.Val(
                                "#Results_ClassT",
                                resultModel.ClassT.ToControl(ss, column));
                            break;
                        case "ClassU":
                            res.Val(
                                "#Results_ClassU",
                                resultModel.ClassU.ToControl(ss, column));
                            break;
                        case "ClassV":
                            res.Val(
                                "#Results_ClassV",
                                resultModel.ClassV.ToControl(ss, column));
                            break;
                        case "ClassW":
                            res.Val(
                                "#Results_ClassW",
                                resultModel.ClassW.ToControl(ss, column));
                            break;
                        case "ClassX":
                            res.Val(
                                "#Results_ClassX",
                                resultModel.ClassX.ToControl(ss, column));
                            break;
                        case "ClassY":
                            res.Val(
                                "#Results_ClassY",
                                resultModel.ClassY.ToControl(ss, column));
                            break;
                        case "ClassZ":
                            res.Val(
                                "#Results_ClassZ",
                                resultModel.ClassZ.ToControl(ss, column));
                            break;
                        case "NumA":
                            res.Val(
                                "#Results_NumA",
                                resultModel.NumA.ToControl(ss, column));
                            break;
                        case "NumB":
                            res.Val(
                                "#Results_NumB",
                                resultModel.NumB.ToControl(ss, column));
                            break;
                        case "NumC":
                            res.Val(
                                "#Results_NumC",
                                resultModel.NumC.ToControl(ss, column));
                            break;
                        case "NumD":
                            res.Val(
                                "#Results_NumD",
                                resultModel.NumD.ToControl(ss, column));
                            break;
                        case "NumE":
                            res.Val(
                                "#Results_NumE",
                                resultModel.NumE.ToControl(ss, column));
                            break;
                        case "NumF":
                            res.Val(
                                "#Results_NumF",
                                resultModel.NumF.ToControl(ss, column));
                            break;
                        case "NumG":
                            res.Val(
                                "#Results_NumG",
                                resultModel.NumG.ToControl(ss, column));
                            break;
                        case "NumH":
                            res.Val(
                                "#Results_NumH",
                                resultModel.NumH.ToControl(ss, column));
                            break;
                        case "NumI":
                            res.Val(
                                "#Results_NumI",
                                resultModel.NumI.ToControl(ss, column));
                            break;
                        case "NumJ":
                            res.Val(
                                "#Results_NumJ",
                                resultModel.NumJ.ToControl(ss, column));
                            break;
                        case "NumK":
                            res.Val(
                                "#Results_NumK",
                                resultModel.NumK.ToControl(ss, column));
                            break;
                        case "NumL":
                            res.Val(
                                "#Results_NumL",
                                resultModel.NumL.ToControl(ss, column));
                            break;
                        case "NumM":
                            res.Val(
                                "#Results_NumM",
                                resultModel.NumM.ToControl(ss, column));
                            break;
                        case "NumN":
                            res.Val(
                                "#Results_NumN",
                                resultModel.NumN.ToControl(ss, column));
                            break;
                        case "NumO":
                            res.Val(
                                "#Results_NumO",
                                resultModel.NumO.ToControl(ss, column));
                            break;
                        case "NumP":
                            res.Val(
                                "#Results_NumP",
                                resultModel.NumP.ToControl(ss, column));
                            break;
                        case "NumQ":
                            res.Val(
                                "#Results_NumQ",
                                resultModel.NumQ.ToControl(ss, column));
                            break;
                        case "NumR":
                            res.Val(
                                "#Results_NumR",
                                resultModel.NumR.ToControl(ss, column));
                            break;
                        case "NumS":
                            res.Val(
                                "#Results_NumS",
                                resultModel.NumS.ToControl(ss, column));
                            break;
                        case "NumT":
                            res.Val(
                                "#Results_NumT",
                                resultModel.NumT.ToControl(ss, column));
                            break;
                        case "NumU":
                            res.Val(
                                "#Results_NumU",
                                resultModel.NumU.ToControl(ss, column));
                            break;
                        case "NumV":
                            res.Val(
                                "#Results_NumV",
                                resultModel.NumV.ToControl(ss, column));
                            break;
                        case "NumW":
                            res.Val(
                                "#Results_NumW",
                                resultModel.NumW.ToControl(ss, column));
                            break;
                        case "NumX":
                            res.Val(
                                "#Results_NumX",
                                resultModel.NumX.ToControl(ss, column));
                            break;
                        case "NumY":
                            res.Val(
                                "#Results_NumY",
                                resultModel.NumY.ToControl(ss, column));
                            break;
                        case "NumZ":
                            res.Val(
                                "#Results_NumZ",
                                resultModel.NumZ.ToControl(ss, column));
                            break;
                        case "DateA":
                            res.Val(
                                "#Results_DateA",
                                resultModel.DateA.ToControl(ss, column));
                            break;
                        case "DateB":
                            res.Val(
                                "#Results_DateB",
                                resultModel.DateB.ToControl(ss, column));
                            break;
                        case "DateC":
                            res.Val(
                                "#Results_DateC",
                                resultModel.DateC.ToControl(ss, column));
                            break;
                        case "DateD":
                            res.Val(
                                "#Results_DateD",
                                resultModel.DateD.ToControl(ss, column));
                            break;
                        case "DateE":
                            res.Val(
                                "#Results_DateE",
                                resultModel.DateE.ToControl(ss, column));
                            break;
                        case "DateF":
                            res.Val(
                                "#Results_DateF",
                                resultModel.DateF.ToControl(ss, column));
                            break;
                        case "DateG":
                            res.Val(
                                "#Results_DateG",
                                resultModel.DateG.ToControl(ss, column));
                            break;
                        case "DateH":
                            res.Val(
                                "#Results_DateH",
                                resultModel.DateH.ToControl(ss, column));
                            break;
                        case "DateI":
                            res.Val(
                                "#Results_DateI",
                                resultModel.DateI.ToControl(ss, column));
                            break;
                        case "DateJ":
                            res.Val(
                                "#Results_DateJ",
                                resultModel.DateJ.ToControl(ss, column));
                            break;
                        case "DateK":
                            res.Val(
                                "#Results_DateK",
                                resultModel.DateK.ToControl(ss, column));
                            break;
                        case "DateL":
                            res.Val(
                                "#Results_DateL",
                                resultModel.DateL.ToControl(ss, column));
                            break;
                        case "DateM":
                            res.Val(
                                "#Results_DateM",
                                resultModel.DateM.ToControl(ss, column));
                            break;
                        case "DateN":
                            res.Val(
                                "#Results_DateN",
                                resultModel.DateN.ToControl(ss, column));
                            break;
                        case "DateO":
                            res.Val(
                                "#Results_DateO",
                                resultModel.DateO.ToControl(ss, column));
                            break;
                        case "DateP":
                            res.Val(
                                "#Results_DateP",
                                resultModel.DateP.ToControl(ss, column));
                            break;
                        case "DateQ":
                            res.Val(
                                "#Results_DateQ",
                                resultModel.DateQ.ToControl(ss, column));
                            break;
                        case "DateR":
                            res.Val(
                                "#Results_DateR",
                                resultModel.DateR.ToControl(ss, column));
                            break;
                        case "DateS":
                            res.Val(
                                "#Results_DateS",
                                resultModel.DateS.ToControl(ss, column));
                            break;
                        case "DateT":
                            res.Val(
                                "#Results_DateT",
                                resultModel.DateT.ToControl(ss, column));
                            break;
                        case "DateU":
                            res.Val(
                                "#Results_DateU",
                                resultModel.DateU.ToControl(ss, column));
                            break;
                        case "DateV":
                            res.Val(
                                "#Results_DateV",
                                resultModel.DateV.ToControl(ss, column));
                            break;
                        case "DateW":
                            res.Val(
                                "#Results_DateW",
                                resultModel.DateW.ToControl(ss, column));
                            break;
                        case "DateX":
                            res.Val(
                                "#Results_DateX",
                                resultModel.DateX.ToControl(ss, column));
                            break;
                        case "DateY":
                            res.Val(
                                "#Results_DateY",
                                resultModel.DateY.ToControl(ss, column));
                            break;
                        case "DateZ":
                            res.Val(
                                "#Results_DateZ",
                                resultModel.DateZ.ToControl(ss, column));
                            break;
                        case "DescriptionA":
                            res.Val(
                                "#Results_DescriptionA",
                                resultModel.DescriptionA.ToControl(ss, column));
                            break;
                        case "DescriptionB":
                            res.Val(
                                "#Results_DescriptionB",
                                resultModel.DescriptionB.ToControl(ss, column));
                            break;
                        case "DescriptionC":
                            res.Val(
                                "#Results_DescriptionC",
                                resultModel.DescriptionC.ToControl(ss, column));
                            break;
                        case "DescriptionD":
                            res.Val(
                                "#Results_DescriptionD",
                                resultModel.DescriptionD.ToControl(ss, column));
                            break;
                        case "DescriptionE":
                            res.Val(
                                "#Results_DescriptionE",
                                resultModel.DescriptionE.ToControl(ss, column));
                            break;
                        case "DescriptionF":
                            res.Val(
                                "#Results_DescriptionF",
                                resultModel.DescriptionF.ToControl(ss, column));
                            break;
                        case "DescriptionG":
                            res.Val(
                                "#Results_DescriptionG",
                                resultModel.DescriptionG.ToControl(ss, column));
                            break;
                        case "DescriptionH":
                            res.Val(
                                "#Results_DescriptionH",
                                resultModel.DescriptionH.ToControl(ss, column));
                            break;
                        case "DescriptionI":
                            res.Val(
                                "#Results_DescriptionI",
                                resultModel.DescriptionI.ToControl(ss, column));
                            break;
                        case "DescriptionJ":
                            res.Val(
                                "#Results_DescriptionJ",
                                resultModel.DescriptionJ.ToControl(ss, column));
                            break;
                        case "DescriptionK":
                            res.Val(
                                "#Results_DescriptionK",
                                resultModel.DescriptionK.ToControl(ss, column));
                            break;
                        case "DescriptionL":
                            res.Val(
                                "#Results_DescriptionL",
                                resultModel.DescriptionL.ToControl(ss, column));
                            break;
                        case "DescriptionM":
                            res.Val(
                                "#Results_DescriptionM",
                                resultModel.DescriptionM.ToControl(ss, column));
                            break;
                        case "DescriptionN":
                            res.Val(
                                "#Results_DescriptionN",
                                resultModel.DescriptionN.ToControl(ss, column));
                            break;
                        case "DescriptionO":
                            res.Val(
                                "#Results_DescriptionO",
                                resultModel.DescriptionO.ToControl(ss, column));
                            break;
                        case "DescriptionP":
                            res.Val(
                                "#Results_DescriptionP",
                                resultModel.DescriptionP.ToControl(ss, column));
                            break;
                        case "DescriptionQ":
                            res.Val(
                                "#Results_DescriptionQ",
                                resultModel.DescriptionQ.ToControl(ss, column));
                            break;
                        case "DescriptionR":
                            res.Val(
                                "#Results_DescriptionR",
                                resultModel.DescriptionR.ToControl(ss, column));
                            break;
                        case "DescriptionS":
                            res.Val(
                                "#Results_DescriptionS",
                                resultModel.DescriptionS.ToControl(ss, column));
                            break;
                        case "DescriptionT":
                            res.Val(
                                "#Results_DescriptionT",
                                resultModel.DescriptionT.ToControl(ss, column));
                            break;
                        case "DescriptionU":
                            res.Val(
                                "#Results_DescriptionU",
                                resultModel.DescriptionU.ToControl(ss, column));
                            break;
                        case "DescriptionV":
                            res.Val(
                                "#Results_DescriptionV",
                                resultModel.DescriptionV.ToControl(ss, column));
                            break;
                        case "DescriptionW":
                            res.Val(
                                "#Results_DescriptionW",
                                resultModel.DescriptionW.ToControl(ss, column));
                            break;
                        case "DescriptionX":
                            res.Val(
                                "#Results_DescriptionX",
                                resultModel.DescriptionX.ToControl(ss, column));
                            break;
                        case "DescriptionY":
                            res.Val(
                                "#Results_DescriptionY",
                                resultModel.DescriptionY.ToControl(ss, column));
                            break;
                        case "DescriptionZ":
                            res.Val(
                                "#Results_DescriptionZ",
                                resultModel.DescriptionZ.ToControl(ss, column));
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
                        case "Comments":
                            res.Val(
                                "#Results_Comments",
                                resultModel.Comments.ToControl(ss, column));
                            break;
                        case "Creator":
                            res.Val(
                                "#Results_Creator",
                                resultModel.Creator.ToControl(ss, column));
                            break;
                        case "Updator":
                            res.Val(
                                "#Results_Updator",
                                resultModel.Updator.ToControl(ss, column));
                            break;
                        case "CreatedTime":
                            res.Val(
                                "#Results_CreatedTime",
                                resultModel.CreatedTime.ToControl(ss, column));
                            break;
                        case "AttachmentsA":
                            res.ReplaceAll(
                                "#Results_AttachmentsAField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsA.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsB":
                            res.ReplaceAll(
                                "#Results_AttachmentsBField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsB.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsC":
                            res.ReplaceAll(
                                "#Results_AttachmentsCField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsC.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsD":
                            res.ReplaceAll(
                                "#Results_AttachmentsDField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsD.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsE":
                            res.ReplaceAll(
                                "#Results_AttachmentsEField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsE.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsF":
                            res.ReplaceAll(
                                "#Results_AttachmentsFField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsF.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsG":
                            res.ReplaceAll(
                                "#Results_AttachmentsGField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsG.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsH":
                            res.ReplaceAll(
                                "#Results_AttachmentsHField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsH.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsI":
                            res.ReplaceAll(
                                "#Results_AttachmentsIField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsI.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsJ":
                            res.ReplaceAll(
                                "#Results_AttachmentsJField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsJ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsK":
                            res.ReplaceAll(
                                "#Results_AttachmentsKField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsK.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsL":
                            res.ReplaceAll(
                                "#Results_AttachmentsLField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsL.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsM":
                            res.ReplaceAll(
                                "#Results_AttachmentsMField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsM.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsN":
                            res.ReplaceAll(
                                "#Results_AttachmentsNField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsN.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsO":
                            res.ReplaceAll(
                                "#Results_AttachmentsOField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsO.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsP":
                            res.ReplaceAll(
                                "#Results_AttachmentsPField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsP.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsQ":
                            res.ReplaceAll(
                                "#Results_AttachmentsQField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsQ.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsR":
                            res.ReplaceAll(
                                "#Results_AttachmentsRField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsR.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsS":
                            res.ReplaceAll(
                                "#Results_AttachmentsSField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsS.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsT":
                            res.ReplaceAll(
                                "#Results_AttachmentsTField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsT.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsU":
                            res.ReplaceAll(
                                "#Results_AttachmentsUField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsU.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsV":
                            res.ReplaceAll(
                                "#Results_AttachmentsVField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsV.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsW":
                            res.ReplaceAll(
                                "#Results_AttachmentsWField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsW.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsX":
                            res.ReplaceAll(
                                "#Results_AttachmentsXField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsX.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsY":
                            res.ReplaceAll(
                                "#Results_AttachmentsYField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsY.ToJson(),
                                        columnPermissionType: column.ColumnPermissionType()));
                            break;
                        case "AttachmentsZ":
                            res.ReplaceAll(
                                "#Results_AttachmentsZField",
                                new HtmlBuilder()
                                    .Field(
                                        ss: ss,
                                        column: column,
                                        value: resultModel.AttachmentsZ.ToJson(),
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
            var resultCollection = new ResultCollection(
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
                    TotalCount = resultCollection.TotalCount,
                    Data = resultCollection.Select(o => o.GetByApi(ss))
                }
            }.ToJson());
        }

        public static System.Web.Mvc.ContentResult GetByApi(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId, methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = ResultValidators.OnEditing(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            ss.SetColumnAccessControls(resultModel.Mine());
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = resultModel.GetByApi(ss).ToSingleList()
                }
            }.ToJson());
        }

        public static string Create(SiteSettings ss)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var resultModel = new ResultModel(ss, 0, setByForm: true);
            var invalid = ResultValidators.OnCreating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Create(ss, notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else if (Linked(ss, resultModel))
            {
                Sessions.Set("Message", Messages.Created(resultModel.Title.DisplayValue).Html);
                return new ResponseCollection()
                    .SetMemory("formChanged", false)
                    .Href(Locations.ItemEdit(Forms.Long("LinkId")))
                    .ToJson();
            }
            else
            {
                ss = SiteSettingsUtilities.Get(ss.SiteId, resultModel.ResultId);
                return EditorResponse(
                    ss,
                    resultModel,
                    Messages.Created(resultModel.Title.DisplayValue),
                    GetSwitchTargets(
                        ss, resultModel.ResultId, resultModel.SiteId).Join())
                            .ToJson();
            }
        }

        private static bool Linked(SiteSettings ss, ResultModel resultModel)
        {
            var siteId = Forms.Long("FromSiteId");
            return
                siteId > 0 &&
                resultModel.PropertyValue(ss.Links.FirstOrDefault(o =>
                    o.SiteId == siteId)?.ColumnName) == Forms.Data("LinkId");
        }

        public static System.Web.Mvc.ContentResult CreateByApi(SiteSettings ss)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return ApiResults.Error(Error.Types.ItemsLimit);
            }
            var resultModel = new ResultModel(ss, 0, setByApi: true);
            var invalid = ResultValidators.OnCreating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(ss);
            var error = resultModel.Create(ss, notice: true);
            if (error.Has())
            {
                return ApiResults.Error(error);
            }
            else
            {
                return ApiResults.Success(
                    resultModel.ResultId,
                    Displays.Created(resultModel.Title.DisplayValue));
            }
        }

        public static string Update(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId, setByForm: true);
            var invalid = ResultValidators.OnUpdating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            var error = resultModel.Update(
                ss: ss,
                notice: true,
                permissions: Forms.List("CurrentPermissionsAll"),
                permissionChanged: Forms.Exists("CurrentPermissionsAll"));
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(resultModel.Updator.Name).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new ResultsResponseCollection(resultModel);
                return ResponseByUpdate(res, ss, resultModel)
                    .PrependComment(ss, resultModel.Comments, resultModel.VerType)
                    .ToJson();
            }
        }

        private static ResponseCollection ResponseByUpdate(
            ResultsResponseCollection res,
            SiteSettings ss, 
            ResultModel resultModel)
        {
            return res
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FieldResponse(ss, resultModel)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", resultModel.Title.DisplayValue)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(
                    baseModel: resultModel, tableName: "Results"))
                .Html("#Links", new HtmlBuilder().Links(
                    ss: ss, id: resultModel.ResultId))
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(resultModel.Title.DisplayValue))
                .Comment(ss, resultModel.Comments, resultModel.DeleteCommentId)
                .ClearFormData();
        }

        public static System.Web.Mvc.ContentResult UpdateByApi(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId, setByApi: true);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = ResultValidators.OnUpdating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(ss);
            var error = resultModel.Update(ss, notice: true);
            if (error.Has())
            {
                return ApiResults.Error(error);
            }
            else
            {
                return ApiResults.Success(
                    resultModel.ResultId,
                    Displays.Updated(resultModel.Title.DisplayValue));
            }
        }

        public static string Copy(SiteSettings ss, long resultId)
        {
            if (Contract.ItemsLimit(ss.SiteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var resultModel = new ResultModel(ss, resultId, setByForm: true);
            var invalid = ResultValidators.OnCreating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            resultModel.ResultId = 0;
            if (ss.EditorColumns.Contains("Title"))
            {
                resultModel.Title.Value += Displays.SuffixCopy();
            }
            if (!Forms.Bool("CopyWithComments"))
            {
                resultModel.Comments.Clear();
            }
            var error = resultModel.Create(ss, forceSynchronizeSourceSummary: true, paramAll: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
            return EditorResponse(
                ss,
                resultModel,
                Messages.Copied(),
                GetSwitchTargets(
                    ss, resultModel.ResultId, resultModel.SiteId).Join())
                        .ToJson();
            }
        }

        public static string Move(SiteSettings ss, long resultId)
        {
            var siteId = Forms.Long("MoveTargets");
            if (Contract.ItemsLimit(siteId))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            var resultModel = new ResultModel(ss, resultId);
            var invalid = ResultValidators.OnMoving(ss, SiteSettingsUtilities.Get(siteId, resultId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Move(ss, siteId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                ss = SiteSettingsUtilities.Get(siteId, resultModel.ResultId);
                return EditorResponse(ss, resultModel)
                    .Message(Messages.Moved(resultModel.Title.Value))
                    .Val("#BackUrl", Locations.ItemIndex(siteId))
                    .ToJson();
            }
        }

        public static string Delete(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId);
            var invalid = ResultValidators.OnDeleting(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Delete(ss, notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(resultModel.Title.Value).Html);
                var res = new ResultsResponseCollection(resultModel);
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.Get(
                        "Items", ss.SiteId.ToString(), ViewModes.GetBySession(ss.SiteId)));
                return res.ToJson();
            }
        }

        public static System.Web.Mvc.ContentResult DeleteByApi(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId, methodType: BaseModel.MethodTypes.Edit);
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return ApiResults.Get(ApiResponses.NotFound());
            }
            var invalid = ResultValidators.OnDeleting(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return ApiResults.Error(invalid);
            }
            resultModel.SiteId = ss.SiteId;
            resultModel.SetTitle(ss);
            var error = resultModel.Delete(ss, notice: true);
            if (error.Has())
            {
                return ApiResults.Error(error);
            }
            else
            {
                return ApiResults.Success(
                    resultModel.ResultId,
                    Displays.Deleted(resultModel.Title.DisplayValue));
            }
        }

        public static string Restore(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel();
            var invalid = ResultValidators.OnRestoring();
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = resultModel.Restore(ss, resultId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new ResultsResponseCollection(resultModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId);
            ss.SetColumnAccessControls(resultModel.Mine());
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
                        new ResultCollection(
                            ss: ss,
                            where: Rds.ResultsWhere().ResultId(resultModel.ResultId),
                            orderBy: Rds.ResultsOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(resultModelHistory => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(resultModelHistory.Ver)
                                            .DataLatest(1, _using:
                                                resultModelHistory.Ver == resultModel.Ver),
                                        action: () => columns
                                            .ForEach(column => hb
                                                .TdValue(
                                                    ss: ss,
                                                    column: column,
                                                    resultModel: resultModelHistory))))));
            return new ResultsResponseCollection(resultModel)
                .Html("#FieldSetHistories", hb).ToJson();
        }

        public static string History(SiteSettings ss, long resultId)
        {
            var resultModel = new ResultModel(ss, resultId);
            ss.SetColumnAccessControls(resultModel.Mine());
            resultModel.Get(
                ss, 
                where: Rds.ResultsWhere()
                    .ResultId(resultModel.ResultId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            resultModel.VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            return EditorResponse(ss, resultModel).ToJson();
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
                Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    join: ss.Join(),
                    where: Views.GetBySession(ss).Where(
                        ss, Rds.ResultsWhere()
                            .SiteId(ss.SiteId)
                            .ResultId_In(
                                value: selector.Selected,
                                negative: selector.All,
                                _using: selector.Selected.Any()))));
        }

        private static int BulkMove(
            long siteId,
            SiteSettings ss,
            GridSelector selector)
        {
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateResults(
                        where: Views.GetBySession(ss).Where(
                            ss, Rds.ResultsWhere()
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
                });
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
                ss, Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(
                        value: checkedItems,
                        negative: negative,
                        _using: checkedItems.Any()));
            var sub = Rds.SelectResults(
                column: Rds.ResultsColumn().ResultId(),
                where: where);
            var statements = new List<SqlStatement>();
            statements.OnBulkDeletingExtendedSqls(ss.SiteId);
            statements.Add(Rds.DeleteItems(
                where: Rds.ItemsWhere()
                    .ReferenceId_In(sub: sub)));
            statements.Add(Rds.PhysicalDeleteLinks(
                where: Rds.LinksWhere()
                    .Or(or: Rds.LinksWhere()
                        .DestinationId_In(sub: sub)
                        .SourceId_In(sub: sub))));
            statements.Add(Rds.DeleteResults(
                where: where, 
                countRecord: true));
            statements.OnBulkDeletedExtendedSqls(ss.SiteId);
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: statements.ToArray());
        }

        public static string Import(SiteModel siteModel)
        {
            if (!Contract.Import()) return null;
            var ss = siteModel.ResultsSiteSettings(siteModel.SiteId, setAllChoices: true);
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
                    if (column?.ColumnName == "ResultId")
                    {
                        idColumn = data.Index;
                    }
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(ss, columnHash.Values
                    .Select(o => o.ColumnName));
                if (error != null) return error;
                Rds.ExecuteNonQuery(
                    transactional: true,
                    statements: new List<SqlStatement>()
                        .OnImportingExtendedSqls(ss.SiteId).ToArray());
                var resultHash = new Dictionary<int, ResultModel>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var resultModel = new ResultModel() { SiteId = ss.SiteId };
                    if (Forms.Bool("UpdatableImport") && idColumn > -1)
                    {
                        var model = new ResultModel(ss, data.Row[idColumn].ToLong());
                        if (model.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            resultModel = model;
                        }
                    }
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], ss.InheritPermission);
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
                            case "Manager":
                                resultModel.Manager.Id = recordingData.ToInt();
                                break;
                            case "Owner":
                                resultModel.Owner.Id = recordingData.ToInt();
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
                            case "Comments":
                                if (resultModel.AccessStatus != Databases.AccessStatuses.Selected &&
                                    !data.Row[column.Key].IsNullOrEmpty())
                                {
                                    resultModel.Comments.Prepend(data.Row[column.Key]);
                                }
                                break;
                        }
                    });
                    resultHash.Add(data.Index, resultModel);
                });
                var insertCount = 0;
                var updateCount = 0;
                resultHash.Values.ForEach(resultModel =>
                {
                    resultModel.SetTitle(ss);
                    if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                    {
                        resultModel.VerUp = Versions.MustVerUp(resultModel);
                        if (resultModel.Updated())
                        {
                            resultModel.Update(ss: ss, extendedSqls: false, get: false);
                            updateCount++;
                        }
                    }
                    else
                    {
                        resultModel.Create(ss: ss, extendedSqls: false, get: false);
                        insertCount++;
                    }
                });
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

        private static string ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            return recordingData;
        }

        public static string OpenExportSelectorDialog(SiteSettings ss, SiteModel siteModel)
        {
            if (!Contract.Export())
            {
                return HtmlTemplates.Error(Error.Types.InvalidRequest);
            }
            var invalid = ResultValidators.OnExporting(ss);
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
            var invalid = ResultValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            return ExportUtilities.Csv(ss, ss.GetExport(QueryStrings.Int("id")));
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
            var columnName = view.GetCalendarColumn(ss);
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(month);
            var end = Calendars.EndDate(month);
            var dataRows = CalendarDataRows(
                ss,
                view,
                columnName,
                Calendars.BeginDate(month),
                Calendars.EndDate(month));
            var inRange = dataRows.Count() <= Parameters.General.CalendarLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.CalendarLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                gridData: gridData,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb
                    .Calendar(
                        ss: ss,
                        columnName: columnName,
                        month: month,
                        begin: begin,
                        dataRows: dataRows,
                        bodyOnly: false,
                        inRange: inRange));
        }

        public static string UpdateByCalendar(SiteSettings ss)
        {
            var resultModel = new ResultModel(ss, Forms.Long("CalendarId"), setByForm: true);
            var invalid = ResultValidators.OnUpdating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(resultModel);
            resultModel.Update(ss, notice: true);
            return CalendarJson(ss);
        }

        public static string CalendarJson(SiteSettings ss)
        {
            if (!ss.EnableViewMode("Calendar"))
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var gridData = GetGridData(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Calendar");
            var columnName = view.GetCalendarColumn(ss);
            var month = view.CalendarMonth != null
                ? view.CalendarMonth.ToDateTime()
                : DateTime.Now;
            var begin = Calendars.BeginDate(month);
            var end = Calendars.EndDate(month);
            var dataRows = CalendarDataRows(
                ss,
                view,
                columnName,
                Calendars.BeginDate(month),
                Calendars.EndDate(month));
            return dataRows.Count() <= Parameters.General.CalendarLimit
                ? new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CalendarBody",
                        new HtmlBuilder().Calendar(
                            ss: ss,
                            columnName: columnName,
                            month: month,
                            begin: begin,
                            dataRows: dataRows,
                            bodyOnly: bodyOnly,
                            inRange: true))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Invoke("setCalendar")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CalendarBody",
                        new HtmlBuilder().Calendar(
                            ss: ss,
                            columnName: columnName,
                            month: month,
                            begin: begin,
                            dataRows: dataRows,
                            bodyOnly: bodyOnly,
                            inRange: false))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.CalendarLimit.ToString()))
                    .ToJson();
        }

        private static EnumerableRowCollection<DataRow> CalendarDataRows(
            SiteSettings ss, View view, string columnName, DateTime begin, DateTime end)
        {
            var where = Rds.ResultsWhere();
            switch (columnName)
            {
                case "UpdatedTime": 
                    where.UpdatedTime_Between(begin, end);
                    break;
                case "DateA": 
                    where.DateA_Between(begin, end);
                    break;
                case "DateB": 
                    where.DateB_Between(begin, end);
                    break;
                case "DateC": 
                    where.DateC_Between(begin, end);
                    break;
                case "DateD": 
                    where.DateD_Between(begin, end);
                    break;
                case "DateE": 
                    where.DateE_Between(begin, end);
                    break;
                case "DateF": 
                    where.DateF_Between(begin, end);
                    break;
                case "DateG": 
                    where.DateG_Between(begin, end);
                    break;
                case "DateH": 
                    where.DateH_Between(begin, end);
                    break;
                case "DateI": 
                    where.DateI_Between(begin, end);
                    break;
                case "DateJ": 
                    where.DateJ_Between(begin, end);
                    break;
                case "DateK": 
                    where.DateK_Between(begin, end);
                    break;
                case "DateL": 
                    where.DateL_Between(begin, end);
                    break;
                case "DateM": 
                    where.DateM_Between(begin, end);
                    break;
                case "DateN": 
                    where.DateN_Between(begin, end);
                    break;
                case "DateO": 
                    where.DateO_Between(begin, end);
                    break;
                case "DateP": 
                    where.DateP_Between(begin, end);
                    break;
                case "DateQ": 
                    where.DateQ_Between(begin, end);
                    break;
                case "DateR": 
                    where.DateR_Between(begin, end);
                    break;
                case "DateS": 
                    where.DateS_Between(begin, end);
                    break;
                case "DateT": 
                    where.DateT_Between(begin, end);
                    break;
                case "DateU": 
                    where.DateU_Between(begin, end);
                    break;
                case "DateV": 
                    where.DateV_Between(begin, end);
                    break;
                case "DateW": 
                    where.DateW_Between(begin, end);
                    break;
                case "DateX": 
                    where.DateX_Between(begin, end);
                    break;
                case "DateY": 
                    where.DateY_Between(begin, end);
                    break;
                case "DateZ": 
                    where.DateZ_Between(begin, end);
                    break;
                case "CreatedTime": 
                    where.CreatedTime_Between(begin, end);
                    break;
            }
            return Rds.ExecuteTable(statements:
                Rds.SelectResults(
                    column: Rds.ResultsTitleColumn(ss)
                        .ResultId()
                        .ResultsColumn(columnName, _as: "Date")
                        .ItemTitle(ss.ReferenceType, Rds.IdColumn(ss.ReferenceType)),
                    join: ss.Join(),
                    where: view.Where(ss: ss, where: where)))
                        .AsEnumerable();
        }

        private static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            SiteSettings ss,
            string columnName,
            DateTime month,
            DateTime begin,
            EnumerableRowCollection<DataRow> dataRows,
            bool bodyOnly,
            bool inRange)
        {
            return !bodyOnly
                ? hb.Calendar(
                    ss: ss,
                    columnName: columnName,
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange)
                : hb.CalendarBody(
                    ss: ss,
                    column: ss.GetColumn(columnName),
                    month: month,
                    begin: begin,
                    dataRows: dataRows,
                    inRange: inRange);
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
                value = ss.GetColumn("ResultId");
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
                        Parameters.General.CrosstabXLimit.ToString()).Html);
            }
            else if (!inRangeY)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyColumnCases(
                        Parameters.General.CrosstabYLimit.ToString()).Html);
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
                value = ss.GetColumn("ResultId");
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
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CrosstabBody",
                        !bodyOnly
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
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Invoke("setCrosstab")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CrosstabBody",
                        !bodyOnly
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
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Message(!inRangeX
                        ? Messages.TooManyColumnCases(
                            Parameters.General.CrosstabXLimit.ToString())
                        : Messages.TooManyRowCases(
                            Parameters.General.CrosstabYLimit.ToString()))
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
                    Rds.SelectResults(
                        column: Rds.ResultsColumn()
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
                        groupBy: Rds.ResultsGroupBy()
                            .Add(ss, groupByX)
                            .Add(ss, groupByY)))
                                .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    ss, groupByX, timePeriod);
                dataRows = Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        column: Rds.ResultsColumn()
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
                    Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()).Html);
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
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#TimeSeriesBody",
                        new HtmlBuilder().TimeSeries(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly,
                            inRange: true))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Invoke("drawTimeSeries")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#TimeSeriesBody",
                        new HtmlBuilder().TimeSeries(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly,
                            inRange: false))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()))
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
                    Rds.SelectResults(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.ResultsColumn()
                            .ResultId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .Add(ss: ss,column: groupBy)
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
                    Messages.TooManyCases(Parameters.General.KambanLimit.ToString()).Html);
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
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#KambanBody",
                        new HtmlBuilder().Kamban(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly,
                            changedItemId: Forms.Long("KambanId")))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Invoke("setKamban")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#KambanBody",
                        new HtmlBuilder().Kamban(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly,
                            inRange: false))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.KambanLimit.ToString()))
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
            var groupByY = ss.GetColumn(view.GetKambanGroupByY(ss));
            var aggregateType = view.GetKambanAggregationType(ss);
            var value = view.GetKambanValue(ss);
            var aggregationView = view.KambanAggregationView ?? false;
            if (groupByX == null)
            {
                return hb;
            }
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
                    data: KambanElements(
                        ss,
                        view,
                        groupByX,
                        groupByY,
                        value,
                        KambanColumns(ss, groupByX, groupByY, value)),
                    inRange: inRange)
                : hb.KambanBody(
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: ss.GetColumn(value),
                    columns: view.KambanColumns,
                    aggregationView: aggregationView,
                    data: KambanElements(
                        ss,
                        view,
                        groupByX,
                        groupByY,
                        value,
                        KambanColumns(ss, groupByX, groupByY, value)),
                    changedItemId: changedItemId,
                    inRange: inRange);
        }

        private static SqlColumnCollection KambanColumns(
            SiteSettings ss, Column groupByX, Column groupByY, string value)
        {
            return Rds.ResultsColumn()
                .ResultId()
                .ResultsColumn(groupByX.ColumnName)
                .ResultsColumn(groupByY.ColumnName)
                .ResultsColumn(value)
                .ItemTitle(tableName: "Results", idColumn: "ResultId");
        }

        private static IEnumerable<Libraries.ViewModes.KambanElement> KambanElements(
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            string value,
            SqlColumnCollection column)
        {
            return new GridData(
                ss: ss,
                view: view,
                column: column)
                    .DataRows
                    .Select(o => new Libraries.ViewModes.KambanElement()
                    {
                        Id = o.Long("ResultId"),
                        Title = o.String("ItemTitle"),
                        GroupX = o.String(groupByX.ColumnName),
                        GroupY = o.String(groupByY.ColumnName),
                        Value = o.Decimal(value)
                    });
        }

        public static string UpdateByKamban(SiteSettings ss)
        {
            var resultModel = new ResultModel(ss, Forms.Long("KambanId"), setByForm: true);
            var invalid = ResultValidators.OnUpdating(ss, resultModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            if (resultModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Messages.ResponseDeleteConflicts().ToJson();
            }
            resultModel.VerUp = Versions.MustVerUp(resultModel);
            resultModel.Update(ss, notice: true);
            return KambanJson(ss);
        }

        public static void SetLinks(this List<ResultModel> results, SiteSettings ss)
        {
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: results
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel => resultModel.SetTitle(ss));
            }
        }
    }
}
