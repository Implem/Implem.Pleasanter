using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class TenantsController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Edit()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.TenantsController();
            var htmlOrJson = controller.Edit(context: context);
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
