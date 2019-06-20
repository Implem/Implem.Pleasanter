using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class DeptsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Index()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var htmlOrJson = controller.Index(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [HttpGet]
        public ActionResult New(long id = 0)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var html = controller.New(context: context, id: id);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var htmlOrJson = controller.Edit(context: context, id: id);
            if (!context.Ajax)
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
        public string GridRows()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.GridRows(context: context);
            return json;
        }

        [HttpPost]
        public string Create()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.Create(context: context);
            return json;
        }

        [HttpPut]
        public string Update(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.Update(context: context, id: id);
            return json;
        }

        [HttpDelete]
        public string Delete(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.Delete(context: context, id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteComment(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.DeleteComment(context: context, id: id);
            return json;
        }

        [HttpPost]
        public string Histories(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.Histories(context: context, id: id);
            return json;
        }

        [HttpPost]
        public string History(int id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DeptsController();
            var json = controller.History(context: context, id: id);
            return json;
        }
    }
}
