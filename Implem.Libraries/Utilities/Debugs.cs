using System.Diagnostics;
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
    }
}