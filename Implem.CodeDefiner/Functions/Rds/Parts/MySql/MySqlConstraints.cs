using CsvHelper;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    internal static class MySqlConstraints
    {
        internal static string CreateModifyColumnCommand(
            ISqlObjectFactory factory,
            string tableNameTemp,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string command)
        {
            bool NeedsDefault(ColumnDefinition o)
            {
                return !o.Default.IsNullOrEmpty() &&
                    !(tableNameTemp.EndsWith("_history") && o.ColumnName == "Ver");
            }
            bool NeedsAutoIncrement(ColumnDefinition o)
            {
                return o.Identity &&
                    !tableNameTemp.EndsWith("_history") &&
                    !tableNameTemp.EndsWith("_deleted");
            }
            string GetModifyColumnSqls(
                ISqlObjectFactory factory,
                ColumnDefinition columnDefinition,
                bool needsDefault,
                bool needsAutoIncrement,
                int seed)
            {
                return Def.Sql.ModifyColumn
                    .Replace("#ColumnDefinition#", MySqlColumns.Sql_Create(
                        factory: factory,
                        columnDefinition: columnDefinition))
                    .Replace("#Default#", needsDefault
                        ? " default " + Constraints.DefaultDefinition(factory, columnDefinition)
                        : string.Empty)
                    .Replace("#AutoIncrement#", needsAutoIncrement
                        ? " auto_increment"
                        : string.Empty)
                    .Replace("#SetSeed#", needsAutoIncrement
                        ? $"\r\nalter table \"#TableName#\" auto_increment = {seed};"
                        : string.Empty);
            }
            return command.Replace(
                "#ModifyColumn#", columnDefinitionCollection
                    .Where(o => NeedsDefault(o) || NeedsAutoIncrement(o))
                    .Select(o => GetModifyColumnSqls(
                        factory: factory,
                        columnDefinition: o,
                        needsDefault: NeedsDefault(o),
                        needsAutoIncrement: NeedsAutoIncrement(o),
                        seed: o.Seed == 0 ? 1 : o.Seed))
                    .JoinReturn());
        }

        internal static string DropConstraintCommand(
            ISqlObjectFactory factory,
            string sourceTableName,
            string command)
        {
            return command
                .Replace("#DropConstraint#", Indexes.Get(
                    factory: factory,
                    sourceTableName: sourceTableName)
                        .Where(o => o != "PRIMARY")
                        .Select(o => Def.Sql.DropIndex
                            .Replace("#IndexName#", o.ToString())
                            .Replace("#SourceTableName#", sourceTableName))
                        .JoinReturn());
        }
    }
}
