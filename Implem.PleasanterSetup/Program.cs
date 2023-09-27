using Implem.PleasanterSetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;

public class Program
{
    public static void Main(string[] args)
    {
        var app = ConsoleApp.CreateBuilder(args)
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                services.AddSingleton(configuration);
                LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
                services.AddSingleton<ILogger>(LogManager.GetCurrentClassLogger());
            })
            .Build();

        app.AddCommands<PleasanterSetup>();
        app.Run();
    }
}