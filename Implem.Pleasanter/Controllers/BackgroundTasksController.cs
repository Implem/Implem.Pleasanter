using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
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
    }
}