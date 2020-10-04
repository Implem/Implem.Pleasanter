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
            var first = new DateTime(date.Year, date.Month, 1);
            switch (timePeriod)
            {
                case "Yearly":
                    return first.ToUniversal(context: context);
                case "Monthly":
                case "Weekly":
                    DateTime begin;
                    if ((int)first.DayOfWeek < Parameters.General.FirstDayOfWeek)
                    {
                        begin = first.AddDays(
                            ((int)first.DayOfWeek - Parameters.General.FirstDayOfWeek + 7) * -1);
                    }
                    else
                    {
                        begin = first.AddDays(
                            ((int)first.DayOfWeek - Parameters.General.FirstDayOfWeek) * -1);
                    }
                    if (timePeriod == "Weekly")
                    {
                        begin = begin.AddDays(((date - begin).Days / 7) *7);
                    }
                    return begin.ToUniversal(context: context);

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
                case "Weekly":
                    return BeginDate(
                        context: context,
                        date: date,
                        timePeriod: timePeriod).AddDays(8).AddMilliseconds(-3);
                default:
                    return DateTime.MinValue;
            }
        }
    }
}