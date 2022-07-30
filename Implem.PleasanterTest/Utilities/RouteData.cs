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

        public static Dictionary<string, string> TenantsUpdate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "tenants" },
                { "action", "update" },
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

        public static Dictionary<string, string> DeptsCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "create" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> DeptsUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "update" },
                { "id", id.ToString() },
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

        public static Dictionary<string, string> GroupsCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "create" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> GroupsUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "delete" },
                { "id", id.ToString() },
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

        public static Dictionary<string, string> UsersCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "create" },
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

        public static Dictionary<string, string> UsersUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "update" },
                { "id", id.ToString() },
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

        public static Dictionary<string, string> ItemsGridRows(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "gridrows" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsTrashBox(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "trashbox" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsTrashBoxGridRows(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "trashboxgridrows" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsCalendar(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "calendar" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsCrosstab(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "crosstab" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsGantt(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "gantt" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsBurnDown(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "burndown" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsTimeSeries(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "timeseries" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsKamban(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "kamban" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsImageLib(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "imagelib" },
                { "id", siteId.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsNew(long siteId)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "new" },
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

        public static Dictionary<string, string> ItemsCreate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "create" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "delete" },
                { "id", id.ToString() },
            };
        }
    }
}
