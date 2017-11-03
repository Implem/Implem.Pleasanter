using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
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
        public string ReloadParameters()
        {
            var log = new SysLogModel();
            var json = ParametersInitializer.Initialize();
            log.Finish(json.Length);
            return json;
        }
    }
}
