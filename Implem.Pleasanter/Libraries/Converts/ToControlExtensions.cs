using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
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
            this Enum self, Column column, Permissions.Types permissionType)
        {
            switch (column.TypeName)
            {
                case "int": return self.ToInt().ToString();
                case "bigint": return self.ToLong().ToString();
                default: return self.ToInt().ToString();
            }
        }

        public static string ToControl(
            this bool self, Column column, Permissions.Types permissionType)
        {
            return self.ToString();
        }

        public static string ToControl(
            this DateTime self, Column column, Permissions.Types permissionType)
        {
            return column.DisplayControl(self.ToLocal());
        }

        public static string ToControl(
            this string self, Column column, Permissions.Types permissionType)
        {
            return self;
        }

        public static string ToControl(
            this int self, Column column, Permissions.Types permissionType)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this long self, Column column, Permissions.Types permissionType)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this decimal self, Column column, Permissions.Types permissionType)
        {
            return self != 0
                ? column.Display(self, permissionType)
                : string.Empty;
        }

        public static string ToControl<T>(
            this IEnumerable<T> self, Column column, Permissions.Types permissionType)
        {
            return self?.Join() ?? string.Empty;
        }
    }
}