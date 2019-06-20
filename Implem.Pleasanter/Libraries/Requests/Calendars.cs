using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Calendars
    {
        public static DateTime BeginDate(Context context, DateTime date, string timePeriod)
        {
            date = date.ToLocal(context: context).Date;
            date = new DateTime(date.Year, date.Month, 1);
            switch (timePeriod)
            {
                case "Yearly":
                    return date.ToUniversal(context: context);
                case "Monthly":
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
                default:
                    return DateTime.MinValue;
            }
        }

        public static DateTime EndDate(Context context, DateTime date, string timePeriod)
        {
            switch (timePeriod)
            {
                case "Yearly":
                    return BeginDate(
                        context: context,
                        date: date,
                        timePeriod: timePeriod).AddYears(1).AddMilliseconds(-3);
                case "Monthly":
                    return BeginDate(
                        context: context,
                        date: date,
                        timePeriod: timePeriod).AddDays(43).AddMilliseconds(-3);
                default:
                    return DateTime.MinValue;
            }
        }
    }
}