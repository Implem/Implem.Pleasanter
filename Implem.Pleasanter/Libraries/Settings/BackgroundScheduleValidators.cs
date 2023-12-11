using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Text.RegularExpressions;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class BackgroundScheduleValidators
    {
        public static ErrorData OnCreating(Context context, BackgroundSchedule schedule)
        {
            return Validator(context: context, script: schedule);
        }

        public static ErrorData OnUpdating(Context context, BackgroundSchedule schedule)
        {
            return OnCreating(
                context: context,
                schedule: schedule);
        }

        private static ErrorData Validator(Context context, BackgroundSchedule script)
        {
            var validateRegex = "^(([0-1][0-9])|20|21|22|23):[0-5][0-9]$";
            var regexDate = new Regex(validateRegex, RegexOptions.Compiled);
            if (script.ScheduleDailyTime != null && !regexDate.IsMatch(script.ScheduleDailyTime))
            {
                return new ErrorData(type: Error.Types.InvalidDateHhMmFormat);
            }
            if (script.ScheduleWeeklyTime!= null && !regexDate.IsMatch(script.ScheduleWeeklyTime))
            {
                return new ErrorData(type: Error.Types.InvalidDateHhMmFormat);
            }
            if (script.ScheduleMonthlyTime!= null && !regexDate.IsMatch(script.ScheduleMonthlyTime))
            {
                return new ErrorData(type: Error.Types.InvalidDateHhMmFormat);
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
