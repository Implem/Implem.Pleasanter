using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class QuartzIndexes
    {
        private static bool IsQuartzTable(string sourceTableName, Sqls.TableTypes tableType)
        {
            return Parameters.Quartz?.Clustering?.Enabled == true
                && tableType == Sqls.TableTypes.Normal
                && Tables.IsQuartzTable(sourceTableName);
        }

        private static StringComparer IndexNameComparer()
        {
            return Parameters.Rds.Dbms == "PostgreSQL"
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase;
        }

        internal static IEnumerable<IndexInfo> Definitions(
            ISqlObjectFactory factory,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            if (!IsQuartzTable(sourceTableName, tableType))
            {
                yield break;
            }
            var tablePrefix = Parameters.Quartz.Clustering.TablePrefix;
            var tableName = sourceTableName[tablePrefix.Length..].ToUpperInvariant();
            var isPostgreSql = Parameters.Rds.Dbms == "PostgreSQL";
            foreach (var definition in Definitions(tableName))
            {
                var indexName = tableName == "BLOB_TRIGGERS"
                    ? definition.Name
                    : $"IDX_{tablePrefix}{definition.Name}";
                yield return new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Quartz",
                    columnCollection: [.. definition.Columns.Select((column, index) =>
                        new IndexInfo.Column(
                            isPostgreSql ? column.ToLowerInvariant() : column,
                            index + 1,
                            "asc"))],
                    fixedName: isPostgreSql ? indexName.ToLowerInvariant() : indexName);
            }
        }

        private static (string Name, string[] Columns)[] Definitions(string tableName)
        {
            return (Parameters.Rds.Dbms, tableName) switch
            {
                ("SQLServer", "TRIGGERS") =>
                [
                    ("T_G_J", ["SCHED_NAME", "JOB_GROUP", "JOB_NAME"]),
                    ("T_C", ["SCHED_NAME", "CALENDAR_NAME"]),
                    ("T_N_G_STATE", ["SCHED_NAME", "TRIGGER_GROUP", "TRIGGER_STATE"]),
                    ("T_STATE", ["SCHED_NAME", "TRIGGER_STATE"]),
                    ("T_N_STATE", ["SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP", "TRIGGER_STATE"]),
                    ("T_NEXT_FIRE_TIME", ["SCHED_NAME", "NEXT_FIRE_TIME"]),
                    ("T_NFT_ST", ["SCHED_NAME", "TRIGGER_STATE", "NEXT_FIRE_TIME"]),
                    ("T_NFT_ST_MISFIRE",
                        ["SCHED_NAME", "MISFIRE_INSTR", "NEXT_FIRE_TIME", "TRIGGER_STATE"]),
                    ("T_NFT_ST_MISFIRE_GRP",
                        ["SCHED_NAME", "MISFIRE_INSTR", "NEXT_FIRE_TIME",
                            "TRIGGER_GROUP", "TRIGGER_STATE"]),
                ],
                ("SQLServer", "FIRED_TRIGGERS") =>
                [
                    ("FT_INST_JOB_REQ_RCVRY", ["SCHED_NAME", "INSTANCE_NAME", "REQUESTS_RECOVERY"]),
                    ("FT_G_J", ["SCHED_NAME", "JOB_GROUP", "JOB_NAME"]),
                    ("FT_G_T", ["SCHED_NAME", "TRIGGER_GROUP", "TRIGGER_NAME"]),
                ],
                ("PostgreSQL", "JOB_DETAILS") =>
                [
                    ("J_REQ_RECOVERY", ["REQUESTS_RECOVERY"]),
                ],
                ("PostgreSQL", "TRIGGERS") =>
                [
                    ("T_NEXT_FIRE_TIME", ["NEXT_FIRE_TIME"]),
                    ("T_STATE", ["TRIGGER_STATE"]),
                    ("T_NFT_ST", ["NEXT_FIRE_TIME", "TRIGGER_STATE"]),
                ],
                ("PostgreSQL", "FIRED_TRIGGERS") =>
                [
                    ("FT_TRIG_NAME", ["TRIGGER_NAME"]),
                    ("FT_TRIG_GROUP", ["TRIGGER_GROUP"]),
                    ("FT_TRIG_NM_GP", ["SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP"]),
                    ("FT_TRIG_INST_NAME", ["INSTANCE_NAME"]),
                    ("FT_JOB_NAME", ["JOB_NAME"]),
                    ("FT_JOB_GROUP", ["JOB_GROUP"]),
                    ("FT_JOB_REQ_RECOVERY", ["REQUESTS_RECOVERY"]),
                ],
                ("MySQL", "JOB_DETAILS") =>
                [
                    ("J_REQ_RECOVERY", ["SCHED_NAME", "REQUESTS_RECOVERY"]),
                    ("J_GRP", ["SCHED_NAME", "JOB_GROUP"]),
                ],
                ("MySQL", "TRIGGERS") =>
                [
                    ("T_J", ["SCHED_NAME", "JOB_NAME", "JOB_GROUP"]),
                    ("T_JG", ["SCHED_NAME", "JOB_GROUP"]),
                    ("T_C", ["SCHED_NAME", "CALENDAR_NAME"]),
                    ("T_G", ["SCHED_NAME", "TRIGGER_GROUP"]),
                    ("T_STATE", ["SCHED_NAME", "TRIGGER_STATE"]),
                    ("T_N_STATE", ["SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP", "TRIGGER_STATE"]),
                    ("T_N_G_STATE", ["SCHED_NAME", "TRIGGER_GROUP", "TRIGGER_STATE"]),
                    ("T_NEXT_FIRE_TIME", ["SCHED_NAME", "NEXT_FIRE_TIME"]),
                    ("T_NFT_ST", ["SCHED_NAME", "TRIGGER_STATE", "NEXT_FIRE_TIME"]),
                    ("T_NFT_MISFIRE", ["SCHED_NAME", "MISFIRE_INSTR", "NEXT_FIRE_TIME"]),
                    ("T_NFT_ST_MISFIRE",
                        ["SCHED_NAME", "MISFIRE_INSTR", "NEXT_FIRE_TIME", "TRIGGER_STATE"]),
                    ("T_NFT_ST_MISFIRE_GRP",
                        ["SCHED_NAME", "MISFIRE_INSTR", "NEXT_FIRE_TIME",
                            "TRIGGER_GROUP", "TRIGGER_STATE"]),
                ],
                ("MySQL", "FIRED_TRIGGERS") =>
                [
                    ("FT_TRIG_INST_NAME", ["SCHED_NAME", "INSTANCE_NAME"]),
                    ("FT_INST_JOB_REQ_RCVRY", ["SCHED_NAME", "INSTANCE_NAME", "REQUESTS_RECOVERY"]),
                    ("FT_J_G", ["SCHED_NAME", "JOB_NAME", "JOB_GROUP"]),
                    ("FT_JG", ["SCHED_NAME", "JOB_GROUP"]),
                    ("FT_T_G", ["SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP"]),
                    ("FT_TG", ["SCHED_NAME", "TRIGGER_GROUP"]),
                ],
                ("MySQL", "BLOB_TRIGGERS") =>
                [
                    ("SCHED_NAME", ["SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP"]),
                ],
                _ => []
            };
        }

        internal static bool HasChangesPrimaryKey(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            if (!IsQuartzTable(sourceTableName, tableType))
            {
                return false;
            }
            var primaryKey = Indexes.IndexInfoCollection(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: tableType).FirstOrDefault(index => index.Type == IndexInfo.Types.Pk);
            if (primaryKey == null)
            {
                return false;
            }
            if (Parameters.Rds.Dbms == "MySQL")
            {
                var columns = Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                    factory: factory,
                    commandText: Def.Sql.Indexes
                        .Replace("#TableName#", sourceTableName)
                        .Replace("#InitialCatalog#", Environments.ServiceName))
                    .AsEnumerable()
                    .Where(row => row["Name"].ToString() == "PRIMARY")
                    .OrderBy(row => row["No"].ToInt())
                    .Select(row => row["ColumnName"] + "," + row["OrderType"])
                    .Join(",");
                return !string.Equals(
                    primaryKey.IndexInfoString(), columns, StringComparison.OrdinalIgnoreCase);
            }
            return !Indexes.Get(factory: factory, sourceTableName: sourceTableName)
                .Contains(primaryKey.IndexName(), IndexNameComparer());
        }

        internal static List<string> MissingIndexNames(
            ISqlObjectFactory factory,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<string> existingNames)
        {
            if (!IsQuartzTable(sourceTableName, tableType))
            {
                return [];
            }
            var names = new HashSet<string>(existingNames, IndexNameComparer());
            return [.. Definitions(
                factory: factory,
                sourceTableName: sourceTableName,
                tableType: tableType)
                .Select(index => index.IndexName())
                .Where(name => !names.Contains(name))];
        }

        internal static bool Configure(
            ISqlObjectFactory factory,
            string sourceTableName,
            Sqls.TableTypes tableType,
            bool checkMigration)
        {
            var definitions = Definitions(
                factory: factory,
                sourceTableName: sourceTableName,
                tableType: tableType).ToList();
            if (definitions.Count == 0)
            {
                return false;
            }
            var missingNames = MissingIndexNames(
                factory: factory,
                sourceTableName: sourceTableName,
                tableType: tableType,
                existingNames: Indexes.Get(factory: factory, sourceTableName: sourceTableName));
            foreach (var index in definitions
                .Where(index => missingNames.Contains(index.IndexName())))
            {
                Consoles.Write($"{sourceTableName}: {index.IndexName()}", Consoles.Types.Info);
                if (checkMigration)
                {
                    continue;
                }
                Def.SqlIoByAdmin(factory: factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText: Indexes.CreateIxSql(index)
                        .Replace("#TableName#", sourceTableName));
            }
            return missingNames.Count != 0;
        }
    }
}
