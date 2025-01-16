using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Migrator
    {
        public static void MigrateDatabaseAsync(
            ISqlObjectFactory factoryFrom,
            ISqlObjectFactory factoryTo)
        {
            Def.ColumnDefinitionCollection
                .Where(columnDefinition => !columnDefinition.TableName.StartsWith("_"))
                .Where(columnDefinition => !columnDefinition.NotUpdate)
                .Where(columnDefinition => columnDefinition.JoinTableName.IsNullOrEmpty())
                .Where(columnDefinition => columnDefinition.Calc.IsNullOrEmpty())
                .Where(columnDefinition => Parameters.Migration.ExcludeTables?
                    .Contains(columnDefinition.TableName) != true)
                .Select(columnDefinition => columnDefinition.TableName)
                .Distinct()
                .ForEach(tableName =>
                    MigrateTable(
                        tableName: tableName,
                        identity: Def.ColumnDefinitionCollection
                            .Where(o => o.TableName == tableName)
                            .FirstOrDefault(o => o.Identity)
                            ?.ColumnName,
                        factoryFrom: factoryFrom,
                        factoryTo: factoryTo));
        }

        private static void MigrateTable(
            string tableName,
            string identity,
            ISqlObjectFactory factoryFrom,
            ISqlObjectFactory factoryTo)
        {
            var connFrom = factoryFrom.CreateSqlConnection(
                connectionString: Parameters.Migration.SourceConnectionString);
            var connTo = factoryTo.CreateSqlConnection(
                connectionString: Parameters.Rds.OwnerConnectionString);
            using (connTo)
            {
                //この位置でコネクションをOpenする構造上、後続のMigrateTableメソッドでは、
                //プリザンターの主要なDB処理と異なり、SqlIo.csは使用せずにSQLコマンドを実行させる。
                connTo.OpenAsync();
                using (connFrom)
                {
                    connFrom.Open();
                    try
                    {
                        MigrateTable(
                            tableName: tableName,
                            identity: identity,
                            factoryFrom: factoryFrom,
                            factoryTo: factoryTo,
                            connFrom: connFrom,
                            connTo: connTo);
                        MigrateTable(
                            tableName: tableName + "_deleted",
                            identity: null,
                            factoryFrom: factoryFrom,
                            factoryTo: factoryTo,
                            connFrom: connFrom,
                            connTo: connTo);
                        MigrateTable(
                            tableName: tableName + "_history",
                            identity: null,
                            factoryFrom: factoryFrom,
                            factoryTo: factoryTo,
                            connFrom: connFrom,
                            connTo: connTo);
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        Consoles.Write(
                            text: $"[{e.Number}] {e.Message}",
                            type: Consoles.Types.Error,
                            abort: Parameters.Migration.AbortWhenException);
                    }
                    catch (System.Exception e)
                    {
                        Consoles.Write(
                            text: $"[{tableName}]: {e}",
                            type: Consoles.Types.Error,
                            abort: Parameters.Migration.AbortWhenException);
                    }
                    connFrom.Close();
                }
                connTo.Close();
            }
        }

        private static void MigrateTable(
            string tableName,
            string identity,
            ISqlObjectFactory factoryFrom,
            ISqlObjectFactory factoryTo,
            ISqlConnection connFrom,
            ISqlConnection connTo)
        {
            Consoles.Write(tableName, Consoles.Types.Info);

            var cmdFrom = factoryFrom.CreateSqlCommand(
                cmdText: Def.Sql.MigrateDatabaseSelectFrom
                    .Replace("#TableName#", tableName),
                connection: connFrom);
            SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdFrom, Sqls.LogsPath);
            using (var reader = cmdFrom.ExecuteReader())
            {
                while (reader.Read())
                {
                    var columns = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }
                    var partsColumnNames = columns.Select(columnName => "[" + columnName + "]").Join();
                    var partsValues = columns.Select(columnName => "@" + columnName).Join();
                    var cmdTo = factoryTo.CreateSqlCommand(
                        cmdText: Def.Sql.MigrateDatabaseInsert
                            .Replace("#TableName#", tableName)
                            .Replace("#ColumnNames#", partsColumnNames)
                            .Replace("#Values#", partsValues),
                        connection: connTo);
                    columns.ForEach(columnName =>
                        cmdTo.Parameters_AddWithValue(
                            "@ip" + columnName,
                            reader[columnName]));
                    SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
                    cmdTo.ExecuteNonQuery();
                }
            }
            if (Def.Sql.MigrateDatabaseSelectSetval != "" && identity != null)
            {
                var cmdTo = factoryTo.CreateSqlCommand(
                    cmdText: Def.Sql.MigrateDatabaseSelectSetval
                        .Replace("#TableName#", tableName)
                        .Replace("#Identity#", identity),
                    connection: connTo);
                SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
                cmdTo.ExecuteNonQuery();
            }
        }
    }
}