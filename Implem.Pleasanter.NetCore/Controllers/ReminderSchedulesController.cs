using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers
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