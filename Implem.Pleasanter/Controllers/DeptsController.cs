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
    public class DeptsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel(context: context);
                var html = DeptUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = DeptUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = DeptUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
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
                var html = DeptUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                    deptId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = DeptUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                    deptId: id);
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
            var json = DeptUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = DeptUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                deptId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
