using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class InitialValueExtensions
    {
        public static bool InitialValue(this bool self, Context context)
        {
            return self == false;
        }

        public static bool InitialValue(this Enum self, Context context)
        {
            return self == default(Enum);
        }

        public static bool InitialValue(this int self, Context context)
        {
            return self == 0;
        }

        public static bool InitialValue(this long self, Context context)
        {
            return self == 0;
        }

        public static bool InitialValue(this double self, Context context)
        {
            return self == 0;
        }

        public static bool InitialValue(this decimal self, Context context)
        {
            return self == 0;
        }

        public static bool InitialValue(this DateTime self, Context context)
        {
            return self == 0.ToDateTime();
        }

        public static bool InitialValue(this string self, Context context)
        {
            return self.IsNullOrEmpty();
        }

        public static bool InitialValue(this byte[] self, Context context)
        {
            return self == null;
        }

        public static bool InitialValue<T>(this List<T> self, Context context)
        {
            return self?.Any() != true;
        }

        public static bool InitialValue(this System.Net.Mail.MailAddress self, Context context)
        {
            return self.Address.IsNullOrEmpty();
        }
    }
}