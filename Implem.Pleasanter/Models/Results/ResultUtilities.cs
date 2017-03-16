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
            return hb.Template(
                ss: ss,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: ss.HasPermission(),
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
                .ToJson();
        }

        private static ResultCollection ResultCollection(
            SiteSettings ss, View view, int offset = 0)
        {
            return new ResultCollection(
                ss: ss,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss, Rds.ResultsWhere().SiteId(ss.SiteId)),
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
            var columns = ss.GetGridColumns();
            ss.Links?
                .Where(o => ss.GridColumns.Contains(o.ColumnName))
                .Where(o => ss.GetColumn(o.ColumnName).UseSearch == true)
                .ForEach(link =>
                    ss.SetChoiceHash(
                        columnName: link.ColumnName,
                        selectedValues: resultCollection
                            .Select(o => o.PropertyValue(link.ColumnName).ToLong())
                            .Distinct()));
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columnCollection: columns, 
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
                switch (column.ColumnName)
                {
                    case "SiteId": return hb.Td(column: column, value: resultModel.SiteId);
                    case "UpdatedTime": return hb.Td(column: column, value: resultModel.UpdatedTime);
                    case "ResultId": return hb.Td(column: column, value: resultModel.ResultId);
                    case "Ver": return hb.Td(column: column, value: resultModel.Ver);
                    case "Title": return hb.Td(column: column, value: resultModel.Title);
                    case "Body": return hb.Td(column: column, value: resultModel.Body);
                    case "TitleBody": return hb.Td(column: column, value: resultModel.TitleBody);
                    case "Status": return hb.Td(column: column, value: resultModel.Status);
                    case "Manager": return hb.Td(column: column, value: resultModel.Manager);
                    case "Owner": return hb.Td(column: column, value: resultModel.Owner);
                    case "ClassA": return hb.Td(column: column, value: resultModel.ClassA);
                    case "ClassB": return hb.Td(column: column, value: resultModel.ClassB);
                    case "ClassC": return hb.Td(column: column, value: resultModel.ClassC);
                    case "ClassD": return hb.Td(column: column, value: resultModel.ClassD);
                    case "ClassE": return hb.Td(column: column, value: resultModel.ClassE);
                    case "ClassF": return hb.Td(column: column, value: resultModel.ClassF);
                    case "ClassG": return hb.Td(column: column, value: resultModel.ClassG);
                    case "ClassH": return hb.Td(column: column, value: resultModel.ClassH);
                    case "ClassI": return hb.Td(column: column, value: resultModel.ClassI);
                    case "ClassJ": return hb.Td(column: column, value: resultModel.ClassJ);
                    case "ClassK": return hb.Td(column: column, value: resultModel.ClassK);
                    case "ClassL": return hb.Td(column: column, value: resultModel.ClassL);
                    case "ClassM": return hb.Td(column: column, value: resultModel.ClassM);
                    case "ClassN": return hb.Td(column: column, value: resultModel.ClassN);
                    case "ClassO": return hb.Td(column: column, value: resultModel.ClassO);
                    case "ClassP": return hb.Td(column: column, value: resultModel.ClassP);
                    case "ClassQ": return hb.Td(column: column, value: resultModel.ClassQ);
                    case "ClassR": return hb.Td(column: column, value: resultModel.ClassR);
                    case "ClassS": return hb.Td(column: column, value: resultModel.ClassS);
                    case "ClassT": return hb.Td(column: column, value: resultModel.ClassT);
                    case "ClassU": return hb.Td(column: column, value: resultModel.ClassU);
                    case "ClassV": return hb.Td(column: column, value: resultModel.ClassV);
                    case "ClassW": return hb.Td(column: column, value: resultModel.ClassW);
                    case "ClassX": return hb.Td(column: column, value: resultModel.ClassX);
                    case "ClassY": return hb.Td(column: column, value: resultModel.ClassY);
                    case "ClassZ": return hb.Td(column: column, value: resultModel.ClassZ);
                    case "NumA": return hb.Td(column: column, value: resultModel.NumA);
                    case "NumB": return hb.Td(column: column, value: resultModel.NumB);
                    case "NumC": return hb.Td(column: column, value: resultModel.NumC);
                    case "NumD": return hb.Td(column: column, value: resultModel.NumD);
                    case "NumE": return hb.Td(column: column, value: resultModel.NumE);
                    case "NumF": return hb.Td(column: column, value: resultModel.NumF);
                    case "NumG": return hb.Td(column: column, value: resultModel.NumG);
                    case "NumH": return hb.Td(column: column, value: resultModel.NumH);
                    case "NumI": return hb.Td(column: column, value: resultModel.NumI);
                    case "NumJ": return hb.Td(column: column, value: resultModel.NumJ);
                    case "NumK": return hb.Td(column: column, value: resultModel.NumK);
                    case "NumL": return hb.Td(column: column, value: resultModel.NumL);
                    case "NumM": return hb.Td(column: column, value: resultModel.NumM);
                    case "NumN": return hb.Td(column: column, value: resultModel.NumN);
                    case "NumO": return hb.Td(column: column, value: resultModel.NumO);
                    case "NumP": return hb.Td(column: column, value: resultModel.NumP);
                    case "NumQ": return hb.Td(column: column, value: resultModel.NumQ);
                    case "NumR": return hb.Td(column: column, value: resultModel.NumR);
                    case "NumS": return hb.Td(column: column, value: resultModel.NumS);
                    case "NumT": return hb.Td(column: column, value: resultModel.NumT);
                    case "NumU": return hb.Td(column: column, value: resultModel.NumU);
                    case "NumV": return hb.Td(column: column, value: resultModel.NumV);
                    case "NumW": return hb.Td(column: column, value: resultModel.NumW);
                    case "NumX": return hb.Td(column: column, value: resultModel.NumX);
                    case "NumY": return hb.Td(column: column, value: resultModel.NumY);
                    case "NumZ": return hb.Td(column: column, value: resultModel.NumZ);
                    case "DateA": return hb.Td(column: column, value: resultModel.DateA);
                    case "DateB": return hb.Td(column: column, value: resultModel.DateB);
                    case "DateC": return hb.Td(column: column, value: resultModel.DateC);
                    case "DateD": return hb.Td(column: column, value: resultModel.DateD);
                    case "DateE": return hb.Td(column: column, value: resultModel.DateE);
                    case "DateF": return hb.Td(column: column, value: resultModel.DateF);
                    case "DateG": return hb.Td(column: column, value: resultModel.DateG);
                    case "DateH": return hb.Td(column: column, value: resultModel.DateH);
                    case "DateI": return hb.Td(column: column, value: resultModel.DateI);
                    case "DateJ": return hb.Td(column: column, value: resultModel.DateJ);
                    case "DateK": return hb.Td(column: column, value: resultModel.DateK);
                    case "DateL": return hb.Td(column: column, value: resultModel.DateL);
                    case "DateM": return hb.Td(column: column, value: resultModel.DateM);
                    case "DateN": return hb.Td(column: column, value: resultModel.DateN);
                    case "DateO": return hb.Td(column: column, value: resultModel.DateO);
                    case "DateP": return hb.Td(column: column, value: resultModel.DateP);
                    case "DateQ": return hb.Td(column: column, value: resultModel.DateQ);
                    case "DateR": return hb.Td(column: column, value: resultModel.DateR);
                    case "DateS": return hb.Td(column: column, value: resultModel.DateS);
                    case "DateT": return hb.Td(column: column, value: resultModel.DateT);
                    case "DateU": return hb.Td(column: column, value: resultModel.DateU);
                    case "DateV": return hb.Td(column: column, value: resultModel.DateV);
                    case "DateW": return hb.Td(column: column, value: resultModel.DateW);
                    case "DateX": return hb.Td(column: column, value: resultModel.DateX);
                    case "DateY": return hb.Td(column: column, value: resultModel.DateY);
                    case "DateZ": return hb.Td(column: column, value: resultModel.DateZ);
                    case "DescriptionA": return hb.Td(column: column, value: resultModel.DescriptionA);
                    case "DescriptionB": return hb.Td(column: column, value: resultModel.DescriptionB);
                    case "DescriptionC": return hb.Td(column: column, value: resultModel.DescriptionC);
                    case "DescriptionD": return hb.Td(column: column, value: resultModel.DescriptionD);
                    case "DescriptionE": return hb.Td(column: column, value: resultModel.DescriptionE);
                    case "DescriptionF": return hb.Td(column: column, value: resultModel.DescriptionF);
                    case "DescriptionG": return hb.Td(column: column, value: resultModel.DescriptionG);
                    case "DescriptionH": return hb.Td(column: column, value: resultModel.DescriptionH);
                    case "DescriptionI": return hb.Td(column: column, value: resultModel.DescriptionI);
                    case "DescriptionJ": return hb.Td(column: column, value: resultModel.DescriptionJ);
                    case "DescriptionK": return hb.Td(column: column, value: resultModel.DescriptionK);
                    case "DescriptionL": return hb.Td(column: column, value: resultModel.DescriptionL);
                    case "DescriptionM": return hb.Td(column: column, value: resultModel.DescriptionM);
                    case "DescriptionN": return hb.Td(column: column, value: resultModel.DescriptionN);
                    case "DescriptionO": return hb.Td(column: column, value: resultModel.DescriptionO);
                    case "DescriptionP": return hb.Td(column: column, value: resultModel.DescriptionP);
                    case "DescriptionQ": return hb.Td(column: column, value: resultModel.DescriptionQ);
                    case "DescriptionR": return hb.Td(column: column, value: resultModel.DescriptionR);
                    case "DescriptionS": return hb.Td(column: column, value: resultModel.DescriptionS);
                    case "DescriptionT": return hb.Td(column: column, value: resultModel.DescriptionT);
                    case "DescriptionU": return hb.Td(column: column, value: resultModel.DescriptionU);
                    case "DescriptionV": return hb.Td(column: column, value: resultModel.DescriptionV);
                    case "DescriptionW": return hb.Td(column: column, value: resultModel.DescriptionW);
                    case "DescriptionX": return hb.Td(column: column, value: resultModel.DescriptionX);
                    case "DescriptionY": return hb.Td(column: column, value: resultModel.DescriptionY);
                    case "DescriptionZ": return hb.Td(column: column, value: resultModel.DescriptionZ);
                    case "CheckA": return hb.Td(column: column, value: resultModel.CheckA);
                    case "CheckB": return hb.Td(column: column, value: resultModel.CheckB);
                    case "CheckC": return hb.Td(column: column, value: resultModel.CheckC);
                    case "CheckD": return hb.Td(column: column, value: resultModel.CheckD);
                    case "CheckE": return hb.Td(column: column, value: resultModel.CheckE);
                    case "CheckF": return hb.Td(column: column, value: resultModel.CheckF);
                    case "CheckG": return hb.Td(column: column, value: resultModel.CheckG);
                    case "CheckH": return hb.Td(column: column, value: resultModel.CheckH);
                    case "CheckI": return hb.Td(column: column, value: resultModel.CheckI);
                    case "CheckJ": return hb.Td(column: column, value: resultModel.CheckJ);
                    case "CheckK": return hb.Td(column: column, value: resultModel.CheckK);
                    case "CheckL": return hb.Td(column: column, value: resultModel.CheckL);
                    case "CheckM": return hb.Td(column: column, value: resultModel.CheckM);
                    case "CheckN": return hb.Td(column: column, value: resultModel.CheckN);
                    case "CheckO": return hb.Td(column: column, value: resultModel.CheckO);
                    case "CheckP": return hb.Td(column: column, value: resultModel.CheckP);
                    case "CheckQ": return hb.Td(column: column, value: resultModel.CheckQ);
                    case "CheckR": return hb.Td(column: column, value: resultModel.CheckR);
                    case "CheckS": return hb.Td(column: column, value: resultModel.CheckS);
                    case "CheckT": return hb.Td(column: column, value: resultModel.CheckT);
                    case "CheckU": return hb.Td(column: column, value: resultModel.CheckU);
                    case "CheckV": return hb.Td(column: column, value: resultModel.CheckV);
                    case "CheckW": return hb.Td(column: column, value: resultModel.CheckW);
                    case "CheckX": return hb.Td(column: column, value: resultModel.CheckX);
                    case "CheckY": return hb.Td(column: column, value: resultModel.CheckY);
                    case "CheckZ": return hb.Td(column: column, value: resultModel.CheckZ);
                    case "Comments": return hb.Td(column: column, value: resultModel.Comments);
                    case "Creator": return hb.Td(column: column, value: resultModel.Creator);
                    case "Updator": return hb.Td(column: column, value: resultModel.Updator);
                    case "CreatedTime": return hb.Td(column: column, value: resultModel.CreatedTime);
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
            return ss.CanCreate()
                ? Editor(ss, new ResultModel(ss, methodType: BaseModel.MethodTypes.New))
                : new HtmlBuilder().NotFoundTemplate().ToString();
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
            var hb = new HtmlBuilder();
            return hb.Template(
                ss: ss,
                verType: resultModel.VerType,
                methodType: resultModel.MethodType,
                allowAccess:
                    ss.CanRead() &&
                    resultModel.AccessStatus != Databases.AccessStatuses.NotFound,
                siteId: resultModel.SiteId,
                referenceType: "Results",
                title: resultModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.New()
                    : resultModel.Title.DisplayValue,
                useTitle: ss.EditorColumns.Contains("Title"),
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
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("ResultForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(resultModel.ResultId != 0 
                            ? resultModel.ResultId
                            : resultModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: resultModel,
                            tableName: "Results")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: resultModel.Comments,
                                verType: resultModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(resultModel: resultModel)
                            .FieldSetGeneral(
                                ss: ss,
                                resultModel: resultModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: resultModel.MethodType != BaseModel.MethodTypes.New)
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
                .EditorExtensions(resultModel: resultModel, ss: ss));
        }

        private static HtmlBuilder EditorTabs(this HtmlBuilder hb, ResultModel resultModel)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: resultModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            ResultModel resultModel)
        {
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
                                resultModel.ResultId.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Ver":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Ver.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Title":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Title.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Body":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Body.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Status":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Status.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Manager":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Manager.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Owner":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.Owner.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassA.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassB.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassC.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassD.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassE.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassF.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassG.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassH.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassI.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassJ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassK.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassL.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassM.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassN.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassO.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassP.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassQ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassR.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassS.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassT.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassU.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassV.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassW.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassX.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassY.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.ClassZ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumA.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumB.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumC.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumD.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumE.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumF.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumG.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumH.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumI.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumJ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumK.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumL.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumM.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumN.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumO.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumP.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumQ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumR.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumS.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumT.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumU.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumV.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumW.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumX.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumY.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.NumZ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateA.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateB.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateC.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateD.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateE.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateF.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateG.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateH.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateI.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateJ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateK.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateL.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateM.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateN.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateO.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateP.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateQ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateR.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateS.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateT.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateU.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateV.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateW.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateX.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateY.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DateZ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionA.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionB.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionC.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionD.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionE.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionF.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionG.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionH.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionI.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionJ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionK.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionL.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionM.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionN.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionO.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionP.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionQ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionR.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionS.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionT.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionU.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionV.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionW.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionX.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionY.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.DescriptionZ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckA":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckA.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckB":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckB.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckC":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckC.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckD":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckD.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckE":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckE.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckF":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckF.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckG":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckG.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckH":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckH.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckI":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckI.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckJ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckJ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckK":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckK.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckL":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckL.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckM":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckM.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckN":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckN.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckO":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckO.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckP":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckP.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckQ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckQ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckR":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckR.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckS":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckS.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckT":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckT.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckU":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckU.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckV":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckV.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckW":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckW.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckX":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckX.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckY":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckY.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckZ":
                            hb.Field(
                                ss,
                                column,
                                resultModel.MethodType,
                                resultModel.CheckZ.ToControl(column, ss),
                                column.ColumnPermissionType(ss));
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
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static List<long> GetSwitchTargets(SiteSettings ss, long resultId, long siteId)
        {
            var view = Views.GetBySession(ss);
            var switchTargets = Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectResults(
                    column: Rds.ResultsColumn().ResultId(),
                    where: view.Where(
                        ss: ss, where: Rds.ResultsWhere().SiteId(siteId)),
                    orderBy: view.OrderBy(ss, Rds.ResultsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc))))
                            .AsEnumerable()
                            .Select(o => o["ResultId"].ToLong())
                            .ToList();
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
                                resultModel.NumA.ToControl(column, ss));
                            break;
                        case "NumB":
                            res.Val(
                                "#Results_NumB",
                                resultModel.NumB.ToControl(column, ss));
                            break;
                        case "NumC":
                            res.Val(
                                "#Results_NumC",
                                resultModel.NumC.ToControl(column, ss));
                            break;
                        case "NumD":
                            res.Val(
                                "#Results_NumD",
                                resultModel.NumD.ToControl(column, ss));
                            break;
                        case "NumE":
                            res.Val(
                                "#Results_NumE",
                                resultModel.NumE.ToControl(column, ss));
                            break;
                        case "NumF":
                            res.Val(
                                "#Results_NumF",
                                resultModel.NumF.ToControl(column, ss));
                            break;
                        case "NumG":
                            res.Val(
                                "#Results_NumG",
                                resultModel.NumG.ToControl(column, ss));
                            break;
                        case "NumH":
                            res.Val(
                                "#Results_NumH",
                                resultModel.NumH.ToControl(column, ss));
                            break;
                        case "NumI":
                            res.Val(
                                "#Results_NumI",
                                resultModel.NumI.ToControl(column, ss));
                            break;
                        case "NumJ":
                            res.Val(
                                "#Results_NumJ",
                                resultModel.NumJ.ToControl(column, ss));
                            break;
                        case "NumK":
                            res.Val(
                                "#Results_NumK",
                                resultModel.NumK.ToControl(column, ss));
                            break;
                        case "NumL":
                            res.Val(
                                "#Results_NumL",
                                resultModel.NumL.ToControl(column, ss));
                            break;
                        case "NumM":
                            res.Val(
                                "#Results_NumM",
                                resultModel.NumM.ToControl(column, ss));
                            break;
                        case "NumN":
                            res.Val(
                                "#Results_NumN",
                                resultModel.NumN.ToControl(column, ss));
                            break;
                        case "NumO":
                            res.Val(
                                "#Results_NumO",
                                resultModel.NumO.ToControl(column, ss));
                            break;
                        case "NumP":
                            res.Val(
                                "#Results_NumP",
                                resultModel.NumP.ToControl(column, ss));
                            break;
                        case "NumQ":
                            res.Val(
                                "#Results_NumQ",
                                resultModel.NumQ.ToControl(column, ss));
                            break;
                        case "NumR":
                            res.Val(
                                "#Results_NumR",
                                resultModel.NumR.ToControl(column, ss));
                            break;
                        case "NumS":
                            res.Val(
                                "#Results_NumS",
                                resultModel.NumS.ToControl(column, ss));
                            break;
                        case "NumT":
                            res.Val(
                                "#Results_NumT",
                                resultModel.NumT.ToControl(column, ss));
                            break;
                        case "NumU":
                            res.Val(
                                "#Results_NumU",
                                resultModel.NumU.ToControl(column, ss));
                            break;
                        case "NumV":
                            res.Val(
                                "#Results_NumV",
                                resultModel.NumV.ToControl(column, ss));
                            break;
                        case "NumW":
                            res.Val(
                                "#Results_NumW",
                                resultModel.NumW.ToControl(column, ss));
                            break;
                        case "NumX":
                            res.Val(
                                "#Results_NumX",
                                resultModel.NumX.ToControl(column, ss));
                            break;
                        case "NumY":
                            res.Val(
                                "#Results_NumY",
                                resultModel.NumY.ToControl(column, ss));
                            break;
                        case "NumZ":
                            res.Val(
                                "#Results_NumZ",
                                resultModel.NumZ.ToControl(column, ss));
                            break;
                        default: break;
                    }
                });
            return res;
        }

        public static string Create(SiteSettings ss)
        {
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
                    Messages.Created(resultModel.Title.Value),
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
            var error = resultModel.Update(ss, notice: true);
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(resultModel.Updator.FullName()).ToJson()
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
                .Message(Messages.Updated(resultModel.Title.ToString()))
                .RemoveComment(resultModel.DeleteCommentId, _using: resultModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Copy(SiteSettings ss, long resultId)
        {
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
            var error = resultModel.Create(ss, paramAll: true);
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
                res.Href(Locations.ItemIndex(resultModel.SiteId));
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
            var columns = ss.GetHistoryColumns();
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: columns,
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
            if (Permissions.CanMove(ss, SiteSettingsUtilities.Get(siteId, siteId)))
            {
                var count = 0;
                if (Forms.Bool("GridCheckAll"))
                {
                    count = BulkMove(
                        siteId,
                        ss,
                        GridItems("GridUnCheckedItems"),
                        negative: true);
                }
                else
                {
                    var checkedItems = GridItems("GridCheckedItems");
                    if (checkedItems.Any())
                    {
                        count = BulkMove(
                            siteId,
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
                    message: Messages.BulkMoved(count.ToString()));
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        private static int BulkMove(
            long siteId,
            SiteSettings ss,
            IEnumerable<long> checkedItems,
            bool negative = false)
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
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Any())),
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
            if (csv != null && csv.Rows.Count() != 0)
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
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.ResultsParam();
                    param.ResultId(raw: Def.Sql.Identity);
                    param.SiteId(siteModel.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], siteModel.InheritPermission);
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
                    paramHash.Add(data.Index, param);
                });
                paramHash.Values.ForEach(param =>
                    new ResultModel()
                    {
                        SiteId = siteModel.SiteId,
                        Title = new Title(param.FirstOrDefault(o =>
                            o.Name == "Title").Value.ToString())
                    }.Create(ss, param: param));
                return GridRows(ss, res
                    .WindowScrollTop()
                    .CloseDialog("#ImportSettingsDialog")
                    .Message(Messages.Imported(csv.Rows.Count().ToString())));
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
            var invalid = IssueValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(ss);
            var resultCollection = new ResultCollection(
                ss: ss,
                where: view.Where(
                    ss: ss, where: Rds.ResultsWhere().SiteId(siteModel.SiteId)),
                orderBy: view.OrderBy(ss, Rds.ResultsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
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
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            resultModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(ResultModel resultModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = resultModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = resultModel.UpdatedTime.ToExport(column); break;
                case "ResultId": value = resultModel.ResultId.ToExport(column); break;
                case "Ver": value = resultModel.Ver.ToExport(column); break;
                case "Title": value = resultModel.Title.ToExport(column); break;
                case "Body": value = resultModel.Body.ToExport(column); break;
                case "TitleBody": value = resultModel.TitleBody.ToExport(column); break;
                case "Status": value = resultModel.Status.ToExport(column); break;
                case "Manager": value = resultModel.Manager.ToExport(column); break;
                case "Owner": value = resultModel.Owner.ToExport(column); break;
                case "ClassA": value = resultModel.ClassA.ToExport(column); break;
                case "ClassB": value = resultModel.ClassB.ToExport(column); break;
                case "ClassC": value = resultModel.ClassC.ToExport(column); break;
                case "ClassD": value = resultModel.ClassD.ToExport(column); break;
                case "ClassE": value = resultModel.ClassE.ToExport(column); break;
                case "ClassF": value = resultModel.ClassF.ToExport(column); break;
                case "ClassG": value = resultModel.ClassG.ToExport(column); break;
                case "ClassH": value = resultModel.ClassH.ToExport(column); break;
                case "ClassI": value = resultModel.ClassI.ToExport(column); break;
                case "ClassJ": value = resultModel.ClassJ.ToExport(column); break;
                case "ClassK": value = resultModel.ClassK.ToExport(column); break;
                case "ClassL": value = resultModel.ClassL.ToExport(column); break;
                case "ClassM": value = resultModel.ClassM.ToExport(column); break;
                case "ClassN": value = resultModel.ClassN.ToExport(column); break;
                case "ClassO": value = resultModel.ClassO.ToExport(column); break;
                case "ClassP": value = resultModel.ClassP.ToExport(column); break;
                case "ClassQ": value = resultModel.ClassQ.ToExport(column); break;
                case "ClassR": value = resultModel.ClassR.ToExport(column); break;
                case "ClassS": value = resultModel.ClassS.ToExport(column); break;
                case "ClassT": value = resultModel.ClassT.ToExport(column); break;
                case "ClassU": value = resultModel.ClassU.ToExport(column); break;
                case "ClassV": value = resultModel.ClassV.ToExport(column); break;
                case "ClassW": value = resultModel.ClassW.ToExport(column); break;
                case "ClassX": value = resultModel.ClassX.ToExport(column); break;
                case "ClassY": value = resultModel.ClassY.ToExport(column); break;
                case "ClassZ": value = resultModel.ClassZ.ToExport(column); break;
                case "NumA": value = resultModel.NumA.ToExport(column); break;
                case "NumB": value = resultModel.NumB.ToExport(column); break;
                case "NumC": value = resultModel.NumC.ToExport(column); break;
                case "NumD": value = resultModel.NumD.ToExport(column); break;
                case "NumE": value = resultModel.NumE.ToExport(column); break;
                case "NumF": value = resultModel.NumF.ToExport(column); break;
                case "NumG": value = resultModel.NumG.ToExport(column); break;
                case "NumH": value = resultModel.NumH.ToExport(column); break;
                case "NumI": value = resultModel.NumI.ToExport(column); break;
                case "NumJ": value = resultModel.NumJ.ToExport(column); break;
                case "NumK": value = resultModel.NumK.ToExport(column); break;
                case "NumL": value = resultModel.NumL.ToExport(column); break;
                case "NumM": value = resultModel.NumM.ToExport(column); break;
                case "NumN": value = resultModel.NumN.ToExport(column); break;
                case "NumO": value = resultModel.NumO.ToExport(column); break;
                case "NumP": value = resultModel.NumP.ToExport(column); break;
                case "NumQ": value = resultModel.NumQ.ToExport(column); break;
                case "NumR": value = resultModel.NumR.ToExport(column); break;
                case "NumS": value = resultModel.NumS.ToExport(column); break;
                case "NumT": value = resultModel.NumT.ToExport(column); break;
                case "NumU": value = resultModel.NumU.ToExport(column); break;
                case "NumV": value = resultModel.NumV.ToExport(column); break;
                case "NumW": value = resultModel.NumW.ToExport(column); break;
                case "NumX": value = resultModel.NumX.ToExport(column); break;
                case "NumY": value = resultModel.NumY.ToExport(column); break;
                case "NumZ": value = resultModel.NumZ.ToExport(column); break;
                case "DateA": value = resultModel.DateA.ToExport(column); break;
                case "DateB": value = resultModel.DateB.ToExport(column); break;
                case "DateC": value = resultModel.DateC.ToExport(column); break;
                case "DateD": value = resultModel.DateD.ToExport(column); break;
                case "DateE": value = resultModel.DateE.ToExport(column); break;
                case "DateF": value = resultModel.DateF.ToExport(column); break;
                case "DateG": value = resultModel.DateG.ToExport(column); break;
                case "DateH": value = resultModel.DateH.ToExport(column); break;
                case "DateI": value = resultModel.DateI.ToExport(column); break;
                case "DateJ": value = resultModel.DateJ.ToExport(column); break;
                case "DateK": value = resultModel.DateK.ToExport(column); break;
                case "DateL": value = resultModel.DateL.ToExport(column); break;
                case "DateM": value = resultModel.DateM.ToExport(column); break;
                case "DateN": value = resultModel.DateN.ToExport(column); break;
                case "DateO": value = resultModel.DateO.ToExport(column); break;
                case "DateP": value = resultModel.DateP.ToExport(column); break;
                case "DateQ": value = resultModel.DateQ.ToExport(column); break;
                case "DateR": value = resultModel.DateR.ToExport(column); break;
                case "DateS": value = resultModel.DateS.ToExport(column); break;
                case "DateT": value = resultModel.DateT.ToExport(column); break;
                case "DateU": value = resultModel.DateU.ToExport(column); break;
                case "DateV": value = resultModel.DateV.ToExport(column); break;
                case "DateW": value = resultModel.DateW.ToExport(column); break;
                case "DateX": value = resultModel.DateX.ToExport(column); break;
                case "DateY": value = resultModel.DateY.ToExport(column); break;
                case "DateZ": value = resultModel.DateZ.ToExport(column); break;
                case "DescriptionA": value = resultModel.DescriptionA.ToExport(column); break;
                case "DescriptionB": value = resultModel.DescriptionB.ToExport(column); break;
                case "DescriptionC": value = resultModel.DescriptionC.ToExport(column); break;
                case "DescriptionD": value = resultModel.DescriptionD.ToExport(column); break;
                case "DescriptionE": value = resultModel.DescriptionE.ToExport(column); break;
                case "DescriptionF": value = resultModel.DescriptionF.ToExport(column); break;
                case "DescriptionG": value = resultModel.DescriptionG.ToExport(column); break;
                case "DescriptionH": value = resultModel.DescriptionH.ToExport(column); break;
                case "DescriptionI": value = resultModel.DescriptionI.ToExport(column); break;
                case "DescriptionJ": value = resultModel.DescriptionJ.ToExport(column); break;
                case "DescriptionK": value = resultModel.DescriptionK.ToExport(column); break;
                case "DescriptionL": value = resultModel.DescriptionL.ToExport(column); break;
                case "DescriptionM": value = resultModel.DescriptionM.ToExport(column); break;
                case "DescriptionN": value = resultModel.DescriptionN.ToExport(column); break;
                case "DescriptionO": value = resultModel.DescriptionO.ToExport(column); break;
                case "DescriptionP": value = resultModel.DescriptionP.ToExport(column); break;
                case "DescriptionQ": value = resultModel.DescriptionQ.ToExport(column); break;
                case "DescriptionR": value = resultModel.DescriptionR.ToExport(column); break;
                case "DescriptionS": value = resultModel.DescriptionS.ToExport(column); break;
                case "DescriptionT": value = resultModel.DescriptionT.ToExport(column); break;
                case "DescriptionU": value = resultModel.DescriptionU.ToExport(column); break;
                case "DescriptionV": value = resultModel.DescriptionV.ToExport(column); break;
                case "DescriptionW": value = resultModel.DescriptionW.ToExport(column); break;
                case "DescriptionX": value = resultModel.DescriptionX.ToExport(column); break;
                case "DescriptionY": value = resultModel.DescriptionY.ToExport(column); break;
                case "DescriptionZ": value = resultModel.DescriptionZ.ToExport(column); break;
                case "CheckA": value = resultModel.CheckA.ToExport(column); break;
                case "CheckB": value = resultModel.CheckB.ToExport(column); break;
                case "CheckC": value = resultModel.CheckC.ToExport(column); break;
                case "CheckD": value = resultModel.CheckD.ToExport(column); break;
                case "CheckE": value = resultModel.CheckE.ToExport(column); break;
                case "CheckF": value = resultModel.CheckF.ToExport(column); break;
                case "CheckG": value = resultModel.CheckG.ToExport(column); break;
                case "CheckH": value = resultModel.CheckH.ToExport(column); break;
                case "CheckI": value = resultModel.CheckI.ToExport(column); break;
                case "CheckJ": value = resultModel.CheckJ.ToExport(column); break;
                case "CheckK": value = resultModel.CheckK.ToExport(column); break;
                case "CheckL": value = resultModel.CheckL.ToExport(column); break;
                case "CheckM": value = resultModel.CheckM.ToExport(column); break;
                case "CheckN": value = resultModel.CheckN.ToExport(column); break;
                case "CheckO": value = resultModel.CheckO.ToExport(column); break;
                case "CheckP": value = resultModel.CheckP.ToExport(column); break;
                case "CheckQ": value = resultModel.CheckQ.ToExport(column); break;
                case "CheckR": value = resultModel.CheckR.ToExport(column); break;
                case "CheckS": value = resultModel.CheckS.ToExport(column); break;
                case "CheckT": value = resultModel.CheckT.ToExport(column); break;
                case "CheckU": value = resultModel.CheckU.ToExport(column); break;
                case "CheckV": value = resultModel.CheckV.ToExport(column); break;
                case "CheckW": value = resultModel.CheckW.ToExport(column); break;
                case "CheckX": value = resultModel.CheckX.ToExport(column); break;
                case "CheckY": value = resultModel.CheckY.ToExport(column); break;
                case "CheckZ": value = resultModel.CheckZ.ToExport(column); break;
                case "Comments": value = resultModel.Comments.ToExport(column); break;
                case "Creator": value = resultModel.Creator.ToExport(column); break;
                case "Updator": value = resultModel.Updator.ToExport(column); break;
                case "CreatedTime": value = resultModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings ss, ResultModel resultModel)
        {
            var displayValue = ss.GetTitleColumns()
                .Select(column => TitleDisplayValue(column, resultModel))
                .Where(o => o != string.Empty)
                .Join(ss.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, ResultModel resultModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(resultModel.Title.Value).Text
                    : resultModel.Title.Value;
                case "ClassA": return column.HasChoices()
                    ? column.Choice(resultModel.ClassA).Text
                    : resultModel.ClassA;
                case "ClassB": return column.HasChoices()
                    ? column.Choice(resultModel.ClassB).Text
                    : resultModel.ClassB;
                case "ClassC": return column.HasChoices()
                    ? column.Choice(resultModel.ClassC).Text
                    : resultModel.ClassC;
                case "ClassD": return column.HasChoices()
                    ? column.Choice(resultModel.ClassD).Text
                    : resultModel.ClassD;
                case "ClassE": return column.HasChoices()
                    ? column.Choice(resultModel.ClassE).Text
                    : resultModel.ClassE;
                case "ClassF": return column.HasChoices()
                    ? column.Choice(resultModel.ClassF).Text
                    : resultModel.ClassF;
                case "ClassG": return column.HasChoices()
                    ? column.Choice(resultModel.ClassG).Text
                    : resultModel.ClassG;
                case "ClassH": return column.HasChoices()
                    ? column.Choice(resultModel.ClassH).Text
                    : resultModel.ClassH;
                case "ClassI": return column.HasChoices()
                    ? column.Choice(resultModel.ClassI).Text
                    : resultModel.ClassI;
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassJ).Text
                    : resultModel.ClassJ;
                case "ClassK": return column.HasChoices()
                    ? column.Choice(resultModel.ClassK).Text
                    : resultModel.ClassK;
                case "ClassL": return column.HasChoices()
                    ? column.Choice(resultModel.ClassL).Text
                    : resultModel.ClassL;
                case "ClassM": return column.HasChoices()
                    ? column.Choice(resultModel.ClassM).Text
                    : resultModel.ClassM;
                case "ClassN": return column.HasChoices()
                    ? column.Choice(resultModel.ClassN).Text
                    : resultModel.ClassN;
                case "ClassO": return column.HasChoices()
                    ? column.Choice(resultModel.ClassO).Text
                    : resultModel.ClassO;
                case "ClassP": return column.HasChoices()
                    ? column.Choice(resultModel.ClassP).Text
                    : resultModel.ClassP;
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassQ).Text
                    : resultModel.ClassQ;
                case "ClassR": return column.HasChoices()
                    ? column.Choice(resultModel.ClassR).Text
                    : resultModel.ClassR;
                case "ClassS": return column.HasChoices()
                    ? column.Choice(resultModel.ClassS).Text
                    : resultModel.ClassS;
                case "ClassT": return column.HasChoices()
                    ? column.Choice(resultModel.ClassT).Text
                    : resultModel.ClassT;
                case "ClassU": return column.HasChoices()
                    ? column.Choice(resultModel.ClassU).Text
                    : resultModel.ClassU;
                case "ClassV": return column.HasChoices()
                    ? column.Choice(resultModel.ClassV).Text
                    : resultModel.ClassV;
                case "ClassW": return column.HasChoices()
                    ? column.Choice(resultModel.ClassW).Text
                    : resultModel.ClassW;
                case "ClassX": return column.HasChoices()
                    ? column.Choice(resultModel.ClassX).Text
                    : resultModel.ClassX;
                case "ClassY": return column.HasChoices()
                    ? column.Choice(resultModel.ClassY).Text
                    : resultModel.ClassY;
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(resultModel.ClassZ).Text
                    : resultModel.ClassZ;
                default: return string.Empty;
            }
        }

        public static string TitleDisplayValue(SiteSettings ss, DataRow dataRow)
        {
            var displayValue = ss.GetTitleColumns()
                .Select(column => TitleDisplayValue(column, dataRow))
                .Where(o => o != string.Empty)
                .Join(ss.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, DataRow dataRow)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(dataRow["Title"].ToString()).Text
                    : dataRow["Title"].ToString();
                case "ClassA": return column.HasChoices()
                    ? column.Choice(dataRow["ClassA"].ToString()).Text
                    : dataRow["ClassA"].ToString();
                case "ClassB": return column.HasChoices()
                    ? column.Choice(dataRow["ClassB"].ToString()).Text
                    : dataRow["ClassB"].ToString();
                case "ClassC": return column.HasChoices()
                    ? column.Choice(dataRow["ClassC"].ToString()).Text
                    : dataRow["ClassC"].ToString();
                case "ClassD": return column.HasChoices()
                    ? column.Choice(dataRow["ClassD"].ToString()).Text
                    : dataRow["ClassD"].ToString();
                case "ClassE": return column.HasChoices()
                    ? column.Choice(dataRow["ClassE"].ToString()).Text
                    : dataRow["ClassE"].ToString();
                case "ClassF": return column.HasChoices()
                    ? column.Choice(dataRow["ClassF"].ToString()).Text
                    : dataRow["ClassF"].ToString();
                case "ClassG": return column.HasChoices()
                    ? column.Choice(dataRow["ClassG"].ToString()).Text
                    : dataRow["ClassG"].ToString();
                case "ClassH": return column.HasChoices()
                    ? column.Choice(dataRow["ClassH"].ToString()).Text
                    : dataRow["ClassH"].ToString();
                case "ClassI": return column.HasChoices()
                    ? column.Choice(dataRow["ClassI"].ToString()).Text
                    : dataRow["ClassI"].ToString();
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassJ"].ToString()).Text
                    : dataRow["ClassJ"].ToString();
                case "ClassK": return column.HasChoices()
                    ? column.Choice(dataRow["ClassK"].ToString()).Text
                    : dataRow["ClassK"].ToString();
                case "ClassL": return column.HasChoices()
                    ? column.Choice(dataRow["ClassL"].ToString()).Text
                    : dataRow["ClassL"].ToString();
                case "ClassM": return column.HasChoices()
                    ? column.Choice(dataRow["ClassM"].ToString()).Text
                    : dataRow["ClassM"].ToString();
                case "ClassN": return column.HasChoices()
                    ? column.Choice(dataRow["ClassN"].ToString()).Text
                    : dataRow["ClassN"].ToString();
                case "ClassO": return column.HasChoices()
                    ? column.Choice(dataRow["ClassO"].ToString()).Text
                    : dataRow["ClassO"].ToString();
                case "ClassP": return column.HasChoices()
                    ? column.Choice(dataRow["ClassP"].ToString()).Text
                    : dataRow["ClassP"].ToString();
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassQ"].ToString()).Text
                    : dataRow["ClassQ"].ToString();
                case "ClassR": return column.HasChoices()
                    ? column.Choice(dataRow["ClassR"].ToString()).Text
                    : dataRow["ClassR"].ToString();
                case "ClassS": return column.HasChoices()
                    ? column.Choice(dataRow["ClassS"].ToString()).Text
                    : dataRow["ClassS"].ToString();
                case "ClassT": return column.HasChoices()
                    ? column.Choice(dataRow["ClassT"].ToString()).Text
                    : dataRow["ClassT"].ToString();
                case "ClassU": return column.HasChoices()
                    ? column.Choice(dataRow["ClassU"].ToString()).Text
                    : dataRow["ClassU"].ToString();
                case "ClassV": return column.HasChoices()
                    ? column.Choice(dataRow["ClassV"].ToString()).Text
                    : dataRow["ClassV"].ToString();
                case "ClassW": return column.HasChoices()
                    ? column.Choice(dataRow["ClassW"].ToString()).Text
                    : dataRow["ClassW"].ToString();
                case "ClassX": return column.HasChoices()
                    ? column.Choice(dataRow["ClassX"].ToString()).Text
                    : dataRow["ClassX"].ToString();
                case "ClassY": return column.HasChoices()
                    ? column.Choice(dataRow["ClassY"].ToString()).Text
                    : dataRow["ClassY"].ToString();
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(dataRow["ClassZ"].ToString()).Text
                    : dataRow["ClassZ"].ToString();
                default: return string.Empty;
            }
        }

        public static string UpdateByKamban(SiteSettings ss)
        {
            var resultModel = new ResultModel(
                ss, Forms.Long("KambanId"), setByForm: true);
            resultModel.VerUp = Versions.MustVerUp(resultModel);
            resultModel.Update(ss, notice: true);
            return KambanJson(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeries(SiteSettings ss)
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
                viewModeBody: () => hb.TimeSeries(
                    ss: ss,
                    view: view,
                    bodyOnly: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeriesJson(
            SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("TimeSeries");
            return new ResponseCollection()
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
                .Invoke("drawTimeSeries")
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
            var formData = Forms.All();
            var groupBy = !view.TimeSeriesGroupBy.IsNullOrEmpty()
                ? view.TimeSeriesGroupBy
                : ss.TimeSeriesGroupByOptions().First().Key;
            var aggregateType = !view.TimeSeriesAggregateType.IsNullOrEmpty()
                ? view.TimeSeriesAggregateType
                : "Count";
            var value = !view.TimeSeriesValue.IsNullOrEmpty()
                ? view.TimeSeriesValue
                : ss.TimeSeriesValueOptions().First().Key;
            var dataRows = TimeSeriesDataRows(
                ss: ss,
                view: view,
                groupBy: groupBy,
                value: value);
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
                        where: view.Where(ss, Rds.ResultsWhere().SiteId(ss.SiteId))))
                            .AsEnumerable()
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Kamban(SiteSettings ss)
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
                viewModeBody: () => hb.Kamban(
                    ss: ss,
                    view: view,
                    bodyOnly: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string KambanJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var resultCollection = ResultCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Kamban");
            return new ResponseCollection()
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
                .Invoke("setKamban").ToJson();
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
            var formData = Forms.All();
            var groupByX = !view.KambanGroupByX.IsNullOrEmpty()
                ? view.KambanGroupByX
                : ss.KambanGroupByOptions().First().Key;
            var groupByY = !view.KambanGroupByY.IsNullOrEmpty()
                ? view.KambanGroupByY
                : string.Empty;
            var aggregateType = !view.KambanAggregateType.IsNullOrEmpty()
                ? view.KambanAggregateType
                : ss.KambanAggregationTypeOptions().First().Key;
            var value = !view.KambanValue.IsNullOrEmpty()
                ? view.KambanValue
                : KambanValue(ss);
            return !bodyOnly
                ? hb.Kamban(
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    columns: view.KambanColumns,
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
                where: view.Where(ss, Rds.ResultsWhere().SiteId(ss.SiteId)),
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
