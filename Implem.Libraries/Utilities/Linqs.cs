using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Libraries.Utilities
{
    public static class Linqs
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }

        public static V Get<K, V>(this IDictionary<K, V> self, K key)
        {
            return key != null && self?.ContainsKey(key) == true
                ? self[key]
                : default(V);
        }

        public static void RemoveAll<K, V>(
            this IDictionary<K, V> self, Func<K, V, bool> peredicate)
        {
            foreach (var key in self.Keys.ToArray().Where(key => peredicate(key, self[key])))
            {
                self.Remove(key);
            }
        }

        public static Dictionary<K, V> AddIfNotConainsKey<K, V>(
            this IDictionary<K, V> self, K key, V value)
        {
            if (!self.ContainsKey(key))
            {
                self.Add(key, value);
            }
            return self.ToDictionary(o => o.Key, o => o.Value);
        }

        public static Dictionary<K, V> AddRange<K, V>(
            this IDictionary<K, V> self, IDictionary<K, V> data)
        {
            foreach (var dataPart in data)
            {
                self.Add(dataPart);
            }
            return self.ToDictionary(o => o.Key, o => o.Value);
        }

        public static Dictionary<K, V> UpdateOrAdd<K, V>(
            this Dictionary<K, V> self, K key, V value)
        {
            if (self.ContainsKey(key))
            {
                self[key] = value;
            }
            else
            {
                self.Add(key, value);
            }
            return self.ToDictionary(o => o.Key, o => o.Value);
        }

        public static TResult MinOrDefault<T, TResult>(
            this IEnumerable<T> self, Func<T, TResult> func)
        {
            var list = self.ToList();
            return list.Any()
                ? list.Min(func)
                : default(TResult);
        }

        public static TResult MaxOrDefault<T, TResult>(
            this IEnumerable<T> self, Func<T, TResult> func)
        {
            var list = self.ToList();
            return list.Any()
                ? list.Max(func)
                : default(TResult);
        }

        public static List<T> ToSingleList<T>(this T self)
        {
            return new List<T>() { self };
        }

        public static T[] ToSingleArray<T>(this T self)
        {
            return new T[] { self };
        }

        public static bool Any(this MatchCollection self)
        {
            return self?.Count > 0;
        }

        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int count)
        {
            IEnumerable<IEnumerable<T>> BufferImpl()
            {
                for (; source.Any(); source = source.Skip(count))
                {
                    yield return source.Take(count);
                }
            }
            return BufferImpl();
        }
    }
}
