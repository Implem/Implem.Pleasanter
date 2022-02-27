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
    public class ExtendedController : ControllerBase
    {
        [HttpPost("Sql")]
        public ContentResult Sql()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? ExtensionUtilities.Sql(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }
    }
}