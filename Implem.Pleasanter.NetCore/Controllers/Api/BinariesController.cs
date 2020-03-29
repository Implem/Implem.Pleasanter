using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers.Api
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BinariesController : ControllerBase
    {
        [HttpPost]
        public ContentResult Get(string guid)
        {
            var body = default(string);
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.BinariesController();
            var result = controller.Get(
                context: context,
                guid: guid);
            return result.ToHttpResponse(request: Request);
        }
    }
}