using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToResponseExtensions
    {
        public static string ToResponse(this Enum self, Context context)
        {
            return self.ToString();
        }

        public static string ToResponse(this bool self, Context context)
        {
            return self.ToString();
        }

        public static string ToResponse(this DateTime self, Context context)
        {
            return self.InRange()
                ? self.ToString()
                : string.Empty;
        }

        public static string ToResponse(this string self, Context context)
        {
            return self;
        }

        public static string ToResponse(this int self, Context context)
        {
            return self.ToString();
        }

        public static string ToResponse(this long self, Context context)
        {
            return self.ToString();
        }

        public static string ToResponse(this decimal self, Context context)
        {
            return self.ToString();
        }

        public static string ToResponse<T>(this IEnumerable<T> self, Context context)
        {
            return self.ToString();
        }
    }
}