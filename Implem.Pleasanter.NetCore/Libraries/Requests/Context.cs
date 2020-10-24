using AspNetCoreCurrentRequestContext;
using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.NetCore.Libraries.Requests
{
    public class ContextImplement : Context
    {
        public override Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public override bool Authenticated { get; set; }
        public override bool SwitchUser { get; set; }
        public override string SessionGuid { get; set; } = Strings.NewGuid();
        public override Dictionary<string, string> SessionData { get; set; } = new Dictionary<string, string>();
        public override Dictionary<string, string> UserSessionData { get; set; } = new Dictionary<string, string>();
        public override bool Publish { get; set; }
        public override QueryStrings QueryStrings { get; set; } = new QueryStrings();
        public override Forms Forms { get; set; } = new Forms();
        public override string FormStringRaw { get; set; }
        public override string FormString { get; set; }
        public override List<PostedFile> PostedFiles { get; set; } = new List<PostedFile>();
        public override bool HasRoute { get; set; } = AspNetCoreHttpContext.Current != null;
        public override string HttpMethod { get; set; }
        public override bool Ajax { get; set; }
        public override bool Mobile { get; set; }
        public override Dictionary<string, string> RouteData { get; set; } = new Dictionary<string, string>();
        public override string ApplicationPath { get; set; }
        public override string AbsoluteUri { get; set; }
        public override string AbsolutePath { get; set; }
        public override string Url { get; set; }
        public override string UrlReferrer { get; set; }
        public override string Query { get; set; }
        public override string Controller { get; set; }
        public override string Action { get; set; }
        public override string Page { get; set; }
        public override string Server { get; set; }
        public override int TenantId { get; set; }
        public override long SiteId { get; set; }
        public override long Id { get; set; }
        public override Dictionary<long, Permissions.Types> PermissionHash { get; set; }
        public override string Guid { get; set; }
        public override TenantModel.LogoTypes LogoType { get; set; }
        public override string TenantTitle { get; set; }
        public override string SiteTitle { get; set; }
        public override string RecordTitle { get; set; }
        public override bool DisableAllUsersPermission { get; set; }
        public override string HtmlTitleTop { get; set; }
        public override string HtmlTitleSite { get; set; }
        public override string HtmlTitleRecord { get; set; }
        public override int DeptId { get; set; }
        public override int UserId { get; set; }
        public override string LoginId { get; set; } = AspNetCoreHttpContext.Current?.User?.Identity?.Name;
        public override Dept Dept { get; set; }
        public override User User { get; set; }
        public override string UserHostName { get; set; }
        public override string UserHostAddress { get; set; }
        public override string UserAgent { get; set; }
        public override string Language { get; set; } = Parameters.Service.DefaultLanguage;
        public override bool Developer { get; set; }
        public override TimeZoneInfo TimeZoneInfo { get; set; } = Environments.TimeZoneInfoDefault;
        public override UserSettings UserSettings { get; set; }
        public override bool HasPrivilege { get; set; }
        public override ContractSettings ContractSettings { get; set; } = new ContractSettings();
        public override decimal ApiVersion { get; set; } = 1.000M;
        public override string ApiRequestBody { get; set; }
        public override string RequestDataString { get => !string.IsNullOrEmpty(ApiRequestBody) ? ApiRequestBody : FormString; }

        public override string AuthenticationType { get => AspNetCoreHttpContext.Current?.User?.Identity?.AuthenticationType; }
        public override bool? IsAuthenticated { get => AspNetCoreHttpContext.Current?.User?.Identity?.IsAuthenticated; }

        public override IEnumerable<Claim> UserClaims { get => AspNetCoreHttpContext.Current?.User?.Claims; }

        public ContextImplement(
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
            if (ApiRequestBody != null)
            {
                SiteInfo.Reflesh(context: this);
            }
        }

        public ContextImplement(int tenantId, int deptId = 0, int userId = 0, string language = null)
        {
            SetRequests();
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
            Language = language ?? Language;
            UserHostAddress = HasRoute
                ? GetUserHostAddress(AspNetCoreHttpContext.Current?.Connection)
                : null;
            SetTenantProperties();
            SetPublish();
            SetPermissions();
            SetTenantCaches();
        }

        public ContextImplement(ICollection<IFormFile> files)
        {
            Set();
            SetPostedFiles(files: files);
        }

        public override void Set(
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
                var request = AspNetCoreHttpContext.Current?.Request;
                FormStringRaw = CreateFormStringRaw(AspNetCoreHttpContext.Current.Request);
                FormString = HttpUtility.UrlDecode(FormStringRaw, System.Text.Encoding.UTF8);
                HttpMethod = request?.Method;
                Ajax = IsAjax();
                Mobile = IsMobile(AspNetCoreHttpContext.Current.Request);
                RouteData = GetRouteData();
                Server = CreateServer();
                ApplicationPath = CreateApplicationPath();
                AbsoluteUri = CreateAbsoluteUri();
                AbsolutePath = CreateAbsolutePath();
                Url = CreateUrl(AspNetCoreHttpContext.Current.Request);
                UrlReferrer = CreateUrlReferrer(AspNetCoreHttpContext.Current.Request);
                Query = AspNetCoreHttpContext.Current.Request.QueryString.ToString();
                Controller = RouteData.Get("controller")?.ToLower() ?? string.Empty;
                Action = RouteData.Get("action")?.ToLower() ?? string.Empty;
                Id = RouteData.Get("id")?.ToLong() ?? 0;
                Guid = RouteData.Get("guid")?.ToUpper();
                UserHostName = GetUserHostAddress(request?.HttpContext?.Connection);
                UserHostAddress = CreateUserHostAddress(AspNetCoreHttpContext.Current.Request);
                UserAgent = CreateUserAgent(AspNetCoreHttpContext.Current.Request);
            }
        }

        private void SetSessionGuid()
        {
            try
            {
                var session = AspNetCoreHttpContext.Current?.Session;
            }
            catch (InvalidOperationException)
            {
                return;
            }
            var sessionGuid = GetSessionData<string>("SessionGuid");
            if (!string.IsNullOrWhiteSpace(sessionGuid))
            {
                SessionGuid = sessionGuid;
            }
            else
            {
                SetSessionData("SessionGuid", SessionGuid);
            }
        }

        private void SetItemProperties()
        {
            if (HasRoute)
            {
                switch (Controller)
                {
                    case "items":
                    case "publishes":
                        Repository.ExecuteTable(
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
                    SetUser(userModel: GetUser(where: Rds.UsersWhere()
                        .LoginId(Strings.CoalesceEmpty(
                            Permissions.PrivilegedUsers(
                                loginId: AspNetCoreHttpContext.Current?.User?.Identity.Name)
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
                UserHostAddress = GetUserHostAddress(AspNetCoreHttpContext.Current?.Connection);
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
                var dataRow = Repository.ExecuteTable(
                    context: this,
                    statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn()
                            .Title()
                            .ContractSettings()
                            .ContractDeadline()
                            .LogoType()
                            .DisableAllUsersPermission()
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
            var request = AspNetCoreHttpContext.Current.Request;
            foreach (var o in request.QueryString.Value?.PadLeft(1, '?').Substring(1).Split('&'))
            {
                var keyAndValue = o.Split('=');
                var key = HttpUtility.UrlDecode(keyAndValue.FirstOrDefault());
                var value = HttpUtility.UrlDecode(keyAndValue.Skip(1).FirstOrDefault());
                QueryStrings[key] = value;
            }
            if (request.HasFormContentType)
                request.Form.Keys
                    .Where(o => o != null)
                    .ForEach(key =>
                        Forms.AddIfNotConainsKey(key, request.Form[key]));
        }

        private void SetPostedFiles(ICollection<IFormFile> files)
        {
            files?.ForEach(file =>
            {
                PostedFiles.Add(new PostedFile()
                {
                    Guid = new HttpPostedFile(file).WriteToTemp(),
                    FileName = file.FileName.Split(System.IO.Path.DirectorySeparatorChar).Last(),
                    Extension = new HttpPostedFile(file).Extension(),
                    Size = file.Length,
                    ContentType = file.ContentType
                });
            });
        }

        public override RdsUser RdsUser()
        {
            return new RdsUser()
            {
                TenantId = TenantId,
                DeptId = DeptId,
                UserId = UserId
            };
        }

        public override CultureInfo CultureInfo()
        {
            return new CultureInfo(Language);
        }

        public override Message Message()
        {
            return SessionData.Get("Message")?.Deserialize<Message>();
        }

        public override double SessionAge()
        {
            return (DateTime.Now - (SessionData.Get("StartTime")?.ToDateTime()
                ?? DateTime.Now)).TotalMilliseconds;
        }

        public override double SessionRequestInterval()
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
                        var dataRow = Repository.ExecuteTable(
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
                                    tableBracket: "\"Tenants\"",
                                    joinType: SqlJoin.JoinTypes.Inner,
                                    joinExpression: "\"Sites\".\"TenantId\"=\"Tenants\".\"TenantId\"")),
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
                            var cs = dataRow.String("ContractSettings")
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

        public override Dictionary<string, string> GetRouteData()
        {
            return AspNetCoreHttpContext.Current.GetRouteData()?.Values?
                    .ToDictionary(
                        o => o.Key.ToLower(),
                        o => o.Value.ToString())
                            ?? new Dictionary<string, string>();
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
                            language = Parameters.Service?.DefaultLanguage;
                            break;
                    }
                }
                language = SessionData.Get("Language") ?? language;
            }
            return types.Contains(language)
                ? language
                : Parameters.Service?.DefaultLanguage;
        }

        static string HttpAcceptLanguage()
        {
            return AspNetCoreHttpContext.Current?.Request?.GetTypedHeaders()?.AcceptLanguage?.FirstOrDefault()?.Value.ToString();
        }

        public override void SetTenantCaches()
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

        static bool IsAjax()
        {
            return IsAjaxRequest(AspNetCoreHttpContext.Current.Request);
        }

        static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }


        bool IsMobile(HttpRequest request)
        {
            return request?.Headers.TryGetValue("User-Agent", out var value) == true ? value.Any(v => v.Contains("Mobile")) : false;
        }

        string CreateHttpMethod(HttpRequest request)
        {
            return request != null
               ? request.Method
               : null;
        }

        string CreateUrl(HttpRequest request)
        {
            return request != null
               ? $"{request.Scheme}://{request.Host.Value}{request.Path.Value}{request.QueryString.Value}"
               : null;
        }

        public string CreateUrlReferrer(HttpRequest request)
        {
            return request.Headers.TryGetValue("Referer", out var value)
                ? value.FirstOrDefault().ToString()
                : null;
        }

        string CreateUserHostName(HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress != null
               ? request.HttpContext.Connection.RemoteIpAddress.ToString()
               : null;
        }

        string CreateUserHostAddress(HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress != null
               ? request.HttpContext.Connection.RemoteIpAddress.ToString()
               : null;
        }

        string CreateUserLanguage(HttpRequest request)
        {
            return request.Headers.TryGetValue("Accept-Language", out var value)
                ? value.FirstOrDefault()
                : null;
        }

        string CreateUserAgent(HttpRequest request)
        {
            return request.Headers.TryGetValue("User-Agent", out var value)
                ? value.FirstOrDefault()
                : null;
        }

        private string CreateFormStringRaw(HttpRequest request)
        {
            if (!AspNetCoreHttpContext.Current.Request.HasFormContentType) return string.Empty;
            if (request.Form.Count == 1 && string.IsNullOrEmpty(request.Form.Keys.First())) return request.Form.First().Value;
            return string.Join('&', request.Form?.Select(data => $"{HttpUtility.UrlEncode(data.Key)}={HttpUtility.UrlEncode(data.Value)}"));
        }


        public static string CreateApplicationPath()
        {
            var path = AspNetCoreHttpContext.Current.Request.PathBase.Value;
            return path.EndsWith("/")
                ? path
                : path + "/";
        }

        public static string CreateAbsoluteUri()
        {
            var request = AspNetCoreHttpContext.Current.Request;
            if (string.IsNullOrEmpty(request.Host.Host)) return string.Empty;
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            if (request.Host.Port.HasValue) uriBuilder.Port = request.Host.Port.Value;
            uriBuilder.Path = request.PathBase.Value + request.Path.Value;
            uriBuilder.Query = request.QueryString.ToString();
            var uri = uriBuilder.Uri.AbsoluteUri;
            return uri;
        }

        public static string CreateAbsolutePath()
        {
            return AspNetCoreHttpContext.Current.Request.PathBase.Value
                + AspNetCoreHttpContext.Current.Request.Path.Value;
        }

        public static string CreateServer()
        {
            return
                AspNetCoreHttpContext.Current.Request.Scheme + "://" +
                AspNetCoreHttpContext.Current.Request.Host;
        }

        public static void SetSessionData(string name, object data)
        {
            AspNetCoreHttpContext.Current.Session.Set(name, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        }

        public static T GetSessionData<T>(string name)
        {
            if (!AspNetCoreHttpContext.Current.Session.TryGetValue(name, out var bytes)) return default(T);
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public override Context CreateContext() { return new ContextImplement(); }
        public override Context CreateContext(int tenantId) { return new ContextImplement(tenantId: tenantId); }
        public override Context CreateContext(int tenantId, int userId, int deptId) { return new ContextImplement(tenantId: tenantId, userId: userId, deptId: deptId); }
        public override Context CreateContext(int tenantId, string language) { return new ContextImplement(tenantId: tenantId, language: language); }
        public override Context CreateContext(int tenantId, int userId, string language) { return new ContextImplement(tenantId: tenantId, userId: userId, language: language); }
        public override Context CreateContext(bool request, bool sessionStatus, bool sessionData, bool user) { return new ContextImplement(request: request, sessionStatus: sessionStatus, sessionData: sessionData, user: user); }

        public static void Init() { Context.SetFactory(item => new ContextImplement(item: item)); }

        public override string VirtualPathToAbsolute(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath)) return virtualPath;
            if (!virtualPath.StartsWith('~')) return virtualPath;
            return virtualPath.Replace("~", AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current.Request.PathBase);
        }

        public override void FormsAuthenticationSignIn(string userName, bool createPersistentCookie)
        {
            var userClaims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "Forms"));
            AspNetCoreHttpContext.Current.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public override void FormsAuthenticationSignOut()
        {
            AspNetCoreHttpContext.Current.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            AspNetCoreHttpContext.Current.Session.Clear();
        }

        public override void SessionAbandon() { AspNetCoreHttpContext.Current.Session.Clear(); }

        public override void FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie()
        {
            var responceCookies = AspNetCoreHttpContext.Current.Response.Cookies;
            foreach (var requestCookie in AspNetCoreHttpContext.Current.Request.Cookies)
                responceCookies.Delete(requestCookie.Key);
        }

        public override bool AuthenticationsWindows()
        {
            return AspNetCoreHttpContext.Current.User.Identity?.GetType().Name.Contains("Windows") ?? false;
        }

        private string GetUserHostAddress(ConnectionInfo request)
        {
            return request?.RemoteIpAddress?.ToString();
        }

        private static readonly Lazy<ISqlObjectFactory> _sqlObjectFactory = new Lazy<ISqlObjectFactory>(() =>
        {
            return RdsFactory.Create(Parameters.Rds.Dbms);
        });

        protected override ISqlObjectFactory GetSqlObjectFactory()
        {
            return _sqlObjectFactory.Value;
        }
    }
}
