using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [RefleshSiteInfo]
    public class VersionsController : Controller
    {
        public ActionResult Index()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var html = new HtmlBuilder().AssemblyVersions(context: context);
            ViewBag.HtmlBody = html;
            log.Finish(context: context, responseSize: html.Length);
            return View();
        }
    }
}