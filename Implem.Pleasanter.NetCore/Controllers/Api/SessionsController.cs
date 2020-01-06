using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Implem.Pleasanter.NetCore.Controllers.Api
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        [HttpPost("Get")]
        public ContentResult Get()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Set")]
        public ContentResult Set()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Set(context: context);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Delete")]
        public ContentResult Delete()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Delete(context: context);
            return result.ToHttpResponse(request: Request);
        }
    }
}
