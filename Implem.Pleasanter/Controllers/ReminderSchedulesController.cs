using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class ReminderSchedulesController
    {
        public string Remind(Context context)
        {
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