using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class SearchIndexExtensions
    {
        public static List<string> SearchIndexes(this string self, bool createIndex = false)
        {
            return new Search.WordBreaker(self, createIndex).Results
                .Select(o => o.Trim().ToLower())
                .Where(o => o != string.Empty)
                .Distinct()
                .ToList();
        }
    }
}