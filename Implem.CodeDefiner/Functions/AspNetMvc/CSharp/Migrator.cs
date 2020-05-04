using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Migrator
    {
        public static void MigrateDatabaseAsync()
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
                    MigrateTable(tableName: tableName));
        }

        private static void MigrateTable(string tableName)
        {
            using (var pconn = new NpgsqlConnection(Parameters.Rds.UserConnectionString))
            {
                pconn.OpenAsync();
                using (var sconn = new SqlConnection(Parameters.Migration.SourceConnectionString))
                {
                    sconn.Open();
                    try
                    {
                        MigrateTable(tableName, pconn, sconn);
                        MigrateTable(tableName + "_deleted", pconn, sconn);
                        MigrateTable(tableName + "_history", pconn, sconn);
                    }
                    catch (System.Exception e)
                    {
                        Consoles.Write(tableName + ":" + e.Message, Consoles.Types.Info);
                    }
                    sconn.Close();
                }
                pconn.Close();
            }
        }

        private static void MigrateTable(
            string tableName,
            NpgsqlConnection pconn,
            SqlConnection sconn,
            string suffix = "")
        {
            Consoles.Write(tableName, Consoles.Types.Info);
            var scmd = new SqlCommand(
                cmdText: $"select * from [{tableName}];",
                connection: sconn);
            using (var reader = scmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var columns = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));
                    }
                    var pcmd = new NpgsqlCommand(
                        cmdText:
                            $"insert into \"{tableName}\"" +
                            $"({columns.Select(columnName => "\"" + columnName + "\"").Join()})" +
                            $"values" +
                            $"({columns.Select(columnName => "@ip" + columnName).Join()});",
                        connection: pconn);
                    columns.ForEach(columnName =>
                        pcmd.Parameters.AddWithValue(
                            "@ip" + columnName,
                            reader[columnName]));
                    pcmd.ExecuteNonQuery();
                }
            }
        }
    }
}