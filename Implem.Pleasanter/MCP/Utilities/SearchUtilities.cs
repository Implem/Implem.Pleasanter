using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;

namespace Implem.Pleasanter.MCP.Utilities
{
    public static class SearchUtilities
    {
        public const string DefaultSearchTypeName = "ExactMatch";

        public enum SearchTypes
        {
            ExactMatch,
            PartialMatch,
            ForwardMatch
        }

        public static List<long> GetChildSiteIds(
            Context context,
            long parentId)
        {
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SiteId(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .ParentId(parentId)));

            return dataTable
                .AsEnumerable()
                .Select(row => row.Field<long>("SiteId"))
                .ToList();
        }

        public static SearchTypes ParseSearchType(string searchType)
        {
            if (Enum.TryParse<SearchTypes>(
                searchType,
                ignoreCase: true,
                out var result))
            {
                return result;
            }

            return SearchTypes.ExactMatch;
        }

        public static ContentResultInheritance SearchAndCreateResultForSites(
            Context context,
            SearchTypes searchType,
            string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return CreateGetByNameResult(dataArray: new JsonArray());
            }

            var siteCollection =
                SearchSites(
                    context: context,
                    searchType: searchType,
                    searchValue: searchValue);

            var dataArray =
                CreateJsonArray(
                    collection: siteCollection,
                    idName: "SiteId",
                    valueName: "Title",
                    idSelector: site => site.SiteId,
                    valueSelector: site => site.Title.Value);

            return CreateGetByNameResult(dataArray: dataArray);
        }

        public static ContentResultInheritance SearchAndCreateResultForUsers(
            Context context,
            SearchTypes searchType,
            string searchValue,
            SiteSettings ss)
        {
            var userCollection =
                SearchUsers(
                    context: context,
                    searchType: searchType,
                    searchValue: searchValue,
                    ss: ss);

            var dataArray =
                CreateJsonArray(
                    collection: userCollection,
                    idName: "UserId",
                    valueName: "Name",
                    idSelector: user => user.UserId,
                    valueSelector: user => user.Name);

            return CreateGetByNameResult(dataArray: dataArray);
        }

        private static (string pattern, string searchOperator) BuildSearchCondition(
            Context context,
            SearchTypes searchType,
            string searchValue)
        {
            var escapedValue = context.Sqls.EscapeValue(searchValue);
            var likeOperator = $" like {{0}}{context.Sqls.Escape}";

            return searchType switch
            {
                SearchTypes.PartialMatch
                    => ($"%{escapedValue}%", likeOperator),

                SearchTypes.ForwardMatch
                    => ($"{escapedValue}%", likeOperator),

                _ => (searchValue, null)
            };
        }

        private static ContentResultInheritance CreateGetByNameResult(JsonArray dataArray)
        {
            var jsonObject = new JsonObject
            {
                ["StatusCode"] = (int)HttpStatusCode.OK,
                ["Response"] = new JsonObject
                {
                    ["TotalCount"] = dataArray.Count,
                    ["Data"] = dataArray
                }
            };

            return ApiResults.Get(
                apiResponse: jsonObject.ToJsonString(),
                statusCode: (int)HttpStatusCode.OK);
        }

        private static JsonArray CreateJsonArray<T>(
            IEnumerable<T> collection,
            string idName,
            string valueName,
            Func<T, object> idSelector,
            Func<T, object> valueSelector)
        {
            return new JsonArray(
                collection.Select(
                    item => new JsonObject
                    {
                        [idName] = JsonValue.Create(value: idSelector(item)),
                        [valueName] = JsonValue.Create(value: valueSelector(item))
                    }).ToArray());
        }

        private static SiteCollection SearchSites(
            Context context,
            SearchTypes searchType,
            string searchValue)
        {
            var where = Rds.SitesWhere()
                .TenantId(value: context.TenantId);

            if (!string.IsNullOrEmpty(searchValue))
            {
                var (pattern, searchOperator) =
                    BuildSearchCondition(
                        context: context,
                        searchType: searchType,
                        searchValue: searchValue);

                where = (searchOperator == null)
                    ? where.Title(
                        value: pattern)
                    : where.Title(
                        value: pattern,
                        _operator: searchOperator);
            }

            return new SiteCollection(
                context: context,
                where: where);
        }

        private static UserCollection SearchUsers(
            Context context,
            SearchTypes searchType,
            string searchValue,
            SiteSettings ss)
        {
            var where = Rds.UsersWhere()
                .TenantId(value: context.TenantId);

            if (!string.IsNullOrEmpty(searchValue))
            {
                var (pattern, searchOperator) =
                    BuildSearchCondition(
                        context: context,
                        searchType: searchType,
                        searchValue: searchValue);

                where = (searchOperator == null)
                    ? where.Name(
                        value: pattern)
                    : where.Name(
                        value: pattern,
                        _operator: searchOperator);
            }

            return new UserCollection(
                context: context,
                where: where,
                ss: ss);
        }
    }
}
