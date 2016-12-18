using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.Tools;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [RefleshSiteInfo]
    public class BackgroundTasksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Do()
        {
            if (QueryStrings.Bool("NoLog"))
            {
                return BackgroundTasks.Do();
            }
            else
            {
                var log = new SysLogModel();
                var html = BackgroundTasks.Do();
                log.Finish(html.Length);
                return html;
            }
        }
    }
}