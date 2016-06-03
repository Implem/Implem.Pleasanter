using System;
namespace Implem.Libraries.Utilities
{
    public static class DateTimes
    {
        public static bool NotZero(this DateTime self)
        {
            return self.ToOADate() != 0;
        }

        public static string Full()
        {
            return Full(DateTime.Now);
        }

        public static string Full(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }

        public static string Full_()
        {
            return Full_(DateTime.Now);
        }

        public static string Full_(DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static string YearMonthDay()
        {
            return YearMonthDay(DateTime.Now);
        }

        public static string YearMonthDay(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static string YearMonth()
        {
            return YearMonth(DateTime.Now);
        }

        public static string YearMonth(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMM");
        }

        public static string Year(DateTime dateTime)
        {
            return dateTime.ToString("yyyy");
        }
    }
}
