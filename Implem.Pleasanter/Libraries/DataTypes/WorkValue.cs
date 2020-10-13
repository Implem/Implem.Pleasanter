using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    [Serializable]
    public class WorkValue : IConvertable
    {
        public decimal Value;
        public decimal ProgressRate;

        public WorkValue()
        {
        }

        public WorkValue(DataRow dataRow, ColumnNameInfo column)
        {
            ProgressRate = dataRow.Decimal(Rds.DataColumnName(column, "ProgressRate"));
            Value = dataRow.Decimal(Rds.DataColumnName(column, "WorkValue"));
        }

        public WorkValue(decimal value, decimal progressRate)
        {
            Value = value;
            ProgressRate = progressRate;
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return column.Display(
                context: context,
                ss: ss,
                value: Value);
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return column.Display(
                context: context,
                ss: ss,
                value: Value);
        }

        public HtmlBuilder Td(HtmlBuilder hb, Context context, Column column, int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(),
                action: () => Svg(
                    hb: hb,
                    context: context,
                    column: column));
        }

        private HtmlBuilder Svg(HtmlBuilder hb, Context context, Column column)
        {
            var width = column.Max != null && column.Max != 0
                ? Convert.ToInt32(Value / column.Max.ToInt() * 100)
                : 0;
            return hb.Svg(css: "svg-work-value", action: () => hb
                .SvgText(
                    text: column.Display(
                        context: context,
                        value: Value,
                        unit: true),
                    x: 0,
                    y: Parameters.General.WorkValueTextTop)
                .Rect(
                    x: 0,
                    y: Parameters.General.WorkValueHeight / 2,
                    width: width.ToString(),
                    height: (Parameters.General.WorkValueHeight / 2).ToString())
                .Rect(
                    x: 0,
                    y: Parameters.General.WorkValueHeight / 2,
                    width: ProgressRate != 0
                        ? (width * (ProgressRate / 100)).ToString()
                        : "0",
                    height: (Parameters.General.WorkValueHeight / 2).ToString()));
        }

        public string GridText(Context context, Column column)
        {
            return column.Display(
                context: context,
                value: Value,
                unit: true);
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return Value.ToString();
        }

        public string ToNotice(
            Context context,
            decimal saved,
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
            return Value == 0;
        }
    }
}