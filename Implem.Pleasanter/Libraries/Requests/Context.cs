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
using System.Web.Mvc;
using System.Web.Routing;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Context
    {
        public bool Authenticated;
        public string SessionGuid = Guid.NewGuid().ToString().Replace("-", string.Empty);
        public Dictionary<string, string> SessionData = new Dictionary<string, string>();
        public QueryStrings QueryStrings = new QueryStrings();
        public Forms Forms = new Forms();
        public string FormStringRaw;
        public string FormString;
        public List<PostedFile> PostedFiles = new List<PostedFile>();
        public string HttpMethod;
        public bool Ajax;
        public bool Mobile;
        public Dictionary<string, string> RouteData = new Dictionary<string, string>();
        public string ApplicationPath;
        public string AbsoluteUri;
        public string AbsolutePath;
        public string Url;
        public string UrlReferrer;
        public string Query;
        public string Controller;
        public string Action;
        public long Id;
        public long SiteId;
        public string Page;
        public string Server;
        public int TenantId;
        public int DeptId;
        public int UserId;
        public string LoginId = HttpContext.Current?.User?.Identity.Name;
        public Dept Dept;
        public User User;
        public string UserHostName;
        public string UserHostAddress;
        public string UserAgent;
        public string Language = Parameters.Service.DefaultLanguage;
        public bool Developer;
        public TimeZoneInfo TimeZoneInfo = Environments.TimeZoneInfoDefault;
        public UserSettings UserSettings;
        public bool HasPrivilege;
        public ContractSettings ContractSettings = new ContractSettings();
        public string ApiRequestBody;
        public string RequestDataString { get => ApiRequestBody ?? FormString; }
        public Context(
            bool request = true,
            bool sessionStatus = true,
            bool sessionData = true,
            bool user = true,
            string apiRequestBody = null)
        {
            Set(
                request: request,
                sessionStatus: sessionStatus,
                setData: sessionData,
                user: user,
                apiRequestBody: apiRequestBody);
        }

        public Context(int tenantId, int deptId = 0, int userId = 0, string language = null)
        {
            SetRequests();
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

        public Context(HttpPostedFileBase[] files)
        {
            Set();
            SetPostedFiles(files: files);
        }

        public void Set(
            bool request = true,
            bool sessionStatus = true,
            bool setData = true,
            bool user = true,
            string apiRequestBody = null)
        {
            if (request) SetRequests();
            if (sessionStatus) SetSessionStatuses();
            ApiRequestBody = apiRequestBody;
            if (user && HasRoute())
            {
                var api = RequestDataString.Deserialize<Api>();
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
                        setData: setData);
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
                        setData: setData);
                }
                else
                {
                    if (setData) SetData();
                    if (sessionStatus) Language = SessionLanguage();
                }
            }
        }

        private void Set(UserModel userModel, bool setData)
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
                SetPage();
                if (setData) SetData();
            }
        }

        private void SetData()
        {
            var request = HttpContext.Current.Request;
            SessionData = SessionUtilities.Get(this);
            request.QueryString.AllKeys
                .Where(o => o != null)
                .ForEach(key =>
                    QueryStrings.Add(key, request.QueryString[key]));
            request.Form.AllKeys
                .Where(o => o != null)
                .ForEach(key =>
                    Forms.Add(key, request.Form[key]));
        }

        private void SetPostedFiles(HttpPostedFileBase[] files)
        {
            files?.ForEach(file =>
            {
                PostedFiles.Add(new PostedFile()
                {
                    Guid = file.WriteToTemp(),
                    FileName = file.FileName.Split('\\').Last(),
                    Extension = file.Extension(),
                    Size = file.ContentLength,
                    ContentType = file.ContentType
                });
            });
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
                SessionGuid = HttpContext.Current?.Session.SessionID;
            }
        }

        public double SessionAge()
        {
            return (DateTime.Now - (SessionData.Get("StartTime")?.ToDateTime()
                ?? DateTime.Now)).TotalMilliseconds;
        }

        public double SessionRequestInterval()
        {
            SessionUtilities.SetLastAccessTime(context: this);
            return (DateTime.Now - (SessionData.Get("LastAccessTime")?.ToDateTime()
                ?? DateTime.Now)).TotalMilliseconds;
        }

        private void SetRequests()
        {
            if (HasRoute())
            {
                var request = HttpContext.Current.Request;
                FormStringRaw = HttpContext.Current.Request.Form.ToString();
                FormString = HttpUtility.UrlDecode(FormStringRaw, System.Text.Encoding.UTF8);
                Ajax = new HttpRequestWrapper(request).IsAjaxRequest();
                Mobile = request.Browser.IsMobileDevice;
                RouteData = GetRouteData();
                Server = request.Url.Scheme + "://" + request.Url.Authority;
                ApplicationPath = request.ApplicationPath.EndsWith("/")
                    ? request.ApplicationPath
                    : request.ApplicationPath + "/";
                AbsoluteUri = request.Url.AbsoluteUri;
                AbsolutePath = request.Url.AbsolutePath;
                Url = request.Url.ToString();
                UrlReferrer = request.UrlReferrer?.ToString();
                Query = request.Url.Query;
                Controller = RouteData.Get("controller")?.ToLower() ?? string.Empty;
                Action = RouteData.Get("action")?.ToLower() ?? string.Empty;
                Id = RouteData.Get("id")?.ToLong() ?? 0;
                UserHostName = request.UserHostName;
                UserHostAddress = request.UserHostAddress;
                UserAgent = request.UserAgent;
            }
        }

        public Dictionary<string, string> GetRouteData()
        {
            return HasRoute()
                ? RouteTable.Routes
                    .GetRouteData(new HttpContextWrapper(HttpContext.Current))?
                    .Values
                    .ToDictionary(
                        o => o.Key,
                        o => o.Value.ToString()) ?? new Dictionary<string, string>()
                : new Dictionary<string, string>();
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
                language = SessionData.Get("Language") ?? language;
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

        private void SetPage()
        {
            switch (Controller)
            {
                case "items":
                    SiteId = Rds.ExecuteScalar_long(
                        context: this,
                        statements: Rds.SelectItems(
                            column: Rds.ItemsColumn().SiteId(),
                            where: Rds.ItemsWhere().ReferenceId(Id)));
                    Page = "items/" + SiteId;
                    break;
                default:
                    Page = Controller;
                    break;
            }
        }
        
    }
}