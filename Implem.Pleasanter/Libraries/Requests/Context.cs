using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Context
    {
        public int TenantId;
        public int DeptId;
        public int UserId;
        public string LoginId;
        public Dept Dept;
        public User User;
        public string Language;
        public bool Developer;
        public TimeZoneInfo TimeZoneInfo;
        public RdsUser RdsUser;
        public UserSettings UserSettings;
        public bool HasPrivilege;
        public string Controller;
        public string Action;
        public long Id;

        public Context(bool empty = false)
        {
            SetBySession(empty);
        }

        public void SetBySession(bool empty = false)
        {
            LoginId = HttpContext.Current?.User?.Identity.Name;
            if (HttpContext.Current?.Session != null && !empty)
            {
                TenantId = HttpContext.Current.Session["TenantId"].ToInt();
                DeptId = HttpContext.Current.Session["DeptId"].ToInt();
                UserId = HttpContext.Current.Session["UserId"].ToInt();
                Dept = SiteInfo.Dept(tenantId: TenantId, deptId: DeptId);
                User = SiteInfo.User(context: this, userId: UserId);
                Language = HttpContext.Current.Session["Language"].ToStr();
                Developer = HttpContext.Current.Session["Developer"].ToBool();
                TimeZoneInfo = HttpContext.Current.Session["TimeZoneInfo"] as TimeZoneInfo;
                RdsUser = HttpContext.Current?.Session?["RdsUser"] as RdsUser;
                UserSettings = HttpContext.Current.Session["UserSettings"]?
                    .ToString()
                    .Deserialize<UserSettings>()
                        ?? new UserSettings();
                HasPrivilege = HttpContext.Current.Session["HasPrivilege"].ToBool();
                Controller = Routes.Controller();
                Action = Routes.Action();
                Id = Routes.Id();
            }
        }

        public Context(int tenantId, int deptId, int userId)
        {
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
        }
    }
}