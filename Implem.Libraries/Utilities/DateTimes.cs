using System;
namespace Implem.Libraries.Utilities
{
    public static class DateTimes
    {
        public static int FirstDayOfWeek;
        public static int FirstMonth;
        public static DateTime MinTime;
        public static DateTime MaxTime;

        public static bool InRange(this DateTime self)
        {
            return self >= MinTime && self <= MaxTime;
        }

        public static string Full()
        {
            return Full(DateTime.Now);
        }

        public static string Full(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static int Quarter(this DateTime self)
        {
            return self.Month < FirstMonth
                ? (self.Month - FirstMonth + 13) / 3 + 1
                : (self.Month - FirstMonth + 1) / 3 + 1;
        }

        public static int Half(this DateTime self)
        {
            return self.Month < FirstMonth
                ? (self.Month - FirstMonth + 13) / 6
                : (self.Month - FirstMonth + 1) / 6;
        }

        public static int Fy(this DateTime self)
        {
            return self.Month < FirstMonth
                ? self.Year - 1
                : self.Year;
        }

        public static int Years(this TimeSpan self)
        {
            return (int)(self.Days / 365.2425);
        }

        public static int Months(this TimeSpan self)
        {
            return (int)(self.Days / 30.436875);
        }

        public static DateTime WeekFrom(this DateTime self)
        {
            var diff = self.DayOfWeek.ToInt() - FirstDayOfWeek;
            return self.AddDays(diff >= 0
                ? diff * -1
                : 7 - diff);
        }

        public static DateTime MonthFrom(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, 1);
        }

        public static DateTime QuarterFrom(this DateTime self)
        {
            var diff = self.Month % 3 - FirstMonth % 3;
            diff = diff < 0
                ? (3 + diff) * -1
                : diff * -1;
            var d = self.AddMonths(diff);
            return new DateTime(d.Year, d.Month, 1);
        }

        public static DateTime HalfFrom(this DateTime self)
        {
            var diff = self.Month % 6 - FirstMonth % 6;
            diff = diff < 0
                ? (6 + diff) * -1
                : diff * -1;
            var d = self.AddMonths(diff);
            return new DateTime(d.Year, d.Month, 1);
        }

        public static DateTime FyFrom(this DateTime self)
        {
            return self.Month < FirstMonth
                ? new DateTime(self.AddYears(-1).Year, FirstMonth, 1)
                : new DateTime(self.Year, FirstMonth, 1);
        }
    }
}
