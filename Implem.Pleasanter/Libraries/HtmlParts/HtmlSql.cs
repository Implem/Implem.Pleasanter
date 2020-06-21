using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSql
    {
        public static HtmlBuilder ExtendedSql(this HtmlBuilder hb, Context context)
        {
            Parameters.ExtendedSqls
                ?.Where(o => o.Html)
                .Where(o => o.SiteIdList?.Any() != true || o.SiteIdList.Contains(context.SiteId))
                .Where(o => o.IdList?.Any() != true || o.IdList.Contains(context.Id))
                .Where(o => o.Controllers?.Any() != true || o.Controllers.Contains(context.Controller))
                .Where(o => o.Actions?.Any() != true || o.Actions.Contains(context.Action))
                .Where(o => !o.Disabled)
                .ForEach(extendedSql =>
                {
                    var dataSet = Rds.ExecuteDataSet(
                        context: context,
                        statements: new SqlStatement(commandText: extendedSql.CommandText));
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
                        hb.Hidden(
                            controlId: dataSet.Tables.Count > 1
                                ? $"{extendedSql.Name}_{dataTable.TableName}"
                                : $"{extendedSql.Name}",
                            value: table.ToJson());
                    }
                });
            return hb;
        }
    }
}