using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
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
    public class UtilityController : ControllerBase
    {
        [HttpPost("GetLicenseInfo")]
        public ContentResult GetLicenseInfo()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? ApiResults.Get(new
                {
                    StatusCode = 200,
                    Response = new
                    {
                        CommercialLicense = Parameters.CommercialLicense(),
                        LicenseDeadline = Parameters.LicenseDeadline(),
                    }
                }.ToJson())
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

    }
}