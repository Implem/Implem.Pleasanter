using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace Implem.PleasanterSetup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = ConsoleApp.CreateBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettngs.json")
                        .Build();
                    services.AddSingleton(configuration);
                    services.AddLogging(logBuilder =>
                    {
                        logBuilder.AddNLog(new NLog.Config.XmlLoggingConfiguration("NLog.config"));
                    });
                })
                .Build();
            app.AddCommands<PleasanterSetup>();
            app.Run();
        }
    }
}
