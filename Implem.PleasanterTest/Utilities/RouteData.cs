using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class RouteData
    {
        public static Dictionary<string, string> DeptsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "index" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> GroupsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "index" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> UsersIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "index" },
                { "id", "0" },
            };
        }

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
