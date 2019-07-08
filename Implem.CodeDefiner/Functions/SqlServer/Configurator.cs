using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal class Configurator
    {
        internal static void Configure()
        {
            if (Environments.RdsProvider == "Local")
            {
                try
                {
                    RdsConfigurator.Configure();
                    LoginsConfigurator.Configure();
                }
                catch(System.Data.SqlClient.SqlException e)
                {
                    Consoles.Write($"[{e.Number}] {e.Message}", Consoles.Types.Error, true);
                }
                catch (System.Exception e)
                {
                    Consoles.Write(e.ToString(), Consoles.Types.Error, true);
                }
            }
            TablesConfigurator.Configure();
        }
    }
}
