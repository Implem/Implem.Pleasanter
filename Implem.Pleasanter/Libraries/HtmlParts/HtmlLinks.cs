using Implem.Libraries.DataSources.Interfaces;
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

        public static HtmlBuilder LinkField(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            long linkId,
            List<Link> links,
            DataSet dataSet,
            BaseModel.MethodTypes? methodType,
            int tabIndex)
        {
            new[]
            {
                new
                {
                    Ss = TargetSiteSettings(ss.Destinations, linkId),
                    Direction = "Destination"
                },
                new
                {
                    Ss = TargetSiteSettings(ss.Sources, linkId),
                    Direction = "Source"
                }
            }.Where(target => target.Ss != null).ForEach(target =>
            {
                var targetSs = ss.JoinedSsHash.Get(linkId);
                if (targetSs != null)
                {
                    var dataTableName = DataTableName(
                        ss: targetSs,
                        direction: target.Direction);
                    hb.Div(
                        id: dataTableName + "Field",
                        action: () => hb.Link(
                            context: context,
                            ss: ss,
                            id: id,
                            linkId: linkId,
                            direction: target.Direction,
                            targetSs: targetSs,
                            links: links,
                            dataSet: dataSet,
                            methodType: methodType,
                            tabIndex: tabIndex));
                }
            });
            return hb;
        }

        public static ResponseCollection Links(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            long id,
            BaseModel.MethodTypes? methodType)
        {
            var dataSet = DataSet(
                context: context,
                ss: ss,
                id: id);
            var links = HtmlLinkCreations.Links(
                context: context,
                ss: ss);
            new[] { ss.TabName(0) }
                .Concat(ss.Tabs?.Select(tab => ss.TabName(tab.Id)) ?? new List<string>())
                .Select((tabName, tabIndex) => new { tabName, tabIndex })
                .ForEach(data =>
                {
                    ss.EditorColumnHash?.Get(data.tabName)?.ForEach(columnName =>
                    {
                        var linkId = ss.LinkId(columnName);
                        if (linkId > 0)
                        {
                            var sourceSs = TargetSiteSettings(ss.Sources, linkId);
                            var destinationSs = TargetSiteSettings(ss.Destinations, linkId);
                            var targetSs = sourceSs ?? destinationSs;
                            if (targetSs != null)
                            {
                                var direction = sourceSs != null
                                    ? "Source"
                                    : "Destination";
                                var dataTableName = DataTableName(
                                    ss: targetSs,
                                    direction: direction);
                                res.Html("#" + dataTableName + "Field", new HtmlBuilder().Link(
                                    context: context,
                                    ss: ss,
                                    id: id,
                                    linkId: linkId,
                                    direction: direction,
                                    targetSs: targetSs,
                                    links: links,
                                    dataSet: dataSet,
                                    methodType: methodType,
                                    tabIndex: data.tabIndex));
                            }
                        }
                    });
                });
            return res;
        }

        private static HtmlBuilder Link(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            long linkId,
            string direction,
            SiteSettings targetSs,
            List<Link> links,
            DataSet dataSet,
            BaseModel.MethodTypes? methodType,
            int tabIndex)
        {
            return hb.FieldSet(
                css: " enclosed link-creations",
                legendText: Displays.Links(context: context),
                action: () => hb.Div(action: () =>
                {
                    var link = links.FirstOrDefault(o => o.SourceId == linkId);
                    if (link != null)
                    {
                        hb.Div(action: () => hb.LinkCreationButton(
                            context: context,
                            ss: ss,
                            linkId: id,
                            sourceId: link.SourceId,
                            text: link.SiteTitle,
                            tabIndex: tabIndex));
                    }
                    hb.LinkTable(
                        context: context,
                        ss: targetSs,
                        direction: direction,
                        dataSet: dataSet);
                }),
                _using: methodType != BaseModel.MethodTypes.New);
        }

        private static SiteSettings TargetSiteSettings(
            Dictionary<long, SiteSettings> links,
            long linkId)
        {
            return links
                ?.Where(o => o.Key == linkId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static DataSet DataSet(Context context, SiteSettings ss, long id)
        {
            var statements = new List<SqlStatement>();
            ss.Sources
                .Values
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
                .Values
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
                .Values
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
                .Values
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
                ? Repository.ExecuteDataSet(
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
            var column = IssuesLinkColumns(
                context: context,
                ss: ss,
                view: view,
                direction: direction);
            var where = view.Where(
                context: context,
                ss: ss,
                where: Rds.IssuesWhere()
                    .IssueId_In(sub: Targets(
                        context: context,
                        id: id,
                        direction: direction))
                    .CanRead(
                        context: context,
                        idColumnBracket: "\"Issues\".\"IssueId\"")
                    .Sites_TenantId(context.TenantId));
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            return Rds.SelectIssues(
                dataTableName: DataTableName(
                    ss: ss,
                    direction: direction),
                column: column,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        column,
                        where,
                        orderBy
                    })
                        .Add(
                            tableName: "\"Sites\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Sites\".\"SiteId\"=\"Issues\".\"SiteId\""),
                where: where,
                orderBy: orderBy);
        }

        public static SqlColumnCollection IssuesLinkColumns(
            Context context,
            SiteSettings ss,
            View view,
            string direction)
        {
            if (ss.Links?.Any() == true)
            {
                ss.SetLinkedSiteSettings(context: context);
            }
            var column = ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                columns: ss.GetLinkTableColumns(
                    context: context,
                    view: view));
            column.Add("'" + direction + "' as Direction");
            return new SqlColumnCollection(column
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray());
        }

        private static SqlStatement SelectResults(
            Context context,
            SiteSettings ss,
            View view,
            long id,
            string direction)
        {
            var column = ResultsLinkColumns(
                context: context,
                ss: ss,
                view: view,
                direction: direction);
            var where = view.Where(
                context: context,
                ss: ss,
                where: Rds.ResultsWhere()
                    .ResultId_In(sub: Targets(
                        context: context,
                        id: id,
                        direction: direction))
                    .CanRead(
                        context: context,
                        idColumnBracket: "\"Results\".\"ResultId\"")
                    .Sites_TenantId(context.TenantId));
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            return Rds.SelectResults(
                dataTableName: DataTableName(
                    ss: ss,
                    direction: direction),
                column: column,
                join: ss.Join(
                    context: context,
                    join: new IJoin[]
                    {
                        column,
                        where,
                        orderBy
                    })
                        .Add(
                            tableName: "\"Sites\"",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "\"Sites\".\"SiteId\"=\"Results\".\"SiteId\""),
                where: where,
                orderBy: orderBy);
        }

        public static SqlColumnCollection ResultsLinkColumns(
            Context context,
            SiteSettings ss,
            View view,
            string direction)
        {
            if (ss.Links?.Any() == true)
            {
                ss.SetLinkedSiteSettings(context: context);
            }
            var column = ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                columns: ss.GetLinkTableColumns(
                    context: context,
                    view: view));
            column.Add("'" + direction + "' as Direction");
            return new SqlColumnCollection(column
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray());
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
            if (Contains(
                dataSet: dataSet,
                ssList: ss.Destinations.Values,
                direction: "Destination"))
            {
                return true;
            }
            if (Contains(
                dataSet: dataSet,
                ssList: ss.Sources.Values,
                direction: "Source"))
            {
                return true;
            }
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
                ss.Destinations.Keys.ForEach(siteId =>
                {
                    var currentSs = ss.JoinedSsHash.Get(siteId);
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
                ss.Sources.Keys.ForEach(siteId =>
                {
                    var currentSs = ss.JoinedSsHash.Get(siteId);
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
                    return Repository.ExecuteTable(
                        context: context,
                        statements: SelectIssues(
                            context: context,
                            ss: ss,
                            view: view,
                            id: id,
                            direction: direction))
                                .AsEnumerable();
                case "Results":
                    return Repository.ExecuteTable(
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
            SiteSettings ss,
            string direction,
            DataSet dataSet)
        {
            var dataTableName = DataTableName(
                ss: ss,
                direction: direction);
            return hb.LinkTable(
                context: context,
                ss: ss,
                view: Views.GetBySession(
                    context: context,
                    ss: ss,
                    dataTableName: dataTableName),
                dataRows: DataRows(
                    dataSet: dataSet,
                    ss: ss,
                    dataTableName: dataTableName),
                direction: direction,
                dataTableName: dataTableName);
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
            ss.SetChoiceHash(dataRows: dataRows);
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
                                    .Caption(
                                        action: () => hb
                                            .Span(
                                                css: "caption-direction",
                                                action: () => hb
                                                    .Text(text: "{0} : ".Params(Caption(
                                                        context: context,
                                                        direction: direction))))
                                            .Span(
                                                css: "caption-title",
                                                action: () => hb
                                                    .Text(text: "{0}".Params(siteMenu.Breadcrumb(
                                                        context: context,
                                                        siteId: ss.SiteId)
                                                            .Select(o => o.Title)
                                                            .Join(" > "))))
                                            .Span(
                                                css: "caption-quantity",
                                                action: () => hb
                                                    .Text(text: " - {0} ".Params(
                                                        Displays.Quantity(context: context))))
                                            .Span(
                                                css: "caption-count",
                                                action: () => hb
                                                    .Text(text: "{0}".Params(dataRows.Count()))))
                                    .THead(action: () => hb
                                        .GridHeader(
                                            context: context,
                                            ss: ss,
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
                                    .Caption(
                                        action: () => hb
                                            .Span(
                                                css: "caption-direction",
                                                action: () => hb
                                                    .Text(text: "{0} : ".Params(Caption(
                                                        context: context,
                                                        direction: direction))))
                                            .Span(
                                                css: "caption-title",
                                                action: () => hb
                                                    .Text(text: "{0}".Params(siteMenu.Breadcrumb(
                                                        context: context,
                                                        siteId: ss.SiteId)
                                                            .Select(o => o.Title)
                                                            .Join(" > "))))
                                            .Span(
                                                css: "caption-quantity",
                                                action: () => hb
                                                    .Text(text: " - {0} ".Params(
                                                        Displays.Quantity(context: context))))
                                            .Span(
                                                css: "caption-count",
                                                action: () => hb
                                                    .Text(text: "{0}".Params(dataRows.Count()))))
                                    .THead(action: () => hb
                                        .GridHeader(
                                            context: context,
                                            ss: ss,
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
