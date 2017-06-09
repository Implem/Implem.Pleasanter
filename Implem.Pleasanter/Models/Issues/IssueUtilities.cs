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
            var invalid = IssueValidators.OnEntry(ss);
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
                .Paging("#Grid")
                .ToJson();
        }

        private static IssueCollection IssueCollection(
            SiteSettings ss, View view, int offset = 0)
        {
            return new IssueCollection(
                ss: ss,
                column: GridSqlColumnCollection(ss),
                where: view.Where(ss: ss),
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
                .Paging("#Grid")
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
            var columns = ss.GetGridColumns(checkPermission: true);
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: issueCollection
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issueCollection.ForEach(issueModel =>
                    issueModel.Title = new Title(
                        ss,
                        issueModel.IssueId,
                        issueModel.PropertyValues(ss.TitleColumns)));
            }
            return hb
                .THead(
                    _using: addHeader,
                    action: () => hb
                        .GridHeader(
                            columns: columns, 
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
                var mine = issueModel.Mine();
                switch (column.ColumnName)
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
                referenceType: "Issues",
                title: issueModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.New()
                    : issueModel.Title.DisplayValue,
                useTitle: ss.TitleColumns?.Any(o => ss.EditorColumns.Contains(o)) == true,
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

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            IssueModel issueModel)
        {
            var commentsColumn = ss.GetColumn("Comments");
            var commentsColumnPermissionType = commentsColumn.ColumnPermissionType();
            var showComments = ss.EditorColumns?.Contains("Comments") == true &&
                commentsColumnPermissionType != Permissions.ColumnPermissionTypes.Deny;
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
                            href: "#FieldSetRecordAccessControl",
                            text: Displays.RecordAccessControl())));
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
                                column.ColumnPermissionType());
                            break;
                        case "Ver":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Ver.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Title":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Title.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Body":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Body.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "StartTime":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.StartTime.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CompletionTime":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CompletionTime.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "WorkValue":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.WorkValue.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ProgressRate":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ProgressRate.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "RemainingWorkValue":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.RemainingWorkValue.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Status":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Status.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Manager":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Manager.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "Owner":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.Owner.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "ClassZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.ClassZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "NumZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.NumZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DateZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DateZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "DescriptionZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.DescriptionZ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckA":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckA.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckB":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckB.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckC":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckC.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckD":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckD.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckE":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckE.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckF":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckF.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckG":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckG.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckH":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckH.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckI":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckI.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckJ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckJ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckK":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckK.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckL":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckL.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckM":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckM.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckN":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckN.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckO":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckO.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckP":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckP.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckQ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckQ.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckR":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckR.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckS":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckS.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckT":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckT.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckU":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckU.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckV":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckV.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckW":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckW.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckX":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckX.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckY":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckY.ToControl(ss, column),
                                column.ColumnPermissionType());
                            break;
                        case "CheckZ":
                            hb.Field(
                                ss,
                                column,
                                issueModel.MethodType,
                                issueModel.CheckZ.ToControl(ss, column),
                                column.ColumnPermissionType());
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
                .SetMemory("formChanged", false)
                .Invoke("setCurrentIndex")
                .Message(message)
                .ClearFormData();
        }

        private static List<long> GetSwitchTargets(SiteSettings ss, long issueId, long siteId)
        {
            var view = Views.GetBySession(ss);
            var where = view.Where(ss: ss);
            var switchTargets = Rds.ExecuteScalar_int(statements:
                Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: where)) <= Parameters.General.SwitchTargetsLimit
                        ? Rds.ExecuteTable(statements: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssueId(),
                            where: where,
                            orderBy: view.OrderBy(ss, Rds.IssuesOrderBy()
                                .UpdatedTime(SqlOrderBy.Types.desc))))
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
            if (error.Has())
            {
                return error.MessageJson();
            }
            else
            {
                return EditorResponse(
                    ss,
                    issueModel,
                    Messages.Created(issueModel.Title.DisplayValue),
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
                    ? Messages.ResponseUpdateConflicts(issueModel.Updator.Name).ToJson()
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
                .SetMemory("formChanged", false)
                .Message(Messages.Updated(issueModel.Title.DisplayValue))
                .RemoveComment(issueModel.DeleteCommentId, _using: issueModel.DeleteCommentId != 0)
                .ClearFormData();
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
            if (!Forms.Data("CopyWithComments").ToBool())
            {
                issueModel.Comments.Clear();
            }
            var error = issueModel.Create(ss, forceSynchronizeSourceSummary: true, paramAll: true);
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
                res
                    .SetMemory("formChanged", false)
                    .Href(Locations.ItemIndex(issueModel.SiteId));
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
            ss.SetColumnAccessControls(issueModel.Mine());
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
                    issueModel.Update(ss, forceSynchronizeSourceSummary: true, paramAll: true);
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
                Rds.SelectIssues(
                    column: Rds.IssuesColumn().IssuesCount(),
                    where: Views.GetBySession(ss).Where(
                        ss, Rds.IssuesWhere()
                            .SiteId(ss.SiteId)
                            .IssueId_In(
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
                    Rds.UpdateIssues(
                        where: Views.GetBySession(ss).Where(
                            ss, Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId_In(
                                    value: selected,
                                    negative: all,
                                    _using: selected.Any())),
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
            if (!Contract.Import()) return null;
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
                    .Select(o => o.ColumnName), "Title", "CompletionTime");
                if (error != null) return error;
                var issueHash = new Dictionary<int, IssueModel>();
                var paramHash = new Dictionary<int, SqlParamCollection>();
                csv.Rows.Select((o, i) => new { Row = o, Index = i }).ForEach(data =>
                {
                    var param = Rds.IssuesParam();
                    param.SiteId(ss.SiteId);
                    columnHash.ForEach(column =>
                    {
                        var recordingData = ImportRecordingData(
                            column.Value, data.Row[column.Key], ss.InheritPermission);
                        if (column.Value.ColumnName == "IssueId")
                        {
                            var issueId = recordingData.ToLong();
                            if (Forms.Bool("UpdatableImport") && issueId > 0)
                            {
                                var issueModel = new IssueModel().Get(ss, where: Rds.IssuesWhere()
                                    .SiteId(ss.SiteId)
                                    .IssueId(issueId));
                                if (issueModel.AccessStatus == Databases.AccessStatuses.Selected)
                                {
                                    issueHash.Add(data.Index, issueModel);
                                }
                            }
                        }
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
                    if (!issueHash.ContainsKey(data.Index))
                    {
                        param.IssueId(raw: Def.Sql.Identity);
                    }
                    paramHash.Add(data.Index, param);
                });
                var errorTitle = Imports.Validate(
                    paramHash, ss.GetColumn("Title"));
                if (errorTitle != null) return errorTitle;
                var errorCompletionTime = Imports.Validate(
                    paramHash, ss.GetColumn("CompletionTime"));
                if (errorCompletionTime != null) return errorCompletionTime;
                var insertCount = 0;
                var updateCount = 0;
                paramHash.ForEach(data =>
                {
                    if (issueHash.ContainsKey(data.Key))
                    {
                        var issueModel = issueHash[data.Key];
                        issueModel.VerUp = Versions.MustVerUp(issueModel);
                        issueModel.Update(ss, param: data.Value);
                        updateCount++;
                    }
                    else
                    {
                        new IssueModel()
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
            if (!Contract.Export()) return null;
            var invalid = IssueValidators.OnExporting(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return null;
            }
            var view = Views.GetBySession(ss);
            var issueCollection = new IssueCollection(
                ss: ss,
                where: view.Where(ss: ss),
                orderBy: view.OrderBy(ss, Rds.IssuesOrderBy()
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
                var mine = issueModel.Mine();
                exportColumns
                    .Columns
                    .Where(o => o.Value)
                    .ForEach(exportColumn =>
                        row.Add(CsvColumn(
                            ss,
                            issueModel, 
                            exportColumn.Key, 
                            columnHash[exportColumn.Key],
                            mine)));
                csv.Append(row.Join(","), "\n");
            });
            return new ResponseFile(csv.ToString(), ResponseFileNames.Csv(siteModel));
        }

        private static string CsvColumn(
            SiteSettings ss,
            IssueModel issueModel,
            string columnName,
            Column column,
            List<string> mine)
        {
            var value = string.Empty;
            switch (columnName)
            {
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.SiteId.ToExport(column)
                        : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.UpdatedTime.ToExport(column)
                        : string.Empty;
                    break;
                case "IssueId":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.IssueId.ToExport(column)
                        : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Ver.ToExport(column)
                        : string.Empty;
                    break;
                case "Title":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Title.ToExport(column)
                        : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Body.ToExport(column)
                        : string.Empty;
                    break;
                case "TitleBody":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.TitleBody.ToExport(column)
                        : string.Empty;
                    break;
                case "StartTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.StartTime.ToExport(column)
                        : string.Empty;
                    break;
                case "CompletionTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CompletionTime.ToExport(column)
                        : string.Empty;
                    break;
                case "WorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.WorkValue.ToExport(column)
                        : string.Empty;
                    break;
                case "ProgressRate":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ProgressRate.ToExport(column)
                        : string.Empty;
                    break;
                case "RemainingWorkValue":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.RemainingWorkValue.ToExport(column)
                        : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Status.ToExport(column)
                        : string.Empty;
                    break;
                case "Manager":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Manager.ToExport(column)
                        : string.Empty;
                    break;
                case "Owner":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Owner.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassA.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassB.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassC.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassD.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassE.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassF.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassG.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassH.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassI.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassJ.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassK.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassL.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassM.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassN.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassO.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassP.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassQ.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassR.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassS.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassT.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassU.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassV.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassW.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassX.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassY.ToExport(column)
                        : string.Empty;
                    break;
                case "ClassZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.ClassZ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumA.ToExport(column)
                        : string.Empty;
                    break;
                case "NumB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumB.ToExport(column)
                        : string.Empty;
                    break;
                case "NumC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumC.ToExport(column)
                        : string.Empty;
                    break;
                case "NumD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumD.ToExport(column)
                        : string.Empty;
                    break;
                case "NumE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumE.ToExport(column)
                        : string.Empty;
                    break;
                case "NumF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumF.ToExport(column)
                        : string.Empty;
                    break;
                case "NumG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumG.ToExport(column)
                        : string.Empty;
                    break;
                case "NumH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumH.ToExport(column)
                        : string.Empty;
                    break;
                case "NumI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumI.ToExport(column)
                        : string.Empty;
                    break;
                case "NumJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumJ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumK.ToExport(column)
                        : string.Empty;
                    break;
                case "NumL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumL.ToExport(column)
                        : string.Empty;
                    break;
                case "NumM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumM.ToExport(column)
                        : string.Empty;
                    break;
                case "NumN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumN.ToExport(column)
                        : string.Empty;
                    break;
                case "NumO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumO.ToExport(column)
                        : string.Empty;
                    break;
                case "NumP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumP.ToExport(column)
                        : string.Empty;
                    break;
                case "NumQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumQ.ToExport(column)
                        : string.Empty;
                    break;
                case "NumR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumR.ToExport(column)
                        : string.Empty;
                    break;
                case "NumS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumS.ToExport(column)
                        : string.Empty;
                    break;
                case "NumT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumT.ToExport(column)
                        : string.Empty;
                    break;
                case "NumU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumU.ToExport(column)
                        : string.Empty;
                    break;
                case "NumV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumV.ToExport(column)
                        : string.Empty;
                    break;
                case "NumW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumW.ToExport(column)
                        : string.Empty;
                    break;
                case "NumX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumX.ToExport(column)
                        : string.Empty;
                    break;
                case "NumY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumY.ToExport(column)
                        : string.Empty;
                    break;
                case "NumZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.NumZ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateA.ToExport(column)
                        : string.Empty;
                    break;
                case "DateB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateB.ToExport(column)
                        : string.Empty;
                    break;
                case "DateC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateC.ToExport(column)
                        : string.Empty;
                    break;
                case "DateD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateD.ToExport(column)
                        : string.Empty;
                    break;
                case "DateE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateE.ToExport(column)
                        : string.Empty;
                    break;
                case "DateF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateF.ToExport(column)
                        : string.Empty;
                    break;
                case "DateG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateG.ToExport(column)
                        : string.Empty;
                    break;
                case "DateH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateH.ToExport(column)
                        : string.Empty;
                    break;
                case "DateI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateI.ToExport(column)
                        : string.Empty;
                    break;
                case "DateJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateJ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateK.ToExport(column)
                        : string.Empty;
                    break;
                case "DateL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateL.ToExport(column)
                        : string.Empty;
                    break;
                case "DateM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateM.ToExport(column)
                        : string.Empty;
                    break;
                case "DateN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateN.ToExport(column)
                        : string.Empty;
                    break;
                case "DateO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateO.ToExport(column)
                        : string.Empty;
                    break;
                case "DateP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateP.ToExport(column)
                        : string.Empty;
                    break;
                case "DateQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateQ.ToExport(column)
                        : string.Empty;
                    break;
                case "DateR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateR.ToExport(column)
                        : string.Empty;
                    break;
                case "DateS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateS.ToExport(column)
                        : string.Empty;
                    break;
                case "DateT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateT.ToExport(column)
                        : string.Empty;
                    break;
                case "DateU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateU.ToExport(column)
                        : string.Empty;
                    break;
                case "DateV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateV.ToExport(column)
                        : string.Empty;
                    break;
                case "DateW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateW.ToExport(column)
                        : string.Empty;
                    break;
                case "DateX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateX.ToExport(column)
                        : string.Empty;
                    break;
                case "DateY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateY.ToExport(column)
                        : string.Empty;
                    break;
                case "DateZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DateZ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionA.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionB.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionC.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionD.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionE.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionF.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionG.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionH.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionI.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionJ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionK.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionL.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionM.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionN.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionO.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionP.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionQ.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionR.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionS.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionT.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionU.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionV.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionW.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionX.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionY.ToExport(column)
                        : string.Empty;
                    break;
                case "DescriptionZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.DescriptionZ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckA":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckA.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckB":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckB.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckC":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckC.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckD":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckD.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckE":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckE.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckF":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckF.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckG":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckG.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckH":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckH.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckI":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckI.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckJ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckJ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckK":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckK.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckL":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckL.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckM":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckM.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckN":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckN.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckO":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckO.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckP":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckP.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckQ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckQ.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckR":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckR.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckS":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckS.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckT":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckT.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckU":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckU.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckV":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckV.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckW":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckW.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckX":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckX.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckY":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckY.ToExport(column)
                        : string.Empty;
                    break;
                case "CheckZ":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CheckZ.ToExport(column)
                        : string.Empty;
                    break;
                case "SiteTitle":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.SiteTitle.ToExport(column)
                        : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Comments.ToExport(column)
                        : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Creator.ToExport(column)
                        : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.Updator.ToExport(column)
                        : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(column, ss.PermissionType, mine)
                        ? issueModel.CreatedTime.ToExport(column)
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
            var issueCollection = IssueCollection(ss, view);
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
                issueCollection: issueCollection,
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
            var issueModel = new IssueModel(ss, Forms.Long("CalendarId"), setByForm: true);
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
            return CalendarJson(ss);
        }

        public static string CalendarJson(SiteSettings ss)
        {
            if (ss.EnableCalendar != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
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
                        aggregations: issueCollection.Aggregations))
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
                        aggregations: issueCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.CalendarLimit.ToString()))
                    .ToJson();
        }

        private static EnumerableRowCollection<DataRow> CalendarDataRows(
            SiteSettings ss, View view, string columnName, DateTime begin, DateTime end)
        {
            var where = Rds.IssuesWhere();
            switch (columnName)
            {
                case "UpdatedTime": 
                    where.UpdatedTime_Between(begin, end);
                    break;
                case "StartTime": 
                    where.StartTime_Between(begin, end);
                    break;
                case "CompletionTime": 
                    where.CompletionTime_Between(begin, end);
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
                Rds.SelectIssues(
                    column: Rds.IssuesTitleColumn(ss)
                        .IssueId(_as: "Id")
                        .Title()
                        .IssuesColumn(columnName, _as: "Date"),
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
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = view.GetCrosstabGroupByX(ss);
            var groupByY = view.GetCrosstabGroupByY(ss);
            var columns = view.CrosstabColumns;
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = view.GetCrosstabValue(ss);
            if (value.IsNullOrEmpty())
            {
                value = "IssueId";
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
                issueCollection: issueCollection,
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
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupByX = view.GetCrosstabGroupByX(ss);
            var groupByY = view.GetCrosstabGroupByY(ss);
            var columns = view.CrosstabColumns;
            var aggregateType = view.GetCrosstabAggregateType(ss);
            var value = view.GetCrosstabValue(ss);
            if (value.IsNullOrEmpty())
            {
                value = "IssueId";
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
                        aggregations: issueCollection.Aggregations))
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
                        aggregations: issueCollection.Aggregations))
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
                    Rds.SelectIssues(
                        column: Rds.IssuesColumn()
                            .IssuesColumn(groupByX, _as: "GroupByX")
                            .CrosstabColumns(
                                ss,
                                groupByY,
                                columns.Deserialize<IEnumerable<string>>(),
                                value,
                                aggregateType),
                        where: view.Where(ss: ss),
                        groupBy: Rds.IssuesGroupBy()
                            .IssuesGroupBy(groupByX)
                            .IssuesGroupBy(groupByY, _using: groupByY != "Columns")))
                                .AsEnumerable();
            }
            else
            {
                var dateGroup = Libraries.ViewModes.CrosstabUtilities.DateGroup(
                    ss, column, timePeriod);
                return Rds.ExecuteTable(statements:
                    Rds.SelectIssues(
                        column: Rds.IssuesColumn()
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
                        groupBy: Rds.IssuesGroupBy()
                            .Add(dateGroup)
                            .IssuesGroupBy(groupByY, _using: groupByY != "Columns")))
                                .AsEnumerable();
            }
        }

        private static Rds.IssuesColumnCollection CrosstabColumns(
            this Rds.IssuesColumnCollection self,
            SiteSettings ss,
            string groupByY,
            IEnumerable<string> columns,
            string value,
            string aggregateType)
        {
            if (groupByY != "Columns")
            {
                return self
                    .IssuesColumn(groupByY, _as: "GroupByY")
                    .IssuesColumn(value, _as: "Value", function: Sqls.Function(aggregateType));
            }
            else
            {
                Libraries.ViewModes.CrosstabUtilities.GetColumns(ss, columns)
                    .ForEach(column =>
                        self.IssuesColumn(
                            column,
                            _as: column,
                            function: Sqls.Function(aggregateType)));
                return self;
            }
        }

        public static string Gantt(SiteSettings ss)
        {
            if (ss.EnableGantt != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var groupBy = view.GetGanttGroupBy();
            var sortBy = view.GetGanttSortBy();
            var inRange = issueCollection.Aggregations.TotalCount <=
                Parameters.General.GanttLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.GanttLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
                view: view,
                viewMode: viewMode,
                viewModeBody: () =>
                {
                    if (inRange)
                    {
                        hb.Gantt(
                            ss: ss,
                            view: view,
                            dataRows: GanttDataRows(ss, view, groupBy, sortBy),
                            groupBy: groupBy,
                            sortBy: sortBy,
                            bodyOnly: false);
                    }
                });
        }

        public static string GanttJson(SiteSettings ss)
        {
            if (ss.EnableGantt != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Gantt");
            var groupBy = view.GetGanttGroupBy();
            var sortBy = view.GetGanttSortBy();
            return issueCollection.Aggregations.TotalCount <= Parameters.General.GanttLimit
                ? new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#GanttBody",
                        new HtmlBuilder().Gantt(
                            ss: ss,
                            view: view,
                            dataRows: GanttDataRows(ss, view, groupBy, sortBy),
                            groupBy: groupBy,
                            sortBy: sortBy,
                            bodyOnly: bodyOnly))
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: issueCollection.Aggregations))
                    .ClearFormData()
                    .Invoke("drawGantt")
                    .ToJson()
                : new ResponseCollection()
                    .Html(
                        !bodyOnly ? "#ViewModeContainer" : "#GanttBody",
                        new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: issueCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.GanttLimit.ToString()))
                    .ToJson();
        }

        private static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            EnumerableRowCollection<DataRow> dataRows,
            string groupBy,
            string sortBy,
            bool bodyOnly)
        {
            return !bodyOnly
                ? hb.Gantt(
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    dataRows: dataRows)
                : hb.GanttBody(
                    ss: ss,
                    groupBy: groupBy,
                    sortBy: sortBy,
                    dataRows: dataRows);
        }

        private static EnumerableRowCollection<DataRow> GanttDataRows(
            SiteSettings ss, View view, string groupBy, string sortBy)
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
                        .IssuesColumn(
                            groupBy, _as: "GroupBy", function: Sqls.Functions.SingleColumn)
                        .IssuesColumn(
                            sortBy, _as: "SortBy", function: Sqls.Functions.SingleColumn),
                    where: view.Where(ss: ss)))
                        .AsEnumerable();
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDown(SiteSettings ss)
        {
            if (ss.EnableBurnDown != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = issueCollection.Aggregations.TotalCount <=
                Parameters.General.BurnDownLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.BurnDownLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string BurnDownJson(SiteSettings ss)
        {
            if (ss.EnableBurnDown != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            return issueCollection.Aggregations.TotalCount <= Parameters.General.BurnDownLimit
                ? new ResponseCollection()
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
                    .ClearFormData()
                    .Invoke("drawBurnDown")
                    .ToJson()
                : new ResponseCollection()
                    .Html("#ViewModeContainer", new HtmlBuilder())
                    .View(ss: ss, view: view)
                    .ReplaceAll(
                        "#Aggregations", new HtmlBuilder().Aggregations(
                        ss: ss,
                        aggregations: issueCollection.Aggregations))
                    .ClearFormData()
                    .Message(Messages.TooManyCases(Parameters.General.BurnDownLimit.ToString()))
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
            var where = view.Where(ss: ss);
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
            if (ss.EnableTimeSeries != true)
            {
                return HtmlTemplates.Error(Error.Types.HasNotPermission);
            }
            var hb = new HtmlBuilder();
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = issueCollection.Aggregations.TotalCount <=
                Parameters.General.TimeSeriesLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.TimeSeriesLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
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
        public static string TimeSeriesJson(SiteSettings ss)
        {
            if (ss.EnableTimeSeries != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("TimeSeries");
            return issueCollection.Aggregations.TotalCount <= Parameters.General.TimeSeriesLimit
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
                        aggregations: issueCollection.Aggregations))
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
                        aggregations: issueCollection.Aggregations))
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
                    Rds.SelectIssues(
                        tableType: Sqls.TableTypes.NormalAndHistory,
                        column: Rds.IssuesColumn()
                            .IssueId(_as: "Id")
                            .Ver()
                            .UpdatedTime()
                            .IssuesColumn(groupBy, _as: "Index")
                            .IssuesColumn(value, _as: "Value"),
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
            var issueCollection = IssueCollection(ss, view);
            var viewMode = ViewModes.GetBySession(ss.SiteId);
            var inRange = issueCollection.Aggregations.TotalCount <=
                Parameters.General.KambanLimit;
            if (!inRange)
            {
                Sessions.Set(
                    "Message",
                    Messages.TooManyCases(Parameters.General.KambanLimit.ToString()).Html);
            }
            return hb.ViewModeTemplate(
                ss: ss,
                issueCollection: issueCollection,
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
        public static string KambanJson(
            SiteSettings ss)
        {
            if (ss.EnableKamban != true)
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            var view = Views.GetBySession(ss);
            var issueCollection = IssueCollection(ss, view);
            var bodyOnly = Forms.ControlId().StartsWith("Kamban");
            return issueCollection.Aggregations.TotalCount <= Parameters.General.KambanLimit
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
                        aggregations: issueCollection.Aggregations))
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
                        aggregations: issueCollection.Aggregations))
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
                : "RemainingWorkValue";
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
        private static Rds.IssuesColumnCollection KambanColumns(
            SiteSettings ss, string groupByX, string groupByY, string value)
        {
            var column = Rds.IssuesColumn()
                .IssueId()
                .Title()
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
                where: view.Where(ss: ss),
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
