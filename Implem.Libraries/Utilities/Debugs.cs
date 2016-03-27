using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public static string Trace()
        {
            var stringBuilder = new StringBuilder();
            new StackTrace(1, false).GetFrames()
                .Select(o => o.GetMethod())
                .Where(o => o.Name != ".ctor")
                .ForEach(method =>
                {
                    var callerOfClass = method.ReflectedType.Name;
                    var callerOfMethod = method.Name;
                    if (!callerOfClass.StartsWith("<>"))
                    {
                        stringBuilder.Append(callerOfClass, ".", callerOfMethod, "\r\n");
                    }
                });
            return stringBuilder.ToString();
        }
    }
}