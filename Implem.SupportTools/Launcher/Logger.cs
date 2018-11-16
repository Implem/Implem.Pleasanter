using Implem.SupportTools.Common;
using System;
using System.Collections.ObjectModel;

namespace Implem.SupportTools
{
    public interface IObservableLogger<TLog> : ILogger
    {
        ObservableCollection<TLog> Logs { get; }
    }

    public class Logger : IObservableLogger<Log>
    {
        public ObservableCollection<Log> Logs { get; } = new ObservableCollection<Log>();

        public Logger(string logFileName)
        {
            System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(logFileName));
        }

        public void Trace(string module, string message, Exception exception = null)
        {
            Add(Log.CreateTraceLog(module, message, exception));
        }

        public void Debug(string module, string message, Exception exception = null)
        {
            Add(Log.CreateDebugLog(module, message, exception));
        }

        public void Info(string module, string message, Exception exception = null)
        {
            Add(Log.CreateInfoLog(module, message, exception));
        }

        public void Error(string module, string message, Exception exception = null)
        {
            Add(Log.CreateErrorLog(module, message, exception));
        }

        public void Fatal(string module, string message, Exception exception = null)
        {
            Add(Log.CreateFatalLog(module, message, exception));
        }

        private void Add(Log log)
        {
            System.Diagnostics.Trace.WriteLine(log);
            System.Diagnostics.Trace.Flush();
            Logs.Add(log);
        }
    }

}