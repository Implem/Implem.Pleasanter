using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToExportExtensions
    {
        public static string ToExport(
            this TimeZoneInfo value, Context context, Column column, ExportColumn exportColumn = null)
        {
            return value?.StandardName;
        }

        public static string ToExport(
            this string value, Context context, Column column, ExportColumn exportColumn = null)
        {
            if (column.HasChoices())
            {
                var choiceParts = column.ChoiceParts(
                    context: context,
                    selectedValues: value,
                    type: exportColumn?.Type ?? ExportColumn.Types.Text);
                return !exportColumn?.ChoiceValue.IsNullOrEmpty() == true
                    ? column.MultipleSelections == true
                        ? value.Deserialize<List<string>>()?.Contains(exportColumn.ChoiceValue) == true
                            ? "1"
                            : string.Empty
                        : value == exportColumn.ChoiceValue
                            ? "1"
                            : string.Empty
                    : column.MultipleSelections == true
                        ? choiceParts.ToJson()
                        : choiceParts.FirstOrDefault();
            }
            else
            {
                return value;
            }
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