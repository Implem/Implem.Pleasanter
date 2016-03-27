using System.Collections.Generic;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Arrays
    {
        public static T Next<T>(this List<T> collection, T current)
        {
            if (!collection.Contains(current))
            {
                return current;
            }
            var index = collection.IndexOf(current);
            return index != collection.Count() - 1
                ? collection.Skip(index + 1).FirstOrDefault()
                : current;
        }

        public static T Previous<T>(this List<T> collection, T current)
        {
            if (!collection.Contains(current))
            {
                return current;
            }
            var index = collection.IndexOf(current);
            return index != 0
                ? collection.Skip(index - 1).FirstOrDefault()
                : current;
        }

        public static string _1st(this IEnumerable<string> self)
        {
            return self.FirstOrDefault() ?? string.Empty;
        }

        public static string _2nd(this IEnumerable<string> self)
        {
            return self.Skip(1).FirstOrDefault() ?? string.Empty;
        }

        public static string _3rd(this IEnumerable<string> self)
        {
            return self.Skip(2).FirstOrDefault() ?? string.Empty;
        }

        public static string _4th(this IEnumerable<string> self)
        {
            return self.Skip(3).FirstOrDefault() ?? string.Empty;
        }

        public static string _5th(this IEnumerable<string> self)
        {
            return self.Skip(4).FirstOrDefault() ?? string.Empty;
        }

        public static string[] Swap(string[] self, int sourceIndex, int distinationIndex)
        {
            var argsTemp = self;
            var temp = argsTemp[sourceIndex];
            argsTemp[sourceIndex] = argsTemp[distinationIndex];
            argsTemp[distinationIndex] = temp;
            return argsTemp;
        }

        public static IEnumerable<string> ToStringEnumerable(this object self)
        {
            return self is IEnumerable<string>
                ? self as IEnumerable<string>
                : self is IEnumerable<int>
                    ? (self as IEnumerable<int>).Select(p => p.ToString())
                    : self is IEnumerable<long>
                        ? (self as IEnumerable<long>).Select(p => p.ToString())
                        : null;
        }

        public static IEnumerable<object> ToObjectEnumerable(this object self)
        {
            return self is IEnumerable<string>
                ? (self as IEnumerable<string>).Select(p => p as object)
                : self is IEnumerable<int>
                    ? (self as IEnumerable<int>).Select(p => p as object)
                    : self is IEnumerable<long>
                        ? (self as IEnumerable<long>).Select(p => p as object)
                        : null;
        }
    }
}
