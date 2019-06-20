using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Implem.Pleasanter.NetCore.Libraries.Responses;

namespace Implem.Pleasanter.NetCore.Controllers.Api
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class DeptsController : ControllerBase
    {
        [HttpPost("Get")]
        public ContentResult Get()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.DeptsController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(request: Request);
        }
    }
}