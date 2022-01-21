using System;
using System.Collections.Generic;
using Implem.ParameterAccessor.Parts;
using Implem.Libraries.Classes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System.Text;
using System.Dynamic;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.DataTypes;
using System.Security.Claims;
using System.Globalization;
using Implem.IRds;
namespace Implem.TestAutomation
{
    class ContextForAutoTest : Context
    {
        public override Stopwatch Stopwatch { get; set; }
        public override StringBuilder LogBuilder { get; set; }
        public override ExpandoObject UserData { get; set; }
        public override List<Message> Messages { get; set; }
        public override ErrorData ErrorData { get; set; }
        public override bool InvalidJsonData { get; set; }
        public override bool Authenticated { get; set; }
        public override bool SwitchUser { get; set; }
        public override string SessionGuid { get; set; }
        public override Dictionary<string, string> SessionData { get; set; }
        public override Dictionary<string, string> UserSessionData { get; set; }
        public override bool Publish { get; set; }
        public override QueryStrings QueryStrings { get; set; }
        public override Forms Forms { get; set; }
        public override string FormStringRaw { get; set; }
        public override string FormString { get; set; }
        public override List<PostedFile> PostedFiles { get; set; }
        public override bool HasRoute { get; set; }
        public override string HttpMethod { get; set; }
        public override bool Ajax { get; set; }
        public override bool Mobile { get; set; }
        public override bool Responsive { get; set; }
        public override Dictionary<string, string> RouteData { get; set; }
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
        public override Dictionary<long, Pleasanter.Libraries.Security.Permissions.Types> PermissionHash { get; set; }
        public override string Guid { get; set; }
        public override Pleasanter.Models.TenantModel.LogoTypes LogoType { get; set; }
        public override string TenantTitle { get; set; }
        public override string SiteTitle { get; set; }
        public override string RecordTitle { get; set; }
        public override bool DisableAllUsersPermission { get; set; }
        public override string HtmlTitleTop { get; set; }
        public override string HtmlTitleSite { get; set; }
        public override string HtmlTitleRecord { get; set; }
        public override string TopStyle { get; set; }
        public override string TopScript { get; set; }
        public override int DeptId { get; set; }
        public override int UserId { get; set; }
        public override string LoginId { get; set; }
        public override Dept Dept { get; set; }
        public override Pleasanter.Libraries.DataTypes.User User { get; set; }
        public override string UserHostName { get; set; }
        public override string UserHostAddress { get; set; }
        public override string UserAgent { get; set; }
        public override string Language { get; set; }
        public override string Theme { get; set; }
        public override bool Developer { get; set; }
        public override TimeZoneInfo TimeZoneInfo { get; set; }
        public override Pleasanter.Libraries.Settings.UserSettings UserSettings { get; set; }
        public override bool HasPrivilege { get; set; }
        public override Pleasanter.Libraries.Settings.ContractSettings ContractSettings { get; set; }
        public override decimal ApiVersion { get; set; }
        public override string ApiRequestBody { get; set; }
        public override string ApiKey { get; set; }
        public override string RequestDataString => throw new NotImplementedException();
        public override string ContentType { get; set; }
        public override List<ExtendedField> ExtendedFields { get; set; }
        public override string AuthenticationType => throw new NotImplementedException();
        public override bool? IsAuthenticated => throw new NotImplementedException();
        public override IEnumerable<Claim> UserClaims => throw new NotImplementedException();

        public override bool AuthenticationsWindows()
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext()
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(string apiRequestBody)
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(int tenantId)
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(int tenantId, int userId, int deptId)
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(int tenantId, string language)
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(int tenantId, int userId, string language)
        {
            throw new NotImplementedException();
        }

        public override Context CreateContext(bool request, bool sessionStatus, bool sessionData, bool user)
        {
            throw new NotImplementedException();
        }

        public override CultureInfo CultureInfo()
        {
            throw new NotImplementedException();
        }

        public override CultureInfo CultureInfoCurrency(string language)
        {
            throw new NotImplementedException();
        }

        public override Pleasanter.Libraries.Settings.Column ExtendedFieldColumn(Pleasanter.Libraries.Settings.SiteSettings ss, string columnName, string extendedFieldType)
        {
            throw new NotImplementedException();
        }

        public override List<Pleasanter.Libraries.Settings.Column> ExtendedFieldColumns(Pleasanter.Libraries.Settings.SiteSettings ss, string extendedFieldType)
        {
            throw new NotImplementedException();
        }

        public override void FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie()
        {
            throw new NotImplementedException();
        }

        public override void FormsAuthenticationSignIn(string userName, bool createPersistentCookie)
        {
            throw new NotImplementedException();
        }

        public override void FormsAuthenticationSignOut()
        {
            throw new NotImplementedException();
        }

        public override string GetLog()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetRouteData()
        {
            throw new NotImplementedException();
        }

        public override Message Message()
        {
            throw new NotImplementedException();
        }

        public override RdsUser RdsUser()
        {
            throw new NotImplementedException();
        }

        public override void SessionAbandon()
        {
            throw new NotImplementedException();
        }

        public override double SessionAge()
        {
            throw new NotImplementedException();
        }

        public override double SessionRequestInterval()
        {
            throw new NotImplementedException();
        }

        public override void Set(bool request = true, bool sessionStatus = true, bool setData = true, bool user = true, bool item = true, bool setPermissions = true, string apiRequestBody = null, string contentType = null)
        {
            throw new NotImplementedException();
        }

        public override void SetTenantCaches()
        {
            throw new NotImplementedException();
        }

        public override bool SiteTop()
        {
            throw new NotImplementedException();
        }

        public override string Token()
        {
            throw new NotImplementedException();
        }

        public override string VirtualPathToAbsolute(string virtualPath)
        {
            throw new NotImplementedException();
        }

        protected override ISqlObjectFactory GetSqlObjectFactory()
        {
            throw new NotImplementedException();
        }
    }
}
