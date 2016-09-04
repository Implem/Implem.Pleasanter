using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToExportExtensions
    {
        public static string ToExport(this string value, Column column)
        {
            return column.HasChoices()
                ? column.Choice(value).Text()
                : value;
        }

        public static string ToExport(this int value, Column column)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(this long value, Column column)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(this decimal value, Column column)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(this DateTime value, Column column)
        {
            return value.InRange()
                ? column.DisplayExport(value.ToLocal())
                : string.Empty;
        }

        public static string ToExport(this bool value, Column column)
        {
            return value
                ? "1"
                : string.Empty;
        }

        public static string ToExport(this Enum value, Column column)
        {
            return value.ToString();
        }
    }
}