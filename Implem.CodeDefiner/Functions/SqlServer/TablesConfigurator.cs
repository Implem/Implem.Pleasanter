using Implem.CodeDefiner.Functions.SqlServer.Parts;
using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal class TablesConfigurator
    {
        internal static void Configure()
        {
            Def.TableNameCollection().ForEach(generalTableName =>
                ConfigureTableSet(generalTableName));
        }

        private static void ConfigureTableSet(string generalTableName)
        {
            Consoles.Write(generalTableName, Consoles.Types.Info);
            var deletedTableName = generalTableName + "_deleted";
            var historyTableName = generalTableName + "_history";
            var columnDefinitionCollection = Def.ColumnDefinitionCollection
                .Where(o => o.TableName == generalTableName)
                .Where(o => !o.NotUpdate)
                .Where(o => o.JoinTableName == string.Empty)
                .Where(o => o.Calc == string.Empty)
                .OrderBy(o => o.No);
            var columnDefinitionHistoryCollection = columnDefinitionCollection
                .Where(o => o.History > 0)
                .OrderBy(o => o.History);
            ConfigureTablePart(
                generalTableName,
                generalTableName,
                Sqls.TableTypes.Normal,
                columnDefinitionCollection);
            ConfigureTablePart(
                generalTableName,
                deletedTableName,
                Sqls.TableTypes.Deleted,
                columnDefinitionCollection);
            ConfigureTablePart(
                generalTableName,
                historyTableName,
                Sqls.TableTypes.History,
                columnDefinitionHistoryCollection);
        }

        private static void ConfigureTablePart(
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            if (!Tables.Exists(sourceTableName))
            {
                Tables.CreateTable(
                    generalTableName,
                    sourceTableName,
                    tableType,
                    columnDefinitionCollection,
                    Indexes.IndexInfoCollection(generalTableName, sourceTableName, tableType),
                    Columns.Get(sourceTableName));
            }
            else
            {
                if (Tables.HasChanges(
                    generalTableName,
                    sourceTableName,
                    tableType,
                    columnDefinitionCollection,
                    Columns.Get(sourceTableName)))
                {
                    Tables.MigrateTable(
                        generalTableName,
                        sourceTableName,
                        tableType,
                        columnDefinitionCollection,
                        Indexes.IndexInfoCollection(generalTableName, sourceTableName, tableType));
                }
            }
        }
    }
}
