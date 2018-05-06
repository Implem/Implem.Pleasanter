using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToExportExtensions
    {
        public static string ToExport(
            this string value, Column column, ExportColumn exportColumn = null)
        {
            return column.HasChoices()
                ? column.ChoicePart(value, exportColumn?.Type ?? ExportColumn.Types.Text)
                : value;
        }

        public static string ToExport(
            this int value, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this long value, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this decimal value, Column column, ExportColumn exportColumn = null)
        {
            return column.Display(value, format: false);
        }

        public static string ToExport(
            this DateTime value, Column column, ExportColumn exportColumn = null)
        {
            return value.InRange()
                ? value.ToLocal().Display(
                    exportColumn?.Format ??
                    column?.EditorFormat ??
                    "Ymd")
                : string.Empty;
        }

        public static string ToExport(
            this bool value, Column column, ExportColumn exportColumn = null)
        {
            return value
                ? "1"
                : string.Empty;
        }

        public static string ToExport(
            this Enum value, Column column, ExportColumn exportColumn = null)
        {
            return value.ToString();
        }
    }
}