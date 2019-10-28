using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
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
        public bool SwitchUser;
        public string SessionGuid = Strings.NewGuid();
        public Dictionary<string, string> SessionData = new Dictionary<string, string>();
        public bool Publish;
        public QueryStrings QueryStrings = new QueryStrings();
        public Forms Forms = new Forms();
        public string FormStringRaw;
        public string FormString;
        public List<PostedFile> PostedFiles = new List<PostedFile>();
        public bool HasRoute = RouteTable.Routes.Count != 0 && HttpContext.Current != null;
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
        public string Page;
        public string Server;
        public int TenantId;
        public long SiteId;
        public long Id;
        public Dictionary<long, Permissions.Types> PermissionHash;
        public string Guid;
        public TenantModel.LogoTypes LogoType;
        public string TenantTitle;
        public string SiteTitle;
        public string RecordTitle;
        public bool DisableAllUsersPermission;
        public bool DisableStartGuide;
        public string HtmlTitleTop;
        public string HtmlTitleSite;
        public string HtmlTitleRecord;
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
        public decimal ApiVersion = 1.000M;
        public string ApiRequestBody;
        public string RequestDataString { get => ApiRequestBody ?? FormString; }

        public Context(
            bool request = true,
            bool sessionStatus = true,
            bool sessionData = true,
            bool user = true,
            bool item = true,
            string apiRequestBody = null)
        {
            Set(
                request: request,
                sessionStatus: sessionStatus,
                setData: sessionData,
                user: user,
                item: item,
                apiRequestBody: apiRequestBody);
        }

        public Context(int tenantId, int deptId = 0, int userId = 0, string language = null)
        {
            SetRequests();
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
            Language = language ?? Language;
            UserHostAddress = HasRoute
                ? GetUserHostAddress(HttpContext.Current.Request)
                : null;
            SetTenantProperties();
            SetPublish();
            SetPermissions();
            SetTenantCaches();
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
            bool item = true,
            string apiRequestBody = null)
        {
            ApiRequestBody = apiRequestBody;
            if (request) SetRequests();
            if (sessionStatus) SetSessionGuid();
            if (item) SetItemProperties();
            if (user) SetUserProperties(sessionStatus, setData);
            SetTenantProperties();
            if (request) SetPublish();
            if (request) SetPermissions();
            SetTenantCaches();
        }

        private void SetRequests()
        {
            if (HasRoute)
            {
                var request = HttpContext.Current.Request;
                FormStringRaw = HttpContext.Current.Request.Form.ToString();
                FormString = HttpUtility.UrlDecode(FormStringRaw, System.Text.Encoding.UTF8);
                HttpMethod = request.HttpMethod;
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
                Guid = RouteData.Get("guid");
                UserHostName = request.UserHostName;
                UserHostAddress = GetUserHostAddress(request);
                UserAgent = request.UserAgent;
            }
        }

        private void SetSessionGuid()
        {
            SessionGuid = HttpContext.Current?.Request?.Cookies["ASP.NET_SessionId"]?.Value
                ?? SessionGuid;
        }

        private void SetItemProperties()
        {
            if (HasRoute)
            {
                switch (Controller)
                {
                    case "items":
                    case "publishes":
                        Rds.ExecuteTable(
                            context: this,
                            statements: Rds.SelectItems(
                                column: Rds.ItemsColumn()
                                    .SiteId()
                                    .ReferenceId()
                                    .Title(),
                                where: Rds.ItemsWhere()
                                    .Add(or: Rds.ItemsWhere()
                                        .ReferenceId(Id)
                                        .ReferenceId(sub: Rds.SelectItems(
                                            column: Rds.ItemsColumn().SiteId(),
                                            where: Rds.ItemsWhere().ReferenceId(Id)))),
                                distinct: true))
                                    .AsEnumerable()
                                    .ForEach(dataRow =>
                                    {
                                        if (dataRow.Long("SiteId") == dataRow.Long("ReferenceId"))
                                        {
                                            SiteId = dataRow.Long("ReferenceId");
                                            SiteTitle = dataRow.String("Title");
                                        }
                                        else
                                        {
                                            RecordTitle = dataRow.String("Title");
                                        }
                                    });
                        Page = Controller + "/"
                            + SiteId
                            + (TrashboxActions()
                                ? "/trashbox"
                                : string.Empty);
                        break;
                    default:
                        Page = Controller;
                        break;
                }
            }
        }

        private void SetUserProperties(bool sessionStatus, bool setData)
        {
            if (HasRoute)
            {
                if (setData) SetData();
                var api = RequestDataString.Deserialize<Api>();
                if (api?.ApiKey.IsNullOrEmpty() == false)
                {
                    ApiVersion = api.ApiVersion;
                    SetUser(userModel: GetUser(where: Rds.UsersWhere()
                        .ApiKey(api.ApiKey)));
                }
                else if (!LoginId.IsNullOrEmpty())
                {
                    ApiVersion = api?.ApiVersion ?? ApiVersion;
                    SetUser(userModel: GetUser(where: Rds.UsersWhere()
                        .LoginId(Strings.CoalesceEmpty(
                            Permissions.PrivilegedUsers(
                                loginId: HttpContext.Current?.User?.Identity.Name)
                                    ? SessionData.Get("SwitchLoginId")
                                    : null,
                            LoginId))));
                }
                else
                {
                    if (sessionStatus) Language = SessionLanguage();
                }
            }
        }

        private UserModel GetUser(Rds.UsersWhereCollection where)
        {
            return new UserModel().Get(
                context: this,
                ss: null,
                where: where
                    .Disabled(false)
                    .Lockout(false));
        }

        private void SetUser(UserModel userModel)
        {
            if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                SwitchUser = LoginId != userModel.LoginId;
                Authenticated = true;
                TenantId = userModel.TenantId;
                DeptId = userModel.DeptId;
                UserId = userModel.UserId;
                Dept = SiteInfo.Dept(tenantId: TenantId, deptId: DeptId);
                User = SiteInfo.User(context: this, userId: UserId);
                Language = userModel.Language;
                UserHostAddress = GetUserHostAddress(HttpContext.Current.Request);
                Developer = userModel.Developer;
                TimeZoneInfo = userModel.TimeZoneInfo;
                UserSettings = userModel.UserSettings;
                HasPrivilege = Permissions.PrivilegedUsers(userModel.LoginId);
            }
        }

        private void SetTenantProperties()
        {
            if (HasRoute)
            {
                var dataRow = Rds.ExecuteTable(
                    context: this,
                    statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn()
                            .Title()
                            .ContractSettings()
                            .ContractDeadline()
                            .LogoType()
                            .DisableAllUsersPermission()
                            .DisableStartGuide()
                            .HtmlTitleTop()
                            .HtmlTitleSite()
                            .HtmlTitleRecord(),
                        where: Rds.TenantsWhere().TenantId(TenantId)))
                            .AsEnumerable()
                            .FirstOrDefault();
                if (dataRow != null)
                {
                    TenantTitle = dataRow.String("Title");
                    ContractSettings = dataRow.String("ContractSettings")
                        .Deserialize<ContractSettings>() ?? ContractSettings;
                    ContractSettings.Deadline = dataRow?.DateTime("ContractDeadline");
                    LogoType = (TenantModel.LogoTypes)dataRow.Int("LogoType");
                    DisableAllUsersPermission = dataRow.Bool("DisableAllUsersPermission");
                    DisableStartGuide = dataRow.Bool("DisableStartGuide");
                    HtmlTitleTop = dataRow.String("HtmlTitleTop");
                    HtmlTitleSite = dataRow.String("HtmlTitleSite");
                    HtmlTitleRecord = dataRow.String("HtmlTitleRecord");
                }
            }
        }

        private void SetData()
        {
            SessionData = SessionUtilities.Get(
                context: this, 
                includeUserArea: Controller == "sessions");
            var request = HttpContext.Current.Request;
            request.QueryString.AllKeys
                .Where(o => o != null)
                .ForEach(key =>
                    QueryStrings.AddIfNotConainsKey(key, request.QueryString[key]));
            request.Form.AllKeys
                .Where(o => o != null)
                .ForEach(key =>
                    Forms.AddIfNotConainsKey(key, request.Form[key]));
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

        private void SetPublish()
        {
            if (HasRoute)
            {
                switch (Controller)
                {
                    case "publishes":
                    case "publishbinaries":
                        var dataRow = Rds.ExecuteTable(
                            context: this,
                            statements: Rds.SelectSites(
                                column: Rds.SitesColumn()
                                    .TenantId()
                                    .Publish()
                                    .Tenants_ContractSettings()
                                    .Tenants_HtmlTitleTop()
                                    .Tenants_HtmlTitleSite()
                                    .Tenants_HtmlTitleRecord(),
                                join: Rds.SitesJoin().Add(new SqlJoin(
                                    tableBracket: "[Tenants]",
                                    joinType: SqlJoin.JoinTypes.Inner,
                                    joinExpression: "[Sites].[TenantId]=[Tenants].[TenantId]")),
                                where: Rds.SitesWhere()
                                    .SiteId(sub: Rds.SelectItems(
                                        column: Rds.ItemsColumn().SiteId(),
                                        where: Controller == "publishes"
                                            ? Rds.ItemsWhere().ReferenceId(Id)
                                            : Rds.ItemsWhere().ReferenceId(sub: Rds.SelectBinaries(
                                                column: Rds.BinariesColumn().ReferenceId(),
                                                where: Rds.BinariesWhere().Guid(Guid)))))))
                                                    .AsEnumerable()
                                                    .FirstOrDefault();
                        var publish = dataRow.Bool("Publish");
                        if (publish)
                        {
                            var cs= dataRow.String("ContractSettings")
                                .Deserialize<ContractSettings>() ?? ContractSettings;
                            if (cs.Extensions.Get("Publish"))
                            {
                                TenantId = dataRow.Int("TenantId");
                                Publish = true;
                                ContractSettings = cs;
                                HtmlTitleTop = dataRow.String("HtmlTitleTop");
                                HtmlTitleSite = dataRow.String("HtmlTitleSite");
                                HtmlTitleRecord = dataRow.String("HtmlTitleRecord");
                            }
                        }
                        break;
                }
            }
        }

        public Dictionary<string, string> GetRouteData()
        {
            return HasRoute
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
            if (HasRoute)
            {
                language = QueryStrings.Data("Language");
                if (!language.IsNullOrEmpty())
                {
                    SessionUtilities.Set(
                        context: this,
                        key: "Language",
                        value: language);
                }
                else
                {
                    switch (HttpAcceptLanguage())
                    {
                        case "en":
                        case "en-GB":
                        case "en-US":
                            language = "en";
                            break;
                        default:
                            language= Parameters.Service?.DefaultLanguage;
                            break;
                    }
                }
                language = SessionData.Get("Language") ?? language;
            }
            return types.Contains(language)
                ? language
                : Parameters.Service?.DefaultLanguage;
        }

        private static string HttpAcceptLanguage()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]?.Split_1st();
        }

        private void SetPermissions()
        {
            PermissionHash = Permissions.Get(context: this);
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

        private string GetUserHostAddress(HttpRequest request)
        {
            var address = request?.Headers["X-Forwarded-For"]?.Split(',')?.FirstOrDefault();
            if (address == null)
            {
                return request?.UserHostAddress;
            }
            var sn = address.IndexOf("[");
            var en = address.IndexOf("]");
            if (sn >= 0 && en > sn)
            {
                return address.Substring(sn + 1, en - 1);
            }
            var n = address.IndexOf(":");
            return (n > 0) ? address.Substring(0, n) : address;
        }

        public string RequestData(string name)
        {
            return HttpMethod == "GET"
                ? QueryStrings.Data(name)
                : Forms.Data(name);
        }

        public bool TrashboxActions()
        {
            switch (Action)
            {
                case "trashbox":
                case "trashboxgridrows":
                case "restore":
                case "physicaldelete":
                    return true;
                default:
                    return false;
            }
        }
    }
}