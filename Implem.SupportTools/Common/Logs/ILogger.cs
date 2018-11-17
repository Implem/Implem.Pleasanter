using System;

namespace Implem.SupportTools.Common
{
    public interface ILogger
    {
        void Trace(string module, string message, Exception exception = null);
        void Debug(string module, string message, Exception exception = null);
        void Info(string module, string message, Exception exception = null);
        void Error(string module, string message, Exception exception = null);
        void Fatal(string module, string message, Exception exception = null);
    }
}