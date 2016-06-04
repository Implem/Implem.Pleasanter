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
    }
}
