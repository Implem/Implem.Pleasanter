using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class BackgroundTasksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Do()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BackgroundTasksController();
            var html = controller.Do(context: context, backgroundTasks: new BackgroundTasks(context: context));
            return html;
        }

        [AllowAnonymous]
        [HttpGet]
        public string RebuildSearchIndexes()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BackgroundTasksController();
            var html = controller.RebuildSearchIndexes(context: context);
            return html;
        }
    }
}