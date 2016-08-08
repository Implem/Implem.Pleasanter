using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, SearchIndexModel searchIndexModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: searchIndexModel.Ver);
                case "Comments": return hb.Td(column: column, value: searchIndexModel.Comments);
                case "Creator": return hb.Td(column: column, value: searchIndexModel.Creator);
                case "Updator": return hb.Td(column: column, value: searchIndexModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: searchIndexModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: searchIndexModel.UpdatedTime);
                default: return hb;
            }
        }

        public static ResponseCollection FormResponse(
            this ResponseCollection responseCollection, SearchIndexModel searchIndexModel)
        {
            Forms.All().Keys.ForEach(key =>
            {
                switch (key)
                {
                    case "SearchIndexes_Priority": responseCollection.Val("#" + key, searchIndexModel.Priority.ToControl(searchIndexModel.SiteSettings.AllColumn("Priority"), searchIndexModel.PermissionType)); break;
                    default: break;
                }
            });
            return responseCollection;
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
                count: Rds.Count(dataSet),
                byRest: false).ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AjaxSearch()
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
                    .PushState(
                        "Search",
                        Navigations.Get("Items", "Search?text=" + Url.Encode(text)),
                        _using: !QueryStrings.Bool("reload"))
                    .ReplaceAll(
                        "#MainContainer",
                        MainContainer(
                            text: text,
                            offset: 0,
                            results: results,
                            count: Rds.Count(dataSet),
                            byRest: true))
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
            int count,
            bool byRest)
        {
            var hb = new HtmlBuilder();
            var searchIndexes = text.SearchIndexes();
            return hb.Template(
                siteId: 0,
                referenceType: "SearchIndexes",
                title: string.Empty,
                permissionType: Permissions.Types.Read,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: true,
                useNavigationButtons: false,
                useTitle: false,
                useSearch: false,
                useBreadcrumb: false,
                byRest: byRest,
                action: () => hb
                    .Div(id: "SearchResults", css: "search-results", action: () => hb
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
                    controlCss: " w600",
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
            results?.ForEach(dataRow => Libraries.Search.Responses.Get(hb, dataRow, text));
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
                    .Title()
                    .Subset()
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
                        .ReferenceType()
                        .Title()
                        .Subset(),
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
    }
}
