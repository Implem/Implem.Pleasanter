using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Locations
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

        public static string ItemEdit(long id)
        {
            return Get("Items", id.ToString(), "Edit");
        }

        public static string ItemEditAbsoluteUri(long id)
        {
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + "/items/" + id
                : Url.AbsoluteUri().Replace(Url.AbsolutePath(), ItemEdit(id));
        }

        public static string DemoUri(string passphrase)
        {
            var path = "/demos/login?passphrase=" + passphrase;
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + path
                : Url.AbsoluteUri().Replace(Url.AbsolutePath(), Get(path));
        }

        public static string ItemView(long id, string action)
        {
            return Get("Items", id.ToString(), action);
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

        public static string DeleteImage(string guid)
        {
            return Get("binaries", guid, "deleteimage");
        }

        public static string DownloadFile(string guid, bool temp = false)
        {
            return Get("binaries", guid, !temp
                ? "/download"
                : "/downloadtemp");
        }

        public static string ShowFile(string guid, bool temp = false)
        {
            return Get("binaries", guid, !temp
                ? "/show"
                : "/showtemp");
        }

        public static string BadRequest()
        {
            return Get("Errors", "BadRequest");
        }

        public static string ParameterSyntaxError()
        {
            return Get("Errors", "ParameterSyntaxError");
        }

        public static string ApplicationError()
        {
            return Get("Errors");
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
