using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace Implem.Pleasanter.Libraries.Requests
{
    public abstract class Context : ISqlObjectFactory
    {
        public abstract bool Authenticated { get; set; }
        public abstract bool SwitchUser { get; set; }
        public abstract string SessionGuid { get; set; }
        public abstract Dictionary<string, string> SessionData { get; set; }
        public abstract bool Publish { get; set; }
        public abstract QueryStrings QueryStrings { get; set; }
        public abstract Forms Forms { get; set; }
        public abstract string FormStringRaw { get; set; }
        public abstract string FormString { get; set; }
        public abstract List<PostedFile> PostedFiles { get; set; }
        public abstract bool HasRoute { get; set; }
        public abstract string HttpMethod { get; set; }
        public abstract bool Ajax { get; set; }
        public abstract bool Mobile { get; set; }
        public abstract Dictionary<string, string> RouteData { get; set; }
        public abstract string ApplicationPath { get; set; }
        public abstract string AbsoluteUri { get; set; }
        public abstract string AbsolutePath { get; set; }
        public abstract string Url { get; set; }
        public abstract string UrlReferrer { get; set; }
        public abstract string Query { get; set; }
        public abstract string Controller { get; set; }
        public abstract string Action { get; set; }
        public abstract string Page { get; set; }
        public abstract string Server { get; set; }
        public abstract int TenantId { get; set; }
        public abstract long SiteId { get; set; }
        public abstract long Id { get; set; }
        public abstract Dictionary<long, Permissions.Types> PermissionHash { get; set; }
        public abstract string Guid { get; set; }
        public abstract TenantModel.LogoTypes LogoType { get; set; }
        public abstract string TenantTitle { get; set; }
        public abstract string SiteTitle { get; set; }
        public abstract string RecordTitle { get; set; }
        public abstract bool DisableAllUsersPermission { get; set; }
        public abstract string HtmlTitleTop { get; set; }
        public abstract string HtmlTitleSite { get; set; }
        public abstract string HtmlTitleRecord { get; set; }
        public abstract int DeptId { get; set; }
        public abstract int UserId { get; set; }
        public abstract string LoginId { get; set; }
        public abstract Dept Dept { get; set; }
        public abstract User User { get; set; }
        public abstract string UserHostName { get; set; }
        public abstract string UserHostAddress { get; set; }
        public abstract string UserAgent { get; set; }
        public abstract string Language { get; set; }
        public abstract bool Developer { get; set; }
        public abstract TimeZoneInfo TimeZoneInfo { get; set; }
        public abstract UserSettings UserSettings { get; set; }
        public abstract bool HasPrivilege { get; set; }
        public abstract ContractSettings ContractSettings { get; set; }
        public abstract decimal ApiVersion { get; set; }
        public abstract string ApiRequestBody { get; set; }
        public abstract string RequestDataString { get; }

        public abstract string AuthenticationType { get; }
        public abstract bool? IsAuthenticated { get; }

        public abstract void Set(bool request = true, bool sessionStatus = true, bool setData = true, bool user = true, bool item = true, string apiRequestBody = null);

        public abstract RdsUser RdsUser();
        public abstract CultureInfo CultureInfo();
        public abstract Message Message();
        public abstract double SessionAge();
        public abstract double SessionRequestInterval();
        public abstract Dictionary<string, string> GetRouteData();

        public abstract void SetTenantCaches();

        static Func<bool, Context> _factory;
        protected static void SetFactory(Func<bool, Context> factory)
        {
            _factory = factory;
        }

        public static Context CreateContext(bool item)
        {
            return _factory(item);
        }

        public abstract Context CreateContext();
        public abstract Context CreateContext(int tenantId);
        public abstract Context CreateContext(int tenantId, int userId, int deptId);
        public abstract Context CreateContext(int tenantId, string language);
        public abstract Context CreateContext(int tenantId, int userId, string language);
        public abstract Context CreateContext(bool request, bool sessionStatus, bool sessionData, bool user);

        public abstract string VirtualPathToAbsolute(string virtualPath);

        public abstract void FormsAuthenticationSignIn(string userName, bool createPersistentCookie);
        public abstract void FormsAuthenticationSignOut();

        public abstract void SessionAbandon();

        public abstract void FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie();

        public abstract bool AuthenticationsWindows();

        protected void SetPermissions()
        {
            PermissionHash = Permissions.Get(context: this);
        }

        public string RequestData(string name)
        {
            return HttpMethod == "GET"
                ? QueryStrings.Data(name)
                : Forms.Data(name);
        }

        protected abstract ISqlObjectFactory GetSqlObjectFactory();

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