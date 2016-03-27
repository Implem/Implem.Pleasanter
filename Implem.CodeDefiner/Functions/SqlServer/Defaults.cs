using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class Defaults
    {
        internal static EnumerableRowCollection<DataRow> Get(string sourceTableName)
        {
            return Def.GetSqlIoOfAdmin().ExecuteTable(
                Def.Code.Sql_GetDefaults.Replace("#TableName#", sourceTableName),
                SqlCmd.Types.PlainSql)
                .AsEnumerable();
        }

        internal static bool HasDifference(
            string sourceTableName, IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            return
                columnDefinitionCollection
                    .Where(o => o.Default != string.Empty)
                    .Where(o => !(sourceTableName.EndsWith("_old") && o.ColumnName == "Ver"))
                    .OrderBy(o => o.ColumnName)
                    .Select(o => o.ColumnName + "," + GetDefaultDefinition(o))
                    .JoinReturn() !=
                Defaults.Get(sourceTableName)
                    .Where(o => !(sourceTableName.EndsWith("_old") && o["column_name"].ToString() == "Ver"))
                    .OrderBy(o => o["column_name"])
                    .Select(o => o["column_name"] + "," + o["column_default"])
                    .JoinReturn();
        }

        internal static void CreateDefault(
            this SqlCmd sqlCmd,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> dbColumnCollection,
            string tableNameTemp = "")
        {
            sqlCmd.CommandText = sqlCmd.CommandText.Replace(
                "#Defaults#", Def.Code.Sql_DeleteDefault + columnDefinitionCollection
                    .Where(o => o.Default != string.Empty)
                    .Where(o => !(sourceTableName.EndsWith("_old") && o.ColumnName == "Ver"))
                    .Select(o => Defaults.Sql_Create(Def.Code.Sql_CreateDefault, Strings.CoalesceEmpty(tableNameTemp, sourceTableName), o))
                    .JoinReturn());
        }

        private static string Sql_Create(
            string sqlCreateDefault, string sourceTableName, ColumnDefinition columnDefinition)
        {
            return sqlCreateDefault
                .Replace("#DefaultName#", sourceTableName + Strings.NewGuid())
                .Replace("#ColumnName#", columnDefinition.ColumnName)
                .Replace("#DefaultValue#", GetDefaultDefinition(columnDefinition));
        }

        private static string GetDefaultDefinition(ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName.CsTypeSummary())
            {
                case Types.CsString:
                    return "('" + columnDefinition.Default + "')";
                case Types.CsDateTime:
                    if (columnDefinition.Default.ToLower() == "now")
                    {
                        return "(getdate())";
                    }
                    else
                    {
                        return "('" + columnDefinition.Default + "')";
                    }
                default:
                    return "((" + columnDefinition.Default + "))";
            }
        }
    }
}
