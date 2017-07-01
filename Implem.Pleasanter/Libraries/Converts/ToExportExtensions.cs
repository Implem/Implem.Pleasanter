using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToExportExtensions
    {
        public static string ToExport(
            this string value, Column column, ExportColumn exportColumn)
        {
            return column.HasChoices()
                ? column.ChoicePart(value, exportColumn.Type)
                : value;
        }

        public static string ToExport(
            this int value, Column column, ExportColumn exportColumn)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this long value, Column column, ExportColumn exportColumn)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this decimal value, Column column, ExportColumn exportColumn)
        {
            return value.ToString(column.StringFormat);
        }

        public static string ToExport(
            this DateTime value, Column column, ExportColumn exportColumn)
        {
            return value.InRange()
                ? value.ToLocal().Display(exportColumn.Format)
                : string.Empty;
        }

        public static string ToExport(
            this bool value, Column column, ExportColumn exportColumn)
        {
            return value
                ? "1"
                : string.Empty;
        }

        public static string ToExport(
            this Enum value, Column column, ExportColumn exportColumn)
        {
            return value.ToString();
        }
    }
}