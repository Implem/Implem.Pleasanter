﻿using Implem.CodeDefiner.Functions.Rds.Parts.MySql;
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
                commandText: Def.Sql.Columns
                    .Replace("#TableName#", sourceTableName)
                    .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName)
                    .Replace("#InitialCatalog#", Environments.ServiceName))
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
            bool ColumnSizeHasChanges()
            {
                switch (Parameters.Rds.Dbms)
                {
                    case "SQLServer":
                    case "PostgreSQL":
                        if (ColumnSize.HasChanges(
                            factory: factory,
                            rdsColumn: rdsColumn,
                            columnDefinition: columnDefinition))
                        {
                            return true;
                        }
                        break;
                    case "MySQL":
                        if (MySqlColumnSize.HasChanges(
                            factory: factory,
                            rdsColumn: rdsColumn,
                            columnDefinition: columnDefinition))
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            if (!(rdsColumn["ColumnName"].ToString() == columnDefinition.ColumnName))
            {
                return true;
            }
            if (!(factory.SqlDataType.ConvertBack(rdsColumn["TypeName"].ToString()) == columnDefinition.TypeName))
            {
                return true;
            }
            if (ColumnSizeHasChanges())
            {
                return true;
            }
            if (rdsColumn["is_nullable"].ToBool() != columnDefinition.Nullable)
            {
                return true;
            }
            if (!sourceTableName.EndsWith("_history")
                && !sourceTableName.EndsWith("_deleted")
                && rdsColumn["is_identity"].ToBool() != columnDefinition.Identity)
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
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                case "PostgreSQL":
                    columnDefinitionCollection.ForEach(columnDefinition =>
                        sqlCreateColumnCollection.Add(Sql_Create(
                            factory: factory,
                            columnDefinition: columnDefinition,
                            noIdentity:
                                sourceTableName.EndsWith("_history")
                                || sourceTableName.EndsWith("_deleted"))));
                    break;
                case "MySQL":
                    columnDefinitionCollection.ForEach(columnDefinition =>
                        sqlCreateColumnCollection.Add(MySqlColumns.Sql_Create(
                            factory: factory,
                            columnDefinition: columnDefinition)));
                    break;
            }
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#Columns#", sqlCreateColumnCollection.Join(","));
        }

        private static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition,
            bool noIdentity)
        {
            var commandText = "\"{0}\" {1}".Params(columnDefinition.ColumnName, columnDefinition.TypeName);
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
            return factory.SqlDataType.Convert(commandText);
        }
    }
}
