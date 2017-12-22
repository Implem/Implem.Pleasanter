using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
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

        public enum RepeatTypes : int
        {
            Daily = 10,
            Weekly = 20,
            NumberWeekly = 30,
            Monthly = 40,
            EndOfMonth = 50,
            Yearly = 60
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

        public static double DateDiff(Types interval, DateTime from, DateTime to)
        {
            if (!InRange(from, to))
            {
                return 0;
            }
            switch (interval)
            {
                case Types.Seconds:
                    return Math.Ceiling((to - from).TotalSeconds);
                case Types.Minutes:
                    return Math.Ceiling((to - from).TotalMinutes);
                case Types.Hours:
                    return Math.Ceiling((to - from).TotalHours);
                case Types.Days:
                    return Math.Ceiling((to - from).TotalDays);
                case Types.Months:
                    return to.Month - from.Month + (to.Year * 12 - from.Year * 12);
                case Types.Years:
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

        public static string PreviousMonth(DateTime month)
        {
            var data = month.ToLocal().AddMonths(-1);
            return new DateTime(data.Year, data.Month, 1).ToString();
        }

        public static string NextMonth(DateTime month)
        {
            var data = month.ToLocal().AddMonths(1);
            return new DateTime(data.Year, data.Month, 1).ToString();
        }

        public static string ThisMonth()
        {
            var data = DateTime.Now.ToLocal();
            return new DateTime(data.Year, data.Month, 1).ToString();
        }

        public static DateTime Next(this DateTime self, RepeatTypes type)
        {
            var now = DateTime.Now.ToLocal();
            var start = now > self
                ? now
                : self;
            switch (type)
            {
                case RepeatTypes.Daily:
                    return self.NextDaily(start);
                case RepeatTypes.Weekly:
                    return self.NextWeek(start);
                case RepeatTypes.NumberWeekly:
                    return self.NextNumberWeekly(start);
                case RepeatTypes.Monthly:
                    return self.NextMonthly(start);
                case RepeatTypes.EndOfMonth:
                    return self.NextEndOfMonth(start);
                case RepeatTypes.Yearly:
                    return self.NextYearly(start);
                default:
                    return Parameters.General.MaxTime;
            }
        }

        public static int DaysInMonth(this DateTime self)
        {
            return DateTime.DaysInMonth(self.Year, self.Month);
        }

        public static DateTime AddTime(this DateTime self, DateTime additional)
        {
            return self
                .AddHours(additional.Hour)
                .AddMinutes(additional.Minute)
                .AddSeconds(additional.Second)
                .AddMilliseconds(additional.Millisecond);
        }

        private static DateTime NextDaily(this DateTime self, DateTime start)
        {
            var ret = self.AddDays((DateDiff(Types.Days, self, start).ToInt()));
            return ret >= start
                ? ret
                : ret.AddDays(1);
        }

        private static DateTime NextWeek(this DateTime self, DateTime start)
        {
            var days = start.SameDayOfWeek(self.DayOfWeek)
                .Select(o => o.AddTime(self))
                .ToList();
            return (days.
                Any(o => o >= start)
                ? days.FirstOrDefault(o => o >= start)
                : days.Last().AddDays(7));
        }

        private static DateTime NextMonthly(this DateTime self, DateTime start)
        {
            var ret = self.AddMonths((DateDiff(Types.Months, self, start).ToInt()));
            return ret >= start
                ? ret
                : ret.AddMonths(1);
        }

        private static DateTime NextEndOfMonth(this DateTime self, DateTime start)
        {
            var ret = new DateTime(start.Year, start.Month, start.DaysInMonth()).AddTime(self);
            return ret >= start
                ? ret
                : ret.AddMonths(1);
        }

        private static DateTime NextNumberWeekly(this DateTime self, DateTime start)
        {
            var index = self.NumberWeek();
            var dt = new DateTime(start.Year, start.Month, 1);
            while (dt <= Parameters.General.MaxTime)
            {
                var days = SameDayOfWeek(dt, self.DayOfWeek).ToList();
                if (days.Count > index)
                {
                    var ret = days[index].AddTime(self);
                    if (ret >= start)
                    {
                        return ret;
                    }
                }
                dt = dt.AddMonths(1);
            }
            return Parameters.General.MaxTime;
        }

        private static int NumberWeek(this DateTime self)
        {
            return self
                .SameDayOfWeek()
                .Select(o => o.Day)
                .ToList()
                .IndexOf(self.Day);
        }

        private static DateTime NextYearly(this DateTime self, DateTime start)
        {
            var ret = self.AddYears((DateDiff(Types.Years, self, start).ToInt()));
            return ret >= start
                ? ret
                : ret.AddYears(1);
        }

        private static IEnumerable<DateTime> SameDayOfWeek(
            this DateTime self, DayOfWeek? dayOfWeek = null)
        {
            return Enumerable.Range(1, self.DaysInMonth())
                .Select(o => new DateTime(self.Year, self.Month, o))
                .Where(o => o.DayOfWeek == (dayOfWeek ?? self.DayOfWeek));
        }

        public static string ToViewText(this DateTime self, string format = "")
        {
            return self.InRange()
                ? self.ToString(format, Sessions.CultureInfo())
                : string.Empty;
        }
    }
}