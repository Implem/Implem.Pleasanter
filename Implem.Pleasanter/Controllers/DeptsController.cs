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
    public class DeptsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = DeptUtilities.Index(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Admins());
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = DeptUtilities.IndexJson(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Admins());
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var log = new SysLogModel();
            var html = DeptUtilities.EditorNew();
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
                var html = DeptUtilities.Editor(id, clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = new DeptModel(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Admins(),
                    id)
                        .EditorJson();
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
            var json = DeptUtilities.GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var log = new SysLogModel();
            var json = DeptUtilities.Create(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins());
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = DeptUtilities.Update(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                id);
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
                id,
                setByForm: true)
                    .Delete();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = DeptUtilities.Update(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                id);
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                id)
                    .History();
            log.Finish(json.Length);
            return json;
        }
    }
}
