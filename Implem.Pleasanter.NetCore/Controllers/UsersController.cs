using DocumentFormat.OpenXml.Spreadsheet;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sustainsys.Saml2.AspNetCore2;
using System.Collections.Generic;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var htmlOrJson = controller.Index(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var html = controller.New(context: context, id: id);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var htmlOrJson = controller.Edit(context: context, id: id);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.GridRows(context: context);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Create(context: context);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Update(context: context, id: id);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Delete(context: context, id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.DeleteComment(context: context, id: id);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Histories(context: context, id: id);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.History(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpDelete]
        public string BulkDelete(long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.BulkDelete(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string Import(long id, ICollection<IFormFile> file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Import(context: context, id: id, file: HttpPostedFile.Create(file));
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string OpenExportSelectorDialog()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.OpenExportSelectorDialog(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpGet]
        public ActionResult Export()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var responseFile = controller.Export(context: context);
            if (responseFile != null)
            {
                return responseFile.ToFileContentResult();

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
                var context = new ContextImplement();
                return new RedirectResult(
                    Pleasanter.Libraries.Responses.Locations.Login(context: context));
            }
            return new ChallengeResult(Saml2Defaults.Scheme,
                new AuthenticationProperties(
                    items: string.IsNullOrEmpty(idp)
                        ? null
                        : new Dictionary<string, string> { ["idp"] = idp })
                {
                    RedirectUri = Url.Action(nameof(SsoSync))
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl, string ssocode = "")
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var (redirectUrl, redirectResultUrl, html, ssoLogin) = controller.Login(
                context: context,
                returnUrl: returnUrl,
                isLocalUrl: Url.IsLocalUrl(returnUrl),
                ssocode: ssocode);

            if (ssoLogin && !string.IsNullOrEmpty(redirectResultUrl))
            {
                return new ChallengeResult(Saml2Defaults.Scheme,
                    new AuthenticationProperties(
                        items: null,
                        parameters: new Dictionary<string, object> { ["idp"] = redirectResultUrl })
                    {
                        RedirectUri = Url.Action(nameof(SsoSync), new { returnUrl })
                    });
            }
            if (!string.IsNullOrEmpty(redirectUrl)) return base.Redirect(redirectUrl);
            if (!string.IsNullOrEmpty(redirectResultUrl)) return new RedirectResult(redirectResultUrl);
            ViewBag.HtmlBody = html;
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult SsoSync()
        {
           
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var result = controller.SsoSync(context: context);
            var redirectResult = new RedirectResult(result.redirectResultUrl);
            return redirectResult;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string Authenticate(string returnUrl)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.Authenticate(context: context, returnUrl: returnUrl);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Logout(string returnUrl)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var url = controller.Logout(context: context, returnUrl: returnUrl);
            this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            this.HttpContext.Session.Clear();
            return Redirect(url);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ChangePassword(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.ChangePassword(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string ChangePasswordAtLogin()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.ChangePasswordAtLogin(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ResetPassword(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.ResetPassword(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.AddMailAddress(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteMailAddresses(int id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.DeleteMailAddresses(context: context, id: id);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        public string SyncByLdap()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.SyncByLdap(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult EditApi()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var html = controller.EditApi(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CreateApiKey()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.CreateApiKey(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string DeleteApiKey()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.DeleteApiKey(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string SwitchUser()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.SwitchUser(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ReturnOriginalUser()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.ReturnOriginalUser(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string SetStartGuide()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.UsersController();
            var json = controller.SetStartGuide(context: context);
            return json;
        }
    }
}
