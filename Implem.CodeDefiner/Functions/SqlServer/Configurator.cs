using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal class Configurator
    {
        internal static void Configure()
        {
            if (Environments.RdsProvider == "Local")
            {
                RdsConfigurator.Configure();
                LoginsConfigurator.Configure();
            }
            TablesConfigurator.Configure();
            if (Parameters.Service.ImportData)
            {
                DataImporter.Import();
            }
        }
    }
}
