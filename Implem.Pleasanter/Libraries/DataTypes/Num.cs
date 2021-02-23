using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Globalization;
using System.Text;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class Num : IConvertable
    {
        public decimal? Value;

        public Num()
        {
        }

        public Num(decimal? value)
        {
            Value = value;
        }

        public Num(DataRow dataRow, string name)
        {
            if (dataRow[name] != DBNull.Value)
            {
                Value = dataRow.Field<decimal>(name);
            }
        }

        public Num(Context context, Column column, string value)
        {
            if (column?.Nullable == true)
            {
                if (value.IsNullOrEmpty()) return;
                decimal decimalValue;
                if (!decimal.TryParse(
                    value,
                    NumberStyles.Any,
                    context.CultureInfo(),
                    out decimalValue)) return;
            }
            Value = column?.Round(value.ToDecimal(
                cultureInfo: context.CultureInfo()))
                    ?? 0;
        }

        public bool InitialValue(Context context)
        {
            return Value == null;
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptValues)
        {
            return hb.Td(
                css: column.CellCss(serverScriptValues?.ExtendedCellCss),
                action: () => hb
                    .Text(text: column.Display(
                        context: context,
                        value: Value,
                        unit: true)));
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return ControlValue(
                context: context,
                column: column);
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return column.Display(
                context: context,
                value: Value,
                format: false);
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return ControlValue(
                context: context,
                column: column);
        }

        private string ControlValue(Context context, Column column)
        {
            return column.ControlType == "Spinner"
                ? column.Display(
                    context: context,
                    value: Value,
                    format: false)
                : column.Display(
                    context: context,
                    value: Value,
                    format: column.Format == "C" || column.Format == "N")
                        + (column.EditorReadOnly == true
                            || column.ColumnPermissionType(
                                context: context,
                                baseModel: null) != Permissions.ColumnPermissionTypes.Update
                                    ? column.Unit
                                    : string.Empty);
        }

        public void FullText(
            Context context,
            Column column,
            StringBuilder fullText)
        {
            switch (column?.FullTextType)
            {
                case Column.FullTextTypes.DisplayName:
                case Column.FullTextTypes.Value:
                case Column.FullTextTypes.ValueAndDisplayName:
                    fullText
                        .Append(" ")
                        .Append(Value?.ToString());
                    break;
            }
        }

        public string ToNotice(
            Context context,
            decimal? saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.Display(
                    context: context,
                    value: Value,
                    unit: true),
                saved: column.Display(
                    context: context,
                    value: saved,
                    unit: true),
                column: column,
                updated: updated,
                update: update);
        }
    }
}