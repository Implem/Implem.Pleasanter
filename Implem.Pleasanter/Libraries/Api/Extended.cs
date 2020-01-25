using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Api
{
    public static class Extended
    {
        public static ContentResult Sql(Context context)
        {
            var extendedApi = context.RequestDataString.Deserialize<ExtendedApi>();
            if (extendedApi == null)
            {
                return ApiResults.BadRequest(context: context);
            }
            var extendedSql = Parameters.ExtendedSqls
                .Where(o => o.Api)
                .Where(o => o.Name == extendedApi.Name)
                .Where(o => !o.Disabled)
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
    }
}