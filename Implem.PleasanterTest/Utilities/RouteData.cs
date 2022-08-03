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
        public static Dictionary<string, string> DeptsNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "new" },
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

        public static Dictionary<string, string> DeptsUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> DeptsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "deletecomment" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> DeptsDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "delete" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> DeptsHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "histories" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> DeptsApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "depts" },
                { "action", "get" },
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

        public static Dictionary<string, string> GroupsNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "new" },
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

        public static Dictionary<string, string> GroupsUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "deletecomment" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "delete" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "histories" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "get" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsApiCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "create" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> GroupsApiUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "groups" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> GroupsApiDelete(int id)
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

        public static Dictionary<string, string> UsersNew()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "new" },
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

        public static Dictionary<string, string> UsersCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "create" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> UsersUpdate(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "deletecomment" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersDelete(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "delete" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersHistories(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "histories" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersResetPassword(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "resetpassword" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersApiGet(int id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "get" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersApiCreate()
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "create" },
                { "id", "0" },
            };
        }

        public static Dictionary<string, string> UsersApiUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> UsersApiDelete(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "users" },
                { "action", "delete" },
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

        public static Dictionary<string, string> ItemsDeleteComment(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "deletecomment" },
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

        public static Dictionary<string, string> ItemsHistories(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "histories" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsHistory(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "history" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsSelectedIds(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "selectedids" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsLinkTable(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "linktable" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsOpenExportSelectorDialog(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "openexportselectordialog" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsApiGet(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "get" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsApiUpdate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "update" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsApiCreate(long id)
        {
            return new Dictionary<string, string>()
            {
                { "controller", "items" },
                { "action", "create" },
                { "id", id.ToString() },
            };
        }

        public static Dictionary<string, string> ItemsApiDelete(long id)
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
