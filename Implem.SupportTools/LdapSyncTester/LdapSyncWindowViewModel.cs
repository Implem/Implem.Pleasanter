using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Implem.SupportTools.Common;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Implem.SupportTools.LdapSyncTester
{
    public class LdapSyncWindowViewModel : BindableBase
    {
        private readonly string settingsPath;
        private readonly ILogger logger;

        public ObservableCollection<LdapResult> Items { get; set; } = new ObservableCollection<LdapResult>();

        public LdapSyncWindowViewModel()
        {}

        public LdapSyncWindowViewModel(ILogger logger, string settingsPath)
        {
            this.settingsPath = System.IO.Path.Combine(settingsPath, "Parameters\\Authentication.json");
            this.logger = logger;
        }

        public void Sync()
        {
            logger.Info(nameof(LdapSyncTester), "Sync Start");
            
            var auth = JsonConvert.DeserializeObject<Authentication>(System.IO.File.ReadAllText(settingsPath));
            Items.Clear();
            
            Items.AddRange(LdapSync.Sync(auth, logger));

            logger.Info(nameof(LdapSyncTester), "Sync End");
        }

        public void OpenSettingsFile()
        {
            Process.Start(settingsPath);
        }

        
    }
}
