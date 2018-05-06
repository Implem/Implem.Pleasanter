using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class TimeExtensions
    {
        public static DateTime AddDifferenceOfDates(
            this DateTime self, string format, bool minus = false)
        {
            return self.AddDays(DifferenceOfDates(format, minus));
        }

        public static int DifferenceOfDates(string format, bool minus = false)
        {
            switch (format)
            {
                case "Ymd": return minus ? -1 : 1;
                default: return 0;
            }
        }
    }
}