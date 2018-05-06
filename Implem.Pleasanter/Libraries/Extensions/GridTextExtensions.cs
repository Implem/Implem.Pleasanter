using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class GridTextExtensions
    {
        public static string GridText(this IConvertable value, Column column)
        {
            return column != null && value != null
                ? value.GridText(column)
                : string.Empty;
        }

        public static string GridText(this TimeZoneInfo value, Column column)
        {
            return value.StandardName;
        }

        public static string GridText(this string value, Column column)
        {
            return column.HasChoices()
                ? column.Choice(value).TextMini
                : value;
        }

        public static string GridText(this int value, Column column)
        {
            return value.ToString(column.StringFormat) + column.Unit;
        }

        public static string GridText(this long value, Column column)
        {
            return value.ToString(column.StringFormat) + column.Unit;
        }

        public static string GridText(this decimal value, Column column)
        {
            return column.Display(value, unit: true);
        }

        public static string GridText(this DateTime value, Column column)
        {
            return column.DisplayGrid(value.ToLocal());
        }

        public static string GridText(this bool value, Column column)
        {
            return column.HasChoices()
                ? value
                    ? column.ChoicesText.SplitReturn()._1st()
                    : column.ChoicesText.SplitReturn()._2nd()
                : value.ToString();
        }

        public static string GridText(this Enum value, Column column)
        {
            return value.ToString();
        }
    }
}