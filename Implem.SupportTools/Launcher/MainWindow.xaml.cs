using Implem.SupportTools.Common;
using Implem.SupportTools.SysLogViewer.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Implem.SupportTools
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string pleasanterSettingsPath;

        private IObservableLogger<Log> logger;
        public MainWindow(IObservableLogger<Log> logger)
        {
            InitializeComponent();
            this.logger = logger;
            DataContext = new MainWindowViewModel(logger);

            pleasanterSettingsPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.PleasanterSettingsPath);
            SetDebugPleasanterSettingsPath();
        }

        [Conditional("DEBUG")]
        private void SetDebugPleasanterSettingsPath()
        {
            if (!System.IO.Directory.Exists(pleasanterSettingsPath))
            {
                pleasanterSettingsPath = @".\App_Data";
            }
        }
        
        private void MailTest_Click(object sender, RoutedEventArgs e)
        {
            var module = nameof(MailTester);
            var button = (sender as Button);
            button.Click -= MailTest_Click;

            var win = new Window()
            {
                Owner = this,
                Title = button.Content.ToString(),
                DataContext = new MailTester.ViewModel.MailTesterWindowViewModel(logger, pleasanterSettingsPath),
                Content = new MailTester.View.MailTesterWindow(),
                Width = 800,
                Height = 480,
            };

            logger.Info(module, module + "Start >>>");
            win.Closed += (_, __) =>
            {
                button.Click += MailTest_Click;
                logger.Info(module, module + "End <<<");
            };

            win.Show();
        }

        private void SysLog_Click(object sender, RoutedEventArgs e)
        {
            var module = nameof(SysLogViewer);
            var button = (sender as Button);
            button.Click -= SysLog_Click;

            var win = new Window()
            {
                Owner = this,
                Title = (sender as Button).Content.ToString(),
                DataContext = new SysLogViewerViewModel(logger, pleasanterSettingsPath),
                Content = new SysLogViewer.View.SysLogViewerWindow(),
            };

            logger.Info(module, module + "Start >>>");

            win.Closed += (_, __) =>
            {

                button.Click += SysLog_Click;
                logger.Info(module, module + "End <<<");
            };

            win.Show();
        }

        private void LdapSync_Click(object sender, RoutedEventArgs e)
        {
            var module = nameof(LdapSyncTester);
            var button = (sender as Button);
            button.Click -= LdapSync_Click;

            var win = new Window()
            {
                Owner = this,
                Title = (sender as Button).Content.ToString(),
                DataContext = new LdapSyncTester.LdapSyncWindowViewModel(logger, pleasanterSettingsPath),
                Content = new LdapSyncTester.LdapSyncTesterWindow(),
            };

            logger.Info(module, module + "Start >>>");

            win.Closed += (_, __) =>
            {

                button.Click += LdapSync_Click;
                logger.Info(module, module + "End <<<");
            };

            win.Show();
        }
    }


}
