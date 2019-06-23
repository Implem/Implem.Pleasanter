using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
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
            Context context, EnumerableRowCollection<DataRow> dataRows)
        {
            var statements = new List<SqlStatement>();
            if (dataRows.Any(o => o.String("ReferenceType") == "Sites"))
            {
                statements.Add(Rds.SelectSites(
                    dataTableName: "Sites",
                    column: Rds.SitesColumn()
                        .ParentId(_as: "SiteId")
                        .SiteId(_as: "Id")
                        .Body()
                        .Items_Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Items]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[ReferenceId]=[Sites].[SiteId]")),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId_In(dataRows
                            .Where(o => o.String("ReferenceType") == "Sites")
                            .Select(o => o.Long("ReferenceId")))));
            }
            if (dataRows.Any(o => o.String("ReferenceType") == "Issues"))
            {
                statements.Add(Rds.SelectIssues(
                    dataTableName: "Issues",
                    column: Rds.IssuesColumn()
                        .SiteId()
                        .IssueId(_as: "Id")
                        .Body()
                        .Items_Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Items]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[ReferenceId]=[Issues].[IssueId]")),
                    where: Rds.IssuesWhere()
                        .IssueId_In(dataRows
                            .Where(o => o.String("ReferenceType") == "Issues")
                            .Select(o => o.Long("ReferenceId")))));
            }
            if (dataRows.Any(o => o.String("ReferenceType") == "Results"))
            {
                statements.Add(Rds.SelectResults(
                    dataTableName: "Results",
                    column: Rds.ResultsColumn()
                        .SiteId()
                        .ResultId(_as: "Id")
                        .Body()
                        .Items_Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Items]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[ReferenceId]=[Results].[ResultId]")),
                    where: Rds.ResultsWhere()
                        .ResultId_In(dataRows
                            .Where(o => o.String("ReferenceType") == "Results")
                            .Select(o => o.Long("ReferenceId")))));
            }
            if (dataRows.Any(o => o.String("ReferenceType") == "Wikis"))
            {
                statements.Add(Rds.SelectWikis(
                    dataTableName: "Wikis",
                    column: Rds.WikisColumn()
                        .SiteId()
                        .WikiId(_as: "Id")
                        .Body()
                        .Items_Title(),
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Items]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[ReferenceId]=[Wikis].[WikiId]")),
                    where: Rds.WikisWhere()
                        .WikiId_In(dataRows
                            .Where(o => o.String("ReferenceType") == "Wikis")
                            .Select(o => o.Long("ReferenceId")))));
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
            var dataSet = Get(
                context: context,
                searchText: context.QueryStrings.Data("text"),
                dataTableName: "SearchResults",
                offset: context.QueryStrings.Int("offset"),
                pageSize: Parameters.Search.PageSize);
            return MainContainer(
                context: context,
                text: context.QueryStrings.Data("text"),
                offset: 0,
                results: dataSet?.Tables["SearchResults"].AsEnumerable(),
                count: Rds.Count(dataSet)).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SearchJson(Context context)
        {
            var offset = context.QueryStrings.Int("offset");
            var searchText = context.QueryStrings.Data("text");
            var dataSet = Get(
                context: context,
                searchText: searchText,
                dataTableName: "SearchResults",
                offset: offset,
                pageSize: Parameters.Search.PageSize);
            var dataRows = dataSet?.Tables["SearchResults"].AsEnumerable();
            var res = new ResponseCollection();
            return offset == 0
                ? res
                    .ReplaceAll(
                        "#MainContainer",
                        MainContainer(
                            context: context,
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
                            text: searchText,
                            offset: offset,
                            dataRows: dataRows))
                    .Val(
                        "#SearchOffset",
                        (dataRows != null &&
                        dataRows.Any() &&
                        dataRows.Count() == Parameters.Search.PageSize
                            ? offset + Parameters.Search.PageSize
                            : -1).ToString())
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder MainContainer(
            Context context,
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
                view: null,
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
                        .Command(
                            context: context,
                            text: text)
                        .Count(
                            context: context,
                            count: count)
                        .Results(
                            context: context,
                            text: text,
                            offset: offset,
                            dataRows: results))
                    .Hidden(
                        controlId: "SearchOffset",
                        value: Parameters.Search.PageSize.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Command(this HtmlBuilder hb, Context context, string text)
        {
            return hb.Div(css: "command-center", action: () => hb
                .TextBox(
                    controlId: "Search",
                    controlCss: " w600 focus",
                    text: text,
                    placeholder: Displays.Search(context: context)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Count(this HtmlBuilder hb, Context context, int count)
        {
            return hb.Div(css: "count", action: () => hb
                .Span(css: "label", action: () => hb
                    .Text(text: Displays.Quantity(context: context)))
                .Span(css: "data", action: () => hb
                    .Text(text: count.ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Results(
            this HtmlBuilder hb,
            Context context,
            string text,
            int offset,
            EnumerableRowCollection<DataRow> dataRows)
        {
            if (dataRows?.Any() == true)
            {
                var dataSet = ResultContents(context: context, dataRows: dataRows);
                dataRows.ForEach(result =>
                {
                    var referenceType = result.String("ReferenceType");
                    var referenceId = result.Long("ReferenceId");
                    var dataRow = dataSet.Tables[referenceType]
                        .AsEnumerable()
                        .FirstOrDefault(o => o["Id"].ToLong() == referenceId);
                    if (dataRow != null)
                    {
                        var href = string.Empty;
                        switch (referenceType)
                        {
                            case "Sites":
                                href = Locations.ItemIndex(
                                    context: context,
                                    id: referenceId);
                                break;
                            default:
                                href = Locations.ItemEdit(
                                    context: context,
                                    id: referenceId);
                                break;
                        }
                        href += "?back=1";
                        hb.Section(
                            attributes: new HtmlAttributes()
                                .Class("result")
                                .Add("data-href", href),
                            action: () => hb
                                .Breadcrumb(
                                    context: context,
                                    ss: new SiteSettings()
                                    {
                                        SiteId = dataRow.Long("SiteId")
                                    })
                                .H(number: 3, action: () => hb
                                     .A(
                                         href: href,
                                         text: dataRow.String("Title")))
                                .P(action: () => hb
                                    .Text(text: dataRow.String("Body"))));
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
            SiteSettings ss,
            string searchText,
            IEnumerable<long> siteIdList)
        {
            if (ss != null && ss.TableType != Sqls.TableTypes.Normal)
            {
                return Select(
                    ss: ss,
                    searchText: searchText,
                    siteIdList: siteIdList,
                    like: Rds.Items_FullText_WhereLike(forward: false));
            }
            switch (ss?.SearchType)
            {
                case SiteSettings.SearchTypes.PartialMatch:
                    return Select(
                        ss: ss,
                        searchText: searchText,
                        siteIdList: siteIdList,
                        like: Rds.Items_FullText_WhereLike(forward: false));
                case SiteSettings.SearchTypes.MatchInFrontOfTitle:
                    return Select(
                        ss: ss,
                        searchText: searchText,
                        siteIdList: siteIdList,
                        like: Rds.Items_Title_WhereLike(forward: true));
                case SiteSettings.SearchTypes.BroadMatchOfTitle:
                    return Select(
                        ss: ss,
                        searchText: searchText,
                        siteIdList: siteIdList,
                        like: Rds.Items_Title_WhereLike(forward: false));
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
            SiteSettings ss, string searchText, IEnumerable<long> siteIdList, string like)
        {
            return Rds.SelectItems(
                tableType: ss.TableType,
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere()
                    .SiteId_In(siteIdList)
                    .SqlWhereLike(
                        tableName: null,
                        name: "SearchText",
                        searchText: searchText,
                        clauseCollection: like.ToSingleList()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection FullTextWhere(
            this SqlWhereCollection where,
            SiteSettings ss,
            string tableName,
            string searchText)
        {
            var name = Strings.NewGuid();
            if (ss != null && ss.TableType != Sqls.TableTypes.Normal)
            {
                return where.Add(Rds.ItemsWhere().SqlWhereLike(
                    tableName: tableName + "_Items",
                    name: name,
                    searchText: searchText,
                    clauseCollection: Rds.Items_FullText_WhereLike(
                        tableName: tableName + "_Items",
                        name: name,
                        forward: false)
                            .ToSingleList()));
            }
            switch (ss?.SearchType)
            {
                case SiteSettings.SearchTypes.PartialMatch:
                    return where.Add(Rds.ItemsWhere().SqlWhereLike(
                        tableName: tableName + "_Items",
                        name: name,
                        searchText: searchText,
                        clauseCollection: Rds.Items_FullText_WhereLike(
                            tableName: tableName + "_Items",
                            name: name,
                            forward: false)
                                .ToSingleList()));
                case SiteSettings.SearchTypes.MatchInFrontOfTitle:
                    return where.Add(Rds.ItemsWhere().SqlWhereLike(
                        tableName: tableName + "_Items",
                        name: name,
                        searchText: searchText,
                        clauseCollection: Rds.Items_Title_WhereLike(
                            tableName: tableName + "_Items",
                            name: name,
                            forward: true)
                                .ToSingleList()));
                case SiteSettings.SearchTypes.BroadMatchOfTitle:
                    return where.Add(Rds.ItemsWhere().SqlWhereLike(
                        tableName: tableName + "_Items",
                        name: name,
                        searchText: searchText,
                        clauseCollection: Rds.Items_Title_WhereLike(
                            tableName: tableName + "_Items",
                            name: name,
                            forward: false)
                                .ToSingleList()));
                default:
                    switch (Parameters.Search.Provider)
                    {
                        case "FullText":
                            var words = Words(searchText);
                            if (words?.Any() != true) return where;
                            where.Add(new SqlWhere(
                                name: name,
                                value: words,
                                raw: FullTextWhere(
                                    words: words,
                                    itemsTableName: tableName + "_Items",
                                    name: name)));
                            return where;
                        default:
                            return where.Add(Rds.ItemsWhere().SqlWhereLike(
                                tableName: tableName + "_Items",
                                name: name,
                                searchText: searchText,
                                clauseCollection: Rds.Items_FullText_WhereLike(
                                    tableName: tableName + "_Items",
                                    name: name,
                                    forward: false)
                                        .ToSingleList()));
                    }
            }
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
            var statements = new List<SqlStatement>
            {
                SelectByFullText(
                    context: context,
                    words: words,
                    column: Rds.ItemsColumn()
                        .ReferenceId()
                        .ReferenceType()
                        .Title(),
                    orderBy: Rds.ItemsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc),
                    siteIdList: siteIdList,
                    dataTableName: dataTableName,
                    offset: offset,
                    pageSize: pageSize)
            };
            if (countRecord)
            {
                statements.Add(SelectByFullText(
                    context: context,
                    words: words,
                    countRecord: true));
            }
            return Rds.ExecuteDataSet(
                context: context,
                statements: statements.ToArray());
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
            List<string> words,
            SqlColumnCollection column = null,
            SqlOrderByCollection orderBy = null,
            IEnumerable<long> siteIdList = null,
            string dataTableName = null,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false)
        {
            return !countRecord
                ? Rds.SelectItems(
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
                            _using: !context.HasPrivilege && !context.Publish)
                        .Add(
                            raw: "[Items].[SiteId] in ({0})".Params(siteIdList?.Join()),
                            _using: siteIdList?.Any() == true),
                    param: FullTextParam(words),
                    orderBy: orderBy,
                    offset: offset,
                    pageSize: pageSize)
                : Rds.SelectCount(
                    tableName: "Items",
                    join: new SqlJoinCollection(
                        new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.ItemsWhere()
                        .Add(raw: FullTextWhere(words))
                        .Add(
                            raw: Def.Sql.CanRead,
                            _using: !context.HasPrivilege && !context.Publish)
                        .Add(
                            raw: "[Items].[SiteId] in ({0})".Params(siteIdList?.Join()),
                            _using: siteIdList?.Any() == true),
                    param: FullTextParam(words));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string FullTextWhere(List<string> words, string itemsTableName = "Items", string name = "SearchText")
        {
            var contains = new List<string>();
            for (var count = 0; count < words.Count(); count++)
            {
                var item = $"(contains([{itemsTableName}].[FullText], @{name}{count}_#CommandCount#))";
                var binary = $"(exists(select * from [Binaries] where [Binaries].[ReferenceId]=[{itemsTableName}].[ReferenceId] and contains([Bin], @{name}{count}_#CommandCount#)))";
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
                .SelectMany(part => new List<string>
                {
                    part,
                    ForwardMatchSearch(part: part)
                })
                .Where(o => o != null)
                .Distinct()
                .Select(o => "\"" + o + "\"")
                .Join(" or ") + ")";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ForwardMatchSearch(string part)
        {
            var separators = "!#$%&()*+,-./:<=>?@[\\]^_`{|}~";
            foreach (var separator in separators)
            {
                if ( part.Split(separator).Any(o => o.RegexExists("^[0-9]$")))
                {
                    return null;
                }
            }
            return part + "*";
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
                pageSize: pageSize);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static decimal Concordance(IEnumerable<string> searchIndexes)
        {
            return Math.Ceiling(searchIndexes.Count() * Parameters.Search.ConcordanceRate);
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
        public static string RebuildSearchIndexes(Context context, SiteModel siteModel)
        {
            siteModel.SiteSettings = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: siteModel.SiteId);
            var ss = siteModel.SiteSettings.SiteSettingsOnUpdate(context: context);
            var invalid = SiteValidators.OnUpdating(
                context: context,
                ss: ss,
                siteModel: siteModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default: return invalid.Type.MessageJson(context: context);
            }
            RebuildSearchIndexes(
                context: context,
                siteId: siteModel.SiteId);
            return new ResponseCollection()
                .Message(Messages.RebuildingCompleted(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void RebuildSearchIndexes(Context context, long siteId = -1)
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
                        joinExpression: "[Users].[UserId]=[Items].[Updator]")),
                    where: siteId > 0
                        ? Rds.ItemsWhere().SiteId(siteId)
                        : Rds.ItemsWhere().Add(raw: new List<string>()
                        {
                            "[Items].[SearchIndexCreatedTime] is null",
                            "[Items].[SearchIndexCreatedTime]<>[Items].[UpdatedTime]"
                        }.Join(" or ")),
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
                            var currentContext = new Context(
                                tenantId: data.TenantId,
                                deptId: data.DeptId,
                                userId: data.UserId);
                            var siteModel = hash.Get(data.SiteId) ??
                                new SiteModel().Get(
                                    context: currentContext,
                                    where: Rds.SitesWhere().SiteId(data.SiteId));
                            if (!hash.ContainsKey(data.SiteId))
                            {
                                siteModel.SiteSettings = SiteSettingsUtilities.Get(
                                    context: currentContext,
                                    siteModel: siteModel,
                                    referenceId: siteModel.SiteId);
                                hash.Add(data.SiteId, siteModel);
                            }
                            Libraries.Search.Indexes.Create(
                                context: currentContext,
                                ss: siteModel.SiteSettings,
                                id: data.ReferenceId,
                                force: true);
                            Rds.ExecuteNonQuery(
                                context: currentContext,
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
