using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Calendars
    {
        public static DateTime BeginDate(DateTime date)
        {
            date = date.ToLocal().Date;
            date = new DateTime(date.Year, date.Month, 1);
            if ((int)date.DayOfWeek < Parameters.General.FirstDayOfWeek)
            {
                return date.AddDays(
                    ((int)date.DayOfWeek - Parameters.General.FirstDayOfWeek + 7) * -1)
                        .ToUniversal();
            }
            else
            {
                return date.AddDays(
                    ((int)date.DayOfWeek - Parameters.General.FirstDayOfWeek) * -1)
                        .ToUniversal();
            }
        }

        public static DateTime EndDate(DateTime date)
        {
            return BeginDate(date).AddDays(43).AddMilliseconds(-3);
        }
    }
}