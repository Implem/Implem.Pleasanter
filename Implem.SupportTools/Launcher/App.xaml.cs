using Implem.SupportTools.Common;
using System;
using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace Implem.SupportTools
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private IObservableLogger<Log> Logger;

        protected override void OnStartup(StartupEventArgs e)
        {            
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var logFilePath = "Logs\\Implem.SupportTools.log";
            if (File.Exists(SupportTools.Properties.Settings.Default.SettingsPath))
            {
                var json = File.ReadAllText(SupportTools.Properties.Settings.Default.SettingsPath);
                dynamic settings = JsonConvert.DeserializeObject(json);
                logFilePath = settings.logFilePath;
            }
            if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            }
            Logger = new Logger(logFilePath);
            MainWindow = new MainWindow(Logger);
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (e.ExceptionObject as Exception);
            Logger.Fatal(ex.TargetSite.Module.Name, "Current Domein Unhandled Exception", ex);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Logger.Fatal(e.Exception.TargetSite.Module.Name, "Dispatcher Unhandled Exception", e.Exception);
        }

    }
}
