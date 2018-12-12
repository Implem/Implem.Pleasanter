using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Implem.SupportTools.LdapSyncTester
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class LdapSyncTesterWindow : UserControl
    {
        public LdapSyncTesterWindow()
        {
            InitializeComponent();
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LdapSyncWindowViewModel)?.Sync();
        }

        private void openfileButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as LdapSyncWindowViewModel)?.OpenSettingsFile();
        }

        private async void outputButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LdapSyncWindowViewModel;
            if(vm == null) { return; }

            var dialog = new SaveFileDialog()
            {
                Filter = "CSVファイル|*.csv",
                Title = "ファイルに保存",
                FileName = $"LdapSyncItems_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                using (var file = new StreamWriter(dialog.FileName))
                {
                    await file.WriteLineAsync(LdapResult.CsvHeader);
                    
                    foreach (LdapResult viewItem in vm.Items)
                    {
                        await file.WriteLineAsync(viewItem.ToCsv());
                    }
                }
            }
        }
    }
}
