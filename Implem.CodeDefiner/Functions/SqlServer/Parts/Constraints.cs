using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer.Parts
{
    internal static class Constraints
    {
        internal static EnumerableRowCollection<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Defaults
                .Replace("#InitialCatalog#", Environments.ServiceName)
                .Replace("#TableName#", sourceTableName))
                    .AsEnumerable();
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            return
                columnDefinitionCollection
                    .Where(o => !o.Default.IsNullOrEmpty())
                    .Where(o => !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver"))
                    .OrderBy(o => o.ColumnName)
                    .Select(o => o.ColumnName + "," + DefaultDefinition(o))
                    .JoinReturn() !=
                Get(factory: factory, sourceTableName: sourceTableName)
                    .Where(o => !(sourceTableName.EndsWith("_history") && o["column_name"].ToString() == "Ver"))
                    .OrderBy(o => o["column_name"])
                    .Select(o => o["column_name"] + "," + o["column_default"])
                    .JoinReturn();
        }

        internal static void CreateDefault(
            this SqlStatement sqlStatement,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string tableNameTemp = "")
        {
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#Defaults#", Def.Sql.DeleteDefault + columnDefinitionCollection
                    .Where(o => !o.Default.IsNullOrEmpty())
                    .Where(o => !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver"))
                    .Select(o => Sql_Create(Def.Sql.CreateDefault, Strings.CoalesceEmpty(tableNameTemp, sourceTableName), o))
                    .JoinReturn());
        }

        private static string Sql_Create(
            string sqlCreateDefault, string sourceTableName, ColumnDefinition columnDefinition)
        {
            return sqlCreateDefault
                .Replace("#DefaultName#", sourceTableName + Strings.NewGuid())
                .Replace("#ColumnName#", columnDefinition.ColumnName)
                .Replace("#DefaultValue#", DefaultDefinition(columnDefinition));
        }

        private static string DefaultDefinition(ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName.CsTypeSummary())
            {
                case Types.CsString:
                    return "('" + columnDefinition.Default + "')";
                case Types.CsDateTime:
                    if (columnDefinition.Default?.ToLower() == "now")
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

        internal static void DropConstraint(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            sqlStatement.CommandText = sqlStatement.CommandText
                .Replace("#DropConstraint#", tableIndexCollection
                    .Where(o => Indexes.Get(
                        factory: factory,
                        sourceTableName: sourceTableName)
                        .Contains(o.IndexName()))
                    .Select(o => Sql_Drop(o)
                        .Replace("#SourceTableName#", sourceTableName)
                        .Replace("#IndexName#", o.IndexName()))
                    .Join("\r\n"));
        }

        private static string Sql_Drop(IndexInfo o)
        {
            return o.Type == IndexInfo.Types.Pk
                ? Def.Sql.DropConstraint
                : Def.Sql.DropIndex;
        }
    }
}
