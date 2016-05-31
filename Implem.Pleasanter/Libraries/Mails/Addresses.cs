using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class Addresses
    {
        public static IEnumerable<string> GetEnumerable(string addresses)
        {
            return addresses.Split(';')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        public static string BadAddress(string addresses)
        {
            foreach (var address in GetEnumerable(addresses))
            {
                if (Get(address) == string.Empty)
                {
                    return address;
                }
            }
            return string.Empty;
        }

        public static string Get(string address)
        {
            return address.RegexFirst(
                @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}