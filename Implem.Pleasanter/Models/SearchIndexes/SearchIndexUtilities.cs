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
    public static class SearchIndexUtilities
    {
        private static DataSet ResultContents(EnumerableRowCollection<DataRow> dataRows)
        {
            var statements = new List<SqlStatement>();
            if (dataRows.Any(o => o["ReferenceType"].ToString() == "Sites"))
            {
                statements.Add(Rds.SelectSites(
                    dataTableName: "Sites",
                    column: Rds.SitesColumn()
                        .ParentId(_as: "SiteId")
                        .SiteId(_as: "Id")
                        .Title()
                        .Body(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId_In(dataRows
                            .Where(o => o["ReferenceType"].ToString() == "Sites")
                            .Select(o => o["ReferenceId"].ToLong()))));
            }
            if (dataRows.Any(o => o["ReferenceType"].ToString() == "Issues"))
            {
                statements.Add(Rds.SelectIssues(
                    dataTableName: "Issues",
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId(_as: "Id")
                        .Title()
                        .Body(),
                    where: Rds.IssuesWhere()
                        .IssueId_In(dataRows
                            .Where(o => o["ReferenceType"].ToString() == "Issues")
                            .Select(o =>o["ReferenceId"].ToLong()))));
            }
            if (dataRows.Any(o => o["ReferenceType"].ToString() == "Results"))
            {
                statements.Add(Rds.SelectResults(
                    dataTableName: "Results",
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId(_as: "Id")
                        .Title()
                        .Body(),
                    where: Rds.ResultsWhere()
                        .ResultId_In(dataRows
                            .Where(o => o["ReferenceType"].ToString() == "Results")
                            .Select(o =>o["ReferenceId"].ToLong()))));
            }
            if (dataRows.Any(o => o["ReferenceType"].ToString() == "Wikis"))
            {
                statements.Add(Rds.SelectWikis(
                    dataTableName: "Wikis",
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId(_as: "Id")
                        .Title()
                        .Body(),
                    where: Rds.WikisWhere()
                        .WikiId_In(dataRows
                            .Where(o => o["ReferenceType"].ToString() == "Wikis")
                            .Select(o =>o["ReferenceId"].ToLong()))));
            }
            return Rds.ExecuteDataSet(statements: statements.ToArray());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Search()
        {
            var dataSet = Get(
                searchIndexes: QueryStrings.Data("text").SearchIndexes(),
                offset: QueryStrings.Int("offset"),
                pageSize: Parameters.General.SearchPageSize);
            return MainContainer(
                text: QueryStrings.Data("text"),
                offset: 0,
                results: dataSet?.Tables["Main"].AsEnumerable(),
                count: Rds.Count(dataSet)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchJson()
        {
            var offset = QueryStrings.Int("offset");
            var text = QueryStrings.Data("text");
            var dataSet = Get(
                searchIndexes: text.SearchIndexes(),
                offset: offset,
                pageSize: Parameters.General.SearchPageSize);
            var results = dataSet?.Tables["Main"].AsEnumerable();
            var responseCollection = new ResponseCollection();
            if (offset == 0)
            {
                responseCollection
                    .ReplaceAll(
                        "#MainContainer",
                        MainContainer(
                            text: text,
                            offset: 0,
                            results: results,
                            count: Rds.Count(dataSet)))
                    .Focus("#Search");
            }
            else
            {
                responseCollection
                    .Append(
                        "#SearchResults",
                        new HtmlBuilder().Results(text: text, offset: offset, results: results))
                    .Val(
                        "#SearchOffset",
                        (results != null &&
                        results.Any() &&
                        results.Count() == Parameters.General.SearchPageSize
                            ? offset + Parameters.General.SearchPageSize
                            : -1).ToString());
            }
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainContainer(
            string text,
            int offset,
            EnumerableRowCollection<DataRow> results,
            int count)
        {
            var hb = new HtmlBuilder();
            var searchIndexes = text.SearchIndexes();
            return hb.Template(
                permissionType: Permissions.Types.Read,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: true,
                referenceType: "SearchIndexes",
                title: string.Empty,
                useNavigationMenu: false,
                useTitle: false,
                useSearch: false,
                useBreadcrumb: false,
                action: () => hb
                    .Div(id: "SearchResults", action: () => hb
                        .Command(text: text)
                        .Count(count: count)
                        .Results(text: text, offset: offset, results: results))
                    .Hidden(
                        controlId: "SearchOffset",
                        value: Parameters.General.SearchPageSize.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Command(this HtmlBuilder hb, string text)
        {
            return hb.Div(css: "command-center", action: () => hb
                .TextBox(
                    controlId: "Search",
                    controlCss: " w600 focus",
                    text: text,
                    placeholder: Displays.Search()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Count(
            this HtmlBuilder hb, int count)
        {
            return hb.Div(css: "count", action: () => hb
                .Span(css: "label", action: () => hb
                    .Text(text: Displays.Quantity()))
                .Span(css: "data", action: () => hb
                    .Text(text: count.ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Results(
            this HtmlBuilder hb, string text, int offset, EnumerableRowCollection<DataRow> results)
        {
            if (results != null && results.Any())
            {
                var dataSet = ResultContents(results);
                results.ForEach(result =>
                {
                    var referenceType = result["ReferenceType"].ToString();
                    var referenceId = result["ReferenceId"].ToLong();
                    var dataRow = dataSet.Tables[referenceType]
                        .AsEnumerable()
                        .FirstOrDefault(o => o["Id"].ToLong() == referenceId);
                    if (dataRow != null)
                    {
                        var href = string.Empty;
                        switch (referenceType)
                        {
                            case "Sites":
                                href = Navigations.ItemIndex(referenceId);
                                break;
                            default:
                                href = Navigations.ItemEdit(referenceId);
                                break;
                        }
                        hb.Section(
                            attributes: new HtmlAttributes()
                                .Class("result")
                                .Add("data-href", href),
                            action: () => hb
                                .Breadcrumb(dataRow["SiteId"].ToLong())
                                .H(number: 3, action: () => hb
                                     .A(
                                         href: href,
                                         text: dataRow["Title"].ToString()))
                                .P(action: () => hb
                                    .Text(text: dataRow["Body"].ToString())));
                    }
                });
            }
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static IEnumerable<long> GetIdCollection(IEnumerable<string> searchIndexes, long siteId)
        {
            return Get(
                searchIndexes: searchIndexes,
                column: Rds.SearchIndexesColumn().ReferenceId(),
                siteId: siteId)
                    .Tables[0]
                    .AsEnumerable()
                    .Distinct()
                    .Select(o => o["ReferenceId"].ToLong());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static DataSet Get(IEnumerable<string> searchIndexes, int offset, int pageSize)
        {
            return Get(
                searchIndexes: searchIndexes,
                column: Rds.SearchIndexesColumn()
                    .ReferenceId()
                    .ReferenceType()
                    .PriorityTotal()
                    .SearchIndexesCount(),
                offset: offset,
                pageSize: pageSize,
                countRecord: offset == 0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static DataSet Get(
            IEnumerable<string> searchIndexes,
            SqlColumnCollection column,
            int offset = 0,
            int pageSize = 0,
            long siteId = 0,
            bool countRecord = false)
        {
            if (searchIndexes.Count() == 0) return null;
            var concordance = Math.Ceiling(
                searchIndexes.Count() * Parameters.General.SearchConcordanceRate);
            return Rds.ExecuteDataSet(statements:
                Rds.SelectSearchIndexes(
                    dataTableName: "Main",
                    column: column,
                    join: Rds.SearchIndexesJoinDefault(),
                    where: Rds.SearchIndexesWhere()
                        .Word(searchIndexes, multiParamOperator: " or ")
                        .PermissionType(0, _operator: "<>")
                        .Items_SiteId(value: siteId, tableName: "t1", _using: siteId != 0),
                    groupBy: Rds.SearchIndexesGroupBy()
                        .ReferenceId()
                        .ReferenceType(),
                    having: Rds.SearchIndexesHaving()
                        .SearchIndexesCount(concordance, _operator: ">="),
                    orderBy: Rds.SearchIndexesOrderBy()
                        .SearchIndexesCount(SqlOrderBy.Types.desc)
                        .PriorityTotal()
                        .UpdatedTimeMax(SqlOrderBy.Types.desc),
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Maintain()
        {
            if ((DateTime.Now - Applications.SearchIndexesMaintenanceDate).Days > 0)
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.PhysicalDeleteSearchIndexes(
                        where: Rds.SearchIndexesWhere().Add(
                            sub: Rds.ExistsItems(
                                not: true,
                                where: Rds.ItemsWhere()
                                    .ReferenceId(raw: "[SearchIndexes].[ReferenceId]")))));
                Applications.SearchIndexesMaintenanceDate = DateTime.Now.Date;
            }
        }
    }
}
