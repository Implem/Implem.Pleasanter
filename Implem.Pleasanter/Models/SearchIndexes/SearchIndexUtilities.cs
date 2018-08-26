using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
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
        private static DataSet ResultContents(
            Context context, SiteSettings ss, EnumerableRowCollection<DataRow> dataRows)
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
                        .TenantId(context.TenantId)
                        .SiteId_In(dataRows
                            .Where(o => o.String("ReferenceType") == "Sites")
                            .Select(o => o.Long("ReferenceId")))));
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
            return Rds.ExecuteDataSet(
                context: context,
                statements: statements.ToArray());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Search(Context context)
        {
            var ss = new SiteSettings();
            var dataSet = Get(
                context: context,
                searchText: QueryStrings.Data("text"),
                dataTableName: "SearchResults",
                offset: QueryStrings.Int("offset"),
                pageSize: Parameters.General.SearchPageSize);
            return MainContainer(
                context: context,
                ss: ss,
                text: QueryStrings.Data("text"),
                offset: 0,
                results: dataSet?.Tables["SearchResults"].AsEnumerable(),
                count: Rds.Count(dataSet)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchJson(Context context)
        {
            var ss = new SiteSettings();
            var offset = QueryStrings.Int("offset");
            var searchText = QueryStrings.Data("text");
            var dataSet = Get(
                context: context,
                searchText: searchText,
                dataTableName: "SearchResults",
                offset: offset,
                pageSize: Parameters.General.SearchPageSize);
            var dataRows = dataSet?.Tables["SearchResults"].AsEnumerable();
            var res = new ResponseCollection();
            return offset == 0
                ? res
                    .ReplaceAll(
                        "#MainContainer",
                        MainContainer(
                            context: context,
                            ss: ss,
                            text: searchText,
                            offset: 0,
                            results: dataRows,
                            count: Rds.Count(dataSet)))
                    .Focus("#Search")
                    .ToJson()
                : res
                    .Append(
                        "#SearchResults",
                        new HtmlBuilder().Results(
                            context: context,
                            ss: ss,
                            text: searchText,
                            offset: offset,
                            dataRows: dataRows))
                    .Val(
                        "#SearchOffset",
                        (dataRows != null &&
                        dataRows.Any() &&
                        dataRows.Count() == Parameters.General.SearchPageSize
                            ? offset + Parameters.General.SearchPageSize
                            : -1).ToString())
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainContainer(
            Context context,
            SiteSettings ss,
            string text,
            int offset,
            EnumerableRowCollection<DataRow> results,
            int count)
        {
            var hb = new HtmlBuilder();
            var searchIndexes = text.SearchIndexes(context: context);
            return hb.Template(
                context: context,
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
                        .Results(
                            context: context,
                            ss: ss,
                            text: text,
                            offset: offset,
                            dataRows: results))
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
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string text,
            int offset,
            EnumerableRowCollection<DataRow> dataRows)
        {
            if (dataRows?.Any() == true)
            {
                var dataSet = ResultContents(context: context, ss: ss, dataRows: dataRows);
                dataRows.ForEach(result =>
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
                                .Breadcrumb(context: context, ss: ss)
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
            Context context,
            SiteSettings.SearchTypes? searchType,
            string searchText,
            IEnumerable<long> siteIdList)
        {
            switch (searchType)
            {
                case SiteSettings.SearchTypes.PartialMatch:
                    return Select(
                        searchText,
                        siteIdList,
                        Rds.Items_FullText_WhereLike(forward: false));
                case SiteSettings.SearchTypes.MatchInFrontOfTitle:
                    return Select(
                        searchText,
                        siteIdList,
                        Rds.Items_Title_WhereLike(forward: true));
                case SiteSettings.SearchTypes.BroadMatchOfTitle:
                    return Select(
                        searchText,
                        siteIdList,
                        Rds.Items_Title_WhereLike(forward: false));
                default:
                    switch (Parameters.Search.Provider)
                    {
                        case "FullText":
                            var words = Words(searchText);
                            if (words?.Any() != true) return null;
                            return SelectByFullText(
                                context: context,
                                column: Rds.ItemsColumn().ReferenceId(),
                                orderBy: null,
                                siteIdList: siteIdList,
                                words: words);
                        default:
                            var searchIndexes = searchText.SearchIndexes(context: context);
                            if (searchIndexes.Count() == 0) return null;
                            return SelectBySearchIndexes(
                                context: context,
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
            string searchText, IEnumerable<long> siteIdList, string like)
        {
            return Rds.SelectItems(
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere()
                    .SiteId_In(siteIdList)
                    .SqlWhereLike(
                        name: "SearchText",
                        searchText: searchText,
                        clauseCollection: like.ToSingleList()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static DataSet Get(
            Context context,
            string searchText,
            string dataTableName,
            int offset,
            int pageSize)
        {
            switch (Parameters.Search.Provider)
            {
                case "FullText":
                    return Get(
                        context: context,
                        searchText: searchText,
                        dataTableName: dataTableName,
                        offset: offset,
                        pageSize: pageSize,
                        countRecord: offset == 0);
                default:
                    return Get(
                        context: context,
                        searchIndexes: searchText.SearchIndexes(context: context),
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
            Context context,
            string searchText,
            IEnumerable<long> siteIdList = null,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            var words = Words(searchText);
            if (words?.Any() != true) return null;
            return Rds.ExecuteDataSet(
                context: context,
                statements: SelectByFullText(
                    context: context,
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
            Context context,
            SqlColumnCollection column,
            SqlOrderByCollection orderBy,
            IEnumerable<long> siteIdList,
            List<string> words,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
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
                    .Add(raw: FullTextWhere(words))
                    .Add(
                        raw: Def.Sql.CanRead,
                        _using: !context.HasPrivilege)
                    .Add(
                        raw: "[Items].[SiteId] in ({0})".Params(siteIdList?.Join()),
                        _using: siteIdList?.Any() == true),
                param: FullTextParam(words),
                orderBy: orderBy,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string FullTextWhere(List<string> words)
        {
            var contains = new List<string>();
            for (var count = 0; count < words.Count(); count++)
            {
                var item = $"(contains([FullText], @SearchText{count}_Param#CommandCount#))";
                var binary = $"(exists(select * from [Binaries] where [Binaries].[ReferenceId]=[Items].[ReferenceId] and contains([Bin], @SearchText{count}_Param#CommandCount#)))";
                contains.Add(Parameters.Search.SearchDocuments
                    ? $"({item} or {binary})"
                    : item);
            }
            return contains.Join(" and ");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Rds.ItemsParamCollection FullTextParam(List<string> words)
        {
            var param = Rds.ItemsParam();
            words
                .Select((o, i) => new { Word = o, Index = i })
                .ForEach(data =>
                    param.Add(
                        name: "SearchText" + data.Index,
                        value: data.Word));
            return param;
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
            Context context,
            IEnumerable<string> searchIndexes,
            SqlColumnCollection column,
            IEnumerable<long> siteIdList = null,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            if (searchIndexes.Count() == 0) return null;
            return Rds.ExecuteDataSet(
                context: context,
                statements: SelectBySearchIndexes(
                    context: context,
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
            Context context,
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
                        _using: !context.HasPrivilege)
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
        public static void Maintain(Context context)
        {
            if ((DateTime.Now - Applications.SearchIndexesMaintenanceDate).Days > 0)
            {
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteSearchIndexes(
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
        public static void CreateInBackground(Context context)
        {
            if (Parameters.BackgroundTask.Enabled)
            {
                var hash = new Dictionary<long, SiteModel>();
                Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn()
                            .ReferenceId()
                            .SiteId()
                            .UpdatedTime()
                            .Users_TenantId()
                            .Users_DeptId()
                            .Users_UserId(),
                        join: Rds.ItemsJoinDefault().Add(new SqlJoin(
                            tableBracket: "[Users]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "")),
                        where: Rds.ItemsWhere().Add(
                            raw: "[SearchIndexCreatedTime] is null or [SearchIndexCreatedTime]<>[UpdatedTime]"),
                        top: Parameters.BackgroundTask.CreateSearchIndexLot))
                            .AsEnumerable()
                            .Select(o => new
                            {
                                ReferenceId = o["ReferenceId"].ToLong(),
                                SiteId = o["SiteId"].ToLong(),
                                UpdatedTime = o.Field<DateTime>("UpdatedTime")
                                    .ToString("yyyy/M/d H:m:s.fff"),
                                TenantId = o.Int("TenantId"),
                                DeptId = o.Int("DeptId"),
                                UserId = o.Int("UserId")
                            })
                            .ForEach(data =>
                            {
                                var otherContext = new Context(
                                    tenantId: data.TenantId,
                                    deptId: data.DeptId,
                                    userId: data.UserId);
                                var siteModel = hash.Get(data.SiteId) ??
                                    new SiteModel().Get(
                                        context: otherContext,
                                        where: Rds.SitesWhere().SiteId(data.SiteId));
                                if (!hash.ContainsKey(data.SiteId))
                                {
                                    siteModel.SiteSettings = SiteSettingsUtilities.Get(
                                        context: otherContext,
                                        siteModel: siteModel,
                                        referenceId: siteModel.SiteId);
                                    hash.Add(data.SiteId, siteModel);
                                }
                                Libraries.Search.Indexes.Create(
                                    context: otherContext,
                                    ss: siteModel.SiteSettings,
                                    id: data.ReferenceId,
                                    force: true);
                                Rds.ExecuteNonQuery(
                                    context: otherContext,
                                    statements: Rds.UpdateItems(
                                        where: Rds.ItemsWhere()
                                            .ReferenceId(data.ReferenceId),
                                        param: Rds.ItemsParam()
                                            .SearchIndexCreatedTime(data.UpdatedTime),
                                        addUpdatorParam: false,
                                        addUpdatedTimeParam: false));
                            });
            }
        }
    }
}
