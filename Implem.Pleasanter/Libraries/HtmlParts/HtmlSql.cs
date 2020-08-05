using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSql
    {
        public static HtmlBuilder ExtendedSql(
            this HtmlBuilder hb,
            Context context)
        {
            ExtensionUtilities.ExtensionWhere<ExtendedSql>(
                context: context,
                extensions: Parameters.ExtendedSqls
                    ?.Where(o => o.Html)
                    .Where(o => !o.CommandText.IsNullOrEmpty()))
                .ForEach(extendedSql =>
                {
                    var dataSet = DataSources.Rds.ExecuteDataSet(
                        context: context,
                        statements: new SqlStatement(commandText: extendedSql.CommandText
                            .Replace("{{SiteId}}", context.SiteId.ToString())
                            .Replace("{{Id}}", context.Id.ToString())));
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