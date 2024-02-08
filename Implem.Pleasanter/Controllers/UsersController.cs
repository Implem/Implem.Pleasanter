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
        [HttpPost]
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
        public ActionResult Challenge(string returnUrl = "", string idp = "")
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (!Authentications.SAML())
            {
                log.Finish(context: context);
                return new RedirectResult(
                    Locations.Login(context: context));
            }
            log.Finish(context: context);
            return new ChallengeResult(Saml2Defaults.Scheme,
                new AuthenticationProperties(
                    items: string.IsNullOrEmpty(idp)
                        ? null
                        : new Dictionary<string, string> { ["idp"] = idp })
                {
                    RedirectUri = Url.Action(nameof(SamlLogin), new {returnUrl})
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
            var log = new SysLogModel(context: context);
            if (Parameters.Authentication.Provider == "SAML-MultiTenant" && ssocode != string.Empty)
            {
                return ChallengeBySsoCode(ssocode, context);
            }
            var (redirectUrl, html) = Login(
                context: context,
                returnUrl: returnUrl,
                isLocalUrl: Url.IsLocalUrl(returnUrl));
            if (!string.IsNullOrEmpty(redirectUrl)) return base.Redirect(redirectUrl);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult SamlLogin(string returnUrl)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = Saml.SamlLogin(
                context: context,
                returnUrl: Url.IsLocalUrl(returnUrl)
                    ? returnUrl
                    : string.Empty);
            var redirectResult = new RedirectResult(result.redirectResultUrl);
            log.Finish(context: context);
            return redirectResult;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private ActionResult ChallengeBySsoCode(string ssocode, Context context)
        {
            var tenant = UserUtilities.GetContractSettingsBySsoCode(
                context: context,
                ssocode: ssocode);
            if (tenant.TenantId == 0
                || !Saml.HasSamlSettings(contractSettings: tenant.ContractSettings))
            {
                return Redirect(Locations.InvalidSsoCode(context: context));
            }
            var idp = Saml.SetIdpCache(
                context: context,
                tenantId: tenant.TenantId,
                contractSettings: tenant.ContractSettings);
            if (idp == null)
            {
                return Redirect(Locations.InvalidSsoCode(context: context));
            }
            var items = new Dictionary<string, string>
            {
                { "idp", idp.EntityId.Id },
                { "SignOnUrl", tenant.ContractSettings.SamlLoginUrl }
            };
            var properties = new AuthenticationProperties(items: items)
            {
                RedirectUri = Url.Action(nameof(SamlLogin))
            };
            return new ChallengeResult(
                authenticationScheme: Saml2Defaults.Scheme,
                properties: properties);
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
            if (Parameters.BackgroundService.SyncByLdap)
            {
                return null;
            }
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
        [HttpGet]
        public ActionResult SwitchTenant(int id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            Extension.SetSwichTenant(context, id);
            var url = Locations.Top(context: context);
            log.Finish(context: context);
            return Redirect(url);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpGet]
        public ActionResult ReturnOriginalTenant()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            Extension.UnsetSwichTenant(context);
            var url = Locations.Top(context: context);
            log.Finish(context: context);
            return Redirect(url);
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

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CloseAnnouncement()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.CloseAnnouncement(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult TrashBox()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (!context.Ajax)
            {
                var html = UserUtilities.TrashBox(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(
                        context: context,
                        tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return context.RedirectData.Url.IsNullOrEmpty()
                    ? View()
                    : Redirect(context.RedirectData.Url);
            }
            else
            {
                var json = UserUtilities.TrashBoxJson(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(
                        context: context,
                        tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string Restore(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.Restore(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpDelete]
        public string PhysicalDelete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = UserUtilities.PhysicalBulkDelete(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(
                    context: context,
                    tableTypes: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Deleted));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
