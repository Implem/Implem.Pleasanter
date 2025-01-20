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
            //この位置でコネクションをOpenする構造上、後続のMigrateTableメソッドでは、
            //プリザンターの主要なDB処理と異なり、SqlIo.csは使用せずにSQLコマンドを実行させる。
            using (var connTo = factoryTo.CreateSqlConnection(
                connectionString: Parameters.Rds.OwnerConnectionString))
            {
                connTo.OpenAsync();
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
            var cmdFrom = factoryFrom.CreateSqlCommand();
            cmdFrom.CommandText = factoryFrom.Sqls.MigrateDatabaseSelectFrom(tableName);
            cmdFrom.Connection = connFrom;
            SqlDebugs.WriteSqlLog(Parameters.Migration.Dbms, Parameters.Migration.ServiceName, cmdFrom, Sqls.LogsPath);
            using (var reader = cmdFrom.ExecuteReader())
            {
                while (reader.Read())
                {
                    var columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }
                    try
                    {
                        ExecuteInsertInto(
                            tableName: tableName,
                            columnNames: columnNames,
                            factoryTo: factoryTo,
                            connTo: connTo,
                            reader: reader);
                        if (Def.Sql.MigrateDatabaseSelectSetval != "" && identity != null)
                        {
                            ExecuteUpdateSequense(
                                tableName: tableName,
                                identity: identity,
                                factoryTo: factoryTo,
                                connTo: connTo);
                        }
                    }
                    //★catchするエラーの種類は実際にDBMS毎に調査必要。メッセージ本文もみるかも。
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        //★エラーレコード一覧ファイルの書き出し処理。①Migrate失敗と書き出す。※readerを参照して書き出す。

                        if (Parameters.Migration.InsertIfOverflow)
                        {
                            InsertOnlyPrimaryKey(
                                tableName: tableName,
                                identity: identity,
                                factoryTo: factoryTo,
                                connTo: connTo,
                                reader: reader);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (System.Exception e)
                    {
                        throw;
                    }
                }
            }
        }

        private static void ExecuteInsertInto(
            string tableName,
            List<string> columnNames,
            ISqlObjectFactory factoryTo,
            ISqlConnection connTo,
            IDataReader reader)
        {
            var cmdTo = factoryTo.CreateSqlCommand();
            cmdTo.CommandText = Def.Sql.MigrateDatabaseInsert
                .Replace("#TableName#", tableName)
                .Replace(
                    "#ColumnNames#",
                    columnNames.Select(columnName => $@"""{columnName}""").Join())
                .Replace(
                    "#Values#",
                    columnNames.Select(columnName => Parameters.Parameter.SqlParameterPrefix + columnName).Join());
            columnNames.ForEach(columnName => cmdTo.Parameters_AddWithValue(
                parameterName: Parameters.Parameter.SqlParameterPrefix + columnName,
                value: reader[columnName]));
            SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
            cmdTo.Connection = connTo;
            cmdTo.ExecuteNonQuery();
        }

        private static void ExecuteUpdateSequense(
            string tableName,
            string identity,
            ISqlObjectFactory factoryTo,
            ISqlConnection connTo)
        {
            var cmdTo = factoryTo.CreateSqlCommand();
            cmdTo.CommandText = Def.Sql.MigrateDatabaseSelectSetval
            .Replace("#TableName#", tableName)
                .Replace("#Identity#", identity);
            cmdTo.Connection = connTo;
            SqlDebugs.WriteSqlLog(Parameters.Rds.Dbms, Environments.ServiceName, cmdTo, Sqls.LogsPath);
            cmdTo.ExecuteNonQuery();
        }

        private static void InsertOnlyPrimaryKey(
            string tableName,
            string identity,
            ISqlObjectFactory factoryTo,
            ISqlConnection connTo,
            IDataReader reader)
        {
            var pkColumns = Def.ColumnDefinitionCollection
                    .Where(columnDefinition => columnDefinition.TableName == tableName);
            if (Parameters.Rds.Dbms != "MySQL" && !tableName.EndsWith("_history"))
            {
                pkColumns = pkColumns.Where(columnDefinition => columnDefinition.Pk < 0);
            }
            else if (Parameters.Rds.Dbms != "MySQL" && tableName.EndsWith("_history"))
            {
                pkColumns = pkColumns.Where(columnDefinition => columnDefinition.PkHistory < 0);
            }
            else if (Parameters.Rds.Dbms == "MySQL" && !tableName.EndsWith("_history"))
            {
                pkColumns = pkColumns.Where(columnDefinition => columnDefinition.PkMySql < 0);
            }
            else if (tableName.EndsWith("_history"))
            {
                pkColumns = pkColumns.Where(columnDefinition => columnDefinition.PkHistoryMySql < 0);
            }
            var columnNames = new List<string>();
            pkColumns.ForEach(columnDefinition => columnNames.Add(columnDefinition.ColumnName));
            try
            {
                ExecuteInsertInto(
                    tableName: tableName,
                    columnNames: columnNames,
                    factoryTo: factoryTo,
                    connTo: connTo,
                    reader: reader);
                if (Def.Sql.MigrateDatabaseSelectSetval != "" && identity != null)
                {
                    ExecuteUpdateSequense(
                    tableName: tableName,
                        identity: identity,
                        factoryTo: factoryTo,
                        connTo: connTo);
                    //★エラーレコード一覧ファイルの書き出し処理。②-a PKのみInsert済みと書き出す。
                }
            }
            //★catchするエラーの種類は実際にDBMS毎に調査必要。メッセージ本文もみるかも。
            catch (System.Data.SqlClient.SqlException e)
            {
                //★エラーレコード一覧ファイルの書き出し処理。②-b InsertOnlyPrimaryKey失敗と書き出す。
                throw;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }
    }
}