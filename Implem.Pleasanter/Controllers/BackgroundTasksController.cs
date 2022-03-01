using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Models;
using Implem.PleasanterTools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class BackgroundTasksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Do()
        {
            var context = new Context();
            if (Parameters.BackgroundTask.Enabled)
            {
                var backgroundTasks = new BackgroundTasks(context: context);
                if (context.QueryStrings.Bool("NoLog"))
                {
                    return backgroundTasks.Do();
                }
                else
                {
                    var log = new SysLogModel(context: context);
                    var html = backgroundTasks.Do();
                    log.Finish(context: context, responseSize: html.Length);
                    return html;
                }
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public string DeleteLog()
        {
            var context = new Context();
            if (Parameters.BackgroundTask.Enabled)
            {
                var backgroundTasks = new BackgroundTasks(context: context);
                if (context.QueryStrings.Bool("NoLog"))
                {
                    return backgroundTasks.DeleteLog();
                }
                else
                {
                    var log = new SysLogModel(context: context);
                    var html = backgroundTasks.DeleteLog();
                    log.Finish(context: context, responseSize: html.Length);
                    return html;
                }
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public string RebuildSearchIndexes()
        {
            var context = new Context();
            if (Parameters.BackgroundTask.Enabled)
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
                        siteId: context.QueryStrings.Long("SiteId"));
                    log.Finish(context: context, responseSize: 0);
                }
            }
            return null;
        }
    }
}