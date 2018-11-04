using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Context
    {
        public bool Authenticated;
        public bool HasSession;
        public string SessionGuid;
        public double SessionAge;
        public double SessionRequestInterval;
        public Dictionary<string, string> SessionData = new Dictionary<string, string>();
        public string Controller;
        public string Action;
        public long Id;
        public int TenantId;
        public int DeptId;
        public int UserId;
        public string LoginId = HttpContext.Current?.User?.Identity.Name;
        public Dept Dept;
        public User User;
        public string Language = Parameters.Service.DefaultLanguage;
        public string UserHostAddress;
        public bool Developer;
        public TimeZoneInfo TimeZoneInfo;
        public UserSettings UserSettings;
        public bool HasPrivilege;
        public ContractSettings ContractSettings = new ContractSettings();

        public Context(
            bool routeProperties = true,
            bool sessionStatus = true,
            bool sessionData = true,
            bool user = true)
        {
            Set(
                routeProperties: routeProperties,
                sessionStatus: sessionStatus,
                sessionData: sessionData,
                user: user);
        }

        public Context(int tenantId, int deptId = 0, int userId = 0, string language = null)
        {
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
            Language = language ?? Language;
            UserHostAddress = HasRoute()
                ? HttpContext.Current?.Request?.UserHostAddress
                : null;
            SetTenantCaches();
            SetContractSettings();
        }

        public void Set(
            bool routeProperties = true,
            bool sessionStatus = true,
            bool sessionData = true,
            bool user = true)
        {
            if (routeProperties) SetRouteProperties();
            if (sessionStatus) SetSessionStatuses();
            if (user && HttpContext.Current?.Session != null)
            {
                var api = Forms.String().Deserialize<Api>();
                if (api?.ApiKey.IsNullOrEmpty() == false)
                {
                    var userModel = new UserModel().Get(
                        context: this,
                        ss: null,
                        where: Rds.UsersWhere()
                            .ApiKey(api.ApiKey)
                            .Disabled(0));
                    Set(
                        userModel: userModel,
                        sessionData: sessionData);
                }
                else if (!LoginId.IsNullOrEmpty())
                {
                    var userModel = new UserModel().Get(
                        context: this,
                        ss: null,
                        where: Rds.UsersWhere()
                            .LoginId(LoginId)
                            .Disabled(0));
                    Set(
                        userModel: userModel,
                        sessionData: sessionData);
                }
                else
                {
                    if (sessionStatus) Language = SessionLanguage();
                }
            }
        }

        private void Set(UserModel userModel, bool sessionData)
        {
            if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                Authenticated = true;
                TenantId = userModel.TenantId;
                DeptId = userModel.DeptId;
                UserId = userModel.UserId;
                Dept = SiteInfo.Dept(tenantId: TenantId, deptId: DeptId);
                User = SiteInfo.User(context: this, userId: UserId);
                Language = userModel.Language;
                UserHostAddress = HttpContext.Current?.Request?.UserHostAddress;
                Developer = userModel.Developer;
                TimeZoneInfo = userModel.TimeZoneInfo;
                UserSettings = userModel.UserSettings;
                HasPrivilege = Permissions.PrivilegedUsers(loginId: userModel.LoginId);
                SetTenantCaches();
                SetContractSettings();
                if (sessionData) SessionData = SessionUtilities.Get(this);
            }
        }

        public RdsUser RdsUser()
        {
            return new RdsUser()
            {
                TenantId = TenantId,
                DeptId = DeptId,
                UserId = UserId
            };
        }

        public CultureInfo CultureInfo()
        {
            return new CultureInfo(Language);
        }

        public Message Message()
        {
            return SessionData.Get("Message")?.Deserialize<Message>();
        }

        private void SetSessionStatuses()
        {
            if (HttpContext.Current?.Session != null)
            {
                HasSession = true;
                SessionGuid = GetSessionGuid();
                SessionAge = GetSessionAge();
                SessionRequestInterval = GetSessionRequestInterval();
            }
        }

        private static string GetSessionGuid()
        {
            return HttpContext.Current?.Session?["SessionGuid"]?.ToString();
        }

        private static double GetSessionAge()
        {
            return (DateTime.Now - HttpContext.Current.Session["StartTime"].ToDateTime())
                .TotalMilliseconds;
        }

        private static double GetSessionRequestInterval()
        {
            var ret = (DateTime.Now - HttpContext.Current.Session["LastAccessTime"].ToDateTime())
                .TotalMilliseconds;
            HttpContext.Current.Session["LastAccessTime"] = DateTime.Now;
            return ret;
        }

        private void SetRouteProperties()
        {
            Controller = GetController();
            Action = GetAction();
            Id = GetId();
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

        private string SessionLanguage()
        {
            var types = Def.ColumnTable.Users_Language.ChoicesText
                .SplitReturn()
                .Select(o => o.Split_1st())
                .ToList();
            var language = string.Empty;
            if (HasRoute())
            {
                language = QueryStrings.Data("Language");
                if (!language.IsNullOrEmpty())
                {
                    SessionUtilities.Set(
                        context: this,
                        key: "Language",
                        value: language);
                }
                language = SessionUtilities.Language(context: this) ?? language;
            }
            return types.Contains(language)
                ? language
                : Parameters.Service?.DefaultLanguage;
        }

        private static bool HasRoute()
        {
            return RouteTable.Routes.Count != 0 && HttpContext.Current != null;
        }

        public void SetTenantCaches()
        {
            if (TenantId != 0 && !SiteInfo.TenantCaches.ContainsKey(TenantId))
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
            ContractSettings.Deadline = dataRow?.DateTime("ContractDeadline");
        }
    }
}