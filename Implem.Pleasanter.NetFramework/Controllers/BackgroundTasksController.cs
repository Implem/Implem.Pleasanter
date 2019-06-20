using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Tools;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    public class BackgroundTasksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Do()
        {
            var context = new ContextImplement();

            var controller = new Implem.Pleasanter.Controllers.BackgroundTasksController();
            var html = controller.Do(context: context,backgroundTasks: new BackgroundTasks(context: context));
            return html;
        }
    }
}