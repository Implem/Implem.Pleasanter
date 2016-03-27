using System.Diagnostics;
using System.Linq;
using System.Reflection;
namespace Implem.Libraries.Utilities
{
    public static class StackTraces
    {
        public static string Class()
        {
            return Last().ReflectedType.Name;
        }

        public static string Method()
        {
            return Last().Name;
        }

        private static MethodBase Last()
        {
            return new StackTrace(1, false).GetFrames()
                .Select(o => o.GetMethod())
                .Where(o => o.Name != ".ctor")
                .Where(o => o.Name != "SetCaller")
                .Last();
        }
    }
}
