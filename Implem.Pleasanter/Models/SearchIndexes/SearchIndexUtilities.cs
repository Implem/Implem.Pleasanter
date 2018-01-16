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
                searchText: QueryStrings.Data("text"),
                dataTableName: "SearchResults",
                offset: QueryStrings.Int("offset"),
                pageSize: Parameters.General.SearchPageSize);
            return MainContainer(
                text: QueryStrings.Data("text"),
                offset: 0,
                results: dataSet?.Tables["SearchResults"].AsEnumerable(),
                count: Rds.Count(dataSet)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchJson()
        {
            var offset = QueryStrings.Int("offset");
            var searchText = QueryStrings.Data("text");
            var dataSet = Get(
                searchText: searchText,
                dataTableName: "SearchResults",
                offset: offset,
                pageSize: Parameters.General.SearchPageSize);
            var results = dataSet?.Tables["SearchResults"].AsEnumerable();
            var res = new ResponseCollection();
            return offset == 0
                ? res
                    .ReplaceAll(
                        "#MainContainer",
                        MainContainer(
                            text: searchText,
                            offset: 0,
                            results: results,
                            count: Rds.Count(dataSet)))
                    .Focus("#Search")
                    .ToJson()
                : res
                    .Append(
                        "#SearchResults",
                        new HtmlBuilder().Results(
                            text: searchText, offset: offset, results: results))
                    .Val(
                        "#SearchOffset",
                        (results != null &&
                        results.Any() &&
                        results.Count() == Parameters.General.SearchPageSize
                            ? offset + Parameters.General.SearchPageSize
                            : -1).ToString())
                    .ToJson();
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
                ss: new SiteSettings(),
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                referenceType: "SearchIndexes",
                title: string.Empty,
                useNavigationMenu: true,
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
            if (results?.Any() == true)
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
                                href = Locations.ItemIndex(referenceId);
                                break;
                            default:
                                href = Locations.ItemEdit(referenceId);
                                break;
                        }
                        href += "?back=1";
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
        public static SqlSelect Select(
            SiteSettings.SearchTypes? searchType, string searchText, IEnumerable<long> siteIdList)
        {
            switch (searchType)
            {
                case SiteSettings.SearchTypes.MatchInFrontOfTitle:
                    return Select(searchText, siteIdList, forward: true);
                case SiteSettings.SearchTypes.BroadMatchOfTitle:
                    return Select(searchText, siteIdList, forward: false);
                default:
                    switch (Parameters.Search.Provider)
                    {
                        case "FullText":
                            var words = Words(searchText);
                            if (words?.Any() != true) return null;
                            return SelectByFullText(
                                column: Rds.ItemsColumn().ReferenceId(),
                                orderBy: null,
                                siteIdList: siteIdList,
                                words: words);
                        default:
                            var searchIndexes = searchText.SearchIndexes();
                            if (searchIndexes.Count() == 0) return null;
                            return SelectBySearchIndexes(
                                searchIndexes: searchIndexes,
                                column: Rds.SearchIndexesColumn().ReferenceId(),
                                orderBy: null,
                                siteIdList: siteIdList);
                    }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlSelect Select(
            string searchText, IEnumerable<long> siteIdList, bool forward)
        {
            return Rds.SelectItems(
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere()
                    .SiteId_In(siteIdList)
                    .SqlWhereLike(
                        searchText,
                        Rds.Items_Title_WhereLike(forward: forward)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static DataSet Get(
            string searchText, string dataTableName, int offset, int pageSize)
        {
            switch (Parameters.Search.Provider)
            {
                case "FullText":
                    return Get(
                        searchText: searchText,
                        dataTableName: dataTableName,
                        offset: offset,
                        pageSize: pageSize,
                        countRecord: offset == 0);
                default:
                    return Get(
                        searchIndexes: searchText.SearchIndexes(),
                        column: Rds.SearchIndexesColumn()
                            .ReferenceId()
                            .ReferenceType()
                            .Priority(function: Sqls.Functions.Sum)
                            .SearchIndexesCount(),
                        dataTableName: dataTableName,
                        offset: offset,
                        pageSize: pageSize,
                        countRecord: offset == 0);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static DataSet Get(
            string searchText,
            IEnumerable<long> siteIdList = null,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            var words = Words(searchText);
            if (words?.Any() != true) return null;
            return Rds.ExecuteDataSet(statements:
                SelectByFullText(
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType(),
                    orderBy: Rds.ItemsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc),
                    siteIdList: siteIdList,
                    dataTableName: dataTableName,
                    words: words,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<string> Words(string searchText)
        {
            return searchText?
                .Replace("　", " ")
                .Replace("\"", " ")
                .Replace("'", "’")
                .Trim()
                .Split(' ')
                .Where(o => o != string.Empty)
                .Distinct()
                .Select(o => FullTextClause(o))
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlSelect SelectByFullText(
            SqlColumnCollection column,
            SqlOrderByCollection orderBy,
            IEnumerable<long> siteIdList,
            List<string> words,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            string paramName = "@Words_Param")
        {
            return Rds.SelectItems(
                dataTableName: dataTableName,
                column: column,
                join: new SqlJoinCollection(
                    new SqlJoin(
                        tableBracket: "[Sites]",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                where: Rds.ItemsWhere()
                    .Add(raw: FullTextWhere())
                    .Add(
                        raw: Def.Sql.CanRead,
                        _using: !Permissions.HasPrivilege())
                    .Add(
                        raw: "[Items].[SiteId] in ({0})".Params(siteIdList?.Join()),
                        _using: siteIdList?.Any() == true),
                param: Rds.ItemsParam()
                    .Add(name: "SearchText", value: words.Join(" and ")),
                orderBy: orderBy,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <returns></returns>
        private static string FullTextWhere()
        {
            var raw = "contains([FullText], @SearchText_Param#CommandCount#)";
            return Parameters.Search.SearchDocuments
                ? $"(({raw}) or (exists(select * from [Binaries] where [Binaries].[ReferenceId]=[Items].[ReferenceId] and contains([Bin], @SearchText_Param#CommandCount#))))"
                : raw;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string FullTextClause(string word)
        {
            var data = new List<string> { word };
            var katakana = CSharp.Japanese.Kanaxs.KanaEx.ToKatakana(word);
            var hiragana = CSharp.Japanese.Kanaxs.KanaEx.ToHiragana(word);
            if (word != katakana) data.Add(katakana);
            if (word != hiragana) data.Add(hiragana);
            return "(" + data
                .SelectMany(o => new List<string> { o, o + "*" })
                .Distinct()
                .Select(o => "\"" + o + "\"")
                .Join(" or ") + ")";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DataSet Get(
            IEnumerable<string> searchIndexes,
            SqlColumnCollection column,
            IEnumerable<long> siteIdList = null,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            if (searchIndexes.Count() == 0) return null;
            return Rds.ExecuteDataSet(statements:
                SelectBySearchIndexes(
                    searchIndexes: searchIndexes,
                    column: column,
                    orderBy: Rds.SearchIndexesOrderBy()
                        .SearchIndexesCount(SqlOrderBy.Types.desc)
                        .Priority(function: Sqls.Functions.Sum)
                        .UpdatedTime(SqlOrderBy.Types.desc, function: Sqls.Functions.Max),
                    siteIdList: siteIdList,
                    dataTableName: dataTableName,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlSelect SelectBySearchIndexes(
            IEnumerable<string> searchIndexes,
            SqlColumnCollection column,
            SqlOrderByCollection orderBy,
            IEnumerable<long> siteIdList,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            return Rds.SelectSearchIndexes(
                dataTableName: dataTableName,
                column: column,
                join: Rds.SearchIndexesJoinDefault(),
                where: Rds.SearchIndexesWhere()
                    .Word(searchIndexes, multiParamOperator: " or ")
                    .Add(
                        raw: Def.Sql.CanRead,
                        _using: !Permissions.HasPrivilege())
                    .Add(
                        raw: "[Items].[SiteId] in ({0})".Params(siteIdList?.Join()),
                        _using: siteIdList?.Any() == true),
                groupBy: Rds.SearchIndexesGroupBy()
                    .ReferenceId()
                    .ReferenceType(),
                having: Rds.SearchIndexesHaving()
                    .SearchIndexesCount(
                        Concordance(searchIndexes),
                        _operator: ">="),
                orderBy: orderBy,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static decimal Concordance(IEnumerable<string> searchIndexes)
        {
            return Math.Ceiling(searchIndexes.Count() * Parameters.General.SearchConcordanceRate);
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void CreateInBackground()
        {
            if (Parameters.BackgroundTask.Enabled)
            {
                var hash = new Dictionary<long, SiteModel>();
                Rds.ExecuteTable(statements: Rds.SelectItems(
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .SiteId()
                        .Updator()
                        .UpdatedTime(),
                    where: Rds.ItemsWhere().Add(
                        raw: "[SearchIndexCreatedTime] is null or [SearchIndexCreatedTime]<>[UpdatedTime]"),
                    top: Parameters.BackgroundTask.CreateSearchIndexLot))
                        .AsEnumerable()
                        .Select(o => new
                        {
                            ReferenceId = o["ReferenceId"].ToLong(),
                            SiteId = o["SiteId"].ToLong(),
                            Updator = o["Updator"].ToInt(),
                            UpdatedTime = o.Field<DateTime>("UpdatedTime")
                                .ToString("yyyy/M/d H:m:s.fff")
                        })
                        .ForEach(data =>
                        {
                            var siteModel = hash.Get(data.SiteId) ??
                                new SiteModel().Get(where: Rds.SitesWhere().SiteId(data.SiteId));
                            Sessions.Set(siteModel.TenantId, data.Updator);
                            if (!hash.ContainsKey(data.SiteId))
                            {
                                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                                    siteModel, siteModel.SiteId);
                                hash.Add(data.SiteId, siteModel);
                            }
                            Libraries.Search.Indexes.Create(
                                siteModel.SiteSettings, data.ReferenceId, force: true);
                            Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                                where: Rds.ItemsWhere().ReferenceId(data.ReferenceId),
                                param: Rds.ItemsParam().SearchIndexCreatedTime(data.UpdatedTime),
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false));
                        });
            }
        }
    }
}
