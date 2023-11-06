using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Drawing;

namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Calendars
    {
        public static DateTime BeginDate(
            Context context,
            SiteSettings ss,
            DateTime date,
            string timePeriod,
            View view)
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
                    string calendarType = view.CalendarType.ToString();
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
                    if (calendarType == "FullCalendar") {
                        begin = !string.IsNullOrEmpty(view.CalendarStart.ToString())
                            ? (DateTime)view.CalendarStart
                            : begin;
                    }
                    if(ss.DashboardParts.Count != 0)
                    {
                        if (!context.Forms.Data($"CalendarStart_{ss.DashboardParts[0].Id}").IsNullOrEmpty())
                        {
                            begin = (DateTime)view.CalendarStart;
                        }
                    }
                    return begin.ToUniversal(context: context);
                default:
                    return DateTime.MinValue;
            }
        }

        public static DateTime EndDate(
            Context context,
            SiteSettings ss,
            DateTime date,
            string timePeriod,
            View view)
        {
            string calendarType = view.CalendarType.ToString();
            if (calendarType == "FullCalendar")
            {
                if (ss.DashboardParts.Count != 0)
                {
                    if (!context.Forms.Data($"CalendarEnd_{ss.DashboardParts[0].Id}").IsNullOrEmpty())
                    {
                        return context.Forms.DateTime($"CalendarEnd_{ss.DashboardParts[0].Id}");
                    }
                }
                return !string.IsNullOrEmpty(view.CalendarEnd.ToString())
                    ? (DateTime)view.CalendarEnd
                    : BeginDate(
                            context: context,
                            ss: ss,
                            date: date,
                            timePeriod: timePeriod,
                            view: view).AddDays(43).AddMilliseconds(-3);
            }
            else
            {
                switch (timePeriod)
                {
                    case "Yearly":
                        return BeginDate(
                            context: context,
                            ss: ss,
                            date: date,
                            timePeriod: timePeriod,
                            view: view).AddYears(1).AddMilliseconds(-3);
                    case "Monthly":
                        return BeginDate(
                            context: context,
                            ss: ss,
                            date: date,
                            timePeriod: timePeriod,
                            view: view).AddDays(43).AddMilliseconds(-3);
                    case "Weekly":
                        return BeginDate(
                            context: context,
                            ss: ss,
                            date: date,
                            timePeriod: timePeriod,
                            view: view).AddDays(8).AddMilliseconds(-3);
                    default:
                        return DateTime.MinValue;
                }
            }
        }
    }
}