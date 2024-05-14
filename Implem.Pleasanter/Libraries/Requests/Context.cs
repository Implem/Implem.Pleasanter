using AspNetCoreCurrentRequestContext;
using Implem.DefinitionAccessor;
using Implem.Factory;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
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
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Context : ISqlObjectFactory
    {
        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public StringBuilder LogBuilder { get; set; } = new StringBuilder();
        public int SysLogsStatus { get; set; } = 200;
        public string SysLogsDescription { get; set; }
        public ExpandoObject UserData { get; set; } = new ExpandoObject();
        public ResponseCollection ResponseCollection { get; set; } = new ResponseCollection();
        public List<Message> Messages { get; set; } = new List<Message>();
        public ErrorData ErrorData { get; set; } = new ErrorData(type: Error.Types.None);
        public RedirectData RedirectData { get; set; } = new RedirectData();
        public bool InvalidJsonData { get; set; }
        public bool Authenticated { get; set; }
        public bool SwitchUser { get; set; }
        public bool SwitchTenant { get; set; }
        public string SessionGuid { get; set; } = Strings.NewGuid();
        public Dictionary<string, string> SessionData { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> UserSessionData { get; set; } = new Dictionary<string, string>();
        public bool Publish { get; set; }
        public List<string> ControlledOrder { get; set; }
        public QueryStrings QueryStrings { get; set; } = new QueryStrings();
        public Forms Forms { get; set; } = new Forms();
        public string FormStringRaw { get; set; }
        public string FormString { get; set; }
        public List<PostedFile> PostedFiles { get; set; } = new List<PostedFile>();
        public bool HasRoute { get; set; }
        public string HttpMethod { get; set; }
        public bool Ajax { get; set; }
        public bool Mobile { get; set; }
        public bool Responsive { get; set; }
        public Dictionary<string, string> RouteData { get; set; } = new Dictionary<string, string>();
        public string ApplicationPath { get; set; }
        public string AbsoluteUri { get; set; }
        public string AbsolutePath { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string Query { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Page { get; set; }
        public string Server { get; set; }
        public int TenantId { get; set; }
        public int TargetTenantId { get; set; }
        public long SiteId { get; set; }
        public long Id { get; set; }
        public Dictionary<long, Permissions.Types> PermissionHash { get; set; }
        public List<int> Groups { get; set; }
        public string Guid { get; set; }
        public TenantModel.LogoTypes LogoType { get; set; }
        public string TenantTitle { get; set; }
        public string SiteTitle { get; set; }
        public string RecordTitle { get; set; }
        public bool DisableAllUsersPermission { get; set; }
        public bool DisableApi { get; set; }
        public bool DisableStartGuide { get; set; }
        public string HtmlTitleTop { get; set; }
        public string HtmlTitleSite { get; set; }
        public string HtmlTitleRecord { get; set; }
        public string TopStyle { get; set; }
        public string TopScript { get; set; }
        public string TenantTheme { get; set; }
        public int DeptId { get; set; }
        public int UserId { get; set; }
        public string LoginId { get; set; }
        public Dept Dept { get; set; }
        public User User { get; set; }
        public string UserHostName { get; set; }
        public string UserHostAddress { get; set; }
        public string UserAgent { get; set; }
        public string Language { get; set; } = Parameters.Service.DefaultLanguage;
        public string UserTheme { get; set; } = Parameters.User.Theme;
        public bool Developer { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; } = Environments.TimeZoneInfoDefault;
        public UserSettings UserSettings { get; set; }
        public bool HasPrivilege { get; set; }
        public ContractSettings ContractSettings { get; set; } = new ContractSettings();
        public bool Api { get; set; }
        public decimal ApiVersion { get; set; } = Parameters.Api.Version;
        public string ApiRequestBody { get; set; }
        public string ApiKey { get; set; }
        public string RequestDataString { get => !string.IsNullOrEmpty(ApiRequestBody) ? ApiRequestBody : FormString; }
        public string ContentType { get; set; }
        public long ServerScriptDepth { get; set; } = 0;
        public bool ServerScriptDisabled { get; set; }
        public bool IsNew { get; set; }
        public List<ParameterAccessor.Parts.ExtendedField> ExtendedFields { get; set; }
        public string AuthenticationType { get; set; }
        public bool? IsAuthenticated { get; set; }
        public IEnumerable<Claim> UserClaims { get; set; }
        public string IdentityType { get; set; }
        public bool Request { get; set; }
        public bool BackgroundServerScript { get; set; }

        public Context(
            bool request = true,
            bool sessionStatus = true,
            bool sessionData = true,
            bool user = true,
            bool item = true,
            bool setPermissions = true,
            string apiRequestBody = null,
            string contentType = null,
            bool api = false)
        {
            Request = request;
            Set(
                request: request,
                sessionStatus: sessionStatus,
                setData: sessionData,
                user: user,
                item: item,
                setPermissions: setPermissions,
                apiRequestBody: apiRequestBody,
                contentType: contentType);
            if (ApiRequestBody != null)
            {
                SiteInfo.Reflesh(context: this);
            }
            Api = api;
        }

        public Context(
            int tenantId,
            int deptId = 0,
            int userId = 0,
            string language = null,
            bool request = true,
            bool setAuthenticated = false,
            Context context = null)
        {
            if (context?.Request != false)
            {
                if (request)
                {
                    Request = request;
                    SetRequests();
                }
            }
            else
            {
                CopyRequests(context: context);
            }
            TenantId = tenantId;
            DeptId = deptId;
            UserId = userId;
            Dept = SiteInfo.Dept(
                tenantId: TenantId,
                deptId: DeptId);
            User = SiteInfo.User(
                context: this,
                userId: UserId);
            if (setAuthenticated)
            {
                Authenticated = !User.Anonymous() && UserId > 0;
            }
            Language = language ?? Language;
            UserHostAddress = HasRoute
                ? GetUserHostAddress()
                : null;
            HasPrivilege = Permissions.PrivilegedUsers(User.LoginId);
            SetTenantProperties();
            if (context?.Request != false) SetPublish();
            SetPermissions();
            SetTenantCaches();
        }

        public Context(ICollection<IFormFile> files, string apiRequestBody = null, string contentType = null, bool api = false)
        {
            Set(apiRequestBody: apiRequestBody, contentType: contentType);
            SetPostedFiles(files: files);
            Api = api;
        }

        public void Set(
            bool request = true,
            bool sessionStatus = true,
            bool setData = true,
            bool user = true,
            bool item = true,
            bool setPermissions = true,
            string apiRequestBody = null,
            string contentType = null)
        {
            ApiRequestBody = apiRequestBody;
            ContentType = contentType;
            if (request) SetRequests();
            if (sessionStatus) SetSessionGuid();
            if (item) SetItemProperties();
            if (user) SetUserProperties(sessionStatus, setData);
            if (item) SetSwitchTenant(sessionStatus, setData);
            SetTenantProperties();
            if (request) SetPublish();
            if (request && setPermissions) SetPermissions();
            SetTenantCaches();
        }

        private void SetRequests()
        {
            HasRoute = AspNetCoreHttpContext.Current != null;
            if (HasRoute)
            {
                var user = AspNetCoreHttpContext.Current.User;
                if (user != null)
                {
                    LoginId = user.Identity?.Name;
                    AuthenticationType = user.Identity?.AuthenticationType;
                    IsAuthenticated = user.Identity?.IsAuthenticated;
                    UserClaims = user.Claims;
                    IdentityType = user.Identity?.GetType().Name;
                }
                var request = AspNetCoreHttpContext.Current.Request;
                FormStringRaw = CreateFormStringRaw(AspNetCoreHttpContext.Current.Request);
                FormString = HttpUtility.UrlDecode(FormStringRaw, Encoding.UTF8);
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
                UserHostName = AspNetCoreHttpContext.Current?.Connection?.RemoteIpAddress?.ToString();
                UserHostAddress = GetUserHostAddress();
                UserAgent = CreateUserAgent(AspNetCoreHttpContext.Current.Request);
            }
        }

        private void CopyRequests(Context context)
        {
            if (context != null)
            {
                LoginId = context.LoginId;
                AuthenticationType = context.AuthenticationType;
                IsAuthenticated = context.IsAuthenticated;
                UserClaims = context.UserClaims;
                FormStringRaw = context.FormStringRaw;
                FormString = context.FormString;
                HttpMethod = context.HttpMethod;
                Ajax = context.Ajax;
                Mobile = context.Mobile;
                RouteData = context.RouteData;
                Server = context.Server;
                ApplicationPath = context.ApplicationPath;
                AbsoluteUri = context.AbsoluteUri;
                AbsolutePath = context.AbsolutePath;
                Url = context.Url;
                UrlReferrer = context.UrlReferrer;
                Query = context.Query;
                Controller = context.Controller;
                Action = context.Action;
                Id = context.Id;
                Guid = context.Guid;
                UserHostName = context.UserHostName;
                UserHostAddress = context.UserHostAddress;
                UserAgent = context.UserAgent;
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

        public void SetItemProperties()
        {
            if (HasRoute)
            {
                switch (Controller)
                {
                    case "binaries":
                    case "items":
                    case "publishes":
                        if (Id > 0)
                        {
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
                                            TargetTenantId = dataRow.Int("TenantId");
                                        });
                        }
                        Page = Controller + "/"
                            + SiteId
                            + (TrashboxActions()
                                ? "/trashbox"
                                : string.Empty);
                        ExtendedFields = Parameters.ExtendedFields
                            .ExtensionWhere<ParameterAccessor.Parts.ExtendedField>(context: this)
                            .ToList();
                        break;
                    case "groups":
                        Page = Controller;
                        ExtendedFields = GroupUtilities.GetExtendedFields(context: this);
                        break;
                    case "users":
                        Page = Id > 0
                            ? $"{Controller}/{Id}"
                            : Controller;
                        break;
                    default:
                        Page = Controller;
                        break;
                }
            }
        }

        private void SetSwitchTenant(bool sessionStatus, bool setData)
        {
            Extension.SwichTenant(context: this);
            if (this.SwitchTenant) SetUserProperties(sessionStatus: false, setData: false);
        }

        private void SetUserProperties(bool sessionStatus, bool setData)
        {
            if (HasRoute)
            {
                if (setData) SetData();
                var api = RequestDataString.Deserialize<Api>();
                SetApiVersion(api: api);
                if (api?.ApiKey.IsNullOrEmpty() == false)
                {
                    ApiKey = api.ApiKey;
                    SetUser(userModel: GetUser(where: Rds.UsersWhere()
                        .ApiKey(ApiKey)));
                }
                else if (!LoginId.IsNullOrEmpty())
                {
                    var loginId = Strings.CoalesceEmpty(
                        Permissions.PrivilegedUsers(
                            loginId: AspNetCoreHttpContext.Current?.User?.Identity.Name)
                                ? SessionData.Get("SwitchLoginId")
                                : null,
                            SessionData.Get("SwitchTenantId") != null
                                ? SessionData.Get("SwitchLoginId")
                                : null,
                        LoginId);
                    SetUser(userModel: GetUser(where: Rds.UsersWhere().LoginId(
                        value: Sqls.EscapeValue(loginId),
                        _operator: Sqls.LikeWithEscape)));
                }
                else
                {
                    if (sessionStatus) Language = SessionLanguage();
                }
                UserSessionData = SessionUtilities.Get(
                    context: this,
                    includeUserArea: Controller == "sessions",
                    sessionGuid: "@" + UserId);
            }
        }

        private void SetApiVersion(Api api)
        {
            
            if (Parameters.Api.Compatibility_1_3_12)
            {
                if (api?.ApiKey.IsNullOrEmpty() == false)
                {
                    ApiVersion = api.ApiVersion;
                }
            }
            else
            {
                ApiVersion = api?.ApiVersion ?? ApiVersion;
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

        public void SetUserProperties(UserModel userModel, bool noHttpContext = false)
        {
            SetUser(
                userModel: userModel,
                noHttpContext: noHttpContext);
            SetPermissions();
        }

        private void SetUser(UserModel userModel, bool noHttpContext = false)
        {
            if (userModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                SwitchUser = SessionData.Get("SwitchLoginId") != null;
                Authenticated = true;
                TenantId = userModel.TenantId;
                DeptId = userModel.DeptId;
                UserId = userModel.UserId;
                Dept = SiteInfo.Dept(tenantId: TenantId, deptId: DeptId);
                User = SiteInfo.User(context: this, userId: UserId);
                Language = userModel.Language;
                UserTheme = userModel.Theme;
                UserHostAddress = noHttpContext
                    ? string.Empty
                    : GetUserHostAddress();
                Developer = userModel.Developer;
                TimeZoneInfo = userModel.TimeZoneInfo;
                UserSettings = userModel.UserSettings;
                HasPrivilege = Permissions.PrivilegedUsers(userModel.LoginId);
            }
        }

        public void SetTenantProperties(bool force = false)
        {
            if (HasRoute || force)
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
                            .DisableApi()
                            .DisableStartGuide()
                            .HtmlTitleTop()
                            .HtmlTitleSite()
                            .HtmlTitleRecord()
                            .TopStyle()
                            .TopScript()
                            .Theme(),
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
                    DisableApi = dataRow.Bool("DisableApi");
                    DisableStartGuide = dataRow.Bool("DisableStartGuide");
                    HtmlTitleTop = dataRow.String("HtmlTitleTop");
                    HtmlTitleSite = dataRow.String("HtmlTitleSite");
                    HtmlTitleRecord = dataRow.String("HtmlTitleRecord");
                    TopStyle = dataRow.String("TopStyle");
                    TopScript = dataRow.String("TopScript");
                    TenantTheme = dataRow.String("Theme");
                }
            }
        }

        private void SetData()
        {
            SessionData = SessionUtilities.Get(
                context: this,
                includeUserArea: Controller == "sessions");
            var responsive = SessionData.Get("Responsive");
            Responsive = Mobile
                && (responsive.IsNullOrEmpty()
                    || responsive.ToBool())
                        ? true
                        : false;
            SessionUtilities.DeleteOldSessions(context: this);
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
            IsNew = Forms.Bool("IsNew") || Action == "new";
            var controlId = Forms.ControlId();
            if (!controlId.IsNullOrEmpty())
            {
                ControlledOrder = Forms.List("ControlledOrder");
                ControlledOrder.RemoveAll(o => o == controlId);
                ControlledOrder.Insert(0, controlId);
            }
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
                    ContentType = file.ContentType,
                    ContentRange = file.Length > 0
                        ? new System.Net.Http.Headers.ContentRangeHeaderValue(
                            0,
                            file.Length - 1,
                            file.Length)
                        : new System.Net.Http.Headers.ContentRangeHeaderValue(0, 0, 0),
                    InputStream = file.OpenReadStream()
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

        public void SetPublish()
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
                                        where: Guid.IsNullOrEmpty()
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

        public Dictionary<string, string> GetRouteData()
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

        private static string HttpAcceptLanguage()
        {
            return AspNetCoreHttpContext.Current?.Request?.GetTypedHeaders()?.AcceptLanguage?.FirstOrDefault()?.Value.ToString();
        }

        public void SetPermissions()
        {
            PermissionHash = Permissions.Get(context: this);
            Groups = PermissionUtilities.Groups(context: this);
        }

        public void SetTenantCaches()
        {
            if (TenantId != 0 && !SiteInfo.TenantCaches.ContainsKey(TenantId))
            {
                try
                {
                    var temp = SiteInfo.TenantCaches.ToDictionary(o => o.Key, o => o.Value);
                    temp.Add(TenantId, new TenantCache(context: this));
                    SiteInfo.TenantCaches = temp;
                    SiteInfo.Reflesh(context: this);
                }
                catch (Exception)
                {
                }
            }
        }

        private string GetUserHostAddress()
        {
            var xFoForwardedFor = new Microsoft.Extensions.Primitives.StringValues();
            var address = AspNetCoreHttpContext.Current?.Request?.Headers?.TryGetValue("X-Forwarded-For", out xFoForwardedFor) == true
                ? xFoForwardedFor.FirstOrDefault()
                : null;
            if (address == null)
            {
                return AspNetCoreHttpContext.Current?.Connection?.RemoteIpAddress?.ToString();
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

        public string RequestData(string name, bool either = false)
        {
            return either
                ? Strings.CoalesceEmpty(
                    QueryStrings.Data(name),
                    Forms.Data(name))
                : HttpMethod == "GET"
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

        public bool SiteTop()
        {
            return SiteId == 0 && Id == 0 && Controller == "items" && Action == "index";
        }

        public string GetLog()
        {
            return LogBuilder?.ToString();
        }

        public Column ExtendedFieldColumn(
            SiteSettings ss,
            string columnName,
            string extendedFieldType)
        {
            var viewFilter = ExtendedFields?.FirstOrDefault(o =>
                o.Name == columnName
                && o.FieldType == extendedFieldType);
            if (viewFilter != null)
            {
                return ExtendedFieldColumn(
                    ss: ss,
                    viewFilter: viewFilter);
            }
            return null;
        }

        public List<Column> ExtendedFieldColumns(
            SiteSettings ss,
            string extendedFieldType)
        {
            var extendedFieldColumns = ExtendedFields
                ?.Where(viewFilter => viewFilter.FieldType == extendedFieldType)
                .Select(viewFilter => ExtendedFieldColumn(
                    ss: ss,
                    viewFilter: viewFilter))
                .ToList()
                    ?? new List<Column>();
            return extendedFieldColumns;
        }

        private Column ExtendedFieldColumn(
            SiteSettings ss,
            ParameterAccessor.Parts.ExtendedField viewFilter)
        {
            var column = new Column()
            {
                SiteSettings = ss,
                ColumnName = viewFilter.Name,
                TypeName = viewFilter.TypeName,
                LabelText = viewFilter.LabelText,
                GridLabelText = viewFilter.LabelText,
                ChoicesText = viewFilter.ChoicesText,
                DefaultInput = viewFilter.DefaultInput,
                EditorFormat = viewFilter.EditorFormat,
                ControlType = viewFilter.ControlType,
                ValidateRequired = viewFilter.ValidateRequired,
                ValidateNumber = viewFilter.ValidateNumber,
                ValidateDate = viewFilter.ValidateDate,
                ValidateEmail = viewFilter.ValidateEmail,
                MaxLength = viewFilter.MaxLength,
                ValidateEqualTo = viewFilter.ValidateEqualTo,
                ValidateMaxLength = viewFilter.ValidateMaxLength,
                DecimalPlaces = viewFilter.DecimalPlaces,
                Nullable = viewFilter.Nullable,
                Unit = viewFilter.Unit,
                Min = viewFilter.Min,
                Max = viewFilter.Max,
                Step = viewFilter.Step,
                AutoPostBack = viewFilter.AutoPostBack,
                FieldCss = viewFilter.FieldCss ?? (viewFilter.FieldType == "ViewExtensions" ? "field-auto-thin" : null),
                CheckFilterControlType = (ColumnUtilities.CheckFilterControlTypes)viewFilter.CheckFilterControlType,
                DateTimeStep = viewFilter.DateTimeStep,
                Size = ss.ColumnDefinitionHash.Get("NumA")?.Size ?? string.Empty,
                ControlCss = viewFilter.ControlCss,
                After = viewFilter.After,
                SqlParam = viewFilter.SqlParam
            };
            if (column.HasChoices())
            {
                column.SetChoiceHash(
                    context: this,
                    siteId: SiteId);
            }
            return column;
        }
        
        public ISqlCommand CreateSqlCommand()
        {
            return GetSqlObjectFactory().CreateSqlCommand();
        }

        public ISqlParameter CreateSqlParameter()
        {
            return GetSqlObjectFactory().CreateSqlParameter();
        }

        public ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand)
        {
            return GetSqlObjectFactory().CreateSqlDataAdapter(sqlCommand);
        }

        public ISqlParameter CreateSqlParameter(string name, object value)
        {
            return GetSqlObjectFactory().CreateSqlParameter(name, value);
        }

        public ISqlConnection CreateSqlConnection(string connectionString)
        {
            return GetSqlObjectFactory().CreateSqlConnection(connectionString);
        }

        public ISqlConnectionStringBuilder CreateSqlConnectionStringBuilder(string connectionString)
        {
            return GetSqlObjectFactory().CreateSqlConnectionStringBuilder(connectionString);
        }

        public ISqls Sqls
        {
            get
            {
                return GetSqlObjectFactory().Sqls;
            }
        }

        public ISqlCommandText SqlCommandText
        {
            get
            {
                return GetSqlObjectFactory().SqlCommandText;
            }
        }

        public ISqlResult SqlResult
        {
            get
            {
                return GetSqlObjectFactory().SqlResult;
            }
        }

        public ISqlErrors SqlErrors
        {
            get
            {
                return GetSqlObjectFactory().SqlErrors;
            }
        }

        public ISqlDataType SqlDataType
        {
            get
            {
                return GetSqlObjectFactory().SqlDataType;
            }
        }

        public ISqlDefinitionSetting SqlDefinitionSetting
        {
            get
            {
                return GetSqlObjectFactory().SqlDefinitionSetting;
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
            Microsoft.Extensions.Primitives.StringValues value = new Microsoft.Extensions.Primitives.StringValues();
            return request?.Headers.TryGetValue("User-Agent", out value) == true ? value.Any(v => v.Contains("Mobile")) : false;
        }

        string CreateUrl(HttpRequest request)
        {
            var basePath = request.PathBase.HasValue
                ? $"/{request.PathBase.Value.Trim('/')}"
                : string.Empty;
            var url = request != null
                ? $"{request.Scheme}://{request.Host.Value}{basePath}{request.Path.Value}{request.QueryString.Value}"
                : null;
            return url;
        }

        public string CreateUrlReferrer(HttpRequest request)
        {
            return request.Headers.TryGetValue("Referer", out var value)
                ? value.FirstOrDefault().ToString()
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

        public static void Init()
        {
            SetFactory(item => new Context(item: item));
        }

        static Func<bool, Context> _factory;

        protected static void SetFactory(Func<bool, Context> factory)
        {
            _factory = factory;
        }

        public string VirtualPathToAbsolute(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath)) return virtualPath;
            if (!virtualPath.StartsWith('~')) return virtualPath;
            return virtualPath.Replace("~", (Request
                ? AspNetCoreHttpContext.Current.Request.PathBase.ToString()
                : string.Empty));
        }

        public void FormsAuthenticationSignIn(string userName, bool createPersistentCookie)
        {
            var userClaims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "Forms"));
            var properties = new AuthenticationProperties() { IsPersistent = createPersistentCookie };
            AspNetCoreHttpContext.Current.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: principal,
                properties: properties);
        }

        public void FormsAuthenticationSignOut()
        {
            AspNetCoreHttpContext.Current.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            AspNetCoreHttpContext.Current.Session.Clear();
        }

        public void SessionAbandon() { AspNetCoreHttpContext.Current.Session.Clear(); }

        public void FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie()
        {
            var setCookieNames = AspNetCoreHttpContext.Current.Response.Headers.SetCookie
                .Select(o => o.Split_1st(';').Split_1st('=').Trim());
            var responceCookies = AspNetCoreHttpContext.Current.Response.Cookies;
            foreach (var requestCookie in AspNetCoreHttpContext.Current.Request.Cookies
                .Where(o => !setCookieNames.Contains(o.Key) && !o.Key.StartsWith("Pleasanter_")))
            {
                responceCookies.Delete(requestCookie.Key);
            }
        }

        public bool AuthenticationsWindows()
        {
            if (Parameters.Authentication.Provider == "Windows")
            {
                return true;
            }
            return IdentityType?.Contains("Windows") ?? false;
        }

        private static readonly Lazy<ISqlObjectFactory> _sqlObjectFactory = new Lazy<ISqlObjectFactory>(() =>
        {
            return RdsFactory.Create(Parameters.Rds.Dbms);
        });

        protected ISqlObjectFactory GetSqlObjectFactory()
        {
            return _sqlObjectFactory.Value;
        }
        
        public CultureInfo CultureInfoCurrency(string language)
        {
            switch (language)
            {
                case "en":
                    return new CultureInfo("en-US");
                case "zh":
                    return new CultureInfo("zh-CN");
                case "ja":
                    return new CultureInfo("ja-JP");
                case "de":
                    return new CultureInfo("de-DE");
                case "ko":
                    return new CultureInfo("ko-KR");
                case "es":
                    return new CultureInfo("es-ES");
                case "vn":
                    return new CultureInfo("vi-VN");
                default:
                    return new CultureInfo(language);
            }
        }
        
        public string Token()
        {
            return Request
                ? AspNetCoreHttpContext.Current?.Request?.Cookies[".AspNetCore.Session"]?.Sha512Cng()
                : string.Empty;
        }

        public string Theme()
        {
            var theme = Strings.CoalesceEmpty(
                UserTheme,
                TenantTheme,
                Parameters.User.Theme,
                "cerulean");
            return theme;
        }

        public decimal ThemeVersion()
        {
            if (Mobile) { return 1.0M; }
            switch (Theme())
            {
                case "cerulean":
                case "green-tea":
                case "mandarin":
                case "midnight":
                    return 2.0M;
                default:
                    return 1.0M;
            }
        }

        public decimal ThemeVersionForCss()
        {
            switch (Theme())
            {
                case "cerulean":
                case "green-tea":
                case "mandarin":
                case "midnight":
                    return 2.0M;
                default:
                    return 1.0M;
            }
        }

        public bool ThemeVersion1_0()
        {
            return ThemeVersion() == 1.0M;
        }

        public bool ThemeVersionOver2_0()
        {
            return ThemeVersion() >= 2.0M;
        }
    }
}
