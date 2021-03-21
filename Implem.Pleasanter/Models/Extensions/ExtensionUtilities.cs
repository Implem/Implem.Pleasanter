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
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class ExtensionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult Sql(Context context)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var extendedApi = context.RequestDataString.Deserialize<ExtendedApi>();
            if (extendedApi == null)
            {
                return ApiResults.BadRequest(context: context);
            }
            var data = ExecuteDataSetAsDictionary(
                context: context,
                name: extendedApi.Name,
                _params: extendedApi.Params);
            if (data == null)
            {
                return ApiResults.BadRequest(context: context);
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
        public static Dictionary<string, List<Dictionary<string, object>>> ExecuteDataSetAsDictionary(
            Context context,
            string name,
            Dictionary<string, object> _params)
        {
            var dataSet = ExecuteDataSet(context, name, _params);
            return DataSetToDictionary(dataSet);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Dynamic.ExpandoObject ExecuteDataSetAsDynamic(
            Context context,
            string name,
            Dictionary<string, object> _params)
        {
            var dataSet = ExecuteDataSet(context, name, _params);
            return DataSetToExpando(dataSet);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DataSet ExecuteDataSet(
            Context context,
            string name,
            Dictionary<string, object> _params)
        {
            var extendedSql = Parameters.ExtendedSqls
                ?.Where(o => o.Api)
                .Where(o => o.Name == name)
                .ExtensionWhere<ParameterAccessor.Parts.ExtendedSql>(context: context)
                .FirstOrDefault();
            if (extendedSql == null)
            {
                return null;
            }
            var param = new SqlParamCollection();
            _params?.ForEach(part =>
                param.Add(
                    variableName: part.Key,
                    value: part.Value));
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                statements: new SqlStatement(
                    commandText: extendedSql.CommandText
                        .Replace("{{SiteId}}", context.SiteId.ToString())
                        .Replace("{{Id}}", context.Id.ToString()),
                    param: param));
            return dataSet;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, List<Dictionary<string, object>>> DataSetToDictionary(DataSet dataSet)
        {
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
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static System.Dynamic.ExpandoObject DataSetToExpando(DataSet dataSet)
        {
            var dynamicDataSet = new System.Dynamic.ExpandoObject();
            var dataSetDict = (IDictionary<string, object>)dynamicDataSet;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                var table = new List<object>();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var dynamicDataRow = new System.Dynamic.ExpandoObject();
                    var dataRowDic = (IDictionary<string, object>)dynamicDataRow;
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        dataRowDic[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName];
                    }
                    table.Add(dynamicDataRow);
                }
                dataSetDict[dataTable.TableName] = table;
            }
            return dynamicDataSet;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static IEnumerable<T> ExtensionWhere<T>(
            this IEnumerable<ParameterAccessor.Parts.ExtendedBase> extensions,
            Context context,
            long? siteId = null,
            long? id = null,
            string columnName = null)
        {
            return ExtensionWhere<T>(
                extensions: extensions,
                deptId: context.DeptId,
                userId: context.UserId,
                siteId: siteId ?? context.SiteId,
                id: id ?? context.Id,
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
                || list?.Any() != true
                || list.Any(o => o.ToString() == param.ToString())
                || list
                    .Where(o => o.ToString().StartsWith("-"))
                    .All(o => o.ToString() != $"-{param}");
        }
    }
}
