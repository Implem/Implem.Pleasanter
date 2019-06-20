using Implem.DefinitionAccessor;
using Implem.IRds;
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
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection,
            string tableNameTemp = "")
        {
            Consoles.Write(sourceTableName, Consoles.Types.Info);
            if (tableNameTemp.IsNullOrEmpty())
            {
                tableNameTemp = sourceTableName;
            }
            var sqlStatement = new SqlStatement(
                Def.Sql.CreateTable,
                Sqls.SqlParamCollection());
            sqlStatement.CreateColumn(sourceTableName, columnDefinitionCollection);
            sqlStatement.CreatePk(sourceTableName, columnDefinitionCollection, tableIndexCollection);
            sqlStatement.CreateIx(generalTableName, sourceTableName, tableType, columnDefinitionCollection);
            sqlStatement.CreateDefault(tableNameTemp, columnDefinitionCollection);
            sqlStatement.DropConstraint(factory: factory, sourceTableName: sourceTableName, tableIndexCollection: tableIndexCollection);
            sqlStatement.CommandText = sqlStatement.CommandText.Replace("#TableName#", tableNameTemp);
            Def.SqlIoByAdmin(factory: factory, transactional: true).ExecuteNonQuery(factory: factory, sqlStatement: sqlStatement);
        }

        internal static void MigrateTable(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            Consoles.Write(generalTableName, Consoles.Types.Info);
            var destinationTableName = DateTimes.Full() + "_" + sourceTableName;
            CreateTable(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: tableType,
                columnDefinitionCollection: columnDefinitionCollection,
                tableIndexCollection: tableIndexCollection,
                rdsColumnCollection: null,
                tableNameTemp: destinationTableName);
            if (Def.ExistsTable(generalTableName, o => o.Identity &&
                !sourceTableName.EndsWith("_history") &&
                !sourceTableName.EndsWith("_deleted")))
            {
                Def.SqlIoByAdmin(factory: factory).ExecuteNonQuery(
                    factory: factory,
                    commandText: MigrateSql(
                        factory: factory,
                        sql: Def.Sql.MigrateTableWithIdentity,
                        columnDefinitionCollection: columnDefinitionCollection,
                        sourceTableName: sourceTableName,
                        destinationTableName: destinationTableName));
            }
            else
            {
                Def.SqlIoByAdmin(factory: factory).ExecuteNonQuery(
                    factory: factory,
                    commandText: MigrateSql(
                        factory: factory,
                        sql: Def.Sql.MigrateTable,
                        columnDefinitionCollection: columnDefinitionCollection,
                        sourceTableName: sourceTableName,
                        destinationTableName: destinationTableName));
            }
        }

        private static string MigrateSql(
            ISqlObjectFactory factory,
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
                var destinationColumnNameHistoryBracket = columnDefinition.OldColumnName?.SqlBracket();
                if (!Columns.Get(factory: factory, sourceTableName: sourceTableName).Any(
                    o => o["ColumnName"].ToString() == columnDefinition.ColumnName))
                {
                    if (!columnDefinition.OldColumnName.IsNullOrEmpty())
                    {
                        destinationColumnCollection.Add(destinationColumnNameBracket);
                        sourceColumnCollection.Add(destinationColumnNameHistoryBracket);
                    }
                    else if (columnDefinition.Default.IsNullOrEmpty() && !columnDefinition.Nullable)
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

        internal static bool Exists(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.ExistsTable.Replace("#TableName#", sourceTableName))
                .Rows.Count == 1;
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
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
                    Constraints.HasChanges(
                        factory: factory,
                        sourceTableName: sourceTableName,
                        columnDefinitionCollection: columnDefinitionCollection) ||
                    Indexes.HasChanges(
                        factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableType: tableType,
                        columnDefinitionCollection: columnDefinitionCollection);
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
