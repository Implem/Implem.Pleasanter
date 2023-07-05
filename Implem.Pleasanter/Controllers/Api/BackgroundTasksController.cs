using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Search;
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
    public class BackgroundTasksController : ControllerBase
    {
        [HttpPost("RebuildSearchIndexes")]
        [HttpPost("{id}/RebuildSearchIndexes")]
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