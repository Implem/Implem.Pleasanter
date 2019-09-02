using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal class Configurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            try
            {
                RdsConfigurator.Configure(factory: factory);
                UsersConfigurator.Configure(factory: factory);
                TablesConfigurator.Configure(factory: factory);
                PrivilegeConfigurator.Configure(factory: factory);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                Consoles.Write($"[{e.Number}] {e.Message}", Consoles.Types.Error, true);
            }
            catch (System.Exception e)
            {
                Consoles.Write(e.ToString(), Consoles.Types.Error, true);
            }
        }
    }
}
