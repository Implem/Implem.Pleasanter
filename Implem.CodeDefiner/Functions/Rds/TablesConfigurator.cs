using Implem.CodeDefiner.Functions.Rds.Parts;
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
        internal static void Configure(ISqlObjectFactory factory)
        {
            Def.TableNameCollection().ForEach(generalTableName =>
            {
                try
                {
                    ConfigureTableSet(
                        factory: factory,
                        generalTableName: generalTableName);
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
                ConfigureFullTextIndex(factory: factory);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write($"[{e.Number}] {e.Message}", Consoles.Types.Error);
            }
            catch (System.Exception e)
            {
                Consoles.Write($"[{nameof(ConfigureFullTextIndex)}]: {e}", Consoles.Types.Error);
            }
        }

        private static void ConfigureFullTextIndex(ISqlObjectFactory factory)
        {
            Consoles.Write("", Consoles.Types.Info);
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

        private static void ConfigureTableSet(ISqlObjectFactory factory, string generalTableName)
        {
            Consoles.Write(generalTableName, Consoles.Types.Info);
            var deletedTableName = generalTableName + "_deleted";
            var historyTableName = generalTableName + "_history";
            var columnDefinitionCollection = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == generalTableName)
                .Where(o => !o.NotUpdate)
                .Where(o => o.JoinTableName.IsNullOrEmpty())
                .Where(o => o.Calc.IsNullOrEmpty())
                .OrderBy(o => o.No)
                .ToList();
            var columnDefinitionHistoryCollection = columnDefinitionCollection
                .Where(o => o.History > 0)
                .OrderBy(o => o.History);
            ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: generalTableName,
                tableType: Sqls.TableTypes.Normal,
                columnDefinitionCollection: columnDefinitionCollection);
            ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: deletedTableName,
                tableType: Sqls.TableTypes.Deleted,
                columnDefinitionCollection: columnDefinitionCollection);
            ConfigureTablePart(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: historyTableName,
                tableType: Sqls.TableTypes.History,
                columnDefinitionCollection: columnDefinitionHistoryCollection);
        }

        private static void ConfigureTablePart(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
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
                    rdsColumnCollection: Columns.Get(
                        factory: factory,
                        sourceTableName: sourceTableName));
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
                            tableType: tableType));
                }
            }
        }
    }
}
