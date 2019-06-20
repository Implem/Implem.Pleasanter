using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
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