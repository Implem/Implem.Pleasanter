using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class GroupsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel(context: context);
                var html = GroupUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = GroupUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
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
                var html = GroupUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = GroupUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                    groupId: id);
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
            var json = GroupUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                groupId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string SelectableMembers(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = GroupUtilities.SelectableMembersJson(context: context);
            return json;
        }
    }
}
