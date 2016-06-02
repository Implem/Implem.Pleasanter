using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var log = new SysLogModel();
            var html = UsersUtility.Index(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins());
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var log = new SysLogModel();
            var html = UsersUtility.EditorNew();
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var log = new SysLogModel();
            var html = UsersUtility.Editor(id, clearSessions: true);
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
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
        public string DataView()
        {
            var log = new SysLogModel();
            var json = UsersUtility.DataView(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins());
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string GridRows()
        {
            var log = new SysLogModel();
            var json = UsersUtility.GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                setByForm: true).Create();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id,
                setByForm: true)
                    .Update();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .Delete();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id,
                setByForm: true)
                    .Update();
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .Histories();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .History();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Previous(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .Previous();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Next(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .Next();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Reload(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .Reload();
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
                return base.Redirect(Navigations.Top());
            }
            else
            {
                var html = UsersUtility.HtmlLogin(returnUrl);
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
            var json = Securities.Authenticate(returnUrl);
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
            Securities.Logout();
            var url = Navigations.Login();
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
            var json = Passwords.Reset(id);
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .AddMailAddress();
            log.Finish(json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteMailAddresses(int id)
        {
            var log = new SysLogModel();
            var json = new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Admins(),
                id)
                    .DeleteMailAddresses();
            log.Finish(json.Length);
            return json;
        }
    }
}
