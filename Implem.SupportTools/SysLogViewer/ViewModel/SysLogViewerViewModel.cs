using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Implem.SupportTools.Common;
using Prism.Mvvm;
using Implem.SupportTools.SysLogViewer.Model;
using System.IO;
using Newtonsoft.Json;

namespace Implem.SupportTools.SysLogViewer.ViewModel
{
    public class SysLogViewerViewModel : BindableBase, IDisposable
    {
        private bool isInfoChecked = true;
        private bool isWarningChecked = true;
        private bool isUserErrorChecked = true;
        private bool isSystemErrorChecked = true;
        private bool isExceptionChecked = true;
        private ushort count;
        private ushort interval;
        private DateTime lastCreatedTime;
        private System.Threading.Timer timer;


        public ObservableCollection<SysLogModel> SysLogs { get; private set; } = new ObservableCollection<SysLogModel>();
        public bool IsInfoChecked { get => isInfoChecked; set => SetProperty(ref isInfoChecked, value); }
        public bool IsWarningChecked { get => isWarningChecked; set => SetProperty(ref isWarningChecked, value); }
        public bool IsUserErrorChecked { get => isUserErrorChecked; set => SetProperty(ref isUserErrorChecked, value); }
        public bool IsSystemErrorChecked { get => isSystemErrorChecked; set => SetProperty(ref isSystemErrorChecked, value); }
        public bool IsExceptionChecked { get => isExceptionChecked; set => SetProperty(ref isExceptionChecked, value); }
        public ushort Count { get => count; set => SetProperty(ref count, value); }
        public ushort Interval { get => interval; set => SetProperty(ref interval, value); }

        private readonly ILogger logger;
        private readonly string connectionString = "Server=(local);Database=master;UID=sa;PWD=SetSaPWD;Connection Timeout=30;";

        public SysLogViewerViewModel(ILogger logger)
        {
            this.logger = logger;
            if (File.Exists(Properties.Settings.Default.SettingsPath))
            {
                var json = File.ReadAllText(Properties.Settings.Default.SettingsPath);
                dynamic settings = JsonConvert.DeserializeObject(json);

                count = settings?.acquiredRecords ?? 1000;
                interval = settings?.refreshInterval ?? 1;
            }
            if (File.Exists(Properties.Settings.Default.SettingsPath))
            {
                var json = File.ReadAllText(Properties.Settings.Default.ParamsRdsPath);
                dynamic rds = JsonConvert.DeserializeObject(json);
                connectionString = rds?.SaConnectionString ?? connectionString;
            }
        }

        public async Task GetSysLogsAsync(Dispatcher dispatcher)
        {
            timer?.Dispose();
            SysLogs?.Clear();
            try
            {
                using (var client = new PleasanterDbClient(connectionString))
                {
                    var syslogs = await client.GetSysLogsAsync(Count);

                    SysLogs.AddRange(syslogs);
                }

                var lastItem = SysLogs.FirstOrDefault();
                if (lastItem != null)
                {
                    lastCreatedTime = lastItem.CreatedTime;
                }

                timer = new System.Threading.Timer(async state => await Timer_Callback(state), dispatcher, interval * 1000, interval * 1000);
            }
            catch (Exception e)
            {
                logger.Error(nameof(SysLogViewer), "Getting SysLogs Error.", e);
            }
        }


        public async Task Timer_Callback(object state)
        {
            try
            {
                using (var client = new PleasanterDbClient(connectionString))
                {
                    var syslogs = await client.GetSysLogsAsync(lastCreatedTime);
                    if (syslogs.Any())
                    {
                        lastCreatedTime = syslogs.First().CreatedTime;
                        (state as Dispatcher)?.Invoke(() =>
                        {
                            SysLogs.AddRange(syslogs);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(nameof(SysLogViewer), "Getting SysLogs Error.", e);
            }
        }

        public bool AcceptedSysLogTypes(SysLogTypes type)
        {
            switch (type)
            {
                case SysLogTypes.Info: return IsInfoChecked;
                case SysLogTypes.Warning: return IsWarningChecked;
                case SysLogTypes.UserError: return IsUserErrorChecked;
                case SysLogTypes.SystemError: return IsSystemErrorChecked;
                case SysLogTypes.Execption: return IsExceptionChecked;
                default: return true;
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
            SysLogs.Clear();
        }


    }
}
