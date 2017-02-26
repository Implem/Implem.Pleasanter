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
                    ss: SiteSettingsUtilities.GroupsSiteSettings());
                ViewBag.HtmlBody = html;
                log.Finish(html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel();
                var json = GroupUtilities.IndexJson(
                    ss: SiteSettingsUtilities.GroupsSiteSettings());
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
                    ss: SiteSettingsUtilities.GroupsSiteSettings(),
                    groupId: id);
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
            var json = GroupUtilities.GridRows();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Create(
                ss: SiteSettingsUtilities.GroupsSiteSettings());
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Update(
                ss: SiteSettingsUtilities.GroupsSiteSettings(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Delete(
                ss: SiteSettingsUtilities.UsersSiteSettings(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Update(
                ss: SiteSettingsUtilities.GroupsSiteSettings(),
                groupId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public string Histories(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Histories(
                ss: SiteSettingsUtilities.GroupsSiteSettings(),
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

        [HttpPost]
        public string Set(int id)
        {
            var log = new SysLogModel();
            var json = GroupUtilities.Set(id);
            log.Finish(json.Length);
            return json;
        }
    }
}
