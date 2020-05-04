using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class SearchIndexExtensions
    {
        public static List<string> SearchIndexes(this string self)
        {
            return self?.Replace("　", " ")
                .Split(' ')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct()
                .ToList()
                    ?? new List<string>();
        }
    }
}