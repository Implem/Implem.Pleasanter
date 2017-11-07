using Implem.DefinitionAccessor;
using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [RefleshSiteInfo]
    public class ReminderSchedulesController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public string Remind()
        {
            if (Parameters.Reminder.Enabled)
            {
                if (QueryStrings.Bool("NoLog"))
                {
                    return ReminderScheduleUtilities.Remind();
                }
                else
                {
                    var log = new SysLogModel();
                    var json = ReminderScheduleUtilities.Remind();
                    log.Finish(json.Length);
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