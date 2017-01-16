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
                    SiteSettingsUtilities.UsersSiteSettings(),
                    Permissions.Admins());
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = UserUtilities.IndexJson(
                    SiteSettingsUtilities.UsersSiteSettings(),
                    Permissions.Admins());
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var log = new SysLogModel();
            var html = UserUtilities.EditorNew();
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
                var html = UserUtilities.Editor(id, clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = UserUtilities.EditorJson(
                    SiteSettingsUtilities.UsersSiteSettings(),
                    Permissions.Admins(),
                    id);
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult Export(long id)
        {
            var log = new SysLogModel();
            var responseFile = new ItemModel(id).Export();
            log.Finish(responseFile.Length);
            return responseFile.ToFile();
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
                SiteSettingsUtilities.UsersSiteSettings(),
                Permissions.Admins());
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Update(
                SiteSettingsUtilities.UsersSiteSettings(),
                Permissions.Admins(),
                id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Delete(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                pt: Permissions.Admins(),
                userId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Update(
                SiteSettingsUtilities.UsersSiteSettings(),
                Permissions.Admins(),
                id);
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = UserUtilities.Histories(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                pt: Permissions.Admins(),
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
                pt: Permissions.Admins(),
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
                log.Finish();
                return base.Redirect(Locations.Top());
            }
            else
            {
                var html = UserUtilities.HtmlLogin(returnUrl);
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
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
            var json = Passwords.Change(id);
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
            var json = Passwords.ChangeAtLogin();
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
            var json = Passwords.Reset(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                pt: Permissions.Admins(),
                userId: id);
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
                pt: Permissions.Admins(),
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
                pt: Permissions.Admins(), 
                userId: id);
            log.Finish(json.Length);
            return json;
        }
    }
}
