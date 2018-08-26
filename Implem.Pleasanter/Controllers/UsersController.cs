using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [ValidateInput(false)]
    [RefleshSiteInfo]
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

        [HttpGet]
        public ActionResult Export(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var responseFile = new ItemModel(
                context: context,
                referenceId: id)
                    .Export(context: context);
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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (Sessions.LoggedIn())
            {
                if (QueryStrings.Bool("new"))
                {
                    Authentications.SignOut();
                }
                log.Finish(context: context);
                return base.Redirect(Locations.Top());
            }
            var html = UserUtilities.HtmlLogin(
                context: context,
                returnUrl: returnUrl,
                message: Request.QueryString["expired"] == "1" && !Request.IsAjaxRequest()
                    ? Messages.Expired().Text
                    : string.Empty);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
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
            var json = Authentications.SignIn(context: context, returnUrl: returnUrl);
            log.Finish(context: context, responseSize: json.Length);
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
            Authentications.SignOut();
            var url = Locations.Login();
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
    }
}
