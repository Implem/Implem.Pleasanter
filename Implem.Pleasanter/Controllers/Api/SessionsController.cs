using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Implem.Pleasanter.Controllers.Api
{
    [CheckApiContextAttributes]
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
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Set")]
        public ContentResult Set()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.SetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Delete")]
        public ContentResult Delete()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.DeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }
    }
}
