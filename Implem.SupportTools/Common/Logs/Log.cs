using System;

namespace Implem.SupportTools.Common
{
    public class Log
    {
        public DateTime CreatedTime { get; }
        public string Module { get; }
        public LogLevel LogLevel { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public bool HasError { get => Exception != null; }

        public Log(string module, LogLevel logLevel, string message, Exception exception)
        {
            Module = module;
            LogLevel = logLevel;
            Message = message;
            Exception = exception;
            CreatedTime = DateTime.Now;
        }

        public static Log CreateTraceLog(string module, string message, Exception exception = null)
        {
            return new Log(module, LogLevel.Trace, message, exception);
        }

        public static Log CreateDebugLog(string module, string message, Exception exception = null)
        {
            return new Log(module, LogLevel.Debug, message, exception);
        }

        public static Log CreateInfoLog(string module, string message, Exception exception = null)
        {
            return new Log(module, LogLevel.Info, message, exception);
        }

        public static Log CreateErrorLog(string module, string message, Exception exception = null)
        {
            return new Log(module, LogLevel.Error, message, exception);
        }

        public static Log CreateFatalLog(string module, string message, Exception exception = null)
        {
            return new Log(module, LogLevel.Fatal, message, exception);
        }

        public override string ToString()
        {
            return $"{CreatedTime.ToString("yyyy/MM/dd HH:mm:ss")} [{LogLevel.ToString()}]({Module}): {Message}" 
                + (HasError 
                    ? Environment.NewLine + ">" + Exception.ToString()
                    : string.Empty );
        }
    }
    
}