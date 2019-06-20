using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class AdminsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.AdminsController();
            var html = controller.Index(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [HttpGet]
        public ActionResult InitializeItems()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.AdminsController();
            controller.InitializeItems(context: context);
            return View();
        }

        [HttpGet]
        public ActionResult MigrateSiteSettings()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.AdminsController();
            controller.MigrateSiteSettings(context: context);
            return View();
        }

        [HttpGet]
        public string ReloadParameters()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.AdminsController();
            var json = controller.ReloadParameters(context: context);
            return json;
        }
    }
}
