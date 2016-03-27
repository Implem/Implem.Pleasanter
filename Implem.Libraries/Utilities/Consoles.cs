using System;
using System.Diagnostics;
namespace Implem.Libraries.Utilities
{
    public static class Consoles
    {
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
            Console.WriteLine("<{0}> {1}: {2}".Params(
                type.ToString().ToUpper(),
                publisher,
                text));
            if (abort)
            {
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
