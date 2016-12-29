using Implem.DefinitionAccessor;
using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [RefleshSiteInfo]
    public class AdminsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var log = new SysLogModel();
            var html = new HtmlBuilder().AdminsIndex();
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [HttpGet]
        public ActionResult ResetDefinitions()
        {
            var log = new SysLogModel();
            SetDefinition();
            log.Finish();
            return View();
        }

        private static void SetDefinition()
        {
            Initializer.SetDefinitions();
        }
    }
}
