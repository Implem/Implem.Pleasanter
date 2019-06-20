using Implem.Pleasanter.NetFramework.Filters;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    [Publishes]
    public class PublishesController : Controller
    {
        public ActionResult Index(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishesController();
            var htmlOrJson = controller.Index(context: context, id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpPost]
        public ActionResult SearchDropDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishesController();
            var json = controller.SearchDropDown(context: context, id: id);
            return Content(json);
        }

        [HttpPost]
        public ActionResult SelectSearchDropDown(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishesController();
            var json = controller.SelectSearchDropDown(context: context, id: id);
            return Content(json);
        }

        [HttpPost]
        public string GridRows(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishesController();
            var json = controller.GridRows(context: context, id: id);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishesController();
            var htmlOrJson = controller.Edit(context: context, id: id);
            if (!Request.IsAjaxRequest())
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }
    }
}