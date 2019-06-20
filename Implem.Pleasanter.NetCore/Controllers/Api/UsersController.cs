using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore.Controllers.Api
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpPost("Get")]
        public ContentResult Get()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.UsersController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Create")]
        public ContentResult Create()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.UsersController();
            var result = controller.Create(context: context);
            return result.ToHttpResponse(Request);
        }

        [HttpPost("{id}/Update")]
        public ContentResult Update(int id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.UsersController();
            var result = controller.Update(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost("{id}/Delete")]
        public ContentResult Delete(int id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.UsersController();
            var result = controller.Delete(context: context, id: id);
            return result.ToHttpResponse(Request);
        }
    }
}