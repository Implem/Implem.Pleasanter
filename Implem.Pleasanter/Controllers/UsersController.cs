using Implem.Pleasanter.Filters;
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
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = UserUtilities.Index(
                    ss: SiteSettingsUtilities.UsersSiteSettings());
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = UserUtilities.IndexJson(
                    ss: SiteSettingsUtilities.UsersSiteSettings());
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var log = new SysLogModel();
            var html = UserUtilities.EditorNew(
                SiteSettingsUtilities.UsersSiteSettings());
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = UserUtilities.Editor(
                    SiteSettingsUtilities.UsersSiteSettings(),
                    id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = UserUtilities.EditorJson(
                    ss: SiteSettingsUtilities.UsersSiteSettings(),
                    userId: id);
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult Export(long id)
        {
            var log = new SysLogModel();
            var responseFile = new ItemModel(id).Export();
            if (responseFile != null)
            {
                log.Finish(responseFile.Length);
                return responseFile.ToFile();
            }
            else
            {
                log.Finish(0);
                return null;
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var log = new SysLogModel();
            var json = UserUtilities.GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var log = new SysLogModel();
            var json = UserUtilities.Create(
                ss: SiteSettingsUtilities.UsersSiteSettings());
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Update(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Delete(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Update(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Histories(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.History(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var log = new SysLogModel();
            if (Sessions.LoggedIn())
            {
                if (Libraries.Requests.QueryStrings.Bool("new"))
                {
                    Authentications.SignOut();
                }
                log.Finish();
                return base.Redirect(Locations.Top());
            }
            var html = UserUtilities.HtmlLogin(
                returnUrl,
                Request.QueryString["expired"] == "1" && !Request.IsAjaxRequest()
                    ? Messages.Expired().Text
                    : string.Empty);
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string Authenticate(string returnUrl)
        {
            var log = new SysLogModel();
            var json = Authentications.SignIn(returnUrl);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Logout(string returnUrl)
        {
            var log = new SysLogModel();
            Authentications.SignOut();
            var url = Locations.Login();
            log.Finish();
            return Redirect(url);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ChangePassword(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.ChangePassword(id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string ChangePasswordAtLogin()
        {
            var log = new SysLogModel();
            var json = UserUtilities.ChangePasswordAtLogin();
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string ResetPassword(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.ResetPassword(id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.AddMailAddresses(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteMailAddresses(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.DeleteMailAddresses(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        public string SyncByLdap()
        {
            var log = new SysLogModel();
            var json = UserUtilities.SyncByLdap();
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ActionResult EditApi()
        {
            var log = new SysLogModel();
            var html = UserUtilities.ApiEditor(
                ss: SiteSettingsUtilities.UsersSiteSettings());
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string CreateApiKey()
        {
            var log = new SysLogModel();
            var json = UserUtilities.CreateApiKey(
                ss: SiteSettingsUtilities.UsersSiteSettings());
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string DeleteApiKey()
        {
            var log = new SysLogModel();
            var json = UserUtilities.DeleteApiKey(
                ss: SiteSettingsUtilities.UsersSiteSettings());
            log.Finish(json.Length);
            return json;
        }
    }
}
