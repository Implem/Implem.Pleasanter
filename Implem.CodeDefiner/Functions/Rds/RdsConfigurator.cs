using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class RdsConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            if (!Exists(factory: factory, databaseName: Environments.ServiceName))
            {
                CreateDatabase(factory: factory, databaseName: Environments.ServiceName);
            }
        }

        private static void CreateDatabase(ISqlObjectFactory factory, string databaseName)
        {
            Consoles.Write(Environments.ServiceName, Consoles.Types.Info);
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.CreateDatabase.Replace("#InitialCatalog#", databaseName));
        }

        private static bool Exists(ISqlObjectFactory factory, string databaseName)
        {
            return Def.SqlIoBySa(factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.ExistsDatabase.Replace("#InitialCatalog#", databaseName))
                .Rows.Count == 1;
        }
    }
}
