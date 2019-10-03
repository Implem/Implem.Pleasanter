using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class GridTextExtensions
    {
        public static string GridText(this IConvertable value, Context context, Column column)
        {
            return column != null && value != null
                ? value.ToString()
                : string.Empty;
        }

        public static string GridText(this bool value, Context context, Column column)
        {
            return column.HasChoices()
                ? value
                    ? column.ChoicesText.SplitReturn()._1st()
                    : column.ChoicesText.SplitReturn()._2nd()
                : value.ToString();
        }

        public static string GridText(this int value, Context context, Column column)
        {
            return value.ToString(column.StringFormat) + column.Unit;
        }

        public static string GridText(this long value, Context context, Column column)
        {
            return value.ToString(column.StringFormat) + column.Unit;
        }

        public static string GridText(this decimal value, Context context, Column column)
        {
            return column.Display(
                context: context,
                value: value,
                unit: true);
        }

        public static string GridText(this DateTime value, Context context, Column column)
        {
            return column.DisplayGrid(
                context: context,
                value: value.ToLocal(context: context));
        }

        public static string GridText(this string value, Context context, Column column)
        {
            return column.HasChoices()
                ? column.Choice(value).TextMini
                : value;
        }

        public static string GridText(this TimeZoneInfo value, Context context, Column column)
        {
            return value.StandardName;
        }

        public static string GridText(this Attachments value, Context context, Column column)
        {
            return value
                .Select(o => $"[{o.Name}]({Locations.Get(context, "binaries", o.Guid, "download")})")
                .Join("\n");
        }
    }
}