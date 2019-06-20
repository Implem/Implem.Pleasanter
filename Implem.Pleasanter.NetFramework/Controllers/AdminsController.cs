using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
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
