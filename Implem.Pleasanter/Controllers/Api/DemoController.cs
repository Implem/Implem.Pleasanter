using Implem.DefinitionAccessor;
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
    public class DemoController : ControllerBase
    {
        [HttpPost("Register")]
        public ContentResultInheritance Register()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.ContentType);
            var log = new SysLogModel(context: context);
            if (Parameters.Service.DemoApi)
            {
                var result = DemoUtilities.RegisterByApi(context: context);
                log.Finish(context: context, responseSize: result.Content.Length);
                return result.ToHttpResponse(request: Request);
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }
    }
}