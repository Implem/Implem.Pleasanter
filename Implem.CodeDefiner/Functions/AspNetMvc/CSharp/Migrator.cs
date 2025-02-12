using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Migrator
    {
        private static int MigrateError = 0;
        private static StreamWriter MigrateLogWriter = null;

        public static bool MigrateDatabaseAsync(
            string logFolderName,
            string logNameText,
            ISqlObjectFactory factoryFrom,
            ISqlObjectFactory factoryTo)
        {
            MigrateLogWriter = new StreamWriter(Path.Combine(logFolderName, $"{logNameText}_Migrate.log"));
            using (MigrateLogWriter)
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
                if (MigrateError == 0)
                {
                    MigrateLogWriter.WriteLine("All data was successful.");
                }
                return MigrateError == 0;
            }
        }

        private static void MigrateTable(
            string tableName,
            string identity,
            ISqlObjectFactory factoryFrom,
            ISqlObjectFactory factoryTo)
        {
            using (var connTo = factoryTo.CreateSqlConnection(
                connectionString: Parameters.Rds.OwnerConnectionString))
            {
                connTo.Open();
                using (var connFrom = factoryFrom.CreateSqlConnection(
                    connectionString: Parameters.Migration.SourceConnectionString))
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
                    catch (Exception e)
                    {
                        MigrateError++;
                        var log = $@"[ERROR]    Table:""{tableName}""    {e.GetType()} caught.";
                        MigrateLogWriter.WriteLine(log);
                        MigrateLogWriter.Flush();
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
            var cmdFrom = factoryFrom.CreateSqlCommand();
            cmdFrom.CommandText = factoryFrom.Sqls.MigrateDatabaseSelectFrom(tableName);
            cmdFrom.Connection = connFrom;
            SqlDebugs.WriteSqlLog(Parameters.Migration.Dbms, Parameters.Migration.ServiceName, cmdFrom, Sqls.LogsPath);
            using (var reader = cmdFrom.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (connTo.State == ConnectionState.Closed)
                    {
                        connTo.Open();
                    }
                    var columns = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }
                    var cmdTo = factoryTo.CreateSqlCommand();
                    cmdTo.CommandText = Def.Sql.MigrateDatabaseInsert
                        .Replace("#TableName#", tableName)
                        .Replace(
                            "#ColumnNames#",
                            columns.Select(columnName => $@"""{columnName}""").Join())
                        .Replace(
                            "#Values#",
                            columns.Select(columnName => Parameters.Parameter.SqlParameterPrefix + columnName).Join());
                    columns.ForEach(columnName => cmdTo.Parameters_AddWithValue(
                        parameterName: Parameters.Parameter.SqlParameterPrefix + columnName,
                        value: reader[columnName]));
                    SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
                    cmdTo.Connection = connTo;
                    try
                    {
                        cmdTo.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MigrateError++;
                        var log = $@"[ERROR]    Table:""{tableName}""    Data:";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            object obj = reader[reader.GetName(i)];
                            if (obj != null && obj is DateTime)
                            {
                                obj = ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            else if (obj != null && obj.ToString().Length > 1024)
                            {
                                obj = $"{obj.ToString()[..1024]} [text truncated]";
                            }
                            log = log + $@"""{reader.GetName(i)}""={obj}, ";
                        }
                        MigrateLogWriter.WriteLine(log.Remove(log.Length - 2));
                        MigrateLogWriter.Flush();
                        Consoles.Write(
                            text: $"[{tableName}]: {e}",
                            type: Consoles.Types.Error,
                            abort: Parameters.Migration.AbortWhenException);
                    }
                }
            }
            if (Def.Sql.MigrateDatabaseSelectSetval != "" && identity != null)
            {
                var cmdTo = factoryTo.CreateSqlCommand();
                cmdTo.CommandText = Def.Sql.MigrateDatabaseSelectSetval
                    .Replace("#TableName#", tableName)
                    .Replace("#Identity#", identity);
                cmdTo.Connection = connTo;
                SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
                cmdTo.ExecuteNonQuery();
            }
        }
    }
}