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
    public class GroupsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel();
                var html = GroupUtilities.Index(
                    SiteSettingsUtilities.GroupsSiteSettings(),
                    Permissions.Admins());
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = GroupUtilities.IndexJson(
                    SiteSettingsUtilities.GroupsSiteSettings(),
                    Permissions.Admins());
                log.Finish(json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var log = new SysLogModel();
            var html = GroupUtilities.EditorNew();
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
                var html = GroupUtilities.Editor(id, clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = GroupUtilities.EditorJson(
                    SiteSettingsUtilities.GroupsSiteSettings(),
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
            var json = GroupUtilities.GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Create(
                SiteSettingsUtilities.GroupsSiteSettings(),
                Permissions.Admins());
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Update(
                SiteSettingsUtilities.GroupsSiteSettings(),
                Permissions.Admins(),
                id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Delete(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                pt: Permissions.Admins(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Update(
                SiteSettingsUtilities.GroupsSiteSettings(),
                Permissions.Admins(),
                id);
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Histories(
                ss: SiteSettingsUtilities.GroupsSiteSettings(),
                pt: Permissions.Admins(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.History(
                ss: SiteSettingsUtilities.GroupsSiteSettings(),
                pt: Permissions.Admins(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string SelectableMembers(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.SelectableMembersJson();
            return json;
        }
    }
}
