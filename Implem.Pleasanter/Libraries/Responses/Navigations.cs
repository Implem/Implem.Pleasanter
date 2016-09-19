using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Navigations
    {
        public static string Top()
        {
            return Get();
        }

        public static string BaseUrl()
        {
            return Get(Url.RouteData("controller")) + "/";
        }

        public static string Login()
        {
            return Get("Users", "Login");
        }

        public static string Logout()
        {
            return Get("Users", "Logout");
        }

        public static string Index(string controller)
        {
            return Get(controller);
        }

        public static string ItemIndex(long id)
        {
            return Get("Items", id.ToString(), "Index");
        }

        public static string New(string controller)
        {
            return Get(controller, "New");
        }

        public static string ItemNew(long id)
        {
            return Get("Items", id.ToString(), "New");
        }

        public static string Edit(string controller, long id)
        {
            return Get(controller, id.ToString(), "Edit");
        }

        public static string Edit(string table, long id, string controller)
        {
            return Get(table, id.ToString(), controller, "Edit");
        }

        public static string ItemEdit(long id)
        {
            return Get("Items", id.ToString(), "Edit");
        }

        public static string ItemEdit(long id, string controller)
        {
            return Get("Items", id.ToString(), controller, "Edit");
        }

        public static string ItemView(long id, string action)
        {
            return Get("Items", id.ToString(), action);
        }

        public static string Export(string controller, long id)
        {
            return Get(controller, id.ToString(), "Export");
        }

        public static string Images(params string[] parts)
        {
            var imageUrl = parts.ToList<string>();
            imageUrl.Insert(0, "Images");
            return Get(imageUrl.ToArray());
        }

        public static string Action(string controller)
        {
            return Get(controller, "_action_");
        }

        public static string Action(string controller, long id)
        {
            return Get(controller, id.ToString(), "_action_");
        }

        public static string Action(string table, long id, string controller)
        {
            return Get(table, id.ToString(), controller, "_action_");
        }

        public static string ItemAction(long id)
        {
            return id != -1
                ? Get("Items", id.ToString(), "_action_")
                : Get("Items", "_action_");
        }

        public static string ItemAction(long id, string controller)
        {
            return Get("Items", id.ToString(), controller, "_action_");
        }

        public static string ItemAction(long id, string controller, string action)
        {
            return Get("Items", id.ToString(), controller, action);
        }

        public static string Get(params string[] parts)
        {
            return Url.ApplicationPath() + parts
                .Select(o => Trim(o))
                .Where(o => o != string.Empty)
                .Join("/")
                .ToLower();
        }

        private static string Trim(string data)
        {
            var ret = data;
            ret = ret.StartsWith("/") ? ret.Substring(1) : ret;
            ret = ret.EndsWith("/") ? ret.Substring(0, ret.Length - 1) : ret;
            return ret;
        }
    }
}
