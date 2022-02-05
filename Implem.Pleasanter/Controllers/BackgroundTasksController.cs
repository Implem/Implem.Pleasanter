using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Search;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.Tools;

namespace Implem.Pleasanter.Controllers
{
    public class BackgroundTasksController
    {
        public string Do(Context context, IBackgroundTasks backgroundTasks)
        {
            if (Parameters.BackgroundTask.Enabled)
            {
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

        public string DeleteLog(Context context, IBackgroundTasks backgroundTasks)
        {
            if (Parameters.BackgroundTask.Enabled)
            {
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

        public string RebuildSearchIndexes(Context context)
        {
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