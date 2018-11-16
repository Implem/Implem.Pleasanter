using Implem.SupportTools.Common;
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
        private IObservableLogger<Log> logger;
        public MainWindow(IObservableLogger<Log> logger)
        {
            InitializeComponent();
            this.logger = logger;
            DataContext = new MainWindowViewModel(logger);
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
                DataContext = new MailTester.ViewModel.MailTesterWindowViewModel(logger),
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
                DataContext = new SysLogViewer.ViewModel.SysLogViewerViewModel(logger),
                Content = new SysLogViewer.View.SysLogViewerWindow(),
            };

            logger.Info(module, module+ "Start >>>");
            
            win.Closed += (_, __) =>
            {
                
                button.Click += SysLog_Click;
                logger.Info(module, module + "End <<<");
            };

            win.Show();
        }
    }


}
