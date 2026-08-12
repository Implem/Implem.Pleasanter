using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
namespace Implem.Pleasanter.NetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info("init main");
            try
            {
                var host = CreateHostBuilder(args).Build();
                ClearRestartScheduledTime();
                host.Run();
            }
            catch (System.Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ClearRestartScheduledTime()
        {
            var context = new Context(
                tenantId: 0,
                request: false);
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateTenants(
                    param: Rds.TenantsParam()
                        .RestartScheduledTime(value: null),
                    where: Rds.TenantsWhere()
                        .RestartScheduledTime(_operator: " is not null")));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}