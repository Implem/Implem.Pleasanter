using Implem.CodeDefiner.Functions.Rds.Parts;
using Implem.CodeDefiner.Functions.Rds.Parts.PostgreSql;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal class TablesConfigurator
    {
        /// <summary>
        /// ベースカラムのカラム名セットを取得（キャッシュ）
        /// </summary>
        private static HashSet<string> BaseColumnNameCache = null;

        private static bool IsSkipQuartzTable(string tableName)
        {
            var enableClustering = Parameters.Quartz?.Clustering?.Enabled ?? false;

            return !enableClustering && Tables.IsQuartzTable(tableName);
        }

        private static bool IsPostgreSqlQuartzTable(string tableName)
        {
            return Parameters.Rds.Dbms == "PostgreSQL" && Tables.IsQuartzTable(tableName);
        }

        private static string NormalizeTableName(string tableName)
        {
            if (IsPostgreSqlQuartzTable(tableName))
            {
                //PostgreSQLのQRTZテーブルは、小文字で作成されるため、小文字に変更
                return tableName.ToLower();
            }
            return tableName;
        }

        internal static bool Configure(
            ISqlObjectFactory factory,
            bool checkMigration = false)
        {
            var isChanged = false;
            Def.TableNameCollection().ForEach(generalTableName =>
            {
                try
                {
                    isChanged |= ConfigureTableSet(
                        factory: factory,
                        generalTableName: generalTableName,
                        checkMigration: checkMigration);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    Consoles.Write($"[{e.Number}] {e.Message}", Consoles.Types.Error);
                }
                catch (System.Exception e)
                {
                    Consoles.Write($"[{generalTableName}]: {e}", Consoles.Types.Error);
                }
            });
            try
            {
                switch (Parameters.Rds.Dbms)
                {
                    case "SQLServer":
                        ConfigureFullTextIndexSqlServer(factory: factory);
                        break;
                    case "PostgreSQL":
                        ConfigureFullTextIndexPostgreSql(factory: factory);
                        break;
                    case "MySQL":
                        ConfigureFullTextIndexMySql(factory: factory);
                        break;
                }
            }
            catch (System.Exception e)
            {
                Consoles.Write($"{e.Message}", Consoles.Types.Error);
            }
            return isChanged;
        }

        private static void ConfigureFullTextIndexSqlServer(ISqlObjectFactory factory)
        {
            try
            {
                var pkItems = Def.SqlIoByAdmin(
                    factory: factory,
                    statements: new SqlStatement(Def.Sql.SelectPkName.Replace("#TableName#", "Items")))
                        .ExecuteScalar_string(factory: factory, dbTransaction: null, dbConnection: null);
                var pkBinaries = Def.SqlIoByAdmin(
                    factory: factory,
                    statements: new SqlStatement(Def.Sql.SelectPkName.Replace("#TableName#", "Binaries")))
                        .ExecuteScalar_string(factory: factory, dbTransaction: null, dbConnection: null);
                Def.SqlIoBySa(factory: factory, initialCatalog: Environments.ServiceName)
                    .ExecuteNonQuery(
                        factory: factory,
                        dbTransaction: null,
                        dbConnection: null,
                        commandText: Def.Sql.CreateFullText
                            .Replace("#PKItems#", pkItems)
                            .Replace("#PKBinaries#", pkBinaries));
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write($"[{e.Number}] [{nameof(ConfigureFullTextIndexSqlServer)}]: {e}", Consoles.Types.Error);
            }
        }

        private static void ConfigureFullTextIndexPostgreSql(ISqlObjectFactory factory)
        {
            try
            {
                if (!factory.SqlDefinitionSetting.IsCreatingDb)
                {
                    return;
                }
                Def.SqlIoByAdmin(factory: factory)
                    .ExecuteNonQuery(
                        factory: factory,
                        dbTransaction: null,
                        dbConnection: null,
                        commandText: Def.Sql.CreateFullText);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write($"[{e.Number}] [{nameof(ConfigureFullTextIndexPostgreSql)}]: {e}", Consoles.Types.Error);
            }
        }

        private static void ConfigureFullTextIndexMySql(ISqlObjectFactory factory)
        {
            bool Exists()
            {
                return Def.SqlIoByAdmin(factory: factory)
                    .ExecuteTable(
                        factory: factory,
                        commandText: Def.Sql.ExistsFullText
                            .Replace("#InitialCatalog#", Environments.ServiceName))
                    .Rows.Count == 1;
            }
            if (!Exists())
            {
                Def.SqlIoByAdmin(factory: factory)
                    .ExecuteNonQuery(
                        factory: factory,
                        dbTransaction: null,
                        dbConnection: null,
                        commandText: Def.Sql.CreateFullText);
            }
        }

        private static bool ConfigureTableSet(
            ISqlObjectFactory factory,
            string generalTableName,
            bool checkMigration)
        {
            if (IsSkipQuartzTable(generalTableName))
            {
                // Quartzのクラスタリングが無効な場合、QRTZ_系のテーブルはスキップ
                return true;
            }

            Consoles.Write(generalTableName, Consoles.Types.Info);
            // PostgreSQLのQRTZテーブルは、小文字で作成する必要がある。
            var sourceTableName = NormalizeTableName(generalTableName);
            var deletedTableName = sourceTableName + "_deleted";
            var historyTableName = sourceTableName + "_history";
            var columnDefinitionCollection = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == generalTableName)
                .Where(o => !o.NotUpdate)
                .Where(o => o.JoinTableName.IsNullOrEmpty())
                .Where(o => o.Calc.IsNullOrEmpty())
                .Where(o => !o.LowSchemaVersion())
                // QRTZ_系のテーブルは、ベースカラムを必要としない
                .Where(o => ShouldIncludeColumn(generalTableName, o))
                .OrderBy(o => o.No)
                .ToList();

            if (IsPostgreSqlQuartzTable(generalTableName))
            {
                columnDefinitionCollection = columnDefinitionCollection.Select(o =>
                {
                   // PostgreSQLのQRTZテーブルは、小文字で作成されるため、小文字に変更
                    o.ColumnName = o.ColumnName.ToLower();
                    return o;
                }).ToList();
            }

            var columnDefinitionHistoryCollection = columnDefinitionCollection
                .Where(o => o.History > 0)
                .OrderBy(o => o.History);
            var isChanged = false;
            isChanged |= ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: Sqls.TableTypes.Normal,
                columnDefinitionCollection: columnDefinitionCollection,
                checkMigration: checkMigration);
            if (columnDefinitionHistoryCollection.Count() == 0)
            {
                // QRTZ_系のテーブルで、ベースカラムを必要としない場合、
                // historyのテーブル作成時にカラムがないため、エラーが発生する。
                // deletedとhistoryのどちらも不要なのでテーブルを作成しない。
                return isChanged;
            }
            isChanged |= ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: deletedTableName,
                tableType: Sqls.TableTypes.Deleted,
                columnDefinitionCollection: columnDefinitionCollection,
                checkMigration: checkMigration);
            isChanged |= ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: historyTableName,
                tableType: Sqls.TableTypes.History,
                columnDefinitionCollection: columnDefinitionHistoryCollection,
                checkMigration: checkMigration);
            return isChanged;
        }

        private static bool ConfigureTablePart(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            bool checkMigration)
        {
            var isChanged = false;
            if (!Tables.Exists(factory: factory, sourceTableName: sourceTableName))
            {
                Tables.CreateTable(
                    factory: factory,
                    generalTableName: generalTableName,
                    sourceTableName: sourceTableName,
                    tableType: tableType,
                    columnDefinitionCollection: columnDefinitionCollection,
                    tableIndexCollection: Indexes.IndexInfoCollection(
                        factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableType: tableType),
                    checkMigration: checkMigration);
                isChanged = true;
            }
            else
            {
                if (Tables.HasChanges(
                    factory: factory,
                    generalTableName: generalTableName,
                    sourceTableName: sourceTableName,
                    tableType: tableType,
                    columnDefinitionCollection: columnDefinitionCollection,
                    rdsColumnCollection: Columns.Get(factory, sourceTableName)))
                {
                    Tables.MigrateTable(
                        factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableType: tableType,
                        columnDefinitionCollection: columnDefinitionCollection,
                        tableIndexCollection: Indexes.IndexInfoCollection(
                            factory: factory,
                            generalTableName: generalTableName,
                            sourceTableName: sourceTableName,
                            tableType: tableType),
                        checkMigration: checkMigration);
                    isChanged = true;
                }
            }
            return isChanged;
        }

        /// <summary>
        /// ベースカラムのカラム名セットを取得
        /// Base: 1 が設定されているカラムを動的に取得
        /// </summary>
        private static HashSet<string> GetBaseColumnNames()
        {
            if (BaseColumnNameCache == null)
            {
                BaseColumnNameCache = new HashSet<string>();

                // _Base のカラムを追加
                foreach (var column in Def.BaseColumnDefinitionCollection())
                {
                    BaseColumnNameCache.Add(column.ColumnName);
                }

                // _BaseItem のカラムを追加
                foreach (var column in Def.BaseItemColumnDefinitionCollection())
                {
                    BaseColumnNameCache.Add(column.ColumnName);
                }
            }

            return BaseColumnNameCache;
        }

        /// <summary>
        /// カラムがベースカラムかどうかを判定
        /// Base: 1 が設定されているカラム定義から動的に判定
        /// </summary>
        private static bool IsBaseColumn(ColumnDefinition column)
        {
            return GetBaseColumnNames().Contains(column.ColumnName);
        }

        /// <summary>
        /// テーブルにカラムを含めるべきか判定
        /// ExcludeBaseColumns の設定に基づいてベースカラムを除外
        /// </summary>
        private static bool ShouldIncludeColumn(string tableName, ColumnDefinition column)
        {
            // ベースカラムでない場合は常に含める
            if (!IsBaseColumn(column))
            {
                return true;
            }

            // ベースカラムの場合、ExcludeBaseColumns の設定を確認
            return !ShouldExcludeBaseColumns(tableName);
        }

        /// <summary>
        /// テーブルにベースカラムを含めるべきか判定
        /// 全カラム定義で ExcludeBaseColumns が true の場合のみ true を返す
        /// </summary>
        private static bool ShouldExcludeBaseColumns(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }

            var columnDefinitions = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == tableName)
                .Where(o => !IsBaseColumn(o))
                .ToList();

            if (!columnDefinitions.Any())
            {
                return false;
            }

            // テーブルの全カラム定義で ExcludeBaseColumns が true の場合のみ true を返す
            // 1つでも false または未設定があれば false を返す（デフォルト false）
            return columnDefinitions.All(o => o.ExcludeBaseColumns);
        }
    }
}
