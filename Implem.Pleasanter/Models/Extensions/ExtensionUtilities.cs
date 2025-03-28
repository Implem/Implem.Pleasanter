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
using System.Text.Json;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class ExtensionUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance Sql(Context context)
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
            string connectionString;
            switch (extendedSql.DbUser)
            {
                case "Owner":
                    connectionString = Parameters.Rds.OwnerConnectionString;
                    break;
                default:
                    connectionString = null;
                    break;
            }
            var param = new SqlParamCollection();
            _params?.ForEach(part =>
                param.Add(
                    variableName: part.Key,
                    value: part.Value));
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                connectionString: connectionString,
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
            string name = null,
            long? siteId = null,
            long? id = null,
            string columnName = null)
        {
            return ExtensionWhere<T>(
                extensions: extensions,
                name: name,
                deptId: context.DeptId,
                groups: context.Groups,
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
            string name,
            int deptId,
            List<int> groups,
            int userId,
            long siteId,
            long id,
            string controller,
            string action,
            string columnName = null)
        {
            return extensions
                ?.Where(o => !o.SpecifyByName || o.Name == name)
                .Where(o => MeetConditions(o.DeptIdList, deptId))
                .Where(o => o.GroupIdList?.Any() != true
                    || groups?.Any(groupId => MeetConditions(o.GroupIdList, groupId)) == true)
                .Where(o => MeetConditions(o.UserIdList, userId))
                .Where(o => MeetConditions(o.SiteIdList, siteId))
                .Where(o => MeetConditions(o.IdList, id))
                .Where(o => MeetConditions(o.Controllers, controller))
                .Where(o => MeetConditions(o.Actions, action))
                .Where(o => MeetConditions(o.ColumnList, columnName))
                .Where(o => !o.Disabled)
                .Cast<T>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool MeetConditions<T, U>(List<T> list, U param)
        {
            var data = param?.ToString();
            var items = GetItems(list);
            return NoConditions(
                items: items,
                data: data)
                    || PositiveConditions(
                        items: items,
                        data: data)
                    || NegativeConditions(
                        items: items,
                        data: data);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<string> GetItems<T>(List<T> list)
        {
            return list?
                .Select(o => o?.ToString())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool NoConditions(List<string> items, string data)
        {
            return data.IsNullOrEmpty()
                || items?.Any() != true;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool PositiveConditions(List<string> items, string data)
        {
            return items.Any(item => item == data);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool NegativeConditions(List<string> items, string data)
        {
            return (items
                .Where(item => item.StartsWith("-"))
                .Any(item => item != $"-{data}")
                    && items
                        .Where(item => item.StartsWith("-"))
                        .All(item => item != $"-{data}"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance GetByApi(
            Context context,
            SiteSettings ss,
            int extensionId)
        {
            var deniedEnvironmentData = ExtensionValidators.ValidateEnvironment(context: context, api: true);
            if (deniedEnvironmentData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: deniedEnvironmentData);
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
                return ApiResults.BadRequest(context: context);
            var invalid = ExtensionValidators.OnEntry(ss:ss, context: context, api: true);
            if(invalid.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: invalid);
            var api = context.RequestDataString.Deserialize<Api>();
            if (api == null && !context.RequestDataString.IsNullOrEmpty())
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            var view = api?.View ?? new View();
            var pageSize = Parameters.Api.PageSize;
            var tableType = (api?.TableType) ?? Sqls.TableTypes.Normal;
            if (extensionId > 0)
            {
                view.ColumnFilterHash ??= new Dictionary<string, string>();
                view.ColumnFilterHash.Add("ExtensionId", extensionId.ToString());
            }
            var session = Views.GetBySession(context: context, ss: ss);
            view.MergeSession(view);
            var extensions = new ExtensionCollection(
                context: context,
                where: view.Where(context: context, ss: ss),
                orderBy: view.OrderBy(context: context, ss: ss),
                tableType: tableType);
            return ApiResults.Get(new
            {
                StatusCode = 200,
                Response = new
                {
                    Data = extensions.Select(o=>o.GetByApi(context: context, ss: ss))
                }
            }.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance CreateByApi(Context context, SiteSettings ss)
        {
            var deniedEnvironmentData = ExtensionValidators.ValidateEnvironment(context: context, api: true);
            if (deniedEnvironmentData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: deniedEnvironmentData);
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
                return ApiResults.BadRequest(context: context);
            var extensionApiModel = context.RequestDataString.Deserialize<ExtensionApiModel>();
            if (extensionApiModel == null || extensionApiModel.HasInvalidExtensionSettings())
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            var extensionModel = new ExtensionModel(context: context, extensionId: 0, extensionApiModel: extensionApiModel);
            var invalid = ExtensionValidators.OnCreating(context: context, ss: ss, extensionModel: extensionModel, api: true);
            if (invalid.Type != Error.Types.None)
                    return ApiResults.Error(context: context, errorData: invalid);
            var errorData = extensionModel.Create(context: context, ss: ss);
            if (errorData.Type != Error.Types.None)
                return ApiResults.Error(
                    context: context,
                    errorData: errorData);
            return ApiResults.Success(
                id: extensionModel.ExtensionId,
                message: CreatedMessage(context: context, extensionModel: extensionModel).Text);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance UpdateByApi(
            Context context,
            SiteSettings ss,
            int extensionId)
        {
            var deniedEnvironmentData = ExtensionValidators.ValidateEnvironment(context: context, api: true);
            if(deniedEnvironmentData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: deniedEnvironmentData);
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
                return ApiResults.BadRequest(context: context);
            var extensionApiModel = context.RequestDataString.Deserialize<ExtensionApiModel>();
            if (extensionApiModel == null || extensionApiModel.HasInvalidExtensionSettings())
                context.InvalidJsonData = !context.RequestDataString.IsNullOrEmpty();
            var extensionModel = new ExtensionModel(
                context: context,
                extensionId: extensionId,
                extensionApiModel: extensionApiModel);
            if (extensionModel.AccessStatus != Databases.AccessStatuses.Selected)
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            var invalid = ExtensionValidators.OnUpdating(
                context: context,
                ss: ss,
                extensionModel: extensionModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            foreach (var column in ss.Columns
                         .Where(o => o.ValidateRequired ?? false)
                         .Where(o => typeof(GroupApiModel).GetField(o.ColumnName) != null))
            {
                if (extensionModel.GetType().GetField(column.ColumnName).GetValue(extensionModel).ToString().IsNullOrEmpty())
                    return ApiResults.Error(
                        context: context,
                        errorData: new ErrorData(type: Error.Types.NotIncludedRequiredColumn),
                        data: column.ColumnName);
            }
            var errorData = extensionModel.Update(context: context, ss: ss, get: false);
            if (errorData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: errorData);
            return ApiResults.Success(
                    id: extensionModel.ExtensionId,
                    message: UpdatedMessage(context: context, ss: ss, extensionModel: extensionModel).Text);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ContentResultInheritance DeleteByApi(Context context, SiteSettings ss, int extensionId)
        {
            var deniedEnvironmentData = ExtensionValidators.ValidateEnvironment(context: context, api: true);
            if (deniedEnvironmentData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: deniedEnvironmentData);
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
                return ApiResults.BadRequest(context: context);
            var extensionModel = new ExtensionModel(
                context: context,
                extensionId: extensionId,
                methodType: BaseModel.MethodTypes.Edit);
            if (extensionModel.AccessStatus != Databases.AccessStatuses.Selected)
                return ApiResults.Get(ApiResponses.NotFound(context: context));
            var invalid = ExtensionValidators.OnDeleting(
                context: context,
                ss: ss,
                extensionModel: extensionModel,
                api: true);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                    context: context,
                    errorData: invalid);
            }
            var errorData = extensionModel.Delete(context: context);
            if (errorData.Type != Error.Types.None)
                return ApiResults.Error(context: context, errorData: errorData);
            return ApiResults.Success(
                id: extensionModel.ExtensionId,
                message: Displays.Deleted(
                    context: context,
                    data: extensionModel.ExtensionName));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Message CreatedMessage(
            Context context,
            ExtensionModel extensionModel)
        {
            return Messages.Created(
                context: context,
                data: extensionModel.ExtensionName);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Message UpdatedMessage(
            Context context,
            SiteSettings ss,
            ExtensionModel extensionModel)
        {
            return Messages.Updated(
                context: context,
                data: extensionModel.ExtensionName);
        }
    }
}
