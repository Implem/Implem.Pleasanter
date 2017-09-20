using System.Collections.Generic;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Enumerables
    {
        public static string Join(this IEnumerable<string> self, string delimiter = ",")
        {
            return string.Join(delimiter, self.ToArray());
        }

        public static string Join<T>(this IEnumerable<T> self, string delimiter = ",")
        {
            return string.Join(delimiter, self.Select(o => o.ToString()).ToArray());
        }

        public static string JoinReturn(this IEnumerable<string> self)
        {
            return string.Join("\r\n", self);
        }

        public static string JoinParam(this string self, params string[] list)
        {
            return string.Join(self, list);
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> self, int size)
        {
            var _size = size != 0
                ? size
                : self.Count();
            while (self.Any())
            {
                yield return self.Take(_size);
                self = self.Skip(_size);
            }
        }

        public static bool AllEqual<T>(this IEnumerable<T> self)
        {
            if (!self.Any()) return true;
            var first = self.First();
            return self.Skip(1).All(o => first.Equals(o));
        }
    }
}
