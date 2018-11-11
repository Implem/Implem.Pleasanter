using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class ReminderSchedulesController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Remind()
        {
            var context = new Context();
            if (Parameters.Reminder.Enabled)
            {
                if (context.QueryStrings.Bool("NoLog"))
                {
                    return ReminderScheduleUtilities.Remind(context: context);
                }
                else
                {
                    var log = new SysLogModel(context: context);
                    var json = ReminderScheduleUtilities.Remind(context: context);
                    log.Finish(context: context, responseSize: json.Length);
                    return json;
                }
            }
            else
            {
                return null;
            }
        }
    }
}