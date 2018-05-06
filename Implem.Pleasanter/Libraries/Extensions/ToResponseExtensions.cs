using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToResponseExtensions
    {
        public static string ToResponse(this Enum self)
        {
            return self.ToString();
        }

        public static string ToResponse(this bool self)
        {
            return self.ToString();
        }

        public static string ToResponse(this DateTime self)
        {
            return self.InRange()
                ? self.ToString()
                : string.Empty;
        }

        public static string ToResponse(this string self)
        {
            return self;
        }

        public static string ToResponse(this int self)
        {
            return self.ToString();
        }

        public static string ToResponse(this long self)
        {
            return self.ToString();
        }

        public static string ToResponse(this decimal self)
        {
            return self.ToString();
        }

        public static string ToResponse<T>(this IEnumerable<T> self)
        {
            return self.ToString();
        }
    }
}