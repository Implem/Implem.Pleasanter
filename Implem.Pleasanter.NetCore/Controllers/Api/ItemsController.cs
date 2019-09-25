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
    public class ItemsController : ControllerBase
    {
        [HttpPost("{id}/Get")]
        public ContentResult Get(long id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Get(context: context, id: id);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Create")]
        public ContentResult Create(long id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Create(context: context, id: id);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Update")]
        public ContentResult Update(long id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Update(context: context, id: id);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Delete")]
        public ContentResult Delete(long id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Delete(context: context, id: id);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Export")]
        public ContentResult Export(long id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Export(context: context, id: id);
            return result.ToHttpResponse(request: Request);
        }
    }
}