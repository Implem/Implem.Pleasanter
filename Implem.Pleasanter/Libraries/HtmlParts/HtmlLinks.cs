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
        private struct LinkData
        {
            public long SiteId;
            public int? Priority;
            public string Direction;
            public SiteSettings SiteSettings;
            public string DataTableName;
        }

        public static HtmlBuilder Links(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long id,
            DataSet dataSet = null)
        {
            dataSet = dataSet ?? DataSet(
                context: context,
                ss: ss,
                id: id);
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
                            new[]
                            {
                                new
                                {
                                    Ss = TargetSiteSettings(
                                        links: ss.Destinations,
                                        linkId: linkId),
                                    Direction = "Destination"
                                },
                                new
                                {
                                    Ss = TargetSiteSettings(
                                        links: ss.Sources,
                                        linkId: linkId),
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
                                    res.Html("#" + dataTableName + "Field", new HtmlBuilder()
                                        .Link(
                                            context: context,
                                            ss: ss,
                                            id: id,
                                            linkId: linkId,
                                            direction: target.Direction,
                                            targetSs: targetSs,
                                            links: links,
                                            dataSet: dataSet,
                                            methodType: methodType,
                                            tabIndex: data.tabIndex));
                                }
                            });
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
            var dataRows = DataRows(
                dataSet: dataSet,
                ss: targetSs,
                dataTableName: DataTableName(
                    ss: targetSs,
                    direction: direction));
            var link = links.FirstOrDefault(o => o.SourceId == linkId);
            var addButton = link != null && direction == "Source";
            // レコードが存在するか、追加ボタンが無い場合にはフィールドセットを作成しない
            // linksには、NoAddButtonがtrueのものは含まれていないためNoAddButtonのチェックは不要
            // linksはHtmlLinkCreations.Linksメソッドで生成される
            return dataRows?.Any() == true || addButton
                ? hb.FieldSet(
                    css: " enclosed link-creations",
                    legendText: Displays.Links(context: context),
                    action: () => hb.Div(action: () =>
                    {
                        if (addButton)
                        {
                            hb.Div(action: () => hb.LinkCreationButton(
                                context: context,
                                ss: ss,
                                linkId: id,
                                sourceId: link.SourceId,
                                text: link.SiteTitle,
                                tabIndex: tabIndex,
                                notReturnParentRecord: link.NotReturnParentRecord ?? false));
                        }
                        hb.LinkTable(
                            context: context,
                            ss: targetSs,
                            direction: direction,
                            dataSet: dataSet,
                            tabIndex: tabIndex);
                    }),
                    _using: methodType != BaseModel.MethodTypes.New)
                : hb;
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
            var targets = ss.EditorColumnHash.Values
                .SelectMany(o => o
                    .Where(p => p.StartsWith("_Links-"))
                    .Select(p => p.Split_2nd('-').ToLong()))
                .ToList();
            var statements = new List<SqlStatement>();
            statements.AddRange(ss.Sources
                .Values
                .Where(o => !targets.Any() || targets.Contains(o.SiteId))
                .Where(currentSs => currentSs.ReferenceType == "Issues")
                .Select(currentSs => SelectIssues(
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
            statements.AddRange(ss.Destinations
                .Values
                .Where(o => !targets.Any() || targets.Contains(o.SiteId))
                .Where(currentSs => currentSs.ReferenceType == "Issues")
                .Select(currentSs => SelectIssues(
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
            statements.AddRange(ss.Sources
                .Values
                .Where(o => !targets.Any() || targets.Contains(o.SiteId))
                .Where(currentSs => currentSs.ReferenceType == "Results")
                .Select(currentSs => SelectResults(
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
            statements.AddRange(ss.Destinations
                .Values
                .Where(o => !targets.Any() || targets.Contains(o.SiteId))
                .Where(currentSs => currentSs.ReferenceType == "Results")
                .Select(currentSs => SelectResults(
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
                    .Sites_TenantId(context.TenantId),
                requestSearchCondition: false);
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
                pageSize: ss.LinkPageSize ?? 0,
                orderBy: orderBy);
        }

        public static SqlColumnCollection IssuesLinkColumns(
            Context context,
            SiteSettings ss,
            View view,
            string direction)
        {
            if (ss?.Links
                ?.Where(o => o.SiteId > 0)
                .Any() == true)
            {
                ss.SetLinkedSiteSettings(context: context);
            }
            var column = ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                view: view,
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
                    .Sites_TenantId(context.TenantId),
                requestSearchCondition: false);
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
                pageSize: ss.LinkPageSize ?? 0,
                orderBy: orderBy);
        }

        public static SqlColumnCollection ResultsLinkColumns(
            Context context,
            SiteSettings ss,
            View view,
            string direction)
        {
            if (ss?.Links
                ?.Where(o => o.SiteId > 0)
                .Any() == true)
            {
                ss.SetLinkedSiteSettings(context: context);
            }
            var column = ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                view: view,
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
            var linkData = new List<LinkData>();
            return hb.Div(action: () =>
            {
                ss.Destinations.Keys.ForEach(siteId =>
                {
                    var direction = "Destination";
                    var currentSs = ss.JoinedSsHash.Get(siteId);
                    var dataTableName = DataTableName(
                        ss: currentSs,
                        direction: direction);
                    linkData.Add(new LinkData()
                    {
                        SiteId = siteId,
                        Priority = ss.Links
                            ?.Where(link => link.SiteId == siteId)
                            .OrderBy(link => link.Priority ?? int.MaxValue)
                            .FirstOrDefault()
                            ?.Priority ?? int.MaxValue,
                        Direction = direction,
                        SiteSettings = currentSs,
                        DataTableName = dataTableName
                    });
                });
                ss.Sources.Keys.ForEach(siteId =>
                {
                    var direction = "Source";
                    var currentSs = ss.JoinedSsHash.Get(siteId);
                    var dataTableName = DataTableName(
                        ss: currentSs,
                        direction: direction);
                    linkData.Add(new LinkData()
                    {
                        SiteId = siteId,
                        Priority = currentSs.Links
                            ?.Where(link => link.SiteId == ss.SiteId)
                            .OrderBy(link => link.Priority ?? int.MaxValue)
                            .FirstOrDefault()
                            ?.Priority ?? int.MaxValue,
                        Direction = direction,
                        SiteSettings = currentSs,
                        DataTableName = dataTableName
                    });
                });
                // Priorityに明示的に値が設定されている場合にはソートを行う
                if (linkData.Any(data => data.Priority != int.MaxValue))
                {
                    linkData = linkData
                        .OrderBy(data => data.Priority)
                        .ThenBy(data => data.SiteId)
                        .ToList();
                }
                linkData.ForEach(data => hb.LinkTable(
                    context: context,
                    ss: data.SiteSettings,
                    view: Views.GetBySession(
                        context: context,
                        ss: data.SiteSettings,
                        dataTableName: data.DataTableName),
                    dataRows: DataRows(
                        dataSet: dataSet,
                        ss: data.SiteSettings,
                        dataTableName: data.DataTableName),
                    direction: data.Direction,
                    dataTableName: data.DataTableName));
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
            return dataSet?.Tables[dataTableName]?
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
            DataSet dataSet,
            int tabIndex)
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
                dataTableName: dataTableName,
                tabIndex: tabIndex);
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
            string dataTableName,
            int tabIndex = 0)
        {
            if (dataRows != null)
            {
                ss.SetChoiceHash(
                    context: context,
                    dataRows: dataRows);
            }
            return hb.GridTable(
                id: dataTableName,
                css: ss.GetNoDisplayIfReadOnly(context: context)
                    ? " not-link"
                    : string.Empty,
                attributes: new HtmlAttributes()
                    .DataId(ss.SiteId.ToString())
                    .DataName(direction)
                    .DataValue("back")
                    .DataAction("LinkTable")
                    .DataMethod("post")
                    .Add(
                        name: "from-tab-index",
                        value: tabIndex.ToString()),
                action: () =>
                {
                    var siteMenu = SiteInfo.TenantCaches.Get(context.TenantId)?.SiteMenu;
                    if (dataRows != null && dataRows.Any())
                    {
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
                                    .TBody(action: () => issueCollection.ForEach(issueModel =>
                                    {
                                        var serverScriptModelRow = issueModel?.SetByBeforeOpeningRowServerScript(
                                            context: context,
                                            ss: ss,
                                            view: view);
                                        var extendedRowCss = serverScriptModelRow?.ExtendedRowCss;
                                        extendedRowCss = extendedRowCss.IsNullOrEmpty()
                                            ? string.Empty
                                            : " " + extendedRowCss;
                                        hb.Tr(
                                            attributes: new HtmlAttributes()
                                                .Class("grid-row" + extendedRowCss)
                                                .DataId(issueModel.IssueId.ToString())
                                                .Add(name: "data-extension", value: serverScriptModelRow?.ExtendedRowData),
                                            action: () => columns.ForEach(column =>
                                            {
                                                var serverScriptModelColumn = serverScriptModelRow
                                                    ?.Columns
                                                    ?.Get(column?.ColumnName);
                                                hb.TdValue(
                                                    context: context,
                                                    ss: ss,
                                                    column: column,
                                                    issueModel: issueModel,
                                                    tabIndex: tabIndex,
                                                    serverScriptModelColumn: serverScriptModelColumn);
                                            }));
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
                                    .TBody(action: () => resultCollection.ForEach(resultModel =>
                                    {
                                        var serverScriptModelRow = resultModel?.SetByBeforeOpeningRowServerScript(
                                            context: context,
                                            ss: ss,
                                            view: view);
                                        var extendedRowCss = serverScriptModelRow?.ExtendedRowCss;
                                        extendedRowCss = extendedRowCss.IsNullOrEmpty()
                                            ? string.Empty
                                            : " " + extendedRowCss;
                                        hb.Tr(
                                            attributes: new HtmlAttributes()
                                                .Class("grid-row" + extendedRowCss)
                                                .DataId(resultModel.ResultId.ToString())
                                                .Add(name: "data-extension", value: serverScriptModelRow?.ExtendedRowData),
                                            action: () => columns.ForEach(column =>
                                            {
                                                var serverScriptModelColumn = serverScriptModelRow
                                                    ?.Columns
                                                    ?.Get(column?.ColumnName);
                                                hb.TdValue(
                                                    context: context,
                                                    ss: ss,
                                                    column: column,
                                                    resultModel: resultModel,
                                                    tabIndex: tabIndex,
                                                    serverScriptModelColumn: serverScriptModelColumn);
                                            }));
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
