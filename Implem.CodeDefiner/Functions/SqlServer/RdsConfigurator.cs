using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class RdsConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            Consoles.Write(Environments.ServiceName, Consoles.Types.Info);
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                commandText: Def.Sql.CreateDatabase.Replace("#InitialCatalog#", Environments.ServiceName));
        }
    }
}
