using Implem.Pleasanter.Libraries.Requests;
using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class QueryStringsUtilities
    {
        public static QueryStrings Get(params KeyValue[] keyValues)
        {
            var QueryStrings = new QueryStrings();
            foreach (var keyValue in keyValues)
            {
                QueryStrings.Add(keyValue.Key, keyValue.Value);
            }
            return QueryStrings;
        }
    }
}
