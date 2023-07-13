using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Implem.Libraries.Utilities
{
    public static class Debugs
    {
        public static bool InDebug()
        {
            return Debugger.IsAttached;
        }

        public static Process CurrentProcess()
        {
            return Process.GetCurrentProcess();
        }

        public static string GetSysLogsDescription(
            [CallerLineNumber] int lineNumber = 0)
        {
            StackFrame caller = new StackFrame(1);
            string className = caller.GetMethod().ReflectedType.Name;
            string methodName = caller.GetMethod().Name;
            return $"{className}.{methodName}:line {lineNumber}";
        }
    }
}