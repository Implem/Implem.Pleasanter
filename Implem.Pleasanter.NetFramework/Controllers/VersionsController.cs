using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    public class VersionsController : Controller
    {
        public ActionResult Index()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.VersionsController();
            var html = controller.Index(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }
    }
}