using Implem.DefinitionAccessor;
using Implem.IRds;
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
                    catch (Exception e)
                    {
                        Consoles.Write(tableName + ":" + e.Message, Consoles.Types.Info);
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
                cmdText: $"select * from [{tableName}];",
                connection: connFrom);
            using (var reader = cmdFrom.ExecuteReader())
            {
                while (reader.Read())
                {
                    var columns = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }
                    var cmdTo = factoryTo.CreateSqlCommand(
                        cmdText:
                            $"insert into [{tableName}]" +
                            $"({columns.Select(columnName => "[" + columnName + "]").Join()})" +
                            $"values" +
                            $"({columns.Select(columnName => "@" + columnName).Join()});",
                        connection: connTo);

                    columns.ForEach(columnName =>
                        cmdTo.Parameters_AddWithValue(
                            "@ip" + columnName,
                            reader[columnName]));
                    cmdTo.ExecuteNonQuery();
                }
            }
            if (identity != null)
            {
                var cmdTo = factoryTo.CreateSqlCommand(
                    cmdText: $"select setval('{tableName}_{identity}_seq', (select max({identity}) from {tableName}));",
                    connection: connTo);
                cmdTo.ExecuteNonQuery();
            }
        }
    }
}