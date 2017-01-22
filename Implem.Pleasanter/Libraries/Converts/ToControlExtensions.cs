using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToControlExtensions
    {
        public static string ToControl(
            this Enum self, Column column, Permissions.Types pt)
        {
            switch (column.TypeName)
            {
                case "int": return self.ToInt().ToString();
                case "bigint": return self.ToLong().ToString();
                default: return self.ToInt().ToString();
            }
        }

        public static string ToControl(
            this bool self, Column column, Permissions.Types pt)
        {
            return self.ToString();
        }

        public static string ToControl(
            this DateTime self, Column column, Permissions.Types pt)
        {
            return column.DisplayControl(self.ToLocal());
        }

        public static string ToControl(
            this string self, Column column, Permissions.Types pt)
        {
            return self;
        }

        public static string ToControl(
            this int self, Column column, Permissions.Types pt)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this long self, Column column, Permissions.Types pt)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this decimal self, Column column, Permissions.Types pt)
        {
            return column.ControlType == "Spinner"
                ? column.Display(self, format: false)
                : self != 0
                    ? column.Display(self, pt)
                    : string.Empty;
        }

        public static string ToControl<T>(
            this IEnumerable<T> self, Column column, Permissions.Types pt)
        {
            return self?.Join() ?? string.Empty;
        }
    }
}