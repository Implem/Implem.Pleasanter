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
