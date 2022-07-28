using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class RouteData
    {
        public static Dictionary<string, string> TenantsEdit()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "edit" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> DeptsIndex()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "index" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> DeptsEdit(int deptId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "edit" },
                { "id", deptId.ToString() },
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

        public static Dictionary<string, string> GroupsEdit(int groupId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "edit" },
                { "id", groupId.ToString() },
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

        public static Dictionary<string, string> UsersEdit(int userId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "edit" },
                { "id", userId.ToString() },
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

        public static Dictionary<string, string> ItemsEdit(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "edit" },
                { "id", id.ToString() },
            };
        }
    }
}
