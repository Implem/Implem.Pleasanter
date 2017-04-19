using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class RdsConfigurator
    {
        internal static void Configure()
        {
            Consoles.Write(Environments.ServiceName, Consoles.Types.Info);
            Def.SqlIoBySa().ExecuteNonQuery(
                Def.Sql.CreateDatabase.Replace("#InitialCatalog#", Environments.ServiceName));
        }
    }
}
