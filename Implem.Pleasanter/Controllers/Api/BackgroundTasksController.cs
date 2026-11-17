using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IO;

namespace Implem.Pleasanter.Controllers.Api
{
    [CheckApiContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("Api")]
    public class BackgroundTasksController : ControllerBase
    {
        [HttpPost("RebuildSearchIndexes")]
        [HttpPost("{id}/RebuildSearchIndexes")]
        [EnableRateLimiting("ApiHeavy")]
        public ContentResult RebuildSearchIndexes(long id = 0)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            if (!context.Authenticated)
            {
                return ApiResults.Unauthorized(context: context);
            }
            else if (!Parameters.BackgroundTask.Enabled)
            {
                return ApiResults.Forbidden(context: context);
            }
            else
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: false);
                switch (apiErrorData.Type)
                {
                    case Error.Types.None: break;
                    default:
                        return ApiResults.Error(
                            context: context,
                            errorData: apiErrorData);
                }
                if (context.QueryStrings.Bool("NoLog"))
                {
                    Indexes.RebuildSearchIndexes(context: context);
                }
                else
                {
                    var log = new SysLogModel(context: context);
                    Indexes.RebuildSearchIndexes(
                        context: context,
                        siteId: id);
                    log.Finish(context: context, responseSize: 0);
                }
                return ApiResults.Success(
                    id: id,
                    message: Displays.RebuildingCompleted(context: context));
            }
        }
    }
}