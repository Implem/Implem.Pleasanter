using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.WebPages;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [AllowAnonymous]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ContentResult Scripts()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ResourcesController();
            var result = controller.Scripts(context: context);
            return result.ToRecourceContentResult(request: Request);
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ContentResult Styles()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ResourcesController();
            var result = controller.Styles(context: context);
            return result.ToRecourceContentResult(request: Request);
        }

        [HttpPost]
        [ResponseCache(Duration = int.MaxValue)]
        public string Responsive(string Responsive)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.ResourcesController();
            var result = controller.Responsive(context: context, Responsive: Responsive);

            return result;
        }
    }
}