using Implem.Libraries.Utilities;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class QueryStrings : Dictionary<string, string>
    {
        public string Data(string key)
        {
            return this.Get(key) ?? string.Empty;
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