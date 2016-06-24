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
            SiteSettings siteSettings,
            string tableName,
            FormData formData,
            SqlWhereCollection where)
        {
            return where
                .Generals(siteSettings: siteSettings, tableName: tableName, formData: formData)
                .Columns(siteSettings: siteSettings, tableName: tableName, formData: formData)
                .Search(tableName: tableName, formData: formData);
        }

        private static SqlWhereCollection Generals(
            this SqlWhereCollection sqlWhereCollection,
            SiteSettings siteSettings,
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
                            _operator: " < {0}".Params(Parameters.General.CompletionCode));
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
                            _operator: " between '{0}' and '{1}' ".Params(
                                DateTime.Now.ToLocal().Date
                                    .AddDays(siteSettings.NearCompletionTimeBeforeDays.ToInt() * (-1)),
                                DateTime.Now.ToLocal().Date
                                    .AddDays(siteSettings.NearCompletionTimeAfterDays.ToInt() + 1)
                                    .AddMilliseconds(-1)
                                    .ToString("yyyy/M/d H:m:s.fff")));
                        break;
                    case "DataViewFilters_Delay":
                        sqlWhereCollection
                            .Add(
                                columnBrackets: new string[] { "[t0].[Status]" },
                                name: "_U",
                                _operator: " < {0}".Params(Parameters.General.CompletionCode))
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
                                _operator: " < {0}".Params(Parameters.General.CompletionCode))
                            .Add(
                                columnBrackets: new string[] { "[t0].[CompletionTime]" },
                                _operator: " < getdate() ");
                        break;
                    default: break;
                }
            });
            return sqlWhereCollection;
        }

        public static FormData SessionFormData(long? siteId = null)
        {
            var key = siteId == null
                 ? "DataView_" + Pages.Key()
                 : "DataView_" + siteId;
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
            SiteSettings siteSettings,
            string tableName,
            FormData formData)
        {
            var prefix = "DataViewFilters_" + tableName + "_";
            var prefixLength = prefix.Length;
            formData.Keys
                .Where(o => o.StartsWith(prefix))
                .Select(o => new
                {
                    Column = siteSettings.AllColumn(o.Substring(prefixLength)),
                    ColumnName = o.Substring(prefixLength),
                    Value = Forms.Data(o)
                })
                .Where(o => o.Column != null)
                .ForEach(data =>
                {
                    switch (data.Column.TypeName.CsTypeSummary())
                    {
                        case Types.CsBool:
                            if (data.Value.ToBool())
                            {
                                where.Add(raw: "[t0].[{0}] = 1".Params(data.ColumnName));
                            }
                            break;
                        case Types.CsNumeric:
                            if (data.Value == "\t")
                            {
                                if (data.Column.UserColumn)
                                {
                                    where.Add(raw: "[t0].[{0}] is null or [t0].[{0}]={1}"
                                        .Params(
                                            data.ColumnName,
                                            User.UserTypes.Anonymous.ToInt()));
                                }
                                else
                                {
                                    where.Add(raw: "[t0].[{0}] is null or [t0].[{0}]=0"
                                        .Params(data.ColumnName));
                                }
                            }
                            else
                            {
                                where.Add(
                                    columnBrackets: new string[] { "[t0].[{0}]"
                                        .Params(data.ColumnName) },
                                    name: data.ColumnName,
                                    value: data.Value.ToLong());
                            }
                            break;
                        case Types.CsString:
                            if (data.Value == "\t")
                            {
                                where.Add(raw: "[t0].[{0}] is null or [t0].[{0}] = ''"
                                    .Params(data.ColumnName));
                            }
                            else
                            {
                                where.Add(
                                    columnBrackets: new string[] { "[t0].[{0}]"
                                        .Params(data.ColumnName) },
                                    name: data.ColumnName,
                                    value: data.Value);
                            }
                            break;
                    }
                });
            return where;
        }

        private static SqlWhereCollection Search(
            this SqlWhereCollection where, string tableName, FormData formData)
        {
            var words = formData.Get("DataViewFilters_Search").SearchIndexes();
            return words.Count() != 0
                ? where.Search(tableName, words)
                : where;
        }

        private static SqlWhereCollection Search(
            this SqlWhereCollection where, string tableName, IEnumerable<string> words)
        {
            var results = SearchIndexesUtility.Get(searchIndexes: words)
                .Tables[0]
                .AsEnumerable()
                .Distinct()
                .Select(o => o["ReferenceId"].ToLong())
                .Join();
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
                where.Add(raw: "0 = 1");
            }
            return where;
        }
    }
}
