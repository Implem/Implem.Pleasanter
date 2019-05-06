using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class AdminsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = new HtmlBuilder().AdminsIndex(context: context);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }

        [HttpGet]
        public ActionResult InitializeItems()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            ItemsInitializer.Initialize(context: context);
            log.Finish(context: context, responseSize: 0);
            return View();
        }

        [HttpGet]
        public ActionResult MigrateSiteSettings()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            SiteSettingsMigrator.Migrate(context: context);
            log.Finish(context: context, responseSize: 0);
            return View();
        }

        [HttpGet]
        public string ReloadParameters()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = ParametersInitializer.Initialize(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
