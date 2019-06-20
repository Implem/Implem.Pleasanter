using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.TenantsController();
            var htmlOrJson = controller.Edit(context: context);
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

        [HttpPut]
        public string Update()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.TenantsController();
            var json = controller.Update(context: context);
            return json;
        }
    }
}
