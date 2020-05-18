using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Security.Claims;
using System.Data.Common;
namespace Implem.Pleasanter.Controllers
{
    public class UsersController : Controller
    {
        public string Index(Context context)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = UserUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = UserUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string New(Context context, long id = 0)
        {
            var log = new SysLogModel(context: context);
            var html = UserUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        public string Edit(Context context, int id)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = UserUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                    userId: id,
                    clearSessions: true);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = UserUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                    userId: id);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string GridRows(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Create(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Update(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Delete(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteComment(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Histories(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string History(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string BulkDelete(Context context, long id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Import(Context context, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Import(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string OpenExportSelectorDialog(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.OpenExportSelectorDialog(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public FileContentResult Export(Context context)
        {
            var log = new SysLogModel(context: context);
            var responseFile = UserUtilities.Export(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            if (responseFile != null)
            {
                log.Finish(context: context, responseSize: responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(context: context, responseSize: 0);
                return null;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public (string redirectUrl, string redirectResultUrl, string html) Login(
            Context context, string returnUrl, bool isLocalUrl, string ssocode = "")
        {
            var log = new SysLogModel(context: context);
            if (context.Authenticated)
            {
                if (context.QueryStrings.Bool("new"))
                {
                    Authentications.SignOut(context: context);
                }
                log.Finish(context: context);
                return (isLocalUrl
                    ? returnUrl
                    : Locations.Top(context: context), null, null);
            }
            if ((Parameters.Authentication.Provider == "SAML-MultiTenant") && (ssocode != string.Empty))
            {
                var tenant = new TenantModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context), 
                    where: Rds.TenantsWhere().Comments(ssocode));
                if (tenant.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    var redirectUrl = Saml.SetIdpConfiguration(context, tenant.TenantId);
                    if (redirectUrl != null)
                    {
                        return (null, redirectUrl, null);
                    }
                }
                return (null, Locations.InvalidSsoCode(context), null);
            }
            var html = UserUtilities.HtmlLogin(
                context: context,
                returnUrl: isLocalUrl
                    ? returnUrl
                    : string.Empty,
                message: context.QueryStrings.ContainsKey("expired") && context.QueryStrings["expired"] == "1" && !context.Ajax
                    ? Messages.Expired(context: context).Text
                    : string.Empty);
            log.Finish(context: context, responseSize: html.Length);
            return (null, null, html);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public (string redirectUrl, string redirectResultUrl, string html) SamlLogin(Context context)
        {
            if (context.AuthenticationType == "Federation"
                && context.IsAuthenticated == true)
            {
                Authentications.SignOut(context: context);
                var loginId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier);
                var firstName = string.Empty;
                var lastName = string.Empty;
                var tenantManager = false;
                foreach(var claim in ClaimsPrincipal.Current.Claims)
                {
                    switch (claim.Type)
                    {
                        case "FirstName":
                            firstName = claim.Value;
                            break;
                        case "LastName":
                            lastName = claim.Value;
                            break;
                        case "TenantManager":
                            tenantManager = claim.Value.ToLower() == "true" ? true : false;
                            break;
                    }
                }
                var space = (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName)) ? string.Empty : "　";
                var name = lastName + space + firstName;
                if (name == string.Empty)
                {
                    return (null, Locations.EmptyUserName(context: context), null);
                }
                var ssocode = loginId.Issuer.TrimEnd('/').Substring(loginId.Issuer.TrimEnd('/').LastIndexOf('/') + 1);
                var tenant = new TenantModel().Get(
                    context: context,
                    ss:SiteSettingsUtilities.TenantsSiteSettings(context), 
                    where: Rds.TenantsWhere().Comments(ssocode));
                try
                {
                    Saml.UpdateOrInsert(
                        context: context,
                        tenantId: tenant.TenantId,
                        loginId: loginId.Value,
                        name: name,
                        mailAddress: loginId.Value,
                        tenantManager: tenantManager,
                        synchronizedTime: System.DateTime.Now);
                }
                catch (DbException e)
                {
                    if (context.SqlErrors.ErrorCode(e) == 2601)
                    {
                        return (null, Locations.LoginIdAlreadyUse(context: context), null);
                    }
                    throw;
                }
                var user = new UserModel().Get(
                    context: context,
                    ss: null,
                    where: Rds.UsersWhere()
                        .TenantId(tenant.TenantId)
                        .LoginId(loginId.Value));
                if (user.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    if (user.Disabled)
                    {
                        return (null, Locations.UserDisabled(context: context), null);
                    }
                    if (user.Lockout)
                    {
                        return (null,  Locations.UserLockout(context: context), null);
                    }
                    user.Allow(context: context, returnUrl: Locations.Top(context), createPersistentCookie: true);
                    return (null, Locations.Top(context), null);
                }
                else
                {
                    return (null, Locations.SamlLoginFailed(context: context), null);
                }
            }
            return (null, Locations.SamlLoginFailed(context: context), null);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Authenticate(Context context, string returnUrl)
        {
            var log = new SysLogModel(context: context);
            var json = Authentications.SignIn(
                context: context,
                returnUrl: returnUrl);
            log.Finish(
                context: context,
                responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Logout(Context context, string returnUrl)
        {
            var log = new SysLogModel(context: context);
            Authentications.SignOut(context: context);
            var url = Locations.Login(context: context);
            log.Finish(context: context);
            return url;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ChangePassword(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ChangePassword(context: context, userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ChangePasswordAtLogin(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ChangePasswordAtLogin(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ResetPassword(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ResetPassword(context: context, userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.AddMailAddresses(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteMailAddresses(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.DeleteMailAddresses(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SyncByLdap(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SyncByLdap(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string EditApi(Context context)
        {
            var log = new SysLogModel(context: context);
            var html = UserUtilities.ApiEditor(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string CreateApiKey(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.CreateApiKey(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteApiKey(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.DeleteApiKey(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SwitchUser(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SwitchUser(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ReturnOriginalUser(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ReturnOriginalUser(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SetStartGuide(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SetStartGuide(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
