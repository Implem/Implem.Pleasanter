using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Times
    {
        public enum Types
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Months,
            Years
        }

        public static DateTime ToLocal(this DateTime value)
        {
            var timeZoneInfo = Sessions.TimeZoneInfo();
            return timeZoneInfo != null && timeZoneInfo.Id != TimeZoneInfo.Local.Id
                ? TimeZoneInfo.ConvertTime(value, timeZoneInfo)
                : value;
        }

        public static string ToLocal(this DateTime value, string format)
        {
            return value.ToLocal().ToString(format, Sessions.CultureInfo());
        }

        public static DateTime ToUniversal(this DateTime value)
        {
            var timeZoneInfo = Sessions.TimeZoneInfo();
            return timeZoneInfo != null && timeZoneInfo.Id != TimeZoneInfo.Local.Id
                ? TimeZoneInfo.ConvertTime(value, timeZoneInfo, TimeZoneInfo.Local)
                : value;
        }

        public static int DateDiff(Times.Types interval, DateTime from, DateTime to)
        {
            if (!InRange(from, to))
            {
                return 0;
            }
            switch (interval)
            {
                case Times.Types.Seconds:
                    return Math.Ceiling((to - from).TotalSeconds).ToInt();
                case Times.Types.Minutes:
                    return Math.Ceiling((to - from).TotalMinutes).ToInt();
                case Times.Types.Hours:
                    return Math.Ceiling((to - from).TotalHours).ToInt();
                case Times.Types.Days:
                    return Math.Ceiling((to - from).TotalDays).ToInt();
                case Times.Types.Months:
                    return to.Month - from.Month + (to.Year * 12 - from.Year * 12);
                case Times.Types.Years:
                    return to.Year - from.Year;
                default:
                    return 0;
            }
        }

        public static bool InRange(params DateTime[] times)
        {
            return !times.ToList().Any(o =>
                o < Parameters.General.MinTime ||
                o > Parameters.General.MaxTime);
        }
    }
}