using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class InitialValueExtensions
    {
        public static bool InitialValue(this bool self)
        {
            return self == false;
        }

        public static bool InitialValue(this Enum self)
        {
            return self == default(Enum);
        }

        public static bool InitialValue(this int self)
        {
            return self == 0;
        }

        public static bool InitialValue(this long self)
        {
            return self == 0;
        }

        public static bool InitialValue(this double self)
        {
            return self == 0;
        }

        public static bool InitialValue(this decimal self)
        {
            return self == 0;
        }

        public static bool InitialValue(this DateTime self)
        {
            return self == 0.ToDateTime();
        }

        public static bool InitialValue(this string self)
        {
            return self.IsNullOrEmpty();
        }

        public static bool InitialValue(this byte[] self)
        {
            return self == null;
        }

        public static bool InitialValue<T>(this List<T> self)
        {
            return self?.Any() != true;
        }

        public static bool InitialValue(this System.Net.Mail.MailAddress self)
        {
            return self.Address.IsNullOrEmpty();
        }
    }
}