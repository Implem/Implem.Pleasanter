using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewFilters
    {
        public static SqlWhereCollection Get(
            SiteSettings ss,
            string tableName,
            FormData formData,
            SqlWhereCollection where)
        {
            return where
                .Generals(ss: ss, tableName: tableName, formData: formData)
                .Columns(ss: ss, tableName: tableName, formData: formData)
                .Search(tableName: tableName, formData: formData, siteId: ss.SiteId);
        }

        private static SqlWhereCollection Generals(
            this SqlWhereCollection sqlWhereCollection,
            SiteSettings ss,
            string tableName,
            FormData formData)
        {
            var tableBracket = "[" + tableName + "]";
            formData.Where(o => o.Value.Value.ToBool()).Select(o => o.Key).ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "DataViewFilters_Incomplete":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[Status]" },
                            name: "_U",
                            _operator: "<{0}".Params(Parameters.General.CompletionCode));
                        break;
                    case "DataViewFilters_Own":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[Manager]", "[t0].[Owner]" },
                            name: "_U",
                            value: Sessions.UserId());
                        break;
                    case "DataViewFilters_NearCompletionTime":
                        sqlWhereCollection.Add(
                            columnBrackets: new string[] { "[t0].[CompletionTime]" },
                            _operator: " between '{0}' and '{1}'".Params(
                                DateTime.Now.ToLocal().Date
                                    .AddDays(ss.NearCompletionTimeBeforeDays.ToInt() * (-1)),
                                DateTime.Now.ToLocal().Date
                                    .AddDays(ss.NearCompletionTimeAfterDays.ToInt() + 1)
                                    .AddMilliseconds(-1)
                                    .ToString("yyyy/M/d H:m:s.fff")));
                        break;
                    case "DataViewFilters_Delay":
                        sqlWhereCollection
                            .Add(
                                columnBrackets: new string[] { "[t0].[Status]" },
                                name: "_U",
                                _operator: "<{0}".Params(Parameters.General.CompletionCode))
                            .Add(
                                columnBrackets: new string[] { "[t0].[ProgressRate]" },
                                _operator: "<",
                                raw: Def.Sql.ProgressRateDelay
                                    .Replace("#TableName#", tableName));
                        break;
                    case "DataViewFilters_Overdue":
                        sqlWhereCollection
                            .Add(
                                columnBrackets: new string[] { "[t0].[Status]" },
                                name: "_U",
                                _operator: "<{0}".Params(Parameters.General.CompletionCode))
                            .Add(
                                columnBrackets: new string[] { "[t0].[CompletionTime]" },
                                _operator: "<getdate()");
                        break;
                    default: break;
                }
            });
            return sqlWhereCollection;
        }

        public static FormData SessionFormData(long siteId = 0)
        {
            var key = "DataView_" + (siteId == 0
                ? Pages.Key()
                : siteId.ToString());
            if (HttpContext.Current.Session[key] != null)
            {
                return (HttpContext.Current.Session[key] as FormData)
                    .Update(HttpContext.Current.Request.Form)
                    .RemoveIfEmpty();
            }
            else
            {
                var formData = new FormData(HttpContext.Current.Request.Form);
                HttpContext.Current.Session[key] = formData;
                return formData.RemoveIfEmpty();
            }
        }

        private static SqlWhereCollection Columns(
            this SqlWhereCollection where,
            SiteSettings ss,
            string tableName,
            FormData formData)
        {
            var prefix = "DataViewFilters_" + tableName + "_";
            var prefixLength = prefix.Length;
            formData.Keys
                .Where(o => o.StartsWith(prefix))
                .Select(o => new
                {
                    Column = ss.GetColumn(o.Substring(prefixLength)),
                    ColumnName = o.Substring(prefixLength),
                    Value = formData[o].Value
                })
                .Where(o => o.Column != null)
                .ForEach(data =>
                {
                    switch (data.Column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            CsBoolColumns(data.ColumnName, data.Value, where);
                            break;
                        case Types.CsNumeric:
                            CsNumericColumns(data.Column, data.ColumnName, data.Value, where);
                            break;
                        case Types.CsDateTime:
                            CsDateTimeColumns(data.Column, data.ColumnName, data.Value, where);
                            break;
                        case Types.CsString:
                            CsStringColumns(data.ColumnName, data.Value, where);
                            break;
                    }
                });
            return where;
        }

        private static void CsBoolColumns(
            string columnName, string value, SqlWhereCollection where)
        {
            if (value.ToBool())
            {
                where.Add(raw: "[t0].[{0}] = 1".Params(columnName));
            }
        }

        private static void CsNumericColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsNumericColumnsWhere(columnName, param),
                    CsNumericColumnsWhereNull(column, columnName, param)));
            }
        }

        private static SqlWhere CsNumericColumnsWhere(string columnName, List<string> param)
        {
            return param.Where(o => o != "\t").Any()
                ? new SqlWhere(
                    columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                    name: columnName,
                    _operator: " in ({0})".Params(param
                        .Where(o => o != "\t")
                        .Select(o => o.ToLong())
                        .Join()))
                : null;
        }

        private static SqlWhere CsNumericColumnsWhereNull(
            Column column, string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: "={0}".Params(column.UserColumn
                            ? User.UserTypes.Anonymous.ToInt()
                            : 0))))
                : null;
        }

        private static void CsDateTimeColumns(
            Column column, string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsDateTimeColumnsWhere(columnName, param),
                    CsDateTimeColumnsWhereNull(column, columnName, param)));
            }
        }

        private static SqlWhere CsDateTimeColumnsWhere(string columnName, List<string> param)
        {
            return param.Where(o => o != "\t").Any()
                ? new SqlWhere(raw: param.Select(range =>
                    "[t0].[{0}] is null or [t0].[{0}] between '{1}' and '{2}'".Params(
                        columnName,
                        range.Split_1st().ToDateTime().ToUniversal()
                            .ToString("yyyy/M/d H:m:s"),
                        range.Split_2nd().ToDateTime().ToUniversal()
                            .ToString("yyyy/M/d H:m:s.fff"))).Join(" or "))
                : null;
        }

        private static SqlWhere CsDateTimeColumnsWhereNull(
            Column column, string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " not between '{1}' and '{2}'".Params(
                            Parameters.General.MinTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s"),
                            Parameters.General.MaxTime.ToUniversal()
                                .ToString("yyyy/M/d H:m:s")))))
                : null;
        }

        private static void CsStringColumns(
            string columnName, string value, SqlWhereCollection where)
        {
            var param = value.Deserialize<List<string>>();
            if (param.Any())
            {
                where.Add(or: new SqlWhereCollection(
                    CsStringColumnsWhere(columnName, param),
                    CsStringColumnsWhereNull(columnName, param)));
            }
        }

        private static SqlWhere CsStringColumnsWhere(string columnName, List<string> param)
        {
            return param.Where(o => o != "\t").Any()
                ? new SqlWhere(
                    columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                    name: columnName,
                    value: param.Where(o => o != "\t"),
                    multiParamOperator: " or ")
                : null;
        }

        private static SqlWhere CsStringColumnsWhereNull(string columnName, List<string> param)
        {
            return param.Any(o => o == "\t")
                ? new SqlWhere(or: new SqlWhereCollection(
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: " is null"),
                    new SqlWhere(
                        columnBrackets: new string[] { "[t0].[{0}]".Params(columnName) },
                        _operator: "=''")))
                : null;
        }

        private static SqlWhereCollection Search(
            this SqlWhereCollection where, string tableName, FormData formData, long siteId)
        {
            var words = formData.Get("DataViewFilters_Search").SearchIndexes();
            return words.Count() != 0
                ? where.Search(tableName, words, siteId)
                : where;
        }

        private static SqlWhereCollection Search(
            this SqlWhereCollection where,
            string tableName,
            IEnumerable<string> words,
            long siteId)
        {
            var results = SearchIndexUtilities.GetIdCollection(
                searchIndexes: words, siteId: siteId).Join();
            if (results != string.Empty)
            {
                switch (tableName)
                {
                    case "Issues":
                        where.Add(
                            columnBrackets: new string[] { "[IssueId]" },
                            name: "IssueId",
                            _operator: " in (" + results + ")");
                        break;
                    case "Results":
                        where.Add(
                            columnBrackets: new string[] { "[ResultId]" },
                            name: "ResultId",
                            _operator: " in (" + results + ")");
                        break;
                    case "Wikis":
                        where.Add(
                            columnBrackets: new string[] { "[WikiId]" },
                            name: "WikiId",
                            _operator: " in (" + results + ")");
                        break;
                }
            }
            else
            {
                where.Add(raw: "0=1");
            }
            return where;
        }
    }
}
