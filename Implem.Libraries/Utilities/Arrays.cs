using System.Collections.Generic;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Arrays
    {
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

        public static T[] Swap<T>(T[] self, int sourceIndex, int distinationIndex)
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

        public static List<T> Concat<T>(params List<T>[] data)
        {
            var list = new List<T>();
            data?
                .Where(o => o != null)
                .ForEach(o => list = list.Concat(o).ToList());
            return list;
        }

        public static List<T> Concat<T>(this List<T> self, params T[] data)
        {
            var list = self?.ToList() ?? new List<T>();
            list.AddRange(data);
            return list;
        }
    }
}
