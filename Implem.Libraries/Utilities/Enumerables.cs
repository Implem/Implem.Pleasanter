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

        public static string JoinDot(this IEnumerable<string> self)
        {
            return string.Join(".", self);
        }

        public static string JoinReturn(this IEnumerable<string> self)
        {
            return string.Join("\r\n", self);
        }

        public static string JoinParam(this string self, params string[] list)
        {
            return string.Join(self, list);
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(
            this IEnumerable<T> self, int size)
        {
            while (self.Any())
            {
                yield return self.Take(size);
                self = self.Skip(size);
            }
        }

        public static SortedSet<string> SortedSet(this string self, char delimiter = ',')
        {
            var sortedSet = new SortedSet<string>();
            if (self.Trim() != string.Empty)
            {
                self.Split(delimiter).ForEach(data => sortedSet.Add(data));
            }
            return sortedSet;
        }
    }
}
