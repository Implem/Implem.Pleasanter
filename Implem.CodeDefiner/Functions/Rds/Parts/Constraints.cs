using Implem.CodeDefiner.Functions.Rds.Parts.MySql;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class Constraints
    {
        internal const string DecimalFormat = "0.############################";

        internal static EnumerableRowCollection<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Defaults
                .Replace("#InitialCatalog#", Environments.ServiceName)
                .Replace("#TableName#", sourceTableName))
                    .AsEnumerable();
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            return Differences(
                factory: factory,
                sourceTableName: sourceTableName,
                columnDefinitionCollection: columnDefinitionCollection)
                    .Any();
        }

        internal static IEnumerable<string> Differences(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            var definitionDefaults = columnDefinitionCollection
                .Where(o => !o.Default.IsNullOrEmpty())
                .Where(o => !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver"))
                .Select(o => o.ColumnName + "," + NormalizedDefault(
                    factory: factory,
                    typeName: o.TypeName,
                    value: o.Default))
                .ToList();
            var rdsDefaults = Get(factory: factory, sourceTableName: sourceTableName)
                .Where(o => !(sourceTableName.EndsWith("_history") && o["column_name"].ToString() == "Ver"))
                .Select(o => o["column_name"] + "," + NormalizedDefault(
                    factory: factory,
                    typeName: columnDefinitionCollection
                        .FirstOrDefault(columnDefinition =>
                            columnDefinition.ColumnName == o["column_name"].ToString())
                        ?.TypeName,
                    value: factory.SqlDataType.DefaultValue(o)))
                .ToList();
            return definitionDefaults
                .Except(rdsDefaults)
                .Select(o => $"definition: {o}")
                .Concat(rdsDefaults
                    .Except(definitionDefaults)
                    .Select(o => $"database: {o}"))
                .OrderBy(o => o, StringComparer.Ordinal)
                .ToList();
        }

        internal static string NormalizedDefault(
            ISqlObjectFactory factory,
            string typeName,
            string value)
        {
            switch (typeName?.CsTypeSummary())
            {
                case Types.CsNumeric:
                    return decimal.TryParse(
                        value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var number)
                            ? number.ToString(DecimalFormat, CultureInfo.InvariantCulture)
                            : value;
                case Types.CsBool:
                    switch (value.ToLower())
                    {
                        case "1":
                        case "true":
                            return "1";
                        case "0":
                        case "false":
                            return "0";
                        default:
                            return value;
                    }
                case Types.CsDateTime:
                    return value.ToLower() == "now"
                        || value.ToLower() == factory.Sqls.CurrentDateTime.Trim().ToLower()
                            ? "now"
                            : value;
                default:
                    return value;
            }
        }

        internal static void CreateDefault(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string tableNameTemp,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                case "PostgreSQL":
                    sqlStatement.CommandText = CreateDefaultCommand(
                        factory: factory,
                        tableNameTemp: tableNameTemp,
                        columnDefinitionCollection: columnDefinitionCollection,
                        command: sqlStatement.CommandText);
                    break;
                case "MySQL":
                    sqlStatement.CommandText = MySqlConstraints.CreateModifyColumnCommand(
                        factory: factory,
                        tableNameTemp: tableNameTemp,
                        columnDefinitionCollection: columnDefinitionCollection,
                        command: sqlStatement.CommandText);
                    break;
            }
        }

        private static string CreateDefaultCommand(
            ISqlObjectFactory factory,
            string tableNameTemp,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string command)
        {
            return command.Replace(
                "#Defaults#", columnDefinitionCollection
                    .Where(o => !o.Default.IsNullOrEmpty())
                    .Where(o => !(tableNameTemp.EndsWith("_history") && o.ColumnName == "Ver"))
                    .Select(o => Sql_Create(factory, Def.Sql.CreateDefault, tableNameTemp, o))
                    .JoinReturn());
        }

        private static string Sql_Create(
            ISqlObjectFactory factory,
            string sqlCreateDefault,
            string sourceTableName,
            ColumnDefinition columnDefinition)
        {
            return sqlCreateDefault
                .Replace("#DefaultName#", sourceTableName + Strings.NewGuid())
                .Replace("#ColumnName#", columnDefinition.ColumnName)
                .Replace("#DefaultValue#", DefaultDefinition(factory, columnDefinition));
        }

        internal static string DefaultDefinition(ISqlObjectFactory factory, ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName.CsTypeSummary())
            {
                case Types.CsString:
                    return factory.SqlDataType.StringLiteral(columnDefinition.Default);
                case Types.CsDateTime:
                    if (columnDefinition.Default?.ToLower() == "now")
                    {
                        return factory.Sqls.CurrentDateTime.Trim();
                    }
                    else
                    {
                        return columnDefinition.Default;
                    }
                case Types.CsBool:
                    return factory.Sqls.BooleanString(columnDefinition.Default);
                default:
                    return columnDefinition.Default;
            }
        }

        internal static void DropConstraint(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                case "PostgreSQL":
                    sqlStatement.CommandText = DropConstraintCommand(
                        factory: factory,
                        sourceTableName: sourceTableName,
                        tableIndexCollection: tableIndexCollection,
                        command: sqlStatement.CommandText);
                    break;
                case "MySQL":
                    sqlStatement.CommandText = MySqlConstraints.DropConstraintCommand(
                        factory: factory,
                        sourceTableName: sourceTableName,
                        command: sqlStatement.CommandText);
                    break;
            }
        }

        private static string DropConstraintCommand(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<IndexInfo> tableIndexCollection,
            string command)
        {
            return command
                .Replace("#DropConstraint#", tableIndexCollection
                    .Where(o => Indexes.Get(
                        factory: factory,
                        sourceTableName: sourceTableName)
                        .Contains(o.IndexName()))
                    .Select(o => Sql_Drop(o)
                        .Replace("#SourceTableName#", sourceTableName)
                        .Replace("#IndexName#", o.IndexName()))
                    .Join("\r\n"));
        }

        internal static string Sql_Drop(IndexInfo o)
        {
            return o.Type == IndexInfo.Types.Pk
                ? Def.Sql.DropConstraint
                : Def.Sql.DropIndex;
        }
    }
}
