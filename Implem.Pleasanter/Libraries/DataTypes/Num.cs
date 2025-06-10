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
using System.Linq;
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
                if (value.IsNullOrEmpty())
                {
                    return;
                }
                if (!decimal.TryParse(
                    new string(value
                        .SkipWhile(c => c == (char)92 || c == (char)165)
                        .ToArray()),
                    NumberStyles.Any,
                    context.CultureInfo(),
                    out _))
                {
                    return;
                }
            }
            Value = column?.Round(value: value.ToDecimal(cultureInfo: context.CultureInfo()))
                ?? 0;
        }

        public string ToControl(
            Context context,
            SiteSettings ss,
            Column column)
        {
            return ControlValue(
                context: context,
                ss: ss,
                column: column);
        }

        public string ToResponse(
            Context context,
            SiteSettings ss,
            Column column)
        {
            return ControlValue(
                context: context,
                ss: ss,
                column: column);
        }

        public string ToDisplay(
            Context context,
            SiteSettings ss,
            Column column)
        {
            return column.Display(
                context: context,
                value: Value,
                unit: true);
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return column.DecimalPlaces.ToInt() == 0
                        ? Value.ToDecimal().ToString("0", "0")
                        : column.DisplayValue(Value.ToDecimal());
            }
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Text(text: column.Display(
                        context: context,
                        value: Value,
                        unit: true)));
        }

        public string GridText(Context context, Column column)
        {
            return column.Display(
                context: context,
                value: Value,
                unit: true);
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return Value;
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Value;
        }

        public string ToExport(
            Context context,
            Column column,
            ExportColumn exportColumn = null)
        {
            return column.Display(
                context: context,
                value: Value,
                format: false);
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

        public bool InitialValue(Context context)
        {
            return Value == null;
        }

        private string ControlValue(
            Context context,
            SiteSettings ss,
            Column column)
        {
            return column.ControlType == "Spinner"
                ? column.Display(
                    context: context,
                    value: Value,
                    format: false)
                : column.Display(
                    context: context,
                    value: Value,
                    format: column.Format == "C" || column.Format == "N" || column.EditorReadOnly == true);
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
    }
}