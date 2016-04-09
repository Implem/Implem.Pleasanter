using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal class Configurator
    {
        internal static void Configure()
        {
            if (Environments.DbEnvironmentType == Sqls.DbEnvironmentTypes.Local)
            {
                DbConfigurator.Configure();
                LoginsConfigurator.Configure();
            }
            TablesConfigurator.Configure();
            if (Def.Parameters.DataImport == 1)
            {
                DataImporter.Import();
            }
        }
    }
}
