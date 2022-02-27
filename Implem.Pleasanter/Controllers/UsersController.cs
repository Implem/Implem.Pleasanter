using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sustainsys.Saml2.AspNetCore2;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!context.Ajax)
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

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new Context();
            if (!context.Ajax)
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

        [HttpPost]
        public ActionResult SearchDropDown()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = Libraries.Models.DropDowns.SearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = Libraries.Models.DropDowns.SelectSearchDropDown(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return Content(json);
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
        public string Import(long id, ICollection<IFormFile> file)
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
                return responseFile.ToFile().ToFileContentResult();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Challenge(string idp = "")
        {
            if (!Authentications.SAML())
            {
                var context = new Context();
                return new RedirectResult(
                    Pleasanter.Libraries.Responses.Locations.Login(context: context));
            }
            return new ChallengeResult(Saml2Defaults.Scheme,
                new AuthenticationProperties(
                    items: string.IsNullOrEmpty(idp)
                        ? null
                        : new Dictionary<string, string> { ["idp"] = idp })
                {
                    RedirectUri = Url.Action(nameof(SamlLogin))
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl, string ssocode = "")
        {
            var context = new Context();
            if (Parameters.Authentication.Provider == "SAML-MultiTenant" && ssocode != string.Empty)
            {
                var contractSettings = GetTenantSamlSettings(context, ssocode);
                if (contractSettings == null)
                {
                    return Redirect(Locations.InvalidSsoCode(context: context));
                }
                var metadataLocation = SetSamlMetadataFile(context: context, guid: contractSettings.SamlMetadataGuid);
                return new ChallengeResult(Saml2Defaults.Scheme,
                    new AuthenticationProperties(
                        items: new Dictionary<string, string>
                        {
                            ["idp"] = contractSettings.SamlLoginUrl.Substring(0, contractSettings.SamlLoginUrl.TrimEnd('/').LastIndexOf('/') + 1),
                            ["SamlLoginUrl"] = contractSettings.SamlLoginUrl,
                            ["SamlMetadataLocation"] = metadataLocation
                        })
                    {
                        RedirectUri = Url.Action(nameof(SamlLogin))
                    });
            }
            var (redirectUrl, html) = Login(
                context: context,
                returnUrl: returnUrl,
                isLocalUrl: Url.IsLocalUrl(returnUrl));
            if (!string.IsNullOrEmpty(redirectUrl)) return base.Redirect(redirectUrl);
            ViewBag.HtmlBody = html;
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult SamlLogin()
        {
            var context = new Context();
            var result = SamlLogin(context: context);
            var redirectResult = new RedirectResult(result.redirectResultUrl);
            return redirectResult;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private (string redirectUrl, string html) Login(
            Context context, string returnUrl, bool isLocalUrl)
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
                    : Locations.Top(context: context), null);
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
            return (null, html);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ContractSettings GetTenantSamlSettings(Context context, string ssocode)
        {
            var tenant = new TenantModel().Get(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context),
                where: Rds.TenantsWhere().Comments(ssocode));
            if (tenant.AccessStatus == Databases.AccessStatuses.Selected)
            {
                var contractSettings = Saml.GetTenantSamlSettings(context: context, tenantId: tenant.TenantId);
                if (contractSettings != null)
                {
                    return contractSettings;
                }
            }
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string SetSamlMetadataFile(Context context, string guid)
        {
            var metadataPath = System.IO.Path.Combine(Directories.Temp(), "SamlMetadata", guid + ".xml");
            if (!System.IO.File.Exists(metadataPath))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(metadataPath));
                var bytes = Repository.ExecuteScalar_bytes(
                    context: context,
                    transactional: false,
                    statements: new Implem.Libraries.DataSources.SqlServer.SqlStatement[]
                    {
                        Rds.SelectBinaries(
                            column: Rds.BinariesColumn().Bin(),
                            where: Rds.BinariesWhere().Guid(guid))
                    });
                System.IO.File.WriteAllBytes(metadataPath, bytes);
            }
            return metadataPath;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private (string redirectUrl, string redirectResultUrl, string html) SamlLogin(Context context)
        {
            if (!Authentications.SAML()
                || context.AuthenticationType != "Federation"
                || context.IsAuthenticated != true)
            {
                return (null, Locations.SamlLoginFailed(context: context), null);
            }
            Authentications.SignOut(context: context);
            var loginId = context.UserClaims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var attributes = Saml.MapAttributes(context.UserClaims, loginId.Value);
            var name = attributes.UserName;
            TenantModel tenant;
            if (Parameters.Authentication.Provider == "SAML-MultiTenant")
            {
                if (string.IsNullOrEmpty(name))
                {
                    return (null, Locations.EmptyUserName(context: context), null);
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
                            Rds.IdentityInsertTenants(factory: context, on: true),
                            Rds.InsertTenants(
                                param: Rds.TenantsParam()
                                    .TenantId(Parameters.Authentication.SamlParameters.SamlTenantId)
                                    .TenantName("DefaultTenant")),
                            Rds.IdentityInsertTenants(factory: context, on: false)
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
                    return (null, Locations.UserLockout(context: context), null);
                }
                user.Allow(context: context, returnUrl: Locations.Top(context), createPersistentCookie: true);
                return (null, Locations.Top(context), null);
            }
            else
            {
                return (null, Locations.SamlLoginFailed(context: context), null);
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
                returnUrl: Url.IsLocalUrl(returnUrl)
                    ? returnUrl
                    : string.Empty);
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
        public string OpenChangePasswordDialog()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.OpenChangePasswordDialog(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
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
