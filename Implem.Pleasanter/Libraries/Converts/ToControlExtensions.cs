using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
using Implem.Pleasanter.Libraries.ServerData;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToControlExtensions
    {
        public static string ToControl(this Enum self, Column column)
        {
            switch (column.TypeName)
            {
                case "int": return self.ToInt().ToString();
                case "bigint": return self.ToLong().ToString();
                default: return self.ToInt().ToString();
            }
        }

        public static string ToControl(this bool self, Column column)
        {
            return self.ToString();
        }

        public static string ToControl(this DateTime self, Column column)
        {
            return self.NotZero()
                ? self.Formatted(column)
                : string.Empty;
        }

        private static string Formatted(this DateTime self, Column column)
        {
            return column.ControlDateTime != string.Empty
                ? self.ToString(
                    Displays.Get(column.ControlDateTime + "Format"),
                    Sessions.CultureInfo())
                : self.ToString(
                    Displays.YmdahmsFormat(),
                    Sessions.CultureInfo());
        }

        public static string ToControl(this string self, Column column)
        {
            return self;
        }

        public static string ToControl(this int self, Column column)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(this long self, Column column)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(this decimal self, Column column)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl<T>(this IEnumerable<T> self, Column column)
        {
            return self?.Join() ?? string.Empty;
        }
    }
}