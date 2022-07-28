using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterTest.Utilities
{
    public static class RouteData
    {
        public static Dictionary<string, string> ItemsIndex(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "index" },
                { "id", siteId.ToString() },
            };
        }

    }
}
