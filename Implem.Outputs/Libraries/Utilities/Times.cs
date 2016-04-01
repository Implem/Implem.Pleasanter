using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.ServerData;
using System;
namespace Implem.Pleasanter.Libraries.Utilities
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

        public static DateTime ToUniversal(this DateTime value)
        {
            var timeZoneInfo = Sessions.TimeZoneInfo();
            return timeZoneInfo != null && timeZoneInfo.Id != TimeZoneInfo.Local.Id
                ? TimeZoneInfo.ConvertTime(value, timeZoneInfo, TimeZoneInfo.Local)
                : value;
        }

        public static int DateDiff(Types interval, DateTime date1, DateTime date2)
        {
            if (date2 <= Parameters.MinTime)
            {
                return 0;
            }
            switch (interval)
            {
                case Types.Seconds:
                    return Math.Ceiling((date2 - date1).TotalSeconds).ToInt();
                case Types.Minutes:
                    return Math.Ceiling((date2 - date1).TotalMinutes).ToInt();
                case Types.Hours:
                    return Math.Ceiling((date2 - date1).TotalHours).ToInt();
                case Types.Days:
                    return Math.Ceiling((date2 - date1).TotalDays).ToInt();
                case Types.Months:
                    return date2.Month - date1.Month + (date2.Year * 12 - date1.Year * 12);
                case Types.Years:
                    return date2.Year - date1.Year;
                default:
                    return 0;
            }
        }
    }
}