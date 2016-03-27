using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class Columns
    {
        internal static EnumerableRowCollection<DataRow> Get(
            string sourceTableName)
        {
            return Def.GetSqlIoOfAdmin().ExecuteTable(
                Def.Code.Sql_GetColumns.Replace("#TableName#", sourceTableName),
                SqlCmd.Types.PlainSql).AsEnumerable();
        }

        internal static bool HasDifference(
            string sourceTableName, 
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> dbColumnCollection)
        {
            return columnDefinitionCollection
                .Select((o, i) => new { ColumnDefinition = o, Count = i })
                .Any(data => HasDifference(
                    sourceTableName,
                    dbColumnCollection.ToList()[data.Count],
                    data.ColumnDefinition));
        }

        private static bool HasDifference(
            string sourceTableName, DataRow dbColumn, ColumnDefinition columnDefinition)
        {
            if (!dbColumn["ColumnName"].Equals(columnDefinition.ColumnName))
            {
                return true;
            }
            if (!dbColumn["TypeName"].Equals(columnDefinition.TypeName))
            {
                return true;
            }
            if (ColumnSize.HasDifference(dbColumn, columnDefinition))
            {
                return true;
            }
            if (dbColumn["is_nullable"].ToBool() != columnDefinition.Nullable)
            {
                return true;
            }
            if (!sourceTableName.EndsWith("_old") &&
                !sourceTableName.EndsWith("_deleted") &&
                dbColumn["is_identity"].ToBool() != columnDefinition.Identity)
            {
                return true;
            }
            return false;
        }

        internal static void CreateColumn(
            this SqlCmd sqlCmd,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            var sqlCreateColumnCollection = new List<string>();
            columnDefinitionCollection.ForEach(columnDefinition =>
                sqlCreateColumnCollection.Add(Columns.Sql_Create(
                    columnDefinition,
                    noIdentity: sourceTableName.EndsWith("_old") || sourceTableName.EndsWith("_deleted"))));
            sqlCmd.CommandText = sqlCmd.CommandText.Replace(
                "#Columns#", sqlCreateColumnCollection.Join(","));
        }

        private static string Sql_Create(
            ColumnDefinition columnDefinition, bool noIdentity)
        {
            var commandText = string.Empty;
            commandText = "[{0}] [{1}]".Params(
                columnDefinition.ColumnName, columnDefinition.TypeName);
            if (columnDefinition.MaxLength == -1)
            {
                commandText += "(max)";
            }
            else
            {
                if (columnDefinition.MaxLength != 0)
                {
                    commandText += "({0})".Params(columnDefinition.MaxLength);
                }
            }
            if (!noIdentity && columnDefinition.Identity)
            {
                commandText += " identity({0}, 1)".Params(columnDefinition.Seed);
            }
            if (columnDefinition.Nullable)
            {
                commandText += " null";
            }
            else
            {
                commandText += " not null";
            }
            return commandText;
        }
    }
}
