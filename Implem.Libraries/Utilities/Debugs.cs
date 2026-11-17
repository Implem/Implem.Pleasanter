using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Implem.Libraries.Utilities
{
    public static partial class Debugs
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
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            var match = GetFileNameRegex().Match(callerFilePath ?? string.Empty);
            var className = match.Success ? match.Value : string.Empty;
            return $"{className}.{callerMemberName}:line {lineNumber}";
        }

        [GeneratedRegex(@"[^\\/]+(?=\.[^\\/]+$)")]
        private static partial Regex GetFileNameRegex();
    }
}