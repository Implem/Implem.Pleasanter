using Implem.DefinitionAccessor;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
namespace Implem.Libraries.Utilities
{
    public static partial class Consoles
    {
        public static int ErrorCount { get; private set; } = 0;
        public enum Types
        {
            Info,
            Success,
            Error
        }

        public static void Write(
            string text,
            Types type,
            bool abort = false,
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "")
        {
            var match = GetFileNameRegex().Match(callerFilePath);
            var className = match.Success ? match.Value : "";

            var publisher = $"{className}.{callerMemberName}";
            Trace.WriteLine("<{0}> {1}: {2}".Params(
                type.ToString().ToUpper(),
                publisher,
                text));
            Trace.Flush();
            if (type == Types.Error) ErrorCount++;
            if (abort)
            {
                Trace.WriteLine("\r\nAbort. Press any key to close.");
                Trace.Flush();
                if (!Console.IsInputRedirected)
                {
                    Console.ReadKey();
                }
                Environment.Exit(CodeDefinerExitCodes.Error);
            }
        }

        [GeneratedRegex(@"[^\\/]+(?=\.[^\\/]+$)")]
        private static partial Regex GetFileNameRegex();
    }
}
