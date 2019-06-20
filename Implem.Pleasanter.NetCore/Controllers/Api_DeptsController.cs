using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [AllowAnonymous]
    public class Api_DeptsController : Controller
    {
        [HttpPost]
        public ContentResult Get()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_DeptsController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(request: Request);
        }
    }
}