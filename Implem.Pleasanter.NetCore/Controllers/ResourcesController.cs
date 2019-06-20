using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var controller = new Implem.Pleasanter.Controllers.ResourcesController();
            var result = controller.Scripts(context: context);
            return result.ToRecourceContentResult(request: Request);
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ContentResult Styles()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ResourcesController();
            var result = controller.Styles(context: context);
            return result.ToRecourceContentResult(request: Request);
        }
    }
}