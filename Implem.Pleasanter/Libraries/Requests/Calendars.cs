using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Calendars
    {
        public static DateTime BeginDate(Context context, DateTime date)
        {
            date = date.ToLocal(context: context).Date;
            date = new DateTime(date.Year, date.Month, 1);
            if ((int)date.DayOfWeek < Parameters.General.FirstDayOfWeek)
            {
                return date.AddDays(
                    ((int)date.DayOfWeek - Parameters.General.FirstDayOfWeek + 7) * -1)
                        .ToUniversal(context: context);
            }
            else
            {
                return date.AddDays(
                    ((int)date.DayOfWeek - Parameters.General.FirstDayOfWeek) * -1)
                        .ToUniversal(context: context);
            }
        }

        public static DateTime EndDate(Context context, DateTime date)
        {
            return BeginDate(
                context: context,
                date: date).AddDays(43).AddMilliseconds(-3);
        }
    }
}