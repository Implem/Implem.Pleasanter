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
    public class RegistrationsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new Context();
            if (!Request.IsAjaxRequest())
            {
                var log = new SysLogModel(context: context);
                var html = RegistrationUtilities.Index(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = RegistrationUtilities.IndexJson(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = RegistrationUtilities.EditorNew(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
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
                var html = RegistrationUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                    registrationId: id,
                    clearSessions: true);
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = RegistrationUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                    registrationId: id);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPost]
        public string GridRows()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.GridRows(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Create(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Delete(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Histories(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.History(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
                registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpDelete]
        public string BulkDelete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.BulkDelete(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = RegistrationUtilities.Login(
                context: context,
                ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context));
            log.Finish(context: context);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public string ApprovalReauest(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.ApprovalReauest(context: context,
            ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
            registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPut]
        public string Approval(int id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = RegistrationUtilities.Approval(context: context,
            ss: SiteSettingsUtilities.RegistrationsSiteSettings(context: context),
            registrationId: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
