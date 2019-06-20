using Implem.SupportTools.SysLogViewer.Model;
using Implem.SupportTools.SysLogViewer.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Implem.SupportTools.SysLogViewer.View
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class SysLogViewerWindow : UserControl
    {
        private SysLogViewerViewModel VM { get => DataContext as SysLogViewerViewModel; }
        public SysLogViewerWindow()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await VM?.GetSysLogsAsync(listView.Dispatcher);

            tabControl.SelectionChanged += TabControl_SelectionChanged;
            startDatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            endDatePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var syslog = (e.Item as SysLogModel);
            e.Accepted = VM?.AcceptedSysLogTypes(syslog.SysLogType) == true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (listView.ItemsSource == null) { return; }
            var view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            VM?.Dispose();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            VM?.GetSysLogsAsync(listView.Dispatcher);
        }

        private async void SaveButton_Click(object sender , RoutedEventArgs e)
        {
            VM?.StopTimer();
            try
            {
                var dialog = new SaveFileDialog()
                {
                    Filter = "CSVファイル|*.csv",
                    Title = "ファイルに保存",
                    FileName = $"SysLogs_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv"
                };
                if (dialog.ShowDialog() == true)
                {
                    using (var file = new StreamWriter(dialog.FileName))
                    {
                        await file.WriteLineAsync(SysLogModel.CsvHeader);
                        
                        var collectionViewSource = Resources["listViewSource"] as CollectionViewSource;
                        foreach (SysLogModel viewItem in collectionViewSource.View)
                        {
                            await file.WriteLineAsync(viewItem.ToCsv());
                        }
                    }
                }
            }
            finally
            {
                VM?.RestertTimer();
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = ((sender as ListViewItem)?.DataContext as SysLogModel);
            if (item == null) { return; }

            var window = new DetailWindow() { DataContext = new DetailWindowViewModel(item) };
            window.ShowDialog();

        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await VM?.GetSysLogsAsync(listView.Dispatcher);
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await VM?.GetSysLogsAsync(listView.Dispatcher);
        }
    }
}
