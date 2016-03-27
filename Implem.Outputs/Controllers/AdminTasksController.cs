using Implem.Pleasanter.Libraries.Admins;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class AdminTasksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Do()
        {
            var log = new SysLogModel();
            var html = AdminTasks.Do();
            log.Finish(html.Length);
            return html;
        }
    }
}