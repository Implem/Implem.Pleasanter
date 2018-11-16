using Implem.SupportTools.MailTester.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Implem.SupportTools.MailTester.View
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class MailTesterWindow : UserControl
    {
        public MailTesterWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as MailTesterWindowViewModel)?.SaveSettings();
        }
        
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var saveCursor = Cursor;
            Cursor = Cursors.Wait;
            sendButton.IsEnabled = false;
            
            try
            {
                (DataContext as MailTesterWindowViewModel)?.SendMail();
            }
            finally
            {
                Cursor = saveCursor;
                sendButton.IsEnabled = true;
            }
        }
    }
}
