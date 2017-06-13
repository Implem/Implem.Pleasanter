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
            var resultCollection = ResultCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                resultCollection: resultCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Grid(
                   resultCollection: resultCollection,
                   ss: ss,
                   view: view));
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultCollection resultCollection,
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
            ss.SetColumnAccessControls();
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                siteId: ss.SiteId,
                parentId: ss.ParentId,
                referenceType: "Results",
                script: Libraries.Scripts.JavaScripts.ViewMode(viewMode),
                userScript: ss.GridScript,
                userStyle: ss.GridStyle,
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
                                aggregations: resultCollection.Aggregations)
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
                        .Id("ExportSettingsDialog")
                        .Class("dialog")
                        .Title(Displays.ExportSettings())))
                    .ToString();
        }

        public static string IndexJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            return new ResponseCollection()
                .Html("#ViewModeContainer", new HtmlBuilder().Grid(
                    ss: ss,
                    resultCollection: resultCollection,
                    view: view))
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: resultCollection.Aggregations))
                .Paging("#Grid")
                .ToJson();
        }

        private static ResultCollection ResultCollection(
            SiteSettings ss, View view, int offset = 0)
        {
            return new ResultCollection(
                ss: ss,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss: ss),
                orderBy: view.OrderBy(ss, Rds.ResultsOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultCollection resultCollection,
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
                            resultCollection: resultCollection,
                            view: view))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        resultCollection.Count(),
                        resultCollection.Aggregations.TotalCount)
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
            var resultCollection = ResultCollection(ss, view, offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: resultCollection.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    resultCollection: resultCollection,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    resultCollection.Count(),
                    resultCollection.Aggregations.TotalCount))
                .Paging("#Grid")
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultCollection resultCollection,
            View view,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            var columns = ss.GetGridColumns(checkPermission: true);
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: resultCollection
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                resultCollection.ForEach(resultModel =>
                    resultModel.Title = new Title(
                        ss,
                        resultModel.ResultId,
                        resultModel.PropertyValues(ss.TitleColumns)));
            }
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columns: columns, 
                            view: view,
                            checkAll: checkAll))
                .TBody(action: () => resultCollection
                    .ForEach(resultModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(resultModel.ResultId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: resultModel.ResultId.ToString()));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            ss: ss,
                                            column: column,
                                            resultModel: resultModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.ResultsColumn();
            new List<string> { "SiteId", "ResultId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
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
                switch (column.ColumnName)
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

        public static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, ResultModel resultModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.ColumnName)
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
                userScript: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? ss.NewScript
                    : ss.EditScript,
                userStyle: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? ss.NewStyle
                    : ss.EditStyle,
                action: () =>
                {
                    hb
                        .Editor(
                            ss: ss,
                            resultModel: resultModel)
                        .Hidden(controlId: "TableName", value: "Results")
                        .Hidden(controlId: "Id", value: resultModel.ResultId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.EditorColumns?.Contains("Comments") == true &&
                commentsColumnPermissionType != Permissions.ColumnPermissionTypes.Deny;
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
                                referenceType: "items",
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
                        text: Displays.Basic()))
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

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultModel resultModel)
        {
            var mine = resultModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                ss.GetEditorColumns().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "ResultId":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ResultId.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Ver":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Ver.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Title":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Title.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Body":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Body.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Status":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Status.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Manager":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Manager.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Owner":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Owner.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                    }
                });
                hb.VerUpCheckBox(resultModel);
                hb
                    .Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            ss: ss,
                            linkId: resultModel.ResultId,
                            methodType: resultModel.MethodType))
                    .Div(id: "Links", css: "links", action: () => hb
                        .Links(ss: ss, id: resultModel.ResultId));
            });
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
            var siteModel = new SiteModel(resultModel.SiteId);
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
            var switchTargets = Rds.ExecuteScalar_int(statements:
                Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultId(),
                            where: where,
                            orderBy: view.OrderBy(ss, Rds.ResultsOrderBy()
                                .UpdatedTime(SqlOrderBy.Types.desc))))
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
                    switch (column.ColumnName)
                    {
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
                        default: break;
                    }
                });
            return res;
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
            else
            {
                return EditorResponse(
                    ss,
                    resultModel,
                    Messages.Created(resultModel.Title.DisplayValue),
                    GetSwitchTargets(
                        ss, resultModel.ResultId, resultModel.SiteId).Join())
                            .ToJson();
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
                    .PrependComment(resultModel.Comments, resultModel.VerType)
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
                .RemoveComment(resultModel.DeleteCommentId, _using: resultModel.DeleteCommentId != 0)
                .ClearFormData();
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
            if (!Forms.Data("CopyWithComments").ToBool())
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
                Messages.Copied(resultModel.Title.Value),
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
                    .Href(Locations.ItemIndex(resultModel.SiteId));
                return res.ToJson();
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
            var all = Forms.Bool("GridCheckAll");
            var selected = all
                ? GridItems("GridUnCheckedItems")
                : GridItems("GridCheckedItems");
            var count = BulkMoveCount(siteId, ss, selected, all: all);
            if (Contract.ItemsLimit(siteId, count))
            {
                return Error.Types.ItemsLimit.MessageJson();
            }
            if (Permissions.CanMove(ss, SiteSettingsUtilities.Get(siteId, siteId)))
            {
                if (all)
                {
                    count = BulkMove(siteId, ss, selected, all: all);
                }
                else
                {
                    if (selected.Any())
                    {
                        count = BulkMove(siteId, ss, selected);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
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
            IEnumerable<long> selected,
            bool all = false)
        {
            return Rds.ExecuteScalar_int(statements:
                Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultsCount(),
                    where: Views.GetBySession(ss).Where(
                        ss, Rds.ResultsWhere()
                            .SiteId(ss.SiteId)
                            .ResultId_In(
                                value: selected,
                                negative: all,
                                _using: selected.Any()))));
        }

        private static int BulkMove(
            long siteId,
            SiteSettings ss,
            IEnumerable<long> selected,
            bool all = false)
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
                                    value: selected,
                                    negative: all,
                                    _using: selected.Any())),
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
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkDelete(
                        ss,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Any())
                    {
                        count = BulkDelete(
                            ss,
                            checkedItems);
                    }
                    else
                    {
                        return Messages.ResponseSelectTargets().ToJson();
                    }
                }
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
            return Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteItems(
                        where: Rds.ItemsWhere()
                            .ReferenceId_In(
                                sub: Rds.SelectResults(
                                    column: Rds.ResultsColumn().ResultId(),
                                    where: where))),
                    Rds.DeleteResults(
                        where: where, 
                        countRecord: true)
                });
        }

        private static IEnumerable<long> GridItems(string name)
        {
            return Forms.Data(name)
                .Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .Distinct();
        }

        public static string Import(SiteModel siteModel)
        {
            if (!Contract.Import()) return null;
            var ss = siteModel.ResultsSiteSettings(siteModel.SiteId);
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
                var columnHash = new Dictionary<int, Column>();
                csv.Headers.Select((o, i) => new { Header = o, Index = i }).ForEach(data =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == data.Header)
                        .FirstOrDefault();
                    if (column != null) columnHash.Add(data.Index, column);
                });
                var error = Imports.ColumnValidate(ss, columnHash.Values
                    .Select(o => o.ColumnName));
                if (error != null) return error;
                var resultHash = new Dictionary<int, ResultModel>();
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.ResultsParam();
                    param.SiteId(ss.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], ss.InheritPermission);
                        if (column.Value.ColumnName == "ResultId")
                        {
                            var resultId = recordingData.ToLong();
                            if (Forms.Bool("UpdatableImport") && resultId > 0)
                            {
                                var resultModel = new ResultModel().Get(ss, where: Rds.ResultsWhere()
                                    .SiteId(ss.SiteId)
                                    .ResultId(resultId));
                                if (resultModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    resultHash.Add(data.Index, resultModel);
                                }
                            }
                        }
                        if (!param.Any(o => o.Name == column.Value.ColumnName))
                        {
                            switch (column.Value.ColumnName)
                            {
                                case "Title": param.Title(recordingData, _using: recordingData != null); break;
                                case "Body": param.Body(recordingData, _using: recordingData != null); break;
                                case "Status": param.Status(recordingData, _using: recordingData != null); break;
                                case "Manager": param.Manager(recordingData, _using: recordingData != null); break;
                                case "Owner": param.Owner(recordingData, _using: recordingData != null); break;
                                case "ClassA": param.ClassA(recordingData, _using: recordingData != null); break;
                                case "ClassB": param.ClassB(recordingData, _using: recordingData != null); break;
                                case "ClassC": param.ClassC(recordingData, _using: recordingData != null); break;
                                case "ClassD": param.ClassD(recordingData, _using: recordingData != null); break;
                                case "ClassE": param.ClassE(recordingData, _using: recordingData != null); break;
                                case "ClassF": param.ClassF(recordingData, _using: recordingData != null); break;
                                case "ClassG": param.ClassG(recordingData, _using: recordingData != null); break;
                                case "ClassH": param.ClassH(recordingData, _using: recordingData != null); break;
                                case "ClassI": param.ClassI(recordingData, _using: recordingData != null); break;
                                case "ClassJ": param.ClassJ(recordingData, _using: recordingData != null); break;
                                case "ClassK": param.ClassK(recordingData, _using: recordingData != null); break;
                                case "ClassL": param.ClassL(recordingData, _using: recordingData != null); break;
                                case "ClassM": param.ClassM(recordingData, _using: recordingData != null); break;
                                case "ClassN": param.ClassN(recordingData, _using: recordingData != null); break;
                                case "ClassO": param.ClassO(recordingData, _using: recordingData != null); break;
                                case "ClassP": param.ClassP(recordingData, _using: recordingData != null); break;
                                case "ClassQ": param.ClassQ(recordingData, _using: recordingData != null); break;
                                case "ClassR": param.ClassR(recordingData, _using: recordingData != null); break;
                                case "ClassS": param.ClassS(recordingData, _using: recordingData != null); break;
                                case "ClassT": param.ClassT(recordingData, _using: recordingData != null); break;
                                case "ClassU": param.ClassU(recordingData, _using: recordingData != null); break;
                                case "ClassV": param.ClassV(recordingData, _using: recordingData != null); break;
                                case "ClassW": param.ClassW(recordingData, _using: recordingData != null); break;
                                case "ClassX": param.ClassX(recordingData, _using: recordingData != null); break;
                                case "ClassY": param.ClassY(recordingData, _using: recordingData != null); break;
                                case "ClassZ": param.ClassZ(recordingData, _using: recordingData != null); break;
                                case "NumA": param.NumA(recordingData, _using: recordingData != null); break;
                                case "NumB": param.NumB(recordingData, _using: recordingData != null); break;
                                case "NumC": param.NumC(recordingData, _using: recordingData != null); break;
                                case "NumD": param.NumD(recordingData, _using: recordingData != null); break;
                                case "NumE": param.NumE(recordingData, _using: recordingData != null); break;
                                case "NumF": param.NumF(recordingData, _using: recordingData != null); break;
                                case "NumG": param.NumG(recordingData, _using: recordingData != null); break;
                                case "NumH": param.NumH(recordingData, _using: recordingData != null); break;
                                case "NumI": param.NumI(recordingData, _using: recordingData != null); break;
                                case "NumJ": param.NumJ(recordingData, _using: recordingData != null); break;
                                case "NumK": param.NumK(recordingData, _using: recordingData != null); break;
                                case "NumL": param.NumL(recordingData, _using: recordingData != null); break;
                                case "NumM": param.NumM(recordingData, _using: recordingData != null); break;
                                case "NumN": param.NumN(recordingData, _using: recordingData != null); break;
                                case "NumO": param.NumO(recordingData, _using: recordingData != null); break;
                                case "NumP": param.NumP(recordingData, _using: recordingData != null); break;
                                case "NumQ": param.NumQ(recordingData, _using: recordingData != null); break;
                                case "NumR": param.NumR(recordingData, _using: recordingData != null); break;
                                case "NumS": param.NumS(recordingData, _using: recordingData != null); break;
                                case "NumT": param.NumT(recordingData, _using: recordingData != null); break;
                                case "NumU": param.NumU(recordingData, _using: recordingData != null); break;
                                case "NumV": param.NumV(recordingData, _using: recordingData != null); break;
                                case "NumW": param.NumW(recordingData, _using: recordingData != null); break;
                                case "NumX": param.NumX(recordingData, _using: recordingData != null); break;
                                case "NumY": param.NumY(recordingData, _using: recordingData != null); break;
                                case "NumZ": param.NumZ(recordingData, _using: recordingData != null); break;
                                case "DateA": param.DateA(recordingData, _using: recordingData != null); break;
                                case "DateB": param.DateB(recordingData, _using: recordingData != null); break;
                                case "DateC": param.DateC(recordingData, _using: recordingData != null); break;
                                case "DateD": param.DateD(recordingData, _using: recordingData != null); break;
                                case "DateE": param.DateE(recordingData, _using: recordingData != null); break;
                                case "DateF": param.DateF(recordingData, _using: recordingData != null); break;
                                case "DateG": param.DateG(recordingData, _using: recordingData != null); break;
                                case "DateH": param.DateH(recordingData, _using: recordingData != null); break;
                                case "DateI": param.DateI(recordingData, _using: recordingData != null); break;
                                case "DateJ": param.DateJ(recordingData, _using: recordingData != null); break;
                                case "DateK": param.DateK(recordingData, _using: recordingData != null); break;
                                case "DateL": param.DateL(recordingData, _using: recordingData != null); break;
                                case "DateM": param.DateM(recordingData, _using: recordingData != null); break;
                                case "DateN": param.DateN(recordingData, _using: recordingData != null); break;
                                case "DateO": param.DateO(recordingData, _using: recordingData != null); break;
                                case "DateP": param.DateP(recordingData, _using: recordingData != null); break;
                                case "DateQ": param.DateQ(recordingData, _using: recordingData != null); break;
                                case "DateR": param.DateR(recordingData, _using: recordingData != null); break;
                                case "DateS": param.DateS(recordingData, _using: recordingData != null); break;
                                case "DateT": param.DateT(recordingData, _using: recordingData != null); break;
                                case "DateU": param.DateU(recordingData, _using: recordingData != null); break;
                                case "DateV": param.DateV(recordingData, _using: recordingData != null); break;
                                case "DateW": param.DateW(recordingData, _using: recordingData != null); break;
                                case "DateX": param.DateX(recordingData, _using: recordingData != null); break;
                                case "DateY": param.DateY(recordingData, _using: recordingData != null); break;
                                case "DateZ": param.DateZ(recordingData, _using: recordingData != null); break;
                                case "DescriptionA": param.DescriptionA(recordingData, _using: recordingData != null); break;
                                case "DescriptionB": param.DescriptionB(recordingData, _using: recordingData != null); break;
                                case "DescriptionC": param.DescriptionC(recordingData, _using: recordingData != null); break;
                                case "DescriptionD": param.DescriptionD(recordingData, _using: recordingData != null); break;
                                case "DescriptionE": param.DescriptionE(recordingData, _using: recordingData != null); break;
                                case "DescriptionF": param.DescriptionF(recordingData, _using: recordingData != null); break;
                                case "DescriptionG": param.DescriptionG(recordingData, _using: recordingData != null); break;
                                case "DescriptionH": param.DescriptionH(recordingData, _using: recordingData != null); break;
                                case "DescriptionI": param.DescriptionI(recordingData, _using: recordingData != null); break;
                                case "DescriptionJ": param.DescriptionJ(recordingData, _using: recordingData != null); break;
                                case "DescriptionK": param.DescriptionK(recordingData, _using: recordingData != null); break;
                                case "DescriptionL": param.DescriptionL(recordingData, _using: recordingData != null); break;
                                case "DescriptionM": param.DescriptionM(recordingData, _using: recordingData != null); break;
                                case "DescriptionN": param.DescriptionN(recordingData, _using: recordingData != null); break;
                                case "DescriptionO": param.DescriptionO(recordingData, _using: recordingData != null); break;
                                case "DescriptionP": param.DescriptionP(recordingData, _using: recordingData != null); break;
                                case "DescriptionQ": param.DescriptionQ(recordingData, _using: recordingData != null); break;
                                case "DescriptionR": param.DescriptionR(recordingData, _using: recordingData != null); break;
                                case "DescriptionS": param.DescriptionS(recordingData, _using: recordingData != null); break;
                                case "DescriptionT": param.DescriptionT(recordingData, _using: recordingData != null); break;
                                case "DescriptionU": param.DescriptionU(recordingData, _using: recordingData != null); break;
                                case "DescriptionV": param.DescriptionV(recordingData, _using: recordingData != null); break;
                                case "DescriptionW": param.DescriptionW(recordingData, _using: recordingData != null); break;
                                case "DescriptionX": param.DescriptionX(recordingData, _using: recordingData != null); break;
                                case "DescriptionY": param.DescriptionY(recordingData, _using: recordingData != null); break;
                                case "DescriptionZ": param.DescriptionZ(recordingData, _using: recordingData != null); break;
                                case "CheckA": param.CheckA(recordingData, _using: recordingData != null); break;
                                case "CheckB": param.CheckB(recordingData, _using: recordingData != null); break;
                                case "CheckC": param.CheckC(recordingData, _using: recordingData != null); break;
                                case "CheckD": param.CheckD(recordingData, _using: recordingData != null); break;
                                case "CheckE": param.CheckE(recordingData, _using: recordingData != null); break;
                                case "CheckF": param.CheckF(recordingData, _using: recordingData != null); break;
                                case "CheckG": param.CheckG(recordingData, _using: recordingData != null); break;
                                case "CheckH": param.CheckH(recordingData, _using: recordingData != null); break;
                                case "CheckI": param.CheckI(recordingData, _using: recordingData != null); break;
                                case "CheckJ": param.CheckJ(recordingData, _using: recordingData != null); break;
                                case "CheckK": param.CheckK(recordingData, _using: recordingData != null); break;
                                case "CheckL": param.CheckL(recordingData, _using: recordingData != null); break;
                                case "CheckM": param.CheckM(recordingData, _using: recordingData != null); break;
                                case "CheckN": param.CheckN(recordingData, _using: recordingData != null); break;
                                case "CheckO": param.CheckO(recordingData, _using: recordingData != null); break;
                                case "CheckP": param.CheckP(recordingData, _using: recordingData != null); break;
                                case "CheckQ": param.CheckQ(recordingData, _using: recordingData != null); break;
                                case "CheckR": param.CheckR(recordingData, _using: recordingData != null); break;
                                case "CheckS": param.CheckS(recordingData, _using: recordingData != null); break;
                                case "CheckT": param.CheckT(recordingData, _using: recordingData != null); break;
                                case "CheckU": param.CheckU(recordingData, _using: recordingData != null); break;
                                case "CheckV": param.CheckV(recordingData, _using: recordingData != null); break;
                                case "CheckW": param.CheckW(recordingData, _using: recordingData != null); break;
                                case "CheckX": param.CheckX(recordingData, _using: recordingData != null); break;
                                case "CheckY": param.CheckY(recordingData, _using: recordingData != null); break;
                                case "CheckZ": param.CheckZ(recordingData, _using: recordingData != null); break;
                                case "Comments": param.Comments(recordingData, _using: recordingData != null); break;
                            }
                        }
                    });
                    if (!resultHash.ContainsKey(data.Index))
                    {
                        param.ResultId(raw: Def.Sql.Identity);
                    }
                    paramHash.Add(data.Index, param);
                });
                var insertCount = 0;
                var updateCount = 0;
                paramHash.ForEach(data =>
                {
                    if (resultHash.ContainsKey(data.Key))
                    {
                        var resultModel = resultHash[data.Key];
                        resultModel.VerUp = Versions.MustVerUp(resultModel);
                        resultModel.Update(ss, param: data.Value);
                        updateCount++;
                    }
                    else
                    {
                        new ResultModel()
                        {
                            SiteId = ss.SiteId,
                            Title = new Title(data.Value.FirstOrDefault(o =>
                                o.Name == "Title")?.Value.ToString() ?? string.Empty)
                        }.Create(ss, param: data.Value);
                        insertCount++;
                    }
                });
                return GridRows(ss, res
                    .WindowScrollTop()
                    .CloseDialog()
                    .Message(Messages.Imported(insertCount.ToString(), updateCount.ToString())));
            }
            else
            {
                return Messages.ResponseFileNotFound().ToJson();
            }
        }

        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            return recordingData;
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
            var view = Views.GetBySession(ss);
            var resultCollection = new ResultCollection(
                ss: ss,
                where: view.Where(ss: ss),
                orderBy: view.OrderBy(ss, Rds.ResultsOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            ss.SetColumnAccessControls();
            var allowedColumns = Permissions.AllowedColumns(ss);
            var exportColumns = Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns")
                    .ToString()
                    .Deserialize<ExportColumns>();
            exportColumns.Columns.RemoveAll((key, value) => !allowedColumns.Contains(key));
            var columnHash = exportColumns.ColumnHash(
                siteModel.ResultsSiteSettings(siteModel.SiteId));
            if (Sessions.PageSession(siteModel.Id, "ExportSettings_AddHeader").ToBool())
            {
                var header = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn => header.Add(
                        "\"" + columnHash[exportColumn.Key].LabelText + "\""));
                csv.Append(header.Join(","), "\n");
            }
            resultCollection.ForEach(resultModel =>
            {
                var row = new List<string>();
                var mine = resultModel.Mine();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            ss,
                            resultModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key],
                            mine)));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            SiteSettings ss,
            ResultModel resultModel,
            string columnName,
            Column column,
            List<string> mine)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.SiteId.ToExport(column)
                        : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.UpdatedTime.ToExport(column)
                        : string.Empty;
                    break;
                case "ResultId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ResultId.ToExport(column)
                        : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Ver.ToExport(column)
                        : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Title.ToExport(column)
                        : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Body.ToExport(column)
                        : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.TitleBody.ToExport(column)
                        : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Status.ToExport(column)
                        : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Manager.ToExport(column)
                        : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Owner.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassA.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassB.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassC.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassD.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassE.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassF.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassG.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassH.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassI.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassJ.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassK.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassL.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassM.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassN.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassO.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassP.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassQ.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassR.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassS.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassT.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassU.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassV.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassW.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassX.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassY.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.ClassZ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumA.ToExport(column)
                        : string.Empty;
                    break;
                case "NumB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumB.ToExport(column)
                        : string.Empty;
                    break;
                case "NumC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumC.ToExport(column)
                        : string.Empty;
                    break;
                case "NumD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumD.ToExport(column)
                        : string.Empty;
                    break;
                case "NumE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumE.ToExport(column)
                        : string.Empty;
                    break;
                case "NumF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumF.ToExport(column)
                        : string.Empty;
                    break;
                case "NumG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumG.ToExport(column)
                        : string.Empty;
                    break;
                case "NumH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumH.ToExport(column)
                        : string.Empty;
                    break;
                case "NumI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumI.ToExport(column)
                        : string.Empty;
                    break;
                case "NumJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumJ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumK.ToExport(column)
                        : string.Empty;
                    break;
                case "NumL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumL.ToExport(column)
                        : string.Empty;
                    break;
                case "NumM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumM.ToExport(column)
                        : string.Empty;
                    break;
                case "NumN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumN.ToExport(column)
                        : string.Empty;
                    break;
                case "NumO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumO.ToExport(column)
                        : string.Empty;
                    break;
                case "NumP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumP.ToExport(column)
                        : string.Empty;
                    break;
                case "NumQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumQ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumR.ToExport(column)
                        : string.Empty;
                    break;
                case "NumS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumS.ToExport(column)
                        : string.Empty;
                    break;
                case "NumT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumT.ToExport(column)
                        : string.Empty;
                    break;
                case "NumU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumU.ToExport(column)
                        : string.Empty;
                    break;
                case "NumV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumV.ToExport(column)
                        : string.Empty;
                    break;
                case "NumW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumW.ToExport(column)
                        : string.Empty;
                    break;
                case "NumX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumX.ToExport(column)
                        : string.Empty;
                    break;
                case "NumY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumY.ToExport(column)
                        : string.Empty;
                    break;
                case "NumZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.NumZ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateA.ToExport(column)
                        : string.Empty;
                    break;
                case "DateB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateB.ToExport(column)
                        : string.Empty;
                    break;
                case "DateC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateC.ToExport(column)
                        : string.Empty;
                    break;
                case "DateD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateD.ToExport(column)
                        : string.Empty;
                    break;
                case "DateE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateE.ToExport(column)
                        : string.Empty;
                    break;
                case "DateF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateF.ToExport(column)
                        : string.Empty;
                    break;
                case "DateG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateG.ToExport(column)
                        : string.Empty;
                    break;
                case "DateH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateH.ToExport(column)
                        : string.Empty;
                    break;
                case "DateI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateI.ToExport(column)
                        : string.Empty;
                    break;
                case "DateJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateJ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateK.ToExport(column)
                        : string.Empty;
                    break;
                case "DateL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateL.ToExport(column)
                        : string.Empty;
                    break;
                case "DateM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateM.ToExport(column)
                        : string.Empty;
                    break;
                case "DateN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateN.ToExport(column)
                        : string.Empty;
                    break;
                case "DateO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateO.ToExport(column)
                        : string.Empty;
                    break;
                case "DateP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateP.ToExport(column)
                        : string.Empty;
                    break;
                case "DateQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateQ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateR.ToExport(column)
                        : string.Empty;
                    break;
                case "DateS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateS.ToExport(column)
                        : string.Empty;
                    break;
                case "DateT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateT.ToExport(column)
                        : string.Empty;
                    break;
                case "DateU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateU.ToExport(column)
                        : string.Empty;
                    break;
                case "DateV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateV.ToExport(column)
                        : string.Empty;
                    break;
                case "DateW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateW.ToExport(column)
                        : string.Empty;
                    break;
                case "DateX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateX.ToExport(column)
                        : string.Empty;
                    break;
                case "DateY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateY.ToExport(column)
                        : string.Empty;
                    break;
                case "DateZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DateZ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionA.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionB.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionC.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionD.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionE.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionF.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionG.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionH.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionI.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionJ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionK.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionL.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionM.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionN.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionO.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionP.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionQ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionR.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionS.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionT.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionU.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionV.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionW.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionX.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionY.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.DescriptionZ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckA.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckB.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckC.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckD.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckE.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckF.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckG.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckH.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckI.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckJ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckK.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckL.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckM.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckN.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckO.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckP.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckQ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckR.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckS.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckT.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckU.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckV.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckW.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckX.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckY.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CheckZ.ToExport(column)
                        : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.SiteTitle.ToExport(column)
                        : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Comments.ToExport(column)
                        : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Creator.ToExport(column)
                        : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.Updator.ToExport(column)
                        : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? resultModel.CreatedTime.ToExport(column)
                        : string.Empty;
                    break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string Calendar(SiteSettings ss)
        {
            if (ss.EnableCalendar != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
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
                resultCollection: resultCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.Calendar(
                            ss: ss,
                            columnName: columnName,
                            month: month,
                            begin: begin,
                            dataRows: dataRows,
                            bodyOnly: false);
                    }
                });
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
            if (ss.EnableCalendar != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
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
                            bodyOnly: bodyOnly))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Invoke("setCalendar")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CalendarBody",
                        new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
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
                        .ResultId(_as: "Id")
                        .Title()
                        .ResultsColumn(columnName, _as: "Date"),
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
            bool bodyOnly)
        {
            return !bodyOnly
                ? hb.Calendar(
                    ss: ss,
                    columnName: columnName,
                    month: month,
                    begin: begin,
                    dataRows: dataRows)
                : hb.CalendarBody(
                    ss: ss,
                    column: ss.GetColumn(columnName),
                    month: month,
                    begin: begin,
                    dataRows: dataRows);
        }

        public static string Crosstab(SiteSettings ss)
        {
            if (ss.EnableCrosstab != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = view.GetCrosstabGroupByX(ss);
            var groupByY = view.GetCrosstabGroupByY(ss);
            var columns = view.CrosstabColumns;
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = view.GetCrosstabValue(ss);
            if (value.IsNullOrEmpty())
            {
                value = "ResultId";
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                ss, view, groupByX, groupByY, columns, value, aggregateType, timePeriod, month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(dataRows);
            var inRangeY =
                groupByY == "Columns" ||
                Libraries.ViewModes.CrosstabUtilities.InRangeY(dataRows);
            return hb.ViewModeTemplate(
                ss: ss,
                resultCollection: resultCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRangeX && inRangeY)
                    {
                        hb.Crosstab(
                            ss: ss,
                            view: view,
                            groupByX: groupByX,
                            groupByY: groupByY,
                            columns: columns,
                            aggregateType: aggregateType,
                            value: value,
                            timePeriod: timePeriod,
                            month: month,
                            dataRows: dataRows);
                    }
                });
        }

        public static string CrosstabJson(SiteSettings ss)
        {
            if (ss.EnableCrosstab != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = view.GetCrosstabGroupByX(ss);
            var groupByY = view.GetCrosstabGroupByY(ss);
            var columns = view.CrosstabColumns;
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = view.GetCrosstabValue(ss);
            if (value.IsNullOrEmpty())
            {
                value = "ResultId";
                aggregateType = "Count";
            }
            var timePeriod = view.GetCrosstabTimePeriod(ss);
            var month = view.GetCrosstabMonth(ss);
            var dataRows = CrosstabDataRows(
                ss, view, groupByX, groupByY, columns, value, aggregateType, timePeriod, month);
            var inRangeX = Libraries.ViewModes.CrosstabUtilities.InRangeX(dataRows);
            var inRangeY =
                groupByY == "Columns" ||
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
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Invoke("setCrosstab")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#CrosstabBody",
                        new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(!inRangeX
                        ? Parameters.General.CrosstabXLimit.ToString()
                        : Parameters.General.CrosstabYLimit.ToString()))
                    .ToJson();
        }

        private static EnumerableRowCollection<DataRow> CrosstabDataRows(
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string columns,
            string value,
            string aggregateType,
            string timePeriod,
            DateTime month)
        {
            var column = ss.GetColumn(groupByX);
            if (column?.TypeName != "datetime")
            {
                return Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        column: Rds.ResultsColumn()
                            .ResultsColumn(groupByX, _as: "GroupByX")
                            .CrosstabColumns(
                                ss,
                                groupByY,
                                columns.Deserialize<IEnumerable<string>>(),
                                value,
                                aggregateType),
                        where: view.Where(ss: ss),
                        groupBy: Rds.ResultsGroupBy()
                            .ResultsGroupBy(groupByX)
                            .ResultsGroupBy(groupByY, _using: groupByY != "Columns")))
                                .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    ss, column, timePeriod);
                return Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        column: Rds.ResultsColumn()
                            .Add(dateGroup, _as: "GroupByX")
                            .CrosstabColumns(
                                ss,
                                groupByY,
                                columns.Deserialize<IEnumerable<string>>(),
                                value,
                                aggregateType),
                        where: view.Where(
                            ss: ss,
                            where: Libraries.ViewModes.CrosstabUtilities.Where(
                                ss, ss.GetColumn(groupByX)?.ColumnName, timePeriod, month)),
                        groupBy: Rds.ResultsGroupBy()
                            .Add(dateGroup)
                            .ResultsGroupBy(groupByY, _using: groupByY != "Columns")))
                                .AsEnumerable();
            }
        }

        private static Rds.ResultsColumnCollection CrosstabColumns(
            this Rds.ResultsColumnCollection self,
            SiteSettings ss,
            string groupByY,
            IEnumerable<string> columns,
            string value,
            string aggregateType)
        {
            if (groupByY != "Columns")
            {
                return self
                    .ResultsColumn(groupByY, _as: "GroupByY")
                    .ResultsColumn(value, _as: "Value", function: Sqls.Function(aggregateType));
            }
            else
            {
                Libraries.ViewModes.CrosstabUtilities.GetColumns(ss, columns)
                    .ForEach(column =>
                        self.ResultsColumn(
                            column,
                            _as: column,
                            function: Sqls.Function(aggregateType)));
                return self;
            }
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeries(SiteSettings ss)
        {
            if (ss.EnableTimeSeries != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = resultCollection.Aggregations.TotalCount <=
                Parameters.General.TimeSeriesLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                resultCollection: resultCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.TimeSeries(
                            ss: ss,
                            view: view,
                            bodyOnly: false);
                    }
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeriesJson(
            SiteSettings ss)
        {
            if (ss.EnableTimeSeries != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("TimeSeries");
            return resultCollection.Aggregations.TotalCount <= Parameters.General.TimeSeriesLimit
                ? new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#TimeSeriesBody",
                        new HtmlBuilder().TimeSeries(
                            ss: ss,
                            view: view,
                            bodyOnly: bodyOnly))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Invoke("drawTimeSeries")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#TimeSeriesBody",
                        new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder TimeSeries(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly)
        {
            var groupBy = !view.TimeSeriesGroupBy.IsNullOrEmpty()
                ? view.TimeSeriesGroupBy
                : ss.TimeSeriesGroupByOptions().FirstOrDefault().Key;
            var aggregateType = !view.TimeSeriesAggregateType.IsNullOrEmpty()
                ? view.TimeSeriesAggregateType
                : "Count";
            var value = !view.TimeSeriesValue.IsNullOrEmpty()
                ? view.TimeSeriesValue
                : ss.TimeSeriesValueOptions().FirstOrDefault().Key;
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
                    dataRows: dataRows)
                : hb.TimeSeriesBody(
                    ss: ss,
                    groupBy: groupBy,
                    aggregateType: aggregateType,
                    value: value,
                    dataRows: dataRows);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> TimeSeriesDataRows(
            SiteSettings ss, View view, string groupBy, string value)
        {
            return groupBy != string.Empty && value != string.Empty
                ? Rds.ExecuteTable(statements:
                    Rds.SelectResults(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.ResultsColumn()
                            .ResultId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .ResultsColumn(groupBy, _as: "Index")
                            .ResultsColumn(value, _as: "Value"),
                        where: view.Where(ss: ss)))
                            .AsEnumerable()
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Kamban(SiteSettings ss)
        {
            if (ss.EnableKamban != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = resultCollection.Aggregations.TotalCount <=
                Parameters.General.KambanLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.KambanLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                resultCollection: resultCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.Kamban(
                            ss: ss,
                            view: view,
                            bodyOnly: false);
                    }
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string KambanJson(SiteSettings ss)
        {
            if (ss.EnableKamban != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Kamban");
            return resultCollection.Aggregations.TotalCount <= Parameters.General.KambanLimit
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
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Invoke("setKamban")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#KambanBody",
                        new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: resultCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.KambanLimit.ToString()))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly,
            long changedItemId = 0)
        {
            var groupByX = !view.KambanGroupByX.IsNullOrEmpty()
                ? view.KambanGroupByX
                : ss.KambanGroupByOptions().FirstOrDefault().Key;
            var groupByY = !view.KambanGroupByY.IsNullOrEmpty()
                ? view.KambanGroupByY
                : string.Empty;
            var aggregateType = !view.KambanAggregateType.IsNullOrEmpty()
                ? view.KambanAggregateType
                : ss.KambanAggregationTypeOptions().FirstOrDefault().Key;
            var value = !view.KambanValue.IsNullOrEmpty()
                ? view.KambanValue
                : KambanValue(ss);
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
                        KambanColumns(ss, groupByX, groupByY, value)))
                : hb.KambanBody(
                    ss: ss,
                    view: view,
                    groupByX: ss.GetColumn(groupByX),
                    groupByY: ss.GetColumn(groupByY),
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
                    changedItemId: changedItemId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string KambanValue(SiteSettings ss)
        {
            var column = ss.GetEditorColumns()
                .Where(o => o.Computable)
                .Where(o => o.TypeName != "datetime")
                .FirstOrDefault();
            return column != null
                ? column.ColumnName
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Rds.ResultsColumnCollection KambanColumns(
            SiteSettings ss, string groupByX, string groupByY, string value)
        {
            var column = Rds.ResultsColumn()
                .ResultId()
                .Title()
                .Manager()
                .Owner();
            ss.GetTitleColumns().ForEach(titleColumn =>
                column.ResultsColumn(titleColumn.ColumnName));
            return column
                .ResultsColumn(groupByX)
                .ResultsColumn(groupByY)
                .ResultsColumn(value);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static IEnumerable<Libraries.ViewModes.KambanElement> KambanElements(
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string value,
            Rds.ResultsColumnCollection column)
        {
            return new ResultCollection(
                ss: ss,
                column: column,
                where: view.Where(ss: ss),
                orderBy: view.OrderBy(ss, Rds.ResultsOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)))
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Id,
                            Title = o.Title.DisplayValue,
                            Manager = o.Manager,
                            Owner = o.Owner,
                            GroupX = o.PropertyValue(groupByX),
                            GroupY = o.PropertyValue(groupByY),
                            Value = o.PropertyValue(value).ToDecimal()
                        });
        }
    }
}
