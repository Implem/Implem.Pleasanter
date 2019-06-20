using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class QueryStrings : Dictionary<string, string>
    {
        public string Data(string key)
        {
            return this
                .Where(o => o.Key.ToLower() == key.ToLower())
                .Select(o => o.Value)
                .FirstOrDefault()
                    ?? string.Empty;
        }

        public bool Bool(string key)
        {
            return Data(key).ToBool();
        }

        public int Int(string key)
        {
            return Data(key).ToInt();
        }

        public long Long(string key)
        {
            return Data(key).ToLong();
        }
    }
}