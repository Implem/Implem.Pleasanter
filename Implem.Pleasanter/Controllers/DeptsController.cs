using Implem.Pleasanter.Libraries.Models;
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
        [HttpGet]
        public ActionResult Index()
        {
            var log = new SysLogModel();
            var html = DeptUtilities.Index(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins());
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var log = new SysLogModel();
            var html = DeptUtilities.Editor(id, clearSessions: true);
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
            var json = DeptUtilities.DataView(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins());
            log.Finish(json.Length);
            return json;
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                setByForm: true).Create();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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

        [HttpPost]
        public string Previous(int id)
        {
            var log = new SysLogModel();
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
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
            var json = new DeptModel(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Admins(),
                id)
                    .Reload();
            log.Finish(json.Length);
            return json;
        }
    }
}
