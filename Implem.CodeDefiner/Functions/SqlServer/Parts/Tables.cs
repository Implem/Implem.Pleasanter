using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer.Parts
{
    internal static class Tables
    {
        internal static void CreateTable(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection,
            string tableNameTemp = "")
        {
            Consoles.Write(sourceTableName, Consoles.Types.Info);
            if (tableNameTemp == string.Empty)
            {
                tableNameTemp = sourceTableName;
            }
            var sqlStatement = new SqlStatement(
                Def.Sql.CreateTable,
                Sqls.SqlParamCollection());
            sqlStatement.CreateColumn(sourceTableName, columnDefinitionCollection);
            sqlStatement.CreatePk(sourceTableName, columnDefinitionCollection, tableIndexCollection);
            sqlStatement.CreateIx(generalTableName, sourceTableName, old, columnDefinitionCollection);
            sqlStatement.CreateDefault(tableNameTemp, columnDefinitionCollection, rdsColumnCollection);
            sqlStatement.DropConstraint(sourceTableName, tableIndexCollection);
            sqlStatement.CommandText = sqlStatement.CommandText.Replace("#TableName#", tableNameTemp);
            Def.SqlIoByAdmin(transactional: true).ExecuteNonQuery(sqlStatement);
        }

        internal static void MigrateTable(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            Consoles.Write(generalTableName, Consoles.Types.Info);
            var destinationTableName = DateTimes.Full() + "_" + sourceTableName;
            CreateTable(
                generalTableName,
                sourceTableName,
                old,
                columnDefinitionCollection,
                tableIndexCollection,
                null,
                destinationTableName);
            if (Def.ExistsTable(generalTableName, o => o.Identity &&
                !sourceTableName.EndsWith("_history") &&
                !sourceTableName.EndsWith("_deleted")))
            {
                Def.SqlIoByAdmin().ExecuteNonQuery(
                    MigrateSql(
                        Def.Sql.MigrateTableWithIdentity,
                        columnDefinitionCollection,
                        sourceTableName,
                        destinationTableName));
            }
            else
            {
                Def.SqlIoByAdmin().ExecuteNonQuery(
                    MigrateSql(
                        Def.Sql.MigrateTable,
                        columnDefinitionCollection,
                        sourceTableName,
                        destinationTableName));
            }
        }

        private static string MigrateSql(
            string sql,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string sourceTableName,
            string destinationTableName)
        {
            var sourceColumnCollection = new List<string>();
            var destinationColumnCollection = new List<string>();
            columnDefinitionCollection.ForEach(columnDefinition =>
            {
                var destinationColumnNameBracket = columnDefinition.ColumnName.SqlBracket();
                var destinationColumnNameHistoryBracket = columnDefinition.OldColumnName.SqlBracket();
                if (!Columns.Get(sourceTableName).Any(
                    o => o["ColumnName"].ToString() == columnDefinition.ColumnName))
                {
                    if (columnDefinition.OldColumnName != string.Empty)
                    {
                        destinationColumnCollection.Add(destinationColumnNameBracket);
                        sourceColumnCollection.Add(destinationColumnNameHistoryBracket);
                    }
                    else if (columnDefinition.Default == string.Empty && !columnDefinition.Nullable)
                    {
                        destinationColumnCollection.Add(destinationColumnNameBracket);
                        switch (columnDefinition.TypeName.CsTypeSummary())
                        {
                            case Types.CsString: sourceColumnCollection.Add("@empty"); break;
                            case Types.CsDateTime: sourceColumnCollection.Add("getdate()"); break;
                            default: sourceColumnCollection.Add("0"); break;
                        }
                    }
                }
                else
                {
                    destinationColumnCollection.Add(destinationColumnNameBracket);
                    sourceColumnCollection.Add(destinationColumnNameBracket);
                }
            });
            return sql
                .Replace("#SourceTableName#", sourceTableName)
                .Replace("#DestinationTableName#", destinationTableName)
                .Replace("#SourceColumnCollection#", sourceColumnCollection.Join())
                .Replace("#DestinationColumnCollection#", destinationColumnCollection.Join());
        }

        private static string SqlBracket(this string columnName)
        {
            return columnName.Split('.').Select(o => "[" + o + "]").Join(".");
        }

        internal static bool Exists(string sourceTableName)
        {
            return Def.SqlIoByAdmin().ExecuteTable(
                Def.Sql.ExistsTable.Replace("#TableName#", sourceTableName)).Rows.Count == 1;
        }

        internal static bool HasChanges(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection)
        {
            if (HasChanges(
                columnDefinitionCollection, rdsColumnCollection))
            {
                return true;
            }
            else
            {
                return
                    Columns.HasChanges(sourceTableName, columnDefinitionCollection, rdsColumnCollection) ||
                    Constraints.HasChanges(sourceTableName, columnDefinitionCollection) ||
                    Indexes.HasChanges(generalTableName, sourceTableName, old, columnDefinitionCollection);
            }
        }

        private static bool HasChanges(
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection)
        {
            return rdsColumnCollection.Count() != columnDefinitionCollection.Count();
        }
    }
}
