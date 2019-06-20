using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    public class ReminderSchedulesController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Remind()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ReminderSchedulesController();
            var json = controller.Remind(context: context);
            return json;
        }
    }
}