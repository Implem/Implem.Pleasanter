using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToExportExtensions
    {
        public static string ToExport(
            this TimeZoneInfo value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value.StandardName;
        }

        public static string ToExport(
            this string value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return column.HasChoices()
                ? column.ChoicePart(
                    context: context,
                    selectedValue: value,
                    type: exportColumn?.Type ?? ExportColumn.Types.Text)
                : value;
        }

        public static string ToExport(
            this int value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this long value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this decimal value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return column.Display(
                context: context,
                value: value,
                format: false);
        }

        public static string ToExport(
            this DateTime value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value.InRange()
                ? value.ToLocal(context: context).Display(
                    context: context,
                    format: exportColumn?.Format
                        ?? column?.EditorFormat
                        ?? "Ymd")
                : string.Empty;
        }

        public static string ToExport(
            this bool value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value
                ? "1"
                : string.Empty;
        }

        public static string ToExport(
            this Enum value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString();
        }
    }
}