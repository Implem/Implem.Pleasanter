using Implem.Pleasanter.Libraries.Admins;
using Implem.Pleasanter.Libraries.Requests;
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
            if (QueryStrings.Bool("NoLog"))
            {
                return AdminTasks.Do();
            }
            else
            {
                var log = new SysLogModel();
                var html = AdminTasks.Do();
                log.Finish(html.Length);
                return html;
            }
        }
    }
}