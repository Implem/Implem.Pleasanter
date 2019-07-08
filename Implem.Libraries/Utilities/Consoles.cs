using System;
using System.Diagnostics;
namespace Implem.Libraries.Utilities
{
    public static class Consoles
    {
        public static int ErrorCount { get; private set; } = 0;
        public enum Types
        {
            Info,
            Success,
            Error
        }

        public static void Write(string text, Types type, bool abort = false)
        {
            var method = new StackFrame(1).GetMethod();
            var publisher = method.ReflectedType.Name + "." + method.Name;
            Trace.WriteLine("<{0}> {1}: {2}".Params(
                type.ToString().ToUpper(),
                publisher,
                text));
            Trace.Flush();
            if(type == Types.Error) ErrorCount++;
            if (abort)
            {
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
