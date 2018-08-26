using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
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

        public static void Abandon()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        public static void Clear(string name)
        {
            HttpContext.Current.Session[name] = null;
        }

        public static bool Created()
        {
            return HttpContext.Current?.Session != null;
        }

        public static void Set(Context context)
        {
            HttpContext.Current.Session["TenantId"] = context.TenantId;
            HttpContext.Current.Session["RdsUser"] =
                Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UserId().DeptId(),
                        where: Rds.UsersWhere().UserId(context.UserId)))
                            .AsEnumerable()
                            .Select(dataRow => new RdsUser()
                            {
                                DeptId = dataRow.Int("DeptId"),
                                UserId = dataRow.Int("UserId")
                            })
                            .FirstOrDefault();
            if (!SiteInfo.TenantCaches.ContainsKey(context.TenantId))
            {
                SiteInfo.Reflesh(context: context);
            }
        }

        public static int TenantId()
        {
            return HttpContext.Current?.Session?["TenantId"].ToInt() ?? 0;
        }

        public static bool LoggedIn()
        {
            return
                HttpContext.Current?.User?.Identity.Name.IsNullOrEmpty() == false &&
                HttpContext.Current?.User.Identity.Name !=
                    RdsUser.UserTypes.Anonymous.ToInt().ToString();
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

        public static Message Message()
        {
            var message = HttpContext.Current.Session["Message"] as Message;
            if (message != null) Clear("Message");
            return message;
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