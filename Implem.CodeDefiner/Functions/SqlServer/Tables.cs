using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class Tables
    {
        internal static void CreateTable(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection,
            EnumerableRowCollection<DataRow> dbColumnCollection,
            string tableNameTemp = "")
        {
            Consoles.Write(sourceTableName, Consoles.Levels.Info);
            if (tableNameTemp == string.Empty)
            {
                tableNameTemp = sourceTableName;
            }
            var sqlCmd = new SqlCmd(
                Def.Code.Sql_CreateTable,
                SqlCmd.Types.PlainSql,
                Sqls.GetSqlParamCollection());
            sqlCmd.CreateColumn(sourceTableName, columnDefinitionCollection);
            sqlCmd.CreatePk(sourceTableName, columnDefinitionCollection, tableIndexCollection);
            sqlCmd.CreateIx(generalTableName, sourceTableName, old, columnDefinitionCollection);
            sqlCmd.CreateDefault(tableNameTemp, columnDefinitionCollection, dbColumnCollection);
            sqlCmd.DropConstraints(sourceTableName, tableIndexCollection);
            sqlCmd.CommandText = sqlCmd.CommandText.Replace("#TableName#", tableNameTemp);
            Def.GetSqlIoOfAdmin(transactional: true).ExecuteNonQuery(sqlCmd);
        }

        internal static void MigrateTable(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            Consoles.Write(generalTableName, Consoles.Levels.Info);
            var destinationTableName = Times.Full() + "_" + sourceTableName;
            CreateTable(
                generalTableName,
                sourceTableName,
                old,
                columnDefinitionCollection,
                tableIndexCollection,
                null,
                destinationTableName);
            if (Def.ExistsTable(generalTableName, o => o.Identity &&
                !sourceTableName.EndsWith("_deleted") &&
                !sourceTableName.EndsWith("_old")))
            {
                Def.GetSqlIoOfAdmin().ExecuteNonQuery(
                    GetMigrateSql(
                        Def.Code.Sql_MigrateTableWithIdentity,
                        columnDefinitionCollection,
                        sourceTableName,
                        destinationTableName),
                    SqlCmd.Types.PlainSql);
            }
            else
            {
                Def.GetSqlIoOfAdmin().ExecuteNonQuery(
                    GetMigrateSql(
                        Def.Code.Sql_MigrateTable,
                        columnDefinitionCollection,
                        sourceTableName,
                        destinationTableName),
                    SqlCmd.Types.PlainSql);
            }
        }

        private static string GetMigrateSql(
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
                var destinationColumnNameOldBracket = columnDefinition.ColumnNameOld.SqlBracket();
                if (!Columns.Get(sourceTableName).Any(
                    o => o["ColumnName"].ToString() == columnDefinition.ColumnName))
                {
                    if (columnDefinition.ColumnNameOld != string.Empty)
                    {
                        destinationColumnCollection.Add(destinationColumnNameBracket);
                        sourceColumnCollection.Add(destinationColumnNameOldBracket);
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
            return GetCodeSqlBracket(columnName);
        }

        private static string GetCodeSqlBracket(string columnName)
        {
            return columnName.Split('.').Select(o => "[" + o + "]").JoinDot();
        }

        internal static bool Exists(string sourceTableName)
        {
            return Def.GetSqlIoOfAdmin().ExecuteTable(
                Def.Code.Sql_ExistsTable.Replace("#TableName#", sourceTableName),
                SqlCmd.Types.PlainSql).Rows.Count == 1;
        }

        internal static bool HasDifference(
            string generalTableName,
            string sourceTableName,
            bool old,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> dbColumnCollection)
        {
            if (HasDifference(
                columnDefinitionCollection, dbColumnCollection))
            {
                return true;
            }
            else
            {
                return
                    Columns.HasDifference(sourceTableName, columnDefinitionCollection, dbColumnCollection) ||
                    Defaults.HasDifference(sourceTableName, columnDefinitionCollection) ||
                    Indexes.HasDifference(generalTableName, sourceTableName, old, columnDefinitionCollection);
            }
        }

        private static bool HasDifference(
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> dbColumnCollection)
        {
            return dbColumnCollection.Count() != columnDefinitionCollection.Count();
        }
    }
}
