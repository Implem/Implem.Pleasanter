using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Locations
    {
        public static string Top(Context context)
        {
            var topUrl = DashboardUrl(context: context)
                ?? Get(
                    context: context,
                    parts: Parameters.Locations.TopUrl);
            return (Permissions.PrivilegedUsers(context.LoginId)
                && Parameters.Locations.LoginAfterUrlExcludePrivilegedUsers)
                    || topUrl.IsNullOrEmpty()
                        ? Get(context: context)
                        : topUrl;
        }

        public static string DashboardUrl(Context context)
        {
            var tenantModel = new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
            var dashboards = tenantModel.TopDashboards
                ?.Deserialize<long[]>()
                    ?? System.Array.Empty<long>();
            var dashboardId = dashboards
                .FirstOrDefault(id => Permissions
                    .CanRead(
                        context: context,
                        siteId: id));
            return dashboardId != 0
                ? ItemIndex(
                    context: context,
                    id: dashboardId)
                : null;
        }

        public static string BaseUrl(Context context)
        {
            return Get(
                context: context,
                parts: context.Controller) + "/";
        }

        public static string Login(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Users",
                    "Login"
                });
        }

        public static string UsersEdit(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Users",
                    id.ToString()
                });
        }

        public static string Logout(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Users",
                    "Logout"
                });
        }

        public static string Admins(Context context)
        {
            return Get(
                context: context,
                parts: "Admins");
        }

        public static string Index(Context context, string controller)
        {
            return Get(
                context: context,
                parts: controller);
        }

        public static string ItemIndex(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    context.Publish
                        ? "Publishes"
                        : "Items",
                    id.ToString(),
                    "Index"
                });
        }

        public static string ItemPdf(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    context.Publish
                        ? "Publishes"
                        : "Items",
                    id.ToString(),
                    "Pdf"
                });
        }

        public static string ItemTrashBox(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Items",
                    id.ToString(),
                    "TrashBox"
                });
        }

        public static string New(Context context, string controller)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    controller,
                    "New"
                });
        }

        public static string ItemNew(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Items",
                    id.ToString(),
                    "New"
                });
        }

        public static string Edit(Context context, string controller)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    controller,
                    "Edit"
                });
        }

        public static string Edit(Context context, string controller, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    controller,
                    id.ToString(),
                    "Edit"
                });
        }

        public static string ItemEdit(Context context, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    context.Publish
                        ? "Publishes"
                        : "Items",
                    id.ToString(),
                    "Edit"
                });
        }

        public static string ItemIndexAbsoluteUri(Context context, long id)
        {
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + "/items/" + id + ""
                : context.AbsoluteUri.Replace(
                    context.AbsolutePath,
                    ItemIndex(
                        context: context,
                        id: id));
        }

        public static string ItemEditAbsoluteUri(Context context, long id)
        {
            var itemEdit = ItemEdit(
                context: context,
                id: id);
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + "/items/" + id
                // BackgroundServiceでリマインダーを動作させている場合にはcontext.AbsoluteUriがnullになる
                // その場合にURLを生成できないため /items/{id}/edit をリマインダーに記載する
                // BackgroundServiceでリマインダーを動作させている場合にはParameters.Service.AbsoluteUriの設定を推奨する
                : context.AbsoluteUri?.Replace(context.AbsolutePath, itemEdit)
                    ?? itemEdit;
        }

        public static string ItemPdfAbsoluteUri(Context context, long id)
        {
            var itemPdf = ItemPdf(
                context: context,
                id: id);
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + "/items/" + id
                : context.AbsoluteUri?.Replace(context.AbsolutePath, itemPdf)
                        ?? itemPdf;
        }

        public static string DemoUri(Context context)
        {
            return Parameters.Service.AbsoluteUri
                ?? context.AbsoluteUri.Replace(
                    context.AbsolutePath,
                    Get(
                        context: context,
                        parts: string.Empty));
        }

        public static string RegistrationUri(Context context, string passphrase)
        {
            var path = "/Registrations/login?passphrase=" + passphrase;
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + path
                : context.AbsoluteUri.Replace(
                    context.AbsolutePath,
                    Get(
                        context: context,
                        parts: path));
        }

        public static string RegistrationEditUri(Context context, string id)
        {
            var path = "/Registrations/" + id + "/edit";
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + path
                : context.AbsoluteUri.Replace(
                    context.AbsolutePath,
                    Get(
                        context: context,
                        parts: path));
        }

        public static string ApprovaUri(Context context)
        {
            var path = "/users/login";
            return Parameters.Service.AbsoluteUri != null
                ? Parameters.Service.AbsoluteUri + path
                : context.AbsoluteUri.Replace(
                    context.AbsolutePath,
                    Get(
                        context: context,
                        parts: path));
        }


        public static string OutGoingMailAbsoluteUri(Context context)
        {
            var controller = context.Forms.Get("Controller");
            var id = context.Forms.Get("Id");
            return Parameters.Service.AbsoluteUri != null
                ? $"{Parameters.Service.AbsoluteUri}/{controller}/{id}"
                : context.AbsoluteUri.Substring(0, context.AbsoluteUri.IndexOf("/outgoingmails"));
        }

        public static string AbsoluteDirectUri(Context context)
        {
            return Parameters.Service.AbsoluteUri != null
                ? $"{Parameters.Service.AbsoluteUri}/{context.Controller}/{context.Id}/index"
                : context.AbsoluteUri;
        }

        public static string ItemView(Context context, long id, string action)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Items",
                    id.ToString(),
                    action
                });
        }

        public static string Images(Context context, params string[] parts)
        {
            var imageUrl = parts.ToList();
            imageUrl.Insert(0, "Images");
            return Get(
                context: context,
                parts: imageUrl.ToArray());
        }

        public static string Action(Context context, string controller)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    controller,
                    "_action_"
                });
        }

        public static string Action(Context context, string controller, long id)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    controller,
                    id.ToString(),
                    "_action_"
                });
        }

        public static string Action(Context context, string table, long id, string controller)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    table,
                    id.ToString(),
                    controller,
                    "_action_"
                });
        }

        public static string ItemAction(Context context, long id)
        {
            return id != -1
                ? Get(
                    context: context,
                    parts: new string[]
                    {
                        context.Publish
                            ? "Publishes"
                            : "Items",
                        id.ToString(),
                        "_action_"
                    })
                : Get(
                    context: context,
                    parts: new string[]
                    {
                        "Items",
                        "_action_"
                    });
        }

        public static string DeleteImage(Context context, string guid)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "binaries",
                    guid,
                    "deleteimage"
                });
        }

        public static string DownloadFile(Context context, string guid, bool temp = false)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "binaries",
                    guid,
                    !temp
                        ? "/download"
                        : "/downloadtemp"
                });
        }

        public static string ShowFile(Context context, string guid, bool temp = false)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "binaries",
                    guid,
                    !temp
                        ? "/show"
                        : "/showtemp"
                });
        }

        public static string BadRequest(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "BadRequest"
                });
        }

        public static string InvalidIpAddress(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "InvalidIpAddress"
                });
        }

        public static string LoginIdAlreadyUse(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "LoginIdAlreadyUse"
                });
        }

        public static string UserLockout(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "UserLockout"
                });
        }

        public static string UserDisabled(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "UserDisabled"
                });
        }

        public static string SamlLoginFailed(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "SamlLoginFailed"
                });
        }

        public static string InvalidSsoCode(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "InvalidSsoCode"
                });
        }

        public static string EmptyUserName(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "EmptyUserName"
                });
        }

        public static string ParameterSyntaxError(Context context)
        {
            return Get(
                context: context,
                parts: new string[]
                {
                    "Errors",
                    "ParameterSyntaxError"
                });
        }

        public static string ApplicationError(Context context)
        {
            return Get(
                context: context,
                parts: "Errors");
        }

        public static string Get(Context context, params string[] parts)
        {
            return Raw(
                context: context,
                parts: parts).ToLower();
        }

        public static string Raw(Context context, params string[] parts)
        {
            return context.ApplicationPath + parts
                .Select(o => Trim(o))
                .Where(o => o != string.Empty)
                .Join("/");
        }

        private static string Trim(string data)
        {
            var ret = data;
            ret = ret?.StartsWith("/") == true
                ? ret.Substring(1)
                : ret;
            ret = ret?.EndsWith("/") == true
                ? ret.Substring(0, ret.Length - 1)
                : ret;
            return ret;
        }
    }
}
