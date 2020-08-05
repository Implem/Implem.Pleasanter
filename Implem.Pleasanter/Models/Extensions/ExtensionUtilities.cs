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
    public static class ExtensionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult Sql(Context context)
        {
            var extendedApi = context.RequestDataString.Deserialize<ExtendedApi>();
            if (extendedApi == null)
            {
                return ApiResults.BadRequest(context: context);
            }
            var extendedSql = ExtensionWhere<ParameterAccessor.Parts.ExtendedSql>(
                context: context,
                extensions: Parameters.ExtendedSqls
                    ?.Where(o => o.Api)
                    .Where(o => o.Name == extendedApi.Name))
                .FirstOrDefault();
            if (extendedSql == null)
            {
                return ApiResults.BadRequest(context: context);
            }
            var param = new SqlParamCollection();
            extendedApi.Params?.ForEach(part =>
                param.Add(
                    variableName: part.Key,
                    value: part.Value));
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement(
                    commandText: extendedSql.CommandText,
                    param: param));
            var data = new Dictionary<string, List<Dictionary<string, object>>>();
            foreach (DataTable dataTable in dataSet.Tables)
            {
                var table = new List<Dictionary<string, object>>();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var row = new Dictionary<string, object>();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        row.AddIfNotConainsKey(
                            dataColumn.ColumnName,
                            dataRow[dataColumn.ColumnName]);
                    }
                    table.Add(row);
                }
                data.AddIfNotConainsKey(dataTable.TableName, table);
            }
            return ApiResults.Get(
                statusCode: 200,
                limitPerDate: 0,
                limitRemaining: 0,
                response: new
                {
                    Data = data
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static IEnumerable<T> ExtensionWhere<T>(
            Context context,
            IEnumerable<ParameterAccessor.Parts.ExtendedBase> extensions,
            string columnName = null)
        {
            return ExtensionWhere<T>(
                extensions: extensions,
                deptId: context.DeptId,
                userId: context.UserId,
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action,
                columnName: columnName);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static IEnumerable<T> ExtensionWhere<T>(
            IEnumerable<ParameterAccessor.Parts.ExtendedBase> extensions,
            int userId,
            int deptId,
            long siteId,
            long id,
            string controller,
            string action,
            string columnName = null)
        {
            return extensions
                ?.Where(o => IsContains(o.DeptIdList, deptId))
                .Where(o => IsContains(o.UserIdList, userId))
                .Where(o => IsContains(o.SiteIdList, siteId))
                .Where(o => IsContains(o.IdList, id))
                .Where(o => IsContains(o.Controllers, controller))
                .Where(o => IsContains(o.Actions, action))
                .Where(o => IsContains(o.ColumnList, columnName))
                .Where(o => !o.Disabled)
                .Cast<T>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool IsContains<T, U>(List<T> list, U param)
        {
            return param == null
                || (list?.Any(o => o.ToString() == param.ToString()) ?? true)
                || (list?
                    .Where(o => o.ToString().StartsWith("-"))
                    .Any(o => o.ToString() != $"-{param}") ?? true);
        }
    }
}
