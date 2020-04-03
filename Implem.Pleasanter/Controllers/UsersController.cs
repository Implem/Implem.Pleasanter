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
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class UsersController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel(context: context);
                var html = UserUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = UserUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = UserUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new Context();
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel(context: context);
                var html = UserUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                    userId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = UserUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                    userId: id);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new Context();
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
        [HttpDelete]
        public string BulkDelete(long id)
        {
            var context = new Context();
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
        [HttpPost]
        public string Import(long id, HttpPostedFileBase[] file)
        {
            var context = new Context(files: file);
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Import(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string OpenExportSelectorDialog()
        {
            var context = new Context();
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
        [HttpGet]
        public ActionResult Export()
        {
            var context = new Context();
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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl, string ssocode = "")
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (context.Authenticated)
            {
                if (context.QueryStrings.Bool("new"))
                {
                    Authentications.SignOut(context: context);
                }
                log.Finish(context: context);
                return base.Redirect(Locations.Top(context: context));
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
                        return new RedirectResult(redirectUrl);
                    }
                }
                return new RedirectResult(Locations.InvalidSsoCode(context));
            }
            var html = UserUtilities.HtmlLogin(
                context: context,
                returnUrl: Url.IsLocalUrl(returnUrl) ? returnUrl : "",
                message: Request.QueryString["expired"] == "1" && !Request.IsAjaxRequest()
                    ? Messages.Expired(context: context).Text
                    : string.Empty);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult SamlLogin()
        {
            var context = new Context();
            if (!Authentications.SAML() 
                || HttpContext.User?.Identity?.AuthenticationType != "Federation"
                || HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                return new RedirectResult(Locations.SamlLoginFailed(context: context));
            }
            Authentications.SignOut(context: context);
            var loginId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier);
            var attributes = Saml.MapAttributes(ClaimsPrincipal.Current.Claims, loginId.Value);
            var name = attributes.UserName;
            TenantModel tenant;
            if (Parameters.Authentication.Provider == "SAML-MultiTenant")
            {
                if (string.IsNullOrEmpty(name))
                {
                    return new RedirectResult(Locations.EmptyUserName(context: context));
                }
                var ssocode = loginId.Issuer.TrimEnd('/').Substring(loginId.Issuer.TrimEnd('/').LastIndexOf('/') + 1);
                tenant = new TenantModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                    where: Rds.TenantsWhere().Comments(ssocode));
            }
            else
            {
                tenant = new TenantModel().Get(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                    where: Rds.TenantsWhere().TenantId(Parameters.Authentication.SamlParameters.SamlTenantId));
                if (tenant.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    Rds.ExecuteNonQuery(
                        context: context,
                        connectionString: Parameters.Rds.OwnerConnectionString,
                        statements: new[] {
                        Rds.IdentityInsertTenants(on:true),
                        Rds.InsertTenants(
                            param: Rds.TenantsParam()
                                .TenantId(Parameters.Authentication.SamlParameters.SamlTenantId)
                                .TenantName("DefaultTenant")),
                        Rds.IdentityInsertTenants(on: false)
                        });
                    tenant.TenantId = Parameters.Authentication.SamlParameters.SamlTenantId;
                }
            }
            try
            {
                Saml.UpdateOrInsert(
                    context: context,
                    tenantId: tenant.TenantId,
                    loginId: loginId.Value,
                    name: string.IsNullOrEmpty(name)
                        ? loginId.Value
                        : name,
                    mailAddress: attributes["MailAddress"],
                    synchronizedTime: System.DateTime.Now,
                    attributes: attributes);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == 2601)
                {
                    return new RedirectResult(Locations.LoginIdAlreadyUse(context: context));
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
                    return new RedirectResult(Locations.UserDisabled(context: context));
                }
                if (user.Lockout)
                {
                    return new RedirectResult(Locations.UserLockout(context: context));
                }
                user.Allow(context: context, returnUrl: Locations.Top(context), createPersistentCookie: true);
                return new RedirectResult(Locations.Top(context));
            }
            else
            {
                return new RedirectResult(Locations.SamlLoginFailed(context: context));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string Authenticate(string returnUrl)
        {
            var context = new Context();
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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Logout(string returnUrl)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            Authentications.SignOut(context: context);
            var url = Locations.Login(context: context);
            log.Finish(context: context);
            return Redirect(url);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ChangePassword(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ChangePassword(context: context, userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string ChangePasswordAtLogin()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ChangePasswordAtLogin(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ResetPassword(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ResetPassword(context: context, userId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress(int id)
        {
            var context = new Context();
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
        public string DeleteMailAddresses(int id)
        {
            var context = new Context();
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
        [AllowAnonymous]
        public string SyncByLdap()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SyncByLdap(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult EditApi()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = UserUtilities.ApiEditor(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CreateApiKey()
        {
            var context = new Context();
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
        [HttpPost]
        public string DeleteApiKey()
        {
            var context = new Context();
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
        [HttpPost]
        public string SwitchUser()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SwitchUser(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ReturnOriginalUser()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.ReturnOriginalUser(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string SetStartGuide()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.SetStartGuide(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
