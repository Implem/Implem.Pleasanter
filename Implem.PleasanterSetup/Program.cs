using Implem.PleasanterSetup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        var app = ConsoleApp.CreateBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(logBuilder =>
                {
                    logBuilder.ClearProviders();
                    var config = new NLog.Config.LoggingConfiguration();
                    var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "Logs/${date:format=yyyyMMdd}.txt" };
                    var logconsole = new NLog.Targets.ConsoleTarget("logconsole") { Layout = "${message}" };
                    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
                    config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, logfile);
                    logBuilder.AddNLog(config);
                });
            })
            .Build();

        app.AddCommands<PleasanterSetup>();
        app.Run();
    }
}