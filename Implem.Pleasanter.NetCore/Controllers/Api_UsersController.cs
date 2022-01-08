using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [AllowAnonymous]
    public class Api_UsersController : Controller
    {
        [HttpPost]
        public ContentResult Get(int id = 0)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.Api_UsersController();
            var result = controller.Get(
                context: context,
                id: id);
            return result.ToHttpResponse(request: Request);
        }
    }
}