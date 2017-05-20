using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Globalization;
using System.Web;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Sessions
    {
        public static string Data(string name)
        {
            return HttpContext.Current.Session[name] != null
                ? HttpContext.Current.Session[name].ToString()
                : string.Empty;
        }

        public static void Set(string name, object data)
        {
            HttpContext.Current.Session[name] = data;
        }

        public static void Clear(string name)
        {
            HttpContext.Current.Session[name] = null;
        }

        public static bool Created()
        {
            return HttpContext.Current?.Session != null;
        }

        public static void SetTenantId(int tenantId)
        {
            HttpContext.Current.Session["TenantId"] = tenantId;
            SiteInfo.Reflesh();
        }

        public static int TenantId()
        {
            if (HttpContext.Current?.Session != null)
            {
                return HttpContext.Current.Session["TenantId"].ToInt();
            }
            else
            {
                return 0;
            }
        }

        public static bool LoggedIn()
        {
            return UserId() != Implem.Libraries.Classes.RdsUser.UserTypes.Anonymous.ToInt();
        }

        private static int UserIdentity()
        {
            return HttpContext.Current?.User.Identity.Name.ToInt() ?? 0;
        }

        public static int UserId()
        {
            return (
                HttpContext.Current?.User != null &&
                HttpContext.Current?.User.Identity.Name != string.Empty)
                    ? UserIdentity()
                    : Implem.Libraries.Classes.RdsUser.UserTypes.Anonymous.ToInt();
        }

        public static int DeptId()
        {
            return (
                HttpContext.Current?.User != null &&
                HttpContext.Current?.User.Identity.Name != string.Empty)
                    ? SiteInfo.User(UserIdentity()).DeptId
                    : 0;
        }

        public static User User()
        {
            return SiteInfo.User(UserId());
        }

        public static RdsUser RdsUser()
        {
            return HttpContext.Current?.Session?["RdsUser"] as RdsUser;
        }

        public static string Language()
        {
            return Created() && HttpContext.Current.Session["Language"] != null
                ? HttpContext.Current.Session["Language"].ToString()
                : string.Empty;
        }

        public static CultureInfo CultureInfo()
        {
            return new CultureInfo(Language());
        }

        public static bool Developer()
        {
            return HttpContext.Current.Session["Developer"].ToBool();
        }

        public static TimeZoneInfo TimeZoneInfo()
        {
            return 
                HttpContext.Current?.Session?["TimeZoneInfo"] as TimeZoneInfo ??
                Environments.TimeZoneInfoDefault;
        }

        public static UserSettings UserSettings()
        {
            return HttpContext.Current?.Session?["UserSettings"]
                .ToString().Deserialize<UserSettings>() ?? new UserSettings();
        }

        public static double SessionAge()
        {
            return Created()
                ? (DateTime.Now - HttpContext.Current.Session["StartTime"].ToDateTime())
                    .TotalMilliseconds
                : 0;
        }

        public static double SessionRequestInterval()
        {
            if (Created())
            {
                var ret = (DateTime.Now - HttpContext.Current.Session["LastAccessTime"].ToDateTime())
                    .TotalMilliseconds;
                HttpContext.Current.Session["LastAccessTime"] = DateTime.Now;
                return ret;
            }
            else
            {
                return 0;
            }
        }

        public static string SessionGuid()
        {
            return HttpContext.Current.Session?["SessionGuid"].ToString();
        }

        public static string Message()
        {
            var html = Data("Message");
            if (html != string.Empty)
            {
                Clear("Message");
                return html;
            }
            else
            {
                return string.Empty;
            }
        }

        public static object PageSession(long id, string name = "")
        {
            return HttpContext.Current.Session[Pages.Key() + name.ExistsTo("/{0}")];
        }

        public static object PageSession(this BaseModel baseModel, string name)
        {
            return HttpContext.Current.Session[Pages.Key(baseModel, name)];
        }

        public static void PageSession(this BaseModel baseModel, string name, object value)
        {
            HttpContext.Current.Session[Pages.Key(baseModel, name)] = value;
        }
    }
}