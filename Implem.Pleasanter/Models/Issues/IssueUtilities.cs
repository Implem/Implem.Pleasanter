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
    public static class IssueUtilities
    {
        public static string Index(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Grid(
                   issueCollection: issueCollection,
                   ss: ss,
                   view: view));
        }

        private static string ViewModeTemplate(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueCollection issueCollection,
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
                referenceType: "Issues",
                script: Libraries.Scripts.JavaScripts.ViewMode(viewMode),
                userScript: ss.GridScript,
                userStyle: ss.GridStyle,
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
                                aggregations: issueCollection.Aggregations)
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
                            .Hidden(controlId: "TableName", value: "Issues")
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
            var issueCollection = IssueCollection(ss, view);
            return new ResponseCollection()
                .Html("#ViewModeContainer", new HtmlBuilder().Grid(
                    ss: ss,
                    issueCollection: issueCollection,
                    view: view))
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: issueCollection.Aggregations))
                .ToJson();
        }

        private static IssueCollection IssueCollection(
            SiteSettings ss, View view, int offset = 0)
        {
            return new IssueCollection(
                ss: ss,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss, Rds.IssuesWhere().SiteId(ss.SiteId)),
                orderBy: view.OrderBy(ss, Rds.IssuesOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: ss.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: ss.Aggregations);
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueCollection issueCollection,
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
                            issueCollection: issueCollection,
                            view: view))
                .Hidden(
                    controlId: "GridOffset",
                    value: ss.GridNextOffset(
                        0,
                        issueCollection.Count(),
                        issueCollection.Aggregations.TotalCount)
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
            var issueCollection = IssueCollection(ss, view, offset);
            return (res ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .ReplaceAll("#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: issueCollection.Aggregations),
                    _using: offset == 0)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    ss: ss,
                    issueCollection: issueCollection,
                    view: view,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Val("#GridOffset", ss.GridNextOffset(
                    offset,
                    issueCollection.Count(),
                    issueCollection.Aggregations.TotalCount))
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueCollection issueCollection,
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
                        selectedValues: issueCollection
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
                .TBody(action: () => issueCollection
                    .ForEach(issueModel => hb
                        .Tr(
                            attributes: new HtmlAttributes()
                                .Class("grid-row")
                                .DataId(issueModel.IssueId.ToString()),
                            action: () =>
                            {
                                hb.Td(action: () => hb
                                    .CheckBox(
                                        controlCss: "grid-check",
                                        _checked: checkAll,
                                        dataId: issueModel.IssueId.ToString()));
                                columns
                                    .ForEach(column => hb
                                        .TdValue(
                                            ss: ss,
                                            column: column,
                                            issueModel: issueModel));
                            })));
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings ss)
        {
            var sqlColumnCollection = Rds.IssuesColumn();
            new List<string> { "SiteId", "IssueId", "Creator", "Updator" }
                .Concat(ss.GridColumns)
                .Concat(ss.IncludedColumns())
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
                switch (column.ColumnName)
                {
                    case "SiteId": return hb.Td(column: column, value: issueModel.SiteId);
                    case "UpdatedTime": return hb.Td(column: column, value: issueModel.UpdatedTime);
                    case "IssueId": return hb.Td(column: column, value: issueModel.IssueId);
                    case "Ver": return hb.Td(column: column, value: issueModel.Ver);
                    case "Title": return hb.Td(column: column, value: issueModel.Title);
                    case "Body": return hb.Td(column: column, value: issueModel.Body);
                    case "TitleBody": return hb.Td(column: column, value: issueModel.TitleBody);
                    case "StartTime": return hb.Td(column: column, value: issueModel.StartTime);
                    case "CompletionTime": return hb.Td(column: column, value: issueModel.CompletionTime);
                    case "WorkValue": return hb.Td(column: column, value: issueModel.WorkValue);
                    case "ProgressRate": return hb.Td(column: column, value: issueModel.ProgressRate);
                    case "RemainingWorkValue": return hb.Td(column: column, value: issueModel.RemainingWorkValue);
                    case "Status": return hb.Td(column: column, value: issueModel.Status);
                    case "Manager": return hb.Td(column: column, value: issueModel.Manager);
                    case "Owner": return hb.Td(column: column, value: issueModel.Owner);
                    case "ClassA": return hb.Td(column: column, value: issueModel.ClassA);
                    case "ClassB": return hb.Td(column: column, value: issueModel.ClassB);
                    case "ClassC": return hb.Td(column: column, value: issueModel.ClassC);
                    case "ClassD": return hb.Td(column: column, value: issueModel.ClassD);
                    case "ClassE": return hb.Td(column: column, value: issueModel.ClassE);
                    case "ClassF": return hb.Td(column: column, value: issueModel.ClassF);
                    case "ClassG": return hb.Td(column: column, value: issueModel.ClassG);
                    case "ClassH": return hb.Td(column: column, value: issueModel.ClassH);
                    case "ClassI": return hb.Td(column: column, value: issueModel.ClassI);
                    case "ClassJ": return hb.Td(column: column, value: issueModel.ClassJ);
                    case "ClassK": return hb.Td(column: column, value: issueModel.ClassK);
                    case "ClassL": return hb.Td(column: column, value: issueModel.ClassL);
                    case "ClassM": return hb.Td(column: column, value: issueModel.ClassM);
                    case "ClassN": return hb.Td(column: column, value: issueModel.ClassN);
                    case "ClassO": return hb.Td(column: column, value: issueModel.ClassO);
                    case "ClassP": return hb.Td(column: column, value: issueModel.ClassP);
                    case "ClassQ": return hb.Td(column: column, value: issueModel.ClassQ);
                    case "ClassR": return hb.Td(column: column, value: issueModel.ClassR);
                    case "ClassS": return hb.Td(column: column, value: issueModel.ClassS);
                    case "ClassT": return hb.Td(column: column, value: issueModel.ClassT);
                    case "ClassU": return hb.Td(column: column, value: issueModel.ClassU);
                    case "ClassV": return hb.Td(column: column, value: issueModel.ClassV);
                    case "ClassW": return hb.Td(column: column, value: issueModel.ClassW);
                    case "ClassX": return hb.Td(column: column, value: issueModel.ClassX);
                    case "ClassY": return hb.Td(column: column, value: issueModel.ClassY);
                    case "ClassZ": return hb.Td(column: column, value: issueModel.ClassZ);
                    case "NumA": return hb.Td(column: column, value: issueModel.NumA);
                    case "NumB": return hb.Td(column: column, value: issueModel.NumB);
                    case "NumC": return hb.Td(column: column, value: issueModel.NumC);
                    case "NumD": return hb.Td(column: column, value: issueModel.NumD);
                    case "NumE": return hb.Td(column: column, value: issueModel.NumE);
                    case "NumF": return hb.Td(column: column, value: issueModel.NumF);
                    case "NumG": return hb.Td(column: column, value: issueModel.NumG);
                    case "NumH": return hb.Td(column: column, value: issueModel.NumH);
                    case "NumI": return hb.Td(column: column, value: issueModel.NumI);
                    case "NumJ": return hb.Td(column: column, value: issueModel.NumJ);
                    case "NumK": return hb.Td(column: column, value: issueModel.NumK);
                    case "NumL": return hb.Td(column: column, value: issueModel.NumL);
                    case "NumM": return hb.Td(column: column, value: issueModel.NumM);
                    case "NumN": return hb.Td(column: column, value: issueModel.NumN);
                    case "NumO": return hb.Td(column: column, value: issueModel.NumO);
                    case "NumP": return hb.Td(column: column, value: issueModel.NumP);
                    case "NumQ": return hb.Td(column: column, value: issueModel.NumQ);
                    case "NumR": return hb.Td(column: column, value: issueModel.NumR);
                    case "NumS": return hb.Td(column: column, value: issueModel.NumS);
                    case "NumT": return hb.Td(column: column, value: issueModel.NumT);
                    case "NumU": return hb.Td(column: column, value: issueModel.NumU);
                    case "NumV": return hb.Td(column: column, value: issueModel.NumV);
                    case "NumW": return hb.Td(column: column, value: issueModel.NumW);
                    case "NumX": return hb.Td(column: column, value: issueModel.NumX);
                    case "NumY": return hb.Td(column: column, value: issueModel.NumY);
                    case "NumZ": return hb.Td(column: column, value: issueModel.NumZ);
                    case "DateA": return hb.Td(column: column, value: issueModel.DateA);
                    case "DateB": return hb.Td(column: column, value: issueModel.DateB);
                    case "DateC": return hb.Td(column: column, value: issueModel.DateC);
                    case "DateD": return hb.Td(column: column, value: issueModel.DateD);
                    case "DateE": return hb.Td(column: column, value: issueModel.DateE);
                    case "DateF": return hb.Td(column: column, value: issueModel.DateF);
                    case "DateG": return hb.Td(column: column, value: issueModel.DateG);
                    case "DateH": return hb.Td(column: column, value: issueModel.DateH);
                    case "DateI": return hb.Td(column: column, value: issueModel.DateI);
                    case "DateJ": return hb.Td(column: column, value: issueModel.DateJ);
                    case "DateK": return hb.Td(column: column, value: issueModel.DateK);
                    case "DateL": return hb.Td(column: column, value: issueModel.DateL);
                    case "DateM": return hb.Td(column: column, value: issueModel.DateM);
                    case "DateN": return hb.Td(column: column, value: issueModel.DateN);
                    case "DateO": return hb.Td(column: column, value: issueModel.DateO);
                    case "DateP": return hb.Td(column: column, value: issueModel.DateP);
                    case "DateQ": return hb.Td(column: column, value: issueModel.DateQ);
                    case "DateR": return hb.Td(column: column, value: issueModel.DateR);
                    case "DateS": return hb.Td(column: column, value: issueModel.DateS);
                    case "DateT": return hb.Td(column: column, value: issueModel.DateT);
                    case "DateU": return hb.Td(column: column, value: issueModel.DateU);
                    case "DateV": return hb.Td(column: column, value: issueModel.DateV);
                    case "DateW": return hb.Td(column: column, value: issueModel.DateW);
                    case "DateX": return hb.Td(column: column, value: issueModel.DateX);
                    case "DateY": return hb.Td(column: column, value: issueModel.DateY);
                    case "DateZ": return hb.Td(column: column, value: issueModel.DateZ);
                    case "DescriptionA": return hb.Td(column: column, value: issueModel.DescriptionA);
                    case "DescriptionB": return hb.Td(column: column, value: issueModel.DescriptionB);
                    case "DescriptionC": return hb.Td(column: column, value: issueModel.DescriptionC);
                    case "DescriptionD": return hb.Td(column: column, value: issueModel.DescriptionD);
                    case "DescriptionE": return hb.Td(column: column, value: issueModel.DescriptionE);
                    case "DescriptionF": return hb.Td(column: column, value: issueModel.DescriptionF);
                    case "DescriptionG": return hb.Td(column: column, value: issueModel.DescriptionG);
                    case "DescriptionH": return hb.Td(column: column, value: issueModel.DescriptionH);
                    case "DescriptionI": return hb.Td(column: column, value: issueModel.DescriptionI);
                    case "DescriptionJ": return hb.Td(column: column, value: issueModel.DescriptionJ);
                    case "DescriptionK": return hb.Td(column: column, value: issueModel.DescriptionK);
                    case "DescriptionL": return hb.Td(column: column, value: issueModel.DescriptionL);
                    case "DescriptionM": return hb.Td(column: column, value: issueModel.DescriptionM);
                    case "DescriptionN": return hb.Td(column: column, value: issueModel.DescriptionN);
                    case "DescriptionO": return hb.Td(column: column, value: issueModel.DescriptionO);
                    case "DescriptionP": return hb.Td(column: column, value: issueModel.DescriptionP);
                    case "DescriptionQ": return hb.Td(column: column, value: issueModel.DescriptionQ);
                    case "DescriptionR": return hb.Td(column: column, value: issueModel.DescriptionR);
                    case "DescriptionS": return hb.Td(column: column, value: issueModel.DescriptionS);
                    case "DescriptionT": return hb.Td(column: column, value: issueModel.DescriptionT);
                    case "DescriptionU": return hb.Td(column: column, value: issueModel.DescriptionU);
                    case "DescriptionV": return hb.Td(column: column, value: issueModel.DescriptionV);
                    case "DescriptionW": return hb.Td(column: column, value: issueModel.DescriptionW);
                    case "DescriptionX": return hb.Td(column: column, value: issueModel.DescriptionX);
                    case "DescriptionY": return hb.Td(column: column, value: issueModel.DescriptionY);
                    case "DescriptionZ": return hb.Td(column: column, value: issueModel.DescriptionZ);
                    case "CheckA": return hb.Td(column: column, value: issueModel.CheckA);
                    case "CheckB": return hb.Td(column: column, value: issueModel.CheckB);
                    case "CheckC": return hb.Td(column: column, value: issueModel.CheckC);
                    case "CheckD": return hb.Td(column: column, value: issueModel.CheckD);
                    case "CheckE": return hb.Td(column: column, value: issueModel.CheckE);
                    case "CheckF": return hb.Td(column: column, value: issueModel.CheckF);
                    case "CheckG": return hb.Td(column: column, value: issueModel.CheckG);
                    case "CheckH": return hb.Td(column: column, value: issueModel.CheckH);
                    case "CheckI": return hb.Td(column: column, value: issueModel.CheckI);
                    case "CheckJ": return hb.Td(column: column, value: issueModel.CheckJ);
                    case "CheckK": return hb.Td(column: column, value: issueModel.CheckK);
                    case "CheckL": return hb.Td(column: column, value: issueModel.CheckL);
                    case "CheckM": return hb.Td(column: column, value: issueModel.CheckM);
                    case "CheckN": return hb.Td(column: column, value: issueModel.CheckN);
                    case "CheckO": return hb.Td(column: column, value: issueModel.CheckO);
                    case "CheckP": return hb.Td(column: column, value: issueModel.CheckP);
                    case "CheckQ": return hb.Td(column: column, value: issueModel.CheckQ);
                    case "CheckR": return hb.Td(column: column, value: issueModel.CheckR);
                    case "CheckS": return hb.Td(column: column, value: issueModel.CheckS);
                    case "CheckT": return hb.Td(column: column, value: issueModel.CheckT);
                    case "CheckU": return hb.Td(column: column, value: issueModel.CheckU);
                    case "CheckV": return hb.Td(column: column, value: issueModel.CheckV);
                    case "CheckW": return hb.Td(column: column, value: issueModel.CheckW);
                    case "CheckX": return hb.Td(column: column, value: issueModel.CheckX);
                    case "CheckY": return hb.Td(column: column, value: issueModel.CheckY);
                    case "CheckZ": return hb.Td(column: column, value: issueModel.CheckZ);
                    case "Comments": return hb.Td(column: column, value: issueModel.Comments);
                    case "Creator": return hb.Td(column: column, value: issueModel.Creator);
                    case "Updator": return hb.Td(column: column, value: issueModel.Updator);
                    case "CreatedTime": return hb.Td(column: column, value: issueModel.CreatedTime);
                    default: return hb;
                }
            }
        }

        public static HtmlBuilder TdCustomValue(
            this HtmlBuilder hb, SiteSettings ss, string gridDesign, IssueModel issueModel)
        {
            ss.IncludedColumns(gridDesign).ForEach(column =>
            {
                var value = string.Empty;
                switch (column.ColumnName)
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
            return ss.CanCreate()
                ? Editor(ss, new IssueModel(ss, methodType: BaseModel.MethodTypes.New))
                : new HtmlBuilder().NotFoundTemplate().ToString();
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
            var hb = new HtmlBuilder();
            ss.SetColumnAccessControls(issueModel.Mine());
            return hb.Template(
                ss: ss,
                verType: issueModel.VerType,
                methodType: issueModel.MethodType,
                allowAccess: AllowAccess(ss, issueModel),
                siteId: issueModel.SiteId,
                referenceType: "Issues",
                title: issueModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.New()
                    : issueModel.Title.DisplayValue,
                useTitle: ss.EditorColumns.Contains("Title"),
                userScript: issueModel.MethodType == BaseModel.MethodTypes.New
                    ? ss.NewScript
                    : ss.EditScript,
                userStyle: issueModel.MethodType == BaseModel.MethodTypes.New
                    ? ss.NewStyle
                    : ss.EditStyle,
                action: () =>
                {
                    hb
                        .Editor(
                            ss: ss,
                            issueModel: issueModel)
                        .Hidden(controlId: "TableName", value: "Issues")
                        .Hidden(controlId: "Id", value: issueModel.IssueId.ToString());
                }).ToString();
        }

        private static bool AllowAccess(SiteSettings ss, IssueModel issueModel)
        {
            return
                ((issueModel.MethodType == BaseModel.MethodTypes.Edit && ss.CanRead()) ||
                (issueModel.MethodType == BaseModel.MethodTypes.New && ss.CanCreate())) &&
                issueModel.AccessStatus != Databases.AccessStatuses.NotFound;
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("IssueForm")
                        .Class("main-form")
                        .Action(Locations.ItemAction(issueModel.IssueId != 0 
                            ? issueModel.IssueId
                            : issueModel.SiteId)),
                    action: () => hb
                        .RecordHeader(
                            ss: ss,
                            baseModel: issueModel,
                            tableName: "Issues")
                        .Div(id: "EditorComments", action: () => hb
                            .Comments(
                                comments: issueModel.Comments,
                                verType: issueModel.VerType))
                        .Div(id: "EditorTabsContainer", action: () => hb
                            .EditorTabs(issueModel: issueModel, ss: ss)
                            .FieldSetGeneral(
                                ss: ss,
                                issueModel: issueModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: issueModel.MethodType != BaseModel.MethodTypes.New)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetPermissions")
                                    .DataAction("Permissions")
                                    .DataMethod("get"),
                                _using: ss.CanManagePermission() &&
                                    issueModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                ss: ss,
                                siteId: issueModel.SiteId,
                                verType: issueModel.VerType,
                                referenceType: "items",
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
                        text: Displays.Basic()))
                .Li(_using: issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.ChangeHistoryList()))
                .Li(_using: ss.CanManagePermission() &&
                        issueModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetPermissions",
                            text: Displays.ManagePermission())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var mine = issueModel.Mine();
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                ss.GetEditorColumns().ForEach(column =>
                {
                    switch (column.ColumnName)
                    {
                        case "IssueId":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.IssueId.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Ver":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Ver.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Title":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Title.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Body":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Body.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "StartTime":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.StartTime.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CompletionTime":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CompletionTime.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "WorkValue":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.WorkValue.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ProgressRate":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ProgressRate.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "RemainingWorkValue":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.RemainingWorkValue.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Status":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Status.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Manager":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Manager.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "Owner":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Owner.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassA.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassB.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassC.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassD.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassE.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassF.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassG.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassH.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassI.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassJ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassK.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassL.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassM.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassN.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassO.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassP.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassQ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassR.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassS.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassT.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassU.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassV.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassW.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassX.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassY.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "ClassZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassZ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumA.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumB.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumC.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumD.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumE.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumF.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumG.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumH.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumI.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumJ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumK.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumL.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumM.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumN.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumO.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumP.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumQ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumR.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumS.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumT.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumU.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumV.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumW.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumX.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumY.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "NumZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumZ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateA.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateB.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateC.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateD.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateE.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateF.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateG.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateH.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateI.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateJ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateK.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateL.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateM.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateN.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateO.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateP.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateQ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateR.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateS.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateT.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateU.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateV.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateW.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateX.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateY.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DateZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateZ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionA.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionB.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionC.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionD.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionE.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionF.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionG.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionH.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionI.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionJ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionK.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionL.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionM.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionN.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionO.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionP.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionQ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionR.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionS.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionT.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionU.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionV.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionW.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionX.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionY.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "DescriptionZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionZ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckA.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckB.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckC.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckD.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckE.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckF.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckG.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckH.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckI.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckJ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckK.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckL.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckM.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckN.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckO.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckP.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckQ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckR.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckS.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckT.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckU.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckV.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckW.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckX.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckY.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                        case "CheckZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckZ.ToControl(ss, column),
                                column.ColumnPermissionType(ss));
                            break;
                    }
                });
                hb.VerUpCheckBox(issueModel);
                hb
                    .Div(id: "LinkCreations", css: "links", action: () => hb
                        .LinkCreations(
                            ss: ss,
                            linkId: issueModel.IssueId,
                            methodType: issueModel.MethodType))
                    .Div(id: "Links", css: "links", action: () => hb
                        .Links(ss: ss, id: issueModel.IssueId));
            });
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
            var siteModel = new SiteModel(issueModel.SiteId);
            issueModel.MethodType = BaseModel.MethodTypes.Edit;
            return new IssuesResponseCollection(issueModel)
                .Invoke("clearDialogs")
                .ReplaceAll("#MainContainer", Editor(ss, issueModel))
                .Val("#SwitchTargets", switchTargets, _using: switchTargets != null)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        public static List<long> GetSwitchTargets(SiteSettings ss, long issueId, long siteId)
        {
            var view = Views.GetBySession(ss);
            var switchTargets = Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssueId(),
                    where: view.Where(
                        ss: ss, where: Rds.IssuesWhere().SiteId(siteId)),
                    orderBy: view.OrderBy(ss, Rds.IssuesOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc))))
                            .AsEnumerable()
                            .Select(o => o["IssueId"].ToLong())
                            .ToList();
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
                    switch (column.ColumnName)
                    {
                        case "WorkValue":
                            res.Val(
                                "#Issues_WorkValue",
                                issueModel.WorkValue.ToControl(ss, column));
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
                        default: break;
                    }
                });
            return res;
        }

        public static string Create(SiteSettings ss)
        {
            var issueModel = new IssueModel(ss, 0, setByForm: true);
            var invalid = IssueValidators.OnCreating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Create(ss, notice: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(
                    ss,
                    issueModel,
                    Messages.Created(issueModel.Title.Value),
                    GetSwitchTargets(
                        ss, issueModel.IssueId, issueModel.SiteId).Join())
                            .ToJson();
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
            if (error.Has())
            {
                return error == Error.Types.UpdateConflicts
                    ? Messages.ResponseUpdateConflicts(issueModel.Updator.FullName()).ToJson()
                    : new ResponseCollection().Message(error.Message()).ToJson();
            }
            else
            {
                var res = new IssuesResponseCollection(issueModel);
                res.Val(
                    "#Issues_RemainingWorkValue",
                    ss.GetColumn("RemainingWorkValue")
                        .Display(ss, issueModel.RemainingWorkValue));
                return ResponseByUpdate(res, ss, issueModel)
                    .PrependComment(issueModel.Comments, issueModel.VerType)
                    .ToJson();
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
                .Message(Messages.Updated(issueModel.Title.ToString()))
                .RemoveComment(issueModel.DeleteCommentId, _using: issueModel.DeleteCommentId != 0)
                .ClearFormData();
        }

        public static string Copy(SiteSettings ss, long issueId)
        {
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
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                issueModel.Comments.Clear();
            }
            var error = issueModel.Create(ss, paramAll: true);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
            return EditorResponse(
                ss,
                issueModel,
                Messages.Copied(issueModel.Title.Value),
                GetSwitchTargets(
                    ss, issueModel.IssueId, issueModel.SiteId).Join())
                        .ToJson();
            }
        }

        public static string Move(SiteSettings ss, long issueId)
        {
            var siteId = Forms.Long("MoveTargets");
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnMoving(ss, SiteSettingsUtilities.Get(siteId, issueId));
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = issueModel.Move(ss, siteId);
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(ss, issueModel)
                    .Message(Messages.Moved(issueModel.Title.Value))
                    .Val("#BackUrl", Locations.ItemIndex(siteId))
                    .ToJson();
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
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                Sessions.Set("Message", Messages.Deleted(issueModel.Title.Value).Html);
                var res = new IssuesResponseCollection(issueModel);
                res.Href(Locations.ItemIndex(issueModel.SiteId));
                return res.ToJson();
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
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                var res = new IssuesResponseCollection(issueModel);
                return res.ToJson();
            }
        }

        public static string Histories(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            var columns = ss.GetHistoryColumns();
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
                            columnCollection: columns,
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new IssueCollection(
                            ss: ss,
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

        public static string History(SiteSettings ss, long issueId)
        {
            var issueModel = new IssueModel(ss, issueId);
            issueModel.Get(
                ss, 
                where: Rds.IssuesWhere()
                    .IssueId(issueModel.IssueId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            issueModel.VerType =  Forms.Bool("Latest")
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
            var issueModel = new IssueModel(ss, issueId);
            var invalid = IssueValidators.OnUpdating(ss, issueModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var number = Forms.Int("SeparateNumber");
            if (number >= 2)
            {
                var idHash = new Dictionary<int, long> { { 1, issueModel.IssueId } };
                var ver = issueModel.Ver;
                var timestampHash = new Dictionary<int, string> { { 1, issueModel.Timestamp } };
                var comments = issueModel.Comments.ToJson();
                for (var index = 2; index <= number; index++)
                {
                    issueModel.IssueId = 0;
                    issueModel.Create(ss, paramAll: true);
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
                    issueModel.Update(ss, paramAll: true);
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
                    Rds.UpdateIssues(
                        where: Views.GetBySession(ss).Where(
                            ss, Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId_In(
                                    value: checkedItems,
                                    negative: negative,
                                    _using: checkedItems.Any())),
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
                ss, Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(
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
                                sub: Rds.SelectIssues(
                                    column: Rds.IssuesColumn().IssueId(),
                                    where: where))),
                    Rds.DeleteIssues(
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
            var ss = siteModel.IssuesSiteSettings(siteModel.SiteId);
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
                    .Select(o => o.ColumnName), "Title", "CompletionTime");
                if (error != null) return error;
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.IssuesParam();
                    param.IssueId(raw: Def.Sql.Identity);
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
                                case "StartTime": param.StartTime(recordingData, _using: recordingData != null); break;
                                case "CompletionTime": param.CompletionTime(recordingData, _using: recordingData != null); break;
                                case "WorkValue": param.WorkValue(recordingData, _using: recordingData != null); break;
                                case "ProgressRate": param.ProgressRate(recordingData, _using: recordingData != null); break;
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
                var errorTitle = Imports.Validate(
                    paramHash, ss.GetColumn("Title"));
                if (errorTitle != null) return errorTitle;
                var errorCompletionTime = Imports.Validate(
                    paramHash, ss.GetColumn("CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                paramHash.Values.ForEach(param =>
                    new IssueModel()
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static object ImportRecordingData(
            Column column, string value, long inheritPermission)
        {
            var recordingData = column.RecordingData(value, inheritPermission);
            switch (column.ColumnName)
            {
                case "CompletionTime":
                    recordingData = recordingData.ToDateTime().AddDays(1);
                    break;
            }
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
            var issueCollection = new IssueCollection(
                ss: ss,
                where: view.Where(
                    ss: ss, where: Rds.IssuesWhere().SiteId(siteModel.SiteId)),
                orderBy: view.OrderBy(ss, Rds.IssuesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)));
            var csv = new System.Text.StringBuilder();
            var exportColumns = (Sessions.PageSession(
                siteModel.Id, 
                "ExportSettings_ExportColumns").ToString().Deserialize<ExportColumns>());
            var columnHash = exportColumns.ColumnHash(
                siteModel.IssuesSiteSettings(siteModel.SiteId));
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
            issueCollection.ForEach(issueModel =>
            {
                var row = new List<string>();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            issueModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key])));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(IssueModel issueModel, string columnName, Column column)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId": value = issueModel.SiteId.ToExport(column); break;
                case "UpdatedTime": value = issueModel.UpdatedTime.ToExport(column); break;
                case "IssueId": value = issueModel.IssueId.ToExport(column); break;
                case "Ver": value = issueModel.Ver.ToExport(column); break;
                case "Title": value = issueModel.Title.ToExport(column); break;
                case "Body": value = issueModel.Body.ToExport(column); break;
                case "TitleBody": value = issueModel.TitleBody.ToExport(column); break;
                case "StartTime": value = issueModel.StartTime.ToExport(column); break;
                case "CompletionTime": value = issueModel.CompletionTime.ToExport(column); break;
                case "WorkValue": value = issueModel.WorkValue.ToExport(column); break;
                case "ProgressRate": value = issueModel.ProgressRate.ToExport(column); break;
                case "RemainingWorkValue": value = issueModel.RemainingWorkValue.ToExport(column); break;
                case "Status": value = issueModel.Status.ToExport(column); break;
                case "Manager": value = issueModel.Manager.ToExport(column); break;
                case "Owner": value = issueModel.Owner.ToExport(column); break;
                case "ClassA": value = issueModel.ClassA.ToExport(column); break;
                case "ClassB": value = issueModel.ClassB.ToExport(column); break;
                case "ClassC": value = issueModel.ClassC.ToExport(column); break;
                case "ClassD": value = issueModel.ClassD.ToExport(column); break;
                case "ClassE": value = issueModel.ClassE.ToExport(column); break;
                case "ClassF": value = issueModel.ClassF.ToExport(column); break;
                case "ClassG": value = issueModel.ClassG.ToExport(column); break;
                case "ClassH": value = issueModel.ClassH.ToExport(column); break;
                case "ClassI": value = issueModel.ClassI.ToExport(column); break;
                case "ClassJ": value = issueModel.ClassJ.ToExport(column); break;
                case "ClassK": value = issueModel.ClassK.ToExport(column); break;
                case "ClassL": value = issueModel.ClassL.ToExport(column); break;
                case "ClassM": value = issueModel.ClassM.ToExport(column); break;
                case "ClassN": value = issueModel.ClassN.ToExport(column); break;
                case "ClassO": value = issueModel.ClassO.ToExport(column); break;
                case "ClassP": value = issueModel.ClassP.ToExport(column); break;
                case "ClassQ": value = issueModel.ClassQ.ToExport(column); break;
                case "ClassR": value = issueModel.ClassR.ToExport(column); break;
                case "ClassS": value = issueModel.ClassS.ToExport(column); break;
                case "ClassT": value = issueModel.ClassT.ToExport(column); break;
                case "ClassU": value = issueModel.ClassU.ToExport(column); break;
                case "ClassV": value = issueModel.ClassV.ToExport(column); break;
                case "ClassW": value = issueModel.ClassW.ToExport(column); break;
                case "ClassX": value = issueModel.ClassX.ToExport(column); break;
                case "ClassY": value = issueModel.ClassY.ToExport(column); break;
                case "ClassZ": value = issueModel.ClassZ.ToExport(column); break;
                case "NumA": value = issueModel.NumA.ToExport(column); break;
                case "NumB": value = issueModel.NumB.ToExport(column); break;
                case "NumC": value = issueModel.NumC.ToExport(column); break;
                case "NumD": value = issueModel.NumD.ToExport(column); break;
                case "NumE": value = issueModel.NumE.ToExport(column); break;
                case "NumF": value = issueModel.NumF.ToExport(column); break;
                case "NumG": value = issueModel.NumG.ToExport(column); break;
                case "NumH": value = issueModel.NumH.ToExport(column); break;
                case "NumI": value = issueModel.NumI.ToExport(column); break;
                case "NumJ": value = issueModel.NumJ.ToExport(column); break;
                case "NumK": value = issueModel.NumK.ToExport(column); break;
                case "NumL": value = issueModel.NumL.ToExport(column); break;
                case "NumM": value = issueModel.NumM.ToExport(column); break;
                case "NumN": value = issueModel.NumN.ToExport(column); break;
                case "NumO": value = issueModel.NumO.ToExport(column); break;
                case "NumP": value = issueModel.NumP.ToExport(column); break;
                case "NumQ": value = issueModel.NumQ.ToExport(column); break;
                case "NumR": value = issueModel.NumR.ToExport(column); break;
                case "NumS": value = issueModel.NumS.ToExport(column); break;
                case "NumT": value = issueModel.NumT.ToExport(column); break;
                case "NumU": value = issueModel.NumU.ToExport(column); break;
                case "NumV": value = issueModel.NumV.ToExport(column); break;
                case "NumW": value = issueModel.NumW.ToExport(column); break;
                case "NumX": value = issueModel.NumX.ToExport(column); break;
                case "NumY": value = issueModel.NumY.ToExport(column); break;
                case "NumZ": value = issueModel.NumZ.ToExport(column); break;
                case "DateA": value = issueModel.DateA.ToExport(column); break;
                case "DateB": value = issueModel.DateB.ToExport(column); break;
                case "DateC": value = issueModel.DateC.ToExport(column); break;
                case "DateD": value = issueModel.DateD.ToExport(column); break;
                case "DateE": value = issueModel.DateE.ToExport(column); break;
                case "DateF": value = issueModel.DateF.ToExport(column); break;
                case "DateG": value = issueModel.DateG.ToExport(column); break;
                case "DateH": value = issueModel.DateH.ToExport(column); break;
                case "DateI": value = issueModel.DateI.ToExport(column); break;
                case "DateJ": value = issueModel.DateJ.ToExport(column); break;
                case "DateK": value = issueModel.DateK.ToExport(column); break;
                case "DateL": value = issueModel.DateL.ToExport(column); break;
                case "DateM": value = issueModel.DateM.ToExport(column); break;
                case "DateN": value = issueModel.DateN.ToExport(column); break;
                case "DateO": value = issueModel.DateO.ToExport(column); break;
                case "DateP": value = issueModel.DateP.ToExport(column); break;
                case "DateQ": value = issueModel.DateQ.ToExport(column); break;
                case "DateR": value = issueModel.DateR.ToExport(column); break;
                case "DateS": value = issueModel.DateS.ToExport(column); break;
                case "DateT": value = issueModel.DateT.ToExport(column); break;
                case "DateU": value = issueModel.DateU.ToExport(column); break;
                case "DateV": value = issueModel.DateV.ToExport(column); break;
                case "DateW": value = issueModel.DateW.ToExport(column); break;
                case "DateX": value = issueModel.DateX.ToExport(column); break;
                case "DateY": value = issueModel.DateY.ToExport(column); break;
                case "DateZ": value = issueModel.DateZ.ToExport(column); break;
                case "DescriptionA": value = issueModel.DescriptionA.ToExport(column); break;
                case "DescriptionB": value = issueModel.DescriptionB.ToExport(column); break;
                case "DescriptionC": value = issueModel.DescriptionC.ToExport(column); break;
                case "DescriptionD": value = issueModel.DescriptionD.ToExport(column); break;
                case "DescriptionE": value = issueModel.DescriptionE.ToExport(column); break;
                case "DescriptionF": value = issueModel.DescriptionF.ToExport(column); break;
                case "DescriptionG": value = issueModel.DescriptionG.ToExport(column); break;
                case "DescriptionH": value = issueModel.DescriptionH.ToExport(column); break;
                case "DescriptionI": value = issueModel.DescriptionI.ToExport(column); break;
                case "DescriptionJ": value = issueModel.DescriptionJ.ToExport(column); break;
                case "DescriptionK": value = issueModel.DescriptionK.ToExport(column); break;
                case "DescriptionL": value = issueModel.DescriptionL.ToExport(column); break;
                case "DescriptionM": value = issueModel.DescriptionM.ToExport(column); break;
                case "DescriptionN": value = issueModel.DescriptionN.ToExport(column); break;
                case "DescriptionO": value = issueModel.DescriptionO.ToExport(column); break;
                case "DescriptionP": value = issueModel.DescriptionP.ToExport(column); break;
                case "DescriptionQ": value = issueModel.DescriptionQ.ToExport(column); break;
                case "DescriptionR": value = issueModel.DescriptionR.ToExport(column); break;
                case "DescriptionS": value = issueModel.DescriptionS.ToExport(column); break;
                case "DescriptionT": value = issueModel.DescriptionT.ToExport(column); break;
                case "DescriptionU": value = issueModel.DescriptionU.ToExport(column); break;
                case "DescriptionV": value = issueModel.DescriptionV.ToExport(column); break;
                case "DescriptionW": value = issueModel.DescriptionW.ToExport(column); break;
                case "DescriptionX": value = issueModel.DescriptionX.ToExport(column); break;
                case "DescriptionY": value = issueModel.DescriptionY.ToExport(column); break;
                case "DescriptionZ": value = issueModel.DescriptionZ.ToExport(column); break;
                case "CheckA": value = issueModel.CheckA.ToExport(column); break;
                case "CheckB": value = issueModel.CheckB.ToExport(column); break;
                case "CheckC": value = issueModel.CheckC.ToExport(column); break;
                case "CheckD": value = issueModel.CheckD.ToExport(column); break;
                case "CheckE": value = issueModel.CheckE.ToExport(column); break;
                case "CheckF": value = issueModel.CheckF.ToExport(column); break;
                case "CheckG": value = issueModel.CheckG.ToExport(column); break;
                case "CheckH": value = issueModel.CheckH.ToExport(column); break;
                case "CheckI": value = issueModel.CheckI.ToExport(column); break;
                case "CheckJ": value = issueModel.CheckJ.ToExport(column); break;
                case "CheckK": value = issueModel.CheckK.ToExport(column); break;
                case "CheckL": value = issueModel.CheckL.ToExport(column); break;
                case "CheckM": value = issueModel.CheckM.ToExport(column); break;
                case "CheckN": value = issueModel.CheckN.ToExport(column); break;
                case "CheckO": value = issueModel.CheckO.ToExport(column); break;
                case "CheckP": value = issueModel.CheckP.ToExport(column); break;
                case "CheckQ": value = issueModel.CheckQ.ToExport(column); break;
                case "CheckR": value = issueModel.CheckR.ToExport(column); break;
                case "CheckS": value = issueModel.CheckS.ToExport(column); break;
                case "CheckT": value = issueModel.CheckT.ToExport(column); break;
                case "CheckU": value = issueModel.CheckU.ToExport(column); break;
                case "CheckV": value = issueModel.CheckV.ToExport(column); break;
                case "CheckW": value = issueModel.CheckW.ToExport(column); break;
                case "CheckX": value = issueModel.CheckX.ToExport(column); break;
                case "CheckY": value = issueModel.CheckY.ToExport(column); break;
                case "CheckZ": value = issueModel.CheckZ.ToExport(column); break;
                case "Comments": value = issueModel.Comments.ToExport(column); break;
                case "Creator": value = issueModel.Creator.ToExport(column); break;
                case "Updator": value = issueModel.Updator.ToExport(column); break;
                case "CreatedTime": value = issueModel.CreatedTime.ToExport(column); break;
                default: return string.Empty;
            }
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string TitleDisplayValue(SiteSettings ss, IssueModel issueModel)
        {
            var displayValue = ss.GetTitleColumns()
                .Select(column => TitleDisplayValue(column, issueModel))
                .Where(o => o != string.Empty)
                .Join(ss.TitleSeparator);
            return displayValue != string.Empty
                ? displayValue
                : Displays.NoTitle();
        }

        private static string TitleDisplayValue(Column column, IssueModel issueModel)
        {
            switch (column.ColumnName)
            {
                case "Title": return column.HasChoices()
                    ? column.Choice(issueModel.Title.Value).Text
                    : issueModel.Title.Value;
                case "ClassA": return column.HasChoices()
                    ? column.Choice(issueModel.ClassA).Text
                    : issueModel.ClassA;
                case "ClassB": return column.HasChoices()
                    ? column.Choice(issueModel.ClassB).Text
                    : issueModel.ClassB;
                case "ClassC": return column.HasChoices()
                    ? column.Choice(issueModel.ClassC).Text
                    : issueModel.ClassC;
                case "ClassD": return column.HasChoices()
                    ? column.Choice(issueModel.ClassD).Text
                    : issueModel.ClassD;
                case "ClassE": return column.HasChoices()
                    ? column.Choice(issueModel.ClassE).Text
                    : issueModel.ClassE;
                case "ClassF": return column.HasChoices()
                    ? column.Choice(issueModel.ClassF).Text
                    : issueModel.ClassF;
                case "ClassG": return column.HasChoices()
                    ? column.Choice(issueModel.ClassG).Text
                    : issueModel.ClassG;
                case "ClassH": return column.HasChoices()
                    ? column.Choice(issueModel.ClassH).Text
                    : issueModel.ClassH;
                case "ClassI": return column.HasChoices()
                    ? column.Choice(issueModel.ClassI).Text
                    : issueModel.ClassI;
                case "ClassJ": return column.HasChoices()
                    ? column.Choice(issueModel.ClassJ).Text
                    : issueModel.ClassJ;
                case "ClassK": return column.HasChoices()
                    ? column.Choice(issueModel.ClassK).Text
                    : issueModel.ClassK;
                case "ClassL": return column.HasChoices()
                    ? column.Choice(issueModel.ClassL).Text
                    : issueModel.ClassL;
                case "ClassM": return column.HasChoices()
                    ? column.Choice(issueModel.ClassM).Text
                    : issueModel.ClassM;
                case "ClassN": return column.HasChoices()
                    ? column.Choice(issueModel.ClassN).Text
                    : issueModel.ClassN;
                case "ClassO": return column.HasChoices()
                    ? column.Choice(issueModel.ClassO).Text
                    : issueModel.ClassO;
                case "ClassP": return column.HasChoices()
                    ? column.Choice(issueModel.ClassP).Text
                    : issueModel.ClassP;
                case "ClassQ": return column.HasChoices()
                    ? column.Choice(issueModel.ClassQ).Text
                    : issueModel.ClassQ;
                case "ClassR": return column.HasChoices()
                    ? column.Choice(issueModel.ClassR).Text
                    : issueModel.ClassR;
                case "ClassS": return column.HasChoices()
                    ? column.Choice(issueModel.ClassS).Text
                    : issueModel.ClassS;
                case "ClassT": return column.HasChoices()
                    ? column.Choice(issueModel.ClassT).Text
                    : issueModel.ClassT;
                case "ClassU": return column.HasChoices()
                    ? column.Choice(issueModel.ClassU).Text
                    : issueModel.ClassU;
                case "ClassV": return column.HasChoices()
                    ? column.Choice(issueModel.ClassV).Text
                    : issueModel.ClassV;
                case "ClassW": return column.HasChoices()
                    ? column.Choice(issueModel.ClassW).Text
                    : issueModel.ClassW;
                case "ClassX": return column.HasChoices()
                    ? column.Choice(issueModel.ClassX).Text
                    : issueModel.ClassX;
                case "ClassY": return column.HasChoices()
                    ? column.Choice(issueModel.ClassY).Text
                    : issueModel.ClassY;
                case "ClassZ": return column.HasChoices()
                    ? column.Choice(issueModel.ClassZ).Text
                    : issueModel.ClassZ;
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
            var issueModel = new IssueModel(
                ss, Forms.Long("KambanId"), setByForm: true);
            issueModel.VerUp = Versions.MustVerUp(issueModel);
            issueModel.Update(ss, notice: true);
            return KambanJson(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Gantt(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.Gantt(
                    ss: ss,
                    view: view,
                    bodyOnly: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string GanttJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Gantt");
            return new ResponseCollection()
                .Html(
                    !bodyOnly ? "#ViewModeContainer" : "#GanttBody",
                    new HtmlBuilder().Gantt(
                        ss: ss,
                        view: view,
                        bodyOnly: bodyOnly))
                .View(ss: ss, view: view)
                .ReplaceAll(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: issueCollection.Aggregations))
                .Invoke("drawGantt")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            bool bodyOnly)
        {
            var forms = Forms.All();
            var groupBy = !view.GanttGroupBy.IsNullOrEmpty()
                ? view.GanttGroupBy
                : string.Empty;
            var dataRows = GanttDataRows(ss, view, groupBy);
            return !bodyOnly
                ? hb.Gantt(
                    ss: ss,
                    groupBy: groupBy,
                    dataRows: dataRows)
                : hb.GanttBody(
                    ss: ss,
                    groupBy: groupBy,
                    dataRows: dataRows);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> GanttDataRows(
            SiteSettings ss, View view, string groupBy)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(ss)
                        .IssueId(_as: "Id")
                        .Title()
                        .WorkValue()
                        .StartTime()
                        .CompletionTime()
                        .ProgressRate()
                        .Status()
                        .Owner()
                        .Updator()
                        .CreatedTime()
                        .UpdatedTime()
                        .IssuesColumn(groupBy, _as: "GroupBy"),
                    where: view.Where(ss, Rds.IssuesWhere().SiteId(ss.SiteId))))
                        .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDown(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () => hb.BurnDown(
                    ss: ss,
                    dataRows: BurnDownDataRows(
                        ss: ss,
                        view: view),
                    ownerLabelText: ss.GetColumn("Owner").GridLabelText,
                    column: ss.GetColumn("WorkValue")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDownJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            return new ResponseCollection()
                .Html(
                    "#ViewModeContainer",
                    new HtmlBuilder().BurnDown(
                        ss: ss,
                        dataRows: BurnDownDataRows(ss, view),
                        ownerLabelText: ss.GetColumn("Owner").GridLabelText,
                        column: ss.GetColumn("WorkValue")))
                .View(ss: ss, view: view)
                .ReplaceAll(
                    "#Aggregations", new HtmlBuilder().Aggregations(
                    ss: ss,
                    aggregations: issueCollection.Aggregations))
                .Invoke("drawBurnDown")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> BurnDownDataRows(
            SiteSettings ss, View view)
        {
            var where = view.Where(ss, Rds.IssuesWhere().SiteId(ss.SiteId));
            return Rds.ExecuteTable(statements: new SqlStatement[]
            {
                Rds.SelectIssues(
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
                    where: Rds.IssuesWhere()
                        .IssueId_In(sub: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssueId(),
                            where: where)),
                    orderBy: Rds.IssuesOrderBy()
                        .IssueId()
                        .Ver())
            }).AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string TimeSeries(SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
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
        public static string TimeSeriesJson(SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
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
                    aggregations: issueCollection.Aggregations))
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
                    Rds.SelectIssues(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.IssuesColumn()
                            .IssueId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .IssuesColumn(groupBy, _as: "Index")
                            .IssuesColumn(value, _as: "Value"),
                        where: view.Where(ss, Rds.IssuesWhere().SiteId(ss.SiteId))))
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
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
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
        public static string KambanJson(
            SiteSettings ss)
        {
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
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
                    aggregations: issueCollection.Aggregations))
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
                : "RemainingWorkValue";
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
        private static Rds.IssuesColumnCollection KambanColumns(
            SiteSettings ss, string groupByX, string groupByY, string value)
        {
            var column = Rds.IssuesColumn()
                .IssueId()
                .StartTime()
                .CompletionTime()
                .WorkValue()
                .ProgressRate()
                .RemainingWorkValue()
                .Manager()
                .Owner();
            ss.GetTitleColumns().ForEach(titleColumn =>
                column.IssuesColumn(titleColumn.ColumnName));
            return column
                .IssuesColumn(groupByX)
                .IssuesColumn(groupByY)
                .IssuesColumn(value);
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
            Rds.IssuesColumnCollection column)
        {
            return new IssueCollection(
                ss: ss,
                column: column,
                where: view.Where(ss, Rds.IssuesWhere().SiteId(ss.SiteId)),
                orderBy: view.OrderBy(ss, Rds.IssuesOrderBy()
                    .UpdatedTime(SqlOrderBy.Types.desc)))
                        .Select(o => new Libraries.ViewModes.KambanElement()
                        {
                            Id = o.Id,
                            Title = o.Title.DisplayValue,
                            StartTime = o.StartTime,
                            CompletionTime = o.CompletionTime,
                            WorkValue = o.WorkValue,
                            ProgressRate = o.ProgressRate,
                            RemainingWorkValue = o.RemainingWorkValue,
                            Manager = o.Manager,
                            Owner = o.Owner,
                            GroupX = o.PropertyValue(groupByX),
                            GroupY = o.PropertyValue(groupByY),
                            Value = o.PropertyValue(value).ToDecimal()
                        });
        }
    }
}
