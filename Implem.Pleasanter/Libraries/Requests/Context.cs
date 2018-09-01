using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Routing;
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
        public ContractSettings ContractSettings;

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
                Controller = GetController();
                Action = GetAction();
                Id = GetId();
                SetTenantCaches();
                SetContractSettings();
            }
            else
            {
                ContractSettings = new ContractSettings();
            }
        }

        public Context(int tenantId, int deptId = 0, int userId = 0)
        {
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
            SetTenantCaches();
            SetContractSettings();
        }

        private void SetTenantCaches()
        {
            if (!SiteInfo.TenantCaches.ContainsKey(TenantId))
            {
                try
                {
                    SiteInfo.TenantCaches.Add(TenantId, new TenantCache(context: this));
                    SiteInfo.Reflesh(context: this);
                }
                catch (Exception)
                {
                }
            }
        }

        private void SetContractSettings()
        {
            var dataRow = Rds.ExecuteTable(
                context: this,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn()
                        .ContractSettings()
                        .ContractDeadline(),
                    where: Rds.TenantsWhere().TenantId(TenantId)))
                        .AsEnumerable()
                        .FirstOrDefault();
            ContractSettings = dataRow?.String("ContractSettings").Deserialize<ContractSettings>()
                ?? new ContractSettings();
            ContractSettings.Deadline = dataRow?.DateTime("ContractDeadline") ;
        }

        private static string GetController()
        {
            return HasRoute()
                ? Url.RouteData("controller").ToString().ToLower()
                : StackTraces.Class();
        }

        private static string GetAction()
        {
            return HasRoute()
                ? Url.RouteData("action").ToString().ToLower()
                : StackTraces.Method();
        }

        private static long GetId()
        {
            return HasRoute()
                ? Url.RouteData("id").ToLong()
                : 0;
        }

        private static bool HasRoute()
        {
            return RouteTable.Routes.Count != 0 && HttpContext.Current != null;
        }
    }
}