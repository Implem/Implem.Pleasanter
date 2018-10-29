using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.Tools;
using System.Web.Mvc;
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
                if (QueryStrings.Bool("NoLog"))
                {
                    return BackgroundTasks.Do(context);
                }
                else
                {
                    var log = new SysLogModel(context: context);
                    var html = BackgroundTasks.Do(context: context);
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