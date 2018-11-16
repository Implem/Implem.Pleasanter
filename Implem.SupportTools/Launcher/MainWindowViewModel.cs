using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Implem.SupportTools.Common;

namespace Implem.SupportTools
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<Log> Logs { get; }
        public MainWindowViewModel(IObservableLogger<Log> logger)
        {
            Logs = logger.Logs;
        }
    }

    
}
