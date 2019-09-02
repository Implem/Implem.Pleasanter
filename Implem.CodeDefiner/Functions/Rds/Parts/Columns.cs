using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class Columns
    {
        internal static EnumerableRowCollection<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Columns.Replace("#TableName#", sourceTableName))
                .AsEnumerable();
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName, 
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            EnumerableRowCollection<DataRow> rdsColumnCollection)
        {
            return columnDefinitionCollection
                .Select((o, i) => new { ColumnDefinition = o, Count = i })
                .Any(data => HasChanges(
                    factory,
                    sourceTableName,
                    rdsColumnCollection.ToList()[data.Count],
                    data.ColumnDefinition));
        }

        private static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName,
            DataRow rdsColumn,
            ColumnDefinition columnDefinition)
        {
            if (!(rdsColumn["ColumnName"].ToString() == columnDefinition.ColumnName))
            {
                return true;
            }
            if (!(factory.SqlDataTypes.ConvertBack(rdsColumn["TypeName"].ToString()) == columnDefinition.TypeName))
            {
                return true;
            }
            if (ColumnSize.HasChanges(rdsColumn, columnDefinition))
            {
                return true;
            }
            if (rdsColumn["is_nullable"].ToBool() != columnDefinition.Nullable)
            {
                return true;
            }
            if (!sourceTableName.EndsWith("_history") &&
                !sourceTableName.EndsWith("_deleted") &&
                rdsColumn["is_identity"].ToBool() != columnDefinition.Identity)
            {
                return true;
            }
            return false;
        }

        internal static void CreateColumn(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            var sqlCreateColumnCollection = new List<string>();
            columnDefinitionCollection.ForEach(columnDefinition =>
                sqlCreateColumnCollection.Add(Sql_Create(
                    factory,
                    columnDefinition,
                    noIdentity: 
                        sourceTableName.EndsWith("_history") ||
                        sourceTableName.EndsWith("_deleted"))));
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#Columns#", sqlCreateColumnCollection.Join(","));
        }

        private static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition,
            bool noIdentity)
        {
            var commandText = string.Empty;
            commandText = "\"{0}\" {1}".Params(
                columnDefinition.ColumnName, columnDefinition.TypeName);
            if (columnDefinition.MaxLength == -1)
            {
                commandText += "(max)";
            }
            else if (columnDefinition.TypeName == "decimal")
            {
                commandText += "(" + columnDefinition.Size + ")";
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
                // TODO 初期値 0 不可
                commandText += factory.Sqls.GenerateIdentity.Params(columnDefinition.Seed == 0 ? 1 : columnDefinition.Seed);
            }
            if (columnDefinition.Nullable)
            {
                commandText += " null";
            }
            else
            {
                commandText += " not null";
            }
            return factory.SqlDataTypes.Convert(commandText);
        }
    }
}
