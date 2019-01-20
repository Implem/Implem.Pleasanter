using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlLinks
    {
        public static HtmlBuilder Links(
            this HtmlBuilder hb, Context context, SiteSettings ss, long id)
        {
            var dataSet = DataSet(context: context, ss: ss, id: id);
            return Contains(ss, dataSet)
                ? hb.FieldSet(
                    css: " enclosed",
                    legendText: Displays.Links(context: context),
                    action: () => hb
                        .Links(
                            context: context,
                            ss: ss,
                            dataSet: dataSet))
                : hb;
        }

        private static DataSet DataSet(Context context, SiteSettings ss, long id)
        {
            var statements = new List<SqlStatement>();
            ss.Sources
                .Where(currentSs => currentSs.ReferenceType == "Issues")
                .ForEach(currentSs =>
                    statements.Add(SelectIssues(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: DataTableName(
                                ss: currentSs,
                                direction: "Source")),
                        id: id,
                        direction: "Source")));
            ss.Destinations
                .Where(currentSs => currentSs.ReferenceType == "Issues")
                .ForEach(currentSs =>
                    statements.Add(SelectIssues(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: DataTableName(
                                ss: currentSs,
                                direction: "Destination")),
                        id: id,
                        direction: "Destination")));
            ss.Sources
                .Where(currentSs => currentSs.ReferenceType == "Results")
                .ForEach(currentSs =>
                    statements.Add(SelectResults(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: DataTableName(
                                ss: currentSs,
                                direction: "Source")),
                        id: id,
                        direction: "Source")));
            ss.Destinations
                .Where(currentSs => currentSs.ReferenceType == "Results")
                .ForEach(currentSs =>
                    statements.Add(SelectResults(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: DataTableName(
                                ss: currentSs,
                                direction: "Destination")),
                        id: id,
                        direction: "Destination")));
            return statements.Any()
                ? Rds.ExecuteDataSet(
                    context: context,
                    statements: statements.ToArray())
                : null;
        }

        private static SqlStatement SelectIssues(
            Context context,
            SiteSettings ss,
            View view,
            long id,
            string direction)
        {
            return Rds.SelectIssues(
                dataTableName: DataTableName(
                    ss: ss,
                    direction: direction),
                column: IssuesLinkColumns(
                    context: context,
                    ss: ss,
                    view: view,
                    direction: direction),
                join: Rds.IssuesJoinDefault()
                    .Add(
                        tableName: "Sites",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "[Sites].[SiteId]=[Issues].[SiteId]"),
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(sub: Targets(
                        context: context,
                        id: id,
                        direction: direction))
                    .CanRead(context: context, idColumnBracket: "[Issues].[IssueId]")
                    .Sites_TenantId(context.TenantId),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss));
        }

        public static Rds.IssuesColumnCollection IssuesLinkColumns(
            Context context, SiteSettings ss, View view, string direction)
        {
            var column = Rds.IssuesColumn()
                .SiteId()
                .IssueId();
            var linkTableColumns = ss.GetLinkTableColumns(
                context: context,
                view: view)
                    .Select(o => o.ColumnName)
                    .ToList();
            linkTableColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "Title":
                        ss.TitleColumns?
                            .Where(o => o == "Title"
                                || linkTableColumns.Contains(o) != true)
                            .ForEach(o =>
                                column.IssuesColumn(o));
                        break;
                    default:
                        column.IssuesColumn(columnName);
                        break;
                }
            });
            return column.Add("'" + direction + "' as Direction");
        }

        private static SqlStatement SelectResults(
            Context context,
            SiteSettings ss,
            View view,
            long id,
            string direction)
        {
            return Rds.SelectResults(
                dataTableName: DataTableName(
                    ss: ss,
                    direction: direction),
                column: ResultsLinkColumns(
                    context: context,
                    ss: ss,
                    view: view,
                    direction: direction),
                join: Rds.ResultsJoinDefault()
                    .Add(
                        tableName: "Sites",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "[Sites].[SiteId]=[Results].[SiteId]"),
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(sub: Targets(
                        context: context,
                        id: id,
                        direction: direction))
                    .CanRead(context: context, idColumnBracket: "[Results].[ResultId]")
                    .Sites_TenantId(context.TenantId),
                orderBy: view.OrderBy(
                    context: context,
                    ss: ss));
        }

        public static Rds.ResultsColumnCollection ResultsLinkColumns(
            Context context, SiteSettings ss, View view, string direction)
        {
            var column = Rds.ResultsColumn()
                .SiteId()
                .ResultId();
            var linkTableColumns = ss.GetLinkTableColumns(
                context: context,
                view: view)
                    .Select(o => o.ColumnName)
                    .ToList();
            linkTableColumns.ForEach(columnName =>
            {
                switch (columnName)
                {
                    case "Title":
                        ss.TitleColumns?
                            .Where(o => o == "Title"
                                || linkTableColumns.Contains(o) != true)
                            .ForEach(o =>
                                column.ResultsColumn(o));
                        break;
                    default:
                        column.ResultsColumn(columnName);
                        break;
                }
            });
            return column.Add("'" + direction + "' as Direction");
        }

        private static SqlSelect Targets(
            Context context, long id, string direction)
        {
            switch (direction)
            {
                case "Destination":
                    return Rds.SelectLinks(
                        column: Rds.LinksColumn().DestinationId(),
                        where: Rds.LinksWhere().SourceId(id));
                case "Source":
                    return Rds.SelectLinks(
                        column: Rds.LinksColumn().SourceId(),
                        where: Rds.LinksWhere().DestinationId(id));
                default:
                    return null;
            }
        }

        private static bool Contains(SiteSettings ss, DataSet dataSet)
        {
            if (Contains(dataSet, ss.Destinations, "Destination")) return true;
            if (Contains(dataSet, ss.Sources, "Source")) return true;
            return false;
        }

        private static bool Contains(
            DataSet dataSet, IEnumerable<SiteSettings> ssList, string direction)
        {
            foreach (var ss in ssList)
            {
                if (dataSet?.Tables[DataTableName(
                    ss: ss,
                    direction: direction)]?
                        .AsEnumerable()
                        .Where(dataRow => dataRow.Long("SiteId") == ss.SiteId).Any() == true)
                {
                    return true;
                };
            }
            return false;
        }

        private static string DataTableName(SiteSettings ss, string direction)
        {
            return ss.ReferenceType + "_" + direction + ss.SiteId;
        }

        private static HtmlBuilder Links(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            DataSet dataSet)
        {
            return hb.Div(action: () =>
            {
                ss.Destinations.ForEach(currentSs =>
                {
                    var dataTableName = DataTableName(
                        ss: currentSs,
                        direction: "Destination");
                    hb.LinkTable(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: dataTableName),
                        dataRows: DataRows(
                            dataSet: dataSet,
                            ss: currentSs,
                            dataTableName: dataTableName),
                        direction: "Destination",
                        dataTableName: dataTableName);
                });
                ss.Sources.ForEach(currentSs =>
                {
                    var dataTableName = DataTableName(
                        ss: currentSs,
                        direction: "Source");
                    hb.LinkTable(
                        context: context,
                        ss: currentSs,
                        view: Views.GetBySession(
                            context: context,
                            ss: currentSs,
                            dataTableName: dataTableName),
                        dataRows: DataRows(
                            dataSet: dataSet,
                            ss: currentSs,
                            dataTableName: dataTableName),
                        direction: "Source",
                        dataTableName: dataTableName);
                });
                hb.Button(
                    controlId: "ViewSorters_Reset",
                    controlCss: "hidden",
                    action: "LinkTable",
                    method: "post");
            });
        }

        private static EnumerableRowCollection<DataRow> DataRows(
            DataSet dataSet, SiteSettings ss, string dataTableName)
        {
            return dataSet.Tables[dataTableName]?
                .AsEnumerable()
                .Where(dataRow => dataRow.Long("SiteId") == ss.SiteId);
        }

        private static EnumerableRowCollection<DataRow> DataRows(
            Context context,
            SiteSettings ss,
            View view,
            string direction,
            long id)
        {
            switch (ss.ReferenceType)
            {
                case "Issues":
                    return Rds.ExecuteTable(
                        context: context,
                        statements: SelectIssues(
                            context: context,
                            ss: ss,
                            view: view,
                            id: id,
                            direction: direction))
                                .AsEnumerable();
                case "Results":
                    return Rds.ExecuteTable(
                        context: context,
                        statements: SelectResults(
                            context: context,
                            ss: ss,
                            view: view,
                            id: id,
                            direction: direction))
                                .AsEnumerable();
                default:
                    return null;
            }
        }

        public static HtmlBuilder LinkTable(
            this HtmlBuilder hb,
            Context context,
            long siteId,
            string direction,
            string dataTableName)
        {
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            var view = Views.GetBySession(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                setSession: true);
            return hb.LinkTable(
                context: context,
                ss: ss,
                view: view,
                dataRows: DataRows(
                    context: context,
                    ss: ss,
                    view: view,
                    direction: direction,
                    id: context.Id),
                direction: direction,
                dataTableName: dataTableName);
        }

        private static HtmlBuilder LinkTable(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            EnumerableRowCollection<DataRow> dataRows,
            string direction,
            string dataTableName)
        {
            return hb.Table(
                id: dataTableName,
                css: "grid",
                attributes: new HtmlAttributes()
                    .DataId(ss.SiteId.ToString())
                    .DataName(direction)
                    .DataValue("back")
                    .DataAction("LinkTable")
                    .DataMethod("post"),
                action: () =>
                {
                    var siteMenu = SiteInfo.TenantCaches.Get(context.TenantId)?.SiteMenu;
                    if (dataRows != null && dataRows.Any())
                    {
                        ss.SetColumnAccessControls(context: context);
                        var columns = ss.GetLinkTableColumns(
                            context: context,
                            view: view,
                            checkPermission: true);
                        switch (ss.ReferenceType)
                        {
                            case "Issues":
                                var issueCollection = new IssueCollection(
                                    context: context,
                                    ss: ss,
                                    dataRows: dataRows);
                                issueCollection.SetLinks(context: context, ss: ss);
                                hb
                                    .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                        Caption(
                                            context: context,
                                            direction: direction),
                                        siteMenu.Breadcrumb(context: context, siteId: ss.SiteId)
                                            .Select(o => o.Title)
                                            .Join(" > "),
                                        Displays.Quantity(context: context),
                                        dataRows.Count()))
                                    .THead(action: () => hb
                                        .GridHeader(
                                            context: context,
                                            columns: columns,
                                            view: view,
                                            sort: true,
                                            checkRow: false,
                                            action: "LinkTable"))
                                    .TBody(action: () => issueCollection
                                        .ForEach(issueModel =>
                                        {
                                            ss.SetColumnAccessControls(
                                                context: context,
                                                mine: issueModel.Mine(context: context));
                                            hb.Tr(
                                                attributes: new HtmlAttributes()
                                                    .Class("grid-row")
                                                    .DataId(issueModel.IssueId.ToString()),
                                                action: () => columns
                                                    .ForEach(column => hb
                                                        .TdValue(
                                                            context: context,
                                                            ss: ss,
                                                            column: column,
                                                            issueModel: issueModel)));
                                        }));
                                break;
                            case "Results":
                                var resultCollection = new ResultCollection(
                                    context: context,
                                    ss: ss,
                                    dataRows: dataRows);
                                resultCollection.SetLinks(context: context, ss: ss);
                                hb
                                    .Caption(caption: "{0} : {1} - {2} {3}".Params(
                                        Caption(
                                            context: context,
                                            direction: direction),
                                        siteMenu.Breadcrumb(context: context, siteId: ss.SiteId)
                                            .Select(o => o.Title)
                                            .Join(" > "),
                                        Displays.Quantity(context: context),
                                        dataRows.Count()))
                                    .THead(action: () => hb
                                        .GridHeader(
                                            context: context,
                                            columns: columns,
                                            view: view,
                                            sort: true,
                                            checkRow: false,
                                            action: "LinkTable"))
                                    .TBody(action: () => resultCollection
                                        .ForEach(resultModel =>
                                        {
                                            ss.SetColumnAccessControls(
                                                context: context,
                                                mine: resultModel.Mine(context: context));
                                            hb.Tr(
                                                attributes: new HtmlAttributes()
                                                    .Class("grid-row")
                                                    .DataId(resultModel.ResultId.ToString()),
                                                action: () => columns
                                                    .ForEach(column => hb
                                                        .TdValue(
                                                            context: context,
                                                            ss: ss,
                                                            column: column,
                                                            resultModel: resultModel)));
                                        }));
                                break;
                        }
                    }
                });
        }

        private static string Caption(Context context, string direction)
        {
            switch (direction)
            {
                case "Destination":
                    return Displays.LinkDestinations(context: context);
                case "Source":
                    return Displays.LinkSources(context: context);
                default:
                    return null;
            }
        }
    }
}
