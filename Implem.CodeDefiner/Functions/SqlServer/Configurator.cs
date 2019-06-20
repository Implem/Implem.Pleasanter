using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal class Configurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            if (Environments.RdsProvider == "Local")
            {
                RdsConfigurator.Configure(factory: factory);
                LoginsConfigurator.Configure(factory: factory);
            }
            TablesConfigurator.Configure(factory: factory);
        }
    }
}
